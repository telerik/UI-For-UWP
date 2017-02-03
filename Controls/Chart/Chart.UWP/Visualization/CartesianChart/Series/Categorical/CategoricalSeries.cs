using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a chart categorical series that supports combining.
    /// </summary>
    public abstract class CategoricalSeries : CategoricalSeriesBase
    {     
        /// <summary>
        /// Identifies the <see cref="CombineMode"/> property.
        /// </summary>
        public static readonly DependencyProperty CombineModeProperty =
            DependencyProperty.Register(nameof(CombineMode), typeof(ChartSeriesCombineMode), typeof(CategoricalSeries), new PropertyMetadata(ChartSeriesCombineMode.None, OnCombineModePropertyChanged));

        private ChartSeriesCombineMode combineModeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoricalSeries"/> class.
        /// </summary>
        protected CategoricalSeries()
        {
        }

        /// <summary>
        /// Gets or sets the combination mode to be used when data points are plotted.
        /// </summary>
        public ChartSeriesCombineMode CombineMode
        {
            get
            {
                return this.combineModeCache;
            }
            set
            {
                this.SetValue(CombineModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the key that defines in which stack group this series will be included if its <see cref="CombineMode"/> equals Stack or Stack100.
        /// </summary>
        public object StackGroupKey
        {
            get
            {
                return (this.Model as CategoricalSeriesModel).StackGroupKey;
            }
            set
            {
                (this.Model as CategoricalSeriesModel).StackGroupKey = value;
            }
        } 

        private static void OnCombineModePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var series = target as CategoricalSeries;
            series.combineModeCache = (ChartSeriesCombineMode)args.NewValue;
            (series.Model as CategoricalSeriesModel).CombineMode = series.CombineMode;
        }
    }
}
