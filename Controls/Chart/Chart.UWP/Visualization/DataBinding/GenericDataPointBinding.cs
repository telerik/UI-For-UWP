using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a <see cref="DataPointBinding"/> that uses a generic delegate to retrieve the value to be applied for the generated data points.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the generic delegate.</typeparam>
    public class GenericDataPointBinding<TElement, TResult> : DataPointBinding
    {
        private Func<TElement, TResult> valueSelector;

        /// <summary>
        /// Gets or sets the generic delegate used to retrieve bound objects values.
        /// </summary>
        public Func<TElement, TResult> ValueSelector
        {
            get
            {
                return this.valueSelector;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (this.valueSelector == value)
                {
                    return;
                }

                this.valueSelector = value;
                this.OnPropertyChanged(nameof(this.ValueSelector));
            }
        }

        /// <summary>
        /// Retrieves the value for the specified object instance.
        /// </summary>
        public override object GetValue(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (this.valueSelector == null)
            {
                throw new InvalidOperationException("ValueSelector can not be null. Please specify a valid ValueSelector callback.");
            }

            TElement typedInstance = (TElement)instance;
            if (typedInstance == null)
            {
                throw new ArgumentException("Unexpected object type");
            }

            return this.valueSelector(typedInstance);
        }
    }
}
