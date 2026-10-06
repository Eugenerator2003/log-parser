using LogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Configuration
{
    [TestClass]
    public class RegexGroupTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            var name = "name";
            var fieldDefinition = new FieldDefinition(FieldType.Decimal, isIndexed: false, isUniq: true);
            var group = new RegexGroup(name, fieldDefinition);
            Assert.AreEqual(name, group.Name);
            Assert.AreEqual(fieldDefinition, group.FieldDefinition);
        }
    }
}
