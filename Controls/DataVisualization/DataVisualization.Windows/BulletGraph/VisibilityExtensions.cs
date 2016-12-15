using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.DataVisualization.BulletGraph
{
    internal static class VisibilityExtensions
    {
        internal static Visibility Opposite(this Visibility value)
        {
            if (value == Visibility.Collapsed)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
    }
}
