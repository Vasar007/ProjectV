using System;
using System.Threading;

namespace ThingAppraiser
{
    /// <summary>
    /// Contains helper methods to simplify development.
    /// </summary>
    public static class SHelperMethods
    {
        /// <inheritdoc cref="Thread.Sleep(Int32)" />
        public static void Sleep(Int32 millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }
    }
}
