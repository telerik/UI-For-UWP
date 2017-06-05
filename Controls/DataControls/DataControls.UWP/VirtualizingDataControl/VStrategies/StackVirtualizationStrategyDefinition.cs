using System;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a UI virtualization strategy definition for a <see cref="StackVirtualizationStrategy"/> instance.
    /// </summary>
    public class StackVirtualizationStrategyDefinition : VirtualizationStrategyDefinition
    {
        internal CollectionChangeItemReorderMode reorderMode;

        /// <summary>
        /// Gets or sets the reorder mode of the viewport items
        /// which defines the direction of reordering when a collection change occurs.
        /// </summary>
        /// <value>
        /// The reorder mode.
        /// </value>
        public CollectionChangeItemReorderMode ReorderMode
        {
            get
            {
                return this.reorderMode;
            }
            set
            {
                if (this.reorderMode != value)
                {
                    this.reorderMode = value;
                    this.OnPropertyChanged(nameof(this.ReorderMode));
                }
            }
        }

        /// <summary>
        /// Creates an actual <see cref="VirtualizationStrategy" /> instance
        /// based on the properties defined on this <see cref="VirtualizationStrategyDefinition" /> instance.
        /// </summary>
        /// .
        /// <returns>
        /// An instance of the <see cref="VirtualizationStrategy" /> class that
        /// represents the UI virtualization strategy.
        /// </returns>
        internal override VirtualizationStrategy CreateStrategy()
        {
            StackVirtualizationStrategy strategy = new StackVirtualizationStrategy();
            strategy.Orientation = this.Orientation;
            return strategy;
        }

        /// <summary>
        /// Synchronizes the properties of the provided <see cref="VirtualizationStrategy" /> instance
        /// with the current definition.
        /// </summary>
        /// <param name="strategy">The <see cref="VirtualizationStrategy" /> instance which property values will
        /// be synchronized with the ones defined by this <see cref="VirtualizationStrategyDefinition" /> instance.</param>
        internal override void SynchStrategyProperties(VirtualizationStrategy strategy)
        {
            StackVirtualizationStrategy typedStrategy = strategy as StackVirtualizationStrategy;

            if (typedStrategy == null)
            {
                throw new InvalidOperationException("This virtualization strategy definition can only synch properties for " + typeof(StackVirtualizationStrategy).ToString() + " instances.");
            }

            typedStrategy.Orientation = this.Orientation;
            typedStrategy.ReorderMode = this.ReorderMode;
        }
    }
}
