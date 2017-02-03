using System;
using System.Collections;
using System.Collections.Specialized;
using Telerik.Core;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a logical object that may be used to feed a <see cref="RadChartBase"/> instance with data, leaving the series creation to the chart itself.
    /// </summary>
    public class ChartSeriesProvider : DependencyObject, IWeakEventListener
    {
        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(ChartSeriesProvider), new PropertyMetadata(null, OnSourceChanged));

        /// <summary>
        /// Identifies the <see cref="SeriesDescriptorSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesDescriptorSelectorProperty =
            DependencyProperty.Register(nameof(SeriesDescriptorSelector), typeof(ChartSeriesDescriptorSelector), typeof(ChartSeriesProvider), new PropertyMetadata(null, OnSeriesDescriptorSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="SeriesDescriptorSelector"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDynamicSeriesProperty =
            DependencyProperty.RegisterAttached("IsDynamicSeries", typeof(bool), typeof(ChartSeriesProvider), new PropertyMetadata(false));

        private ChartSeriesDescriptorCollection descriptors;
        private WeakReferenceList<RadChartBase> charts;
        private WeakEventHandler<NotifyCollectionChangedEventArgs> collectionChangedHandler;
        private WeakEventHandler<IVectorChangedEventArgs> vectorChangedHandler;
        private IEnumerable sourceAsEnumerable;
        private ChartSeriesDescriptorSelector descriptorSelectorCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeriesProvider" /> class.
        /// </summary>
        public ChartSeriesProvider()
        {
            this.charts = new WeakReferenceList<RadChartBase>();

            this.descriptors = new ChartSeriesDescriptorCollection();
            this.descriptors.CollectionChanged += this.OnDescriptorsCollectionChanged;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ChartSeriesProvider" /> class, detaches the weak events from the instance.
        /// </summary>
        ~ChartSeriesProvider()
        {
            this.DetachSourceEvents();
        }

        /// <summary>
        /// Notifies for a change in the Source collection. Used for testing purposes.
        /// </summary>
        internal event EventHandler SourceChanged;

        /// <summary>
        /// Gets or sets the collection of objects that contain the data for the dynamic series to be created.
        /// </summary>
        public object Source
        {
            get
            {
                return this.GetValue(SourceProperty);
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="ChartSeriesDescriptor"/> objects that specify what chart series are to be created.
        /// </summary>
        public ChartSeriesDescriptorCollection SeriesDescriptors
        {
            get
            {
                return this.descriptors;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartSeriesDescriptorSelector"/> instance that may be used for context-based descriptor selection.
        /// </summary>
        public ChartSeriesDescriptorSelector SeriesDescriptorSelector
        {
            get
            {
                return this.GetValue(SeriesDescriptorSelectorProperty) as ChartSeriesDescriptorSelector;
            }
            set
            {
                this.SetValue(SeriesDescriptorSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets the current Source (if any) casted to an IEnumerable instance.
        /// </summary>
        internal IEnumerable SourceAsEnumerable
        {
            get
            {
                return this.sourceAsEnumerable;
            }
        }

        /// <summary>
        /// Gets the WeakEventHandler that hooks the CollectionChanged event in case the Source is INotifyCollectionChanged. Exposed for testing purposes.
        /// </summary>
        internal WeakEventHandler<NotifyCollectionChangedEventArgs> CollectionChangedHandler
        {
            get
            {
                return this.collectionChangedHandler;
            }
        }

        /// <summary>
        /// Gets the WeakEventHandler that hooks the VectorChanged event in case the Source is IObservableVector. Exposed for testing purposes.
        /// </summary>
        internal WeakEventHandler<IVectorChangedEventArgs> VectorChangedHandler
        {
            get
            {
                return this.vectorChangedHandler;
            }
        }

        /// <summary>
        /// Sets a value indicating that the specified ChartSeries instance is dynamically created by a series provider instance.
        /// </summary>
        public static void SetIsDynamicSeries(DependencyObject instance, bool value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            instance.SetValue(IsDynamicSeriesProperty, value);
        }

        /// <summary>
        /// Determines whether the specified ChartSeries instance is dynamically created by a series provider.
        /// </summary>
        public static bool GetIsDynamicSeries(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            return (bool)instance.GetValue(IsDynamicSeriesProperty);
        }

        /// <summary>
        /// Forces all attached chart instances to re-evaluate all the series created from this provider.
        /// </summary>
        public void RefreshAttachedCharts()
        {
            this.NotifyListeners();
        }

        void IWeakEventListener.ReceiveEvent(object sender, object args)
        {
            if (this.SourceChanged != null)
            {
                this.SourceChanged(this, EventArgs.Empty);
            }

            if (this.Source != null)
            {
                this.NotifyListeners();
            }
        }

        internal void AddListener(RadChartBase chart)
        {
            int index = this.charts.IndexOf(chart);
            if (index < 0)
            {
                this.charts.Add(chart);
            }
        }

        internal void RemoveListener(RadChartBase chart)
        {
            this.charts.Remove(chart);
        }

        internal IEnumerable CreateSeries()
        {
            IEnumerable source = this.GetSourceAsEnumerable();
            if (source == null)
            {
                yield break;
            }

            int index = 0;
            foreach (object context in source)
            {
                var descriptor = this.GetDescriptor(index, context);
                if (descriptor != null)
                {
                    ChartSeries series = descriptor.CreateInstance(context);
                    SetIsDynamicSeries(series, true);
                    yield return series;
                }
                index++;
            }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeriesProvider provider = d as ChartSeriesProvider;

            provider.DetachSourceEvents();
            provider.sourceAsEnumerable = provider.GetSourceAsEnumerable();
            provider.AttachSourceEvents();

            provider.NotifyListeners();
        }

        private static void OnSeriesDescriptorSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var provider = d as ChartSeriesProvider;

            provider.descriptorSelectorCache = e.NewValue as ChartSeriesDescriptorSelector;
            if (provider.Source != null)
            {
                provider.NotifyListeners();
            }
        }

        private void NotifyListeners()
        {
            foreach (RadChartBase chart in this.charts)
            {
                chart.OnSeriesProviderStateChanged();
            }
        }

        private IEnumerable GetSourceAsEnumerable()
        {
            object sourceCache = this.Source;

            IEnumerable sourceAsEnumerableCache = sourceCache as IEnumerable;
            if (sourceAsEnumerableCache != null)
            {
                return sourceAsEnumerableCache;
            }

            CollectionViewSource collectionView = sourceCache as CollectionViewSource;
            if (collectionView != null)
            {
                return collectionView.View;
            }

            return null;
        }

        private void OnDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyListeners();
        }

        private ChartSeriesDescriptor GetDescriptor(int index, object context)
        {
            if (this.descriptorSelectorCache != null)
            {
                return this.descriptorSelectorCache.SelectDescriptor(this, context);
            }

            foreach (ChartSeriesDescriptor descriptor in this.descriptors)
            {
                // TODO: Consider caching-by-index if descriptor count goes above 10
                if (descriptor.CollectionIndex == index)
                {
                    return descriptor;
                }
            }

            if (this.descriptors.Count > 0)
            {
                return this.descriptors[0];
            }

            return null;
        }

        private void AttachSourceEvents()
        {
            if (this.sourceAsEnumerable == null)
            {
                return;
            }

            INotifyCollectionChanged collectionChanged = this.sourceAsEnumerable as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                this.collectionChangedHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(collectionChanged, this, KnownEvents.CollectionChanged);
            }

            ICollectionView collectionView = this.sourceAsEnumerable as ICollectionView;
            if (collectionView != null)
            {
                this.vectorChangedHandler = new WeakEventHandler<IVectorChangedEventArgs>(collectionView, this, KnownEvents.VectorChanged);
            }
        }

        private void DetachSourceEvents()
        {
            if (this.collectionChangedHandler != null)
            {
                this.collectionChangedHandler.Unsubscribe();
            }

            if (this.vectorChangedHandler != null)
            {
                this.vectorChangedHandler.Unsubscribe();
            }
        }
    }
}
