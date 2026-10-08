using IConfiguration = LogParser.Configuration.IConfiguration;

namespace LogParser.Data
{
    public interface IDatabaseSchemaAdapter<TConfiguration, TSchema> 
        where TConfiguration : IConfiguration
        where TSchema : IDatabaseSchema
    {
        TSchema Adapt(TConfiguration configuration);
    }
}
