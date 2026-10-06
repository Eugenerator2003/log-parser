namespace LogParser.Configuration
{
    public class RegexGroup
    {
        public string Name { get; }

        public FieldDefinition FieldDefinition { get; }

        public RegexGroup(string name, FieldDefinition fieldDefinition)
        {
            Name = name;
            FieldDefinition = fieldDefinition;
        }

        public override bool Equals(object? obj)
        {
            return obj is RegexGroup group &&
                   Name == group.Name &&
                   EqualityComparer<FieldDefinition>.Default.Equals(FieldDefinition, group.FieldDefinition);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, FieldDefinition);
        }
    }
}
