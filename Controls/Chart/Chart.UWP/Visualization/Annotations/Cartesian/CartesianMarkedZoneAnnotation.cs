using System;
using System.Linq;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an annotation which marks a rectangle area on the chart.
    /// </summary>
    public class CartesianMarkedZoneAnnotation : CartesianFromToAnnotation
    {
        /// <summary>
        /// Identifies the <see cref="Fill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(CartesianMarkedZoneAnnotation), new PropertyMetadata(null, OnFillPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianMarkedZoneAnnotation" /> class.
        /// </summary>
        public CartesianMarkedZoneAnnotation()
        {
            this.DefaultStyleKey = typeof(CartesianMarkedZoneAnnotation);
            this.model = new CartesianMarkedZoneAnnotationModel();
            this.presenter = new Rectangle();
        }

        /// <summary>
        /// Gets or sets the fill brush of the annotation.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return (Brush)this.GetValue(FillProperty);
            }
            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        internal override ChartAnnotationModel Model
        {
            get
            {
                return this.model;
            }
        }

        /// <summary>
        /// Gets the visual presenter of the annotation.
        /// </summary>
        protected override Shape Presenter
        {
            get
            {
                return this.presenter;
            }
        }

        internal override void UpdatePresenters()
        {
            base.UpdatePresenters();

            double offset = this.StrokeThickness % 2 == 0 ? 0 : 0.5;
            Canvas.SetLeft(this.presenter, this.model.layoutSlot.X);
            Canvas.SetTop(this.presenter, this.model.layoutSlot.Y);
            this.presenter.Width = this.model.layoutSlot.Width;
            this.presenter.Height = this.model.layoutSlot.Height;
        }

        private static void OnFillPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CartesianMarkedZoneAnnotation annotation = sender as CartesianMarkedZoneAnnotation;
            annotation.presenter.Fill = e.NewValue as Brush;
        }
    }
}