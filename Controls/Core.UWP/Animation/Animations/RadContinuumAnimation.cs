using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Provides an animation exact copy of the Windows Phone list based transition.
    /// </summary>
    public class RadContinuumAnimation : RadAnimation, IInOutAnimation
    {
        /// <summary>
        /// Identifies the HeaderElement property.
        /// </summary>
        public static readonly DependencyProperty HeaderElementProperty =
            DependencyProperty.RegisterAttached("HeaderElement", typeof(FrameworkElement), typeof(RadContinuumAnimation), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ContinuumElement property.
        /// </summary>
        public static readonly DependencyProperty ContinuumElementProperty =
            DependencyProperty.RegisterAttached("ContinuumElement", typeof(FrameworkElement), typeof(RadContinuumAnimation), new PropertyMetadata(null));

        private FrameworkElement continuumElement;
        private FrameworkElement headerElement;
        private RadFadeAnimation pageAnimation;
        private RadMoveAndFadeAnimation continuumElementAnimation;
        private RadMoveAndFadeAnimation headerElementAnimation;

        private ElementScreenShotInfo headerElementScreenShotInfo;
        private ElementScreenShotInfo continuumElementScreenShotInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadContinuumAnimation"/> class.
        /// </summary>
        public RadContinuumAnimation()
        {
            this.ItemAnimationDuration = TimeSpan.FromMilliseconds(160);
            this.SelectedItemDelay = TimeSpan.FromMilliseconds(100);
            this.InitialDelay = TimeSpan.FromMilliseconds(0);
            this.FillBehavior = AnimationFillBehavior.HoldEnd;
        }

        /// <summary>
        /// Gets or sets the in out animation mode.
        /// </summary>
        /// <value>The in out animation mode.</value>
        public InOutAnimationMode InOutAnimationMode { get; set; }

        /// <summary>
        /// Gets or sets the selected item delay.
        /// </summary>
        /// <value>The selected item delay.</value>
        public TimeSpan SelectedItemDelay { get; set; }

        /// <summary>
        /// Gets or sets the duration of the item animation.
        /// </summary>
        /// <value>The duration of the item animation.</value>
        public Duration ItemAnimationDuration { get; set; }

        /// <summary>
        /// Gets the PageHeader dependency property from an object.
        /// </summary>
        /// <param name="source">The object to get the property from.</param>
        /// <returns>The property's value.</returns>
        public static FrameworkElement GetHeaderElement(DependencyObject source)
        {
            if (source == null)
            {
                return null;
            }

            return (FrameworkElement)source.GetValue(HeaderElementProperty);
        }

        /// <summary>
        /// Sets the PageHeader dependency property on an object.
        /// </summary>
        /// <param name="source">The object to set the property on.</param>
        /// <param name="value">The value to set.</param>
        public static void SetPageHeader(DependencyObject source, FrameworkElement value)
        {
            if (source == null)
            {
                return;
            }

            source.SetValue(HeaderElementProperty, value);
        }

        /// <summary>
        /// Gets the ContinuumElement dependency property from an object.
        /// </summary>
        /// <param name="source">The object to get the property from.</param>
        /// <returns>The property's value.</returns>
        public static FrameworkElement GetContinuumElement(DependencyObject source)
        {
            if (source == null)
            {
                return null;
            }

            return (FrameworkElement)source.GetValue(ContinuumElementProperty);
        }

        /// <summary>
        /// Sets the ContinuumElement dependency property on an object.
        /// </summary>
        /// <param name="source">The object to set the property on.</param>
        /// <param name="value">The value to set.</param>
        public static void SetContinuumElement(DependencyObject source, FrameworkElement value)
        {
            if (source == null)
            {
                return;
            }

            source.SetValue(ContinuumElementProperty, value);
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadContinuumAnimation opposite = base.CreateOpposite() as RadContinuumAnimation;
            return opposite;
        }

        /// <summary>
        /// Removes any property modifications, applied to the specified element by this instance.
        /// </summary>
        /// <param name="target">The element which property values are to be cleared.</param>
        /// <remarks>
        /// It is assumed that the element has been previously animated by this animation.
        /// </remarks>
        public override void ClearAnimation(UIElement target)
        {
            if (this.pageAnimation != null)
            {
                this.pageAnimation.ClearAnimation(target);
            }

            if (this.headerElementAnimation != null)
            {
                this.headerElementAnimation.ClearAnimation(target);
            }

            if (this.continuumElementAnimation != null)
            {
                this.continuumElementAnimation.ClearAnimation(target);
            }

            base.ClearAnimation(target);
        }

        /// <inheritdoc/>
        protected internal override void OnStarted(PlayAnimationInfo info)
        {
            if (this.continuumElementScreenShotInfo != null)
            {
                RadAnimationManager.Play(this.continuumElementScreenShotInfo.ScreenShotContainer, this.continuumElementAnimation);
            }

            if (info != null && this.pageAnimation != null)
            {
                RadAnimationManager.Play(info.Target, this.pageAnimation);
            }

            if (this.headerElementScreenShotInfo != null)
            {
                RadAnimationManager.Play(this.headerElementScreenShotInfo.ScreenShotContainer, this.headerElementAnimation);
            }

            base.OnStarted(info);
        }

        /// <summary>
        /// Core update routine.
        /// </summary>
        /// <param name="context">The context that holds information about the animation.</param>
        protected override void UpdateAnimationOverride(AnimationContext context)
        {
            if (context == null)
            {
                return;
            }

            this.continuumElement = RadContinuumAnimation.GetContinuumElement(context.Target);
            if (this.continuumElement == null)
            {
                return;
            }

            if (!(this.continuumElement is TextBlock))
            {
                TextBlock textBlock = ElementTreeHelper.FindVisualDescendant<TextBlock>(this.continuumElement);
                if (textBlock != null)
                {
                    this.continuumElement = textBlock;
                }
            }

            /////context.Storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            this.headerElement = RadContinuumAnimation.GetHeaderElement(context.Target);
            if (this.headerElement != null)
            {
                this.headerElementScreenShotInfo = new ElementScreenShotInfo(this.headerElement);
                this.headerElementScreenShotInfo.Popup.IsOpen = true;
            }

            this.continuumElementScreenShotInfo = new ElementScreenShotInfo(this.continuumElement);
            this.continuumElementScreenShotInfo.Popup.IsOpen = true;

            this.ApplyPageAnimation();
            this.ApplyContinuumElementAnimation();
            if (this.headerElement != null)
            {
                this.ApplyPageHeaderAnimation();
            }

            base.UpdateAnimationOverride(context);
        }

        private void ApplyContinuumElementAnimation()
        {
            this.continuumElementAnimation = new RadMoveAndFadeAnimation();
            FrameworkElement rootVisual = Window.Current.Content as FrameworkElement;

            QuadraticEase easing = new QuadraticEase();

            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                double durationInMs = 200;
                double keyTime = 0.2 * durationInMs;
                easing.EasingMode = EasingMode.EaseIn;
                this.continuumElementAnimation.MoveAnimation.StartPoint = new Point(0, 0);
                DoubleKeyFrameCollection middlePointsY = new DoubleKeyFrameCollection();
                middlePointsY.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(keyTime)), Value = 20 });
                this.continuumElementAnimation.MoveAnimation.MiddlePointsYAxis = middlePointsY;

                DoubleKeyFrameCollection middlePointsX = new DoubleKeyFrameCollection();
                middlePointsX.Add(new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(keyTime)), Value = 10 });
                this.continuumElementAnimation.MoveAnimation.MiddlePointsXAxis = middlePointsX;

                this.continuumElementAnimation.MoveAnimation.EndPoint = new Point(rootVisual.ActualWidth, this.continuumElement.ActualHeight);
                this.continuumElementAnimation.FadeAnimation.StartOpacity = 1;
                this.continuumElementAnimation.FadeAnimation.EndOpacity = 0;

                this.continuumElementAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(durationInMs));
            }
            else
            {
                this.continuumElementAnimation.MoveAnimation.StartPoint = new Point((-1 * this.continuumElement.ActualWidth) - this.continuumElementScreenShotInfo.OriginalLocation.X, -2 * this.continuumElement.ActualHeight);
                this.continuumElementAnimation.MoveAnimation.EndPoint = new Point(0, 0);
                this.continuumElementAnimation.FadeAnimation.StartOpacity = 0;
                this.continuumElementAnimation.FadeAnimation.EndOpacity = 1;
                this.continuumElementAnimation.Duration = TimeSpan.FromMilliseconds(200);
                easing.EasingMode = EasingMode.EaseOut;
            }

            this.continuumElementAnimation.Easing = easing;

            this.continuumElementAnimation.Ended += this.ContinuumElementAnimation_Ended;
        }

        private void ContinuumElementAnimation_Ended(object sender, EventArgs e)
        {
            (sender as RadAnimation).Ended -= this.ContinuumElementAnimation_Ended;
            this.DisposeContinuumElementAnimation();
        }

        private void DisposeContinuumElementAnimation()
        {
            if (this.continuumElementScreenShotInfo == null)
            {
                return;
            }

            if (this.continuumElement != null)
            {
                this.continuumElement.Opacity = this.continuumElementScreenShotInfo.OriginalOpacity;
            }

            this.continuumElementScreenShotInfo.Dispose();
        }

        private void DisposeHeaderElementAnimation()
        {
            if (this.headerElementScreenShotInfo == null)
            {
                return;
            }

            if (this.headerElement != null)
            {
                this.headerElement.Opacity = this.headerElementScreenShotInfo.OriginalOpacity;
            }

            this.headerElementScreenShotInfo.Dispose();
        }

        private void ApplyPageHeaderAnimation()
        {
            this.headerElementAnimation = new RadMoveAndFadeAnimation();

            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                this.headerElementAnimation.FadeAnimation.EndOpacity = 0;
                this.headerElementAnimation.InitialDelay = TimeSpan.FromMilliseconds(400);
                this.headerElementAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(0));
            }
            else
            {
                this.headerElementAnimation.FadeAnimation.StartOpacity = 0;
                this.headerElementAnimation.FadeAnimation.EndOpacity = this.headerElementScreenShotInfo.OriginalOpacity;

                this.headerElementAnimation.MoveAnimation.StartPoint = new Point(0, -1 * (this.headerElementScreenShotInfo.OriginalLocation.Y + (2 * this.headerElement.ActualHeight)));
                this.headerElementAnimation.MoveAnimation.EndPoint = new Point(0, 0);

                ExponentialEase easing = new ExponentialEase();
                easing.Exponent = 5;
                easing.EasingMode = EasingMode.EaseOut;
                this.headerElementAnimation.Easing = easing;
            }

            this.headerElementAnimation.Ended += this.HeaderElementAnimation_Ended;
        }

        private void HeaderElementAnimation_Ended(object sender, EventArgs e)
        {
            (sender as RadAnimation).Ended -= this.HeaderElementAnimation_Ended;
            this.DisposeHeaderElementAnimation();
        }

        private void ApplyPageAnimation()
        {
            this.pageAnimation = new RadFadeAnimation();
            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                this.pageAnimation.StartOpacity = 1;
                this.pageAnimation.EndOpacity = 0;
            }
            else
            {
                this.pageAnimation.StartOpacity = 0;
                this.pageAnimation.EndOpacity = 1;
            }
        }
    }
}