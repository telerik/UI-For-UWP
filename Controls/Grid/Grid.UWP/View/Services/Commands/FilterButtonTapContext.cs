using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents the execution context of a <see cref="CommandId.FilterButtonTap"/> command.
    /// </summary>
    public class FilterButtonTapContext
    {
        /// <summary>
        /// Gets the <see cref="FilterDescriptorBase"/> instance associated with the context.
        /// Typically this is the <see cref="PropertyFilterDescriptor"/> already applied to the <see cref="P:Column"/> instance.
        /// </summary>
        public FilterDescriptorBase AssociatedDescriptor
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IFilterControl"/> control that represents the UI used to generate the first <see cref="FilterDescriptorBase"/>.
        /// </summary>
        public IFilterControl FirstFilterControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IFilterControl"/> control that represents the UI used to generate the second <see cref="FilterDescriptorBase"/>.
        /// </summary>
        public IFilterControl SecondFilterControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="DataGridColumn"/> instance that own the Filter Glyph Button being tapped.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            internal set;
        }
    }
}
