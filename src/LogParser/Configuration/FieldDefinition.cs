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
    }
}
