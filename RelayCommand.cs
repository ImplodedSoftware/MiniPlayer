using System;
using System.Windows.Input;

namespace MiniPlayer
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> executeAction;
        private readonly Predicate<object> canExecutePredicate;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate)
        {
            this.executeAction = executeAction;
            this.canExecutePredicate = canExecutePredicate;
        }

        public void Execute(object parameter) => this.executeAction?.Invoke(parameter);
        public bool CanExecute(object parameter) => this.canExecutePredicate?.Invoke(parameter) ?? true;

        public void NotifyCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    }
}
