using Application.Types;
using LogParser.Data;
using LogParser.Entities;
using Microsoft.Data.Sqlite;
using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application
{
    internal class Program
    {
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
            var fileOption = new Option<FileInfo>(name: "-file", ["-f"])
            {
                Required = true,
                Description = "Path to log file",
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

            rootCommand.SetAction(async (result) =>
            {
                var file = result.GetValue(fileOption)!;
                var interval = result.GetValue(intervalOption);
                var bufferSize = result.GetValue(bufferOption);
                InitializeBuffer(bufferSize);
                ReadLogfile(file, interval);
            });

            return rootCommand.Parse(args).Invoke();
        }

        static void ReadLogfile(FileInfo fileInfo, int interval)
        {
            Console.WriteLine($"Reading file {fileInfo.FullName} with interval: {interval} ms");
            if (!fileInfo.Exists)
            {
                Console.WriteLine($"File {fileInfo.FullName} doesn't exists");
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
            Db.Dispose();
            _ = Task.Run(Output);

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

        static async Task Output()
        {
            //int count = 0;
            //while (true)
            //{
            //    if (count != Logs.Count)
            //    {
            //        count = Logs.Count;
            //        Console.WriteLine($"Current count {Logs.Count}");
            //    }
            //    await Task.Delay(1000);
            //}
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
