using LogParser.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Data.Common
{
    [TestClass]
    public class ForeignKeyMetadataTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            var column = "Column";
            var targetTable = "TargetTable";
            var targetColumn = "TargetColumn";
            var foreignKeyMetadata = new ForeignKeyMetadata(column, targetTable, targetColumn);
            Assert.AreEqual(column, foreignKeyMetadata.Column);
            Assert.AreEqual(targetTable, foreignKeyMetadata.TargetTable);
            Assert.AreEqual(targetColumn, foreignKeyMetadata.TargetColumn);
        }
    }
}
