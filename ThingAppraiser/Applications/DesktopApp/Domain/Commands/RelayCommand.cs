using System;
using System.Windows.Input;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    /// <summary>
    /// No WPF project is complete without it's own version of this.
    /// </summary>
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        private readonly Func<object, bool> _canExecute;


        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (x => true);
        }

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #region ICommand Implementation

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
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
