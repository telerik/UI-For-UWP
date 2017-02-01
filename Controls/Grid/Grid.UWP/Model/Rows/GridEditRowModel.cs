using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class GridEditRowModel : GridRowModel
    {
        public ItemInfo ReadOnlyRowInfo { get; set; }
    }
}
