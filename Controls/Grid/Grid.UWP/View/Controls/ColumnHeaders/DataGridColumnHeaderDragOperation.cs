using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    internal class DataGridColumnHeaderDragOperation : ReorderItemsDragOperation
    {
        private DataGridColumnHeader header;

        public DataGridColumnHeaderDragOperation(DataGridColumnHeader header)
            : base(header)
        {
            this.header = header;
            this.HeaderOwner = header.Owner;
        }

        public DataGridColumn Column
        {
            get
            {
                return this.header.Column;
            }
        }

        public RadDataGrid HeaderOwner { get; private set; }
    }
}
