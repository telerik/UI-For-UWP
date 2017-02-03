using System;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a UI virtualization strategy definition for a <see cref="WrapVirtualizationStrategy"/> instance.
    /// </summary>
    public class WrapVirtualizationStrategyDefinition : VirtualizationStrategyDefinition
    {
        private WrapLineAlignment wrapLineAlignment = WrapLineAlignment.Near;

        /// <summary>
        /// Gets or sets the alignment of the visual containers within a wrap row.
        /// </summary>
        public WrapLineAlignment WrapLineAlignment
        {
            get
            {
                return this.wrapLineAlignment;
            }
            set
            {
                if (this.wrapLineAlignment != value)
                {
                    this.wrapLineAlignment = value;
                    this.OnPropertyChanged(nameof(WrapLineAlignment));
                }
            }
        }

        /// <summary>
        /// Creates an actual <see cref="VirtualizationStrategy"/> instance
        /// based on the properties defined on this <see cref="VirtualizationStrategyDefinition"/> instance.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="VirtualizationStrategy"/> class that
        /// represents the UI virtualization strategy.
        /// </returns>
        internal override VirtualizationStrategy CreateStrategy()
        {
            WrapVirtualizationStrategy strategy = new WrapVirtualizationStrategy();
            strategy.Orientation = this.Orientation;
            return strategy;
        }

        internal override void SynchStrategyProperties(VirtualizationStrategy strategy)
        {
            WrapVirtualizationStrategy typedStrategy = strategy as WrapVirtualizationStrategy;

            if (typedStrategy == null)
            {
                throw new InvalidOperationException("This virtualization strategy definition can only synch properties for " + typeof(WrapVirtualizationStrategy).ToString() + " instances.");
            }

            typedStrategy.Orientation = this.Orientation;
            typedStrategy.WrapLineAlignment = this.WrapLineAlignment;
        }
    }
}
