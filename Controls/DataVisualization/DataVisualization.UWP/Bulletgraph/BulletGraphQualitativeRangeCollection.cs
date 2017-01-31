using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    // This class is necessary because a generic type can not be instantiated in XAML.

    /// <summary>
    /// This collection is used in RadBulletGraph and contains its qualitative range segments.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class BulletGraphQualitativeRangeCollection : ObservableCollection<BarIndicatorSegment>
    {
    }
}
