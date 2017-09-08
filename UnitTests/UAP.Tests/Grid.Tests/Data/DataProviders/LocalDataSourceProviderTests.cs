using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Telerik.Core.Data;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public partial class LocalDataSourceProviderTests
    {
        [TestMethod]
        public void Settings_WhenInstanceIsCreated_IsSetToAnInstance()
        {
            Assert.IsNotNull(this.providerExplicit.Settings);
        }

        [TestMethod]
        public void Refresh_WhenCalledForTheFirstTime_WhenItemsSourceIsSet_WhenFieldDescriptionsProviderHasReturnedData_InitializesMemberAccessForAllRowGroupDescriptions()
        {
            (this.provider as IDataProvider).FieldDescriptionsProvider = new SynchronousLocalDataSourceFieldDescriptionsProvider(this.provider);
            this.provider.ItemsSource = Order.GetSmallData();
            var groupDefinition1 = new PropertyGroupDescription() { PropertyName = "Product" };
            var groupDefinition2 = new PropertyGroupDescription() { PropertyName = "Quantity" };
            this.provider.RowGroupDescriptions.Add(groupDefinition1);
            this.provider.RowGroupDescriptions.Add(groupDefinition2);

            this.provider.Refresh();

            Assert.IsNotNull(groupDefinition1.MemberAccess);
            Assert.IsNotNull(groupDefinition2.MemberAccess);
        }

        [TestMethod]
        public void Refresh_WhenCalledForTheFirstTime_WhenItemsSourceIsSet_WhenFieldDescriptionsProviderHasReturnedData_InitializesMemberAccessForAllColumnGroupDescriptions()
        {
            (this.provider as IDataProvider).FieldDescriptionsProvider = new SynchronousLocalDataSourceFieldDescriptionsProvider(this.provider);
            this.provider.ItemsSource = Order.GetSmallData();
            var groupDefinition1 = new PropertyGroupDescription() { PropertyName = "Product" };
            var groupDefinition2 = new PropertyGroupDescription() { PropertyName = "Quantity" };
            this.provider.ColumnGroupDescriptions.Add(groupDefinition1);
            this.provider.ColumnGroupDescriptions.Add(groupDefinition2);

            this.provider.Refresh();

            Assert.IsNotNull(groupDefinition1.MemberAccess);
            Assert.IsNotNull(groupDefinition2.MemberAccess);
        }

        [TestMethod]
        public void Refresh_WhenCalledForTheFirstTime_WhenItemsSourceIsSet_WhenFieldDescriptionsProviderHasReturnedData_InitializesMemberAccessForAllAggregateDescriptions()
        {
            (this.provider as IDataProvider).FieldDescriptionsProvider = new SynchronousLocalDataSourceFieldDescriptionsProvider(this.provider);
            this.provider.ItemsSource = Order.GetSmallData();
            var aggregateDefinition1 = new PropertyAggregateDescription() { PropertyName = "Product" };
            var aggregateDefinition2 = new PropertyAggregateDescription() { PropertyName = "Quantity" };
            this.provider.AggregateDescriptions.Add(aggregateDefinition1);
            this.provider.AggregateDescriptions.Add(aggregateDefinition2);

            this.provider.Refresh();

            Assert.IsNotNull(aggregateDefinition1.MemberAccess);
            Assert.IsNotNull(aggregateDefinition2.MemberAccess);
        }

        [TestMethod]
        public void Refresh_WhenNoSourceIsSet_DoesNotChangeStatus()
        {
            var expectedStatus = DataProviderStatus.Uninitialized;

            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();
            var actualStatus = this.provider.Status;

            Assert.AreEqual(expectedStatus, actualStatus);
        }

        [TestMethod]
        public void Refresh_WhenNoSourceIsSet_DoesNotRaiseCompletedEvent()
        {
            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();

            Assert.IsFalse(this.StatuschangedEventWasRaised());
        }

        [TestMethod]
        public void ResultsProperty_WhenProviderHasCompletedRefreshing_HasCorrectGroupsAndItemsData()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order() { Product = "Mouse pad", Quantity = 10, Net = 40 });
            orders.Add(new Order() { Product = "Mouse pad", Quantity = 5, Net = 20 });
            orders.Add(new Order() { Product = "Copy holder", Quantity = 10, Net = 50 });
            orders.Add(new Order() { Product = "Copy holder", Quantity = 5, Net = 25 });
            orders.Add(new Order() { Product = "Copy holder", Quantity = 2, Net = 10 });

            using (this.provider.DeferRefresh())
            {
                this.provider.ItemsSource = orders;
                this.provider.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
            }

            this.provider.BlockUntilRefreshCompletes();
            var results = ((IDataProvider)this.provider).Results;

            var group1 = new Group("Copy holder");
            group1.SetItems(orders.OfType<object>().Skip(2).ToList());
            
            var group2 = new Group("Mouse pad");
            group2.SetItems(orders.OfType<object>().Take(2).ToList());

            List<Group> items = new List<Group>() { group1, group2 };
            Group expectedRowGroup = new Group("Grand Total");
            expectedRowGroup.SetGroups(items);
            
            bool equal = GroupTestsHelper.AreGroupsAndItemsEqual(expectedRowGroup, results.Root.RowGroup);

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void ResultsProperty_WhenProviderHasCompletedRefreshing_HasCorrectGroupsWithCorrectProperties()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order() { Product = "Mouse pad", Promotion = "Promo1", Quantity = 10, Net = 40 });
            orders.Add(new Order() { Product = "Mouse pad", Promotion = "Promo2", Quantity = 5, Net = 20 });
            orders.Add(new Order() { Product = "Copy holder", Promotion = "Promo1", Quantity = 10, Net = 50 });
            orders.Add(new Order() { Product = "Copy holder", Promotion = "Promo2", Quantity = 5, Net = 25 });
            orders.Add(new Order() { Product = "Copy holder", Promotion = "Promo1", Quantity = 2, Net = 10 });

            using (this.provider.DeferRefresh())
            {
                this.provider.ItemsSource = orders;
                this.provider.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Product" });
                this.provider.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Promotion" });
            }

            this.provider.BlockUntilRefreshCompletes();
            var results = ((IDataProvider)this.provider).Results;

            var rootGroup = results.Root.RowGroup;
            Assert.IsTrue(rootGroup.HasItems);
            Assert.IsFalse(rootGroup.IsBottomLevel);

            var group = rootGroup.Items.OfType<IGroup>().ToList()[0];
            Assert.IsTrue(group.HasItems);
            Assert.IsFalse(group.IsBottomLevel);

            var bottomGroup = group.Items.OfType<IGroup>().ToList()[0];
            Assert.IsTrue(bottomGroup.HasItems);
            Assert.IsTrue(bottomGroup.IsBottomLevel);

            bottomGroup = group.Items.OfType<IGroup>().ToList()[1];
            Assert.IsTrue(bottomGroup.HasItems);
            Assert.IsTrue(bottomGroup.IsBottomLevel);

            group = rootGroup.Items.OfType<IGroup>().ToList()[1];
            Assert.IsTrue(group.HasItems);
            Assert.IsFalse(group.IsBottomLevel);

            bottomGroup = group.Items.OfType<IGroup>().ToList()[0];
            Assert.IsTrue(bottomGroup.HasItems);
            Assert.IsTrue(bottomGroup.IsBottomLevel);

            bottomGroup = group.Items.OfType<IGroup>().ToList()[1];
            Assert.IsTrue(bottomGroup.HasItems);
            Assert.IsTrue(bottomGroup.IsBottomLevel);
        }

        [TestMethod]
        public void ResultsProperty_WhenProviderHasCompletedRefreshing_HasCorrectRowGroupsData()
        {
            this.ConfigureProviderToProduceResults();
            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand"),
                
            };

            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();
            var results = ((IDataProvider)this.provider).Results;

            bool equal = GroupTestsHelper.AreGroupsEqual(expectedRowGroup, results.Root.RowGroup);

            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void ResultsProperty_WhenProviderHasCompletedRefreshing_HasCorrectColumnGroupsData()
        {
            this.ConfigureProviderToProduceResults();
            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("1 Free with 10"),
                new TestGroup("Extra Discount"),
            };

            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();
            var results = ((IDataProvider)this.provider).Results;

            bool equal = GroupTestsHelper.AreGroupsEqual(expectedColumnGroup, results.Root.ColumnGroup);
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void ResultsProperty_WhenProviderHasCompletedRefreshing_HasCorrectAggregatesData()
        {
            this.ConfigureProviderToProduceResults();
            var expectedAggregates = new double?[][]
            {
                new double?[] {7851.0, 3949.0, 3902.0},
                new double?[] {1439.0, 792.0, 647.0},
                new double?[] {1708.0, 814.0, 894.0},
                new double?[] {3360.0, 1705.0, 1655.0},
                new double?[] {1344.0, 638.0, 706.0}
            };

            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();
            var results = ((IDataProvider)this.provider).Results;

            bool equal = GroupTestsHelper.AreAggregatesEqual(expectedAggregates, results);
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void ResultsProperty_WhenItemsSourceIsSetToNull_IsWithEmptyRowGroup_And_ColumnGroup()
        {
            this.ConfigureProviderToProduceResults();
            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();

            this.provider.ItemsSource = null;
            var results = ((IDataProvider)this.provider).Results;

            Assert.IsNotNull(results.Root);
            Assert.IsNotNull(results.Root.ColumnGroup);
            Assert.IsNotNull(results.Root.RowGroup);
        }

        [TestMethod]
        public void AddingNewItemToTheSourceCollection_BeforeCallingRefresh_WhenTheNewItemWillIntroduceNewRowGroup_DoesNotIncludeTheNewItemIntoTheResult()
        {
            this.ConfigureProviderToProduceResults();
            var dataSource = this.provider.ItemsSource as IList<Order>;

            dataSource.Add(new Order() { Product = "MyNewProduct" });
            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();
            var results = ((IDataProvider)this.provider).Results;
            var rowGroupNames = results.Root.RowGroup.Items.OfType<IGroup>().Select(rg => (string)rg.Name);

            Assert.IsFalse(rowGroupNames.Contains("MyNewProduct"));
        }

        [TestMethod]
        public void LocalDataSourceProvider_FilterDescriptions()
        {
            LocalDataSourceProvider provider = new LocalDataSourceProvider();
            using (provider.DeferRefresh())
            {
                provider.ItemsSource = Enumerable.Range(0, 10).Select(t => new Tuple<int, int>(t, t)).ToList();
                provider.GroupFactory = new DataGroupFactory();

                provider.AggregateDescriptions.Add(new ListAggregateDescription() { PropertyName = "Item1" });

                provider.FilterDescriptions.Add(new PropertyFilterDescription() { PropertyName = "Item1", Condition = new DelegateCondition((i) => Convert.ToInt32(i) % 3 != 0) });
                provider.FilterDescriptions.Add(new PropertyFilterDescription() { PropertyName = "Item1", Condition = new DelegateCondition((i) => Convert.ToInt32(i) % 5 != 0) });
                provider.FilterDescriptions.Add(new PropertyFilterDescription() { PropertyName = "Item1", Condition = new DelegateCondition((i) => Convert.ToInt32(i) % 7 != 0) });
                // will pass: 1, 2, 4, 8
            }
            
            provider.BlockUntilRefreshCompletes();

            var r = ((IDataProvider)provider).Results;
            IList<int> values = r.GetAggregateResult(0, r.Root).GetValue() as IList<int>;

            Assert.AreEqual(4, values.Count);
            Assert.IsTrue(values.Contains(1));
            Assert.IsTrue(values.Contains(2));
            Assert.IsTrue(values.Contains(4));
            Assert.IsTrue(values.Contains(8));
        }

        [TestMethod]
        public void LocalDataSourceProvider_ThrowException_In_Finalize()
        {
            try
            {
                LocalDataSourceProvider provider = new LocalDataSourceProvider()
                {
                    ItemsSource = Enumerable.Range(0, 10).Select(t => new Tuple<int, int>(t, t)).ToList()
                };

                var description = new PropertyGroupDescription() { PropertyName = "Item1" };
                ((IGroupDescription)description).GroupComparer = new BrokenComparer();
                provider.RowGroupDescriptions.Add(description);

                provider.Refresh();
                provider.BlockUntilRefreshCompletes();

                Assert.AreEqual(DataProviderStatus.Faulted, provider.Status);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [TestMethod]
        public void Setting_ItemsSource_To_Null_Calls_Refresh()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                provider.ItemsSource = new List<Order>();
                provider.BlockUntilRefreshCompletes();
                Assert.IsTrue(this.StatuschangedEventWasRaised());

                this.ClearStatusChangedEvents();
                provider.ItemsSource = null;
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.IsTrue(this.StatuschangedEventWasRaised());
        }

        [TestMethod]
        public void GetState_ShouldReturn_ItemsSource()
        {
            Exception ex = null;
            try
            {
                var source = new List<Order>();
                provider.ItemsSource = source;
                provider.BlockUntilRefreshCompletes();
                Assert.AreEqual(source, provider.State);

                this.ClearStatusChangedEvents();
                provider.ItemsSource = null;
                Assert.IsNull(provider.State);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.IsTrue(this.StatuschangedEventWasRaised());
        }

        [TestMethod]
        public void LocalDataSourceProvider_Clones_Descriptions()
        {
            var engineMock = new PivotEngineMock();
            LocalDataSourceProvider provider = new LocalDataSourceProvider(engineMock);
            using (provider.DeferRefresh())
            {
                provider.AggregateDescriptions.Add(new ListAggregateDescription() { PropertyName = "Item1" });

                provider.FilterDescriptions.Add(new PropertyFilterDescription() { PropertyName = "Item1", Condition = new DelegateCondition((i) => Convert.ToInt32(i) % 3 != 0) });
                provider.SortDescriptions.Add(new PropertySortDescription() { PropertyName = "Item1", SortOrder = SortOrder.Descending });
                provider.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Item2", SortOrder = SortOrder.Descending });
                provider.ItemsSource = Enumerable.Range(0, 10).Select(t => new Tuple<int, int>(t, t)).ToList();
            }

            engineMock.ActionOnRebuildCube = (state) =>
                {
                    Assert.AreNotSame(provider.AggregateDescriptions[0], state.AggregateDescriptions[0]);
                    Assert.AreNotSame(provider.FilterDescriptions[0], state.FilterDescriptions[0]);
                    Assert.AreNotSame(provider.GroupDescriptions[0], state.RowGroupDescriptions[0]);

                    SortFieldComparer fieldComparer = state.ValueProvider.GetSortComparer() as SortFieldComparer;
                    SortFieldComparer.InternalTestHook testHelper = new SortFieldComparer.InternalTestHook(fieldComparer);
                    Assert.AreNotSame(provider.SortDescriptions[0], testHelper.SortDescriptions[0]);
                };

        }

        [TestMethod]
        public void LocalDataSourceProvider_SortDescriptions_NoGrouping_Ascending()
        {
            var data = Enumerable.Range(0, 10000).Select((i) => new DataItem { ID = i, Name = "Name" + i % 500, Surname = "Surname" + (5000 - i) % 1000 }).ToList();

            LocalDataSourceProvider provider = new LocalDataSourceProvider();
            using (provider.DeferRefresh())
            {
                provider.GroupFactory = new DataGroupFactory();
                provider.SortDescriptions.Add(new PropertySortDescription() { PropertyName = "Name", SortOrder = SortOrder.Descending });
                provider.SortDescriptions.Add(new PropertySortDescription() { PropertyName = "ID", SortOrder = SortOrder.Descending });
                provider.ItemsSource = data;
            }
            provider.BlockUntilRefreshCompletes();

            DataItemComparer comparer = new DataItemComparer(new DataItemNameComparer() { Descending = true}, new DataItemIDComparer() { Descending = true});
            data.Sort(comparer);

            for (int i = 0; i < data.Count; i++)
            {
                Assert.AreSame(data[i], provider.Results.Root.RowGroup.Items[i]);
            }
        }

        private class DataItemNameComparer : IComparer<DataItem>
        {
            public bool Descending { get; set; }

            public int Compare(DataItem x, DataItem y)
            {
                int result = x.Name.CompareTo(y.Name);
                if (Descending)
                {
                    result = -result;
                }

                return result;
            }
        }

        private class DataItemIDComparer : IComparer<DataItem>
        {
            public bool Descending { get; set; }

            public int Compare(DataItem x, DataItem y)
            {
                int result = result = x.ID.CompareTo(y.ID);
                if (Descending)
                {
                    result = -result;
                }

                return result;
            }
        }

        private class DataItemComparer : IComparer<DataItem>
        {
            public DataItemComparer(params IComparer<DataItem>[] comparers)
            {
                this.Comparers = new List<IComparer<DataItem>>(comparers);
            }
            public IList<IComparer<DataItem>> Comparers { get; private set; }

            public int Compare(DataItem x, DataItem y)
            {
                int result = 0;
                foreach (var comparer in this.Comparers)
                {
                    result = comparer.Compare(x, y);
                    if (result != 0)
                    {
                        break;
                    }
                }

                return result;
            }
        }
    }
}