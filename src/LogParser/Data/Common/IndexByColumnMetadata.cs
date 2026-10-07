namespace LogParser.Data.Common
{
    public class IndexByColumnMetadata
    {
        public string Column { get; }

        public bool IsAscending { get; }

        public IndexByColumnMetadata(string columnName, bool isAscending)
        {
            Column = columnName;
            IsAscending = isAscending;
        }

        public override bool Equals(object? obj)
        {
            return obj is IndexByColumnMetadata metadata &&
                   Column == metadata.Column &&
                   IsAscending == metadata.IsAscending;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column, IsAscending);
        }
    }
}
