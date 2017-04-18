using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public partial class LocalDataSourceProviderTests
    {
        private LocalDataSourceProvider provider;
        private IDataProvider providerExplicit;
        private List<DataProviderStatusChangedEventArgs> statusChangedEventsSequence;

        private void OnProviderStatusChanged(object sender, DataProviderStatusChangedEventArgs e)
        {
            this.statusChangedEventsSequence.Add(e);
        }

        private void ClearStatusChangedEvents()
        {
            this.statusChangedEventsSequence.Clear();
        }

        private DataProviderStatusChangedEventArgs GetLastStatusChangedEvent()
        {
            return this.statusChangedEventsSequence.Last();
        }

        private bool StatuschangedEventWasRaised()
        {
            return this.statusChangedEventsSequence.Count > 0;
        }

        private void AssertStatusChangedEventSequence(List<DataProviderStatusChangedEventArgs> expectedSequence)
        {
            CollectionAssert.AreEqual(expectedSequence, this.statusChangedEventsSequence, new StatusChangedComparer());
        }

        private class StatusChangedComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var statusChangedX = x as DataProviderStatusChangedEventArgs;
                var statusChangedY = y as DataProviderStatusChangedEventArgs;

                var statusSame = Object.Equals(statusChangedX.NewStatus, statusChangedY.NewStatus);
                var errorSame = Object.Equals(statusChangedX.Error, statusChangedY.Error);
                var dataChangedStame = Object.Equals(statusChangedX.ResultsChanged, statusChangedY.ResultsChanged);

                if (statusSame && errorSame && dataChangedStame)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }

        private IEnumerable<Type> GetNumericTypes()
        {
            return new List<Type>()
            {
               typeof(double),
               typeof(Nullable<double>),
               typeof(int),
               typeof(Nullable<int>),
               typeof(byte),
               typeof(Nullable<byte>),
               typeof(short),
               typeof(Nullable<short>),
               typeof(decimal),
               typeof(Nullable<decimal>),
               typeof(float),
               typeof(Nullable<float>),
               typeof(long),
               typeof(Nullable<long>),
               typeof(uint),
               typeof(Nullable<uint>),
               typeof(sbyte),
               typeof(Nullable<sbyte>),
               typeof(ushort),
               typeof(Nullable<ushort>),
               typeof(ulong),
               typeof(Nullable<ulong>)           
            };
        }

        private void ConfigureProviderToProduceResults()
        {
            (this.provider as IDataProvider).FieldDescriptionsProvider = new SynchronousLocalDataSourceFieldDescriptionsProvider(this.provider);
            this.provider.GroupFactory = new DataGroupFactory();
            this.provider.ItemsSource = Order.GetData();

            this.provider.RowGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            this.provider.ColumnGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Quantity" });
            this.provider.StatusChanged += OnProviderStatusChanged;
        }

#if !SILVERLIGHT && !NETFX_CORE
        private void ConfigureProviderWithDBNullsToProduceResults()
        {
            this.provider.FieldDescriptionsProvider = new SynchronousLocalDataSourceFieldDescriptionsProvider(this.provider);
            this.provider.ItemsSource = Order.GetDataWithDBNulls();
            this.provider.RowGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            this.provider.ColumnGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Quantity" });
            this.provider.StatusChanged += OnProviderStatusChanged;
        }
#endif

        private void InitializeProviderWithXmlaClientThatReturnsErrorOnSendRequestCompleted()
        {
            var fieldInfoProvider = new FieldDescriptionProviderBaseStub();
            fieldInfoProvider.DataToReturn = new EmptyFieldInfoData();

            var engine = new PivotEngineMock();
            engine.SetToReturnErrorResponse();

            this.provider = new LocalDataSourceProvider(engine, fieldInfoProvider);
            this.provider.GroupFactory = new DataGroupFactory();
            this.provider.DeferUpdates = true;
            this.provider.RowGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            this.provider.ColumnGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Quantity" });
            this.provider.StatusChanged += OnProviderStatusChanged;
        }

        private void InitializeProviderThatCannotRetrieveFieldInfosOnRefresh()
        {
            var fieldInfoProvider = new FieldDescriptionProviderBaseStub();
            fieldInfoProvider.ErrorToReturn = new Exception("Dummy exception");

            var engine = new PivotEngineMock();

            this.provider = new LocalDataSourceProvider(engine, fieldInfoProvider);
            this.provider.GroupFactory = new DataGroupFactory();
            this.provider.DeferUpdates = true;
            this.provider.RowGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            this.provider.ColumnGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Quantity" });
            this.provider.StatusChanged += OnProviderStatusChanged;
        }

        private void InitializeProviderThatIsGettingFieldInfosOnRefresh()
        {
            var fieldInfoProvider = new FieldDescriptionProviderBaseStub();
            fieldInfoProvider.ActionOnGetDescriptionsDataAsync = (s) => { /* do not return */ };

            var engine = new PivotEngineMock();

            this.provider = new LocalDataSourceProvider(engine, fieldInfoProvider);
            this.provider.GroupFactory = new DataGroupFactory();
            this.provider.DeferUpdates = true;
            this.provider.RowGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            this.provider.ColumnGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Quantity" });
            this.provider.StatusChanged += OnProviderStatusChanged;
        }

        private void InitializeProviderThatHasReceivedFieldInfosOnRefresh()
        {
            var fieldInfoProvider = new FieldDescriptionProviderBaseStub();
            fieldInfoProvider.DataToReturn = new EmptyFieldInfoData();

            var engine = new PivotEngineMock();

            this.provider = new LocalDataSourceProvider(engine, fieldInfoProvider);
            this.provider.GroupFactory = new DataGroupFactory();
            this.provider.DeferUpdates = true;
            this.provider.RowGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            this.provider.ColumnGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Quantity" });
            this.provider.StatusChanged += OnProviderStatusChanged;
        }

        private void InitializeProviderThatWillRetrieveDataOnRefresh()
        {
            var fieldInfoProvider = new FieldDescriptionProviderBaseStub();
            fieldInfoProvider.DataToReturn = new EmptyFieldInfoData();

            var engine = new PivotEngineMock();
            engine.SetToReturnResponse();

            this.provider = new LocalDataSourceProvider(engine, fieldInfoProvider);
            this.provider.GroupFactory = new DataGroupFactory();
            this.provider.DeferUpdates = true;
            this.provider.RowGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            this.provider.ColumnGroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Quantity" });
            this.provider.StatusChanged += OnProviderStatusChanged;
        }
        

        [TestInitialize]
        public void TestInitialize()
        {
            this.provider = new LocalDataSourceProvider();
            this.provider.GroupFactory = new DataGroupFactory();
            this.providerExplicit = this.provider as IDataProvider;
            this.provider.StatusChanged += OnProviderStatusChanged;
            this.statusChangedEventsSequence = new List<DataProviderStatusChangedEventArgs>();
            this.items = new System.Collections.ObjectModel.ObservableCollection<Item>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.provider.StatusChanged -= OnProviderStatusChanged;
            this.provider.BlockUntilRefreshCompletes();
            this.ClearStatusChangedEvents();
        }

        public class SomeSyncContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback d, object state)
            {
                base.Send(d, state);
            }
        }

        private class BrokenComparer : GroupComparer
        {
            public override int CompareGroups(IAggregateResultProvider results, IGroup x, IGroup y, DataAxis axis)
            {
                throw new NotImplementedException();
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new BrokenComparer();
            }

            protected override void CloneCore(Cloneable source)
            {
            }
        }

        private class ListAggregate : AggregateValue
        {
            IList<int> items = new List<int>();

            protected override object GetValueOverride()
            {
                return this.items;
            }

            protected override void AccumulateOverride(object value)
            {
                items.Add((int)value);
            }

            protected override void MergeOverride(AggregateValue childAggregate)
            {
                ListAggregate agg = childAggregate as ListAggregate;
                foreach (var item in agg.items)
                {
                    this.items.Add(item);
                }
            }
        }

        private class ListAggregateFunction : AggregateFunction
        {
            protected internal override AggregateValue CreateAggregate(Type dataType)
            {
                return new ListAggregate();
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new ListAggregateFunction();
            }

            protected override void CloneCore(Cloneable source)
            {
            }
        }

        private class ListAggregateDescription : PropertyAggregateDescriptionBase
        {
            public ListAggregateDescription()
            {
                this.AggregateFunction = new ListAggregateFunction();
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new ListAggregateDescription();
            }

            protected override void CloneOverride(Cloneable source)
            {
            }
        }

        private class DelegateCondition : Condition
        {
            Predicate<object> d;

            public DelegateCondition(Predicate<object> d)
            {
                this.d = d;
            }

            public override bool PassesFilter(object item)
            {
                return d(item);
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new DelegateCondition(this.d);
            }

            protected override void CloneCore(Cloneable source)
            {

            }
        }
    }
}