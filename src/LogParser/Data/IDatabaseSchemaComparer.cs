namespace LogParser.Data
{
    public interface IDatabaseSchemaComparer<T> where T : IDatabaseSchema
    {
        bool TableEquals(T expectedSchema, T actualSchema);
        bool IndicesEquals(T expectedSchema, T actualSchema);
    }
}
