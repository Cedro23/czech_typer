using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace CzechTyper.ComponentModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private string _displayName;

        public Dispatcher AppDispatcher
        {
            get { return System.Windows.Application.Current?.Dispatcher; }
        }

        public virtual string DisplayName
        {
            get
            {
                return _displayName;
            }
            protected set
            {
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Conditional("DEBUG")]
        public void VerifyPropertyName(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new Exception("Invalid property name: " + propertyName);
            }
        }

        public void InvalidateRequerySuggested()
        {
            AppDispatcher?.BeginInvoke(DispatcherPriority.Normal,
                (Action)(() => { System.Windows.Input.CommandManager.InvalidateRequerySuggested(); }));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }

            InvalidateRequerySuggested();
        }

        protected bool OnPropertyChanged<T>(string propertyName, ref T value, T newValue) where T : IComparable<T>
        {
            return OnPropertyChanged(propertyName, ref value, newValue, delegate (T x, T y)
            {
                if (x != null || y != null)
                {
                    if (x != null)
                    {
                        return x.CompareTo(y) == 0;
                    }
                    return false;
                }
                return true;
            });
        }

        protected bool OnPropertyChanged<T>(string propertyName, ref T value, T newValue, Func<T, T, bool> equalityComparer)
        {
            if (!equalityComparer(value, newValue))
            {
                value = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        [Obsolete("Use OnPropertyChanged() instead")]
        protected bool OnPropertyChangedIfChanged<T>(string propertyName, ref T value, T newValue) where T : IComparable<T>
        {
            return OnPropertyChanged(propertyName, ref value, newValue);
        }

        [Obsolete("Use OnPropertyChanged() instead")]
        protected bool OnPropertyChangedIfChanged<T>(string propertyName, ref T value, T newValue, Func<T, T, bool> equalityComparer)
        {
            return OnPropertyChanged(propertyName, ref value, newValue, equalityComparer);
        }
    }
}
