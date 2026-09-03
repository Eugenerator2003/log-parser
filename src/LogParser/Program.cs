using Application.Types;
using System.CommandLine;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application
{
    internal class Program
    {
        static List<NginxBaseLog> Logs { get; } = new List<NginxBaseLog>();
        static Regex GroupRegex => new Regex(@"^(?<ip>\S+)\s+-\s+\[?(?<user>[^\]]+)\]?\s+\[(?<datetime>[^\]]+)\]\s+""(?<request>[^""]+)""\s+(?<status>\d+)\s+(?<bytes>\d+)\s+""(?<referer>[^""]+)""\s+""(?<agent>[^""]+)""");
        static string DateFormat => "dd/MMM/yyyy:HH:mm:ss zzz";

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
            var rootCommand = new RootCommand("Real-time log reading utility");
            rootCommand.Add(fileOption);
            rootCommand.Add(intervalOption);

            rootCommand.SetAction(async (result) =>
            {
                var file = result.GetValue(fileOption)!;
                var interval = result.GetValue(intervalOption);
                ReadLogfile(file, interval).Wait();
            });

            return rootCommand.Parse(args).Invoke();
        }

        static async Task ReadLogfile(FileInfo fileInfo, int interval)
        {
            Console.WriteLine($"Reading file {fileInfo.FullName} with interval: {interval} ms");
            if (!fileInfo.Exists)
            {
                Console.WriteLine($"File {fileInfo.FullName} doesn't exists");
            }
            using var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                Logs.Add(ParseLine(line));
            }

            _ = Task.Run(Output);

            while(true)
            {
                bool isRead = false;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    isRead = true;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    Logs.Add(ParseLine(line));
                }

                if (!isRead)
                {
                    reader.DiscardBufferedData();
                }

                await Task.Delay(interval);
            }
        }

        static async Task Output()
        {
            int count = 0;
            while (true)
            {
                if (count != Logs.Count)
                {
                    count = Logs.Count;
                    Console.WriteLine($"Current count {Logs.Count}");
                }
                await Task.Delay(1000);
            }
        }

        static NginxBaseLog ParseLine(string line)
        {
            var match = GroupRegex.Match(line);
            if (!match.Success)
            {
                throw new InvalidDataException();
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
    }
}
