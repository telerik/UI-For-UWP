using System;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// A base class for all data points that have a property defining the category of the data point.
    /// </summary>
    public abstract class CategoricalDataPointBase : DataPoint
    {
        internal static readonly int CategoryPropertyKey = PropertyKeys.Register(typeof(CategoricalDataPointBase), "Category", ChartAreaInvalidateFlags.All);

        internal NumericalAxisPlotInfo numericalPlot;
        internal CategoricalAxisPlotInfo categoricalPlot;

        /// <summary>
        /// Defines whether the data point is within its numerical axis range.
        /// </summary>
        protected bool isInNumericalRange;
        
        /// <summary>
        /// Defines whether the data point is within its categorical axis range.
        /// </summary>
        protected bool isInCategoricalRange;

        /// <summary>
        /// Gets or sets the object instance that describes the category of the point.
        /// </summary>
        public object Category
        {
            get
            {
                return this.GetValue(CategoryPropertyKey);
            }
            set
            {
                this.SetValue(CategoryPropertyKey, value);
            }
        }
    }
}