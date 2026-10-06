using LogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Configuration
{
    [TestClass]
    public class SimpleConfigurationTests
    {
        private static SimpleConfiguration GetSimpleConfiguration(string pattern)
        {
            return new SimpleConfiguration(pattern);
        }

        private static RegexGroup GetRegexGroup(string name)
        {
            return new RegexGroup(name, new FieldDefinition(FieldType.Int32, false, false));
        }

        [TestMethod]
        public void InitializeTest()
        {
            var pattern = "(?<GroupName>[abc]+)";
            var configuration = new SimpleConfiguration(pattern);
            Assert.AreEqual(pattern, configuration.RegexPattern);
            Assert.HasCount(0, configuration.Groups);
        }

        [TestMethod]
        public void AddInvalidRegexGroupTest()
        {
            var configuration = GetSimpleConfiguration("(?<GroupName>[abc]+)");
            var invalidGroup = GetRegexGroup("GroupName2");
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                configuration.AddRegexGroup(invalidGroup);
            });
        }

        [TestMethod]
        public void AddDuplicateRegexGroupTest()
        {
            var configuration = GetSimpleConfiguration("(?<GroupName>[abc]+)");
            var invalidGroup1 = GetRegexGroup("GroupName");
            var invalidGroup2 = GetRegexGroup("GroupName");
            configuration.AddRegexGroup(invalidGroup1);
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                configuration.AddRegexGroup(invalidGroup2);
            });
        }

        [TestMethod]
        public void AddRegexGroupTest()
        {
            var configuration = GetSimpleConfiguration("(?<GroupName1>[abc]+)\\s+(?<GroupName2>[abc]+)");
            var invalidGroup1 = GetRegexGroup("GroupName1");
            var invalidGroup2 = GetRegexGroup("GroupName2");
            configuration.AddRegexGroup(invalidGroup1);
            configuration.AddRegexGroup(invalidGroup2);
            Assert.HasCount(2, configuration.Groups);
        }
    }
}
