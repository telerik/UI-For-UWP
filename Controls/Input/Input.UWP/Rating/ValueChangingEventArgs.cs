using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Contains information about an event which occurs
    /// when a given value is about to change. Allows for canceling the change.
    /// </summary>
    /// <typeparam name="T">The type of the value that is about to change.</typeparam>
    public class ValueChangingEventArgs<T> : CancelEventArgs
    {
        private T oldValue;
        private T newValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangingEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ValueChangingEventArgs(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Gets an instance of the <see cref="System.DateTime"/> struct
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
        /// Gets an instance of the <see cref="System.DateTime"/> struct
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