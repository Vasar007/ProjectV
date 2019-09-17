using System;
using System.Windows.Input;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    /// <summary>
    /// No WPF project is complete without it's own version of this.
    /// </summary>
    internal sealed class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        private readonly Func<T, bool> _canExecute;


        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (t => true);
        }

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand Implementation

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T) parameter);

            RaiseCanExecuteChanged();
        }

        #endregion
    }
}
