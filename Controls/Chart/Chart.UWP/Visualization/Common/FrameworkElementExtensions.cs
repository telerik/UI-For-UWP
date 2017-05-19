using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a FrameworkElementExtensions static class.
    /// </summary>
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Adds <see cref="TranslateTransform"/> to a specific <see cref="FrameworkElement"/>.
        /// </summary>
        public static void AddTransform(this FrameworkElement element, TranslateTransform transform)
        {
            if (transform == null)
            {
                return;
            }

            Transform elementTransform = element.RenderTransform;
            if (elementTransform == null || elementTransform is TranslateTransform)
            {
                element.RenderTransform = transform;
                return;
            }

            if (elementTransform is TransformGroup)
            {
                TranslateTransform translateTransform = (elementTransform as TransformGroup).Children.FirstOrDefault(t => t is TranslateTransform) as TranslateTransform;
                if (translateTransform != null)
                {
                    translateTransform.X = transform.X;
                    translateTransform.Y = transform.Y;
                }
                else
                {
                    (elementTransform as TransformGroup).Children.Add(transform);
                }
                return;
            }

            TransformGroup resultTransform = new TransformGroup();
            resultTransform.Children.Add(elementTransform);
            resultTransform.Children.Add(transform);
            element.RenderTransform = resultTransform;
        }
    }
}
