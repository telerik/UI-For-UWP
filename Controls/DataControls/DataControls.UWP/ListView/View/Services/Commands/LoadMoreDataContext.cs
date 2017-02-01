namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Context that is passed as a parameter of the <see cref="LoadMoreDataCommand"/>.
    /// </summary>
    public class LoadMoreDataContext
    {
        // TODO: make common for grid and listview

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public uint? BatchSize { get; set; }
    }
}
