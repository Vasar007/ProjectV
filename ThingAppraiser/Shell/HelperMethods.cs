using System;

namespace ThingAppraiser
{
    public static class HelperMethods
    {
        internal static void ThrowIfNull<T>(this T o, string paramName) where T : class
        {
            if (o == null) throw new ArgumentNullException(paramName);
        }
    }
}
