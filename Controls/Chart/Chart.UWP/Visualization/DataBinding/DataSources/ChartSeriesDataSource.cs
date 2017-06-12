using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class ChartSeriesDataSource
    {
        internal IEnumerable itemsSource;

        private INotifyCollectionChanged itemsSourceAsCollectionChanged;
        private IObservableVector<object> itemsSourceAsObservableVector;
        private ChartSeries owner;
        private List<DataPointBindingEntry> bindings;
        private DataPointBinding isSelectedBinding;

        protected ChartSeriesDataSource()
        {
            this.bindings = new List<DataPointBindingEntry>(8);
        }

        public DataPointBinding IsSelectedBinding
        {
            get
            {
                return this.isSelectedBinding;
            }
            set
            {
                this.isSelectedBinding = value;
                if (this.itemsSource != null)
                {
                    this.Rebind(false, null);
                }
            }
        }

        public ChartSeries Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
            set
            {
                if (this.itemsSource == value)
                {
                    return;
                }

                this.Rebind(true, value);
            }
        }

        /// <summary>
        /// Gets the binding entries corresponding to each data point in the data source.
        /// </summary>
        /// <value>The binding entries.</value>
        internal List<DataPointBindingEntry> Bindings
        {
            get
            {
                return this.bindings;
            }
        }

        internal void Rebind(bool itemsSourceChanged, IEnumerable newSource)
        {
            // do nothing if owner is null or its template is not applied yet
            if (this.owner == null || !this.owner.IsTemplateApplied)
            {
                return;
            }

            this.BeginUpdate();
            this.Unbind();

            if (itemsSourceChanged)
            {
                this.itemsSource = newSource;
                this.itemsSourceAsCollectionChanged = newSource as INotifyCollectionChanged;
                this.itemsSourceAsObservableVector = newSource as IObservableVector<object>;
            }

            this.Bind();
            this.EndUpdate();

            this.owner.OnDataBindingComplete();
        }

        protected abstract DataPoint CreateDataPoint();

        protected abstract void ProcessDouble(DataPoint point, double value);

        protected abstract void ProcessDoubleArray(DataPoint point, double[] values);

        protected abstract void ProcessNullableDoubleArray(DataPoint point, double?[] values);

        protected abstract void ProcessSize(DataPoint point, Size size);

        protected abstract void ProcessPoint(DataPoint dataPoint, Point point);

        protected virtual void InitializeBinding(DataPointBindingEntry binding)
        {
            if (this.isSelectedBinding != null)
            {
                object isSelected = this.isSelectedBinding.GetValue(binding.DataItem);
                if (!(isSelected is bool))
                {
                    throw new ArgumentException("IsSelectedBinding should return a Boolean value.");
                }

                binding.DataPoint.IsSelected = (bool)isSelected;
            }
        }

        protected virtual void BindCore()
        {
            foreach (object item in this.itemsSource)
            {
                DataPoint point = this.GenerateDataPoint(item, -1);
                this.owner.Model.DataPointsInternal.Add(point);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Better readability against small perfrormance gain")]
        protected DataPoint GenerateDataPoint(object dataItem, int index)
        {
            DataPoint point = this.CreateDataPoint();
            if (dataItem == null)
            {
                return point;
            }

            double value;
            if (NumericConverter.TryConvertToDouble(dataItem, out value))
            {
                this.ProcessDouble(point, value);
            }
            else if (dataItem is double[])
            {
                this.ProcessDoubleArray(point, (double[])dataItem);
            }
            else if (dataItem is double?[])
            {
                this.ProcessNullableDoubleArray(point, (double?[])dataItem);
            }
            else if (dataItem is Size)
            {
                this.ProcessSize(point, (Size)dataItem);
            }
            else if (dataItem is Point)
            {
                this.ProcessPoint(point, (Point)dataItem);
            }
            else
            {
                DataPointBindingEntry binding = new DataPointBindingEntry()
                {
                    DataPoint = point,
                    DataItem = dataItem
                };
                this.InitializeBinding(binding);

                if (index == -1)
                {
                    this.bindings.Add(binding);
                }
                else
                {
                    this.bindings.Insert(index, binding);
                }

                this.HookPropertyChanged(dataItem);
            }

            // keep a reference to the associated data item
            point.dataItem = dataItem;

            //// TODO: Do we need to support other primitive types?

            return point;
        }

        /// <summary>
        /// Called when a property of a bound object changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnBoundItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DataPointBindingEntry binding = this.FindBinding(sender);
            if (binding != null)
            {
                this.UpdateBinding(binding);
                this.owner.OnBoundItemPropertyChanged();
            }
            else
            {
                this.Rebind(false, null);
            }
        }

        /// <summary>
        /// Updates the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        protected virtual void UpdateBinding(DataPointBindingEntry binding)
        {
            this.InitializeBinding(binding);
        }

        protected virtual void Unbind()
        {
            if (this.itemsSourceAsCollectionChanged != null)
            {
                this.itemsSourceAsCollectionChanged.CollectionChanged -= this.OnSourceCollectionChanged;
            }

            if (this.itemsSourceAsObservableVector != null)
            {
                this.itemsSourceAsObservableVector.VectorChanged -= this.OnSourceCollectionChanged;
            }

            if (this.itemsSource != null)
            {
                foreach (object item in this.itemsSource)
                {
                    this.UnhookPropertyChanged(item);
                }
            }

            this.bindings.Clear();
            this.owner.Model.DataPointsInternal.Clear();
        }

        private void BeginUpdate()
        {
            this.owner.Model.canModifyDataPoints = true;
        }

        private void EndUpdate()
        {
            this.owner.Model.canModifyDataPoints = false;
            this.owner.Model.isDataBound = this.itemsSource != null;
        }

        private void Bind()
        {
            if (this.itemsSource == null)
            {
                return;
            }

            if (this.itemsSourceAsCollectionChanged != null)
            {
                this.itemsSourceAsCollectionChanged.CollectionChanged += this.OnSourceCollectionChanged;
            }

            if (this.itemsSourceAsObservableVector != null)
            {
                this.itemsSourceAsObservableVector.VectorChanged += this.OnSourceCollectionChanged;
            }

            this.BindCore();
        }        

        private void HookPropertyChanged(object item)
        {
            INotifyPropertyChanged propChanged = item as INotifyPropertyChanged;
            if (propChanged != null)
            {
                propChanged.PropertyChanged += this.OnBoundItemPropertyChanged;
            }
        }

        private void UnhookPropertyChanged(object item)
        {
            INotifyPropertyChanged propChanged = item as INotifyPropertyChanged;
            if (propChanged != null)
            {
                propChanged.PropertyChanged -= this.OnBoundItemPropertyChanged;
            }
        }

        private DataPointBindingEntry FindBinding(object changedInstance)
        {
            foreach (DataPointBindingEntry binding in this.bindings)
            {
                if (object.Equals(changedInstance, binding.DataItem))
                {
                    return binding;
                }
            }

            return null;
        }

        private void OnSourceCollectionChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            // TODO: This is highly inefficient, revisit for the official release.
            this.Rebind(false, null);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.Rebind(false, null);
                    break;
                case NotifyCollectionChangedAction.Add:
                    this.HandleItemAdd(e);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.HandleItemRemove(e);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    // TODO: possible optimization
                    this.HandleItemReplace(e);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.HandleItemMove(e);
                    break;
            }
        }

        private void HandleItemAdd(NotifyCollectionChangedEventArgs e)
        {
            this.BeginUpdate();

            this.PerformAdd(e.NewItems[0], e.NewStartingIndex);

            this.EndUpdate();
        }

        private void HandleItemRemove(NotifyCollectionChangedEventArgs e)
        {
            this.BeginUpdate();

            this.PerformRemove(e.OldItems[0], e.OldStartingIndex);

            this.EndUpdate();
        }

        private void HandleItemReplace(NotifyCollectionChangedEventArgs e)
        {
            this.BeginUpdate();

            // Note: We cannot represent the Replace operation as a combination of Remove & Add due to a bug in Silverlight
            // that always returns erroneous NotifyCollectionChangedEventArgs.OldStartingIndex = -1.
            this.PerformRemove(e.OldItems[0], e.NewStartingIndex);
            this.PerformAdd(e.NewItems[0], e.NewStartingIndex);

            this.EndUpdate();
        }

        private void HandleItemMove(NotifyCollectionChangedEventArgs e)
        {
            this.BeginUpdate();

            this.PerformRemove(e.OldItems[0], e.OldStartingIndex);
            this.PerformAdd(e.NewItems[0], e.NewStartingIndex);

            this.EndUpdate();
        }

        private void PerformAdd(object newDataItem, int newItemIndex)
        {
            DataPoint point = this.GenerateDataPoint(newDataItem, newItemIndex);
            this.owner.Model.DataPointsInternal.Insert(newItemIndex, point);
        }

        private void PerformRemove(object removedDataItem, int removedItemIndex)
        {
            this.UnhookPropertyChanged(removedDataItem);

            // try to remove an existing binding
            if (removedItemIndex >= 0 && removedItemIndex < this.bindings.Count)
            {
                DataPointBindingEntry binding = this.bindings[removedItemIndex];
                if (object.Equals(binding.DataItem, removedDataItem))
                {
                    this.bindings.RemoveAt(removedItemIndex);
                }
            }

            this.owner.Model.DataPointsInternal.RemoveAt(removedItemIndex);
        }
    }
}
