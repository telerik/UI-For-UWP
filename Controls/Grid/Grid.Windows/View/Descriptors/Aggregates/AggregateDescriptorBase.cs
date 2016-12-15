using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents the base class for all descriptors that describe an aggregate function within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public abstract class AggregateDescriptorBase : DataDescriptor
    {
        private string format;

        /// <summary>
        /// Gets or sets the string value used to format the computed value, using the String.Format method.
        /// </summary>
        internal string Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if (this.format == value)
                {
                    return;
                }

                this.format = value;
                this.OnPropertyChanged();
            }
        }
    }
}
