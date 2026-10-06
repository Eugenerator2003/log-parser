namespace LogParser.Configuration
{
    public class JsonConfigurationReader : IConfigurationReader
    {
        private Stream stream;

        public JsonConfigurationReader(Stream stream)
        {
            this.stream = stream;
        }

        public IConfiguration Read()
        {
            throw new NotImplementedException();
        }
    }
}
