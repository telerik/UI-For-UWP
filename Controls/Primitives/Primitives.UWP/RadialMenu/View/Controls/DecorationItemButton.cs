using System;
using System.Globalization;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Represents the custom Control implementation used to visualize the hover and selected state over <see cref="RadialMenuItem"/>.
    /// </summary>
    public class DecorationItemButton : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="ThicknessFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThicknessFactorProperty =
           DependencyProperty.Register(nameof(ThicknessFactor), typeof(double), typeof(DecorationItemButton), new PropertyMetadata(0.2d, OnThicknessFactorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ArrowThicknessFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowThicknessFactorProperty =
         DependencyProperty.Register(nameof(ArrowThicknessFactor), typeof(double), typeof(DecorationItemButton), new PropertyMetadata(0.2d, OnArrowThicknessFactorPropertyChanged));

        internal Path arrowGlyph;
        internal Path selectedPath;
        internal Path pointerOverPath;
        internal RadialSegment segment;
        internal RadialLayoutSlot layoutSlot;

        private bool isPointerOver;
        private double thicknessFactorCache = 0.2d;
        private double arrowThicknessFactorCache = 0.2d;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecorationItemButton"/> class.
        /// </summary>
        public DecorationItemButton()
        {
            this.DefaultStyleKey = typeof(DecorationItemButton);

            this.layoutSlot = new RadialLayoutSlot();
        }

        /// <summary>
        /// Gets or sets the factor defining the thickness of the <see cref="DecorationItemButton"/> as a fraction of the size of the <see cref="NavigationItemButton"/>. 
        /// </summary>
        /// <value>
        /// The value should be between 0 and 1. If the passed value lies outside this range, it is automatically set to the nearest boundary value.
        /// </value>
        /// <example>
        /// <para>This example demonstrates how to style the <see cref="DecorationItemButton"/> of a <see cref="RadRadialMenu"/> using an implicit style.</para>
        /// <para>You will need to add the following namespace: <c>xmlns:telerikPrimitivesMenu="using:Telerik.UI.Xaml.Controls.Primitives.Menu"</c></para>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu.Resources&gt;
        ///     &lt;Style TargetType="telerikPrimitivesMenu:DecorationItemButton"&gt;
        ///         &lt;Setter Property="ThicknessFactor" Value="0.3"/&gt;
        ///     &lt;/Style&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu.Resources&gt;
        /// </code>
        /// </example>
        public double ThicknessFactor
        {
            get
            {
                return this.thicknessFactorCache;
            }
            set
            {
                this.SetValue(ThicknessFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the factor defining the thickness of the arrow part of the <see cref="DecorationItemButton"/> as a fraction of the size of the <see cref="NavigationItemButton"/>. 
        /// </summary>
        /// <value>
        /// The value should be between 0 and 1. If the passed value lies outside this range, it is automatically set to the nearest boundary value.
        /// </value>
        /// /// <example>
        /// <para>This example demonstrates how to style the <see cref="DecorationItemButton"/> of a <see cref="RadRadialMenu"/> using an implicit style.</para>
        /// <para>You will need to add the following namespace: <c>xmlns:telerikPrimitivesMenu="using:Telerik.UI.Xaml.Controls.Primitives.Menu"</c></para>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu.Resources&gt;
        ///     &lt;Style TargetType="telerikPrimitivesMenu:DecorationItemButton"&gt;
        ///         &lt;Setter Property="ArrowThicknessFactor" Value="0.3"/&gt;
        ///     &lt;/Style&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu.Resources&gt;
        /// </code>
        /// </example>
        public double ArrowThicknessFactor
        {
            get
            {
                return this.arrowThicknessFactorCache;
            }
            set
            {
                this.SetValue(ArrowThicknessFactorProperty, value);
            }
        }

        internal bool IsPointerOver
        {
            get
            {
                return this.isPointerOver;
            }
            set
            {
                this.isPointerOver = value;
                this.UpdateVisualState(true);
            }
        }

        internal RadialLayoutSlot LayoutSlot
        {
            get
            {
                return this.layoutSlot;
            }
            set
            {
                this.layoutSlot = value;
            }
        }

        internal RadialSegment Segment
        {
            get
            {
                return this.segment;
            }
            set
            {
                this.segment = value;
            }
        }

        internal static RadPoint GetArcPoint(double angle, RadPoint center, double radius)
        {
            double angleInRad = angle * RadMath.DegToRadFactor;

            double x = center.X + (Math.Cos(angleInRad) * radius);
            double y = center.Y - (Math.Sin(angleInRad) * radius);

            return new RadPoint(x, y);
        }

        internal void Update()
        {
            if (this.IsTemplateApplied)
            {
                this.UpdateGeometry();
                this.UpdateVisualState(true);
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            this.selectedPath = this.GetTemplatePartField<Path>("PART_SelectedPathElement");
            this.pointerOverPath = this.GetTemplatePartField<Path>("PART_PointerOverPathElement");
            this.arrowGlyph = this.GetTemplatePartField<Path>("PART_ArrowElement");

            return base.ApplyTemplateCore() && this.selectedPath != null && this.arrowGlyph != null && this.pointerOverPath != null;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            if (this.Segment == null)
            {
                return;
            }

            this.UpdateGeometry();

            base.OnTemplateApplied();
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.IsPointerOver && this.Segment.TargetItem.IsSelected)
            {
                return "SelectedPointerOver";
            }

            if (this.IsPointerOver)
            {
                return "PointerOver";
            }

            if (this.Segment.TargetItem.IsSelected)
            {
                return string.Format(CultureInfo.InvariantCulture, "Selected{0}{1}", RadControl.VisualStateDelimiter, base.ComposeVisualStateName());
            }

            return string.Format(CultureInfo.InvariantCulture, "Unselected{0}{1}", RadControl.VisualStateDelimiter, base.ComposeVisualStateName());
        }

        private static void OnThicknessFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecorationItemButton stateButton = (DecorationItemButton)d;

            double factor = (double)e.NewValue;
            double clampedFactor = factor;

            if (clampedFactor < 0)
            {
                clampedFactor = 0;
            }
            else if (clampedFactor > 1)
            {
                clampedFactor = 1;
            }

            stateButton.thicknessFactorCache = clampedFactor;
            stateButton.UpdateGeometry();
        }

        private static void OnArrowThicknessFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecorationItemButton stateButton = (DecorationItemButton)d;

            double factor = (double)e.NewValue;
            double clampedFactor = factor;

            if (clampedFactor < 0)
            {
                clampedFactor = 0;
            }
            else if (clampedFactor > 1)
            {
                clampedFactor = 1;
            }

            stateButton.arrowThicknessFactorCache = clampedFactor;
            stateButton.UpdateGeometry();
        }

        private PathGeometry RenderArrow()
        {
            if (this.LayoutSlot == null)
            {
                return null;
            }

            this.LayoutSlot.StartAngle = this.Segment.LayoutSlot.StartAngle;
            this.LayoutSlot.SweepAngle = this.Segment.LayoutSlot.SweepAngle;
            this.LayoutSlot.InnerRadius = this.Segment.LayoutSlot.OuterRadius;

            var centerPoint = new RadPoint(this.layoutSlot.InnerRadius, this.layoutSlot.InnerRadius);

            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            double arrowSize = this.layoutSlot.SweepAngle * (this.arrowThicknessFactorCache / 3.0);
            double outerRadius = this.layoutSlot.InnerRadius + 1;
            double startAngle = this.layoutSlot.StartAngle + (this.layoutSlot.SweepAngle / 2) - arrowSize;
            double endAngle = (this.layoutSlot.StartAngle + this.layoutSlot.SweepAngle) - (this.layoutSlot.SweepAngle / 2) + arrowSize;

            RadPoint startPoint = GetArcPoint(startAngle, centerPoint, outerRadius);
            figure.StartPoint = new Point(startPoint.X, startPoint.Y);

            ArcSegment firstArc = new ArcSegment();
            firstArc.Size = new Size(outerRadius, outerRadius);
            firstArc.IsLargeArc = this.layoutSlot.SweepAngle > 180;
            firstArc.SweepDirection = SweepDirection.Counterclockwise;
            var firstArcPoint = GetArcPoint(endAngle, centerPoint, outerRadius);
            firstArc.Point = new Point(firstArcPoint.X, firstArcPoint.Y);
            figure.Segments.Add(firstArc);

            LineSegment firstLine = new LineSegment();
            var firstLinePoint = GetArcPoint((startAngle + endAngle) / 2, centerPoint, this.layoutSlot.InnerRadius - arrowSize * 2);
            firstLine.Point = new Point(firstLinePoint.X, firstLinePoint.Y);
            figure.Segments.Add(firstLine);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        private PathGeometry RenderArc(bool selected)
        {
            if (this.LayoutSlot == null)
            {
                return null;
            }

            this.LayoutSlot.StartAngle = this.Segment.LayoutSlot.StartAngle;
            this.LayoutSlot.SweepAngle = this.Segment.LayoutSlot.SweepAngle;
            this.LayoutSlot.InnerRadius = this.Segment.LayoutSlot.OuterRadius;

            var centerPoint = new RadPoint(this.layoutSlot.InnerRadius, this.layoutSlot.InnerRadius);

            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            // Change the geometry if selected 
            double outerRadius = this.layoutSlot.InnerRadius + (this.layoutSlot.OuterRadius - this.layoutSlot.InnerRadius) * this.thicknessFactorCache;

            if (selected)
            {
                outerRadius = this.layoutSlot.InnerRadius + (this.layoutSlot.OuterRadius - this.layoutSlot.InnerRadius) * (this.thicknessFactorCache / 2.0);
            }
            RadPoint startPoint = GetArcPoint(this.layoutSlot.StartAngle, centerPoint, outerRadius);
            figure.StartPoint = new Point(startPoint.X, startPoint.Y);

            ArcSegment firstArc = new ArcSegment();
            firstArc.Size = new Size(outerRadius, outerRadius);
            firstArc.IsLargeArc = this.layoutSlot.SweepAngle > 180;
            firstArc.SweepDirection = SweepDirection.Counterclockwise;
            var firstArcPoint = GetArcPoint(this.layoutSlot.StartAngle + this.layoutSlot.SweepAngle, centerPoint, outerRadius);
            firstArc.Point = new Point(firstArcPoint.X, firstArcPoint.Y);
            figure.Segments.Add(firstArc);

            LineSegment firstLine = new LineSegment();
            var firstLinePoint = GetArcPoint(this.layoutSlot.StartAngle + this.layoutSlot.SweepAngle, centerPoint, this.layoutSlot.InnerRadius);
            firstLine.Point = new Point(firstLinePoint.X, firstLinePoint.Y);
            figure.Segments.Add(firstLine);

            ArcSegment secondArc = new ArcSegment();
            secondArc.Size = new Size(this.layoutSlot.InnerRadius, this.layoutSlot.InnerRadius);
            secondArc.IsLargeArc = this.layoutSlot.SweepAngle > 180;
            secondArc.SweepDirection = SweepDirection.Clockwise;
            var secondArcPoint = GetArcPoint(this.layoutSlot.StartAngle, centerPoint, this.layoutSlot.InnerRadius);
            secondArc.Point = new Point(secondArcPoint.X, secondArcPoint.Y);
            figure.Segments.Add(secondArc);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        private void UpdateGeometry()
        {
            this.selectedPath.Data = this.RenderArc(true);
            this.pointerOverPath.Data = this.RenderArc(false);
            this.arrowGlyph.Data = this.RenderArrow();
        }
    }
}
