using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;

namespace DataControls.Tests.ListView
{
    [TestClass]
    public class GridLayoutTests
    {
        private TestGroup root;
        private StackedCompactLayout Layout;

        [TestInitialize]
        public void TestInitialize()
        {

            this.Layout = new StackedCompactLayout(new GroupHierarchyAdapter(), 10, 2);

            root = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail"),
                new TestGroup("Magazine")
            };
        }

        [TestMethod]
        public void GetFirstStackSlotOnRow_WhenFlatList_ShouldReturnCorrectSlot()
        {

            var list = new List<string>(from c in Enumerable.Range(0, 10) select "data" + c);

            Layout.SetSource(list, 0, 0, 0, 0, true);

            var slot = Layout.GetFirstStackSlotOnRow(0, 10);

            Assert.AreEqual(0, slot);

            slot = Layout.GetFirstStackSlotOnRow(2, 10);

            Assert.AreEqual(4, slot);

            slot = Layout.GetFirstStackSlotOnRow(4, 10);

            Assert.AreEqual(8, slot);

        }

        [TestMethod]
        public void GetFirstStackSlotOnRow_WhenGrouped1Level_ShouldReturnCorrectSlot()
        {

            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                group.Add(group.Name + "data11");
            }

            Layout.SetSource(list, 1, 0, 0, 0, true);

            var slot = Layout.GetFirstStackSlotOnRow(0, 20);

            Assert.AreEqual(0, slot);

            slot = Layout.GetFirstStackSlotOnRow(2, 20);

            Assert.AreEqual(2, slot);

            slot = Layout.GetFirstStackSlotOnRow(4, 20);

            Assert.AreEqual(4, slot);

            slot = Layout.GetFirstStackSlotOnRow(19, 20);

            Assert.AreEqual(19, slot);
        }

        [TestMethod]
        public void GetFirstStackSlotOnRow_WhenGrouped1LevelWith2Leaves_ShouldReturnCorrectSlot()
        {

            var list = new List<TestGroup>(from c in Enumerable.Range(0, 5) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                group.Add(group.Name + "data11");
                group.Add(group.Name + "data12");
            }

            Layout.SetSource(list, 1, 0, 0, 0, true);

            var totalSlotCount = 15;

            var slot = Layout.GetFirstStackSlotOnRow(0, totalSlotCount);

            Assert.AreEqual(0, slot);

            slot = Layout.GetFirstStackSlotOnRow(2, totalSlotCount);

            Assert.AreEqual(3, slot);

            slot = Layout.GetFirstStackSlotOnRow(4, totalSlotCount);

            Assert.AreEqual(6, slot);

            slot = Layout.GetFirstStackSlotOnRow(9, totalSlotCount);

            Assert.AreEqual(13, slot);
        }

        [TestMethod]
        public void GetFirstStackSlotOnRow_WhenGrouped1LevelWith3Leaves_ShouldReturnCorrectSlot()
        {

            var list = new List<TestGroup>(from c in Enumerable.Range(0, 5) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                group.Add(group.Name + "data11");
                group.Add(group.Name + "data12");
                group.Add(group.Name + "data13");
            }

            Layout.SetSource(list, 1, 0, 0, 0, true);

            var totalSlotCount = 20;

            var slot = Layout.GetFirstStackSlotOnRow(0, totalSlotCount);

            Assert.AreEqual(0, slot);

            slot = Layout.GetFirstStackSlotOnRow(2, totalSlotCount);

            Assert.AreEqual(3, slot);

            slot = Layout.GetFirstStackSlotOnRow(4, totalSlotCount);

            Assert.AreEqual(5, slot);

            slot = Layout.GetFirstStackSlotOnRow(14, totalSlotCount);

            Assert.AreEqual(19, slot);
        }

        [TestMethod]
        public void GetFirstStackSlotOnRow_WhenGrouped2LevelWith1Leaf_ShouldReturnCorrectSlot()
        {

            int groupLevel = 2;
            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                var childGroup = new TestGroup("SubGroup of" + group.Name);

                childGroup.Add(group.Name + "data11");
                //childGroup.Add(group.Name + "data12");
                //childGroup.Add(group.Name + "data13");

                group.Add(childGroup);
            }

            Layout.SetSource(list, groupLevel, 0, 0, 0, true);

            var totalSlotCount = Layout.TotalSlotCount;

            var slot = Layout.GetFirstStackSlotOnRow(0, totalSlotCount);

            Assert.AreEqual(0, slot);

            slot = Layout.GetFirstStackSlotOnRow(2, totalSlotCount);

            Assert.AreEqual(2, slot);

            slot = Layout.GetFirstStackSlotOnRow(4, totalSlotCount);

            Assert.AreEqual(4, slot);

            slot = Layout.GetFirstStackSlotOnRow(29, totalSlotCount);

            Assert.AreEqual(29, slot);
        }

        [TestMethod]
        public void GetFirstStackSlotOnRow_WhenGrouped2LevelWith2Leaves_ShouldReturnCorrectSlot()
        {

            int groupLevel = 2;
            var list = new List<TestGroup>(from c in Enumerable.Range(0, 5) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                var childGroup = new TestGroup("SubGroup of" + group.Name);

                childGroup.Add(group.Name + "data11");
                childGroup.Add(group.Name + "data12");
                //childGroup.Add(group.Name + "data13");

                group.Add(childGroup);
            }

            Layout.SetSource(list, groupLevel, 0, 0, 0, true);

            var totalSlotCount = Layout.TotalSlotCount;

            var slot = Layout.GetFirstStackSlotOnRow(0, totalSlotCount);

            Assert.AreEqual(0, slot);

            slot = Layout.GetFirstStackSlotOnRow(2, totalSlotCount);

            Assert.AreEqual(2, slot);

            slot = Layout.GetFirstStackSlotOnRow(4, totalSlotCount);

            Assert.AreEqual(5, slot);

            slot = Layout.GetFirstStackSlotOnRow(14, totalSlotCount);

            Assert.AreEqual(18, slot);
        }

        [TestMethod]
        public void GetFirstStackSlotOnRow_WhenGrouped2LevelWith3Leaves_ShouldReturnCorrectSlot()
        {

            int groupLevel = 2;
            var list = new List<TestGroup>(from c in Enumerable.Range(0, 5) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                var childGroup = new TestGroup("SubGroup of" + group.Name);

                childGroup.Add(group.Name + "data11");
                childGroup.Add(group.Name + "data12");
                childGroup.Add(group.Name + "data13");

                group.Add(childGroup);
            }

            Layout.SetSource(list, groupLevel, 0, 0, 0, true);

            Assert.AreEqual(20, Layout.TotalSlotCount);
            Assert.AreEqual(20, Layout.VisibleLineCount);

            var totalSlotCount = Layout.TotalSlotCount;

            var slot = Layout.GetFirstStackSlotOnRow(0, totalSlotCount);

            Assert.AreEqual(0, slot);

            slot = Layout.GetFirstStackSlotOnRow(2, totalSlotCount);

            Assert.AreEqual(2, slot);

            slot = Layout.GetFirstStackSlotOnRow(4, totalSlotCount);

            Assert.AreEqual(5, slot);

            slot = Layout.GetFirstStackSlotOnRow(15, totalSlotCount);

            Assert.AreEqual(19, slot);
        }

        [TestMethod]
        public void AddingItem_WhenFlatList_ShouldUpdateLayoutCorrectly()
        {
            var item = "data11";

            var list = new List<string>(from c in Enumerable.Range(0, 10) select "data" + c);

            Layout.SetSource(list, 0, 0, 0, 0, true);

            Assert.AreEqual(5, Layout.TotalSlotCount);
            Assert.AreEqual(5, Layout.VisibleLineCount);

            list.Add(item);
            Layout.AddItem(item, item, list.Count - 1);

            Assert.AreEqual(6, Layout.TotalSlotCount);
            Assert.AreEqual(6, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.VisibleLineCount, Layout.RenderInfoCount, "VisibleLineCount!=RenderInfoCount");

            list.Add(item);
            Layout.AddItem(item, item, list.Count - 1);

            Assert.AreEqual(6, Layout.TotalSlotCount);
            Assert.AreEqual(6, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.VisibleLineCount);

        }

        [TestMethod]
        public void InsertingItem_WhenFlatList_ShouldUpdateLayoutCorrectly()
        {
            var item = "data11";

            var list = new List<string>(from c in Enumerable.Range(0, 10) select "data" + c);

            Layout.SetSource(list, 0, 0, 0, 0, true);

            Assert.AreEqual(5, Layout.TotalSlotCount);
            Assert.AreEqual(5, Layout.VisibleLineCount);

            list.Insert(0, item);
            Layout.AddItem(item, item, 0);

            Assert.AreEqual(6, Layout.TotalSlotCount);
            Assert.AreEqual(6, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            list.Insert(0, item);
            Layout.AddItem(item, item, 0);

            Assert.AreEqual(6, Layout.TotalSlotCount);
            Assert.AreEqual(6, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.TotalSlotCount);

        }

        [TestMethod]
        public void RemovingItemFromStart_WhenFlatList_ShouldUpdateLayoutCorrectly()
        {

            var list = new List<string>(from c in Enumerable.Range(0, 10) select "data" + c);
            var item = list.Last();

            Layout.SetSource(list, 0, 0, 0, 0, true);

            Assert.AreEqual(5, Layout.TotalSlotCount);
            Assert.AreEqual(5, Layout.VisibleLineCount);


            list.Remove(item);
            Layout.RemoveItem(item, item, 0);

            Assert.AreEqual(5, Layout.TotalSlotCount);
            Assert.AreEqual(5, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            item = list.Last();
            list.Remove(item);
            Layout.RemoveItem(item, item, 0);

            Assert.AreEqual(4, Layout.TotalSlotCount);
            Assert.AreEqual(4, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.TotalSlotCount);

        }

        [TestMethod]
        public void RemovingItemFromEnd_WhenFlatList_ShouldUpdateLayoutCorrectly()
        {

            var list = new List<string>(from c in Enumerable.Range(0, 10) select "data" + c);
            var item = list.Last();

            Layout.SetSource(list, 0, 0, 0, 0, true);

            Assert.AreEqual(5, Layout.TotalSlotCount);
            Assert.AreEqual(5, Layout.VisibleLineCount);

            list.Remove(item);
            Layout.RemoveItem(item, item, list.Count - 1);

            Assert.AreEqual(5, Layout.TotalSlotCount);
            Assert.AreEqual(5, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            item = list.Last();
            list.Remove(item);
            Layout.RemoveItem(item, item, list.Count - 1);

            Assert.AreEqual(4, Layout.TotalSlotCount);
            Assert.AreEqual(4, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.TotalSlotCount);

        }

        [TestMethod]
        public void AddingItem_WhenGrouped1Level_ShouldUpdateLayoutCorrectly()
        {
            var item = "data11";

            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                group.Add(group.Name + "data11");
            }

            Layout.SetSource(list, 1, 0, 0, 0, true);

            Assert.AreEqual(20, Layout.TotalSlotCount);
            Assert.AreEqual(20, Layout.VisibleLineCount);

            list.First().Add("new Data 1");
            Layout.AddItem(list.First(), item, 2);

            Assert.AreEqual(20, Layout.TotalSlotCount);
            Assert.AreEqual(20, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            list.First().Add("new Data 2");
            Layout.AddItem(list.First(), item, 3);

            Assert.AreEqual(21, Layout.TotalSlotCount);
            Assert.AreEqual(21, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.TotalSlotCount);

        }


        [TestMethod]
        public void AddingGroup_WhenGrouped1Level_ShouldUpdateLayoutCorrectly()
        {
            var rootGroup = new TestGroup("Root");

            var newGroup = new TestGroup("new group");

            newGroup.Add(newGroup.Name + "data3");

            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                group.Add(group.Name + "data11");
            }

            Layout.SetSource(list, 1, 0, 0, 0, true);

            Assert.AreEqual(20, Layout.TotalSlotCount);
            Assert.AreEqual(20, Layout.VisibleLineCount);


            list.Insert(0, newGroup);
            Layout.AddItem(rootGroup, newGroup, 0);

            Assert.AreEqual(22, Layout.TotalSlotCount);
            Assert.AreEqual(22, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            newGroup = new TestGroup("new group 1");
            newGroup.Add(newGroup.Name + "new data");

            list.Insert(0, newGroup);
            Layout.AddItem(rootGroup, newGroup, 0);

            Assert.AreEqual(24, Layout.TotalSlotCount);
            Assert.AreEqual(24, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.TotalSlotCount);

        }

        [TestMethod]
        public void RemovingItem_WhenGrouped1Level_ShouldUpdateLayoutCorrectly()
        {

            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                group.Add(group.Name + "data11");
                group.Add(group.Name + "data12");
                group.Add(group.Name + "data13");
            }

            Layout.SetSource(list, 1, 0, 0, 0, true);

            Assert.AreEqual(30, Layout.TotalSlotCount);
            Assert.AreEqual(30, Layout.VisibleLineCount);

            var firstItem = list.First().First();
            list.First().Remove(firstItem);
            Layout.RemoveItem(list.First(), firstItem, 0);

            Assert.AreEqual(29, Layout.TotalSlotCount);
            Assert.AreEqual(29, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            firstItem = list.First().First();
            list.First().Remove(firstItem);
            Layout.RemoveItem(list.First(), firstItem, 0);

            Assert.AreEqual(29, Layout.TotalSlotCount);
            Assert.AreEqual(29, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.TotalSlotCount);

        }

        [TestMethod]
        public void RemovingGroup_WhenGrouped1Level_ShouldUpdateLayoutCorrectly()
        {
            var rootGroup = new Group("Root");
            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                group.Add(group.Name + "data11");
            }

            Layout.SetSource(list, 1, 0, 0, 0, true);

            Assert.AreEqual(20, Layout.TotalSlotCount);
            Assert.AreEqual(20, Layout.VisibleLineCount);

            var oldgroup = list.First();
            list.Remove(list.First());
            Layout.RemoveItem(rootGroup, oldgroup, 0);

            Assert.AreEqual(18, Layout.TotalSlotCount);
            Assert.AreEqual(18, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            oldgroup = list.First();
            list.Remove(list.First());
            Layout.RemoveItem(rootGroup, oldgroup, 0);

            Assert.AreEqual(16, Layout.TotalSlotCount);
            Assert.AreEqual(16, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.RenderInfoCount, Layout.TotalSlotCount);

        }

        [TestMethod]
        public void RemovingItem_WhenGrouped2Levels_ShouldUpdateLayoutCorrectly()
        {
            int groupLevel = 2;
            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                var childGroup = new TestGroup("SubGroup of" + group.Name);

                childGroup.Add(group.Name + "data11");
                childGroup.Add(group.Name + "data12");
                childGroup.Add(group.Name + "data13");

                group.Add(childGroup);
            }

            Layout.SetSource(list, groupLevel, 0, 0, 0, true);

            Assert.AreEqual(40, Layout.TotalSlotCount);
            Assert.AreEqual(40, Layout.VisibleLineCount);

            var targetGroup = (TestGroup)list.First().First();

            var firstItem = targetGroup.First();
            targetGroup.Remove(firstItem);
            Layout.RemoveItem(targetGroup, firstItem, 0);

            Assert.AreEqual(39, Layout.TotalSlotCount);
            Assert.AreEqual(39, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            firstItem = targetGroup.First();
            targetGroup.Remove(firstItem);
            Layout.RemoveItem(targetGroup, firstItem, 0);

            Assert.AreEqual(39, Layout.TotalSlotCount);
            Assert.AreEqual(39, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

        }

        [TestMethod]
        public void RemovingItem_WhenGrouped1LevelWith3Stacks_ShouldUpdateLayoutCorrectly()
        {
            Layout.StackCount = 3;

            var list = new List<TestGroup>(from c in Enumerable.Range(0, 3) select new TestGroup("group " + c));



            foreach (var group in list)
            {
                for (int i = 0; i < 7; i++)
                {
                    group.Add(group.Name + "data1" + i);
                }
            }

            //add one more item in the first group to make them uneven with the others
            list[0].Add("groupd 0 data10");

            Layout.SetSource(list, 1, 0, 0, 0, true);

            Assert.AreEqual(12, Layout.TotalSlotCount);
            Assert.AreEqual(12, Layout.VisibleLineCount);

            var firstItem = list.First().First();
            list.First().Remove(firstItem);
            Layout.RemoveItem(list.First(), firstItem, 0);

            Assert.AreEqual(12, Layout.TotalSlotCount);
            Assert.AreEqual(12, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            firstItem = list.First().First();
            list.First().Remove(firstItem);
            Layout.RemoveItem(list.First(), firstItem, 0);

            Assert.AreEqual(11, Layout.TotalSlotCount);
            Assert.AreEqual(11, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

        }

        [TestMethod]
        public void RemovingGroup_WhenGrouped2Level_ShouldUpdateLayoutCorrectly()
        {
            int groupLevel = 2;
            var rootGroup = new Group("Root");
            var list = new List<TestGroup>(from c in Enumerable.Range(0, 10) select new TestGroup("group " + c));

            foreach (var group in list)
            {
                var childGroup = new TestGroup("SubGroup of" + group.Name);

                childGroup.Add(childGroup.Name + "data11");

                group.Add(childGroup);
            }

            Layout.SetSource(list, groupLevel, 0, 0, 0, true);

            Assert.AreEqual(30, Layout.TotalSlotCount);
            Assert.AreEqual(30, Layout.VisibleLineCount);

            var oldgroup = list.First();
            list.Remove(list.First());
            Layout.RemoveItem(rootGroup, oldgroup, 0);

            Assert.AreEqual(27, Layout.TotalSlotCount);
            Assert.AreEqual(27, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

            oldgroup = list.First();
            list.Remove(list.First());
            Layout.RemoveItem(rootGroup, oldgroup, 0);

            Assert.AreEqual(24, Layout.TotalSlotCount);
            Assert.AreEqual(24, Layout.VisibleLineCount);
            Assert.AreEqual(Layout.TotalSlotCount, Layout.RenderInfoCount);

        }
    }

    [DebuggerDisplay("Name = {Name}, Count = {Count}")]
    internal class TestGroup : Collection<object>, IGroup
    {
        public TestGroup(object name)
        {
            this.Name = this.TransforToExpectedGroupName(name);
            this.Type = GroupType.BottomLevel;
        }

        protected virtual object TransforToExpectedGroupName(object groupName)
        {
            return groupName;
        }

        public new IReadOnlyList<object> Items
        {
            get
            {
                return this;
            }
        }

        public object Name { get; private set; }

        public bool HasItems
        {
            get { return this.Count > 0; }
        }

        public IGroup Parent
        {
            get;
            private set;
        }

        public GroupType Type
        {
            get;
            set;
        }

        public int Level
        {
            get
            {
                return Telerik.Data.Core.IGroupExtensions.GetLevel(this);
            }
        }

        protected override void InsertItem(int index, object item)
        {
            var itemGroup = item as TestGroup;

            if (itemGroup != null)
            {
                itemGroup.Parent = this;
            }
            base.InsertItem(index, item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            foreach (var item in this.OfType<TestGroup>())
            {
                item.Parent = null;
            }
        }

        protected override void RemoveItem(int index)
        {
            var group = this[index] as TestGroup;

            if (group != null)
            {
                group.Parent = null;
            }
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, object item)
        {
            var group = this[index] as TestGroup;
            var itemGroup = item as TestGroup;
            if (group != null)
            {
                group.Parent = null;
                itemGroup.Parent = this;
            }
            base.SetItem(index, item);
        }

        public bool IsBottomLevel
        {
            get { return this.Count == 0; }
        }
    }

    //internal class TestStackedLayout : StackedCompactLayout
    //{
    //    public TestStackedLayout(IHierarchyAdapter adapter, double defaultItemLength, int stackCount) : base(adapter, defaultItemLength, stackCount)
    //    {

    //    }

    //    internal IRenderInfo GetRenderInfo()
    //    {
    //        return 
    //    }
    //}
}
