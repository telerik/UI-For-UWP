using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class GrandTotalComparerTests
    {
        private static AggregatesResultProviderMock resultProvider;
        private TestContext testContextInstance;
        private GrandTotalComparer grandTotalComparer;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

         [TestInitialize]
        public void TestInitialize()
        {
            resultProvider = new AggregatesResultProviderMock();
            grandTotalComparer = new GrandTotalComparer();
        }

        [TestMethod]
        public void GrandTotalComparer_DefaultValues()
        {
            var comparer = grandTotalComparer;
            Assert.AreEqual(0, comparer.AggregateIndex);
        }

        [TestMethod]
        public void GrandTotalComparer_Clone_Correctly()
        {
            GrandTotalComparer comparer = new GrandTotalComparer();
            comparer.AggregateIndex = 1;

            var copy = comparer.Clone() as GrandTotalComparer;
            Assert.IsNotNull(copy);
            Assert.AreNotSame(comparer, copy);
            Assert.IsInstanceOfType(copy, typeof(GrandTotalComparer));
            Assert.AreEqual(comparer.AggregateIndex, copy.AggregateIndex);
        }

        [TestMethod]
        public void GrandTotalComparer_AggregateIndex_Raise_PropertyChanged()
        {
            GrandTotalComparer comparer = new GrandTotalComparer();
            comparer.AssertPropertyChanged("Setting AggregateIndex", () => comparer.AggregateIndex = 1, "AggregateIndex");
        }

        [TestMethod]
        public void GrandTotalComparer_Specify_ColumnRootGroup_When_FilterAxis_Is_Row()
        {
            FakeAggregatesResultProviderMock resultProvider2 = new FakeAggregatesResultProviderMock();
            var axis = resultProvider2.Axis = DataAxis.Rows;
            var group = new Group("Test");
            ((Group)resultProvider2.Root.RowGroup).AddGroup(group);
            List<IGroup> groups = new List<IGroup>();
            groups.Add(group);
            ReadOnlyList<IGroup, IGroup> readOnlyGroups = new ReadOnlyList<IGroup, IGroup>(groups);

            Assert.AreEqual(0, grandTotalComparer.CompareGroups(resultProvider2, group, group, axis));
        }

        [TestMethod]
        public void GrandTotalComparer_Specify_ColumnRootGroup_When_FilterAxis_Is_Column()
        {
            FakeAggregatesResultProviderMock resultProvider2 = new FakeAggregatesResultProviderMock();
            var axis = resultProvider2.Axis = DataAxis.Columns;
            var group = new Group("Test");
            ((Group)resultProvider2.Root.ColumnGroup).AddGroup(group);
            List<IGroup> groups = new List<IGroup>();
            groups.Add(group);
            ReadOnlyList<IGroup, IGroup> readOnlyGroups = new ReadOnlyList<IGroup, IGroup>(groups);

            Assert.AreEqual(0, grandTotalComparer.CompareGroups(resultProvider2, group, group, axis));
        }

        [TestMethod]
        public void GrandTotalComparer_Compare_Level1()
        {
            var comparer = grandTotalComparer;
            comparer.AggregateIndex = 0;
            TestHelper(1, DataAxis.Rows, SortOrder.Ascending);
            TestHelper(1, DataAxis.Rows, SortOrder.Descending);

            comparer.AggregateIndex = 1;
            TestHelper(1, DataAxis.Rows, SortOrder.Ascending);
            TestHelper(1, DataAxis.Rows, SortOrder.Descending);
        }

        [TestMethod]
        public void GrandTotalComparer_Compare_Level2()
        {
            var comparer = grandTotalComparer;
            comparer.AggregateIndex = 0;
            TestHelper(2, DataAxis.Rows, SortOrder.Ascending);
            TestHelper(2, DataAxis.Rows, SortOrder.Descending);

            comparer.AggregateIndex = 1;
            TestHelper(2, DataAxis.Rows, SortOrder.Ascending);
            TestHelper(2, DataAxis.Rows, SortOrder.Descending);
        }

        [TestMethod]
        public void GrandTotalComparer_Compare_Level3()
        {
            var comparer = grandTotalComparer;
            comparer.AggregateIndex = 0;
            TestHelper(3, DataAxis.Rows, SortOrder.Ascending);
            TestHelper(3, DataAxis.Rows, SortOrder.Descending);

            comparer.AggregateIndex = 1;
            TestHelper(3, DataAxis.Rows, SortOrder.Ascending);
            TestHelper(3, DataAxis.Rows, SortOrder.Descending);
        }

        private static List<IGroup> GetGroupsAtLevel(DataAxis axis, int level)
        {
            List<IGroup> groups = new List<IGroup>();
            Coordinate root = resultProvider.Root;
            if (axis == DataAxis.Rows)
            {
                GetSubGroups(root.RowGroup, groups, level);
            }
            else
            {
                GetSubGroups(root.ColumnGroup, groups, level);
            }
            return groups;
        }

        private static void GetSubGroups(IGroup group, List<IGroup> groups, int level)
        {
            foreach (var subGroup in group.Items.OfType<IGroup>())
            {
                int groupLevel = subGroup.Level;
                if (level == groupLevel)
                {
                    groups.Add(subGroup);
                }
                if (subGroup.HasItems && !group.IsBottomLevel)
                {
                    GetSubGroups(subGroup, groups, level);
                }
            }
        }

        private void TestHelper(int level, DataAxis axis, SortOrder sortOrder)
        {
            var groups = GetGroupsAtLevel(axis, level);

            Assert.IsNotNull(groups);
            Assert.IsTrue(groups.Count > 0);
            CollectionAssert.AllItemsAreNotNull(groups);
            CollectionAssert.AllItemsAreUnique(groups);

            var comparer = new GroupComparerDecorator(this.grandTotalComparer, sortOrder, resultProvider, axis);

            if (level > 1)
            {
                var subGroups = GetGroupsAtLevel(axis, level - 1);

                foreach (var group in subGroups.OfType<Group>())
                {
                    group.SortSubGroups(comparer);

                    IGroup temp = null;
                    foreach (var subGroup in group.Items.OfType<IGroup>())
                    {
                        if (temp != null)
                        {
                            int result = grandTotalComparer.CompareGroups(resultProvider, temp, subGroup, axis);
                            if (sortOrder == SortOrder.Ascending)
                            {
                                Assert.IsTrue(result <= 0);
                            }
                            else
                            {
                                Assert.IsTrue(result >= 0);
                            }
                        }

                        temp = subGroup;
                    }
                }
            }
            else
            {
                groups.Sort(comparer);

                IGroup temp = null;
                foreach (var group in groups)
                {
                    if (temp != null)
                    {
                        int result = grandTotalComparer.CompareGroups(resultProvider, temp, group, axis);
                        if (sortOrder == SortOrder.Ascending)
                        {
                            Assert.IsTrue(result <= 0);
                        }
                        else
                        {
                            Assert.IsTrue(result >= 0);
                        }
                    }

                    temp = group;
                }
            }
        }
    }
}