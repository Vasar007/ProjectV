using System.IO;

namespace ProjectV.Options
{
    public static class PredefinedPaths
    {
        public static string DefaultOptionsPath { get; } =
            Path.Combine(Directory.GetCurrentDirectory(), CommonConstants.ConfigFilename);
    }
}
