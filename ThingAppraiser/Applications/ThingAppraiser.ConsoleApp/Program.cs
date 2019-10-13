using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Core;
using ThingAppraiser.Communication;
using ThingAppraiser.Configuration;
using ThingAppraiser.ContentDirectories;
using ThingAppraiser.DAL.EntityFramework;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
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

                await MainXDocument(args);
                //TestEntityFrameworkCore();
                //TestConentDirectories();
                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception occurred in {nameof(Main)} method.");
                return -1;
            }
            finally
            {
                _logger.PrintFooter("Console client application stopped.");
            }
        }

        private static void TestEntityFrameworkCore()
        {
            using (var context = new ThingAppraiserContext())
            {
                var tmdbMovie = new TmdbMovieInfo(
                    thingId:     1,
                    title:       "Test",
                    voteCount:   100,
                    voteAverage: 10.0,
                    overview:    "Overview",
                    releaseDate: DateTime.UtcNow,
                    popularity:  50.0,
                    adult:       true,
                    genreIds:    new List<int> { 1, 2, 4 },
                    posterPath:  "None"
                );

                context.Add(tmdbMovie);

                int count = context.SaveChanges();
                Console.WriteLine($"{count.ToString()} records saved to database.");
            }

            using (var context = new ThingAppraiserContext())
            {
                foreach (TmdbMovieInfo tmdbMovie in context.TmdbMovies)
                {
                    Console.WriteLine(
                        $"TMDB movie: {tmdbMovie.ThingId.ToString()}, {tmdbMovie.Title}, " +
                        $"{tmdbMovie.Popularity.ToString()}"
                    );
                }
            }
        }

        private static void TestConentDirectories()
        {
            IReadOnlyDictionary<string, IReadOnlyList<string>> result = 
                ContentFinder.findContentForDir(
                    @"C:\Users\vasar\Documents\GitHub",
                    ContentFinder.ContentType.Text
                )
                .ToReadOnlyDictionary(
                    tuple => tuple.Item1,
                    tuple => tuple.Item2.ToReadOnlyList()
                );

            foreach ((string directoryName, IReadOnlyList<string> files) in result)
            {
                Console.WriteLine(directoryName);

                Console.WriteLine(
                    $"{string.Join($"{Environment.NewLine}", files)}{Environment.NewLine}"
                );
            }
        }
    }
}
