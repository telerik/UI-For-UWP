using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Telerik.Core
{
    /// <summary>
    /// Base implementation of the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs immediately after a property of this instance has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string changedPropertyName = "")
        {
            // The OnPropertyChanged is not virtual itself because of the [CallerMemberName] attribute which in case overridden should be put in every override - quite error-prone.
            this.PropertyChangedOverride(changedPropertyName);

            var eh = this.PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(changedPropertyName));
            }
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        protected virtual void PropertyChangedOverride(string changedPropertyName)
        {
        }
    }
}