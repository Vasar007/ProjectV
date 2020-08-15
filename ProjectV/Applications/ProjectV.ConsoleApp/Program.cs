﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectV.Core;
using ProjectV.Communication;
using ProjectV.Configuration;
using ProjectV.ContentDirectories;
using ProjectV.DataAccessLayer.EntityFramework;
using ProjectV.Logging;
using ProjectV.Models.Internal;
using ProjectV.DataAccessLayer;
using LinqToDB;

namespace ProjectV.ConsoleApp
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
        private static async Task MainXDocument(IReadOnlyList<string> args)
        {
            // Show the case when we have a movies to appraise.
            var builderDirector = ShellAsync.CreateBuilderDirector(
                XmlConfigCreator.CreateDefaultXmlConfigAsXDocument()
            );
            var shell = builderDirector.MakeShell();
            await Run(args, shell);
        }

        /// <summary>
        /// Method with logic of execution the Console Application which created shell to process
        /// data.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        /// <param name="shell">Represents the main manager of the library.</param>
        private static async Task Run(IReadOnlyList<string> args, ShellAsync shell)
        {
            ServiceStatus status;
            if (args.Count == 1)
            {
                status = await shell.Run(args[0]);
            }
            else
            {
                GlobalMessageHandler.OutputMessage(
                    "Enter filename which contains the Things: "
                );
                status = await shell.Run(GlobalMessageHandler.GetMessage());
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
        private static async Task<int> Main(string[] args)
        {
            try
            {
                _logger.PrintHeader("Console client application started.");

                //await MainXDocument(args);
                TestDbOrmEf();
                //await TestConentDirectories();
                return 0;
            }
            catch (Exception ex)
            {
                const string message = "Exception occurred during execution.";
                _logger.Error(ex, message);
                Console.WriteLine($"{message}{Environment.NewLine}{ex}");
                return -1;
            }
            finally
            {
                _logger.PrintFooter("Console client application stopped.");
            }
        }

        #region Debug-only Code

#if DEBUG

        private static void TestDbOrmEf()
        {
            var storageSettings = ConfigOptions.GetOptions<DatabaseOptions>();
            using (var context = new ProjectVDbContext(storageSettings))
            {
                var jobInfo = new JobDbInfo(
                    id: Guid.NewGuid(),
                    name: "JobName",
                    state: 1,
                    result: 2,
                    config: "TaskConfig"
                );

                context.GetJobDbSet().Add(jobInfo);

                int count = context.SaveChanges();
                Console.WriteLine($"{count.ToString()} records saved to database.");
            }

            using (var context = new ProjectVDbContext(storageSettings))
            {
                foreach (JobDbInfo jobInfo in context.GetJobDbSet())
                {
                    string message =
                        $"Job info: {jobInfo.Id.ToString()}, {jobInfo.Name}, " +
                        $"{jobInfo.State.ToString()}, {jobInfo.Result.ToString()}, " +
                        $"{jobInfo.Config.ToString()}";

                    Console.WriteLine(message);
                }
            }
        }

        private static async Task TestConentDirectories()
        {
            IReadOnlyDictionary<string, IReadOnlyList<string>> result = await ContentFinder
                .FindContentForDirAsync(
                    @"C:\Users\vasar\Documents\GitHub",
                    ContentModels.ContentType.Text
                );

            foreach ((string directoryName, IReadOnlyList<string> files) in result)
            {
                Console.WriteLine(directoryName);

                Console.WriteLine(
                    $"{string.Join($"{Environment.NewLine}", files)}{Environment.NewLine}"
                );
            }
        }

#endif

        #endregion
    }
}
