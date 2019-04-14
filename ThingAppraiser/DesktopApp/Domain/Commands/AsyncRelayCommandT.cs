using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ThingAppraiser;

namespace DesktopApp.Domain.Commands
{
    public class CAsyncRelayCommand<T> : IAsyncCommand<T>
    {
        private Boolean _isExecuting;

        private readonly Func<T, Task> _execute;

        private readonly Func<T, Boolean> _canExecute;

        private readonly IErrorHandler _errorHandler;


        public CAsyncRelayCommand(Func<T, Task> execute, Func<T, Boolean> canExecute,
            IErrorHandler errorHandler)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (t => true);
            _errorHandler = errorHandler;
        }

        public CAsyncRelayCommand(Func<T, Task> execute, Func<T, Boolean> canExecute)
            : this(execute, canExecute, null)
        {
        }

        public CAsyncRelayCommand(Func<T, Task> execute)
            : this(execute, null, null)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region IAsyncCommand<T> Implementation

        public Boolean CanExecute(T parameter)
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

        bool ICommand.CanExecute(Object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(Object parameter)
        {
            ExecuteAsync((T) parameter).FireAndForgetSafeAsync(_errorHandler);
        }
        
        #endregion
    }
}
