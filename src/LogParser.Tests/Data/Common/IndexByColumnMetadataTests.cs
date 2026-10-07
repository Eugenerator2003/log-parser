using LogParser.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Data.Common
{
    [TestClass]
    public class IndexByColumnMetadataTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            var column = "Column";
            var isAscending = false;
            var indexByColumnMetadata = new IndexByColumnMetadata(column, isAscending);
            Assert.AreEqual(column, indexByColumnMetadata.Column);
            Assert.AreEqual(isAscending, indexByColumnMetadata.IsAscending);
        }
    }
}
