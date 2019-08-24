using System;

namespace ThingAppraiser.Models.Exceptions
{
    /// <summary>
    /// Provides library specific exception to show that client could not get config from TMDb
    /// service.
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
        /// <param name="message">Exception description.</param>
        public CannotGetTmdbConfigException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates the exception with description and inner cause.
        /// </summary>
        /// <param name="message">Exception description.</param>
        /// <param name="innerException">Exception inner cause.</param>
        public CannotGetTmdbConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
