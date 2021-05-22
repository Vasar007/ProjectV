using System.Text.Json;
using ProjectV.Models.Data;
using Xunit;

namespace ProjectV.Common.Tests
{
    public sealed class ModelSerializationTests
    {
        public ModelSerializationTests()
        {
        }

        [Fact(Skip = "Current version of JsonSerializer cannot work with class without " +
                     "parameterless constructors.")]
        public void BasicInfoSerializationToJsonAndBack()
        {
            var expectedModel = new BasicInfo(42, "Title", 100, 9.9);

            string json = Serialize(expectedModel);
            var actualModel = Deserialize<BasicInfo>(json);
            Assert.NotNull(actualModel);
            Assert.Equal(expectedModel, actualModel);

            json = SerializePrettyPrint(expectedModel);
            actualModel = Deserialize<BasicInfo>(json);
            Assert.NotNull(actualModel);
            Assert.Equal(expectedModel, actualModel);
        }

        private static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }

        private static string SerializePrettyPrint<T>(T value)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(value, options);
        }

        private static T? Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
