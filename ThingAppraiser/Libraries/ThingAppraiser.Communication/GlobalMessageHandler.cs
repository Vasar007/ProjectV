using System;
using System.Threading;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.Communication
{
    /// <summary>
    /// Provides global message handler. Need to set it before you can use it.
    /// </summary>
    public static class GlobalMessageHandler
    {
        /// <summary>
        /// Synchronization object for lock statement.
        /// </summary>
        private static readonly object _syncRoot = new object();

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
                messageHandler.ThrowIfNull(nameof(messageHandler));
                MessageHandler = messageHandler;
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
                MessageHandler.ThrowIfNull(nameof(MessageHandler));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return MessageHandler.GetMessage();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
                MessageHandler.ThrowIfNull(nameof(MessageHandler));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                MessageHandler.OutputMessage(message);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        /// <summary>
        /// Outputs service information about thread with specified values.
        /// </summary>
        /// <param name="outputObjects">Additional objects to output.</param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="outputObjects" /> is <c>null</c>.
        /// </exception>
        public static void PrintThreadInfoWithParams(params object[] outputObjects)
        {
            string message = outputObjects.Length > 0
                             ? ", params: " + string.Join(", ", outputObjects)
                             : string.Empty;
            OutputMessage($"On thread {Thread.CurrentThread.ManagedThreadId}" + message);
        }
    }
}
