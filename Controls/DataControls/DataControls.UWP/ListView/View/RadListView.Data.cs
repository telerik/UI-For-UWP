using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadListView
    {
        /// <summary>
        /// Gets the <see cref="IDataViewCollection"/> instance that can be used to traverse and/or manipulate the data after all the Sort, Group and Filter operations are applied.
        /// </summary>
        public IDataViewCollection GetDataView()
        {
            return new ListViewDataView(this);
        }
    }
}
