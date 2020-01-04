using System;
using System.Collections.Generic;
using Acolyte.Assertions;
using ThingAppraiser.DesktopApp.Domain;

namespace ThingAppraiser.DesktopApp.Models.Things
{
    internal sealed class ThingsDataToAppraise
    {
        public DataSource DataSource { get; }

        public string StorageName { get; }

        public IReadOnlyList<string> ThingNames { get; }


        private ThingsDataToAppraise(DataSource dataSource, string storageName,
            IReadOnlyList<string> thingNames)
        {
            // Assume that constructor will get only validated data.

            DataSource = dataSource;
            StorageName = storageName;
            ThingNames = thingNames;
        }

        public static ThingsDataToAppraise Create(DataSource dataSource, string storageName,
            IReadOnlyList<string> thingNames)
        {
            storageName.ThrowIfNullOrEmpty(nameof(storageName));
            thingNames.ThrowIfNullOrEmpty(nameof(thingNames));

            return new ThingsDataToAppraise(dataSource, storageName, thingNames);
        }

        public static ThingsDataToAppraise Create(DataSource dataSource, string storageName)
        {
            storageName.ThrowIfNullOrEmpty(nameof(storageName));

            return new ThingsDataToAppraise(dataSource, storageName, Array.Empty<string>());
        }
    }
}
