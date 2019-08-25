using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.CommonTests
{
    public sealed class ModelSerializationTests
    {
        public ModelSerializationTests()
        {
        }

        [Fact]
        public void BasucInfoSerializationTest()
        {
            var expectedModel = new BasicInfo(42, "Title", 100, 9.9);

            var json = Serialize(expectedModel);
            var actualModel = Deserialize<BasicInfo>(json);

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

        private static T Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
