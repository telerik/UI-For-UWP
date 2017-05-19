namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    internal enum UpdateFlags
    {
        None = 0,
        AffectsData = 1,
        AffectsContent = AffectsData << 1,
        AffectsScrollPosition = AffectsContent << 1,
        AffectsDecorations = AffectsScrollPosition << 1,
        AllButData = AffectsContent | AffectsScrollPosition | AffectsDecorations,
        All = AffectsData | AllButData,
    }
}
