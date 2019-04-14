using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ThingAppraiser;

namespace DesktopApp.Domain.Commands
{
    public class CAsyncRelayCommand : IAsyncCommand
    {
        private readonly Func<Task> _execute;

        private readonly Func<Boolean> _canExecute;

        private readonly IErrorHandler _errorHandler;

        private Boolean _isExecuting;


        public CAsyncRelayCommand(Func<Task> execute, Func<Boolean> canExecute,
            IErrorHandler errorHandler)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (() => true);
            _errorHandler = errorHandler;
        }

        public CAsyncRelayCommand(Func<Task> execute, Func<Boolean> canExecute)
            : this(execute, canExecute, null)
        {
        }

        public CAsyncRelayCommand(Func<Task> execute)
            : this(execute, null, null)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region IAsyncCommand Implementation

        public Boolean CanExecute()
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

        Boolean ICommand.CanExecute(Object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(Object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
        }

        #endregion
    }
}
