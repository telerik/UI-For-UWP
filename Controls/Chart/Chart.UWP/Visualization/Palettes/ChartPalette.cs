using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines the palette semantic for a <see cref="RadChartBase"/> instance.
    /// The palette contains several <see cref="PaletteEntryCollection"/> instances, one for each <see cref="PaletteVisualPart"/> value.
    /// </summary>
    [Bindable]
    public class ChartPalette : DependencyObject
    {
        internal const string PieFamily = "Pie";
        internal const string AreaFamily = "Area";
        internal const string PolarAreaFamily = "PolarArea";
        internal const string BarFamily = "Bar";
        internal const string LineFamily = "Line";
        internal const string PointFamily = "Point";
        internal const string OhlcFamily = "Ohlc";
        internal const string CandlestickFamily = "Candlestick";

        private PaletteEntryCollection fillEntries = new PaletteEntryCollection();
        private PaletteEntryCollection strokeEntries = new PaletteEntryCollection();
        private PaletteEntryCollection specialFillEntries = new PaletteEntryCollection();
        private PaletteEntryCollection specialStrokeEntries = new PaletteEntryCollection();

        private Collection<PaletteEntryCollection> fillEntriesByFamily = new Collection<PaletteEntryCollection>();
        private Collection<PaletteEntryCollection> strokeEntriesByFamily = new Collection<PaletteEntryCollection>();
        private Collection<PaletteEntryCollection> specialFillEntriesByFamily = new Collection<PaletteEntryCollection>();
        private Collection<PaletteEntryCollection> specialStrokeEntriesByFamily = new Collection<PaletteEntryCollection>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPalette"/> class.
        /// </summary>
        public ChartPalette()
        {
        }

        /// <summary>
        /// Gets or sets the user-friendly identifier for the palette.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that stores entries not related to any particular series.
        /// </summary>
        public PaletteEntryCollection FillEntries
        {
            get
            {
                return this.fillEntries;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(this.FillEntries));
                }

                this.fillEntries = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection that stores entries not related to any particular series.
        /// </summary>
        public PaletteEntryCollection SpecialFillEntries
        {
            get
            {
                return this.specialFillEntries;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(this.SpecialFillEntries));
                }

                this.specialFillEntries = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection that stores entries not related to any particular series.
        /// </summary>
        public PaletteEntryCollection StrokeEntries
        {
            get
            {
                return this.strokeEntries;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(this.StrokeEntries));
                }

                this.strokeEntries = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection that stores entries not related to any particular series.
        /// </summary>
        public PaletteEntryCollection SpecialStrokeEntries
        {
            get
            {
                return this.specialStrokeEntries;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(this.SpecialStrokeEntries));
                }

                this.specialStrokeEntries = value;
            }
        }

        /// <summary>
        /// Gets the collection of fill brushes, organized by series family.
        /// </summary>
        internal Collection<PaletteEntryCollection> FillEntriesByFamily
        {
            get
            {
                return this.fillEntriesByFamily;
            }
        }

        /// <summary>
        /// Gets the collection of stroke brushes, organized by series family.
        /// </summary>
        internal Collection<PaletteEntryCollection> StrokeEntriesByFamily
        {
            get
            {
                return this.strokeEntriesByFamily;
            }
        }

        /// <summary>
        /// Gets the collection of stroke brushes, organized by series family.
        /// </summary>
        internal Collection<PaletteEntryCollection> SpecialFillEntriesByFamily
        {
            get
            {
                return this.specialFillEntriesByFamily;
            }
        }

        /// <summary>
        /// Gets the collection of stroke brushes, organized by series family.
        /// </summary>
        internal Collection<PaletteEntryCollection> SpecialStrokeEntriesByFamily
        {
            get
            {
                return this.specialStrokeEntriesByFamily;
            }
        }

        /// <summary>
        /// Gets the <see cref="Brush"/> (if present) for the specified <see cref="PaletteVisualPart"/> instance at the specified index.
        /// </summary>
        /// <param name="index">The index for which to look-up a brush.</param>
        /// <param name="visualPart">The visual part for which a brush is required.</param>
        public Brush GetBrush(int index, PaletteVisualPart visualPart)
        {
            return this.GetBrush(index, visualPart, string.Empty);
        }

        /// <summary>
        /// Gets the <see cref="Brush"/> (if present) for the specified <see cref="PaletteVisualPart"/> instance at the specified index and for the specified series family.
        /// </summary>
        /// <param name="index">The index for which to look-up a brush.</param>
        /// <param name="visualPart">The visual part for which a brush is required.</param>
        /// <param name="seriesFamily">The family of the series that request the brush.</param>
        public Brush GetBrush(int index, PaletteVisualPart visualPart, string seriesFamily)
        {
            PaletteEntryCollection brushCollection;
            switch (visualPart)
            {
                case PaletteVisualPart.Stroke:
                    if (!string.IsNullOrEmpty(seriesFamily))
                    {
                        brushCollection = FindCollectionByFamily(this.strokeEntriesByFamily, seriesFamily);
                        if (brushCollection != null)
                        {
                            break;
                        }
                    }

                    brushCollection = this.strokeEntries;
                    break;
                case PaletteVisualPart.SpecialStroke:
                    if (!string.IsNullOrEmpty(seriesFamily))
                    {
                        brushCollection = FindCollectionByFamily(this.specialStrokeEntriesByFamily, seriesFamily);
                        if (brushCollection != null)
                        {
                            break;
                        }
                    }

                    brushCollection = this.specialStrokeEntries;
                    break;
                case PaletteVisualPart.SpecialFill:
                    if (!string.IsNullOrEmpty(seriesFamily))
                    {
                        brushCollection = FindCollectionByFamily(this.specialFillEntriesByFamily, seriesFamily);
                        if (brushCollection != null)
                        {
                            break;
                        }
                    }

                    brushCollection = this.specialFillEntries;
                    break;
                default: // PaletteVisualPart.Fill
                    if (!string.IsNullOrEmpty(seriesFamily))
                    {
                        brushCollection = FindCollectionByFamily(this.fillEntriesByFamily, seriesFamily);
                        if (brushCollection != null)
                        {
                            break;
                        }
                    }

                    brushCollection = this.fillEntries;
                    break;
            }

            if (index >= 0 && brushCollection.Brushes.Count > 0)
            {
                return brushCollection.Brushes[index % brushCollection.Brushes.Count];
            }

            return null;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        private static PaletteEntryCollection FindCollectionByFamily(Collection<PaletteEntryCollection> collection, string family)
        {
            // TODO: Consider caching in a Dictionary in case the number of entries on a per Family basis exceeds 9
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (collection[i].SeriesFamily == family)
                {
                    return collection[i];
                }
            }

            return null;
        }
    }
}
