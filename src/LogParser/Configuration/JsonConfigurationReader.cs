using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.CommandLine.Parsing;

namespace LogParser.Configuration
{
    public class JsonConfigurationReader : IConfigurationReader
    {
        private static string GetPath(JToken token)
        {
            var path = string.IsNullOrEmpty(token.Path) ? "root" : token.Path;
            return path;
        }

        private static JToken ReadToken(JToken parentToken, string tokenName)
        {
            var token = parentToken[tokenName] ??
                throw new InvalidDataException($"JSON property {tokenName} not found in token {GetPath(parentToken)}");
            return token;
        }

        private static T ReadTokenWithCast<T>(JToken parentToken, string tokenName)
        {
            var token = ReadToken(parentToken, tokenName);
            try
            {
                var value = token.Value<T>() 
                    ?? throw new InvalidDataException($"JSON token \"{GetPath(token)}\" is null or missing, but expected {typeof(T).Name}");
                return value;
            }
            catch (Exception ex) when (ex is not InvalidDataException)
            {
                throw new InvalidDataException($"Cannot cast JSON token \"{GetPath(token)}\" to type {typeof(T).Name}", ex);
            }
        }

        private static T ReadTokenWithEnumCast<T>(JToken parentToken, string tokenName) where T : struct, Enum
        {
            var token = ReadToken(parentToken, tokenName);
            var tokenString = token.Value<string>()
                    ?? throw new InvalidDataException($"JSON token \"{GetPath(token)}\" is null or missing, but expected non-null {typeof(T).Name}");
            if (!Enum.TryParse(tokenString, ignoreCase: true, out T value))
            {
                throw new InvalidDataException($"Cannot cast JSON token \"{GetPath(token)}\" to type {typeof(T).Name}");
            }
            return value;
        }

        private static JToken[] ReadTokenAsArray(JToken parentToken, string tokenName)
        {
            var token = ReadToken(parentToken, tokenName);
            if (token.Type != JTokenType.Array)
            {
                throw new InvalidDataException($"JSON token \"{GetPath(token)}\" must be an array");
            }
            return token.ToArray();
        }

        private static RegexGroup ParseToRegexGroup(JToken jToken)
        {
            var name = ReadTokenWithCast<string>(jToken, "name");
            var definitionToken = ReadToken(jToken, "fieldDefinition");
            var fieldDefinition = ParseToFieldDefinition(definitionToken);
            return new RegexGroup(name, fieldDefinition);
        }

        private static FieldDefinition ParseToFieldDefinition(JToken jToken)
        {
            var type = ReadTokenWithEnumCast<FieldType>(jToken, "type");
            var isIndexed = ReadTokenWithCast<bool>(jToken, "isIndexed");
            var isUnique = ReadTokenWithCast<bool>(jToken, "isUnique");
            var definition = new FieldDefinition(type, isIndexed, isUnique);
            return definition;
        }

        private Stream stream;
        private JObject jObject = null!;
        private SimpleConfiguration configuration = null!;

        public JsonConfigurationReader(Stream stream)
        {
            this.stream = stream;
        }

        public IConfiguration Read()
        {
            LoadJObject();
            CreateConfiguration();
            AddGroupsToConfiguration();
            return configuration;
        }

        private JObject LoadJObject()
        {
            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);
            jObject = JObject.Load(jsonReader);
            return jObject;
        }

        private SimpleConfiguration CreateConfiguration()
        {
            var pattern = ReadTokenWithCast<string>(jObject, "pattern");
            configuration = new SimpleConfiguration(pattern);
            return configuration;
        }

        private void AddGroupsToConfiguration()
        {
            var groupTokens = GetGroups();
            foreach (var token in groupTokens)
            {
                var group = ParseToRegexGroup(token);
                configuration.AddRegexGroup(group);
            }
        }

        private JToken[] GetGroups()
        {
            var jArray = ReadTokenAsArray(jObject, "groups");
            return jArray;
        }
    }
}
