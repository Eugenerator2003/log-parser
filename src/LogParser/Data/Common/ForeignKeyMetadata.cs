namespace LogParser.Data.Common
{
    public class ForeignKeyMetadata
    {
        public string Column { get; }

        public string TargetTable { get; }

        public string TargetColumn { get; }

        public ForeignKeyMetadata(string column, string targetTable, string targetColumn)
        {
            Column = column;
            TargetTable = targetTable;
            TargetColumn = targetColumn;
        }

        public override bool Equals(object? obj)
        {
            return obj is ForeignKeyMetadata metadata &&
                   Column == metadata.Column &&
                   TargetTable == metadata.TargetTable &&
                   TargetColumn == metadata.TargetColumn;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column, TargetTable, TargetColumn);
        }
    }
}
