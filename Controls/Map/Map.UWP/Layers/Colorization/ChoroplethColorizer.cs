using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Core;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a <see cref="MapShapeColorizer"/> implementation that groups the associated <see cref="IMapShape"/> objects in ranges, depending on each shape's attribute value and colorizes each range with a different color.
    /// </summary>
    public abstract class ChoroplethColorizer : MapShapeColorizer, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifies the <see cref="AttributeName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AttributeNameProperty =
            DependencyProperty.Register(nameof(AttributeName), typeof(string), typeof(ChoroplethColorizer), new PropertyMetadata(null, new PropertyChangedCallback(OnAttributeNamePropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="RangeDistribution"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeDistributionProperty =
            DependencyProperty.Register(nameof(RangeDistribution), typeof(ChoroplethRangeDistribution), typeof(ChoroplethColorizer), new PropertyMetadata(null, OnRangeDistributionPropertyChanged));

        private string attributeNameCache;
        private ValueRange<double> actualRange;
        private List<double> doubleValues;
        private List<ColorRange> ranges;
        private Dictionary<int, ColorRange> colorRangesByColorShape;
        private ChoroplethRangeDistribution distribution;
        private PropertyChangedEventHandler propertyChangedEh;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoroplethColorizer"/> class.
        /// </summary>
        protected ChoroplethColorizer()
        {
            this.doubleValues = new List<double>();
            this.ranges = new List<ColorRange>();
            this.colorRangesByColorShape = new Dictionary<int, ColorRange>();

            this.distribution = new LinearRangeDistribution();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.propertyChangedEh += value;
            }
            remove
            {
                this.propertyChangedEh -= value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the attribute that identifies the <see cref="System.Double"/> value for each shape. This value is used in the range distribution logic.
        /// </summary>
        public string AttributeName
        {
            get
            {
                return this.attributeNameCache;
            }
            set
            {
                this.SetValue(AttributeNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChoroplethRangeDistribution"/> instance that may be used to override the default linear distribution of shape values.
        /// </summary>
        public ChoroplethRangeDistribution RangeDistribution
        {
            get
            {
                return this.distribution;
            }
            set
            {
                this.SetValue(RangeDistributionProperty, value);
            }
        }

        /// <summary>
        /// Gets the current range, computed by the colorizer.
        /// </summary>
        public ValueRange<double> ActualRange
        {
            get
            {
                return this.actualRange;
            }
        }

        /// <summary>
        /// Gets all the ranges computed by the colorizer.
        /// </summary>
        public IEnumerable<ColorRange> Ranges
        {
            get
            {
                foreach (var range in this.ranges)
                {
                    yield return range;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ColorRange"/> instance where the specified shape falls.
        /// </summary>
        public ColorRange GetRangeForShape(IMapShape shape)
        {
            if (shape == null)
            {
                throw new ArgumentNullException();
            }

            ColorRange range;
            if (this.colorRangesByColorShape.TryGetValue((shape as MapShapeModel).UniqueId, out range))
            {
                return range;
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="D2DShapeStyle" /> instance that defines the appearance of the specified <see cref="IMapShape" /> instance.
        /// </summary>
        /// <param name="shape">The <see cref="IMapShape" /> instance for which the style is to be retrieved.</param>
        protected internal override D2DShapeStyle GetShapeStyle(IMapShape shape)
        {
            var range = this.GetRangeForShape(shape);
            if (range != null)
            {
                return range.Style;
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="ColorRange"/> instance at the specified index.
        /// </summary>
        /// <param name="rangeIndex">The zero-based index of the range.</param>
        protected ColorRange GetRangeAt(int rangeIndex)
        {
            if (rangeIndex < 0 || rangeIndex >= this.ranges.Count)
            {
                return null;
            }

            return this.ranges[rangeIndex];
        }

        /// <summary>
        /// Gets the count of the ranges that will be generated.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected abstract int GetRangeCount();

        /// <summary>
        /// Provides the core logic behind range generation. Allows inheritors to provide custom range generation routine.
        /// </summary>
        protected virtual IEnumerable<ColorRange> BuildRanges(IEnumerable<IMapShape> shapes)
        {
            int rangeCount = this.GetRangeCount();

            if (this.distribution != null)
            {
                foreach (var range in this.distribution.BuildRanges(this.actualRange, rangeCount))
                {
                    this.SetFillForRange(range);

                    yield return range;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ColorRange"/> instance where the specified value falls.
        /// </summary>
        protected virtual ColorRange GetRangeForValue(double value)
        {
            if (value == double.NegativeInfinity || this.distribution == null)
            {
                return null;
            }

            int rangeIndex = this.distribution.GetRangeIndexForValue(value);
            if (rangeIndex < 0)
            {
                return null;
            }

            var range = this.ranges[rangeIndex];
            range.ShapeCount++;

            return range;
        }

        /// <summary>
        /// Implements the core logic behind the Reset routine.
        /// </summary>
        protected override void ResetOverride()
        {
            this.ranges.Clear();
            this.doubleValues.Clear();
            this.colorRangesByColorShape.Clear();
            this.actualRange = new ValueRange<double>(double.NegativeInfinity, double.PositiveInfinity);
        }

        /// <summary>
        /// Implements the core logic behind the Initialize routine.
        /// </summary>
        /// <param name="shapes">The set of shapes to initialize from.</param>
        protected override bool InitializeOverride(IEnumerable<IMapShape> shapes)
        {
            if (string.IsNullOrEmpty(this.attributeNameCache))
            {
                return false;
            }

            int rangeCount = this.GetRangeCount();
            if (rangeCount == 0)
            {
                // TODO: Fail?
                return false;
            }

            bool isAttributeValid;
            this.UpdateActualRange(shapes, out isAttributeValid);

            if (!isAttributeValid)
            {
                return false;
            }

            foreach (var range in this.BuildRanges(shapes))
            {
                this.ranges.Add(range);
            }

            if (this.ranges.Count == 0)
            {
                return false;
            }

            this.BuildShapesByRange(shapes);

            return true;
        }

        /// <summary>
        /// Sets the <see cref="D2DBrush"/> instance that defines the fill for each shape falling within the range.
        /// </summary>
        protected virtual void SetFillForRange(ColorRange range)
        {
        }

        /// <summary>
        /// Provides an extension point for inheritors to perform additional logic when a <see cref="IMapShape"/> is associated with a valid <see cref="ColorRange"/>.
        /// </summary>
        protected virtual void OnShapeAssociated(IMapShape shape, ColorRange range)
        {
        }

        /// <summary>
        /// Provides an entry point that allows inheritors to perform some additional logic upon initialization completion.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            var eh = this.propertyChangedEh;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs("Ranges"));
            }
        }

        private static void OnAttributeNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorizer = d as ChoroplethColorizer;
            colorizer.attributeNameCache = (string)e.NewValue;
            colorizer.OnChanged();
        }

        private static void OnRangeDistributionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorizer = d as ChoroplethColorizer;
            colorizer.distribution = e.NewValue as ChoroplethRangeDistribution;
            colorizer.OnChanged();
        }

        private void UpdateActualRange(IEnumerable<IMapShape> shapes, out bool isAttributeValid)
        {
            this.actualRange = new ValueRange<double>();
            this.actualRange.minimum = double.PositiveInfinity;

            isAttributeValid = false;

            object value;
            double doubleValue;

            foreach (var shape in shapes)
            {
                value = shape.GetAttribute(this.attributeNameCache);
                doubleValue = double.NegativeInfinity;

                if (value != null)
                {
                    isAttributeValid = true;
                    if (!NumericConverter.TryConvertToDouble(value, out doubleValue))
                    {
                        throw new ArgumentException("Expected numeric value for attribute " + this.attributeNameCache);
                    }

                    if (doubleValue > this.actualRange.maximum)
                    {
                        this.actualRange.maximum = doubleValue;
                    }
                    if (doubleValue < this.actualRange.minimum)
                    {
                        this.actualRange.minimum = doubleValue;
                    }
                }

                // TODO: We support null values to keep the doubleValues list indexes synchronized
                this.doubleValues.Add(doubleValue);
            }
        }

        private void BuildShapesByRange(IEnumerable<IMapShape> shapes)
        {
            int shapeIndex = 0;
            ColorRange range;

            foreach (var shape in shapes)
            {
                range = this.GetRangeForValue(this.doubleValues[shapeIndex]);
                shapeIndex++;

                if (range != null)
                {
                    this.colorRangesByColorShape.Add((shape as MapShapeModel).UniqueId, range);
                    this.OnShapeAssociated(shape, range);
                }
            }
        }
    }
}
