using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.Core.Building
{
    public static class ConfigContract
    {
        public static IReadOnlyList<string> AvailableMessageHandlers { get; } = new List<string>
        {
            ConfigOptions.MessageHandlers.ConsoleMessageHandlerName
        };

        public static IReadOnlyList<string> AvailableInput { get; } = new List<string>
        {
            ConfigOptions.Inputters.LocalFileReaderSimpleName,
            ConfigOptions.Inputters.LocalFileReaderFilterName,
            ConfigOptions.Inputters.GoogleDriveReaderSimpleName,
            ConfigOptions.Inputters.GoogleDriveReaderFilterName
        };

        /// <summary>
        /// Service names list with which ThingAppraiser can be interact.
        /// </summary>
        /// <remarks>
        /// There are contract that service names (not beautified) must be equal to crawler name.
        /// </remarks>
        public static IReadOnlyList<string> AvailableServices { get; } = new List<string>
        {
            ConfigOptions.Crawlers.TmdbCrawlerName,
            ConfigOptions.Crawlers.OmdbCrawlerName,
            ConfigOptions.Crawlers.SteamCrawlerName
        };

        public static IReadOnlyList<string> AvailableAppraisals { get; } = new List<string>
        {
            ConfigOptions.Appraisers.TmdbAppraiserCommonName,
            ConfigOptions.Appraisers.TmdbAppraiserFuzzyName,
            ConfigOptions.Appraisers.OmdbAppraiserCommonName,
            ConfigOptions.Appraisers.SteamAppraiserCommonName
        };

        public static IReadOnlyList<string> AvailableOutput { get; } = new List<string>
        {
            ConfigOptions.Outputters.LocalFileWriterName,
            ConfigOptions.Outputters.GoogleDriveWriterName
        };

        public static IReadOnlyList<string> AvailableBeautifiedServices { get; } = new List<string>
        {
            ConfigOptions.BeautifiedServices.TmdbServiceName,
            ConfigOptions.BeautifiedServices.OmdbServiceName,
            ConfigOptions.BeautifiedServices.SteamServiceName
        };

        public static IReadOnlyList<string> AvailableServicesToLower { get; } =
            AvailableServices.Select(service => service.ToLowerInvariant()).ToList();


        /// <summary>
        /// Checks availability different service components which specifid in 
        /// <see cref="ConfigContract" />.
        /// </summary>
        /// <param name="item">Value to check.</param>
        /// <param name="availableList">Available list from <see cref="ConfigContract" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="ConfigContract" /> list doesn't contain passed <paramref name="item" />. -or-
        /// <paramref name="item" /> presents empty string.
        /// </exception>
        public static void CheckAvailability(string item, IReadOnlyList<string> availableList)
        {
            item.ThrowIfNullOrEmpty(nameof(item));
            
            if (!availableList.Contains(item))
            {
                throw new ArgumentException(
                    $"Config data contains invalid items: {item}.", nameof(item)
                );
            }
        }

        public static bool ContainsService(string serviceName)
        {
            serviceName.ThrowIfNullOrEmpty(nameof(serviceName));

            serviceName = serviceName.ToLowerInvariant();
            return AvailableServicesToLower.Contains(serviceName);
        }

        public static string GetProperServiceName(string serviceName)
        {
            serviceName.ThrowIfNullOrEmpty(nameof(serviceName));

            serviceName = serviceName.ToLowerInvariant();
            int index = AvailableServicesToLower.FindIndex(
                service => service.IsEqualWithInvariantCulture(serviceName)
            );

            return AvailableServices[index];
        }
    }
}
