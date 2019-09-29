using System;
using System.Runtime.Serialization;

namespace ThingAppraiser.Exceptions
{
    /// <summary>
    /// The exception that is thrown when processing method encounters task in faulted state.
    /// </summary>
    [Serializable]
    public sealed class TaskFaultedException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        public TaskFaultedException()
        {
        }

        /// <summary>
        /// Creates the exception with description.
        /// </summary>
        /// <param name="message">The exception description.</param>
        public TaskFaultedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates the exception with description and inner cause.
        /// </summary>
        /// <param name="message">The exception description.</param>
        /// <param name="innerException">The exception inner cause.</param>
        public TaskFaultedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception class with serialized data.
        /// </summary>
        /// <param name="serializationInfo">
        /// The <see cref="SerializationInfo" /> that holds the serialized object data about the
        /// exception being thrown.
        /// </param>
        /// <param name="streamingContext">
        /// The <see cref="StreamingContext" /> that contains contextual information about the
        /// source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">The info parameter is <c>null</c>.</exception>
        /// <exception cref="SerializationException">
        /// The class name is <c>null</c> or <see cref="Exception.HResult" /> is zero (0).
        /// </exception>
        private TaskFaultedException(SerializationInfo serializationInfo,
            StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
