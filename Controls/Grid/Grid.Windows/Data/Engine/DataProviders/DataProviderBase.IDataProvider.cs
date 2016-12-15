using System.Collections.Generic;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Base implementation of <see cref="IDataProvider"/>.
    /// </summary>
    internal abstract partial class DataProviderBase
    {
        public event System.EventHandler<ViewChangedEventArgs> ViewChanged;

        DataProviderStatus IDataProvider.Status
        {
            get { return this.status; }
        }

        IDataResults IDataProvider.Results
        {
            get
            {
                return this.Results;
            }
        }

        IDataSettings IDataProvider.Settings
        {
            get
            {
                return this.Settings;
            }
        }

        DataAxis IDataProvider.AggregatesPosition
        {
            get
            {
                return this.AggregatesPosition;
            }

            set
            {
                this.AggregatesPosition = value;
            }
        }

        int IDataProvider.AggregatesLevel
        {
            get
            {
                return this.AggregatesLevel;
            }

            set
            {
                this.AggregatesLevel = value;
            }
        }

        public abstract bool IsSingleThreaded { get; set; }

        public abstract IGroupFactory GroupFactory { get; set; }

        public abstract IFieldInfoData FieldDescriptions { get; }

        public abstract object ItemsSource { get; set; }

        void IDataProvider.Refresh(DataChangeFlags dataChangeFlags)
        {
            this.Refresh(dataChangeFlags);
        }

        IAggregateDescription IDataProvider.GetAggregateDescriptionForFieldDescription(IDataFieldInfo description)
        {
            return this.GetAggregateDescriptionForFieldDescription(description);
        }

        IGroupDescription IDataProvider.GetGroupDescriptionForFieldDescription(IDataFieldInfo description)
        {
            return this.GetGroupDescriptionForFieldDescription(description);
        }

        FilterDescription IDataProvider.GetFilterDescriptionForFieldDescription(IDataFieldInfo description)
        {
            return this.GetFilterDescriptionForFieldDescription(description);
        }

        SortDescription IDataProvider.GetSortDescriptionForFieldDescription(IDataFieldInfo description)
        {
            return this.GetSortDescriptionForFieldDescription(description);
        }

        IEnumerable<object> IDataProvider.GetAggregateFunctionsForAggregateDescription(IAggregateDescription aggregateDescription)
        {
            return this.GetAggregateFunctionsForAggregateDescription(aggregateDescription);
        }

        void IDataProvider.SetAggregateFunctionToAggregateDescription(IAggregateDescription aggregateDescription, object aggregateFunction)
        {
            this.SetAggregateFunctionToAggregateDescription(aggregateDescription, aggregateFunction);
        }

        public abstract void SuspendPropertyChanges(object item);
        void IDataProvider.SuspendPropertyChanges(object item)
        {
            this.SuspendPropertyChanges(item);
        }

        public abstract void ResumePropertyChanges(object item);
        void IDataProvider.ResumePropertyChanges(object item)
        {
            this.ResumePropertyChanges(item);
        }

        public abstract void BeginEditOperation(object item);
        void IDataProvider.BeginEditOperation(object item)
        {
            this.BeginEditOperation(item);
        }

        public abstract void CancelEditOperation(object item);
        void IDataProvider.CancelEditOperation(object item)
        {
            this.CancelEditOperation(item);
        }

        public abstract void CommitEditOperation(object item);
        void IDataProvider.CommitEditOperation(object item)
        {
            this.CommitEditOperation(item);
        }
    }
}