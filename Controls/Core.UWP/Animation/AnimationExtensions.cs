using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// Provides extension methods related to the Telerik Silverlight Animation Framework.
    /// </summary>
    internal static class AnimationExtensions
    {
        internal static void EnsurePlaneProjection(this UIElement element)
        {
            if (element.Projection == null)
            {
                element.Projection = new PlaneProjection();
            }
        }

        internal static void EnsureDefaultTransforms(this UIElement element)
        {
            var group = element.RenderTransform as TransformGroup;
            if (group == null || group.Children.Count < 4 ||
                !(group.Children[0] is ScaleTransform) ||
                !(group.Children[1] is SkewTransform) ||
                !(group.Children[2] is RotateTransform) ||
                !(group.Children[3] is TranslateTransform))
            {
                group = new TransformGroup();
                group.Children.Add(new ScaleTransform());
                group.Children.Add(new SkewTransform());
                group.Children.Add(new RotateTransform());
                group.Children.Add(new TranslateTransform());

                element.RenderTransform = group;
            }
        }

        internal static ScaleTransform GetScaleTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return (element.RenderTransform as TransformGroup).Children[0] as ScaleTransform;
        }

        internal static SkewTransform GetSkewTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return (element.RenderTransform as TransformGroup).Children[1] as SkewTransform;
        }

        internal static RotateTransform GetRotateTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return (element.RenderTransform as TransformGroup).Children[2] as RotateTransform;
        }

        internal static TranslateTransform GetTranslateTransform(this UIElement element)
        {
            element.EnsureDefaultTransforms();
            return (element.RenderTransform as TransformGroup).Children[3] as TranslateTransform;
        }
    }
}