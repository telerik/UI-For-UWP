using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart.Primitives
{
    /// <summary>
    /// Represents the Line part of the <see cref="ChartTrackBallBehavior"/>.
    /// </summary>
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Canvas))]
    public class TrackBallLineControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="LineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(nameof(LineStyle), typeof(Style), typeof(TrackBallLineControl), new PropertyMetadata(null, OnLineStylePropertyChanged));

        private Polyline line;
        private Canvas layoutRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBallLineControl"/> class.
        /// </summary>
        public TrackBallLineControl()
        {
            this.DefaultStyleKey = typeof(TrackBallLineControl);
            this.line = new Polyline();
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that defines the appearance of the line displayed by a <see cref="ChartTrackBallBehavior"/> instance.
        /// The style should target the <see cref="Windows.UI.Xaml.Shapes.Polyline"/> type.
        /// </summary>
        public Style LineStyle
        {
            get
            {
                return this.GetValue(LineStyleProperty) as Style;
            }
            set
            {
                this.SetValue(LineStyleProperty, value);
            }
        }

        internal Polyline Line
        {
            get
            {
                return this.line;
            }
        }

        /// <summary>
        /// Retrieves the LayoutRoot part.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.layoutRoot = this.GetTemplatePartField<Canvas>("PART_LayoutRoot");
            applied = applied && this.layoutRoot != null;

            return applied;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.layoutRoot.Children.Remove(this.line);
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.layoutRoot.Children.Add(this.line);
        }

        private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TrackBallLineControl control = d as TrackBallLineControl;
            control.line.Style = e.NewValue as Style;
        }
    }
}
