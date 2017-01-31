namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Represents the context that is passed as a parameter of a <see cref="ItemTapCommand"/>.
    /// </summary>
    public class ItemTapContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTapContext"/> class.
        /// </summary>
        public ItemTapContext(object item, RadListViewItem visualItem)
        {
            this.Item = item;
            this.Container = visualItem;
        }

        /// <summary>
        /// Gets the tapped item.
        /// </summary>
        public object Item { get; private set; }

        internal RadListViewItem Container { get; private set; }
    }
}
