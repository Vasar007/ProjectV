using System;

namespace ProjectV.Models.Exceptions
{
    /// <summary>
    /// The exception that is thrown when client could not get config from the TMDb service.
    /// </summary>
    [Serializable]
    public sealed class CannotGetTmdbConfigException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        public CannotGetTmdbConfigException()
        {
        }

        /// <summary>
        /// Creates the exception with description.
        /// </summary>
        /// <param name="message">The exception description.</param>
        public CannotGetTmdbConfigException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates the exception with description and inner cause.
        /// </summary>
        /// <param name="message">The exception description.</param>
        /// <param name="innerException">The exception inner cause.</param>
        public CannotGetTmdbConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
