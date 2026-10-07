using LogParser.Data.Common;

namespace LogParser.Data
{
    public interface IDatabaseSchema
    {
        IReadOnlyCollection<TableMetadata> Tables { get; }

        void AddTable(TableMetadata table);

        void Validate();
    }
}
