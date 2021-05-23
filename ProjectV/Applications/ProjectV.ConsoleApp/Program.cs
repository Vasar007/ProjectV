using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ProjectV.Communication;
using ProjectV.Configuration;
using ProjectV.ContentDirectories;
using ProjectV.Core;
using ProjectV.DataAccessLayer;
using ProjectV.DataAccessLayer.Services.Jobs;
using ProjectV.Logging;
using ProjectV.Models.Internal;
using ProjectV.Models.Internal.Jobs;

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
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor(typeof(Program));


        /// <summary>
        /// Main method of the Console Application which using config to manipulate Shell.
        /// </summary>
        /// <param name="args">Represents the command-line arguments.</param>
        private static async Task MainXDocument(IReadOnlyList<string> args)
        {
            // Show the case when we have a movies to appraise.
            var builderDirector = Shell.CreateBuilderDirector(
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
        private static async Task Run(IReadOnlyList<string> args, Shell shell)
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

                await MainXDocument(args);

#if DEBUG
                TestDbOrmEf();
                TestAutomapper();
                await TestConentDirectories();
#endif

                return ExitCodes.Success;
            }
            catch (Exception ex)
            {
                const string message = "Exception occurred during execution.";
                _logger.Error(ex, message);
                Console.WriteLine($"{message}{Environment.NewLine}{ex}");
                return ExitCodes.Fail;
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
                var jobDbInfo = new JobDbInfo(
                    id: Guid.NewGuid(),
                    name: "JobName",
                    state: 1,
                    result: 2,
                    config: "TaskConfig"
                );

                context.GetJobDbSet().Add(jobDbInfo);

                int count = context.SaveChanges();
                Console.WriteLine($"{count.ToString()} records saved to database.");
            }

            using (var context = new ProjectVDbContext(storageSettings))
            {
                foreach (JobDbInfo jobDbInfo in context.GetJobDbSet())
                {
                    string message =
                        $"Job DB info: {jobDbInfo.Id.ToString()}, {jobDbInfo.Name}, " +
                        $"{jobDbInfo.State.ToString()}, {jobDbInfo.Result.ToString()}, " +
                        $"{jobDbInfo.Config.ToString()}";

                    Console.WriteLine(message);
                }
            }
        }

        private static void TestAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobDbInfo, JobInfo>();
                cfg.CreateMap<JobInfo, JobDbInfo>();

                cfg.CreateMap<Guid, JobId>()
                   .ConvertUsing(guid => JobId.Wrap(guid));
                cfg.CreateMap<JobId, Guid>()
                   .ConvertUsing(jobId => jobId.Value);
            });
            config.AssertConfigurationIsValid();

            var mapper = config.CreateMapper();

            var jobDbInfo = new JobDbInfo(
                id: Guid.NewGuid(),
                name: "JobName",
                state: 1,
                result: 2,
                config: "TaskConfig"
            );
            string message =
                $"Job DB info: {jobDbInfo.Id.ToString()}, {jobDbInfo.Name}, " +
                $"{jobDbInfo.State.ToString()}, {jobDbInfo.Result.ToString()}, " +
                $"{jobDbInfo.Config.ToString()}";

            Console.WriteLine(message);

            var jobInfo = mapper.Map<JobInfo>(jobDbInfo);
            message =
                $"Job info: {jobInfo.Id.ToString()}, {jobInfo.Name}, " +
                $"{jobInfo.State.ToString()}, {jobInfo.Result.ToString()}, " +
                $"{jobInfo.Config.ToString()}";

            Console.WriteLine(message);

            jobDbInfo = mapper.Map<JobDbInfo>(jobInfo);
            message =
                $"Job DB info: {jobDbInfo.Id.ToString()}, {jobDbInfo.Name}, " +
                $"{jobDbInfo.State.ToString()}, {jobDbInfo.Result.ToString()}, " +
                $"{jobDbInfo.Config.ToString()}";

            Console.WriteLine(message);
        }

        private static async Task TestConentDirectories()
        {
            IReadOnlyDictionary<string, IReadOnlyList<string>> result =
                await ContentFinder.FindContentForDirAsync(
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
