using LogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Configuration
{
    [TestClass]
    public class FieldDefinitionTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            var type = FieldType.Text;
            var isIndexed = true;
            var isUnique = false;
            var fieldDefinition = new FieldDefinition(type, isIndexed, isUnique);
            Assert.AreEqual(type, fieldDefinition.Type);
            Assert.AreEqual(isIndexed, fieldDefinition.IsIndexed);
            Assert.AreEqual(isUnique, fieldDefinition.IsUniq);
        }
    }
}
