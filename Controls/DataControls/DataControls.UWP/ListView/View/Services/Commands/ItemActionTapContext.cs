namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// The context that is passed as a parameter of an <see cref="ItemActionTapCommand"/>.
    /// </summary>
    public class ItemActionTapContext : ItemTapContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemActionTapContext"/> class.
        /// </summary>
        public ItemActionTapContext(object item, RadListViewItem visualItem, double offset)
            : base(item, visualItem)
        {
            this.Offset = offset;
        }

        /// <summary>
        /// Gets a value that specifies the item current offset. Can be used for determine the action that needs to be invoked. 
        /// </summary>
        public double Offset { get; private set; }
    }
}
