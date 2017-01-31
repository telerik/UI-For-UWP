using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal abstract class VisualStateLayer : SharedUILayer
    {
        internal abstract void UpdateHoverDecoration(RadRect slot);

        internal abstract void UpdateCurrencyDecoration(RadRect slot);

        internal abstract void UpdateVisuals();
    }
}
