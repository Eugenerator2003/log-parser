namespace LogParser.Data.Common
{
    public interface IIndexMetadata
    {
        IReadOnlyCollection<IndexByColumnMetadata> GetIndexColumns();
        
        void AddIndex(IndexByColumnMetadata index);
    }
}
