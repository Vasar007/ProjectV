using System;

namespace ThingAppraiser.Communication
{
    /// <summary>
    /// Provides global message handler. Need to set it before you can use it.
    /// </summary>
    public static class SGlobalMessageHandler
    {
        /// <summary>
        /// Message handler to control communications with service and its components.
        /// </summary>
        public static IMessageHandler MessageHandler { get; set; }

        /// <summary>
        /// Get message from input providing by message handler.
        /// </summary>
        /// <returns>Input message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <see cref="MessageHandler" /> is <c>null</c>.
        /// </exception>
        public static String GetMessage()
        {
            MessageHandler.ThrowIfNull(nameof(MessageHandler));
            return MessageHandler.GetMessage();
        }

        /// <summary>
        /// Print message to the output source providing be message handler.
        /// </summary>
        /// <param name="message">Message to output.</param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="MessageHandler" /> is <c>null</c>.
        /// </exception>
        public static void OutputMessage(String message)
        {
            MessageHandler.ThrowIfNull(nameof(MessageHandler));
            MessageHandler.OutputMessage(message);
        }
    }
}
