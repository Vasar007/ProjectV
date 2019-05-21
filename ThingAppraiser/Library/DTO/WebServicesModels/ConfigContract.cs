using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.Data.Models
{
    public static class ConfigContract
    {
        public static IReadOnlyList<string> AvailableMessageHandlers { get; } = new List<string>
        {
            "ConsoleMessageHandler"
        };

        public static IReadOnlyList<string> AvailableInput { get; } = new List<string>
        {
            "LocalFileReaderSimple",
            "LocalFileReaderFilter",
            "GoogleDriveReaderSimple",
            "GoogleDriveReaderFilter"
        };

        public static IReadOnlyList<string> AvailableServices { get; } = new List<string>
        {
            "Tmdb",
            "Omdb",
            "Steam"
        };

        public static IReadOnlyList<string> AvailableAppraisals { get; } = new List<string>
        {
            "TmdbCommon",
            "TmdbFuzzy",
            "OmdbCommon",
            "SteamCommon"
        };

        public static IReadOnlyList<string> AvailableOutput { get; } = new List<string>
        {
            "LocalFileWriter",
            "GoogleDriveWriter"
        };

        public static IReadOnlyList<string> AvailableBeautifiedServices { get; } = new List<string>
        {
            "TMDb",
            "OMDb",
            "Steam"
        };

        public static IReadOnlyList<string> AvailableServicesToLower { get; } =
            AvailableServices.Select(service => service.ToLowerInvariant()).ToList();


        /// <summary>
        /// Checks availability different service components which specifid in 
        /// <see cref="ConfigContract" />.
        /// </summary>
        /// <param name="item">Value to check.</param>
        /// <param name="availableList">Available list from <see cref="ConfigContract" />.</param>
        /// <exception cref="ArgumentException">
        /// <see cref="ConfigContract" /> list doesn't contain passed <paramref name="item" />. -or-
        /// <paramref name="item" /> is <c>null</c> or presents empty string.
        /// </exception>
        public static void CheckAvailability(string item, IReadOnlyList<string> availableList)
        {
            item.ThrowIfNullOrEmpty(nameof(item));
            if (!availableList.Contains(item))
            {
                throw new ArgumentException(
                    $"Config data contains invalid items: {item}.", "configData"
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
