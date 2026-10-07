using LogParser.Data.Common;
using LogParser.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Data.Sqlite
{
    [TestClass]
    public class SqliteDatabaseSchemaTests
    {
        private readonly struct ForeignKeyValue
        {
            public required int ColumnIndex { get; init; }
            public required string TargetTable { get; init; }
            public required int TargetColumnIndex { get; init; }
        }

        private static TableMetadata GetTable(string name, int columnCount, params ForeignKeyValue[] foreignKeysValues)
        {
            var columns = GetColumns(columnCount);
            var foreignKeys = GetForeignKeys(foreignKeysValues);
            var indices = Array.Empty<IIndexMetadata>();
            var table = new TableMetadata(name, columns, foreignKeys, indices);
            return table;
        }

        private static ColumnMetadata[] GetColumns(int columnCount)
        {
            var columns = new ColumnMetadata[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                columns[i] = new ColumnMetadata($"column{i + 1}", "integer", false);
            }
            return columns;
        }

        private static ForeignKeyMetadata[] GetForeignKeys(params ForeignKeyValue[] foreignKeysValues)
        {
            var foreignKeys = new ForeignKeyMetadata[foreignKeysValues.Length];
            for(int i = 0; i < foreignKeys.Length; i++)
            {
                var column = $"column{foreignKeysValues[i].ColumnIndex}";
                var targetTable = foreignKeysValues[i].TargetTable;
                var targetColumn = $"column{foreignKeysValues[i].TargetColumnIndex}";
                foreignKeys[i] = new ForeignKeyMetadata(column, targetTable, targetColumn);
            }
            return foreignKeys;
        }

        [TestMethod]
        public void AddTest()
        {
            var table1 = GetTable("table1", 3);
            var table2 = GetTable(
                "table2", 
                2,
                [new ForeignKeyValue() { ColumnIndex = 1, TargetTable = "table1", TargetColumnIndex = 2}]);
            var sqliteSchema = new SqliteDatabaseSchema();
            sqliteSchema.AddTable(table1);
            sqliteSchema.AddTable(table2);
            var tables = sqliteSchema.Tables.ToArray();
            Assert.HasCount(2, tables);
            Assert.AreEqual(table1, tables[0]);
            Assert.AreEqual(table2, tables[1]);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            var table1 = GetTable("table1", 3);
            var sqliteSchema = new SqliteDatabaseSchema();
            sqliteSchema.AddTable(table1);
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                sqliteSchema.AddTable(table1);
            });
        }

        [TestMethod]
        public void ValidateTest()
        {
            var table1 = GetTable("table1", 3);
            var table2 = GetTable(
                "table2",
                2,
                [new ForeignKeyValue() { ColumnIndex = 1, TargetTable = "table1", TargetColumnIndex = 2 }]);
            var sqliteSchema = new SqliteDatabaseSchema();
            sqliteSchema.AddTable(table1);
            sqliteSchema.AddTable(table2);
            sqliteSchema.Validate();
        }

        [TestMethod]
        public void ValidateInvalidForeignKeyTargetColumnTest()
        {
            var table1 = GetTable("table1", 3);
            var table2 = GetTable(
                "table2",
                2,
                [new ForeignKeyValue() { ColumnIndex = 1, TargetTable = "table1", TargetColumnIndex = 4 }]);
            var sqliteSchema = new SqliteDatabaseSchema();
            sqliteSchema.AddTable(table1);
            sqliteSchema.AddTable(table2);
            Assert.ThrowsExactly<InvalidOperationException>(sqliteSchema.Validate);
        }

        [TestMethod]
        public void ValidateInvalidForeignKeyTargetTableTest()
        {
            var table1 = GetTable("table1", 3);
            var table2 = GetTable(
                "table2",
                2,
                [new ForeignKeyValue() { ColumnIndex = 1, TargetTable = "table3", TargetColumnIndex = 4 }]);
            var sqliteSchema = new SqliteDatabaseSchema();
            sqliteSchema.AddTable(table1);
            sqliteSchema.AddTable(table2);
            Assert.ThrowsExactly<InvalidOperationException>(sqliteSchema.Validate);
        }
    }
}
