namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a UI virtualization strategy definition for a <see cref="DynamicGridVirtualizationStrategy"/> instance.
    /// This virtualization strategy orders the visual items in a wrapped grid in which each wrapped item is exactly below or next to its stack predecessor.
    /// </summary>
    public class DynamicGridVirtualizationStrategyDefinition : VirtualizationStrategyDefinition
    {
        private int stackCount = 4;

        /// <summary>
        /// Gets or sets the item extent.
        /// </summary>
        /// <value>
        /// The item extent.
        /// </value>
        public int StackCount
        {
            get
            {
                return this.stackCount;
            }
            set
            {
                if (this.stackCount != value)
                {
                    this.stackCount = value;
                    this.OnPropertyChanged(nameof(this.StackCount));
                }
            }
        }

        /// <summary>
        /// Synchronizes the properties of the provided <see cref="VirtualizationStrategy" /> instance
        /// with the current definition.
        /// </summary>
        /// <param name="strategy">The <see cref="VirtualizationStrategy" /> instance which property values will
        /// be synchronized with the ones defined by this <see cref="VirtualizationStrategyDefinition" /> instance.</param>
        internal override void SynchStrategyProperties(VirtualizationStrategy strategy)
        {
            (strategy as DynamicGridVirtualizationStrategy).StackCount = this.StackCount;
            (strategy as DynamicGridVirtualizationStrategy).Orientation = this.Orientation;
        }

        /// <summary>
        /// Creates an actual <see cref="VirtualizationStrategy" /> instance
        /// based on the properties defined on this <see cref="VirtualizationStrategyDefinition" /> instance.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="VirtualizationStrategy" /> class that
        /// represents the UI virtualization strategy.
        /// </returns>
        internal override VirtualizationStrategy CreateStrategy()
        {
            return new DynamicGridVirtualizationStrategy() { StackCount = this.stackCount, Orientation = this.Orientation };
        }
    }
}
