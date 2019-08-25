using System;
using ThingAppraiser.Core;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.ConsoleApp
{
    /// <summary>
    /// Class which starts service and interact with it through console.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(Program));


        /// <summary>
        /// Main method of the Console Application which using config to manipulate Shell.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        private static void MainXDocument(string[] args)
        {
            // Show the case when we have a movies to appraise.
            var builderDirector = ShellAsync.CreateBuilderDirector(
                XmlConfigCreator.CreateDefaultXmlConfigAsXDocument()
            );
            var shell = builderDirector.MakeShell();
            Run(args, shell);
        }

        /// <summary>
        /// Method with logic of execution the Console Application which created shell to process
        /// data.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        /// <param name="shell">Represents the main manager of the library.</param>
        private static void Run(string[] args, ShellAsync shell)
        {
            ServiceStatus status;
            if (args.Length == 1)
            {
                status = shell.Run(args[0]).Result;
            }
            else
            {
                GlobalMessageHandler.OutputMessage(
                    "Enter filename which contains the Things: "
                );
                status = shell.Run(GlobalMessageHandler.GetMessage()).Result;
            }

            if (status == ServiceStatus.Nothing)
            {
                GlobalMessageHandler.OutputMessage("Result is empty. Closing...");
            }
            else
            {
                GlobalMessageHandler.OutputMessage("Work was finished! Press enter to exit...");
            }
            GlobalMessageHandler.GetMessage();
        }

        /// <summary>
        /// Console application start point.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        private static void Main(string[] args)
        {
            try
            {
                _logger.PrintHeader("Console client application started.");

                MainXDocument(args);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception occurred in {nameof(Main)} method.");
            }
            finally
            {
                _logger.PrintFooter("Console client application stopped.");
            }
        }
    }
}
