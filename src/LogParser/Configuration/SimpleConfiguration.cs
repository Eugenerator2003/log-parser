using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace LogParser.Configuration
{
    public class SimpleConfiguration : IConfiguration
    {
        public string RegexPattern
        {
            get => pattern;
        }

        public IReadOnlyCollection<RegexGroup> Groups
        {
            get => groups.AsReadOnly(); 
        }

        private string pattern;
        private List<RegexGroup> groups;

        public SimpleConfiguration(string regexPattern)
        {
            pattern = regexPattern;
            groups = new List<RegexGroup>();
        }

        public void AddRegexGroup(RegexGroup regexGroup)
        {
            RegexMentionCheck(regexGroup);
            UniqueGroupNameCheck(regexGroup);
            AddGroupToList(regexGroup);
        }

        private void UniqueGroupNameCheck(RegexGroup regexGroup)
        {
            if (groups.Any(group => group.Name == regexGroup.Name))
            {
                throw new InvalidOperationException($"Group with name {regexGroup.Name} already mentioned in configuration");
            }
        }

        private void RegexMentionCheck(RegexGroup regexGroup)
        {
            Regex regex = new Regex(@$"\(\?\<{regexGroup.Name}\>.+\)");
            if (!regex.IsMatch(pattern))
            {
                throw new InvalidOperationException($"Group with name {regexGroup.Name} not mentioned in regex pattern");
            }
        }

        private void AddGroupToList(RegexGroup regexGroup)
        {
            groups.Add(regexGroup);
        }
    }
}
