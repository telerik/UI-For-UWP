using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Contains data to update an arc based on an angle.
    /// </summary>
    internal class UpdateArcContext
    {
        internal double angle;
        internal double radius;
        internal double minAngle;
        internal Point center;
        internal ArcSegment arc;
    }
}
