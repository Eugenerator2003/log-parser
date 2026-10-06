namespace LogParser.Configuration
{
    public class FieldDefinition
    {
        public FieldType Type { get; }

        public bool IsIndexed { get; }

        public bool IsUniq { get; }

        public FieldDefinition(FieldType type, bool isIndexed, bool isUniq)
        {
            IsIndexed = isIndexed;
            IsUniq = isUniq;
            Type = type;
        }

        public override bool Equals(object? obj)
        {
            return obj is FieldDefinition definition &&
                   Type == definition.Type &&
                   IsIndexed == definition.IsIndexed &&
                   IsUniq == definition.IsUniq;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, IsIndexed, IsUniq);
        }
    }
}
