using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an annotation which shape may be stroked (outlined).
    /// </summary>
    public abstract class CartesianStrokedAnnotation : CartesianChartAnnotation, IStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="Stroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(CartesianStrokedAnnotation), new PropertyMetadata(null, OnStrokePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(CartesianStrokedAnnotation), new PropertyMetadata(0d, OnStrokeThicknessPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(CartesianStrokedAnnotation), new PropertyMetadata(null, OnStrokeDashArrayChanged));

        /// <summary>
        /// Identifies the <see cref="Label"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(CartesianStrokedAnnotation), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LabelDefinition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelDefinitionProperty =
            DependencyProperty.Register(nameof(LabelDefinition), typeof(ChartAnnotationLabelDefinition), typeof(CartesianStrokedAnnotation), new PropertyMetadata(null));

        internal FrameworkElement labelPresenter;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that specifies how the shape outline is painted.
        /// </summary>
        /// <value>The line stroke.</value>
        public Brush Stroke
        {
            get
            {
                return (Brush)this.GetValue(StrokeProperty);
            }
            set
            {
                this.SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the shape stroke outline.
        /// </summary>
        /// <value>The line stroke thickness.</value>
        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(StrokeThicknessProperty);
            }
            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="T:System.Double" /> values that indicates the pattern of dashes and gaps that is used to outline stroked annotation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)this.GetValue(StrokeDashArrayProperty);
            }
            set
            {
                this.SetValue(StrokeDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label
        {
            get
            {
                return (string)this.GetValue(LabelProperty);
            }
            set
            {
                this.SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the definition that describe the appearance of the label.
        /// </summary>
        /// <value>
        /// The label definition.
        /// </value>
        public ChartAnnotationLabelDefinition LabelDefinition
        {
            get
            {
                return (ChartAnnotationLabelDefinition)this.GetValue(LabelDefinitionProperty);
            }
            set
            {
                this.SetValue(LabelDefinitionProperty, value);
            }
        }

        /// <summary>
        /// Gets the presenter.
        /// </summary>
        /// <value>The presenter.</value>
        protected abstract Shape Presenter { get; }

        /// <summary>
        /// Gets a value indicating whether the stroke goes inwards by the full <see cref="StrokeThickness"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is stroke inset; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool IsStrokeInset
        {
            get
            {
                return false;
            }
        }

        internal virtual ChartAnnotationLabelDefinition CreateDefaultLabelDefinition()
        {
            // TODO: This should be updated to a more appropriate one!
            return new ChartAnnotationLabelDefinition()
            {
                Location = ChartAnnotationLabelLocation.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
        }

        internal override void UpdatePresenters()
        {
            ChartAnnotationLabelUpdateContext labelContext = new ChartAnnotationLabelUpdateContext(this.Model.originalLayoutSlot);
            this.ProcessLabel(labelContext);
        }

        private static void OnStrokePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianStrokedAnnotation annotation = sender as CartesianStrokedAnnotation;
            annotation.Presenter.Stroke = e.NewValue as Brush;
        }

        private static void OnStrokeThicknessPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianStrokedAnnotation annotation = sender as CartesianStrokedAnnotation;
            annotation.Presenter.StrokeThickness = (double)e.NewValue;
            var model = annotation.Model as IStrokedAnnotationModel;
            if (model != null)
            {
                model.StrokeThickness = annotation.Presenter.StrokeThickness;
            }
        }

        private static void OnStrokeDashArrayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianStrokedAnnotation annotation = sender as CartesianStrokedAnnotation;
            annotation.Presenter.StrokeDashArray = (e.NewValue as DoubleCollection).Clone();
            var model = annotation.Model as IStrokedAnnotationModel;
            if (model != null)
            {
                model.DashPatternLength = annotation.Presenter.StrokeDashArray.Sum();
                if (annotation.Presenter.StrokeDashArray.Count % 2 == 1)
                {
                    model.DashPatternLength *= 2;
                }
            }
        }

        private static double GetLabelSlotX(ChartAnnotationLabelUpdateContext context, RadSize labelSize, double strokeThickness, double strokeCompensation)
        {
            ChartAnnotationLabelLocation location = context.Definition.Location;
            RadRect annotationSlot = context.LayoutSlot;
            HorizontalAlignment horizontalAlignment = context.Definition.HorizontalAlignment;
            double horizontalOffset = context.Definition.HorizontalOffset;
            double x = context.Location.X;

            switch (location)
            {
                case ChartAnnotationLabelLocation.Left:
                    {
                        x = annotationSlot.X - strokeCompensation - labelSize.Width;
                        break;
                    }
                case ChartAnnotationLabelLocation.Right:
                    {
                        x = annotationSlot.Right + strokeCompensation;
                        break;
                    }
                case ChartAnnotationLabelLocation.Top:
                case ChartAnnotationLabelLocation.Bottom:
                    {
                        switch (horizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                x = annotationSlot.X;
                                break;
                            case HorizontalAlignment.Center:
                                x = annotationSlot.X + ((annotationSlot.Width - labelSize.Width) / 2);
                                break;
                            case HorizontalAlignment.Right:
                                x = annotationSlot.Right - labelSize.Width;
                                break;
                        }
                        break;
                    }
                case ChartAnnotationLabelLocation.Inside:
                    {
                        switch (horizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                x = annotationSlot.X + strokeThickness;
                                break;
                            case HorizontalAlignment.Center:
                                x = annotationSlot.X + ((annotationSlot.Width - labelSize.Width) / 2);
                                break;
                            case HorizontalAlignment.Right:
                                x = annotationSlot.Right - strokeThickness - labelSize.Width;
                                break;
                        }
                        break;
                    }
            }

            x += horizontalOffset;

            return x;
        }

        private static double GetLabelSlotY(ChartAnnotationLabelUpdateContext context, RadSize labelSize, double strokeThickness, double strokeCompensation)
        {
            ChartAnnotationLabelLocation location = context.Definition.Location;
            RadRect annotationSlot = context.LayoutSlot;
            VerticalAlignment verticalAlignment = context.Definition.VerticalAlignment;
            double verticalOffset = context.Definition.VerticalOffset;
            double y = context.Location.Y;

            switch (location)
            {
                case ChartAnnotationLabelLocation.Top:
                    {
                        y = annotationSlot.Y - strokeCompensation - labelSize.Height;
                        break;
                    }
                case ChartAnnotationLabelLocation.Bottom:
                    {
                        y = annotationSlot.Bottom + strokeCompensation;
                        break;
                    }
                case ChartAnnotationLabelLocation.Left:
                case ChartAnnotationLabelLocation.Right:
                    {
                        switch (verticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                y = annotationSlot.Y;
                                break;
                            case VerticalAlignment.Center:
                                y = annotationSlot.Y + ((annotationSlot.Height - labelSize.Height) / 2);
                                break;
                            case VerticalAlignment.Bottom:
                                y = annotationSlot.Bottom - labelSize.Height;
                                break;
                        }
                        break;
                    }
                case ChartAnnotationLabelLocation.Inside:
                    {
                        switch (verticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                y = annotationSlot.Y + strokeThickness;
                                break;
                            case VerticalAlignment.Center:
                                y = annotationSlot.Y + ((annotationSlot.Height - labelSize.Height) / 2);
                                break;
                            case VerticalAlignment.Bottom:
                                y = annotationSlot.Bottom - strokeThickness - labelSize.Height;
                                break;
                        }
                        break;
                    }
            }

            y += verticalOffset;

            return y;
        }

        private void ProcessLabel(ChartAnnotationLabelUpdateContext context)
        {
            context.Definition = this.LabelDefinition;

            if (context.Definition == null)
            {
                context.Definition = this.CreateDefaultLabelDefinition();
            }

            this.ProcessLabelDefinition(context);
        }

        private void ProcessLabelDefinition(ChartAnnotationLabelUpdateContext context)
        {
            FrameworkElement visual = this.GetLabelVisual(context);
            if (visual == null)
            {
                Debug.Assert(false, "No label visual created.");
                return;
            }

            this.ArrangeLabel(visual, context);
        }

        private void ArrangeLabel(FrameworkElement visual, ChartAnnotationLabelUpdateContext context)
        {
            RadSize size = MeasureVisual(visual);
            this.ArrangeUIElement(visual, this.GetLabelSlot(size, context));
        }

        private RadRect GetLabelSlot(RadSize labelSize, ChartAnnotationLabelUpdateContext context)
        {
            double strokeThickness = this.StrokeThickness;
            double strokeCompensation = 0;
            if (!this.IsStrokeInset)
            {
                strokeCompensation = strokeThickness / 2;
            }

            double x = GetLabelSlotX(context, labelSize, strokeThickness, strokeCompensation);
            double y = GetLabelSlotY(context, labelSize, strokeThickness, strokeCompensation);

            return new RadRect(x, y, labelSize.Width, labelSize.Height);
        }

        private FrameworkElement GetLabelVisual(ChartAnnotationLabelUpdateContext context)
        {
            if (this.labelPresenter == null)
            {
                this.labelPresenter = this.CreateLabelVisual(context);
            }

            ContentPresenter presenter = this.labelPresenter as ContentPresenter;
            if (presenter != null)
            {
                presenter.Content = this.GetLabelContent(context);
                presenter.ContentTemplate = context.Definition.Template;
            }
            else
            {
                TextBlock textBlock = this.labelPresenter as TextBlock;
                if (textBlock != null)
                {
                    object label = this.GetLabelContent(context);
                    textBlock.Text = label == null ? string.Empty : label.ToString();
                }
            }

            return this.labelPresenter;
        }

        private object GetLabelContent(ChartAnnotationLabelUpdateContext context)
        {
            object label = this.Label;

            if (label != null && !string.IsNullOrEmpty(context.Definition.Format))
            {
                return string.Format(CultureInfo.CurrentUICulture, context.Definition.Format, label);
            }

            return label;
        }

        private FrameworkElement CreateLabelVisual(ChartAnnotationLabelUpdateContext context)
        {
            FrameworkElement visual;
            DataTemplate template = context.Definition.Template;

            if (template != null)
            {
                visual = new ContentPresenter();
            }
            else
            {
                visual = new TextBlock() { Style = context.Definition.DefaultVisualStyle };
            }

            this.renderSurface.Children.Add(visual);

            return visual;
        }
    }
}