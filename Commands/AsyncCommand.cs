using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Taskish.Commands
{
    public abstract class AsyncCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAync(parameter);
        }

        protected abstract Task ExecuteAync(object? parameter);
    }
}
