using System;
using System.Windows.Input;
using Google.Apis.Util;

namespace DesktopApp.Domain.Commands
{
    /// <summary>
    /// No WPF project is complete without it's own version of this.
    /// </summary>
    public class CRelayCommand : ICommand
    {
        private readonly Action<Object> _execute;

        private readonly Func<Object, Boolean> _canExecute;


        public CRelayCommand(Action<Object> execute, Func<Object, Boolean> canExecute)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (x => true);
        }

        public CRelayCommand(Action<Object> execute)
            : this(execute, null)
        {
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #region ICommand Implementation

        public Boolean CanExecute(Object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(Object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion
    }
}
