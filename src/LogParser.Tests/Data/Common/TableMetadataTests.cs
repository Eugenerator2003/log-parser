using LogParser.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Data.Common
{
    [TestClass]
    public class TableMetadataTests
    {
        private static string ColumnType => "integer";
        private static bool ColumnIsNullable => false;
        private static bool IndexIsAscending => true;
        private static string ForeignTargetTable => "TargetTable";
        private static string ForeignTargetColumn => "TargetColumn";

        private static ColumnMetadata[] GetColumns(params string[] names)
        {
            var columns = new ColumnMetadata[names.Length];
            for(int i = 0; i < names.Length; i++)
            {
                columns[i] = new ColumnMetadata(names[i], ColumnType, ColumnIsNullable);
            }
            return columns;
        }

        private static IIndexMetadata[] GetIndices(params string[] columns)
        {
            var singleIndices = new IIndexMetadata[columns.Length];
            for(int i = 0; i < columns.Length; i++)
            {
                var indexByColumn = new IndexByColumnMetadata(columns[i], IndexIsAscending);
                var singleIndex = new SingleIndexMetadata();
                singleIndex.AddIndex(indexByColumn);
                singleIndices[i] = singleIndex;
            }
            return singleIndices;
        }

        private static ForeignKeyMetadata[] GetForeignKeys(params string[] columns)
        {
            var foreignKeys = new ForeignKeyMetadata[columns.Length];
            for(int i = 0; i < columns.Length; i++)
            {
                var foreignKey = new ForeignKeyMetadata(columns[i], ForeignTargetTable, ForeignTargetColumn);
                foreignKeys[i] = foreignKey;
            }
            return foreignKeys;
        }

        [TestMethod]
        public void InitializeTest()
        {
            var name = "table";
            string[] columnNames = ["column1", "column2", "column3"];
            var columns = GetColumns(columnNames);
            string[] indexNames = ["column2"];
            var indices = GetIndices(indexNames);
            string[] foreignKeyNames = ["column3"];
            var foreignKeys = GetForeignKeys(foreignKeyNames);
            var tableMetadata = new TableMetadata(name, columns, foreignKeys, indices);
            Assert.AreEqual(name, tableMetadata.Name);
            AssertColumns(columnNames, tableMetadata.Columns);
            AssertIndices(indexNames, tableMetadata.Indices);
            AssertForeignKeys(foreignKeyNames, foreignKeys);
        }

        private static void AssertColumns(string[] names, IReadOnlyCollection<ColumnMetadata> columnCollection)
        {
            Assert.HasCount(names.Length, columnCollection);
            var columns = columnCollection.ToArray();
            for(int i = 0; i < names.Length; i++)
            {
                Assert.AreEqual(names[i], columns[i].Name);
                Assert.AreEqual(ColumnType, columns[i].Type);
                Assert.AreEqual(ColumnIsNullable, columns[i].IsNullable);
            }
        }

        private static void AssertIndices(string[] names, IReadOnlyCollection<IIndexMetadata> indexCollection)
        {
            Assert.HasCount(names.Length, indexCollection);
            var indices = indexCollection.Select(index => (SingleIndexMetadata)index).ToArray();
            for (int i = 0; i < names.Length; i++)
            {
                Assert.AreEqual(names[i], indices[i].GetIndexColumn()?.Column);
                Assert.AreEqual(IndexIsAscending, indices[i].GetIndexColumn()?.IsAscending);
            }
        }

        private static void AssertForeignKeys(string[] names, IReadOnlyCollection<ForeignKeyMetadata> foreignKeyCollection)
        {
            Assert.HasCount(names.Length, foreignKeyCollection);
            var foreignKeys = foreignKeyCollection.ToArray();
            for (int i = 0; i < names.Length; i++)
            {
                Assert.AreEqual(names[i], foreignKeys[i].Column);
                Assert.AreEqual(ForeignTargetTable, foreignKeys[i].TargetTable);
                Assert.AreEqual(ForeignTargetColumn, foreignKeys[i].TargetColumn);
            }
        }

        [TestMethod]
        public void InitializeColumnNameDuplicateTest() 
        {
            var name = "table";
            string[] columnNames = ["column1, column1", "column2"];
            var columns = GetColumns(columnNames);
            string[] indexNames = ["column2"];
            var indices = GetIndices(indexNames);
            string[] foreignKeyNames = ["column3"];
            var foreignKeys = GetForeignKeys(foreignKeyNames);
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var tableMetadata = new TableMetadata(name, columns, foreignKeys, indices);
            });
        }

        [TestMethod]
        public void InitializeIndexColumnNotFoundTest()
        {
            var name = "table";
            string[] columnNames = ["column1, column2", "column3"];
            var columns = GetColumns(columnNames);
            string[] indexNames = ["column4"];
            var indices = GetIndices(indexNames);
            string[] foreignKeyNames = ["column3"];
            var foreignKeys = GetForeignKeys(foreignKeyNames);
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var tableMetadata = new TableMetadata(name, columns, foreignKeys, indices);
            });
        }

        [TestMethod]
        public void InitializeIndexDuplicateTest()
        {
            var name = "table";
            string[] columnNames = ["column1, column2", "column3"];
            var columns = GetColumns(columnNames);
            string[] indexNames = ["column1", "column1"];
            var indices = GetIndices(indexNames);
            string[] foreignKeyNames = ["column3"];
            var foreignKeys = GetForeignKeys(foreignKeyNames);
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var tableMetadata = new TableMetadata(name, columns, foreignKeys, indices);
            });
        }

        [TestMethod]
        public void InitializeForeignKeyColumnNotFoundTest()
        {
            var name = "table";
            string[] columnNames = ["column1, column2", "column3"];
            var columns = GetColumns(columnNames);
            string[] indexNames = ["column2"];
            var indices = GetIndices(indexNames);
            string[] foreignKeyNames = ["column4"];
            var foreignKeys = GetForeignKeys(foreignKeyNames);
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var tableMetadata = new TableMetadata(name, columns, foreignKeys, indices);
            });
        }

        [TestMethod]
        public void InitializeForeignKeyDuplicateTest()
        {
            var name = "table";
            string[] columnNames = ["column1, column2", "column3"];
            var columns = GetColumns(columnNames);
            string[] indexNames = ["column2"];
            var indices = GetIndices(indexNames);
            string[] foreignKeyNames = ["column3", "column3"];
            var foreignKeys = GetForeignKeys(foreignKeyNames);
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                var tableMetadata = new TableMetadata(name, columns, foreignKeys, indices);
            });
        }
    }
}
