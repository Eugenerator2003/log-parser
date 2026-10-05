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
    }
}
