namespace LogParser.Data.Common
{
    public class SingleIndexMetadata : IIndexMetadata
    {
        private IndexByColumnMetadata? indexByColumn;

        public SingleIndexMetadata()
        {

        }

        public void AddIndex(IndexByColumnMetadata index)
        {
            if (indexByColumn != null)
            {
                throw new InvalidDataException("Index column is already taken for single index");
            }
            indexByColumn = index;
        }

        public IndexByColumnMetadata? GetIndexColumn()
        {
            return indexByColumn;
        }

        public IReadOnlyCollection<IndexByColumnMetadata> GetIndexColumns()
        {
            if (indexByColumn != null)
            {
                return [indexByColumn];
            }else
            {
                return Array.Empty<IndexByColumnMetadata>();
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is SingleIndexMetadata metadata &&
                   EqualityComparer<IndexByColumnMetadata?>.Default.Equals(indexByColumn, metadata.indexByColumn);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(indexByColumn);
        }
    }
}
