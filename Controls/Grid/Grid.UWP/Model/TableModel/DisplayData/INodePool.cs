using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface INodePool<T>
    {
        IRenderInfo RenderInfo
        {
            get;
        }

        double AvailableLength
        {
            get;
        }

        double HiddenPixels
        {
            get;
        }

        int ViewportItemCount
        {
            get;
        }

        void RefreshRenderInfo(double defaultValue);

        IEnumerable<KeyValuePair<int, List<T>>> GetDisplayedElements();

        T GetDisplayedElement(int slot);

        /// <summary>
        /// Gets the layout slot associated with the displayed elements. This method reliably returns the last valid displayed state even if node pool is currently recycled.
        /// </summary>
        RadRect GetPreviousDisplayedLayoutSlot(int slot);

        bool IsItemCollapsed(int cellSlot);
    }
}
