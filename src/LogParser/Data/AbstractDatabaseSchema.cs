using LogParser.Data.Common;

namespace LogParser.Data
{
    public abstract class AbstractDatabaseSchema : IDatabaseSchema
    {
        public IReadOnlyCollection<TableMetadata> Tables => throw new NotImplementedException();

        public void AddTable(TableMetadata table)
        {
            throw new NotImplementedException();
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
