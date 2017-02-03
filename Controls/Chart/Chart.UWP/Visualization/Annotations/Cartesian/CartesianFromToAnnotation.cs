using System;
using System.Linq;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an annotation which position is defined by a rectangle defined by <see cref="HorizontalFrom"/>, <see cref="HorizontalTo"/>, <see cref="VerticalFrom"/> and <see cref="VerticalTo"/> properties.
    /// </summary>
    public abstract class CartesianFromToAnnotation : CartesianStrokedAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register(nameof(HorizontalAxis), typeof(CartesianAxis), typeof(CartesianFromToAnnotation), new PropertyMetadata(null, OnHorizontalAxisPropertyChanged));
    
        /// <summary>
        /// Identifies the <see cref="VerticalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register(nameof(VerticalAxis), typeof(CartesianAxis), typeof(CartesianFromToAnnotation), new PropertyMetadata(null, OnVerticalAxisPropertyChanged));
      
        /// <summary>
        /// Identifies the <see cref="HorizontalFrom"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalFromProperty =
            DependencyProperty.Register(nameof(HorizontalFrom), typeof(object), typeof(CartesianFromToAnnotation), new PropertyMetadata(null, OnHorizontalFromPropertyChanged));
     
        /// <summary>
        /// Identifies the <see cref="HorizontalTo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalToProperty =
            DependencyProperty.Register(nameof(HorizontalTo), typeof(object), typeof(CartesianFromToAnnotation), new PropertyMetadata(null, OnHorizontalToPropertyChanged));
     
        /// <summary>
        /// Identifies the <see cref="VerticalFrom"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalFromProperty =
            DependencyProperty.Register(nameof(VerticalFrom), typeof(object), typeof(CartesianFromToAnnotation), new PropertyMetadata(null, OnVerticalFromPropertyChanged));
     
        /// <summary>
        /// Identifies the <see cref="VerticalTo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalToProperty =
            DependencyProperty.Register(nameof(VerticalTo), typeof(object), typeof(CartesianFromToAnnotation), new PropertyMetadata(null, OnVerticalToPropertyChanged));

        internal CartesianFromToAnnotationModel model;
        internal Shape presenter;

        /// <summary>
        /// Gets or sets the end vertical coordinate of the annotation.
        /// </summary>
        public object VerticalTo
        {
            get
            {
                return this.GetValue(VerticalToProperty);
            }
            set
            {
                this.SetValue(VerticalToProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start vertical coordinate of the annotation.
        /// </summary>
        public object VerticalFrom
        {
            get
            {
                return this.GetValue(VerticalFromProperty);
            }
            set
            {
                this.SetValue(VerticalFromProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the end horizontal coordinate of the annotation.
        /// </summary>
        public object HorizontalTo
        {
            get
            {
                return this.GetValue(HorizontalToProperty);
            }
            set
            {
                this.SetValue(HorizontalToProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start horizontal coordinate of the annotation.
        /// </summary>
        public object HorizontalFrom
        {
            get
            {
                return this.GetValue(HorizontalFromProperty);
            }
            set
            {
                this.SetValue(HorizontalFromProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical axis that the annotation is associated with.
        /// </summary>
        public CartesianAxis VerticalAxis
        {
            get
            {
                return (CartesianAxis)this.GetValue(VerticalAxisProperty);
            }
            set
            {
                this.SetValue(VerticalAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal axis that the annotation is associated with.
        /// </summary>
        public CartesianAxis HorizontalAxis
        {
            get
            {
                return (CartesianAxis)this.GetValue(HorizontalAxisProperty);
            }
            set
            {
                this.SetValue(HorizontalAxisProperty, value);
            }
        }

        internal override void SetPresenterVisibility(Visibility annotationVisibility)
        {
            if (this.presenter != null)
            {
                this.presenter.Visibility = annotationVisibility;
            }

            if (this.labelPresenter != null)
            {
                this.labelPresenter.Visibility = annotationVisibility;
            }
        }

        internal override Visibility GetPresenterVisibility()
        {
            if (this.presenter != null)
            {
                return this.presenter.Visibility;
            }
            else
            {
                return this.Visibility;
            }
        }

        internal override void UpdateVisibility()
        {
            if (this.model.horizontalFromPlotInfo != null && this.model.verticalFromPlotInfo != null &&
                this.model.horizontalToPlotInfo != null && this.model.verticalToPlotInfo != null)
            {
                base.UpdateVisibility();
            }
            else
            {
                this.SetPresenterVisibility(Visibility.Collapsed);
            }
        }

        internal override void ChartAxisChanged(CartesianAxis oldAxis, CartesianAxis newAxis)
        {
            base.ChartAxisChanged(oldAxis, newAxis);
            if (this.HorizontalAxis == null)
            {
                if (oldAxis != null && oldAxis.type == AxisType.First)
                {
                    this.model.DetachAxis(oldAxis.model);
                }
                if (newAxis != null && newAxis.type == AxisType.First)
                {
                    this.model.AttachAxis(newAxis.model, AxisType.First);
                }
            }
            if (this.VerticalAxis == null)
            {
                if (oldAxis != null && oldAxis.type == AxisType.Second)
                {
                    this.model.DetachAxis(oldAxis.model);
                }
                if (newAxis != null && newAxis.type == AxisType.Second)
                {
                    this.model.AttachAxis(newAxis.model, AxisType.Second);
                }
            }
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            RadCartesianChart cartesianChart = this.chart as RadCartesianChart;

            if (this.HorizontalAxis == null && cartesianChart.HorizontalAxis != null)
            {
                this.model.FirstAxis = cartesianChart.HorizontalAxis.model;
            }
            if (this.VerticalAxis == null && cartesianChart.VerticalAxis != null)
            {
                this.model.SecondAxis = cartesianChart.VerticalAxis.model;
            }
        }
  
        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null)
            {
                this.renderSurface.Children.Remove(this.presenter);
            }
        }

        /// <summary>
        /// Initializes the render surface template part.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied)
            {
                this.renderSurface.Children.Add(this.presenter);
            }

            return applied;
        }

        private static void OnVerticalToPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianFromToAnnotation annotation = sender as CartesianFromToAnnotation;
            annotation.model.VerticalTo = annotation.VerticalTo;
        }

        private static void OnVerticalFromPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianFromToAnnotation annotation = sender as CartesianFromToAnnotation;
            annotation.model.VerticalFrom = annotation.VerticalFrom;
        }

        private static void OnHorizontalToPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianFromToAnnotation annotation = sender as CartesianFromToAnnotation;
            annotation.model.HorizontalTo = annotation.HorizontalTo;
        }

        private static void OnHorizontalFromPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianFromToAnnotation annotation = sender as CartesianFromToAnnotation;
            annotation.model.HorizontalFrom = annotation.HorizontalFrom;
        }

        private static void OnVerticalAxisPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianFromToAnnotation annotation = sender as CartesianFromToAnnotation;
            if (annotation.VerticalAxis != null)
            {
                annotation.model.SecondAxis = annotation.VerticalAxis.model;
            }
            else
            {
                annotation.model.SecondAxis = null;
            }
        }

        private static void OnHorizontalAxisPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianFromToAnnotation annotation = sender as CartesianFromToAnnotation;
            if (annotation.HorizontalAxis != null)
            {
                annotation.model.FirstAxis = annotation.HorizontalAxis.model;
            }
            else
            {
                annotation.model.FirstAxis = null;
            }
        }
    }
}