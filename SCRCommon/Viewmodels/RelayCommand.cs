using System;
using System.Windows.Input;

namespace SCRCommon.Viewmodels
{
    /// <summary>
    /// A basic command that runs an Action with a specific parameter
    /// </summary>
    public class RelayCommand<ParameterType> : ICommand
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private Action<ParameterType> mAction;

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommand(Action<ParameterType> action)
        {
            mAction = action;
        }

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if(typeof(ParameterType) == null || parameter.GetType() == typeof(ParameterType))
                mAction((ParameterType)parameter);
            else
                throw new ArgumentException("Parameter of type " + parameter.GetType() + ", but it should be " + typeof(ParameterType), "parameter");
        }
    }

    /// <summary>
    /// A basic command that runs an Action
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action mAction;

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action that should be performed upon being called</param>
        public RelayCommand(Action action)
        {
            mAction = action;
        }

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter">Input parameter (unused)</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter">Input parameter (unused)</param>
        public void Execute(object parameter)
        {
            mAction();
        }

    }

}