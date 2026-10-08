using LogParser.Data.Common;

namespace LogParser.Data
{
    public abstract class AbstractDatabaseSchema : IDatabaseSchema
    {
        private static string GetAbsoluteColumnName(string table, string column)
        {
            return $"{table}.{column}";
        }

        public IReadOnlyCollection<TableMetadata> Tables => TablesList.ToArray();

        protected List<TableMetadata> TablesList { get; }

        protected AbstractDatabaseSchema()
        {
            TablesList = new List<TableMetadata>();
        }

        public void AddTable(TableMetadata table)
        {
            ValidateTableDuplicate(table);
            TablesList.Add(table);
        }

        public void Validate()
        {
            var allColumns = GetAllColumnsHashSet();
            ValidateForeignKeys(allColumns);
        }

        private void ValidateTableDuplicate(TableMetadata tableMetadata)
        {
            if (TablesList.Any(table => table.Name == tableMetadata.Name))
            {
                throw new InvalidOperationException($"Found duplicate for table with name {tableMetadata.Name}");
            }
        }

        private HashSet<string> GetAllColumnsHashSet()
        {
            HashSet<string> columnSet = new HashSet<string>();
            foreach (var table in TablesList)
            {
                foreach (var column in table.Columns)
                {
                    columnSet.Add(GetAbsoluteColumnName(table.Name, column.Name));
                }
            }
            return columnSet;
        }

        private void ValidateForeignKeys(HashSet<string> columns)
        {
            foreach (var table in TablesList)
            {
                foreach(var foreignKey in table.ForeignKeys)
                {
                    var foreignKeyAbsoluteName = GetAbsoluteColumnName(foreignKey.TargetTable, foreignKey.TargetColumn);
                    if (!columns.Contains(foreignKeyAbsoluteName))
                    {
                        throw new InvalidOperationException($"Foreign key error: target table {foreignKey.TargetTable} with column {foreignKey.Column} not found");
                    }
                }
            }
        }
    }
}
