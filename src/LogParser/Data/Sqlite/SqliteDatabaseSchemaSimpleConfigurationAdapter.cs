using LogParser.Configuration;

namespace LogParser.Data.Sqlite
{
    public class SqliteDatabaseSchemaSimpleConfigurationAdapter
        : IDatabaseSchemaAdapter<SimpleConfiguration, SqliteDatabaseSchema>
    {
        public SqliteDatabaseSchema Adapt(SimpleConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
