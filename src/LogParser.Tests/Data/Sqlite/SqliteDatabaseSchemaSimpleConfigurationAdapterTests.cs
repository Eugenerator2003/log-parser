using LogParser.Configuration;
using LogParser.Data.Common;
using LogParser.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Tests.Data.Sqlite
{
    [TestClass]
    public class SqliteDatabaseSchemaSimpleConfigurationAdapterTests
    {
        private static SimpleConfiguration GetSimpleConfiguration()
        {
            var configuration = new SimpleConfiguration("(?<GroupName1>[abc]+)\\s+(?<GroupName2>[\\d+]+)\\s+(?<GroupName3>[ABC]+)");
            configuration.AddRegexGroup(new RegexGroup("GroupName1", new FieldDefinition(FieldType.Text, false, true)));
            configuration.AddRegexGroup(new RegexGroup("GroupName2", new FieldDefinition(FieldType.Text, true, false)));
            configuration.AddRegexGroup(new RegexGroup("GroupName3", new FieldDefinition(FieldType.Text, true, false)));
            return configuration;
        }

        private static SqliteDatabaseSchema GetExpectedSqliteSchema()
        {
            var schema = new SqliteDatabaseSchema();
            var table1 = GetReferenceTable();
            var table2 = GetGenertalTable();
            schema.AddTable(table1);
            schema.AddTable(table2);
            return schema;
        }

        private static TableMetadata GetReferenceTable()
        {
            var table = new TableMetadata(
                name: "GroupName1s",
                columns: [
                    new ColumnMetadata("id", "integer", false),
                    new ColumnMetadata("GroupName1", "text", false)
                ],
                foreignKeys: [],
                indices: []);
            return table;
        }

        private static TableMetadata GetGenertalTable()
        {
            var indices = GetIndices();
            var table = new TableMetadata(
                name: "GeneralEntries",
                columns: [
                    new ColumnMetadata("id", "integer", false),
                    new ColumnMetadata("GroupName1Id", "integer", false),
                    new ColumnMetadata("GroupName2", "text", true),
                    new ColumnMetadata("GroupName3", "text", true)
                ],
                foreignKeys: [
                    new ForeignKeyMetadata("GroupName1Id", "GroupName1s", "id")
                ],
                indices: indices
            );
            return table;
        }

        private static SingleIndexMetadata[] GetIndices()
        {
            string[] columns = ["GroupName2", "GroupName3"];
            var indices = new SingleIndexMetadata[2];
            for(int i = 0; i < indices.Length; i++)
            {
                indices[i] = new SingleIndexMetadata();
                var indexByColumn = new IndexByColumnMetadata("", true);
                indices[i].AddIndex(indexByColumn);
            }
            return indices;
        }

        [TestMethod]
        public void AdaptTest()
        {
            var configuration = GetSimpleConfiguration();
            var adapter = new SqliteDatabaseSchemaSimpleConfigurationAdapter();
            var schema = adapter.Adapt(configuration);
            AssertSqliteSchema(schema);
        }

        private void AssertSqliteSchema(SqliteDatabaseSchema actualSchema)
        {
            var expectedSchema = GetExpectedSqliteSchema();
            var expectedTables = expectedSchema.Tables.ToArray();
            var actualTables = actualSchema.Tables.ToArray();
            Assert.HasCount(expectedTables.Length, actualTables);
            Assert.AreEqual(expectedTables[0], actualTables[0]);
            Assert.AreEqual(expectedTables[1], actualTables[1]);
        }
    }
}
