using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp.ViewModels.Base
{
    /// <summary>
    /// A wrapper around ICommand to make running commands easyer than 
    /// using the ICommand class. This one accepts one parameter of type object
    /// </summary>
    public class RelayParameterizedCommand : ICommand
    {
        #region Private Members
        /// <summary>
        /// The action to run
        /// </summary>
        private Action<object> _action;
        /// <summary>
        /// Passed in by the outside class and called in the <see cref="CanExecute"/> method.
        /// Used to determin if the <see cref="_action"/> can execute.
        /// If <see cref="_CanExecute"/> return true <see cref="_action"/> will run
        /// </summary>
        private Predicate<object> _CanExecute;
        #endregion

        #region public events
        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region constructors
        /// <summary>
        /// Default constructor. Passes in a function which gets executed when the <see cref="Execute"/> method is called on the <see cref="ICommand"/>
        /// </summary>
        /// <param name="action">the function to call when <see cref="Execute"/> is called</param>
        public RelayParameterizedCommand(Action<object> action)
        {
            this._action = action;
        }
        /// <summary>
        /// Passes in 2 functions. The first function gets called when 
        /// the <see cref="CanExecute(object)"/> method is called on <see cref="ICommand"/> 
        /// and the second function gets called when the <see cref="Execute(object)"/> method
        /// is called on the <see cref="ICommand"/>
        /// </summary>
        /// <param name="canExecute">Function get get called when <see cref="CanExecute(object)"/> method is called</param>
        /// <param name="action">Function to get called when the <see cref="Execute(object)"/> method is called and determins weather the <see cref="Execute(object)"/> method is called</param>
        public RelayParameterizedCommand(Predicate<object> canExecute, Action<object> action)
        {
            // the method to get called if _CanExecute returns true
            this._action = action;
            this._CanExecute = canExecute;
        }
        #endregion


        /// <summary>
        /// Determins if the IComamnd can be executed will always return true if using the 
        /// constructor that only takes in the Action<object> and not the Predicate<object>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            // if _CanExecute was inishalised in the constructor then call it
            // and return its verdict
            if (this._CanExecute != null)
                return this._CanExecute(parameter);
            // If _CanExecute was not inishalied int eh constructor
            // assume we can execute the command so return true
            else
                return true;
        }


        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this._action(parameter);
        }


        public void RaiseCanExecuteChanged()
        {
            //if (CanExecuteChanged != null)
            //    CanExecuteChanged(this, new EventArgs());
        }
    }
}
