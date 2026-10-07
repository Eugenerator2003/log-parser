using LogParser.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Data.Common
{
    [TestClass]
    public class ColumnMetadataTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            var name = "Column1";
            var type = "INTEGER";
            var isNullable = false;
            var columnMetadata = new ColumnMetadata(name, type, isNullable);
            Assert.AreEqual(name, columnMetadata.Name);
            Assert.AreEqual(type, columnMetadata.Type);
            Assert.AreEqual(isNullable, columnMetadata.IsNullable);
        }
    }
}
