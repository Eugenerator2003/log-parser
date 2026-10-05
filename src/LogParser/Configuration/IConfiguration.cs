namespace LogParser.Configuration
{
    public interface IConfiguration
    {
        string RegexPattern { get; }

        IReadOnlyCollection<RegexGroup> Groups { get; }
    }
}
