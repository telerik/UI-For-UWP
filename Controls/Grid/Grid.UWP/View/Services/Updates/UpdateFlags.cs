using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal enum UpdateFlags
    {
        None = 0,
        AffectsData = 1,
        AffectsColumnHeader = AffectsData << 1,
        AffectsContent = AffectsColumnHeader << 1,
        AffectsScrollPosition = AffectsContent << 1,
        AffectsColumnsWidth = AffectsScrollPosition << 1,
        AffectsDecorations = AffectsColumnsWidth << 1,
        AllButData = AffectsContent | AffectsScrollPosition | AffectsColumnsWidth | AffectsDecorations,
        All = AffectsData | AllButData
    }
}
