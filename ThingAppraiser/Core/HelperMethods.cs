using System;

namespace ThingAppraiser
{
    public static class HelperMethods
    {
        internal static void ThrowIfNull<T>(this T obj, string paramName) where T : class
        {
            if (obj is null)
            {
                throw new ArgumentNullException($"{paramName} is null.");
            }
        }
    }
}
