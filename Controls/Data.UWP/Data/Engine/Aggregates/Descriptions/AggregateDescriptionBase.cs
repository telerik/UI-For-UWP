using Telerik.Data.Core.Totals;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a base type for aggregate description.
    /// </summary>
    internal abstract class AggregateDescriptionBase : DescriptionBase, IAggregateDescription
    {
        private TotalFormat totalFormat;

        /// <summary>
        /// Gets or sets the <see cref="Telerik.Data.Core.Totals.TotalFormat"/> used to format the generated aggregate values.
        /// </summary>
        TotalFormat IAggregateDescription.TotalFormat
        {
            get
            {
                return this.totalFormat;
            }

            set
            {
                if (this.totalFormat != value)
                {
                    this.ChangeSettingsProperty(ref this.totalFormat, value);
                    this.OnPropertyChanged(nameof(Telerik.Data.Core.Totals.TotalFormat));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <inheritdoc />
        protected internal abstract AggregateValue CreateAggregate();

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
            base.CloneCore(source);
            AggregateDescriptionBase aggregateDescriptionBase = source as AggregateDescriptionBase;
            if (aggregateDescriptionBase != null)
            {
                ((IAggregateDescription)this).TotalFormat = Cloneable.CloneOrDefault(((IAggregateDescription)aggregateDescriptionBase).TotalFormat);
            }
        }
    }
}