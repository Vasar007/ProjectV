using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopApp.Domain.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();

        Boolean CanExecute();
    }
}
