using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThingAppraiser.Models.Exceptions
{
    /// <summary>
    /// The exception that is thrown when several arguments provided to a method are not valid.
    /// </summary>
    [Serializable]
    public sealed class MultipleArgumentException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        public MultipleArgumentException()
        {
        }

        /// <summary>
        /// Creates the exception with description.
        /// </summary>
        /// <param name="message">The exception description.</param>
        public MultipleArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates the exception with description and inner cause.
        /// </summary>
        /// <param name="message">The exception description.</param>
        /// <param name="innerException">The exception inner cause.</param>
        public MultipleArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates the exception with description, inner cause and parameters names.
        /// </summary>
        /// <param name="message">The exception description.</param>
        /// <param name="paramNames">The names of parameters that cause exception.</param>
        public MultipleArgumentException(string message, params string[] paramNames)
            : base(FormatMessage(message, paramNames))
        {
        }

        /// <summary>
        /// Creates the exception with description, inner cause and parameters names.
        /// </summary>
        /// <param name="message">The exception description.</param>
        /// <param name="paramNames">The names of parameters that cause exception.</param>
        public MultipleArgumentException(string message, IEnumerable<string> paramNames)
            : base(FormatMessage(message, paramNames))
        {
        }

        /// <summary>
        /// Creates the exception with description, inner cause and parameters names.
        /// </summary>
        /// <param name="message">The exception description.</param>
        /// <param name="innerException">The exception inner cause.</param>
        /// <param name="paramNames">The names of parameters that cause exception.</param>
        public MultipleArgumentException(string message, Exception innerException,
            params string[] paramNames)
            : base(FormatMessage(message, paramNames), innerException)
        {
        }

        /// <summary>
        /// Creates the exception with description, inner cause and parameters names.
        /// </summary>
        /// <param name="message">The exception description.</param>
        /// <param name="innerException">The exception inner cause.</param>
        /// <param name="paramNames">The names of parameters that cause exception.</param>
        public MultipleArgumentException(string message, Exception innerException,
            IEnumerable<string> paramNames)
            : base(FormatMessage(message, paramNames), innerException)
        {
        }

        private static string FormatMessage(string message, IEnumerable<string> paramNames)
        {
            string formattedParamNames = string.Join(
                ", ",
                paramNames.Select(paramName => $"'{paramName}'")
            );

            return $"{message} (Parameters: {formattedParamNames})";
        }
    }
}
