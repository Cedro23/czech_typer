using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CzechTyper.ComponentModel
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public static ICommand Get(ref ICommand command, Action execute)
        {
            return Get(ref command, execute, null);
        }

        public static ICommand Get(ref ICommand command, Action execute, Func<bool> canExecute)
        {
            if (command == null)
                command = new RelayCommand(execute, canExecute);

            return command;
        }

        public static ICommand Get<T>(ref ICommand command, Action<T> execute)
        {
            return Get(ref command, execute, null);
        }

        public static ICommand Get<T>(ref ICommand command, Action<T> execute, Predicate<T> canExecute)
        {
            if (command == null)
                command = new RelayCommand<T>(execute, canExecute);

            return command;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();

            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute((T)parameter);

            return true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
