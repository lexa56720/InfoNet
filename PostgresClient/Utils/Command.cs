using System;
using System.Windows.Input;

namespace PostgresClient.Utils
{
    public class Command : ICommand
    {
        private Action<object> execute;
        private Func<object, bool>? canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public Command(Action execute, Func<object, bool> canExecute = null)
        {
            this.execute = new Action<object>(o => execute());
            this.canExecute = null;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
