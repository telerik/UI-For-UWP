using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents an StaggeredLayoutDefinition layout definition.
    /// </summary>
    public class StaggeredLayoutDefinition : LayoutDefinitionBase
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

        internal override BaseLayoutStrategy CreateStrategy(ItemModelGenerator generator, IOrientedParentView view)
        {
            return new StaggeredLayoutStrategy(generator, view, IndexStorage.UnknownItemLength, this.SpanCount) { IsHorizontal = view.Orientation == Orientation.Horizontal };
        }

        internal override void UpdateStrategy(BaseLayoutStrategy strategy)
        {
            var staggStr = strategy as StaggeredLayoutStrategy;

            if (staggStr != null)
            {
                staggStr.StackCount = this.SpanCount;
            }
        }
    }
}
