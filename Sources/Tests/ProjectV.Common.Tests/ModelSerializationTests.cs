using AwesomeAssertions;
using Newtonsoft.Json;
using ProjectV.Models.Data;
using Xunit;

namespace ProjectV.Common.Tests
{
    [Trait("Category", "Unit")]
    public sealed class ModelSerializationTests
    {
        public ModelSerializationTests()
        {
        }

        [Fact]
        public void BasicInfoSerializationToJsonAndBack()
        {
            // Arrange.
            // BasicInfo is annotated with [JsonConstructor] on its 4-arg ctor
            // (see Sources/Libraries/ProjectV.Models/Data/BasicInfo.cs), so
            // Newtonsoft.Json round-trips correctly even without a parameterless
            // ctor. This replaces the System.Text.Json approach that required a
            // parameterless ctor and was the reason the original test was Skip'd.
            var expectedModel = new BasicInfo(42, "Title", 100, 9.9);

            // Act.
            string compactJson = Serialize(expectedModel);
            var compactRoundTrip = Deserialize<BasicInfo>(compactJson);

            string prettyJson = SerializePrettyPrint(expectedModel);
            var prettyRoundTrip = Deserialize<BasicInfo>(prettyJson);

            // Assert.
            compactRoundTrip.Should().NotBeNull();
            compactRoundTrip.Should().Be(expectedModel);

            prettyRoundTrip.Should().NotBeNull();
            prettyRoundTrip.Should().Be(expectedModel);
        }

        private static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private static string SerializePrettyPrint<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        private static T? Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
