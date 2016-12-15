using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal enum InvalidateMeasureFlags
    {
        None = 0,
        Header = 1,
        Cells = Header << 1,
        All = Header | Cells
    }
}
