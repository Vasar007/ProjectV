using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    internal sealed class AsyncRelayCommand : IAsyncCommand
    {
        private readonly Func<Task> _execute;

        private readonly Func<bool> _canExecute;

        private readonly IErrorHandler? _errorHandler;

        private bool _isExecuting;


        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute,
            IErrorHandler? errorHandler)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (() => true);
            _errorHandler = errorHandler;
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute)
            : this(execute, canExecute, null)
        {
        }

        public AsyncRelayCommand(Func<Task> execute)
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

        #region IAsyncCommand Implementation

        public bool CanExecute()
        {
            return !_isExecuting && _canExecute.Invoke();
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        #endregion

        #region ICommand Explicit Implementations

        public event EventHandler CanExecuteChanged;

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
        }

        #endregion
    }
}
