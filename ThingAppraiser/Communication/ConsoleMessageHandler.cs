using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Communication
{
    /// <summary>
    /// Message handler to interact with console.
    /// </summary>
    public class CConsoleMessageHandler : IMessageHandler
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CConsoleMessageHandler>();


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CConsoleMessageHandler(Boolean setUnicode)
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
                    s_logger.Error(ex, "Cannot set encoding for console, most likely because IO " +
                                       "stream is redirected.");
                }
            }
        }

        /// <summary>
        /// Helper method to display crawlers results.
        /// </summary>
        /// <param name="results">Data to show.</param>
        public static void PrintResultsToConsole(IEnumerable<IEnumerable<CBasicInfo>> results)
        {
            foreach (IEnumerable<CBasicInfo> result in results)
            {
                foreach (CBasicInfo entity in result)
                {
                    Console.WriteLine(JToken.FromObject(entity));
                }
            }
        }

        /// <summary>
        /// Helper method to display appraisers results.
        /// </summary>
        /// <param name="ratings">Data to show.</param>
        public static void PrintRatingsToConsole(IEnumerable<IEnumerable<CResultInfo>> ratings)
        {
            foreach (IEnumerable<CResultInfo> rating in ratings)
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
        public String GetMessage()
        {
            String line = Console.ReadLine();
            line.ThrowIfNull(nameof(line));
            return line;
        }

        /// <summary>
        /// Writes message to the standard output stream.
        /// </summary>
        /// <param name="message">The value to write.</param>
        public void OutputMessage(String message)
        {
            Console.WriteLine(message);
        }
        
        #endregion
    }
}
