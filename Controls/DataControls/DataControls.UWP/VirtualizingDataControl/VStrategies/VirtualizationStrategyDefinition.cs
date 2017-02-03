using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a base class for a definition of a UI virtualization strategy for <see cref="RadDataBoundListBox"/>.
    /// A UI virtualization strategy definition is a set of properties describing the configuration of a UI strategy. A definition
    /// exposes an API to create an actual UI strategy based on the values set to these properties.
    /// </summary>
    public abstract class VirtualizationStrategyDefinition : INotifyPropertyChanged
    {
        private Orientation orientation;

        /// <summary>
        /// Fired when a property of this instance changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the orientation that the UI virtualization mechanism will use to order visual items.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                if (value != this.orientation)
                {
                    this.orientation = value;
                    this.OnPropertyChanged(nameof(Orientation));
                }
            }
        }

        /// <summary>
        /// Synchronizes the properties of the provided <see cref="VirtualizationStrategy"/> instance
        /// with the current definition.
        /// </summary>
        /// <param name="strategy">The <see cref="VirtualizationStrategy"/> instance which property values will
        /// be synchronized with the ones defined by this <see cref="VirtualizationStrategyDefinition"/> instance.</param>
        internal abstract void SynchStrategyProperties(VirtualizationStrategy strategy);

        /// <summary>
        /// Creates an actual <see cref="VirtualizationStrategy"/> instance
        /// based on the properties defined on this <see cref="VirtualizationStrategyDefinition"/> instance.
        /// </summary>
        /// <returns>An instance of the <see cref="VirtualizationStrategy"/> class that
        /// represents the UI virtualization strategy.</returns>
        internal abstract VirtualizationStrategy CreateStrategy();

        /// <summary>
        /// Called when a property of this instance changes. Fires the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
