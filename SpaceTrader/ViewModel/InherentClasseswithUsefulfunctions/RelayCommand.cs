using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SpaceTrader
{
    //public class RelayCommand<T> : ICommand
    //{
    //    private readonly Predicate<T> _canExecute;
    //    private readonly Action<T> _execute;

    //    public RelayCommand(Action<T> execute)
    //       : this(execute, null)
    //    {
    //        _execute = execute;
    //    }

    //    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    //    {
    //        if (execute == null)
    //            throw new ArgumentNullException("execute");
    //        _execute = execute;
    //        _canExecute = canExecute;
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return (_canExecute == null) || _canExecute((T)parameter);
    //    }

    //    public void Execute(object parameter)
    //    {
    //        _execute((T)parameter);
    //    }

    //    public event EventHandler CanExecuteChanged
    //    {
    //        add { CommandManager.RequerySuggested += value; }
    //        remove { CommandManager.RequerySuggested -= value; }
    //    }
    //}
    public class RelayCommand : ICommand
    {

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        //private string _displayText;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        //public string DisplayText
        //{
        //    get { return _displayText; }
        //    set { _displayText = value; }
        //}
        //public static List<string> Log = new List<string>();

        /// Creates a new command.
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// each this() steps further in the overload chain 
        /// 

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }
        //override
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)// : this(execute, canExecute, "")
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }
        //override
        //public RelayCommand(Action<object> execute, Predicate<object> canExecute, string displayText)
        //{
        //    _execute = execute ?? throw new ArgumentNullException("execute");
        //    _canExecute = canExecute;
        //    //_displayText = displayText;
        //}

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }

}
