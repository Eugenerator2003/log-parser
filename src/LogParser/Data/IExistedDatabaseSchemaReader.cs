namespace LogParser.Data
{
    public interface IExistedDatabaseSchemaReader<T> where T : IDatabaseSchema
    {
        T ReadFromExistedDatabase();
    }
}
