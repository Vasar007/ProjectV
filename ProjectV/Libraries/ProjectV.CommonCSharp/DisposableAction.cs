using System;
using Acolyte.Assertions;

namespace ProjectV
{
    public sealed class DisposableAction : IDisposable
    {
        private readonly Action _onDisposeAction;


        public DisposableAction(
            Action onDisposeAction)
        {
            _onDisposeAction = onDisposeAction.ThrowIfNull(nameof(onDisposeAction));
        }

        #region IDisposable Implementation

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _onDisposeAction();

            _disposed = true;
        }

        #endregion
    }
}
