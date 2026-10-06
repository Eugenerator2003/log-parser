using LogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LogParser.Tests.Configuration
{
    [TestClass]
    public class JsonConfigurationReaderTests
    {
        private static string GetTestJson()
        {
            return """
                {
                    "pattern": "(?<GroupName1>[abc]+)\\s+(?<GroupName2>[abc]+)",
                    "groups" : [
                        {
                            "name" : "GroupName1",
                            "fieldDefinition" : {
                                "type" : "Int32",
                                "isIndexed" : true,
                                "isUnique" : true
                            }
                        },
                        {
                            "name" : "GroupName2",
                            "fieldDefinition" : {
                                "type" : "Text",
                                "isIndexed" : true,
                                "isUnique" : false
                            }
                        }
                    ]
                }
                """;
        }

        private static JsonConfigurationReader GetJsonConfigurationReaderWithTestStream()
        {
            var json = GetTestJson();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return new JsonConfigurationReader(stream);
        }

        private static IConfiguration GetExpectedConfiguration()
        {
            var configuration = new SimpleConfiguration(@"(?<GroupName1>[abc]+)\s+(?<GroupName2>[abc]+)");
            var regexGroup1 = new RegexGroup(
                "GroupName1",
                new FieldDefinition(
                        FieldType.Int32,
                        true,
                        true
                    ));
            var regexGroup2 = new RegexGroup(
                "GroupName2",
                new FieldDefinition(
                        FieldType.Text,
                        true,
                        false
                    ));
            configuration.AddRegexGroup(regexGroup1);
            configuration.AddRegexGroup(regexGroup2);
            return configuration;
        }

        [TestMethod]
        public void ReadTest()
        {
            var reader = GetJsonConfigurationReaderWithTestStream();
            var actulaConfiguration = reader.Read();
            var expectedConfiguration = GetExpectedConfiguration();
            Assert.AreEqual(expectedConfiguration.RegexPattern, actulaConfiguration.RegexPattern);
            Assert.HasCount(2, actulaConfiguration.Groups);
            AssertGroups(expectedConfiguration.Groups, actulaConfiguration.Groups);
        }

        private void AssertGroups(IReadOnlyCollection<RegexGroup> expected, IReadOnlyCollection<RegexGroup> actual)
        {
            var expectedList = expected.ToList();
            var actualList = actual.ToList();
            for(int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expectedList[i], actualList[i]);
            }
        }
    }
}
