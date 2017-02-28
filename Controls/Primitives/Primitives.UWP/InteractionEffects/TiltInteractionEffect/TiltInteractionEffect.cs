using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a tilt interaction effect. This effect applies plane projection to the element the user is currently interacting with
    /// to simulate pressure being applied on the element at the interaction point.
    /// </summary>
    public partial class TiltInteractionEffect : InteractionEffectBase
    {
        /// <summary>
        /// Identifies the DedicatedTiltTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty DedicatedTiltTargetProperty =
            DependencyProperty.RegisterAttached("DedicatedTiltTarget", typeof(FrameworkElement), typeof(TiltInteractionEffect), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the MaxRotationAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxRotationAngleProperty =
            DependencyProperty.Register(nameof(MaxRotationAngle), typeof(double), typeof(TiltInteractionEffect), new PropertyMetadata(0.3d));

        /// <summary>
        /// Identifies the MaxZOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxZOffsetProperty =
            DependencyProperty.Register(nameof(MaxZOffset), typeof(double), typeof(TiltInteractionEffect), new PropertyMetadata(25d));

        private static readonly TimeSpan ResetAnimationDelay = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan ResetAnimationDuration = TimeSpan.FromMilliseconds(200);

        private TiltInteractionInfo currentTiltContext = null;

        private Storyboard resetAnimation;
        private DoubleAnimation xRotationAnimation;
        private DoubleAnimation yRotationAnimation;
        private DoubleAnimation zOffsetAnimation;

        private PointerEventHandler elementPointerMovedHandler;
        private PointerEventHandler elementPointerReleasedHandler;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TiltInteractionEffect"/> class.
        /// </summary>
        public TiltInteractionEffect()
        {
            this.elementPointerMovedHandler = new PointerEventHandler(this.OnElementPointerMoved);
            this.elementPointerReleasedHandler = new PointerEventHandler(this.OnElementPointerReleased);
        }

        /// <summary>
        /// Gets or sets the max Z offset.
        /// </summary>
        /// <value>The max Z offset.</value>
        public double MaxZOffset
        {
            get
            {
                return (double)this.GetValue(MaxZOffsetProperty);
            }
            set
            {
                this.SetValue(MaxZOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the max rotation angle.
        /// </summary>
        /// <value>The max rotation angle.</value>
        public double MaxRotationAngle
        {
            get
            {
                return (double)this.GetValue(MaxRotationAngleProperty);
            }
            set
            {
                this.SetValue(MaxRotationAngleProperty, value);
            }
        }

        /// <summary>
        /// Sets the DedicatedTiltTarget attached property to the given <see cref="FrameworkElement"/>.
        /// </summary>
        public static void SetDedicatedTiltTarget(DependencyObject element, object value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(DedicatedTiltTargetProperty, value);
        }

        /// <summary>
        /// Gets the defined DedicatedTiltTarget from the given <see cref="FrameworkElement"/>.
        /// </summary>
        public static FrameworkElement GetDedicatedTiltTarget(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetValue(DedicatedTiltTargetProperty) as FrameworkElement;
        }

        /// <summary>
        /// Cancels the interaction effect applied to the given element.
        /// </summary>
        public override void CancelEffect()
        {
            base.CancelEffect();

            this.EndTilting(false);
        }

        /// <summary>
        /// Called when manipulation has been started on an element subscribed for an interaction effect.
        /// </summary>
        protected override void OnStartEffect(FrameworkElement targetElement, PointerRoutedEventArgs args)
        {
            this.BeginTilting(targetElement, args);
        }

        /// <summary>
        /// Determines whether an effect can be started on the specified target element.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="args">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        /// <returns>
        ///     <c>true</c> if an effect can be started on the specified target element; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanStartEffect(FrameworkElement targetElement, PointerRoutedEventArgs args)
        {
            FrameworkElement elementToTilt = GetElementToTilt(targetElement);
            if (this.currentTiltContext != null && this.currentTiltContext.TargetElement == elementToTilt)
            {
                if (this.currentTiltContext.State == TiltInteractionState.Tilting)
                {
                    return false;
                }
            }

            return true;
        }

        private static FrameworkElement GetElementToTilt(FrameworkElement element)
        {
            // If the ApplyInteractionExplicitly property is set on the target element - apply the tilt directly to this element.
            if (InteractionEffectManager.GetApplyInteractionExplicitly(element))
            {
                return element;
            }

            // If there is a dedicated tilt target assigned - use it; otherwise, use the target element to avoid resetting already existing transformations on it.
            FrameworkElement dedicatedTarget = element.GetValue(DedicatedTiltTargetProperty) as FrameworkElement;
            if (dedicatedTarget != null)
            {
                return dedicatedTarget;
            }

            return element;
        }

        private static void ResetTransformations(FrameworkElement element)
        {
            element.Projection = null;
            element.RenderTransform = null;
        }

        private static bool IsPointInElementBounds(FrameworkElement element, Point position)
        {
            Size emptySize = new Size(0, 0);
            if (element.RenderSize == emptySize || element.DesiredSize == emptySize)
            {
                return false;
            }

            return new Rect(0, 0, element.ActualWidth, element.ActualHeight).Contains(position);
        }

        private static void EnsureTransformations(FrameworkElement element, FrameworkElement container)
        {
            if (element.RenderTransform != null && element.Projection != null)
            {
                return;
            }

            Point elementCenter = new Point(element.ActualWidth / 2, element.ActualHeight / 2);

            Point containerCenter = new Point(container.ActualWidth / 2, container.ActualHeight / 2);

            Size emptySize = new Size(0, 0);
            Point elementCenterInContainerBounds = elementCenter;

            if (element.RenderSize != emptySize && container.RenderSize != emptySize)
            {
                elementCenterInContainerBounds = element.TransformToVisual(container).TransformPoint(elementCenter);
            }

            double xDelta = containerCenter.X - elementCenterInContainerBounds.X;
            double yDelta = containerCenter.Y - elementCenterInContainerBounds.Y;
            TranslateTransform translate = new TranslateTransform();
            translate.X = xDelta;
            translate.Y = yDelta;
            element.RenderTransform = translate;

            PlaneProjection projection = new PlaneProjection();
            projection.GlobalOffsetX = -xDelta;
            projection.GlobalOffsetY = -yDelta;
            element.Projection = projection;
        }

        private void ApplyTilt(FrameworkElement element, Point manipulationPoint)
        {
            Point normalizedPoint = new Point(
                Math.Min(Math.Max(manipulationPoint.X / element.ActualWidth, 0), 1),
                Math.Min(Math.Max(manipulationPoint.Y / element.ActualHeight, 0), 1));

            double xMagnitude = Math.Abs(normalizedPoint.X - 0.5);
            double yMagnitude = Math.Abs(normalizedPoint.Y - 0.5);
            double xDirection = -Math.Sign(normalizedPoint.X - 0.5);
            double yDirection = Math.Sign(normalizedPoint.Y - 0.5);
            double angleMagnitude = xMagnitude + yMagnitude;
            double xAngleContribution = xMagnitude + yMagnitude > 0 ? xMagnitude / (xMagnitude + yMagnitude) : 0;

            double angle = angleMagnitude * this.MaxRotationAngle * 180 / Math.PI;
            double depression = (1 - angleMagnitude) * this.MaxZOffset;

            PlaneProjection projection = element.Projection as PlaneProjection;
            projection.RotationY = angle * xAngleContribution * xDirection;
            projection.RotationX = angle * (1 - xAngleContribution) * yDirection;
            projection.GlobalOffsetZ = -depression;
        }

        private Storyboard CreateResetAnimation(FrameworkElement element, FrameworkElement container)
        {
            EnsureTransformations(element, container);

            if (this.resetAnimation == null)
            {
                this.resetAnimation = new Storyboard();
            }

            if (this.xRotationAnimation == null)
            {
                this.xRotationAnimation = new DoubleAnimation();
                this.xRotationAnimation.To = 0;
                this.xRotationAnimation.Duration = ResetAnimationDuration;
                this.xRotationAnimation.BeginTime = ResetAnimationDelay;
                this.resetAnimation.Children.Add(this.xRotationAnimation);
            }

            Storyboard.SetTargetProperty(this.xRotationAnimation, "RotationX");
            Storyboard.SetTarget(this.xRotationAnimation, element.Projection);

            if (this.yRotationAnimation == null)
            {
                this.yRotationAnimation = new DoubleAnimation();
                this.yRotationAnimation.Duration = ResetAnimationDuration;
                this.yRotationAnimation.BeginTime = ResetAnimationDelay;
                this.yRotationAnimation.To = 0;
                this.resetAnimation.Children.Add(this.yRotationAnimation);
            }

            Storyboard.SetTargetProperty(this.yRotationAnimation, "RotationY");
            Storyboard.SetTarget(this.yRotationAnimation, element.Projection);

            if (this.zOffsetAnimation == null)
            {
                this.zOffsetAnimation = new DoubleAnimation();
                this.zOffsetAnimation.Duration = ResetAnimationDuration;
                this.zOffsetAnimation.BeginTime = ResetAnimationDelay;
                this.zOffsetAnimation.To = 0;
                this.resetAnimation.Children.Add(this.zOffsetAnimation);
            }

            Storyboard.SetTargetProperty(this.zOffsetAnimation, "GlobalOffsetZ");
            Storyboard.SetTarget(this.zOffsetAnimation, element.Projection);

            return this.resetAnimation;
        }

        private void OnElementPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.EndTilting(true);
        }

        private void OnElementPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            FrameworkElement targetElement = this.currentTiltContext.TargetElement;
            Point position = args.GetCurrentPoint(targetElement).Position;

            if (!args.Handled && IsPointInElementBounds(targetElement, position))
            {
                this.ApplyTilt(targetElement, position);
            }
            else
            {
                this.EndTilting(true);
            }
        }

        private void BeginTilting(FrameworkElement element, PointerRoutedEventArgs args)
        {
            FrameworkElement elementToTilt = GetElementToTilt(element);

            if (this.currentTiltContext != null)
            {
                this.EndTilting(false);
            }

            Point origin = args.GetCurrentPoint(elementToTilt).Position;
            this.currentTiltContext = new TiltInteractionInfo(elementToTilt, this.CreateResetAnimation(elementToTilt, elementToTilt));

            elementToTilt.Unloaded += this.OnElementToTilt_Unloaded;
            elementToTilt.PointerCaptureLost += this.OnElementToTilt_PointerCaptureLost;

            this.AttachRootVisualEvents();

            this.ApplyTilt(this.currentTiltContext.TargetElement, origin);
        }

        private void OnElementToTilt_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            // Capture lost is the only event that indicates that another element - like ScrollViewer has captured the mouse.
            this.EndTilting(true);
        }

        private void EndTilting(bool animate)
        {
            // no currently tilted element
            if (this.currentTiltContext == null)
            {
                return;
            }

            this.currentTiltContext.TargetElement.Unloaded -= this.OnElementToTilt_Unloaded;
            this.currentTiltContext.TargetElement.PointerCaptureLost -= this.OnElementToTilt_PointerCaptureLost;

            this.DetachRootVisualEvents();

            this.currentTiltContext.State = TiltInteractionState.EndingTilting;
            this.currentTiltContext.Clear(animate);

            if (!animate)
            {
                this.currentTiltContext = null;
            }
        }

        private void OnElementToTilt_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).Unloaded -= this.OnElementToTilt_Unloaded;
            this.EndTilting(false);
        }

        private void OnApplicationRootVisual_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.currentTiltContext == null)
            {
                return;
            }

            this.DetachRootVisualEvents();
        }

        private void AttachRootVisualEvents()
        {
            this.currentTiltContext.RootVisual.Unloaded += this.OnApplicationRootVisual_Unloaded;
            this.currentTiltContext.RootVisual.AddHandler(FrameworkElement.PointerMovedEvent, this.elementPointerMovedHandler, true);
            this.currentTiltContext.RootVisual.AddHandler(FrameworkElement.PointerReleasedEvent, this.elementPointerReleasedHandler, true);
        }

        private void DetachRootVisualEvents()
        {
            this.currentTiltContext.RootVisual.Unloaded -= this.OnApplicationRootVisual_Unloaded;
            this.currentTiltContext.RootVisual.RemoveHandler(FrameworkElement.PointerMovedEvent, this.elementPointerMovedHandler);
            this.currentTiltContext.RootVisual.RemoveHandler(FrameworkElement.PointerReleasedEvent, this.elementPointerReleasedHandler);
        }
    }
}
