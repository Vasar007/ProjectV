using System;
using Acolyte.Assertions;

namespace ProjectV
{
    public static class UnhandledExceptionEventRegistrator
    {
        public static void Register(UnhandledExceptionEventHandler eventHandler)
        {
            eventHandler.ThrowIfNull(nameof(eventHandler));

            AppDomain.CurrentDomain.UnhandledException += eventHandler;
        }
    }
}
