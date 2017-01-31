using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents provider for <see cref="LegendItem"/> objects.
    /// </summary>
    public interface ILegendInfoProvider
    {
        /// <summary>
        /// Gets a collection of <see cref="LegendItem"/> objects that hold the information required to display legend.
        /// </summary>
        LegendItemCollection LegendInfos { get; }
    }
}
