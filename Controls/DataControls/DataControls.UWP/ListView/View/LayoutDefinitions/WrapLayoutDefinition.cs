using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a UI virtualization strategy definition for a wrap layout strategy.
    /// </summary>
    public class WrapLayoutDefinition : LayoutDefinitionBase
    {
        private double itemWidth = 100;

        /// <summary>
        /// Gets or sets the width (height) of the items in vertical (horizontal) orientation.
        /// </summary>
        public double ItemWidth
        {
            get
            {
                return this.itemWidth;
            }
            set
            {
                if (this.itemWidth != value)
                {
                    this.itemWidth = value;
                    this.OnPropertyChanged(nameof(this.ItemWidth));
                }
            }
        }

        internal override Model.BaseLayoutStrategy CreateStrategy(ItemModelGenerator generator, IOrientedParentView view)
        {
            return new WrapLayoutStrategy(generator, view, 40, this.ItemWidth) { IsHorizontal = view.Orientation == Orientation.Horizontal };
        }

        internal override void UpdateStrategy(BaseLayoutStrategy strategy)
        {
            var wrapStrategy = strategy as WrapLayoutStrategy;

            if (wrapStrategy != null)
            {
                wrapStrategy.ItemWidth = this.ItemWidth;
            }
        }
    }
}
