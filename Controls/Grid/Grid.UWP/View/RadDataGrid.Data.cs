using System;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        /// <summary>
        /// Identifies the <see cref="IncrementalLoadingMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementalLoadingModeProperty =
            DependencyProperty.Register(nameof(IncrementalLoadingMode), typeof(BatchLoadingMode), typeof(RadDataGrid), new PropertyMetadata(BatchLoadingMode.Auto, OnIncrementalLoadingModeChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RadDataGrid), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        public object ItemsSource
        {
            get
            {
                return this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the incremental loading mode.
        /// </summary>
        /// <value>The incremental loading mode.</value>
        public BatchLoadingMode IncrementalLoadingMode
        {
            get
            {
                return (BatchLoadingMode)this.GetValue(IncrementalLoadingModeProperty);
            }
            set
            {
                this.SetValue(IncrementalLoadingModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="SortDescriptorBase"/> objects that defines the current sorting within this instance.
        /// Multiple sort descriptors define a sorting operation by multiple keys.
        /// </summary>
        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.model.SortDescriptors;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="GroupDescriptorBase"/> objects that defines the current grouping within this instance.
        /// Multiple group descriptors define multiple group levels.
        /// </summary>
        public GroupDescriptorCollection GroupDescriptors
        {
            get
            {
                return this.model.GroupDescriptors;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="FilterDescriptorBase"/> objects that defines the current filtering within this instance.
        /// </summary>
        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.model.FilterDescriptors;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="AggregateDescriptorBase"/> objects that defines the current aggregate functions to be applied when the data view is computed.
        /// </summary>
        public AggregateDescriptorCollection AggregateDescriptors
        {
            get
            {
                return this.model.AggregateDescriptors;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDataView"/> instance that can be used to traverse and/or manipulate the data after all the Sort, Group and Filter operations are applied.
        /// </summary>
        public IDataView GetDataView()
        {
            return new DataGridDataView(this);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            if (grid != null)
            {
                grid.GroupDescriptors.TryRemoveCollectionViewGroup();

                var collView = e.NewValue as ICollectionView;
                if (collView != null && collView.CollectionGroups != null)
                {
                   grid.GroupDescriptors.Insert(0, new CollectionViewGroupDescriptor());
                }

                grid.model.OnItemsSourceChanged(e.NewValue);
                grid.CurrencyService.OnItemsSourceChanged(e.NewValue);
                grid.selectionService.ClearSelection();
            }
        }

        private static void OnIncrementalLoadingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            if (grid != null)
            {
                grid.model.DataLoadingMode = (BatchLoadingMode)e.NewValue;

                if (grid.IsTemplateApplied && grid.DecorationLayer != null)
                {
                    Action action = () =>
                    {
                        if (grid.GroupDescriptors.Count > 0)
                        {
                            grid.visualStateService.UnregisterDataLoadingListener(grid.scrolalbleAdornerLayerCache.Listener);
                        }
                    };

                    if (grid.model.DataLoadingMode == BatchLoadingMode.Auto && grid.GroupDescriptors.Count == 0)
                    {
                        grid.visualStateService.RegisterDataLoadingListener(grid.scrolalbleAdornerLayerCache.Listener);
                    }
                    else
                    {
                        grid.visualStateService.UnregisterDataLoadingListener(grid.scrolalbleAdornerLayerCache.Listener);
                    }
                }
            }
        }
    }
}