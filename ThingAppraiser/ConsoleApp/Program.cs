using System;
using ThingAppraiser.Core;
using ThingAppraiser.Communication;
using ThingAppraiser.Core.Building;
using ThingAppraiser.Logging;

namespace ConsoleApp
{
    /// <summary>
    /// Class which starts service and interact with it through console.
    /// </summary>
    public static class SProgram
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceWithName(nameof(SProgram));


        /// <summary>
        /// Main method of the Console Application which using config to manipulate Shell.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        private static void Main_XDocument(String[] args)
        {
            // Show the case when we have a movies to appraise.
            CShell.ShellBuilderDirector.ChangeShellBuilder(
                new CShellBuilderFromXDocument(CXMLConfigCreator.CreateDefaultXMLConfig())
            );
            var shell = CShell.ShellBuilderDirector.MakeShell();
            Run(args, shell);
        }

        /// <summary>
        /// Main method of the Console Application which using config to manipulate Shell.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        private static void Main_Config(String[] args)
        {
            // Show the case when we have a movies to appraise.
            var shell = CShell.ShellBuilderDirector.MakeShell();
            Run(args, shell);
        }

        /// <summary>
        /// Method with logic of execution the Console Application which created shell to process data.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        /// <param name="shell">Represents the main manager of the library.</param>
        private static void Run(String[] args, CShell shell)
        {
            EStatus status;
            if (args.Length == 1)
            {
                status = shell.Run(args[0]);
            }
            else
            {
                SGlobalMessageHandler.OutputMessage(
                    "Enter filename which contains the Things: "
                );
                status = shell.Run(SGlobalMessageHandler.GetMessage());
            }

            if (status == EStatus.Nothing)
            {
                SGlobalMessageHandler.OutputMessage("Result is empty. Closing...");
                SGlobalMessageHandler.GetMessage();
                return;
            }

            SGlobalMessageHandler.OutputMessage("Work was finished! Press enter to exit...");
            SGlobalMessageHandler.GetMessage();
        }

        /// <summary>
        /// Console application start point.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        private static void Main(String[] args)
        {
            try
            {
                Main_Config(args);
                //Main_XDocument(args);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occurred in Main method.");
            }
        }
    }    
}

