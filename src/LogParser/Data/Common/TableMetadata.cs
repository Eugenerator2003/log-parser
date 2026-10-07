using System.Text;

namespace LogParser.Data.Common
{
    public class TableMetadata
    {
        private static string GetIndexDefinition(IIndexMetadata index)
        {
            StringBuilder builder = new StringBuilder();
            var indexColumns = index.GetIndexColumns().ToArray();
            for (int i = 0; i < indexColumns.Length; i++)
            {
                var ascendingSuffix = indexColumns[i].IsAscending ? "asc" : "desc";
                builder.Append($"{indexColumns[i].Column} {ascendingSuffix}");
                if (i + 1 < indexColumns.Length)
                {
                    builder.Append(',');
                }
            }
            return builder.ToString();
        }

        public string Name { get; }
        public IReadOnlyCollection<ColumnMetadata> Columns => columns;
        public IReadOnlyCollection<ForeignKeyMetadata> ForeignKeys => foreignKeys;
        public IReadOnlyCollection<IIndexMetadata> Indices => indices;

        private ColumnMetadata[] columns;
        private ForeignKeyMetadata[] foreignKeys;
        private IIndexMetadata[] indices;

        private HashSet<string> columnSet;

        public TableMetadata(
            string name,
            IReadOnlyCollection<ColumnMetadata> columns,
            IReadOnlyCollection<ForeignKeyMetadata> foreignKeys,
            IReadOnlyCollection<IIndexMetadata> indices)
        {
            Name = name;
            this.columns = columns.ToArray();
            this.foreignKeys = foreignKeys.ToArray();
            this.indices = indices.ToArray();
            columnSet = new HashSet<string>();
            ValidateColumns();
            ValidateIndices();
            ValidateForeignKeys();
        }

        private void ValidateColumns()
        {
            columnSet = new HashSet<string>();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (columnSet.Contains(columns[i].Name))
                {
                    throw new InvalidOperationException($"Found duplicate for column with name {columns[i].Name}");
                }
                else
                {
                    columnSet.Add(columns[i].Name);
                }
            }
        }

        private void ValidateIndices()
        {
            ValidateIndexDuplicates();
            ValidateIndexColumns();
        }

        private void ValidateIndexDuplicates()
        {
            var indexSet = new HashSet<IIndexMetadata>();
            for (int i = 0; i < indices.Length; i++)
            {
                if (indexSet.Contains(indices[i]))
                {

                    throw new InvalidOperationException($"Found duplicate for index: {GetIndexDefinition(indices[i])}");
                }
                else
                {
                    indexSet.Add(indices[i]);
                }
            }
        }

        private void ValidateIndexColumns()
        {
            for (int i = 0; i < indices.Length; i++)
            {
                foreach (var indexByColumn in indices[i].GetIndexColumns())
                {
                    if (!columnSet.Contains(indexByColumn.Column))
                    {
                        throw new InvalidOperationException($"Index {GetIndexDefinition(indices[i])} reference to a non-existent column {indexByColumn.Column}");
                    }
                }
            }
        }

        private void ValidateForeignKeys()
        {
            ValidateForeignKeysDuplicates();
            ValidateForeignKeysColumns();
        }

        private void ValidateForeignKeysDuplicates()
        {
            var foreignKeySet = new HashSet<ForeignKeyMetadata>();
            for (int i = 0; i < indices.Length; i++)
            {
                if (foreignKeySet.Contains(foreignKeys[i]))
                {

                    throw new InvalidOperationException($"Found duplicate for foreign key: {foreignKeys[i].Column}");
                }
                else
                {
                    foreignKeySet.Add(foreignKeys[i]);
                }
            }
        }

        private void ValidateForeignKeysColumns()
        {
            for (int i = 0; i < foreignKeys.Length; i++)
            {
                if (!columnSet.Contains(foreignKeys[i].Column))
                {
                    throw new InvalidOperationException($"Foreign key {foreignKeys[i].Column} reference to a non-existent column");
                }
            }
        }
    }
}
