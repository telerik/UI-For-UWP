using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class PivotSettingsStub : IDataSettings
    {
        public PivotSettingsStub()
        {
            this.FilterDescriptions = new List<object>();
            this.RowGroupDescriptions = new List<object>();
            this.ColumnGroupDescriptions = new List<object>();
            this.AggregateDescriptions = new List<object>();
            this.SortDescriptions = new List<object>();
            this.GroupFactory = new DataGroupFactory();
        }

        public IGroupFactory GroupFactory { get; private set; }

        public System.Collections.IList FilterDescriptions { get; private set; }

        public System.Collections.IList RowGroupDescriptions { get; private set; }

        public System.Collections.IList ColumnGroupDescriptions { get; private set; }

        public System.Collections.IList AggregateDescriptions { get; private set; }
        
        public System.Collections.IList SortDescriptions { get; private set; }

        public int AggregatesLevel { get; set; }

        public DataAxis AggregatesPosition { get; set; }

        event EventHandler<EventArgs> IDataSettings.DescriptionsChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<SettingsChangedEventArgs> IDataSettings.SettingsChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginEdit()
        {
            throw new NotImplementedException();
        }
    }

    internal class DataProviderStub : IDataProvider, INotifyPropertyChanged, ISupportInitialize
    {
        private IDataSettings pivotSettings = new PivotSettingsStub();

        event EventHandler<DataProviderStatusChangedEventArgs> IDataProvider.StatusChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        public bool DeferUpdates
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DataProviderStatus Status { get; set; }

        public IDataResults Results { get; set; }

        public IDataSettings Settings
        {
            get
            {
                return new PivotSettingsStub();
            }
        }

        public IFieldDescriptionProvider FieldDescriptionsProvider { get; set; }

        public DataAxis AggregatesPosition { get; set; }

        public int AggregatesLevel { get; set; }

        public object State { get; set; }

        public void Refresh()
        {
        }

        public void BlockUntilRefreshCompletes()
        {
        }

        public IDisposable DeferRefresh()
        {
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        #region ISupportInitialize Members

        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }

        #endregion

        public bool IsBusy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IAggregateDescription GetAggregateDescriptionForFieldDescription(IDataFieldInfo description)
        {
            throw new NotImplementedException();
        }

        public IGroupDescription GetGroupDescriptionForFieldDescription(IDataFieldInfo description)
        {
            throw new NotImplementedException();
        }

        public FilterDescription GetFilterDescriptionForFieldDescription(IDataFieldInfo description)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAggregateFunctionsForAggregateDescription(IAggregateDescription aggregateDescription)
        {
            throw new NotImplementedException();
        }

        public void SetAggregateFunctionToAggregateDescription(IAggregateDescription aggregateDescription, object aggregateFunction)
        {
            throw new NotImplementedException();
        }

        public SortDescription GetSortDescriptionForFieldDescription(IDataFieldInfo description)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.Collection<PropertyAggregateDescriptionBase> AggregateDescriptions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void BeginEditOperation(object item)
        {
            throw new NotImplementedException();
        }

        public void CancelEditOperation(object item)
        {
            throw new NotImplementedException();
        }

        public void CommitEditOperation(object item)
        {
            throw new NotImplementedException();
        }

        public IDataSourceView DataView
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IFieldInfoData FieldDescriptions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler FieldDescriptionsChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<ViewChangedEventArgs> ViewChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        private void OnFieldDescriptionsChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.Collection<PropertyFilterDescriptionBase> FilterDescriptions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.ObjectModel.Collection<PropertyGroupDescriptionBase> GroupDescriptions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IGroupFactory GroupFactory
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSingleThreaded
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public object ItemsSource
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Refresh(DataChangeFlags dataChangeFlags = DataChangeFlags.None)
        {
            throw new NotImplementedException();
        }

        public void ResumePropertyChanges(object item)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.Collection<SortDescription> SortDescriptions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SuspendPropertyChanges(object item)
        {
            throw new NotImplementedException();
        }

        public IValueProvider ValueProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }
#pragma warning disable 0067
        public event EventHandler<ViewChangingEventArgs> ViewChanging;
        public event EventHandler<object> CurrentChanged;
#pragma warning restore 0067
    }
}