using System;
using System.Threading;

namespace ThingAppraiser.Communication
{
    /// <summary>
    /// Provides global message handler. Need to set it before you can use it.
    /// </summary>
    public static class SGlobalMessageHandler
    {
        /// <summary>
        /// Synchronization object for lock statement.
        /// </summary>
        private static readonly Object s_lockObject = new Object();

        /// <summary>
        /// Value of the message handler property.
        /// </summary>
        private static IMessageHandler s_messageHandler;

        /// <summary>
        /// Message handler to control communications with service and its components.
        /// </summary>
        public static IMessageHandler MessageHandler
        {
            get
            {
                lock (s_lockObject)
                {
                    return s_messageHandler;
                }
            }
            set
            {
                lock (s_lockObject)
                {
                    s_messageHandler = value;
                }
            }
        }

        /// <summary>
        /// Gets message from input providing by message handler.
        /// </summary>
        /// <returns>Input message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <see cref="MessageHandler" /> is <c>null</c>.
        /// </exception>
        public static String GetMessage()
        {
            lock (s_lockObject)
            {
                MessageHandler.ThrowIfNull(nameof(MessageHandler));
                return MessageHandler.GetMessage();
            }
        }

        /// <summary>
        /// Prints message to the output source providing be message handler.
        /// </summary>
        /// <param name="message">Message to output.</param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="MessageHandler" /> is <c>null</c>.
        /// </exception>
        public static void OutputMessage(String message)
        {
            lock (s_lockObject)
            {
                MessageHandler.ThrowIfNull(nameof(MessageHandler));
                MessageHandler.OutputMessage(message);
            }
        }

        /// <summary>
        /// Outputs service information about thread with specified values.
        /// </summary>
        /// <param name="outputObjects">Additional objects to output.</param>
        public static void PrintThreadInfoWithParams(params Object[] outputObjects)
        {
            String message = outputObjects.Length > 0
                             ? ", params: " + String.Join(", ", outputObjects)
                             : String.Empty;
            OutputMessage($"On thread {Thread.CurrentThread.ManagedThreadId}" + message);
        }
    }
}
