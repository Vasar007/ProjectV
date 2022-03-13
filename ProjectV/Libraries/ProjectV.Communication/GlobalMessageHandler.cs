using System;
using System.Threading;
using Acolyte.Assertions;

namespace ProjectV.Communication
{
    /// <summary>
    /// Provides global message handler. Need to set it before you can use it.
    /// </summary>
    public static class GlobalMessageHandler
    {
        /// <summary>
        /// Synchronization object for lock statement.
        /// </summary>
        private static readonly object _syncRoot = new();

        /// <summary>
        /// Message handler to control communications with service and its components.
        /// </summary>
        private static IMessageHandler? MessageHandler { get; set; }

        /// <summary>
        /// Sets new global message handler.
        /// </summary>
        /// <param name="messageHandler">New message handler instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="messageHandler" /> is <c>null</c>.
        /// </exception>
        public static void SetMessageHangler(IMessageHandler messageHandler)
        {
            lock (_syncRoot)
            {
                MessageHandler = messageHandler.ThrowIfNull(nameof(messageHandler));
            }
        }

        /// <summary>
        /// Gets message from input providing by message handler.
        /// </summary>
        /// <returns>Input message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <see cref="MessageHandler" /> is <c>null</c>.
        /// </exception>
        public static string GetMessage()
        {
            lock (_syncRoot)
            {
                return MessageHandler.ThrowIfNull(nameof(MessageHandler)).GetMessage();
            }
        }

        /// <summary>
        /// Prints message to the output source providing be message handler.
        /// </summary>
        /// <param name="message">Message to output.</param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="MessageHandler" /> is <c>null</c>.
        /// </exception>
        public static void OutputMessage(string message)
        {
            lock (_syncRoot)
            {
                MessageHandler.ThrowIfNull(nameof(MessageHandler)).OutputMessage(message);
            }
        }

        /// <summary>
        /// Outputs service information about thread with specified values.
        /// </summary>
        /// <param name="outputObjects">Additional objects to output.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputObjects"/> is <c>null</c>.
        /// </exception>
        public static void PrintThreadInfoWithParams(params object[] outputObjects)
        {
            string message = outputObjects.Length > 0
                             ? ", params: " + string.Join(", ", outputObjects)
                             : string.Empty;
            OutputMessage($"On thread {Environment.CurrentManagedThreadId}" + message);
        }
    }
}
