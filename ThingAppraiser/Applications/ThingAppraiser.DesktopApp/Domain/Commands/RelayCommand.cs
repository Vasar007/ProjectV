using System;
using System.Windows.Input;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    /// <summary>
    /// No WPF project is complete without it's own version of this.
    /// </summary>
    internal sealed class RelayCommand : ICommand
    {
        private readonly Action _execute;

        private readonly Func<bool> _canExecute;


        public RelayCommand(Action execute, Func<bool>? canExecute)
        {
            _execute = execute.ThrowIfNull(nameof(execute));
            _canExecute = canExecute ?? (() => true);
        }

        public RelayCommand(Action execute)
            : this(execute, null)
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

        #region ICommand Implementation

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();

            RaiseCanExecuteChanged();
        }

        #endregion
    }
}
