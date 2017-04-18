using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Windows.Foundation.Collections;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public partial class LocalDataSourceProviderTests
    {
        private ObservableCollection<Item> items = new ObservableCollection<Item>();

        [TestMethod]
        public void AddItemsOnEmptyCollection()
        {
            this.SetSourceAndBlock();
            this.AddItem();
            Assert.AreEqual(1, provider.Results.Root.RowGroup.Items.Count);
        }

        [TestMethod]
        public void InsertedItemsAreInCorrectIndex()
        {
            this.SetSourceAndBlock();

            var item1 = new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 };
            var item2 = new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 };
            var item3 = new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 };

            this.items.Add(item1);
            this.items.Add(item2);
            this.items.Insert(0, item3);

            Assert.AreEqual(3, provider.Results.Root.RowGroup.Items.Count);
            Assert.AreSame(item3, provider.Results.Root.RowGroup.Items[0]);
            Assert.AreSame(item1, provider.Results.Root.RowGroup.Items[1]);
            Assert.AreSame(item2, provider.Results.Root.RowGroup.Items[2]);
        }

        [TestMethod]
        public void RemovedItemsAreInCorrectIndex()
        {
            this.SetSourceAndBlock();

            var item1 = new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 };
            var item2 = new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 };
            var item3 = new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 };

            this.items.Add(item1);
            this.items.Add(item2);
            this.items.Add(item3);

            this.items.RemoveAt(1);

            Assert.AreEqual(2, provider.Results.Root.RowGroup.Items.Count);
            Assert.AreSame(item1, provider.Results.Root.RowGroup.Items[0]);
            Assert.AreSame(item3, provider.Results.Root.RowGroup.Items[1]);
        }

        [TestMethod]
        public void RemoveItemsToEmptyCollection()
        {
            this.AddItem();
            this.SetSourceAndBlock();
            this.items.RemoveAt(0);
            Assert.AreEqual(0, provider.Results.Root.RowGroup.Items.Count);
        }

        [TestMethod]
        public void ChangeItem()
        {
            this.GenerateSetSourceAndBlock();
            this.AssertViewChanged(() => { this.items[0].Id = 10; });
        }

        [TestMethod]
        public void ClearItems()
        {
            this.SetSourceAndBlock();
            this.items.Clear();
            Assert.AreEqual(0, provider.Results.Root.RowGroup.Items.Count);
        }

        [TestMethod]
        public void AggregateDescriptionsAreOkWhenNoGrouping()
        {
            this.SetupAggregates();
            Coordinate root = this.provider.Results.Root;

            var aggregate = this.provider.Results.GetAggregateResult(0, root);
            Assert.AreEqual(300d, aggregate.GetValue());

            aggregate = this.provider.Results.GetAggregateResult(1, root);
            Assert.AreEqual(15d, aggregate.GetValue());
        }

        [TestMethod]
        public void AggregateDescriptionsAreCorrectWhenItemIsRemovedNoGrouping()
        {
            this.SetupAggregates();
            Coordinate root = this.provider.Results.Root;

            this.items.RemoveAt(0);

            var aggregate = this.provider.Results.GetAggregateResult(0, root);
            Assert.AreEqual(280d, aggregate.GetValue());
        }

        [TestMethod]
        public void AggregateDescriptionsAreCorrectWhenItemIsAddedNoGrouping()
        {
            this.SetupAggregates();
            Coordinate root = this.provider.Results.Root;

            this.AddItem();

            var aggregate = this.provider.Results.GetAggregateResult(0, root);
            Assert.AreEqual(320d, aggregate.GetValue());
        }

        [TestMethod]
        public void AggregateDescriptionsAreCorrectWhenItemIsChangedNoGrouping()
        {
            this.SetupAggregates();
            Coordinate root = this.provider.Results.Root;

            this.items[0].Value = 40;

            var aggregate = this.provider.Results.GetAggregateResult(0, root);
            Assert.AreEqual(320d, aggregate.GetValue());
        }

        [TestMethod]
        public void AggregateDescriptionsAreCorrectWhenCollectionResetNoGrouping()
        {
            this.SetupAggregates();
            this.items.Clear();

            Coordinate root = this.provider.Results.Root;
            var aggregate = this.provider.Results.GetAggregateResult(0, root);
            Assert.IsNull(aggregate);
        }

        private void SetupAggregates()
        {
            this.provider.DeferUpdates = true;
            this.GenerateSetSourceAndBlock();
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Value", AggregateFunction = new SumAggregateFunction() });
            this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Id", AggregateFunction = new SumAggregateFunction() });
            this.provider.DeferUpdates = false;
            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();
        }

        // With Sort descriptions
        // - Add/Remove/Reset/PropertyChanged items - check for item count, item index

        [TestMethod]
        public void AddItemsWhenSorted()
        {
            this.SetupSortedProvider();

            Assert.AreEqual(5, provider.Results.Root.RowGroup.Items.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreSame(this.items[i], provider.Results.Root.RowGroup.Items[4 - i]);
            }
        }

        [TestMethod]
        public void InsertedItemsAreInCorrectIndexWhenSorted()
        {
            this.SetupSortedProvider();

            var item1 = new Item() { Id = 0, Value = 20 };
            var item2 = new Item() { Id = 1, Value = 20 };
            var item3 = new Item() { Id = 8, Value = 20 };

            this.items.Add(item1);
            this.items.Add(item2);
            this.items.Insert(0, item3);

            Assert.AreEqual(8, provider.Results.Root.RowGroup.Items.Count);
            Assert.AreSame(item1, provider.Results.Root.RowGroup.Items[7]);
            Assert.AreSame(item2, provider.Results.Root.RowGroup.Items[5]);
            Assert.AreSame(item3, provider.Results.Root.RowGroup.Items[0]);
        }

        [TestMethod]
        public void RemovedItemsAreInCorrectIndexWhenSorted()
        {
            this.SetupSortedProvider();

            var item1 = this.items[0];
            var item2 = this.items[2];

            this.items.Remove(item1);
            this.items.Remove(item2);

            Assert.AreEqual(3, provider.Results.Root.RowGroup.Items.Count);
            Assert.AreSame(this.items[0], provider.Results.Root.RowGroup.Items[2]);
            Assert.AreSame(this.items[1], provider.Results.Root.RowGroup.Items[1]);
            Assert.AreSame(this.items[2], provider.Results.Root.RowGroup.Items[0]);
        }

        [TestMethod]
        public void ChangeItemWhenSorted()
        {
            this.SetupSortedProvider();
            this.AssertViewChanged(() => { this.items[0].Id = 10; });
            Assert.AreSame(this.items[0], provider.Results.Root.RowGroup.Items[0]);
        }

        // With Filter descriptions
        // - Add/Remove/Reset/PropertyChanged items - check for item count, item index

        [TestMethod]
        public void AddItemsWhenFiltered()
        {
            this.SetupFilteredProvider();

            Assert.AreEqual(3, provider.Results.Root.RowGroup.Items.Count);
            int localIndex = 0;
            for (int i = 2; i < 5; i++)
            {
                Assert.AreSame(this.items[i], provider.Results.Root.RowGroup.Items[localIndex++]);
            }
        }

        [TestMethod]
        public void InsertedItemsAreInCorrectIndexWhenFiltered()
        {
            this.SetupFilteredProvider();

            var item1 = new Item() { Id = 0, Value = 20 };
            var item2 = new Item() { Id = 1, Value = 20 };
            var item3 = new Item() { Id = 8, Value = 20 };

            this.items.Add(item1);
            this.items.Add(item2);
            this.items.Insert(0, item3);

            Assert.AreEqual(4, provider.Results.Root.RowGroup.Items.Count);
            Assert.IsFalse(provider.Results.Root.RowGroup.Items.Contains(item1));
            Assert.IsFalse(provider.Results.Root.RowGroup.Items.Contains(item2));
            Assert.AreSame(item3, provider.Results.Root.RowGroup.Items[0]);
        }

        [TestMethod]
        public void RemovedItemsAreInCorrectIndexWhenFiltered()
        {
            this.SetupFilteredProvider();

            Assert.AreEqual(3, provider.Results.Root.RowGroup.Items.Count);

            var item1 = this.items[0];
            Assert.IsFalse(provider.Results.Root.RowGroup.Items.Contains(item1));

            this.items.Remove(item1);
            Assert.AreEqual(3, provider.Results.Root.RowGroup.Items.Count);

            var item2 = this.items[2];
            Assert.IsTrue(provider.Results.Root.RowGroup.Items.Contains(item2));

            this.items.Remove(item2);
            Assert.IsFalse(provider.Results.Root.RowGroup.Items.Contains(item2));

            Assert.AreEqual(2, provider.Results.Root.RowGroup.Items.Count);

            Assert.AreSame(this.items[1], provider.Results.Root.RowGroup.Items[0]);
            Assert.AreSame(this.items[2], provider.Results.Root.RowGroup.Items[1]);
        }

        [TestMethod]
        public void ChangeItemWhenFiltered()
        {
            this.SetupFilteredProvider();
            this.AssertViewChanged(() => { this.items[0].Id = 10; });
            Assert.AreEqual(4, provider.Results.Root.RowGroup.Items.Count);
            Assert.AreSame(this.items[0], provider.Results.Root.RowGroup.Items[0]);
        }

        // With Sort & Filter & Group & Aggregate descriptions
        // - Add/Remove/Reset/PropertyChanged items - check for item count, item index, aggregates & summaries

        // FILTERED: Group0           (Sum of Value) = 100
        // FILTERED: - Inner Group 1  (Sum of Value) = 100
        // FILTERED:   - this.items[0] (value=100, Id=0)
        // Group1           (Sum of Value) = 120
        // - Inner Group 1  (Sum of Value) = 60
        // FILTERED: - this.items[1] (value=20, Id=1)
        // FILTERED: - this.items[2] (value=40, Id=2)
        //   -  this.items[3]  (value=20, Id=3)
        //   -  this.items[4]  (value=40, Id=4)
        // - Inner Group 2  (Sum of Value) = 60
        //   -  this.items[5]  (value=40, Id=5)
        // Group2           (Sum of Value) = 80
        // - Inner Group 1  (Sum of Value) = 80
        //   -  this.items[6]  (value=80, Id=6)
        // FILTERED: Group3           (Sum of Value) = 20
        // FILTERED: - Inner Group 1  (Sum of Value) = 20
        // FILTERED:   - this.items[7] (value=20, Id=1)

        [TestMethod]
        public void InitialStateIsCorrect()
        {
            this.SetupAllDescriptorsAndSetSource();
            this.CheckInitialResult();
        }

        [TestMethod]
        public void InsertedItemsAreInCorrectIndexAllDescriptions()
        {
            this.SetupAllDescriptorsAndSetSource();

            var item1 = new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 1, Value = 20 };
            this.items.Add(item1);
            // Item should be filtered so result should be the same.
            this.CheckInitialResult();

            var item2 = new Item() { Group = "Group2", Group2 = "Inner Group2", Id = 10, Value = 20 };
            this.items.Add(item2);

            var item3 = new Item() { Group = "Group2", Group2 = "Inner Group2", Id = 11, Value = 20 };
            this.items.Add(item3);

            Assert.AreEqual(2, provider.Results.Root.RowGroup.Items.Count);

            Coordinate root = this.provider.Results.Root;
            var rowRootGroup = root.RowGroup;
            var columnRootGroup = root.ColumnGroup;
            var aggregate = this.provider.Results.GetAggregateResult(0, rowRootGroup, columnRootGroup);
            Assert.AreEqual(240d, aggregate.GetValue());

            IGroup group1 = provider.Results.Root.RowGroup.Items[0] as IGroup;
            Assert.AreEqual("Group1", group1.Name);
            Assert.AreEqual(2, group1.Items.Count);

            aggregate = this.provider.Results.GetAggregateResult(0, group1, columnRootGroup);
            Assert.AreEqual(120d, aggregate.GetValue());

            IGroup innerG1 = group1.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(2, innerG1.Items.Count);
            Assert.IsTrue(innerG1.Items.Contains(this.items[3]));
            Assert.IsTrue(innerG1.Items.Contains(this.items[4]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(60d, aggregate.GetValue());

            IGroup innerG2 = group1.Items[1] as IGroup;
            Assert.AreEqual("Inner Group2", innerG2.Name);
            Assert.AreEqual(1, innerG2.Items.Count);
            Assert.IsTrue(innerG2.Items.Contains(this.items[5]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG2, columnRootGroup);
            Assert.AreEqual(60d, aggregate.GetValue());

            IGroup group2 = provider.Results.Root.RowGroup.Items[1] as IGroup;
            Assert.AreEqual("Group2", group2.Name);
            Assert.AreEqual(2, group2.Items.Count);
            aggregate = this.provider.Results.GetAggregateResult(0, group2, columnRootGroup);
            Assert.AreEqual(120d, aggregate.GetValue());

            innerG1 = group2.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(1, innerG1.Items.Count);
            Assert.IsTrue(innerG1.Items.Contains(this.items[6]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());

            innerG2 = group2.Items[1] as IGroup;
            Assert.AreEqual("Inner Group2", innerG2.Name);
            Assert.AreEqual(2, innerG2.Items.Count);
            Assert.AreSame(item3, innerG2.Items[0]);
            Assert.AreSame(item2, innerG2.Items[1]);
            aggregate = this.provider.Results.GetAggregateResult(0, innerG2, columnRootGroup);
            Assert.AreEqual(40d, aggregate.GetValue());
        }

        [TestMethod]
        public void RemovedItemsAreInCorrectIndexAllDescriptions()
        {
            this.SetupAllDescriptorsAndSetSource();

            // Remove of filtered item should not change anything.
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 1, Value = 20 });
            this.CheckInitialResult();

            this.items.Remove(this.items[7]);
            this.CheckInitialResult();

            this.items.Remove(this.items[6]);
            var removedItem = this.items[4];
            this.items.Remove(removedItem);

            Assert.AreEqual(1, provider.Results.Root.RowGroup.Items.Count);

            Coordinate root = this.provider.Results.Root;
            var rowRootGroup = root.RowGroup;
            var columnRootGroup = root.ColumnGroup;
            var aggregate = this.provider.Results.GetAggregateResult(0, rowRootGroup, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());

            IGroup group1 = provider.Results.Root.RowGroup.Items[0] as IGroup;
            Assert.AreEqual("Group1", group1.Name);
            Assert.AreEqual(2, group1.Items.Count);

            aggregate = this.provider.Results.GetAggregateResult(0, group1, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());

            IGroup innerG1 = group1.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(1, innerG1.Items.Count);
            Assert.IsFalse(innerG1.Items.Contains(removedItem));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(20d, aggregate.GetValue());

            IGroup innerG2 = group1.Items[1] as IGroup;
            Assert.AreEqual("Inner Group2", innerG2.Name);
            Assert.AreEqual(1, innerG2.Items.Count);
            Assert.IsTrue(innerG2.Items.Contains(this.items[4]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG2, columnRootGroup);
            Assert.AreEqual(60d, aggregate.GetValue());
        }

        [TestMethod]
        public void ChangeItemFilteredProperty()
        {
            this.SetupAllDescriptorsAndSetSource();
            // Changing item does not pass filter.
            this.AssertViewChanged(() => { this.items[0].Id = 1; });
            this.CheckInitialResult();

            this.AssertViewChanged(() => { this.items[0].Id = 10; this.items[1].Id = 10; });

            Assert.AreEqual(3, provider.Results.Root.RowGroup.Items.Count);

            Coordinate root = this.provider.Results.Root;
            var rowRootGroup = root.RowGroup;
            var columnRootGroup = root.ColumnGroup;
            var aggregate = this.provider.Results.GetAggregateResult(0, rowRootGroup, columnRootGroup);
            Assert.AreEqual(320d, aggregate.GetValue());

            IGroup group0 = provider.Results.Root.RowGroup.Items[0] as IGroup;
            Assert.AreEqual("Group0", group0.Name);
            Assert.AreEqual(1, group0.Items.Count);

            aggregate = this.provider.Results.GetAggregateResult(0, group0, columnRootGroup);
            Assert.AreEqual(100d, aggregate.GetValue());

            IGroup innerG0 = group0.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG0.Name);
            Assert.AreEqual(1, innerG0.Items.Count);
            Assert.IsTrue(innerG0.Items.Contains(this.items[0]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG0, columnRootGroup);
            Assert.AreEqual(100d, aggregate.GetValue());

            IGroup group1 = provider.Results.Root.RowGroup.Items[1] as IGroup;
            Assert.AreEqual("Group1", group1.Name);
            Assert.AreEqual(2, group1.Items.Count);

            aggregate = this.provider.Results.GetAggregateResult(0, group1, columnRootGroup);
            Assert.AreEqual(140d, aggregate.GetValue());

            IGroup innerG1 = group1.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(3, innerG1.Items.Count);
            Assert.IsTrue(innerG1.Items.Contains(this.items[1]));
            Assert.IsTrue(innerG1.Items.Contains(this.items[3]));
            Assert.IsTrue(innerG1.Items.Contains(this.items[4]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());

            IGroup innerG2 = group1.Items[1] as IGroup;
            Assert.AreEqual("Inner Group2", innerG2.Name);
            Assert.AreEqual(1, innerG2.Items.Count);
            Assert.IsTrue(innerG2.Items.Contains(this.items[5]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG2, columnRootGroup);
            Assert.AreEqual(60d, aggregate.GetValue());

            IGroup group2 = provider.Results.Root.RowGroup.Items[2] as IGroup;
            Assert.AreEqual("Group2", group2.Name);
            Assert.AreEqual(1, group2.Items.Count);
            aggregate = this.provider.Results.GetAggregateResult(0, group2, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());

            innerG1 = group2.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(1, innerG1.Items.Count);
            Assert.IsTrue(innerG1.Items.Contains(this.items[6]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());
        }

        [TestMethod]
        public void ChangeItemSortedProperty()
        {
            this.SetupAllDescriptorsAndSetSource();
            // Changing sort propeprty of item that does not pass filter.
            this.AssertViewChanged(() => { this.items[0].Id = 1; });
            this.CheckInitialResult();

            this.AssertViewChanged(() => { this.items[4].Id = 3; });
            this.CheckInitialResult();

            this.items[3].Id = 4;

            IGroup group1 = provider.Results.Root.RowGroup.Items[0] as IGroup;
            IGroup innerG1 = group1.Items[0] as IGroup;

            Assert.AreSame(this.items[3], innerG1.Items[0]);
            Assert.AreSame(this.items[4], innerG1.Items[1]);
        }

        [TestMethod]
        public void ChangeItemGroupedProperty()
        {
            this.SetupAllDescriptorsAndSetSource();
            // Changing group propeprty of item that does not pass filter.
            this.AssertViewChanged(() => { this.items[0].Group = "Group8"; });
            this.CheckInitialResult();

            this.AssertViewChanged(() => { this.items[6].Group = "Group1"; });

            Assert.AreEqual(1, provider.Results.Root.RowGroup.Items.Count);

            Coordinate root = this.provider.Results.Root;
            var rowRootGroup = root.RowGroup;
            var columnRootGroup = root.ColumnGroup;
            var aggregate = this.provider.Results.GetAggregateResult(0, rowRootGroup, columnRootGroup);
            Assert.AreEqual(200d, aggregate.GetValue());

            IGroup group1 = provider.Results.Root.RowGroup.Items[0] as IGroup;
            Assert.AreEqual("Group1", group1.Name);
            Assert.AreEqual(2, group1.Items.Count);

            aggregate = this.provider.Results.GetAggregateResult(0, group1, columnRootGroup);
            Assert.AreEqual(200d, aggregate.GetValue());

            IGroup innerG1 = group1.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(3, innerG1.Items.Count);
            Assert.AreSame(this.items[6], innerG1.Items[0]);
            Assert.AreSame(this.items[4], innerG1.Items[1]);
            Assert.AreSame(this.items[3], innerG1.Items[2]);
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(140d, aggregate.GetValue());

            IGroup innerG2 = group1.Items[1] as IGroup;
            Assert.AreEqual("Inner Group2", innerG2.Name);
            Assert.AreEqual(1, innerG2.Items.Count);
            Assert.IsTrue(innerG2.Items.Contains(this.items[5]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG2, columnRootGroup);
            Assert.AreEqual(60d, aggregate.GetValue());
        }

        [TestMethod]
        public void ClearItemsAllDescriptions()
        {
            this.SetupAllDescriptorsAndSetSource();
            this.items.Clear();
            Assert.AreEqual(0, provider.Results.Root.RowGroup.Items.Count);
        }

        // FILTERED: Group0           (Sum of Value) = 100
        // FILTERED: - Inner Group 1  (Sum of Value) = 100
        // FILTERED:   - this.items[0] (value=100, Id=0)
        // Group1           (Sum of Value) = 120
        // - Inner Group 1  (Sum of Value) = 60
        // FILTERED: - this.items[1] (value=20, Id=1)
        // FILTERED: - this.items[2] (value=40, Id=2)
        //   -  this.items[3]  (value=20, Id=3)
        //   -  this.items[4]  (value=40, Id=4)
        // - Inner Group 2  (Sum of Value) = 60
        //   -  this.items[5]  (value=40, Id=5)
        // Group2           (Sum of Value) = 80
        // - Inner Group 1  (Sum of Value) = 80
        //   -  this.items[6]  (value=80, Id=6)
        // FILTERED: Group3           (Sum of Value) = 20
        // FILTERED: - Inner Group 1  (Sum of Value) = 20
        // FILTERED:   - this.items[7] (value=20, Id=1)

        [TestMethod]
        public void CompactLayout_AddItems()
        {
            this.SetupAllDescriptorsAndSetSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
                {
                    var change = e.Changes[0];
                    changeAction = e.Action;
                    changedItem = change.ChangedCoordinate.RowGroup;
                    addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                    index = change.AddRemoveRowGroupIndex;
                };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(9, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            this.items.Add(new Item() { Group = "Group2", Group2 = "Inner Group1", Id = 3, Value = 80 });
            Assert.AreEqual(CollectionChange.ItemInserted, changeAction);

            layout.AddItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            IGroup root = this.provider.Results.Root.RowGroup;
            IGroup g2 = root.Items[1] as IGroup;
            IGroup ig1 = g2.Items[0] as IGroup;
            Assert.AreEqual(ig1, changedItem);
            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(1, index);
            Assert.AreEqual(10, layout.VisibleLineCount);
            Assert.AreEqual(10, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayout_AddItemsInCollapsedGroup()
        {
            this.SetupAllDescriptorsAndSetSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(9, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            IGroup root = this.provider.Results.Root.RowGroup;
            IGroup g2 = root.Items[1] as IGroup;
            IGroup ig1 = g2.Items[0] as IGroup;

            layout.Collapse(ig1);

            Assert.AreEqual(8, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            this.items.Add(new Item() { Group = "Group2", Group2 = "Inner Group1", Id = 3, Value = 80 });
            Assert.AreEqual(CollectionChange.ItemInserted, changeAction);

            layout.AddItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            Assert.AreEqual(ig1, changedItem);
            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(1, index);
            Assert.AreEqual(8, layout.VisibleLineCount);
            Assert.AreEqual(10, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayoutWithGroupedSource_AddItems_AddNewGroup()
        {
            this.SetupAllDescriptorsAndSetSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            layout.SetSource(this.provider.Results.Root.RowGroup.Items, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(9, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            this.items.Add(new Item() { Group = "Group3", Group2 = "Inner Group1", Id = 3, Value = 80 });
            Assert.AreEqual(CollectionChange.ItemInserted, changeAction);

            layout.AddItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            IGroup root = this.provider.Results.Root.RowGroup;
            IGroup g3 = root.Items[2] as IGroup;

            Assert.AreEqual(root, changedItem);
            Assert.AreEqual(g3, addRemoveItem);

            Assert.AreEqual(2, index);
            Assert.AreEqual(12, layout.VisibleLineCount);
            Assert.AreEqual(12, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayoutWithGroupedSource_RemoveItems()
        {
            this.SetupAllDescriptorsAndSetSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(9, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            this.items.RemoveAt(3);
            Assert.AreEqual(CollectionChange.ItemRemoved, changeAction);

            layout.RemoveItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            IGroup root = this.provider.Results.Root.RowGroup;
            IGroup g1 = root.Items[0] as IGroup;
            IGroup ig1 = g1.Items[0] as IGroup;
            Assert.AreEqual(ig1, changedItem);
            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(1, index);
            Assert.AreEqual(8, layout.VisibleLineCount);
            Assert.AreEqual(8, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayout_RemoveItemsInCollapsedGroup()
        {
            this.SetupAllDescriptorsAndSetSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(9, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            IGroup root = this.provider.Results.Root.RowGroup;
            IGroup g1 = root.Items[0] as IGroup;
            IGroup ig1 = g1.Items[0] as IGroup;

            layout.Collapse(ig1);

            Assert.AreEqual(7, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            this.items.RemoveAt(3);
            this.provider.ViewChanged -= handler;

            layout.RemoveItem(changedItem, addRemoveItem, index);

            Assert.AreEqual(CollectionChange.ItemRemoved, changeAction);
            Assert.AreEqual(ig1, changedItem);
            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(1, index);
            Assert.AreEqual(7, layout.VisibleLineCount);
            Assert.AreEqual(8, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayout_RemoveItems_RemoveEmptyGroup()
        {
            this.SetupAllDescriptorsAndSetSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            layout.SetSource(this.provider.Results.Root.RowGroup.Items, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(9, layout.VisibleLineCount);
            Assert.AreEqual(9, layout.TotalLineCount);

            IGroup root = this.provider.Results.Root.RowGroup;
            IGroup g2 = root.Items[1] as IGroup;

            // Initialize renderInfo;
            var ri = layout.RenderInfo;
            this.items.RemoveAt(6);
            layout.RemoveItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            Assert.AreEqual(CollectionChange.ItemRemoved, changeAction);
            Assert.AreEqual(root, changedItem);
            Assert.AreEqual(g2, addRemoveItem);

            Assert.AreEqual(1, index);
            Assert.AreEqual(6, layout.VisibleLineCount);
            Assert.AreEqual(6, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayoutWithFlatSource_WhenFilterIsApplied_AddItems()
        {
            this.SetupAndSetFlatSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(4, layout.VisibleLineCount);
            Assert.AreEqual(4, layout.TotalLineCount);

            this.items.Add(new Item() { Group = "Group2", Group2 = "Inner Group1", Id = 3, Value = 80 });
            Assert.AreEqual(CollectionChange.ItemInserted, changeAction);

            layout.AddItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(3, index);
            Assert.AreEqual(5, layout.VisibleLineCount);
            Assert.AreEqual(5, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayoutWithFlatSource_NoFilter_AddItems()
        {
            this.SetupAndSetFlatSource(false);

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(7, layout.VisibleLineCount);
            Assert.AreEqual(7, layout.TotalLineCount);

            this.items.Add(new Item() { Group = "Group2", Group2 = "Inner Group1", Id = 3, Value = 80 });
            Assert.AreEqual(CollectionChange.ItemInserted, changeAction);

            layout.AddItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(3, index);
            Assert.AreEqual(8, layout.VisibleLineCount);
            Assert.AreEqual(8, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayoutWithFlatSource_WhenFilterIsApplied_RemoveItems()
        {
            this.SetupAndSetFlatSource();

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(4, layout.VisibleLineCount);
            Assert.AreEqual(4, layout.TotalLineCount);

            this.items.RemoveAt(3);
            Assert.AreEqual(CollectionChange.ItemRemoved, changeAction);

            layout.RemoveItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(3, index);
            Assert.AreEqual(3, layout.VisibleLineCount);
            Assert.AreEqual(3, layout.TotalLineCount);
        }

        [TestMethod]
        public void CompactLayoutWithFlatSource_NoFilter_RemoveItems()
        {
            this.SetupAndSetFlatSource(false);

            CompactLayout layout = new CompactLayout(new GroupHierarchyAdapter(), 40);
            var source = this.provider.Results.Root.RowGroup.Items;
            layout.SetSource(source, this.provider.RowGroupDescriptions.Count, TotalsPosition.None, -1, 0, false);

            object changedItem = null;
            object addRemoveItem = null;
            int index = -1;
            CollectionChange changeAction = CollectionChange.Reset;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) =>
            {
                var change = e.Changes[0];
                changeAction = e.Action;
                changedItem = change.ChangedCoordinate.RowGroup;
                addRemoveItem = change.AddedOrRemovedCoordinate.RowGroup;
                index = change.AddRemoveRowGroupIndex;
            };
            this.provider.ViewChanged += handler;

            Assert.AreEqual(7, layout.VisibleLineCount);
            Assert.AreEqual(7, layout.TotalLineCount);

            this.items.RemoveAt(3);
            Assert.AreEqual(CollectionChange.ItemRemoved, changeAction);

            layout.RemoveItem(changedItem, addRemoveItem, index);
            this.provider.ViewChanged -= handler;

            Assert.IsNull(addRemoveItem);
            Assert.AreEqual(3, index);
            Assert.AreEqual(6, layout.VisibleLineCount);
            Assert.AreEqual(6, layout.TotalLineCount);
        }

        private void CheckInitialResult()
        {
            Assert.AreEqual(2, provider.Results.Root.RowGroup.Items.Count);

            Coordinate root = this.provider.Results.Root;
            var rowRootGroup = root.RowGroup;
            var columnRootGroup = root.ColumnGroup;
            var aggregate = this.provider.Results.GetAggregateResult(0, rowRootGroup, columnRootGroup);
            Assert.AreEqual(200d, aggregate.GetValue());

            IGroup group1 = provider.Results.Root.RowGroup.Items[0] as IGroup;
            Assert.AreEqual("Group1", group1.Name);
            Assert.AreEqual(2, group1.Items.Count);

            aggregate = this.provider.Results.GetAggregateResult(0, group1, columnRootGroup);
            Assert.AreEqual(120d, aggregate.GetValue());

            IGroup innerG1 = group1.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(2, innerG1.Items.Count);
            Assert.AreSame(this.items[4], innerG1.Items[0]);
            Assert.AreSame(this.items[3], innerG1.Items[1]);
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(60d, aggregate.GetValue());

            IGroup innerG2 = group1.Items[1] as IGroup;
            Assert.AreEqual("Inner Group2", innerG2.Name);
            Assert.AreEqual(1, innerG2.Items.Count);
            Assert.IsTrue(innerG2.Items.Contains(this.items[5]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG2, columnRootGroup);
            Assert.AreEqual(60d, aggregate.GetValue());

            IGroup group2 = provider.Results.Root.RowGroup.Items[1] as IGroup;
            Assert.AreEqual("Group2", group2.Name);
            Assert.AreEqual(1, group2.Items.Count);
            aggregate = this.provider.Results.GetAggregateResult(0, group2, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());

            innerG1 = group2.Items[0] as IGroup;
            Assert.AreEqual("Inner Group1", innerG1.Name);
            Assert.AreEqual(1, innerG1.Items.Count);
            Assert.IsTrue(innerG1.Items.Contains(this.items[6]));
            aggregate = this.provider.Results.GetAggregateResult(0, innerG1, columnRootGroup);
            Assert.AreEqual(80d, aggregate.GetValue());
        }

        private void AddItem()
        {
            this.items.Add(new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 });
        }

        private void SetSourceAndBlock()
        {
            using (this.provider.DeferRefresh())
            {
                this.provider.ItemsSource = items;
                this.provider.Refresh();
            }
            this.provider.BlockUntilRefreshCompletes();
        }

        private void SetupSortedProvider()
        {
            this.GenerateSourceCollection();
            using (this.provider.DeferRefresh())
            {
                this.provider.SortDescriptions.Add(new PropertySortDescription() { PropertyName = "Id", SortOrder = SortOrder.Descending });
                this.provider.ItemsSource = items;
                this.provider.Refresh();
            }
            this.provider.BlockUntilRefreshCompletes();
        }

        private void SetupFilteredProvider()
        {
            this.GenerateSourceCollection();
            using (this.provider.DeferRefresh())
            {
                this.provider.FilterDescriptions.Add(new PropertyFilterDescription() { PropertyName = "Id", Condition = new DelegateFilterCondition() { DelegateFilter = (item) => { return ((int)item) > 2; } } });
                this.provider.ItemsSource = items;
                this.provider.Refresh();
            }
            this.provider.BlockUntilRefreshCompletes();
        }

        private void SetupAllDescriptorsAndSetSource()
        {
            // Filtered.
            this.items.Add(new Item() { Group = "Group0", Group2 = "Inner Group1", Id = 0, Value = 100 });

            // Filtered.
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 1, Value = 20 });
            // Filtered.
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 2, Value = 40 });

            // Not filtered.
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 3, Value = 20 });
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 4, Value = 40 });
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group2", Id = 5, Value = 60 });
            this.items.Add(new Item() { Group = "Group2", Group2 = "Inner Group1", Id = 6, Value = 80 });

            using (this.provider.DeferRefresh())
            {
                this.provider.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Group" });
                this.provider.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = "Group2" });
                this.provider.SortDescriptions.Add(new PropertySortDescription() { PropertyName = "Id", SortOrder = SortOrder.Descending });
                this.provider.AggregateDescriptions.Add(new PropertyAggregateDescription() { PropertyName = "Value", AggregateFunction = new SumAggregateFunction() });
                this.provider.FilterDescriptions.Add(new PropertyFilterDescription() { PropertyName = "Id", Condition = new DelegateFilterCondition() { DelegateFilter = (item) => { return ((int)item) > 2; } } });
                this.provider.ItemsSource = items;
                this.provider.Refresh();
            }

            this.provider.BlockUntilRefreshCompletes();
        }

        private void SetupAndSetFlatSource(bool filter = true)
        {
            // Filtered.
            this.items.Add(new Item() { Group = "Group0", Group2 = "Inner Group1", Id = 0, Value = 100 });

            // Filtered.
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 1, Value = 20 });
            // Filtered.
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 2, Value = 40 });

            // Not filtered.
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 3, Value = 20 });
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group1", Id = 4, Value = 40 });
            this.items.Add(new Item() { Group = "Group1", Group2 = "Inner Group2", Id = 5, Value = 60 });
            this.items.Add(new Item() { Group = "Group2", Group2 = "Inner Group1", Id = 6, Value = 80 });

            using (this.provider.DeferRefresh())
            {
                this.provider.SortDescriptions.Add(new PropertySortDescription() { PropertyName = "Id", SortOrder = SortOrder.Descending });

                if (filter)
                {
                    this.provider.FilterDescriptions.Add(new PropertyFilterDescription() { PropertyName = "Id", Condition = new DelegateFilterCondition() { DelegateFilter = (item) => { return ((int)item) > 2; } } });
                }

                this.provider.ItemsSource = items;
                this.provider.Refresh();
            }

            this.provider.BlockUntilRefreshCompletes();
        }

        private void AssertViewChanged(Action action, bool fired = true)
        {
            bool eventFired = false;
            EventHandler<ViewChangedEventArgs> handler = (object sender, ViewChangedEventArgs e) => eventFired = true;
            this.provider.ViewChanged += handler;
            action();
            this.provider.ViewChanged -= handler;
            Assert.AreEqual(fired, eventFired);
        }

        private void GenerateSetSourceAndBlock()
        {
            this.GenerateSourceCollection();
            using (this.provider.DeferRefresh())
            {
                this.provider.ItemsSource = items;
            }
            this.provider.BlockUntilRefreshCompletes();
        }

        private void GenerateSourceCollection()
        {
            this.items.Add(new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 1, Value = 20 });
            this.items.Add(new Item() { Group = "Group 1", Group2 = "Inner Group1", Id = 2, Value = 40 });
            this.items.Add(new Item() { Group = "Group 1", Group2 = "Inner Group2", Id = 3, Value = 60 });
            this.items.Add(new Item() { Group = "Group 2", Group2 = "Inner Group1", Id = 4, Value = 80 });
            this.items.Add(new Item() { Group = "Group 3", Group2 = "Inner Group1", Id = 5, Value = 100 });
        }

        private class Item : INotifyPropertyChanged
        {
            private double value;
            public double Value
            {
                get { return value; }
                set
                {
                    if (this.value != value)
                    {
                        this.value = value;
                        this.NotifyPropertyChanged();
                    }
                }
            }

            private string group;
            public string Group
            {
                get { return group; }
                set
                {
                    if (this.group != value)
                    {
                        this.group = value;
                        this.NotifyPropertyChanged();
                    }
                }
            }

            private int id;
            public int Id
            {
                get { return id; }
                set
                {
                    if (this.id != value)
                    {
                        this.id = value;
                        this.NotifyPropertyChanged();
                    }
                }
            }

            private string group2;
            public string Group2
            {
                get { return group2; }
                set
                {
                    if (this.group2 != value)
                    {
                        this.group2 = value;
                        this.NotifyPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private class DelegateFilterCondition : Condition
        {
            public Func<object, bool> DelegateFilter { get; set; }
            public override bool PassesFilter(object item)
            {
                return this.DelegateFilter(item);
            }

            protected override void CloneCore(Cloneable source)
            {
                this.DelegateFilter = (source as DelegateFilterCondition).DelegateFilter;
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new DelegateFilterCondition();
            }
        }
    }
}