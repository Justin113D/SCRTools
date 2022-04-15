using System;
using System.Windows.Input;

namespace SCR.Tools.Viewmodeling
{
    public class EventCommand : ICommand
    {
        public event EventHandler<object> Executed;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
            => true;

        public void Execute(object parameter)
            => Executed?.Invoke(this, parameter);
    }
}
