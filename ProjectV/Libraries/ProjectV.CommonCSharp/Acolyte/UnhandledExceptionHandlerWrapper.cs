using System;
using Acolyte.Assertions;

namespace Acolyte.Exceptions.Handlers
{
    public sealed class UnhandledExceptionHandlerWrapper : IUnhandledExceptionHandler
    {
        private readonly UnhandledExceptionEventHandler _handler;


        public UnhandledExceptionHandlerWrapper(
            UnhandledExceptionEventHandler handler)
        {
            _handler = handler.ThrowIfNull(nameof(handler));
        }

        #region IUnhandledExceptionHandler Implementation

        public void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            _handler(sender, e);
        }

        #endregion
    }
}
