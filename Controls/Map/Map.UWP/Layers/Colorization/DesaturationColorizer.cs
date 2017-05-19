using System;
using System.Diagnostics.CodeAnalysis;
using Telerik.Core;
using Telerik.UI.Drawing;
using Windows.UI;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a concrete <see cref="ChoroplethColorizer"/> implementation that creates a desaturated color for each generated <see cref="ColorRange"/> given a base color value.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    public class DesaturationColorizer : ChoroplethColorizer
    {
        /// <summary>
        /// Identifies the <see cref="RangeCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeCountProperty = 
            DependencyProperty.Register(nameof(RangeCount), typeof(int), typeof(DesaturationColorizer), new PropertyMetadata(7, new PropertyChangedCallback(OnRangeCountPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="BaseColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseColorProperty =
            DependencyProperty.Register(nameof(BaseColor), typeof(Color), typeof(DesaturationColorizer), new PropertyMetadata(Colors.DarkOrange, new PropertyChangedCallback(OnBaseColorPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="From"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(double), typeof(DesaturationColorizer), new PropertyMetadata(1d, new PropertyChangedCallback(OnFromPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="To"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(double), typeof(DesaturationColorizer), new PropertyMetadata(0d, new PropertyChangedCallback(OnToPropertyChanged)));

        private int rangeCountCache = 7;
        private Color baseColorCache = Colors.DarkOrange;
        private HslColor hslColor;
        private double from = 1d;
        private double to = 0d;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesaturationColorizer"/> class.
        /// </summary>
        public DesaturationColorizer()
        {
            this.hslColor = HslColor.Parse(this.baseColorCache);
        }

        /// <summary>
        /// Gets or sets a value between 0 and 1 that specifies the maximum percentage of desaturation that may be applied to the specified <see cref="BaseColor"/> value. A zero means completely white.
        /// </summary>
        public double From
        {
            get
            {
                return this.from;
            }
            set
            {
                this.SetValue(FromProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value between 0 and 1 that specifies the starting desaturation percentage of the <see cref="BaseColor"/> value. 1 means that the first color will be the BaseColor itself.
        /// </summary>
        public double To
        {
            get
            {
                return this.to;
            }
            set
            {
                this.SetValue(ToProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of ranges to be generated, based on the computed <see cref="P:ActualRange"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public int RangeCount
        {
            get
            {
                return this.rangeCountCache;
            }
            set
            {
                this.SetValue(RangeCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Color value from which the rest desaturated values will be generated. This value will represent the range, containing the shapes with maximum values, as defined by the <see cref="P:AttributeName"/> property.
        /// Defaults to 7.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        public Color BaseColor
        {
            get
            {
                return this.baseColorCache;
            }
            set
            {
                this.SetValue(BaseColorProperty, value);
            }
        }

        /// <summary>
        /// Gets the count of the ranges that will be generated.
        /// </summary>
        protected override int GetRangeCount()
        {
            return this.rangeCountCache;
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

            double ratio = 1d / (this.rangeCountCache - 1) * range.Index;
            double colorRatio = this.to + (ratio * (this.from - this.to));

            var hsl = this.hslColor;
            hsl.S *= colorRatio;
            hsl.L = 1 - ((1 - hsl.L) * colorRatio);

            range.Fill = new D2DSolidColorBrush() { Color = hsl.ToColor() };
        }

        private static void OnRangeCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorizer = d as DesaturationColorizer;
            colorizer.rangeCountCache = (int)e.NewValue;
            if (colorizer.rangeCountCache < 0)
            {
                throw new ArgumentException("RangeCount may not be negative.");
            }
            colorizer.OnChanged();
        }

        private static void OnBaseColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorizer = d as DesaturationColorizer;
            colorizer.baseColorCache = (Color)e.NewValue;
            colorizer.hslColor = HslColor.Parse(colorizer.baseColorCache);
            colorizer.OnChanged();
        }

        private static void OnFromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorizer = d as DesaturationColorizer;
            if (colorizer.IsInternalPropertyChange)
            {
                return;
            }

            colorizer.from = (double)e.NewValue;
            if (colorizer.from > 1)
            {
                colorizer.ChangePropertyInternally(FromProperty, 1d);
            }
            colorizer.OnChanged();
        }

        private static void OnToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorizer = d as DesaturationColorizer;
            if (colorizer.IsInternalPropertyChange)
            {
                return;
            }

            colorizer.to = (double)e.NewValue;
            if (colorizer.to < 0)
            {
                colorizer.ChangePropertyInternally(FromProperty, 0d);
            }
            colorizer.OnChanged();
        }
    }
}
