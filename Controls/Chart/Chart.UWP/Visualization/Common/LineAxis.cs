using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an axis, which is visualized by a straight line.
    /// </summary>
    public abstract partial class LineAxis : Axis
    {
        /// <summary>
        /// Identifies the <see cref="IsInverse"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInverseProperty =
            DependencyProperty.Register(nameof(IsInverse), typeof(bool), typeof(LineAxis), new PropertyMetadata(false, OnIsInverseChanged));

        internal Line line;
        internal ContainerVisual lineContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineAxis"/> class.
        /// </summary>
        protected LineAxis()
        {
            this.DefaultStyleKey = typeof(LineAxis);

            this.line = new Line();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis direction is inverted.
        /// </summary>
        public bool IsInverse
        {
            get
            {
                return this.model.IsInverse;
            }
            set
            {
                this.SetValue(IsInverseProperty, value);
            }
        }

        /// <summary>
        /// Gets the visual that represents the stroke of the axis.
        /// </summary>
        internal override Shape StrokeVisual
        {
            get
            {
                return this.line;
            }
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.UpdateAxisLine(context);
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            base.OnPlotOriginChanged(context);

            this.UpdateAxisLine(context);
        }

        internal override void TransformTitle(FrameworkElement title)
        {
            if (this.type == AxisType.First)
            {
                title.ClearValue(UIElement.RenderTransformProperty);
            }
            else
            {
                RadSize desiredSize = this.model.title.desiredSize;

                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new RotateTransform() { Angle = -90 });
                transform.Children.Add(new TranslateTransform() { Y = (desiredSize.Width / 2) + (desiredSize.Height / 2) });

                title.RenderTransform = transform;
            }
        }

        internal virtual void UpdateAxisLine(ChartLayoutContext context)
        {
            if (!this.drawWithComposition)
            {
                // update stroke thickness
                this.line.StrokeThickness = this.model.LineThickness;
            }
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null && !this.drawWithComposition)
            {
                this.renderSurface.Children.Remove(this.line);
            }
            else if (this.drawWithComposition)
            {
                this.ContainerVisualRoot.Children.Remove(this.lineContainer);
            }
        }

        /// <summary>
        /// Adds the line shape to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                if (this.drawWithComposition)
                {
                    this.lineContainer = this.chart.ContainerVisualsFactory.CreateContainerVisual(this.Compositor, this.GetType());
                    this.ContainerVisualRoot.Children.InsertAtBottom(this.lineContainer);
                }
                else
                {
                    this.renderSurface.Children.Add(this.line);
                }
                
            }

            return applied;
        }

        private static void OnIsInverseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = d as Axis;
            presenter.model.IsInverse = (bool)e.NewValue;
        }
    }
}