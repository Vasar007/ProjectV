using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ThingAppraiser.Extensions;

namespace ThingAppraiser
{
    public static class XmlHelper
    {
        /// <summary>
        /// Serializes passed value to XML as string.
        /// </summary>
        /// <typeparam name="T">Type of the value to serialize.</typeparam>
        /// <param name="value">Value to serialize.</param>
        /// <returns>Serialized string of passed class.</returns>
        public static string SerializeToStringXml<T>(T value)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            string stringXml;

            using (var sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xmlSerializer.Serialize(writer, value);
                stringXml = sww.ToString();
            }

            return stringXml;
        }

        /// <summary>
        /// Deserializes passed value from XML data.
        /// </summary>
        /// <typeparam name="T">Type of the value to deserialize.</typeparam>
        /// <param name="xmlData">XML data to deserialize.</param>
        /// <returns>Deserialized type of specified class.</returns>
        public static T DeserializeFromStringXml<T>(string xmlData)
        {
            xmlData.ThrowIfNull(nameof(xmlData));

            var xmlSerializer = new XmlSerializer(typeof(T));

            object result;

            using (var sww = new StringReader(xmlData))
            using (XmlReader reader = XmlReader.Create(sww))
            {
                result = xmlSerializer.Deserialize(reader);
            }

            return (T) result;
        }
    }
}
