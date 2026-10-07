using LogParser.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Data.Common
{
    [TestClass]
    public class SingleIndexMetadataTests
    {
        [TestMethod]
        public void AddIndexTest()
        {
            var indexByColumn = new IndexByColumnMetadata("Column", false);
            var singleIndexMetadata = new SingleIndexMetadata();
            singleIndexMetadata.AddIndex(indexByColumn);
            Assert.AreEqual(indexByColumn, singleIndexMetadata.GetIndexColumn());
        }

        [TestMethod]
        public void AddSecondIndexTest()
        {
            var indexByColumn1 = new IndexByColumnMetadata("Column1", false);
            var indexByColumn2 = new IndexByColumnMetadata("Column2", false);
            var singleIndexMetadata = new SingleIndexMetadata();
            singleIndexMetadata.AddIndex(indexByColumn1);
            Assert.ThrowsExactly<InvalidDataException>(() =>
            {
                singleIndexMetadata.AddIndex(indexByColumn2);
            });
        }
    }
}
