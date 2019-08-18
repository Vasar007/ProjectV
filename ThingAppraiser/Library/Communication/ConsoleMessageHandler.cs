using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Communication
{
    /// <summary>
    /// Message handler to interact with console.
    /// </summary>
    public class ConsoleMessageHandler : IMessageHandler
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ConsoleMessageHandler>();


        /// <summary>
        /// Initializes message handler and sets unicode encoding if required.
        /// </summary>
        public ConsoleMessageHandler(bool setUnicode)
        {
            if (setUnicode)
            {
                try
                {
                    Console.InputEncoding = Encoding.Unicode;
                    Console.OutputEncoding = Encoding.Unicode;
                }
                catch (IOException ex)
                {
                    _logger.Error(ex, "Cannot set encoding for console, most likely because IO " +
                                       "stream is redirected.");
                }
            }
        }

        /// <summary>
        /// Helper method to display crawlers results.
        /// </summary>
        /// <param name="results">Data to show.</param>
        public static void PrintResultsToConsole(IEnumerable<IEnumerable<BasicInfo>> results)
        {
            foreach (IEnumerable<BasicInfo> result in results)
            {
                foreach (BasicInfo entity in result)
                {
                    Console.WriteLine(JToken.FromObject(entity));
                }
            }
        }

        /// <summary>
        /// Helper method to display appraisers results.
        /// </summary>
        /// <param name="ratings">Data to show.</param>
        public static void PrintRatingsToConsole(IEnumerable<IEnumerable<ResultInfo>> ratings)
        {
            foreach (IEnumerable<ResultInfo> rating in ratings)
            {
                foreach (var (id, value, name) in rating)
                {
                    Console.WriteLine($"{id} has rating {name} = {value}.");
                }
            }
        }

        #region IMessageHandler Implementation

        /// <summary>
        /// Reads message from the standard input stream.
        /// </summary>
        /// <returns>The next line of characters from the input stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// Read input line is <c>null</c>.
        /// </exception>
        public string GetMessage()
        {
            string line = Console.ReadLine();
            line.ThrowIfNull(nameof(line));
            return line;
        }

        /// <summary>
        /// Writes message to the standard output stream.
        /// </summary>
        /// <param name="message">The value to write.</param>
        public void OutputMessage(string message)
        {
            Console.WriteLine(message);
        }
        
        #endregion
    }
}
