using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Provides an animation that sequentially animates items in a container. The end effect is
    /// similar to the animation of the home screen on the Windows Phone OS.
    /// </summary>
    public class RadTileAnimation : RadAnimation, IInOutAnimation
    {
        /// <summary>
        /// Identifies the ContainerToAnimate property.
        /// </summary>
        public static readonly DependencyProperty ContainerToAnimateProperty =
            DependencyProperty.RegisterAttached("ContainerToAnimate", typeof(FrameworkElement), typeof(RadTileAnimation), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ElementToDelay property.
        /// </summary>
        public static readonly DependencyProperty ElementToDelayProperty =
            DependencyProperty.RegisterAttached("ElementToDelay", typeof(FrameworkElement), typeof(RadTileAnimation), new PropertyMetadata(null));

        private Collection<FrameworkElement> itemsToAnimate;
        private Dictionary<FrameworkElement, Transform> itemsOriginalTransform;
        private FrameworkElement containerToAnimate;
        private FrameworkElement elementToDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadTileAnimation"/> class.
        /// </summary>
        public RadTileAnimation()
        {
            this.SequentialItemDelay = TimeSpan.FromMilliseconds(30);
            this.ItemAnimationDuration = TimeSpan.FromMilliseconds(260);
            this.SelectedItemDelay = TimeSpan.FromMilliseconds(200);
            this.InitialDelay = TimeSpan.FromMilliseconds(0);
            this.FillBehavior = AnimationFillBehavior.HoldEnd;
            this.PerspectiveAngleY = 60;
            this.PerspectiveAngleX = 0;
            this.IsTranslationEnabled = true;
            this.SequentialMode = SequentialMode.LastToFirst;
            this.itemsOriginalTransform = new Dictionary<FrameworkElement, Transform>();
        }

        /// <summary>
        /// Gets or sets the in out animation mode.
        /// </summary>
        /// <value>The in out animation mode.</value>
        public InOutAnimationMode InOutAnimationMode { get; set; }

        /// <summary>
        /// Gets or sets the starting angle of the perspective rotation along the Y axis.
        /// </summary>
        public double PerspectiveAngleY { get; set; }

        /// <summary>
        /// Gets or sets the starting angle of the perspective rotation along the X axis.
        /// </summary>
        public double PerspectiveAngleX { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether plane projection will be combined with a translate transform to enable the Turnstile effect.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsTranslationEnabled { get; set; }

        /// <summary>
        /// Gets or sets the delay between the items start animation.
        /// </summary>
        public TimeSpan SequentialItemDelay { get; set; }

        /// <summary>
        /// Gets or sets the sequential mode.
        /// </summary>
        public SequentialMode SequentialMode { get; set; }

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
        /// Gets the ContainerToAnimate dependency property from an object.
        /// </summary>
        /// <param name="source">The object to get the property from.</param>
        /// <returns>The property's value.</returns>
        public static FrameworkElement GetContainerToAnimate(DependencyObject source)
        {
            if (source == null)
            {
                return null;
            }

            return (FrameworkElement)source.GetValue(ContainerToAnimateProperty);
        }

        /// <summary>
        /// Sets the ContainerToAnimate dependency property on an object.
        /// </summary>
        /// <param name="source">The object to set the property on.</param>
        /// <param name="value">The value to set.</param>
        public static void SetContainerToAnimate(DependencyObject source, FrameworkElement value)
        {
            if (source == null)
            {
                return;
            }

            source.SetValue(ContainerToAnimateProperty, value);
        }

        /// <summary>
        /// Gets the ElementToDelay dependency property from an object.
        /// </summary>
        /// <param name="source">The object to get the property from.</param>
        /// <returns>The property's value.</returns>
        public static FrameworkElement GetElementToDelay(DependencyObject source)
        {
            if (source == null)
            {
                return null;
            }

            return (FrameworkElement)source.GetValue(ElementToDelayProperty);
        }

        /// <summary>
        /// Sets the ElementToDelay dependency property on an object.
        /// </summary>
        /// <param name="source">The object to set the property on.</param>
        /// <param name="value">The value to set.</param>
        public static void SetElementToDelay(DependencyObject source, FrameworkElement value)
        {
            if (source == null)
            {
                return;
            }

            source.SetValue(ElementToDelayProperty, value);
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadTileAnimation opposite = base.CreateOpposite() as RadTileAnimation;
            opposite.PerspectiveAngleY = -1 * opposite.PerspectiveAngleY;
            opposite.PerspectiveAngleX = -1 * opposite.PerspectiveAngleX;
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
            if (this.itemsToAnimate != null)
            {
                for (int i = 0; i < this.itemsToAnimate.Count; i++)
                {
                    FrameworkElement itemToAnimate = this.itemsToAnimate[i];
                    this.SetItemOriginalTransform(itemToAnimate);
                }
            }

            base.ClearAnimation(target);
        }

        /// <inheritdoc/>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            if (this.itemsToAnimate != null)
            {
                for (int i = 0; i < this.itemsToAnimate.Count; i++)
                {
                    FrameworkElement itemToAnimate = this.itemsToAnimate[i];
                    this.SetItemOriginalTransform(itemToAnimate);
                }
            }

            base.ApplyAnimationValues(info);
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

            this.containerToAnimate = RadTileAnimation.GetContainerToAnimate(context.Target);
            if (this.containerToAnimate == null)
            {
                // TODO - try cast the context.Target to ITemsControl or panel
                return;
            }

            this.itemsToAnimate = this.GetItemsToAnimate();
            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                this.elementToDelay = RadTileAnimation.GetElementToDelay(context.Target);
                if (this.elementToDelay == null)
                {
                    this.elementToDelay = this.FindElementToDelay();
                }

                // move the delayed element to the end of the sequence
                if (this.elementToDelay != null && this.itemsToAnimate.Remove(this.elementToDelay))
                {
                    this.itemsToAnimate.Add(this.elementToDelay);
                }
            }

            for (int i = 0; i < this.itemsToAnimate.Count; i++)
            {
                FrameworkElement itemToAnimate = this.itemsToAnimate[i];

                TimeSpan animationDelay = TimeSpan.FromMilliseconds((this.SequentialItemDelay.TotalMilliseconds * i) + this.InitialDelay.TotalMilliseconds);

                if (this.InOutAnimationMode == InOutAnimationMode.Out && itemToAnimate == this.elementToDelay)
                {
                    animationDelay = animationDelay.Add(this.SelectedItemDelay);
                }

                this.ApplyItemProjectionAnimation(itemToAnimate, context, animationDelay);

                this.ApplyItemOpacityAnimation(itemToAnimate, context, animationDelay);
            }

            base.UpdateAnimationOverride(context);
        }

        /// <inheritdoc/>
        protected override void OnEnded(PlayAnimationInfo info)
        {
            this.itemsOriginalTransform.Clear();
            this.containerToAnimate = null;
            this.elementToDelay = null;

            if (this.itemsToAnimate != null)
            {
                this.itemsToAnimate.Clear();
                this.itemsToAnimate = null;
            }

            base.OnEnded(info);
        }

        private FrameworkElement FindElementToDelay()
        {
            FrameworkElement element = null;
            ListBox listBox = this.containerToAnimate as ListBox;

            if (listBox != null)
            {
                int selectedIndex = listBox.SelectedIndex;
                if (selectedIndex >= 0)
                {
                    element = listBox.ContainerFromIndex(selectedIndex) as FrameworkElement;
                }
            }

            return element;
        }

        private void ApplyItemProjectionAnimation(FrameworkElement itemToAnimate, AnimationContext context, TimeSpan animationDelay)
        {
            double endProjectionAngleY = this.PerspectiveAngleY;
            double startProjectionAngleY = 0;
            double endProjectionAngleX = this.PerspectiveAngleX;
            double startProjectionAngleX = 0;
            if (this.InOutAnimationMode == InOutAnimationMode.In)
            {
                endProjectionAngleY = 0;
                startProjectionAngleY = -1 * this.PerspectiveAngleY;
                endProjectionAngleX = 0;
                startProjectionAngleX = -1 * this.PerspectiveAngleX;
            }

            PlaneProjection planeProjection = new PlaneProjection();
            planeProjection.CenterOfRotationX = -1;
            planeProjection.RotationY = startProjectionAngleY;
            planeProjection.RotationX = startProjectionAngleX;
            itemToAnimate.Projection = planeProjection;

            if (this.IsTranslationEnabled)
            {
                double offsetY = this.CalculateYOffset(itemToAnimate);
                planeProjection.GlobalOffsetY = offsetY * -1;

                Transform originalTransform = itemToAnimate.RenderTransform;
                this.itemsOriginalTransform[itemToAnimate] = originalTransform;

                TranslateTransform translate = new TranslateTransform();
                translate.Y = offsetY;

                TransformGroup animationTransform = new TransformGroup();
                animationTransform.Children.Add(originalTransform);
                animationTransform.Children.Add(translate);

                itemToAnimate.RenderTransform = animationTransform;
            }

            DoubleAnimation projectionAnimationY = new DoubleAnimation();
            projectionAnimationY.BeginTime = animationDelay;
            projectionAnimationY.Duration = this.ItemAnimationDuration;
            projectionAnimationY.To = endProjectionAngleY;

            projectionAnimationY.EasingFunction = this.GetEasingFunction();
            Storyboard.SetTarget(projectionAnimationY, itemToAnimate);
            Storyboard.SetTargetProperty(projectionAnimationY, "(UIElement.Projection).(PlaneProjection.RotationY)");
            context.Storyboard.Children.Add(projectionAnimationY);

            DoubleAnimation projectionAnimationX = new DoubleAnimation();
            projectionAnimationX.BeginTime = animationDelay;
            projectionAnimationX.Duration = this.ItemAnimationDuration;
            projectionAnimationX.To = endProjectionAngleX;

            projectionAnimationX.EasingFunction = this.GetEasingFunction();
            Storyboard.SetTarget(projectionAnimationX, itemToAnimate);
            Storyboard.SetTargetProperty(projectionAnimationX, "(UIElement.Projection).(PlaneProjection.RotationX)");
            context.Storyboard.Children.Add(projectionAnimationX);
        }

        private EasingFunctionBase GetEasingFunction()
        {
            if (this.Easing != null)
            {
                return this.Easing;
            }

            if (this.InOutAnimationMode == InOutAnimationMode.In)
            {
                QuadraticEase easingIn = new QuadraticEase();
                easingIn.EasingMode = EasingMode.EaseOut;
                return easingIn;
            }

            ExponentialEase easingOut = new ExponentialEase();
            easingOut.EasingMode = EasingMode.EaseIn;
            easingOut.Exponent = 6;
            return easingOut;
        }

        private double CalculateYOffset(FrameworkElement itemToAnimate)
        {
            FrameworkElement rootElement = this.containerToAnimate; // TODO - get the root
            Point globalCoords = ElementTreeHelper.SafeTransformPoint(itemToAnimate, rootElement, new Point());

            double heightAdjustment = itemToAnimate.ActualHeight / 2;
            double yCoord = globalCoords.Y + heightAdjustment;

            return (rootElement.ActualHeight / 2) - yCoord;
        }

        private void ApplyItemOpacityAnimation(FrameworkElement itemToAnimate, AnimationContext context, TimeSpan animationDelay)
        {
            double endOpacity = 1;
            double startOpacity = 0;
            TimeSpan beginTime = animationDelay;
            TimeSpan duration = TimeSpan.FromMilliseconds(200);
            if (this.InOutAnimationMode == InOutAnimationMode.Out)
            {
                beginTime = beginTime.Add(this.ItemAnimationDuration.TimeSpan);
                duration = TimeSpan.FromMilliseconds(10);
                endOpacity = 0;
                startOpacity = 1;
            }

            itemToAnimate.Opacity = startOpacity;
            DoubleAnimation opacityAnimation = new DoubleAnimation();

            opacityAnimation.To = endOpacity;

            opacityAnimation.Duration = duration;
            opacityAnimation.BeginTime = beginTime;

            Storyboard.SetTarget(opacityAnimation, itemToAnimate);
            Storyboard.SetTargetProperty(opacityAnimation, "(UIElement.Opacity)");
            context.Storyboard.Children.Add(opacityAnimation);
        }

        private Collection<FrameworkElement> GetItemsToAnimate()
        {
            Collection<FrameworkElement> items = new Collection<FrameworkElement>();
            ItemsControl itemsControl = this.containerToAnimate as ItemsControl;
            if (itemsControl != null)
            {
                // TODO - visible items only
                for (int i = 0; i < itemsControl.Items.Count; i++)
                {
                    FrameworkElement itemToAdd = itemsControl.ContainerFromIndex(i) as FrameworkElement;
                    if (itemToAdd == null || !this.IsItemVisible(itemToAdd))
                    {
                        continue;
                    }

                    if (this.SequentialMode == SequentialMode.FirstToLast)
                    {
                        items.Add(itemToAdd);
                    }
                    else
                    {
                        items.Insert(0, itemToAdd);
                    }
                }
            }
            else
            {
                if (this.containerToAnimate is IItemsContainer)
                {
                    FrameworkElement[] viewportItems = ((IItemsContainer)this.containerToAnimate).ViewportItems;
                    for (int i = 0; i < viewportItems.Length; i++)
                    {
                        FrameworkElement itemToAdd = viewportItems[i];
                        if (this.SequentialMode == SequentialMode.FirstToLast)
                        {
                            items.Add(itemToAdd);
                        }
                        else
                        {
                            items.Insert(0, itemToAdd);
                        }
                    }
                }
                else if (this.containerToAnimate is Panel)
                {
                    UIElementCollection children = (this.containerToAnimate as Panel).Children;
                    for (int i = 0; i < children.Count; i++)
                    {
                        FrameworkElement itemToAdd = children[i] as FrameworkElement;
                        if (!this.IsItemVisible(itemToAdd))
                        {
                            continue;
                        }

                        if (this.SequentialMode == SequentialMode.FirstToLast)
                        {
                            items.Add(itemToAdd);
                        }
                        else
                        {
                            items.Insert(0, itemToAdd);
                        }
                    }
                }
            }

            return items;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private bool IsItemVisible(FrameworkElement itemToAdd)
        {
            if (itemToAdd == null || itemToAdd.Visibility == Visibility.Collapsed)
            {
                return false;
            }

            Size renderSize = itemToAdd.RenderSize;
            if (renderSize.Width == 0 || renderSize.Height == 0)
            {
                return false;
            }

            FrameworkElement rootVisual = Window.Current.Content as FrameworkElement;

            Point globalPosition = ElementTreeHelper.SafeTransformPoint(itemToAdd, rootVisual, new Point());

            double itemWidth = itemToAdd.Width;
            double itemHeight = itemToAdd.Height;

            if ((globalPosition.X + itemWidth) < 0 ||
                (globalPosition.Y + itemHeight) < 0 ||
                globalPosition.X > rootVisual.ActualWidth ||
                globalPosition.Y > rootVisual.ActualHeight)
            {
                return false;
            }

            ScrollViewer parentScrollViewer = ElementTreeHelper.FindVisualAncestor<ScrollViewer>(itemToAdd);
            if (parentScrollViewer != null)
            {
                Point localPosition = ElementTreeHelper.SafeTransformPoint(itemToAdd, parentScrollViewer, new Point());
                if ((localPosition.X + itemWidth) < 0 ||
                    (localPosition.Y + itemHeight) < 0 ||
                    localPosition.X > parentScrollViewer.ActualWidth ||
                    localPosition.Y > parentScrollViewer.ActualHeight)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetItemOriginalTransform(FrameworkElement itemToAnimate)
        {
            if (this.IsTranslationEnabled)
            {
                Transform originalTransform;
                if (!this.itemsOriginalTransform.TryGetValue(itemToAnimate, out originalTransform))
                {
                    Debug.Assert(false, "Must have the original transform here.");
                }

                itemToAnimate.RenderTransform = originalTransform;
            }

            itemToAnimate.Projection = null;
            itemToAnimate.ClearValue(UIElement.OpacityProperty);
        }
    }
}