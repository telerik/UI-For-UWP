using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IGenerationContext
    {
        bool IsFrozen
        {
            get;
        }
    }
}
