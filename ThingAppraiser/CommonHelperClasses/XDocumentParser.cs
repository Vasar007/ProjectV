using System;
using System.Xml.Linq;

namespace ThingAppraiser
{
    /// <summary>
    /// XML config parser which provides methods with deferred execution to work with it.
    /// </summary>
    public class CXDocumentParser
    {
        /// <summary>
        /// Keeps passed document inside instance.
        /// </summary>
        private readonly XDocument _document;


        /// <summary>
        /// Creates XML parser and saves pins document to created instance.
        /// </summary>
        /// <param name="document">XML config to parse.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="document">document</paramref> or it property <c>Root</c> is <c>null</c>.
        /// </exception>
        public CXDocumentParser(XDocument document)
        {
            document.ThrowIfNull(nameof(document));
            document.Root.ThrowIfNull(nameof(document.Root));

            _document = document;
        }

        /// <summary>
        /// Gets attribute value in passed element.
        /// </summary>
        /// <param name="element">Element to process.</param>
        /// <param name="attribute">Name of the attribute.</param>
        /// <returns>String value if found attribute, otherwise empty string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element">element</paramref> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attribute">attribute</paramref> is <c>null</c> or presents empty string.
        /// </exception>
        public static String GetAttributeValue(XElement element, String attribute)
        {
            element.ThrowIfNull(nameof(element));
            attribute.ThrowIfNullOrEmpty(nameof(attribute));

            String value = element.Attribute(attribute)?.Value;
            return value ?? String.Empty;
        }

        /// <summary>
        /// Gets attribute in passed element and converts to specified type.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <param name="element">Element to process.</param>
        /// <param name="attribute">Name of the attribute.</param>
        /// <returns>
        /// Converted value if found attribute, otherwise exception could be thrown.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element">element</paramref> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attribute">attribute</paramref> is <c>null</c> or presents empty string.
        /// </exception>
        public static T GetAttributeValue<T>(XElement element, String attribute)
            where T : IConvertible
        {
            String stringValue = GetAttributeValue(element, attribute);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Finds subelement in specified element.
        /// </summary>
        /// <param name="element">Element to process.</param>
        /// <param name="subelement">Name of the subelement to find.</param>
        /// <returns>First found subelement which can be <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element">element</paramref> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="subelement">subelement</paramref> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public static XElement FindSubelement(XElement element, String subelement)
        {
            element.ThrowIfNull(nameof(element));
            subelement.ThrowIfNullOrEmpty(subelement);

            return element.Element(subelement);
        }

        /// <summary>
        /// Finds element in XML document and gets attribute value.
        /// </summary>
        /// <param name="element">Name of the element to find.</param>
        /// <param name="attribute">Name of the attribute.</param>
        /// <returns>String value if found attribute, otherwise empty string.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="element">element</paramref> or
        /// <paramref name="attribute">attribute</paramref> is <c>null</c> or presents empty string.
        /// </exception>
        public String GetAttributeValue(String element, String attribute)
        {
            element.ThrowIfNullOrEmpty(nameof(element));
            attribute.ThrowIfNullOrEmpty(nameof(attribute));

            String value = _document.Root.Element(element)?.Attribute(attribute)?.Value;
            return value ?? String.Empty;
        }

        /// <summary>
        /// Finds element in XML document, gets attribute value and converts to specified type.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <param name="element">Name of the element to find.</param>
        /// <param name="attribute">Name of the attribute.</param>
        /// <returns>
        /// Converted value if found attribute, otherwise exception could be thrown.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="element">element</paramref> or
        /// <paramref name="attribute">attribute</paramref> is <c>null</c> or presents empty string.
        /// </exception>
        public T GetAttributeValue<T>(String element, String attribute)
            where T : IConvertible
        {
            String stringValue = GetAttributeValue(element, attribute);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Finds element in XML document.
        /// </summary>
        /// <param name="element">Name of the element to find.</param>
        /// <returns>First found element which can be <c>null</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="element">element</paramref> is <c>null</c> or presents empty string.
        /// </exception>
        public XElement FindElement(String element)
        {
            element.ThrowIfNullOrEmpty(nameof(element));

            return _document.Root.Element(element);
        }
    }
}
