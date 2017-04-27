using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using Telerik.Core;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a concrete <see cref="ChoroplethColorizer" /> implementation that enables color distribution to be specified through a collection of <see cref="D2DBrush" /> objects.
    /// The count of the ranges produced is implicitly defined by the number of brushes present within the <see cref="Brushes" /> collection.
    /// </summary>
    public class PaletteColorizer : ChoroplethColorizer
    {
        /// <summary>
        /// Identifies the <see cref="RangeStops"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeStopsProperty =
            DependencyProperty.Register(nameof(RangeStops), typeof(string), typeof(PaletteColorizer), new PropertyMetadata(null, OnRangeStopsPropertyChanged));

        internal List<RangeStop> rangeStops;
        private readonly string[] separators = { ",", ";", ":", " ", "|" };
        private ObservableCollection<D2DBrush> brushes;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteColorizer"/> class.
        /// </summary>
        public PaletteColorizer()
        {
            this.brushes = new ObservableCollection<D2DBrush>();
            this.brushes.CollectionChanged += this.OnBrushesCollectionChanged;

            this.rangeStops = new List<RangeStop>();
        }

        /// <summary>
        ///  Gets the collection of <see cref="D2DBrush"/> objects that defines the appearance of each range. The number of ranges is implicitly defined by the count of this collection.
        /// </summary>
        public ObservableCollection<D2DBrush> Brushes
        {
            get
            {
                return this.brushes;
            }
        }

        /// <summary>
        /// Gets or sets a comma separated list of the numeric range stop values for the colorizer to use when generating its associated <see cref="ColorRange"/> instances.
        /// </summary>
        /// <remarks>
        /// This property is optional. If set, it has higher priority than <see cref="Brushes"/> count and the range stops control the number of <see cref="ColorRange"/> instances; 
        /// if not set, the colorizer will generate the <see cref="ColorRange"/> instances automatically based on the provided <see cref="Brushes"/> count.
        /// </remarks>
        /// <value>
        /// For example if this property is set to "1, 2.5, 3, 5", the colorizer will generate 3 ranges -- [1, 2.5], [2.5, 3], [3, 5].
        /// </value>
        public string RangeStops
        {
            get
            {
                return (string)this.GetValue(RangeStopsProperty);
            }
            set
            {
                this.SetValue(RangeStopsProperty, value);
            }
        }

        /// <summary>
        /// Provides the core logic behind range generation. Allows inheritors to provide custom range generation routine.
        /// </summary>
        protected override IEnumerable<ColorRange> BuildRanges(IEnumerable<IMapShape> shapes)
        {
            if (this.rangeStops.Count == 0)
            {
                var baseRanges = base.BuildRanges(shapes);
                foreach (var baseRange in baseRanges)
                {
                    yield return baseRange;
                }

                yield break;
            }

            int rangeCount = this.GetRangeCount();
            for (int i = 0; i < rangeCount; i++)
            {
                var range = new ColorRange()
                {
                    Min = this.rangeStops[i].From,
                    Max = this.rangeStops[i].To,
                    Index = i
                };

                this.SetFillForRange(range);

                yield return range;
            }
        }

        /// <summary>
        /// Gets the count of the ranges that will be generated.
        /// </summary>
        protected override int GetRangeCount()
        {
            int rangeCount = this.rangeStops.Count;
            if (rangeCount == 0)
            {
                rangeCount = this.brushes.Count;
            }

            return rangeCount;
        }

        /// <summary>
        /// Sets the <see cref="D2DBrush" /> instance that defines the fill for each shape falling within the range.
        /// </summary>
        protected override void SetFillForRange(ColorRange range)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            if (range.Index < 0 || this.brushes.Count == 0)
            {
                return;
            }

            range.Fill = this.brushes[range.Index % this.brushes.Count];
        }

        /// <summary>
        /// Gets the <see cref="ColorRange" /> instance where the specified value falls.
        /// </summary>
        protected override ColorRange GetRangeForValue(double value)
        {
            if (this.rangeStops.Count == 0)
            {
                return base.GetRangeForValue(value);
            }

            if (value == double.NegativeInfinity)
            {
                return null;
            }

            // TODO: Possible optimization is to use binary search here.
            foreach (var range in this.Ranges)
            {
                // NOTE: Ranges are sorted so we can safely exit the iteration here.
                if (value < range.Min)
                {
                    break;
                }

                if (value <= range.Max)
                {
                    range.ShapeCount++;

                    return range;
                }
            }

            return null;
        }

        private static void OnRangeStopsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var colorizer = (PaletteColorizer)sender;
            colorizer.rangeStops.Clear();

            if (string.IsNullOrEmpty(colorizer.RangeStops))
            {
                return;
            }

            var stringValues = colorizer.RangeStops.Split(colorizer.separators, StringSplitOptions.RemoveEmptyEntries);

            List<double> doubleValues = new List<double>();
            foreach (var stringStop in stringValues)
            {
                double doubleValue;
                if (double.TryParse(stringStop, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
                {
                    doubleValues.Add(doubleValue);
                }
            }

            if (doubleValues.Count <= 1)
            {
                return;
            }

            doubleValues.Sort();

            for (int i = 0; i < doubleValues.Count - 1; i++)
            {
                colorizer.rangeStops.Add(new RangeStop() { From = doubleValues[i], To = doubleValues[i + 1] });
            }
        }

        private void OnBrushesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnChanged();
        }

        internal struct RangeStop
        {
            public double From;

            public double To;
        }
    }
}
