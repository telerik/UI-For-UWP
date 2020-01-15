using System;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Contains information about an editor's value change.
    /// </summary>
    public class EditorValueChangedEventArgs : EventArgs
    {
        internal EditorValueChangedEventArgs(string propertyName, object newValue)
        {
            this.PropertyName = propertyName;
            this.NewValue = newValue;
        }

        /// <summary>
        /// Gets the name of the associated property.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the new value of the editor.
        /// </summary>
        public object NewValue { get; }
    }
}