using System;
using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Encapsulates the base functionality for establishing a <see cref="Telerik.Charting.DataPoint"/> binding.
    /// </summary>
    public abstract class DataPointBinding : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Retrieves the value for the specified object instance.
        /// </summary>
        public abstract object GetValue(object instance);

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property which value has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eh = this.PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
