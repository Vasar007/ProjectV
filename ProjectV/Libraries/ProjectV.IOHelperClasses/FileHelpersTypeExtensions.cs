using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acolyte.Assertions;

namespace FileHelpers
{
    /// <see href="http://stackoverflow.com/questions/3975741/column-headers-in-csv-using-filehelpers-library/8258420#8258420" />
    /// <remarks>
    /// It is unofficial extension of FileHelper library to process CSV files with headers
    /// conveniently.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FieldTitleAttribute : Attribute
    {
        public string Name { get; private set; }


        public FieldTitleAttribute(string name)
        {
            Name = name.ThrowIfNull(nameof(name));
        }
    }

    public static class FileHelpersTypeExtensions
    {
        public static IEnumerable<string> GetFieldTitles(this Type type)
        {
            var fields = from field in type.GetFields(
                BindingFlags.GetField |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance)
                         where field.IsFileHelpersField()
                         select field;

            return from field in fields
                   let attrs = field.GetCustomAttributes(true)
                   let order = attrs.OfType<FieldOrderAttribute>().Single().GetOrder()
                   let title = attrs.OfType<FieldTitleAttribute>().Single().Name
                   orderby order
                   select title;
        }

        public static string GetCsvHeader(this Type type)
        {
            return string.Join(",", type.GetFieldTitles());
        }

        private static bool IsFileHelpersField(this FieldInfo field)
        {
            return field.GetCustomAttributes(true).OfType<FieldOrderAttribute>().Any();
        }

        private static int GetOrder(this FieldOrderAttribute attribute)
        {
            // Hack cos FieldOrderAttribute.Order is internal (why?)
            var pi = typeof(FieldOrderAttribute).GetProperty("Order");
            if (pi is null)
                return 0;

            object? value = pi.GetValue(attribute, null);
            if (value is null)
                return 0;

            return (int) value;
        }
    }
}
