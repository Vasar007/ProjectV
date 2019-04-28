using System;
using System.Threading.Tasks;
using ThingAppraiser.Appraisers;
using ThingAppraiser.Core;
using ThingAppraiser.Communication;
using ThingAppraiser.Core.Building;
using ThingAppraiser.Crawlers;
using ThingAppraiser.IO.Input;
using ThingAppraiser.IO.Output;
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
            }
            else
            {
                SGlobalMessageHandler.OutputMessage("Work was finished! Press enter to exit...");
            }
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

                //TestShellAsync();
                //TestShellRx();
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occurred in Main method.");
            }

        }

        private static void TestShellAsync()
        {
            try
            {
                SGlobalMessageHandler.MessageHandler = new CConsoleMessageHandler(true);

                var im = new CInputManagerAsync("thing_names.txt");
                im.Add(new CLocalFileReaderAsync(new CSimpleFileReaderAsync()));

                var cm = new CCrawlersManagerAsync(true);
                cm.Add(new CCrawlerTMDBAsync(
                    "f7440a70737103fea00fb6e8352a3533",
                    "https://api.themoviedb.org/3/search/movie",
                    "https://api.themoviedb.org/3/configuration",
                    30,
                    "200",
                    20,
                    1000
                ));

                var am = new CAppraisersManagerAsync(true);
                am.Add(new CFuzzyAppraiserTMDBAsync());

                var om = new COutputManagerAsync("appraised_things_async_tests.csv");
                om.Add(new CLocalFileWriterAsync());

                var shellAsync = new CShellAsync(im, cm, am, om, 10);
                Task<EStatus> task = shellAsync.Run("thing_names.txt");

                task.Wait();
                EStatus status = task.Result;

                if (status == EStatus.Nothing)
                {
                    SGlobalMessageHandler.OutputMessage("Result is empty. Closing...");
                }
                else
                {
                    SGlobalMessageHandler.OutputMessage("Work was finished! Press enter to exit...");
                }
                SGlobalMessageHandler.GetMessage();
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occurred in TestShellAsync method.");
            }
        }

        private static void TestShellRx()
        {
            try
            {
                SGlobalMessageHandler.MessageHandler = new CConsoleMessageHandler(true);

                var im = new CInputManagerRx("thing_names.txt");
                im.Add(new CLocalFileReaderRx(new CSimpleFileReaderRx()));

                var cm = new CCrawlersManagerRx(true);
                cm.Add(new CCrawlerTMDBRx(
                    "f7440a70737103fea00fb6e8352a3533",
                    "https://api.themoviedb.org/3/search/movie",
                    "https://api.themoviedb.org/3/configuration",
                    30,
                    "200",
                    20,
                    1000
                ));

                var am = new CAppraisersManagerRx(true);
                am.Add(new CFuzzyAppraiserTMDBRx());

                var om = new COutputManagerRx("appraised_things_rx_tests.csv");
                om.Add(new CLocalFileWriterRx());

                var shellRx = new CShellRx(im, cm, am, om, 10);

                EStatus status = shellRx.Run("thing_names.txt");

                if (status == EStatus.Nothing)
                {
                    SGlobalMessageHandler.OutputMessage("Result is empty. Closing...");
                }
                else
                {
                    SGlobalMessageHandler.OutputMessage("Work was finished! Press enter to exit...");
                }
                SGlobalMessageHandler.GetMessage();
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occurred in TestShellSync method.");
            }
        }
    }    
}

