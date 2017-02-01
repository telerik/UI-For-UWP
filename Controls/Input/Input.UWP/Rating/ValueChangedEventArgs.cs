using System;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// This class contains information about events which occur when
    /// a given value is changed. Here the old and the new values are exposed.
    /// </summary>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        private T oldValue;
        private T newValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Gets an instance of the <see cref="DateTime"/> struct
        /// that represents the previous value.
        /// </summary>
        public T OldValue
        {
            get
            {
                return this.oldValue;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="DateTime"/> struct
        /// that represents the new value.
        /// </summary>
        public T NewValue
        {
            get
            {
                return this.newValue;
            }
        }
    }
}