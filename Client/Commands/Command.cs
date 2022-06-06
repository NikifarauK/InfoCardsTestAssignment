using System;
using System.Windows.Input;

namespace Client.Commands
{
    internal class Command : ICommand
    {
        private Action<object> execute { get; }
        private Func<object, bool> canExecute { get; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            execute(parameter);
        }
    }
}
