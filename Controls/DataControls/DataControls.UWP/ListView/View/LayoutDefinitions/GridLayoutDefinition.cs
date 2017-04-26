using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a UI virtualization strategy definition for a grid layout strategy where items are rendered in a specified number of columns/rows.
    /// </summary>
    public class GridLayoutDefinition : LayoutDefinitionBase
    {
        private int spanCount = 2;

        /// <summary>
        /// Gets or sets the number of rows/ columns.
        /// </summary>
        public int SpanCount
        {
            get
            {
                return this.spanCount;
            }
            set
            {
                if (this.spanCount != value)
                {
                    this.spanCount = value;
                    this.OnPropertyChanged(nameof(this.SpanCount));
                }
            }
        }

        internal override Model.BaseLayoutStrategy CreateStrategy(ItemModelGenerator generator, IOrientedParentView view)
        {
            return new GridLayoutStrategy(generator, view, IndexStorage.UnknownItemLength, this.SpanCount) { IsHorizontal = view.Orientation == Orientation.Horizontal };
        }

        internal override void UpdateStrategy(BaseLayoutStrategy strategy)
        {
            var gridStrategy = strategy as GridLayoutStrategy;

            if (gridStrategy != null)
            {
                gridStrategy.StackCount = this.SpanCount;
            }
        }
    }
}
