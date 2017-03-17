using System;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This class represents and indicator in the form of an arrow with a circular tail.
    /// </summary>
     [TemplatePart(Name = "PART_Path", Type = typeof(Path))]
     [TemplatePart(Name = "PART_Layout", Type = typeof(Grid))]
     [TemplatePart(Name = "PART_ArrowHead", Type = typeof(PathFigure))]
     [TemplatePart(Name = "PART_ArrowBottom", Type = typeof(LineSegment))]
     [TemplatePart(Name = "PART_ArrowTop", Type = typeof(LineSegment))]
     [TemplatePart(Name = "PART_ArrowShaft", Type = typeof(LineGeometry))]
     [TemplatePart(Name = "PART_ArrowTail", Type = typeof(EllipseGeometry))]
     [TemplatePart(Name = "PART_NeedleRotateTransform", Type = typeof(RotateTransform))]
    public class ArrowGaugeIndicator : NeedleGaugeIndicator
    {
        /// <summary>
        /// Identifies the ArrowTailRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowTailRadiusProperty =
            DependencyProperty.Register(nameof(ArrowTailRadius), typeof(double), typeof(ArrowGaugeIndicator), new PropertyMetadata(1.0d, OnArrowTailRadiusPropertyChanged));

        private Panel layout;
        private Path path;
        private PathFigure arrowHead;
        private EllipseGeometry arrowTail;
        private LineGeometry arrowShaft;
        private LineSegment arrowTop;
        private LineSegment arrowBottom;

        private double initialTailRadius;

        /// <summary>
        /// Initializes a new instance of the ArrowGaugeIndicator class.
        /// </summary>
        public ArrowGaugeIndicator()
        {
            this.DefaultStyleKey = typeof(ArrowGaugeIndicator);
            this.initialTailRadius = this.ArrowTailRadius;
        }

        /// <summary>
        /// Gets or sets the radius of the arrow tail.
        /// </summary>
        public double ArrowTailRadius
        {
            get
            {
                return (double)this.GetValue(ArrowGaugeIndicator.ArrowTailRadiusProperty);
            }

            set
            {
                this.SetValue(ArrowGaugeIndicator.ArrowTailRadiusProperty, value);
            }
        }

        /// <summary>
        /// This is a virtual method that resets the state of the indicator.
        /// The parent range is responsible to (indirectly) call this method when
        /// a property of importance changes.
        /// </summary>
        /// <param name="availableSize">A size which can be used by the update logic.</param>
        /// <remarks>
        /// The linear range for example triggers the UpdateOverride() method
        /// of its indicators when its Orientation property changes.
        /// </remarks>
        internal override void UpdateOverride(Size availableSize)
        {
            base.UpdateOverride(availableSize);

            this.UpdateArrow(availableSize);
        }

        /// <summary>
        /// Initializes the template parts of the control (see the TemplatePart attributes for more information).
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.layout = this.GetTemplatePartField<Panel>("PART_Layout");
            applied = applied && this.layout != null;

            this.path = this.GetTemplatePartField<Path>("PART_Path");
            applied = applied && this.path != null;

            // TODO: Think of some dynamic way of updating the path without knowing of its parts
            this.arrowHead = this.GetTemplatePartField<PathFigure>("PART_ArrowHead");
            applied = applied && this.arrowHead != null;

            this.arrowTail = this.GetTemplatePartField<EllipseGeometry>("PART_ArrowTail");
            applied = applied && this.arrowTail != null;

            this.arrowShaft = this.GetTemplatePartField<LineGeometry>("PART_ArrowShaft");
            applied = applied && this.arrowShaft != null;

            this.arrowTop = this.GetTemplatePartField<LineSegment>("PART_ArrowTop");
            applied = applied && this.arrowTop != null;

            this.arrowBottom = this.GetTemplatePartField<LineSegment>("PART_ArrowBottom");
            applied = applied && this.arrowBottom != null;

            if (applied)
            {
                this.initialTailRadius = Math.Max(this.arrowTail.RadiusX, this.arrowTail.RadiusY);
            }

            return applied;
        }

        /// <summary>
        /// Called in the arrange pass of the layout system.
        /// </summary>
        /// <param name="finalSize">The final size that was given by the layout system.</param>
        /// <returns>The final size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.UpdateArrow(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ArrowGaugeIndicatorAutomationPeer(this);
        }

        private static void OnArrowTailRadiusPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ArrowGaugeIndicator indicator = sender as ArrowGaugeIndicator;
            if (!indicator.IsTemplateApplied)
            {
                return;
            }

            indicator.InvalidateArrange();
        }

        private void UpdateArrow(Size size)
        {
            double halfExtent = System.Math.Min(size.Width, size.Height) / 2;

            double halfHeight = size.Height / 2;
            double halfWidth = size.Width / 2;

            double left = halfWidth - (halfExtent * this.RadiusScale);

            double scaledTailRadius = this.initialTailRadius * this.ArrowTailRadius;

            this.arrowHead.StartPoint = new Point(left, halfHeight);
            this.arrowTail.Center = new Point(halfWidth, halfHeight);
            this.arrowTail.RadiusX = scaledTailRadius;
            this.arrowTail.RadiusY = scaledTailRadius;
            this.arrowTop.Point = new Point(left + 6, halfHeight - 4);
            this.arrowBottom.Point = new Point(left + 6, halfHeight + 4);
            this.arrowShaft.StartPoint = new Point(left + 5, halfHeight);
            this.arrowShaft.EndPoint = this.arrowTail.Center;
        }
    }
}
