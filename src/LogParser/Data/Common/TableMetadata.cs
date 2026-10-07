namespace LogParser.Data.Common
{
    public class TableMetadata
    {

        public string Name { get; }
        public IReadOnlyCollection<ColumnMetadata> Columns { get; }
        public IReadOnlyCollection<ForeignKeyMetadata> ForeignKeys { get; }
        public IReadOnlyCollection<IIndexMetadata> Indices { get; }

        public TableMetadata(
            string name, 
            IReadOnlyCollection<ColumnMetadata> columns, 
            IReadOnlyCollection<ForeignKeyMetadata> foreignKeys, 
            IReadOnlyCollection<IIndexMetadata> indices)
        {
            Name = name;
            Columns = columns;
            ForeignKeys = foreignKeys;
            Indices = indices;
        }
    }
}
