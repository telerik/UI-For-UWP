namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// This control is used to indicate that the <see cref="RadListView"/> is loading more data in <see cref="Telerik.Core.Data.BatchLoadingMode.Auto"/>.
    /// </summary>
    public class ListViewAutoDataLoadingControl : ListViewLoadDataControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewAutoDataLoadingControl" /> class.
        /// </summary>
        public ListViewAutoDataLoadingControl()
        {
            this.DefaultStyleKey = typeof(ListViewAutoDataLoadingControl);
        }
    }
}
