using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Data
{
    partial class RadListView
    {
        public IDataViewCollection GetDataView()
        {
            return new ListViewDataView(this);
        }
    }
}
