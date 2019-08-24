using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ThingAppraiser
{
    /// <summary>
    /// XML config parser which provides methods with deferred execution to work with it.
    /// </summary>
    public sealed class XDocumentParser
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
        /// <paramref name="document" /> or it property <c>Root</c> is <c>null</c>.
        /// </exception>
        public XDocumentParser(XDocument document)
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
        /// <returns>string value if found attribute, otherwise empty string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c> -or-
        /// <paramref name="attribute" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attribute" /> presents empty string.
        /// </exception>
        public static string GetAttributeValue(XElement element, string attribute)
        {
            element.ThrowIfNull(nameof(element));
            attribute.ThrowIfNullOrEmpty(nameof(attribute));

            string value = element.Attribute(attribute)?.Value;
            return value ?? string.Empty;
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
        /// <paramref name="element" /> is <c>null</c> -or-
        /// <paramref name="attribute" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attribute" /> presents empty string.
        /// </exception>
        public static T GetAttributeValue<T>(XElement element, string attribute)
            where T : IConvertible
        {
            string stringValue = GetAttributeValue(element, attribute);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Gets element value.
        /// </summary>
        /// <param name="element">Element to process.</param>
        /// <returns>string value if element is valid, otherwise empty string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c>.
        /// </exception>
        public static string GetElementValue(XElement element)
        {
            element.ThrowIfNull(nameof(element));

            string value = element.Value;
            return value ?? string.Empty;
        }

        /// <summary>
        /// Gets element value and converts to specified type.
        /// </summary>
        /// <typeparam name="T">Type to convert.</typeparam>
        /// <param name="element">Element to process.</param>
        /// <returns>
        /// Converted value if element is valid, otherwise exception could be thrown.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c>.
        public static T GetElementValue<T>(XElement element)
            where T : IConvertible
        {
            string stringValue = GetElementValue(element);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Finds subelement (child element) in specified element.
        /// </summary>
        /// <param name="element">Element to process.</param>
        /// <param name="subelement">Name of the subelement to find.</param>
        /// <returns>First found subelement which can be <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c> -or-
        /// <paramref name="subelement" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="subelement" /> presents empty
        /// string.
        /// </exception>
        public static XElement FindSubelement(XElement element, string subelement)
        {
            element.ThrowIfNull(nameof(element));
            subelement.ThrowIfNullOrEmpty(subelement);

            return element.Element(subelement);
        }

        /// <summary>
        /// Finds subelements (child elements) in specified element.
        /// </summary>
        /// <param name="element">Element to process.</param>
        /// <returns>Returns collection of the child elements of passed element.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c>.
        /// </exception>
        public static IEnumerable<XElement> FindSubelements(XElement element)
        {
            element.ThrowIfNull(nameof(element));

            return element.Elements();
        }

        /// <summary>
        /// Finds element in XML document and gets attribute value.
        /// </summary>
        /// <param name="element">Name of the element to find.</param>
        /// <param name="attribute">Name of the attribute.</param>
        /// <returns>string value if found attribute, otherwise empty string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c> -or-
        /// <paramref name="attribute" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> or <paramref name="attribute" /> presents empty string.
        /// </exception>
        public string GetAttributeValue(string element, string attribute)
        {
            element.ThrowIfNullOrEmpty(nameof(element));
            attribute.ThrowIfNullOrEmpty(nameof(attribute));

            string value = _document.Root.Element(element)?.Attribute(attribute)?.Value;
            return value ?? string.Empty;
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c> -or-
        /// <paramref name="attribute" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> or <paramref name="attribute" /> presents empty string.
        /// </exception>
        public T GetAttributeValue<T>(string element, string attribute)
            where T : IConvertible
        {
            string stringValue = GetAttributeValue(element, attribute);
            return (T) Convert.ChangeType(stringValue, typeof(T));
        }

        /// <summary>
        /// Finds element in XML document.
        /// </summary>
        /// <param name="element">Name of the element to find.</param>
        /// <returns>First found element which can be <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="element" /> presents empty string.
        /// </exception>
        public XElement FindElement(string element)
        {
            element.ThrowIfNullOrEmpty(nameof(element));

            return _document.Root.Element(element);
        }
    }
}
