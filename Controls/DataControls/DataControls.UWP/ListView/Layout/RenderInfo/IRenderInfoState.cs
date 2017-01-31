using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core.Layouts
{
    internal interface IRenderInfoState
    {
        double? GetValueAt(int index);
    }
}
