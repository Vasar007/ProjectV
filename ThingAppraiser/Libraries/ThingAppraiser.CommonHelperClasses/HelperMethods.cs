using System.Threading;

namespace ThingAppraiser
{
    /// <summary>
    /// Contains helper methods to simplify development.
    /// </summary>
    public static class HelperMethods
    {
        /// <inheritdoc cref="Thread.Sleep(int)" />
        public static void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }
    }
}
