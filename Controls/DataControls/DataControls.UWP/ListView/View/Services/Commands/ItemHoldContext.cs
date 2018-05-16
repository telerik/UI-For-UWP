namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Represents the context that is passed as a parameter of a <see cref="CommandId.ItemHold"/> command.
    /// </summary>
    public class ItemHoldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemHoldContext"/> class.
        /// </summary>
        public ItemHoldContext(object item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the hold item.
        /// </summary>
        public object Item { get; private set; }
    }
}