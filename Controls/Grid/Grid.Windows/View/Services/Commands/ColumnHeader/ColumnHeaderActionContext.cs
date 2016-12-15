using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public class ColumnHeaderActionContext
    {
        public ColumnHeaderActionContext(string key, DataGridColumnHeader columnHeader)
        {
            this.Key = key;
            this.ColumnHeader = columnHeader;
        }
        public string Key { get; set; }
        public DataGridColumnHeader ColumnHeader { get; set; }
    }
}
