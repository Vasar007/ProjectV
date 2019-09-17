using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    internal sealed class AsyncRelayCommand<T> : IAsyncCommand<T>
    {
        private bool _isExecuting;

        private readonly Func<T, Task> _execute;

        private readonly Func<T, bool> _canExecute;

        private readonly IErrorHandler? _errorHandler;


        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool>? canExecute,
            IErrorHandler? errorHandler)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (t => true);
            _errorHandler = errorHandler;
        }

        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool>? canExecute)
            : this(execute, canExecute, null)
        {
        }

        public AsyncRelayCommand(Func<T, Task> execute)
            : this(execute, null, null)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #region IAsyncCommand<T> Implementation

        public bool CanExecute(T parameter)
        {
            return !_isExecuting && _canExecute.Invoke(parameter);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        #endregion

        #region ICommand Explicit Implementation

        public event EventHandler CanExecuteChanged;

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T) parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T) parameter).FireAndForgetSafeAsync(_errorHandler);
        }

        #endregion
    }
}
