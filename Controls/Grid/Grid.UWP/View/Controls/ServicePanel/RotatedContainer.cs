using System;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a container control that can rotate its child content and apply the rotation transformation to its desired size.
    /// </summary>
    [ContentProperty(Name = "Child")]
    public class RotatedContainer : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Child"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register(nameof(Child), typeof(UIElement), typeof(RotatedContainer), new PropertyMetadata(null, OnChildChanged));

        /// <summary>
        /// Identifies the <see cref="RotationAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register(nameof(RotationAngle), typeof(double), typeof(RotatedContainer), new PropertyMetadata(0d, OnRotationAngleChanged));

        private Size childDesiredSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotatedContainer" /> class.
        /// </summary>
        public RotatedContainer()
        {
            this.DefaultStyleKey = typeof(RotatedContainer);
        }

        /// <summary>
        /// Gets or sets the <see cref="UIElement"/> instance that is parented by this container.
        /// </summary>
        public UIElement Child
        {
            get
            {
                return this.GetValue(ChildProperty) as UIElement;
            }
            set
            {
                this.SetValue(ChildProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the angle to rotate the <see cref="Child"/> by.
        /// </summary>
        public double RotationAngle
        {
            get
            {
                return (double)this.GetValue(RotationAngleProperty);
            }
            set
            {
                this.SetValue(RotationAngleProperty, value);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.UpdateChildTransform();
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            var baseSize = base.MeasureOverride(availableSize);
            var desiredSize = baseSize;

            var child = this.Child;
            if (child != null)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                this.childDesiredSize = child.DesiredSize;

                var bounds = new Rect(0, 0, this.childDesiredSize.Width, this.childDesiredSize.Height);
                bounds = child.RenderTransform.TransformBounds(bounds);

                desiredSize = new Size(bounds.Width, bounds.Height);
            }

            return desiredSize;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var child = this.Child;
            if (child != null)
            {
                var maxWidth = Math.Max(finalSize.Width, this.childDesiredSize.Width);
                var maxHeight = Math.Max(finalSize.Height, this.childDesiredSize.Height);

                var left = 0d;
                if (this.childDesiredSize.Width > finalSize.Width)
                {
                    left = (this.childDesiredSize.Width - finalSize.Width) / 2;
                }

                child.Arrange(new Rect(-left, 0, maxWidth, maxHeight));
            }

            return finalSize;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RotatedContainerAutomationPeer(this);
        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RotatedContainer;
            control.UpdateChildTransform();
        }

        private static void OnRotationAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RotatedContainer;
            control.UpdateChildTransform();
        }

        private void UpdateChildTransform()
        {
            var child = this.Child;
            if (child != null)
            {
                child.RenderTransform = new RotateTransform() { Angle = this.RotationAngle };
                child.RenderTransformOrigin = new Point(0.5, 0.5);
            }
        }
    }
}
