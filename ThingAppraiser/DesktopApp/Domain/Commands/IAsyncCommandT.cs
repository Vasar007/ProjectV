using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopApp.Domain.Commands
{
    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);

        Boolean CanExecute(T parameter);
    }
}
