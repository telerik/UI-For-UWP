using System;
using Telerik.Charting;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows you to place a straight line with specific coordinates on your <see cref="RadCartesianChart"/>. 
    /// </summary>
    public class CartesianCustomLineAnnotation : CartesianFromToAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianCustomLineAnnotation" /> class.
        /// </summary>
        public CartesianCustomLineAnnotation()
        {
            this.DefaultStyleKey = typeof(CartesianCustomLineAnnotation);
            this.model = new CartesianCustomLineAnnotationModel();
            this.presenter = new Line();
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
            (this.presenter as Line).X1 = (this.model as CartesianCustomLineAnnotationModel).line.X1 + offset;
            (this.presenter as Line).Y1 = (this.model as CartesianCustomLineAnnotationModel).line.Y1 + offset;
            (this.presenter as Line).X2 = (this.model as CartesianCustomLineAnnotationModel).line.X2 + offset;
            (this.presenter as Line).Y2 = (this.model as CartesianCustomLineAnnotationModel).line.Y2 + offset;
        }
    }
}