namespace LogParser.Data.Common
{
    public class ColumnMetadata
    {
        public string Name { get; }

        public string Type { get; }

        public bool IsNullable { get; }

        public override bool Equals(object? obj)
        {
            return obj is ColumnMetadata metadata &&
                   Name == metadata.Name &&
                   Type == metadata.Type &&
                   IsNullable == metadata.IsNullable;
        }

        public ColumnMetadata(string name, string type, bool isNullable)
        {
            Name = name;
            Type = type;
            IsNullable = isNullable;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type, IsNullable);
        }
    }
}
