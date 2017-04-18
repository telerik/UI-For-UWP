using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.Core
{
    /// <summary>
    /// Replaces an element with a screenshot and positions the screenshot in a popup, keeping the same location.
    /// </summary>
    internal class ElementScreenShotInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementScreenShotInfo"/> class.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        public ElementScreenShotInfo(FrameworkElement targetElement)
        {
            FrameworkElement rootVisual = Window.Current.Content as FrameworkElement;
            this.Popup = new Popup();
            Canvas popupChild = new Canvas()
            {
                Width = rootVisual.ActualWidth,
                Height = rootVisual.ActualHeight
            };

#if WINDOWS_PHONE
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PageOrientation pageOrientation = frame.Orientation;
                double width = rootVisual.ActualWidth;
                double height = rootVisual.ActualHeight;
                double rotateAngle = 0;
                double translateX = 0;
                double translateY = 0;
                switch (pageOrientation)
                {
                    case PageOrientation.LandscapeLeft:
                        rotateAngle = 90;
                        translateX = rootVisual.ActualWidth;
                        height = rootVisual.ActualWidth;
                        width = rootVisual.ActualHeight;
                        break;
                    case PageOrientation.LandscapeRight:
                        rotateAngle = 270;
                        translateY = rootVisual.ActualHeight;
                        height = rootVisual.ActualWidth;
                        width = rootVisual.ActualHeight;
                        break;
                    case PageOrientation.PortraitDown:
                        rotateAngle = 180;
                        break;
                    default:
                        break;
                }

                popupChild.Width = width;
                popupChild.Height = height;
                TransformGroup transformGroup = new TransformGroup();

                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.Angle = rotateAngle;
                transformGroup.Children.Add(rotateTransform);

                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X = translateX;
                translateTransform.Y = translateY;
                transformGroup.Children.Add(translateTransform);
                popupChild.RenderTransform = transformGroup;
            }
#endif
            this.ScreenShotContainer.Width = targetElement.ActualWidth;
            this.ScreenShotContainer.Height = targetElement.ActualHeight;

            GeneralTransform transfrom = targetElement.TransformToVisual(rootVisual);
            this.OriginalLocation = transfrom.TransformPoint(new Point(0, 0));
            popupChild.Children.Add(this.ScreenShotContainer);
            Canvas.SetLeft(this.ScreenShotContainer, this.OriginalLocation.X);
            Canvas.SetTop(this.ScreenShotContainer, this.OriginalLocation.Y);
            this.Popup.Child = popupChild;
            this.OriginalOpacity = targetElement.Opacity;
            targetElement.Opacity = 0;
        }

        /// <summary>
        /// Gets or sets the original opacity.
        /// </summary>
        /// <value>The original opacity.</value>
        public double OriginalOpacity { get; set; }

        /// <summary>
        /// Gets or sets the original location.
        /// </summary>
        /// <value>The original location.</value>
        public Point OriginalLocation { get; set; }

        /// <summary>
        /// Gets or sets the popup.
        /// </summary>
        /// <value>The popup.</value>
        public Popup Popup { get; set; }

        /// <summary>
        /// Gets or sets the screen shot container.
        /// </summary>
        /// <value>The screen shot container.</value>
        public Rectangle ScreenShotContainer { get; set; }

        internal void Dispose()
        {
            if (this.ScreenShotContainer != null)
            {
                this.ScreenShotContainer.Fill = null;
            }

            if (this.Popup != null)
            {
                this.Popup.IsOpen = false;
                this.Popup = null;
            }
        }
    }
}