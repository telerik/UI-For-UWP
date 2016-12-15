using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Provides an animation exact copy of the Windows Phone list based transition.
    /// </summary>
    public class RadSlideContinuumAnimation : RadAnimation, IInOutAnimation
    {
        /// <summary>
        /// Identifies the HeaderElement property.
        /// </summary>
        public static readonly DependencyProperty HeaderElementProperty =
            DependencyProperty.RegisterAttached("HeaderElement", typeof(FrameworkElement), typeof(RadSlideContinuumAnimation), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ApplicationHeaderElement property.
        /// </summary>
        public static readonly DependencyProperty ApplicationHeaderElementProperty =
            DependencyProperty.RegisterAttached("ApplicationHeaderElement", typeof(FrameworkElement), typeof(RadSlideContinuumAnimation), new PropertyMetadata(null));

        private FrameworkElement applicationHeaderElement;
        private FrameworkElement headerElement;
        private RadMoveAndFadeAnimation pageAnimation;
        private RadMoveAndFadeAnimation applicationHeaderElementAnimation;
        private RadMoveAndFadeAnimation headerElementAnimation;
        private Duration totalDuration = new Duration(TimeSpan.FromMilliseconds(100));

        private ElementScreenShotInfo headerElementScreenShotInfo;
        private ElementScreenShotInfo applicationHeaderElementScreenShotInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadSlideContinuumAnimation"/> class.
        /// </summary>
        public RadSlideContinuumAnimation()
        {
            this.InitialDelay = TimeSpan.FromMilliseconds(0);
            this.FillBehavior = AnimationFillBehavior.HoldEnd;
        }

        /// <summary>
        /// Gets or sets the in out animation mode.
        /// </summary>
        /// <value>The in out animation mode.</value>
        public InOutAnimationMode InOutAnimationMode
        {
            get;
            set;
        }

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
        /// Gets the ApplicationHeaderElement dependency property from an object.
        /// </summary>
        /// <param name="source">The object to get the property from.</param>
        /// <returns>The property's value.</returns>
        public static FrameworkElement GetApplicationHeaderElement(DependencyObject source)
        {
            if (source == null)
            {
                return null;
            }

            return (FrameworkElement)source.GetValue(ApplicationHeaderElementProperty);
        }

        /// <summary>
        /// Sets the ApplicationHeaderElement dependency property on an object.
        /// </summary>
        /// <param name="source">The object to set the property on.</param>
        /// <param name="value">The value to set.</param>
        public static void SetApplicationHeaderElement(DependencyObject source, FrameworkElement value)
        {
            if (source == null)
            {
                return;
            }

            source.SetValue(ApplicationHeaderElementProperty, value);
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadSlideContinuumAnimation opposite = base.CreateOpposite() as RadSlideContinuumAnimation;
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

            if (this.applicationHeaderElementAnimation != null)
            {
                this.applicationHeaderElementAnimation.ClearAnimation(target);
            }

            base.ClearAnimation(target);
        }

        /// <inheritdoc/>
        protected internal override void OnStarted(PlayAnimationInfo info)
        {
            if (this.headerElementScreenShotInfo != null)
            {
                RadAnimationManager.Play(this.headerElementScreenShotInfo.ScreenShotContainer, this.headerElementAnimation);
            }

            if (this.applicationHeaderElementScreenShotInfo != null)
            {
                RadAnimationManager.Play(this.applicationHeaderElementScreenShotInfo.ScreenShotContainer, this.applicationHeaderElementAnimation);
            }

            if (info != null && this.pageAnimation != null)
            {
                RadAnimationManager.Play(info.Target, this.pageAnimation);
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

            context.Storyboard.Duration = this.totalDuration;
            if (this.InOutAnimationMode == InOutAnimationMode.In)
            {
                this.applicationHeaderElement = RadSlideContinuumAnimation.GetApplicationHeaderElement(context.Target);
                this.headerElement = RadSlideContinuumAnimation.GetHeaderElement(context.Target);

                if (this.headerElement != null)
                {
                    this.headerElementScreenShotInfo = new ElementScreenShotInfo(this.headerElement);
                    this.headerElementScreenShotInfo.Popup.IsOpen = true;
                    this.ApplyPageHeaderAnimation();
                }

                if (this.applicationHeaderElement != null)
                {
                    this.applicationHeaderElementScreenShotInfo = new ElementScreenShotInfo(this.applicationHeaderElement);
                    this.applicationHeaderElementScreenShotInfo.Popup.IsOpen = true;
                    this.ApplyApplicationHeaderElementAnimation();
                }
            }

            this.ApplyPageAnimation();
            base.UpdateAnimationOverride(context);
        }

        private void ApplyApplicationHeaderElementAnimation()
        {
            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                return;
            }

            QuadraticEase easing = new QuadraticEase();
            easing.EasingMode = EasingMode.EaseOut;

            this.applicationHeaderElementAnimation = new RadMoveAndFadeAnimation();

            Point startPoint = new Point(this.applicationHeaderElement.ActualWidth, -1 * (this.applicationHeaderElement.ActualHeight + this.applicationHeaderElementScreenShotInfo.OriginalLocation.Y));
            Point endPoint = new Point(0, 0);

            this.applicationHeaderElementAnimation.MoveAnimation.StartPoint = startPoint;
            this.applicationHeaderElementAnimation.MoveAnimation.EndPoint = endPoint;
            this.applicationHeaderElementAnimation.FadeAnimation.StartOpacity = 0;
            this.applicationHeaderElementAnimation.FadeAnimation.EndOpacity = 1;
            this.applicationHeaderElementAnimation.Easing = easing;
            this.applicationHeaderElementAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(260));
            this.applicationHeaderElementAnimation.Ended += this.ApplicationHeaderElementAnimation_Ended;
        }

        private void ApplicationHeaderElementAnimation_Ended(object sender, EventArgs e)
        {
            (sender as RadAnimation).Ended -= this.ApplicationHeaderElementAnimation_Ended;
            this.DisposeApplicationHeaderElementAnimation();
        }

        private void DisposeApplicationHeaderElementAnimation()
        {
            if (this.applicationHeaderElement != null)
            {
                this.applicationHeaderElement.Opacity = this.applicationHeaderElementScreenShotInfo.OriginalOpacity;
            }

            this.applicationHeaderElementScreenShotInfo.Dispose();
        }

        private void DisposeHeaderElementAnimation()
        {
            if (this.headerElement != null)
            {
                this.headerElement.Opacity = this.headerElementScreenShotInfo.OriginalOpacity;
            }

            this.headerElementScreenShotInfo.Dispose();
        }

        private void ApplyPageHeaderAnimation()
        {
            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                return;
            }

            this.headerElementAnimation = new RadMoveAndFadeAnimation();
            this.headerElementScreenShotInfo.ScreenShotContainer.Opacity = 0;
            this.headerElementAnimation.FadeAnimation.StartOpacity = 0;
            this.headerElementAnimation.FadeAnimation.EndOpacity = this.headerElementScreenShotInfo.OriginalOpacity;
            Point startPoint = new Point(0, 250);
            Point endPoint = new Point(0, 0);

            this.headerElementAnimation.MoveAnimation.StartPoint = startPoint;
            this.headerElementAnimation.MoveAnimation.EndPoint = endPoint;

            this.headerElementAnimation.InitialDelay = TimeSpan.FromMilliseconds(70); //// totalDuration.TimeSpan.TotalMilliseconds/2);
            this.headerElementAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(190)); //// new Duration(TimeSpan.FromMilliseconds(totalDuration.TimeSpan.TotalMilliseconds / 2));

            ExponentialEase easing = new ExponentialEase();
            easing.EasingMode = EasingMode.EaseOut;

            this.headerElementAnimation.Easing = easing;
            this.headerElementAnimation.Ended += this.HeaderElementAnimation_Ended;
        }

        private void HeaderElementAnimation_Ended(object sender, EventArgs e)
        {
            (sender as RadAnimation).Ended -= this.HeaderElementAnimation_Ended;
            this.DisposeHeaderElementAnimation();
        }

        private void ApplyPageAnimation()
        {
            this.pageAnimation = new RadMoveAndFadeAnimation();
            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                this.pageAnimation.FadeAnimation.StartOpacity = 1;
                this.pageAnimation.FadeAnimation.EndOpacity = 0;

                this.pageAnimation.MoveAnimation.EndPoint = new Point(0, 200);
                this.pageAnimation.MoveAnimation.StartPoint = new Point(0, 0);
                this.pageAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            }
            else
            {
                this.pageAnimation.FadeAnimation.StartOpacity = 0;
                this.pageAnimation.FadeAnimation.EndOpacity = 1;

                this.pageAnimation.MoveAnimation.StartPoint = new Point(0, 200);
                this.pageAnimation.MoveAnimation.EndPoint = new Point(0, 0);
                this.pageAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            }
        }
    }
}