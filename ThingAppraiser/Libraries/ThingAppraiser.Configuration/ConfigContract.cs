using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using Acolyte.Collections;
using Acolyte.Common;

namespace ThingAppraiser.Configuration
{
    public static class ConfigContract
    {
        public static IReadOnlyList<string> AvailableMessageHandlers { get; } = new List<string>
        {
            ConfigNames.MessageHandlers.ConsoleMessageHandlerName
        };

        public static IReadOnlyList<string> AvailableInput { get; } = new List<string>
        {
            ConfigNames.Inputters.LocalFileReaderSimpleName,
            ConfigNames.Inputters.LocalFileReaderFilterName,
            ConfigNames.Inputters.GoogleDriveReaderSimpleName,
            ConfigNames.Inputters.GoogleDriveReaderFilterName
        };

        /// <summary>
        /// Service names list with which ThingAppraiser can be interact.
        /// </summary>
        /// <remarks>
        /// There are contract that service names (not beautified) must be equal to crawler name.
        /// </remarks>
        public static IReadOnlyList<string> AvailableServices { get; } = new List<string>
        {
            ConfigNames.Crawlers.TmdbCrawlerName,
            ConfigNames.Crawlers.OmdbCrawlerName,
            ConfigNames.Crawlers.SteamCrawlerName
        };

        public static IReadOnlyList<string> AvailableAppraisals { get; } = new List<string>
        {
            ConfigNames.Appraisers.TmdbAppraiserCommonName,
            ConfigNames.Appraisers.TmdbAppraiserFuzzyName,
            ConfigNames.Appraisers.OmdbAppraiserCommonName,
            ConfigNames.Appraisers.SteamAppraiserCommonName
        };

        public static IReadOnlyList<string> AvailableOutput { get; } = new List<string>
        {
            ConfigNames.Outputters.LocalFileWriterName,
            ConfigNames.Outputters.GoogleDriveWriterName
        };

        public static IReadOnlyList<string> AvailableBeautifiedServices { get; } = new List<string>
        {
            ConfigNames.BeautifiedServices.TmdbServiceName,
            ConfigNames.BeautifiedServices.OmdbServiceName,
            ConfigNames.BeautifiedServices.SteamServiceName
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
            int index = AvailableServicesToLower.IndexOf(
                service => string.Equals(service, serviceName, StringComparison.OrdinalIgnoreCase)
            );

            if (index == Constants.NotFoundIndex)
            {
                throw new ArgumentException(
                    $"Service name \"{serviceName}\" was not found in the available services.",
                    nameof(serviceName)
                );
            }

            return AvailableServices[index];
        }
    }
}
