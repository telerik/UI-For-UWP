using System;
using System.IO;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Contains all the predefined palettes for <see cref="RadChartBase"/>. A predefined palette may not be further modified once loaded.
    /// </summary>
    public static class ChartPalettes
    {
        private static ChartPalette defaultDarkPalette;
        private static ChartPalette defaultDarkSelectedPalette;
        private static ChartPalette defaultLightPalette;
        private static ChartPalette defaultLightSelectedPalette;
        private static object lockInstance = new object();

        /// <summary>
        /// Gets the default <see cref="ChartPalette"/> instance, applicable to the Dark Metro Theme.
        /// </summary>
        public static ChartPalette DefaultDark
        {
            get
            {
                if (defaultDarkPalette == null)
                {
                    lock (lockInstance)
                    {
                        if (defaultDarkPalette == null)
                        {
                            defaultDarkPalette = CreateDefaultDarkPalette();
                        }
                    }
                }

                return defaultDarkPalette;
            }
        }

        /// <summary>
        /// Gets the default <see cref="ChartPalette"/> instance, used to represent selection within a RadChart instance. Applicable with the DefaultDark palette.
        /// </summary>
        public static ChartPalette DefaultDarkSelected
        {
            get
            {
                if (defaultDarkSelectedPalette == null)
                {
                    lock (lockInstance)
                    {
                        if (defaultDarkSelectedPalette == null)
                        {
                            defaultDarkSelectedPalette = CreateDefaultDarkSelectedPalette();
                        }
                    }
                }

                return defaultDarkSelectedPalette;
            }
        }

        /// <summary>
        /// Gets the default <see cref="ChartPalette"/> instance, applicable to the Light Metro Theme.
        /// </summary>
        public static ChartPalette DefaultLight
        {
            get
            {
                if (defaultLightPalette == null)
                {
                    lock (lockInstance)
                    {
                        if (defaultLightPalette == null)
                        {
                            defaultLightPalette = CreateDefaultLightPalette();
                        }
                    }
                }

                return defaultLightPalette;
            }
        }

        /// <summary>
        /// Gets the default <see cref="ChartPalette"/> instance, used to represent selection within a RadChart instance. Applicable with the DefaultLight palette.
        /// </summary>
        public static ChartPalette DefaultLightSelected
        {
            get
            {
                if (defaultLightSelectedPalette == null)
                {
                    lock (lockInstance)
                    {
                        if (defaultLightSelectedPalette == null)
                        {
                            defaultLightSelectedPalette = CreateDefaultLightSelectedPalette();
                        }
                    }
                }

                return defaultLightSelectedPalette;
            }
        }

        internal static ChartPalette FromPredefinedName(PredefinedPaletteName name)
        {
            switch (name)
            {
                case PredefinedPaletteName.DefaultDark:
                    return ChartPalettes.DefaultDark;

                case PredefinedPaletteName.DefaultDarkSelected:
                    return ChartPalettes.DefaultDarkSelected;

                case PredefinedPaletteName.DefaultLight:
                    return ChartPalettes.DefaultLight;

                case PredefinedPaletteName.DefaultLightSelected:
                    return ChartPalettes.DefaultLightSelected;

                default:
                    return null;
            }
        }

        private static ChartPalette CreateDefaultDarkPalette()
        {
            ChartPalette palette = new ChartPalette() { Name = "DefaultDark" };

            // fill
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 30, 152, 228)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 197, 0)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 42, 0)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 202, 202, 202)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 67, 67, 67)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 0, 255, 156)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 109, 49, 255)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 0, 178, 161)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 109, 255, 0)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 128, 0)));

            // stroke
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 96, 194, 255)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 225, 122)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 108, 79)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 229, 229, 229)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 84, 84, 84)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 0, 255, 156)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 130, 79, 255)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 69, 204, 191)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 185, 255, 133)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 175, 94)));

            AddCommonPaletteEntries(palette);

            return palette;
        }

        private static ChartPalette CreateDefaultDarkSelectedPalette()
        {
            ChartPalette palette = new ChartPalette() { Name = "DefaultDarkSelected" };

            // fill
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 113, 191, 239)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 253, 220, 111)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 110, 81)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 218, 218, 218)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 123, 123, 123)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 111, 255, 200)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 153, 111, 253)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 101, 210, 200)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 177, 255, 118)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 251, 170, 89)));

            // stroke
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)));

            return palette;
        }

        private static ChartPalette CreateDefaultLightPalette()
        {
            ChartPalette palette = new ChartPalette() { Name = "DefaultLight" };

            // fill
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 30, 152, 228)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 197, 0)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 42, 0)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 202, 202, 202)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 67, 67, 67)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 0, 255, 156)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 109, 49, 255)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 0, 178, 161)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 109, 255, 0)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 128, 0)));

            // stroke
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 31, 126, 184)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 222, 185, 60)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 198, 51, 22)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 178, 178, 178)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 49, 49, 49)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 65, 220, 160)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 99, 49, 224)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 27, 169, 155)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 139, 228, 73)));
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 214, 133, 52)));

            AddCommonPaletteEntries(palette);

            return palette;
        }

        private static ChartPalette CreateDefaultLightSelectedPalette()
        {
            ChartPalette palette = new ChartPalette() { Name = "DefaultLightSelected" };

            // fill
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 113, 191, 239)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 253, 220, 111)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 255, 110, 81)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 218, 218, 218)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 123, 123, 123)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 111, 255, 200)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 153, 111, 253)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 101, 210, 200)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 177, 255, 118)));
            palette.FillEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 251, 170, 89)));

            // stroke
            palette.StrokeEntries.Brushes.Add(new SolidColorBrush(Color.FromArgb(255, 75, 75, 75)));

            return palette;
        }

        private static void AddCommonPaletteEntries(ChartPalette palette)
        {
            // special fills with opacity for the Area series
            PaletteEntryCollection areaFillEntries = new PaletteEntryCollection() { SeriesFamily = "Area" };
            foreach (SolidColorBrush brush in palette.FillEntries.Brushes)
            {
                SolidColorBrush areaBrush = new SolidColorBrush(brush.Color);
                areaBrush.Opacity = 0.7;
                areaFillEntries.Brushes.Add(areaBrush);
            }
            palette.FillEntriesByFamily.Add(areaFillEntries);

            // one transparent fill for the DownFill of the Candlestick series
            palette.SpecialFillEntries.Brushes.Add(new SolidColorBrush(Colors.Transparent));

            // special stroke for Ohlc series
            PaletteEntryCollection ohlcSpecialStrokeEntries = new PaletteEntryCollection() { SeriesFamily = "Ohlc" };
            foreach (SolidColorBrush brush in palette.StrokeEntries.Brushes)
            {
                SolidColorBrush specialStroke = new SolidColorBrush(brush.Color);
                specialStroke.Opacity = 0.5;
                ohlcSpecialStrokeEntries.Brushes.Add(specialStroke);
            }
            palette.SpecialStrokeEntriesByFamily.Add(ohlcSpecialStrokeEntries);
        }
    }
}
