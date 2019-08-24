using System;

namespace ThingAppraiser.Models.Exceptions
{
    [Serializable]
    public sealed class CannotGetTmdbConfigException : Exception
    {
        public CannotGetTmdbConfigException()
        {
        }

        public CannotGetTmdbConfigException(string message)
            : base(message)
        {
        }

        public CannotGetTmdbConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
