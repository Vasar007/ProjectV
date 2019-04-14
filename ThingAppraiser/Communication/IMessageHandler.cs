using System;

namespace ThingAppraiser.Communication
{
    /// <summary>
    /// Common interface of message handlers to interact with input and output.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Gets message from input.
        /// </summary>
        /// <returns>String read from input source.</returns>
        String GetMessage();

        /// <summary>
        /// Writes message to output.
        /// </summary>
        /// <param name="message">Message to write.</param>
        void OutputMessage(String message);
    }
}
