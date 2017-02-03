using System;
using System.Collections;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an abstract definition of a <see cref="ChartSeries"/> instance. 
    /// Used together with a <see cref="ChartSeriesProvider"/> instance to provide dynamic 
    /// chart series generation, depending on the data specified.
    /// The descriptors form a neat hierarchy based on the type of data 
    /// visualized - e.g. Categorical, Scatter, Financial, etc.
    /// This is the base class which encapsulates all the common functionality for all concrete descriptors.
    /// </summary>
    public abstract class ChartSeriesDescriptor : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Style"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(nameof(Style), typeof(Style), typeof(ChartSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemsSourcePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourcePathProperty =
            DependencyProperty.Register(nameof(ItemsSourcePath), typeof(string), typeof(ChartSeriesDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LegendTitlePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitlePathProperty =
            DependencyProperty.Register(nameof(LegendTitlePath), typeof(string), typeof(ChartSeriesDescriptor), new PropertyMetadata(null, OnLegendTitilePathChanged));

        /// <summary>
        /// Identifies the <see cref="TypePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TypePathProperty =
            DependencyProperty.Register(nameof(TypePath), typeof(string), typeof(ChartSeriesDescriptor), new PropertyMetadata(null, OnTypePathChanged));

        /// <summary>
        /// Identifies the <see cref="CollectionIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollectionIndexProperty =
            DependencyProperty.Register(nameof(CollectionIndex), typeof(int), typeof(ChartSeriesDescriptor), new PropertyMetadata(-1));

        private Func<object, object> typeGetter;
        private string typePathCache;

        private Func<object, object> legendTitleGetter;
        private string legendTitlePathCache;

        /// <summary>
        /// Gets the default type of series that are to be created if no TypePath and no Style properties are specified.
        /// </summary>
        public abstract Type DefaultType
        {
            get;
        }

        /// <summary>
        /// Gets or sets the name of the property that defines the LegendTitle for each series created.
        /// </summary>
        public string LegendTitlePath
        {
            get
            {
                return this.legendTitlePathCache;
            }
            set
            {
                this.SetValue(LegendTitlePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the series type that needs to be created.
        /// </summary>
        public string TypePath
        {
            get
            {
                return this.GetValue(TypePathProperty) as string;
            }
            set
            {
                this.SetValue(TypePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style that describes the appearance of the series that are to be created.
        /// If no TypePath is specified, the TargetType property of this style object is used to generate the desired series.
        /// </summary>
        public Style Style
        {
            get
            {
                return this.GetValue(StyleProperty) as Style;
            }
            set
            {
                this.SetValue(StyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that points to the items source that will feed the generated series.
        /// </summary>
        public string ItemsSourcePath
        {
            get
            {
                return this.GetValue(ItemsSourcePathProperty) as string;
            }
            set
            {
                this.SetValue(ItemsSourcePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the index within the Source collection of data (view models) for which the current
        /// descriptor should be used. This property is useful when for example a <see cref="BarSeries"/>
        /// needs to be generated for the first data entry and <see cref="LineSeries"/> for the rest of the entries.
        /// </summary>
        public int CollectionIndex
        {
            get
            {
                return (int)this.GetValue(CollectionIndexProperty);
            }
            set
            {
                this.SetValue(CollectionIndexProperty, value);
            }
        }

        /// <summary>
        /// Creates an instance of the <see cref="ChartSeries"/> type, defined by this descriptor.
        /// </summary>
        /// <param name="context">The context (this is the raw data collection or the data view model) for which a 
        /// <see cref="ChartSeries"/> needs to be created.</param>
        public ChartSeries CreateInstance(object context)
        {
            ChartSeries series = this.CreateInstanceCore(context);
            series.DataContext = context;

            this.BindItemsSource(series);

            var title = this.GetLegendTitle(context);
            if (!string.IsNullOrEmpty(title))
            {
                series.SetDynamicLegendTitle(this.legendTitlePathCache, title);
            }

            return series;
        }

        /// <summary>
        /// Core entry point for creating the <see cref="ChartSeries"/> type defined by this descriptor. Allows inheritors to provide custom implementation.
        /// </summary>
        /// <param name="context">The context (this is the raw data collection or the data view model) for which a <see cref="ChartSeries"/> needs to be created.</param>
        protected virtual ChartSeries CreateInstanceCore(object context)
        {
            return this.CreateDefaultInstance(context);
        }

        /// <summary>
        /// Provides the default implementation for the <see cref="M:CreateInstance"/> routine.
        /// </summary>
        protected ChartSeries CreateDefaultInstance(object context)
        {
            Type type = this.ResolveType(context);
            if (type == null)
            {
                throw new InvalidOperationException("Unknown chart series type");
            }

            ChartSeries series = Activator.CreateInstance(type) as ChartSeries;
            Style style = this.Style;
            if (style != null)
            {
                series.Style = style;
            }

            return series;
        }

        private static void OnTypePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeriesDescriptor descriptor = d as ChartSeriesDescriptor;
            descriptor.typePathCache = e.NewValue as string;
            descriptor.typeGetter = null;
        }

        private static void OnLegendTitilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeriesDescriptor descriptor = d as ChartSeriesDescriptor;
            descriptor.legendTitlePathCache = e.NewValue as string;
            descriptor.legendTitleGetter = null;
        }

        private Type ResolveType(object context)
        {
            // TypePath property is with highest priority
            if (!string.IsNullOrEmpty(this.typePathCache))
            {
                if (this.typeGetter == null)
                {
                    this.typeGetter = DynamicHelper.CreatePropertyValueGetter(context.GetType(), this.typePathCache);
                }

                Type type = this.typeGetter(context) as Type;

                if (type == null)
                {
                    throw new ArgumentException("TypePath property should point to a valid Type property.");
                }

                return type;
            }

            // Next is the Style.TargetType (if Style is specified)
            Style style = this.Style;
            if (style != null)
            {
                return style.TargetType;
            }

            // Last is the DefaultType
            return this.DefaultType;
        }

        private string GetLegendTitle(object context)
        {
            if (string.IsNullOrEmpty(this.LegendTitlePath))
            {
                return string.Empty;
            }

            if (this.legendTitleGetter == null)
            {
                this.legendTitleGetter = DynamicHelper.CreatePropertyValueGetter(context.GetType(), this.legendTitlePathCache);
            }

            return this.legendTitleGetter(context) as string;
        }

        private void BindItemsSource(ChartSeries series)
        {
            Binding binding = new Binding();

            string itemsSourcePath = this.ItemsSourcePath;
            if (!string.IsNullOrEmpty(itemsSourcePath))
            {
                binding.Path = new PropertyPath(itemsSourcePath);
            }

            series.SetBinding(ChartSeries.ItemsSourceProperty, binding);
        }
    }
}
