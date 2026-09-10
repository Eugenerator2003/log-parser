using Application.Types;
using LogParser.Data;
using LogParser.Entities;
using Microsoft.Data.Sqlite;
using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Application
{
    internal class Program
    {
        private enum ProgramMode
        {
            Server,
            Parser,
            Invalid
        }

        static readonly string DateFormat = "dd/MMM/yyyy:HH:mm:ss zzz";
        static readonly Regex GroupRegex = new Regex(@"^(?<ip>\S+)\s+-\s+\[?(?<user>[^\]]*)\]?\s+\[(?<datetime>[^\]]*)\]\s+""(?<request>[^""]*)""\s+(?<status>\d+)\s+(?<bytes>\d+)\s+""(?<referer>[^""]*)""\s+""(?<agent>[^""]*)""");

        static NginxBaseLogDatabase Db = null!;
        static readonly ReferenceValueDictionary<RemoteAddress, string> Addresses = new ReferenceValueDictionary<RemoteAddress, string>((addr) => addr.Address);
        static readonly ReferenceValueDictionary<RemoteUser, string> Users = new ReferenceValueDictionary<RemoteUser, string>((usr) => usr.Name);
        static readonly ReferenceValueDictionary<UserAgent, string> Agents = new ReferenceValueDictionary<UserAgent, string>((agent) => agent.Name);

        static int bufferPosition = 0;
        static Log[] ReadingBuffer = null!;

        static int Main(string[] args)
        {
            var fileOption = new Option<FileInfo>(name: "--file", ["-f"])
            {
                Required = true,
                Description = "Path to log file",
            };
            var modeOption = new Option<ProgramMode>(name: "--mode", ["-m"])
            {
                Required = false,
                Description = "Program mode: server (\"server\") or parse (\"parse\") only. Defaults to server",
                DefaultValueFactory = (args) => ProgramMode.Server,
                CustomParser = (result) =>
                {
                    var str = result.Tokens.Single().Value;
                    if (str == "server")
                    {
                        return ProgramMode.Server;
                    }
                    else if (str == "parse")
                    {
                        return ProgramMode.Parser;
                    }
                    else
                    {
                        return ProgramMode.Invalid;
                    }
                }
            };
            var addressOption = new Option<string>(name: "--address", ["-a"])
            {
                Required = false,
                Description = "Server address. Applied only for server mode. Defaults to 127.0.0.1",
                DefaultValueFactory = (args) => "127.0.0.1"
            };
            var portOption = new Option<int>(name: "--port", ["-p"])
            {
                Required = false,
                Description = "Server port. Applied only for server mode. Defaults to 5000",
                DefaultValueFactory = (args) => 5000
            };
            var intervalOption = new Option<int>(name: "--interval", ["-i"])
            {
                Required = false,
                DefaultValueFactory = (arg) => 3000,
                Description = "Interval for checking new lines (in milliseconds)"
            };
            var bufferOption = new Option<int>(name: "--buffer", ["-b"])
            {
                Required = false,
                DefaultValueFactory = (arg) => 100000,
                Description = "Parsing buffer length",
            };
            var rootCommand = new RootCommand("Real-time log reading utility");
            rootCommand.Add(fileOption);
            rootCommand.Add(intervalOption);
            rootCommand.Add(bufferOption);
            rootCommand.Add(addressOption);
            rootCommand.Add(portOption);

            rootCommand.SetAction(async (result) =>
            {
                var file = result.GetValue(fileOption)!;
                var interval = result.GetValue(intervalOption);
                var bufferSize = result.GetValue(bufferOption);
                var programMode = result.GetValue(modeOption);
                var address = result.GetValue(addressOption)!;
                var port = result.GetValue(portOption);
                if (programMode == ProgramMode.Invalid)
                {
                    Console.WriteLine("Invalid mode option. Requires \"server\" or \"parse\"");
                    return;
                }
                InitializeBuffer(bufferSize);
                if (programMode == ProgramMode.Parser)
                {
                    ReadLogfile(file, interval);
                }
                else
                {
                    _ = Task.Run(() => ReadLogfile(file, interval));
                    StartWebServer(address, port);
                }
            });

            return rootCommand.Parse(args).Invoke();
        }

        static void StartWebServer(string address, int port)
        {
            if (!IPAddress.TryParse(address, out var ip))
            {
                Console.WriteLine($"Invalid ip address");
                return;
            }
            Console.WriteLine($"Starting web application on address \"{address}\" with {port} port");
            var builder = WebApplication.CreateBuilder([]);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(ip, port);
            });
            var app = builder.Build();
            app.UseStaticFiles();
            app.MapGet("/", () => Results.Redirect("/index.html"));
            app.MapGet("/api/query", (string query) => ExecuteRawSql(query));
            app.Run();
        }

        static IResult ExecuteRawSql(string query)
        {
            try
            {
                var result = Db.ExecuteQuery(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        static void ReadLogfile(FileInfo fileInfo, int interval)
        {
            Console.WriteLine($"Reading file {fileInfo.FullName} with interval: {interval} ms");
            if (!fileInfo.Exists)
            {
                Console.WriteLine($"File {fileInfo.FullName} doesn't exists");
                return;
            }
            Addresses.Clear();
            Users.Clear();
            Agents.Clear();
            InitializeDatabase();
            using var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                ReadLineToBuffer(line);
            }
            InsertBufferToDatabase();
            while (true)
            {
                bool isRead = false;
                while ((line = reader.ReadLine()) != null)
                {
                    isRead = true;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    ReadLineToBuffer(line);
                }
                if (!isRead)
                {
                    reader.DiscardBufferedData();
                }
                Task.Delay(interval).Wait();
            }
        }

        static void ReadLineToBuffer(string line)
        {
            var parsedLog = ParseLine(line);
            var address = Addresses.Get(parsedLog.RemoteIp);
            if (address is null)
            {
                address = new RemoteAddress(0, parsedLog.RemoteIp);
                Db.AddRemoteAddress(address);
                Addresses.Add(address);
            }
            var user = Users.Get(parsedLog.RemoteUser);
            if (user is null)
            {
                user = new RemoteUser(0, parsedLog.RemoteUser);
                Db.AddRemoteUser(user);
                Users.Add(user);
            }
            var agent = Agents.Get(parsedLog.UserAgent);
            if (agent is null)
            {
                agent = new UserAgent(0, parsedLog.UserAgent);
                Db.AddUserAgent(agent);
                Agents.Add(agent);
            }
            var log = new Log(
                0,
                remoteAddress: address,
                remoteUser: user,
                statusCode: parsedLog.Status,
                userAgent: agent,
                request: parsedLog.Request,
                dateTimeOffset: parsedLog.TimeLocal,
                bytesCount: parsedLog.BodyBytesSent,
                httpReferer: parsedLog.HttpReferer
                );
            ReadingBuffer[bufferPosition++] = log;
            if (bufferPosition == ReadingBuffer.Length)
            {
                InsertBufferToDatabase();
            }
        }

        static NginxBaseLog ParseLine(string line)
        {
            var match = GroupRegex.Match(line);
            if (!match.Success)
            {
                throw new InvalidDataException($"Failed to match line: {line}");
            }
            return new NginxBaseLog(
                remoteIp: match.Groups["ip"].Value,
                remoteUser: match.Groups["user"].Value,
                timeLocal: DateTimeOffset.ParseExact(match.Groups["datetime"].Value, DateFormat, CultureInfo.InvariantCulture),
                request: match.Groups["request"].Value,
                status: int.Parse(match.Groups["status"].Value),
                bodyBytesSent: int.Parse(match.Groups["bytes"].Value),
                httpReferer: match.Groups["referer"].Value,
                userAgent: match.Groups["agent"].Value
                );
        }

        static void InitializeBuffer(int bufferSize)
        {
            ReadingBuffer = new Log[bufferSize];
        }

        static void InitializeDatabase()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "log.db");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Db = new NginxBaseLogDatabase(@$"Data Source={path};Pooling=false;");
        }

        static void InsertBufferToDatabase()
        {
            if (bufferPosition != 0)
            {
                ArraySegment<Log> logs = new ArraySegment<Log>(ReadingBuffer, 0, bufferPosition);
                Db.AddLogsBatch(logs);
                bufferPosition = 0;
            }
        }
    }
}
