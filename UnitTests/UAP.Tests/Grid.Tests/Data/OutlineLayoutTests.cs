using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class OutlineLayoutTests
    {
        private TestGroup root;
        private OutlineLayout Layout;

         [TestInitialize]
        public void TestInitialize()
        {
            this.Layout = new OutlineLayout(new GroupHierarchyAdapter(), 10);

            root = new TestGroup("Grand Total") 
            { 
                new TestGroup("Direct mail") 
                { 
                    new TestGroup("1 Free with 10"), 
                    new TestGroup("Extra Discount") 
                }, 
                new TestGroup("Magazine") 
                { 
                    new TestGroup("1 Free with 10"), 
                    new TestGroup("Extra Discount") 
                }, 
                new TestGroup("Newspaper") 
                { 
                    new TestGroup("1 Free with 10"), 
                    new TestGroup("Extra Discount") 
                } 
            };
        }

        [TestMethod]
        public void Default_Values()
        {
            Assert.AreEqual(0, Layout.VisibleLineCount);
        }

        [TestMethod]
        public void Collapsed_Event_Should_Not_Fire_On_Group_Without_Children()
        {
            var group = new TestGroup("item");
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Collapsed_Event_Should_Not_Fire_On_Group_With_Type_BottomLevel()
        {
            var group = new TestGroup("item") { Type = GroupType.BottomLevel };
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Collapsed_Event_Should_Not_Fire_On_Group_With_Type_GrandTotal()
        {
            var group = root[0];
            group.Type = GroupType.GrandTotal;

            Layout.SetSource(root.Items);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
        }

        [TestMethod]
        public void Collapsed_Event_Should_Not_Fire_On_Already_Collapsed_Group()
        {
            var group = root[0];
            Layout.SetSource(root.Items);

            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Collapsed_Event_Should_Not_Fire_For_Groups_That_Are_Parents_To_SubTotals()
        {
            var group = root[0][0];
            Layout.SetSource(root.Items, 3, TotalsPosition.First, 3, 2, true);

            Assert.AreEqual("1 Free with 10", group.Name);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);

            Layout.SetSource(root.Items, 3, TotalsPosition.First, 2, 2, true);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Collapse_BottomLevel_TotalsCount_1_AggregatesLevel_0()
        {
            var root = CreateGroups(2, 0, 1);
            var group = root[0][1];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Collapse_Subtotal_TotalsCount_1_AggregatesLevel_0()
        {
            var root = CreateGroups(2, 0, 1);
            var group = root[0][0];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Collapse_SubHeading_At_AggregateIndex_Level_TotalsCount_2_AggregatesLevel_0()
        {
            var root = CreateGroups(2, 0);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Collapsed_Event_Should_Fire_For_SubHeading_TotalsCount_1_AggregatesLevel_0()
        {
            var root = CreateGroups(2, 0, 1);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
        }

        [TestMethod]
        public void Can_Collapse_SubHeading_TotalsCount_2_AggregatesLevel_0()
        {
            var root = CreateGroups(2, 0);
            var group = root[0][0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
        }

        [TestMethod]
        public void Cannot_Collapse_BottomLevel_TotalsCount_2_AggregatesLevel_0()
        {
            var root = CreateGroups(2, 0);
            var group = root[0][0][1];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Collapse_Subtotal_TotalsCount_2_AggregatesLevel_0()
        {
            var root = CreateGroups(2, 0);
            var group = root[0][0][0];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Can_Collapse_SubHeading_TotalsCount_2_AggregatesLevel_1()
        {
            var root = CreateGroups(2, 1);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
        }

        [TestMethod]
        public void Cannot_Collapse_BottomLevel_TotalsCount_2_AggregatesLevel_1()
        {
            var root = CreateGroups(2, 1);
            var group = root[0][2][1];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Collapse_Subtotal_TotalsCount_2_AggregatesLevel_1()
        {
            var root = CreateGroups(2, 1);
            var group = root[0][0];

            Assert.AreEqual(GroupType.Subtotal, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Expand_SubHeading_TotalsCount_1_Beacuse_ItWasntCollapsed()
        {
            var root = CreateGroups(2, 0, 1);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Can_Expand_SubHeading_TotalsCount_1()
        {
            var root = CreateGroups(2, 0, 1);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), true);
        }

        [TestMethod]
        public void Cannot_Expand_BottomLevel_TotalsCount_1()
        {
            var root = CreateGroups(2, 0, 1);
            var group = root[0][1];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Cannot_Expand_Subtotal_TotalsCount_1()
        {
            var root = CreateGroups(2, 0, 1);
            var group = root[0][0];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Cannot_Expand_SubHeading_At_AggregateIndex_Level_TotalsCount_2()
        {
            var root = CreateGroups(2, 0);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Cannot_Expand_SubHeading_TotalsCount_2_Beacuse_ItWasntCollapsed()
        {
            var root = CreateGroups(2, 0);
            var group = root[0][0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Cannot_Expand_Any_Group_Beacuse_IItWasntCollapsed()
        {
            var root = CreateGroups(2, 0);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);

            group = root[0][0];
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);

            group = root[0][0][0];
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Can_Expand_SubHeading_TotalsCount_2()
        {
            var root = CreateGroups(2, 0);
            var group = root[0][0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), true);
        }

        [TestMethod]
        public void Cannot_Expand_BottomLevel_TotalsCount_2()
        {
            var root = CreateGroups(2, 0);
            var group = root[0][0][1];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Cannot_Expand_Subtotal_TotalsCount_2()
        {
            var root = CreateGroups(2, 0);
            var group = root[0][0][0];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Cannot_Expand_SubHeading_TotalsCount_2_AggregatesLevel_1()
        {
            var root = CreateGroups(2, 1);
            var group = root[0][2];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);

            group = root[0][0];

            Assert.AreEqual(GroupType.Subtotal, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);

            group = root[0][2][0];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
            Layout.AssertEventFired("Expanded", () => Layout.Expand(group), false);
        }

        [TestMethod]
        public void Can_Collapse_SubHeading_At_AggregateIndex_Level_TotalsCount_2_AggregatesLevel_2()
        {
            var root = CreateGroups(2, 2);
            var group = root[0];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), true);
        }

        [TestMethod]
        public void Cannot_Collapse_SubHeading_TotalsCount_2_AggregatesLevel_2()
        {
            var root = CreateGroups(2, 2);
            var group = root[0][2];

            Assert.AreEqual(GroupType.Subheading, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Collapse_BottomLevel_TotalsCount_2_AggregatesLevel_2()
        {
            var root = CreateGroups(2, 2);
            var group = root[0][2][1];

            Assert.AreEqual(GroupType.BottomLevel, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void Cannot_Collapse_Subtotal_TotalsCount_2_AggregatesLevel_2()
        {
            var root = CreateGroups(2, 2);
            var group = root[0][0];

            Assert.AreEqual(GroupType.Subtotal, group.Type);
            Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), false);
        }

        [TestMethod]
        public void SetSource()
        {
            List<TestGroup> groups = new List<TestGroup>(5);
            for (int i = 0; i < 5; i++)
            {
                groups.Add(new TestGroup(i));
            }

            Layout.SetSource(groups, 1 /* LayoutLevels */, TotalsPosition.First, 1 /* AggregatesLevel */, 0 /* TotalsCount */, true /* ShowAggregateValuesInline */);

            Assert.AreEqual(groups.Count, Layout.VisibleLineCount);
        }

        [TestMethod]
        public void SetSource_To_NULL()
        {
            Layout.SetSource(null);
            IEnumerable<IList<ItemInfo>> lines = null;
            try
            {
                lines = Layout.GetLines(0, true);
                Assert.IsFalse(lines.Any());
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SetSource_AggregatesLevel_Coerce_Correctly()
        {
            List<TestGroup> groups = new List<TestGroup>(5);
            for (int i = 0; i < 5; i++)
            {
                groups.Add(new TestGroup(i));
            }

            Layout.SetSource(groups, 1 /* LayoutLevels */, TotalsPosition.First, 10 /* AggregatesLevel */, 0 /* TotalsCount */, true /* ShowAggregateValuesInline */);
            Assert.AreEqual(0, Layout.AggregatesLevel);

            Layout.SetSource(groups, 4 /* LayoutLevels */, TotalsPosition.First, 10 /* AggregatesLevel */, 2 /* TotalsCount */, true /* ShowAggregateValuesInline */);
            Assert.AreEqual(3, Layout.AggregatesLevel);

            Layout.SetSource(groups, 1 /* LayoutLevels */, TotalsPosition.Last, -10 /* AggregatesLevel */, 0 /* TotalsCount */, true /* ShowAggregateValuesInline */);
            Assert.AreEqual(0, Layout.AggregatesLevel);
        }

        [TestMethod]
        public void SetSource_TotalsCount()
        {
            List<TestGroup> groups = new List<TestGroup>(5);
            for (int i = 0; i < 5; i++)
            {
                groups.Add(new TestGroup(i));
            }

            Layout.SetSource(groups, 3 /* LayoutLevels */, TotalsPosition.First, 10 /* AggregatesLevel */, 5 /* TotalsCount */, false /* ShowAggregateValuesInline */);
            Assert.AreEqual(5, Layout.TotalsCount);

            Layout.SetSource(groups, 3 /* LayoutLevels */, TotalsPosition.First, 10 /* AggregatesLevel */, 0 /* TotalsCount */, false /* ShowAggregateValuesInline */);
            Assert.AreEqual(0, Layout.TotalsCount);
        }

        [TestMethod]
        public void SetSource_ShowAggregateValuesInline()
        {
            List<TestGroup> groups = new List<TestGroup>(5);
            for (int i = 0; i < 5; i++)
            {
                groups.Add(new TestGroup(i));
            }

            Layout.SetSource(groups, 3 /* LayoutLevels */, TotalsPosition.First, 10 /* AggregatesLevel */, 2 /* TotalsCount */, true /* ShowAggregateValuesInline */);
            Assert.IsTrue(Layout.ShowAggregateValuesInline);

            Layout.SetSource(groups, 3 /* LayoutLevels */, TotalsPosition.First, 10 /* AggregatesLevel */, 2 /* TotalsCount */, false /* ShowAggregateValuesInline */);
            Assert.IsFalse(Layout.ShowAggregateValuesInline);
        }

        [TestMethod]
        public void All_ItemInfos_Should_Have_Unique_ID()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                var root = CreateGroups(3, 0, 2, totalsPosition, showAggregatesInline);

                HashSet<int> ids = new HashSet<int>();
                var lines = Layout.GetLines(0, true);
                foreach (var lineInfo in lines)
                {
                    var item = lineInfo.Last();
                    Assert.IsFalse(ids.Contains(item.Id));
                    ids.Add(item.Id);
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void IsCollapsible_1_Total()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                var root = CreateGroups(3, 0, 1, totalsPosition, showAggregatesInline);
                foreach (var group in root.Flatten())
                {
                    bool expected = group.HasItems && !group.IsBottomLevel;
                    Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), expected);
                    Layout.Expand(group);
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void IsCollapsible_2_Totals_OnLevel_0()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                var root = CreateGroups(3, 0, 2, totalsPosition, showAggregatesInline);
                foreach (var group in root.Flatten())
                {
                    bool expected = group.HasItems && !group.IsBottomLevel && group.Level != 0;
                    Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), expected);
                    Layout.Expand(group);
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void IsCollapsible_2_Totals_OnLevel_1()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                var root = CreateGroups(3, 1, 2, totalsPosition, showAggregatesInline);
                foreach (var group in root.Flatten())
                {
                    bool expected = group.HasItems && !group.IsBottomLevel && group.Level != 1;
                    Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), expected);
                    Layout.Expand(group);
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void IsCollapsible_2_Totals_OnLevel_2()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                var root = CreateGroups(3, 2, 2, totalsPosition, showAggregatesInline);
                foreach (var group in root.Flatten())
                {
                    bool expected = group.HasItems && !group.IsBottomLevel && group.Level != 2;
                    Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), expected);
                    Layout.Expand(group);
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void IsCollapsible_2_Totals_OnLevel_3()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                var root = CreateGroups(3, 3, 2, totalsPosition, showAggregatesInline);
                foreach (var group in root.Flatten())
                {
                    bool expected = group.HasItems && !group.IsBottomLevel && group.Level < 2;
                    Layout.AssertEventFired("Collapsed", () => Layout.Collapse(group), expected);
                    Layout.Expand(group);
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void IsVisible()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                CreateGroups(3, 0, 2, totalsPosition, showAggregatesInline);
                foreach (var lines in this.Layout.GetLines(0, true))
                {
                    foreach (var item in lines)
                    {
                        Assert.IsFalse(item.IsCollapsed);
                    }
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void IsVisible_Return_False_When_Parent_IsCollapsed()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                var root = CreateGroups(3, 0, 2, totalsPosition, showAggregatesInline);
                var group = root[0][0];
                Layout.Collapse(group);

                foreach (var lines in this.Layout.GetLines(0, true))
                {
                    foreach (var item in lines)
                    {
                        if (group.IsParent(item) || group == item.Item)
                        {
                            Assert.IsTrue(item.IsCollapsed);
                        }
                        else
                        {
                            Assert.IsFalse(item.IsCollapsed);
                        }
                    }
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void TestItemAndLayoutInfo()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                for (int totals = 0; totals < 3; totals++)
                {
                    for (int groups = 0; groups <= 3; groups++)
                    {
                        for (int aggregatesLevel = 0; aggregatesLevel <= groups; aggregatesLevel++)
                        {
                            HashSet<int> ids = new HashSet<int>();
                            HashSet<int> slots = new HashSet<int>();

                            var root = CreateGroups(groups /* GroupLevels */, aggregatesLevel  /* AggregatesLevel */, totals /* TotalsCount */, totalsPosition, showAggregatesInline /* ShowAggregateValuesInline */);
                            int exptectedCount = root.Flatten().Count();

                            string testCase = string.Format("Fail Case: AggregatesLevels: {0}, TotalsCount: {1}, ShowAggregateValuesInline: {2}", Layout.AggregatesLevel, Layout.TotalsCount, Layout.ShowAggregateValuesInline);
                            Assert.AreEqual(exptectedCount, Layout.VisibleLineCount, testCase);

                            for (int i = 0; i < Layout.VisibleLineCount; i++)
                            {
                                var expectedGroups = GetExpectedGroups(root, i).ToList();
                                Assert.AreEqual(1, expectedGroups.Count);
                                var expectedGroup = expectedGroups[0];

                                var line = Layout.GetLines(i, true).First();
                                Assert.IsNotNull(line);
                                Assert.IsTrue(line.Count == line.Last().Level + 1);
                                ItemInfo info = line.Last();

                                Assert.AreSame(expectedGroup, info.Item, "Groups mismatch for slot {0}\r\n{1}.", i, testCase);

                                Assert.AreEqual(i, info.Id);
                                Assert.AreEqual(i, info.Slot);
                                Assert.IsFalse(ids.Contains(info.Id));
                                ids.Add(info.Id);
                                Assert.IsFalse(slots.Contains(info.Slot));
                                slots.Add(info.Slot);
                                Assert.AreEqual(expectedGroup.Level, info.Level);
                                Assert.AreEqual(expectedGroup.Type, info.ItemType);

                                bool expectedIsCollapsible = this.IsCollapsible(expectedGroup);
                                Assert.AreEqual(expectedIsCollapsible, info.IsCollapsible);
                                Assert.IsFalse(info.IsCollapsed);
                                Assert.IsFalse(info.IsSummaryVisible);
                                Assert.IsTrue(info.IsDisplayed);

                                LayoutInfo layout = info.LayoutInfo;
                                int expectedLevel = this.GetLayoutLevel(expectedGroup, aggregatesLevel);

                                Assert.AreEqual(0, layout.Indent);
                                Assert.AreEqual(i, layout.Line);
                                Assert.AreEqual(1, layout.LineSpan);
                                Assert.AreEqual(expectedLevel, layout.Level);
                                Assert.AreEqual(1, layout.LevelSpan);
                                Assert.IsTrue(layout.SpansThroughCells);

                                if (expectedIsCollapsible)
                                {
                                    Layout.Collapse(expectedGroup);

                                    line = Layout.GetLines(i, true).First();
                                    Assert.IsNotNull(line);
                                    Assert.IsTrue(line.Count == line.Last().Level + 1);
                                    info = line.Last();

                                    Assert.IsTrue(info.IsCollapsed);
                                    Assert.IsTrue(info.IsDisplayed);

                                    bool isSummaryVisibleExpected = this.IsSummaryVisibleExpected(info, expectedGroup);
                                    Assert.AreEqual(isSummaryVisibleExpected, info.IsSummaryVisible);

                                    layout = info.LayoutInfo;
                                    expectedLevel = this.GetLayoutLevel(expectedGroup, aggregatesLevel);

                                    Assert.AreEqual(0, layout.Indent);
                                    Assert.AreEqual(i, layout.Line);
                                    Assert.AreEqual(1, layout.LineSpan);
                                    Assert.AreEqual(expectedLevel, layout.Level);
                                    Assert.AreEqual(1, layout.LevelSpan);
                                    Assert.IsTrue(layout.SpansThroughCells);

                                    int subGroupsCount = expectedGroup.Flatten().Count() - 1;

                                    int aggLevel = showAggregatesInline ? aggregatesLevel - 1 : aggregatesLevel;

                                    if (totals > 1 && expectedGroup.Level < aggLevel)
                                    {
                                        switch (totalsPosition)
                                        {
                                            case TotalsPosition.Last:
                                                subGroupsCount -= Layout.TotalsCount;
                                                break;
                                            case TotalsPosition.Inline:
                                            case TotalsPosition.First:
                                                subGroupsCount -= Layout.TotalsCount;
                                                break;
                                            case TotalsPosition.None:
                                                break;
                                        }
                                    }

                                    var lineIterator = Layout.GetLines(i + 1, true).GetEnumerator();
                                    if (lineIterator.MoveNext())
                                    {
                                        line = lineIterator.Current;

                                        Assert.IsNotNull(line);
                                        Assert.IsTrue(line.Count == line.Last().Level + 1);
                                        info = line.Last();

                                        expectedLevel = expectedGroup.Level;
                                        if (totals > 1 && expectedLevel < aggLevel && (totalsPosition == TotalsPosition.First || totalsPosition == TotalsPosition.Inline))
                                        {
                                            Assert.AreEqual(i + 1, info.Id);
                                            Assert.AreEqual(i + 1, info.Slot);
                                        }
                                        else if (totalsPosition == TotalsPosition.Last)
                                        {
                                            Assert.AreEqual(i + 1 + subGroupsCount, info.Id);
                                            Assert.AreEqual(i + 1 + subGroupsCount, info.Slot);
                                        }

                                        if ((info.Item as IGroup).HasItems && !(info.Item as IGroup).IsBottomLevel)
                                        {
                                            Assert.AreEqual(GroupType.Subheading, info.ItemType);
                                        }
                                        else if (totalsPosition == TotalsPosition.None)
                                        {
                                            Assert.AreEqual(GroupType.BottomLevel, info.ItemType);
                                        }
                                        else
                                        {
                                            Assert.AreEqual(GroupType.Subtotal, info.ItemType);
                                        }

                                        Assert.IsFalse(info.IsCollapsed);
                                        Assert.IsTrue(info.IsDisplayed);

                                        expectedIsCollapsible = this.IsCollapsible(info.Item as IGroup);
                                        Assert.AreEqual(expectedIsCollapsible, info.IsCollapsible);

                                        isSummaryVisibleExpected = this.IsSummaryVisibleExpected(info, info.Item as IGroup);
                                        Assert.AreEqual(isSummaryVisibleExpected, info.IsSummaryVisible);

                                        layout = info.LayoutInfo;
                                        expectedLevel = this.GetLayoutLevel(info.Item as IGroup, aggregatesLevel);

                                        Assert.AreEqual(0, layout.Indent);
                                        Assert.AreEqual(i + 1, layout.Line);
                                        Assert.AreEqual(1, layout.LineSpan);
                                        Assert.AreEqual(expectedLevel, layout.Level);
                                        Assert.AreEqual(1, layout.LevelSpan);
                                        Assert.IsTrue(layout.SpansThroughCells);

                                        //if (lineIterator.MoveNext())
                                        //{
                                        //    line = lineIterator.Current;

                                        //    Assert.IsNotNull(line);
                                        //    Assert.IsTrue(line.Count == 1);
                                        //    info = line[0];

                                        //    Assert.IsFalse(info.IsDisplayed);
                                        //}
                                    }

                                    Layout.Expand(expectedGroup);
                                }
                            }
                        }
                    }
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void VisibleSlotCount()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    var root = CreateGroups(3, i, 2, totalsPosition, showAggregatesInline);
                    var flattenGroups = root.Flatten().ToList();
                    int count = Layout.VisibleLineCount;

                    int level = i <= 1 ? 2 : 1;
                    var firstLineLastCollapsibleGroup = flattenGroups.First(gr => gr.Type == GroupType.Subheading && gr.Level == level);

                    Layout.Collapse(firstLineLastCollapsibleGroup);

                    int collapsedCount1 = firstLineLastCollapsibleGroup.Items.OfType<IGroup>().Flatten().Count();
                    if (firstLineLastCollapsibleGroup.Level < Layout.AggregatesLevel - (showAggregatesInline ? 1 : 0) && Layout.TotalsCount > 1 && totalsPosition != TotalsPosition.None)
                    {
                        collapsedCount1 -= Layout.TotalsCount;
                    }

                    Assert.AreEqual(count - collapsedCount1, Layout.VisibleLineCount);

                    var collapsibleParent = firstLineLastCollapsibleGroup.Parent;
                    while (collapsibleParent != null && !IsCollapsible(collapsibleParent))
                    {
                        collapsibleParent = collapsibleParent.Parent;
                    }

                    Layout.Collapse(collapsibleParent);

                    int collapsedCount2 = collapsibleParent.Items.OfType<IGroup>().Flatten().Count();
                    if (collapsibleParent.Level < Layout.AggregatesLevel - (showAggregatesInline ? 1 : 0) && Layout.TotalsCount > 1 && totalsPosition != TotalsPosition.None)
                    {
                        collapsedCount2 -= Layout.TotalsCount;
                    }

                    Assert.AreEqual(count - collapsedCount2, Layout.VisibleLineCount);

                    var firstLineLastCollapsibleGroupSibling = flattenGroups.First(gr => gr.Type == GroupType.Subheading && gr.Level == firstLineLastCollapsibleGroup.Level && gr != firstLineLastCollapsibleGroup);
                    Assert.AreEqual(firstLineLastCollapsibleGroup.Parent, firstLineLastCollapsibleGroupSibling.Parent);

                    Layout.Collapse(firstLineLastCollapsibleGroupSibling);
                    Assert.AreEqual(count - collapsedCount2, Layout.VisibleLineCount);

                    Layout.Expand(collapsibleParent);

                    int collapsedCount3 = firstLineLastCollapsibleGroupSibling.Items.OfType<IGroup>().Flatten().Count();
                    if (firstLineLastCollapsibleGroup.Level < Layout.AggregatesLevel - (showAggregatesInline ? 1 : 0) && Layout.TotalsCount > 1 && totalsPosition != TotalsPosition.None)
                    {
                        collapsedCount3 -= Layout.TotalsCount;
                    }

                    int collapsedCount = collapsedCount1 + collapsedCount3;
                    Assert.AreEqual(count - collapsedCount, Layout.VisibleLineCount);
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);

        }

        private int GetLayoutLevel(IGroup group, int aggregatesLevel)
        {
            if (group.Type == GroupType.Subtotal)
            {
                if (group.Parent != null && this.Layout.IsCollapsed(group.Parent))
                {
                    return aggregatesLevel;
                }
                else
                {
                    return group.Level - 1;
                }
            }

            return group.Level;
        }

        private bool IsCollapsible(IGroup group)
        {
            int itemLevel = group.Level;
            bool hasItems = group.HasItems && !group.IsBottomLevel;
            if (Layout.TotalsCount > 1)
            {
                if (Layout.AggregatesLevel >= Layout.GroupLevels - 1)
                {
                    return hasItems && itemLevel < Layout.GroupLevels - 2;
                }
                else
                {
                    return hasItems && itemLevel != Layout.AggregatesLevel;
                }
            }
            else
            {
                return hasItems;
            }
        }

        private bool IsSummaryVisibleExpected(ItemInfo info, IGroup group)
        {
            return info.ItemType == GroupType.Subtotal && group.Parent != null && Layout.IsCollapsed(group.Parent);
        }

        [TestMethod]
        public void OutlineLayout_ThrowException_When_Null_AdapterIsPassed()
        {
            bool exception = false;
            BaseLayout layout;
            try
            {
                layout = new OutlineLayout(null, 10);
            }
            catch (ArgumentNullException)
            {
                exception = true;
            }

            Assert.IsTrue(exception);
        }

        [TestMethod]
        public void SetSource_NoParameters()
        {
            List<TestGroup> groups = new List<TestGroup>(5);
            for (int i = 0; i < 5; i++)
            {
                groups.Add(new TestGroup(i));
            }

            Layout.SetSource(groups);

            Assert.AreEqual(groups.Count, Layout.VisibleLineCount);
            Assert.AreEqual(0, Layout.AggregatesLevel);
            Assert.AreEqual(0, Layout.TotalsCount);
            Assert.AreEqual(TotalsPosition.None, Layout.TotalsPosition);
            Assert.IsFalse(Layout.ShowAggregateValuesInline);

            foreach (var item in this.GetItems())
            {
                Assert.AreEqual(0, item.LayoutInfo.Level);
            }
        }

        [TestMethod]
        public void GetIndent()
        {
            var action = new Action<TotalsPosition, bool>((totalsPosition, showAggregatesInline) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    var root = CreateGroups(3, i, 2, totalsPosition, showAggregatesInline);

                    foreach (var item in this.GetItems())
                    {
                        Assert.AreEqual(0, item.LayoutInfo.Indent);
                    }
                }
            });

            Enumerate_TotalsPosition_ShowAggregateValuesInline(action);
        }

        [TestMethod]
        public void GetLayoutLevel()
        {
            int groupLevels = 3;
            var root = CreateGroups(groupLevels, 0, 1);

            foreach (var item in this.GetItems())
            {
                if (item.ItemType == GroupType.Subtotal)
                {
                    if (item.IsCollapsible)
                    {
                        Assert.AreEqual(item.Level - 1, item.LayoutInfo.Level);
                        Layout.Collapse(item.Item);
                        Assert.AreEqual(groupLevels - 1, item.LayoutInfo.Level);
                        Layout.Expand(item.Item);
                    }
                    else
                    {
                        Assert.AreEqual(item.Level - 1, item.LayoutInfo.Level);
                    }
                }
                else
                {
                    Assert.AreEqual(item.Level, item.LayoutInfo.Level);
                }
            }
        }

        private void Enumerate_TotalsPosition_ShowAggregateValuesInline(Action<TotalsPosition, bool> action)
        {
            for (int j = 0; j < 2; j++)
            {
                bool showAggregatesInline = j == 0;

                var values = Enum.GetValues(typeof(TotalsPosition));
                foreach (var value in values)
                {
                    action((TotalsPosition)value, showAggregatesInline);
                }
            }
        }

        private ReadOnlyList<TestGroup> CreateGroups(int levels, int aggregatesLevel, int totalsCount = 2, TotalsPosition totalsPosition = TotalsPosition.First, bool showAggregatesValueInline = false)
        {
            ReadOnlyList<TestGroup> groups = new ReadOnlyList<TestGroup>();

            Assert.IsTrue(levels >= 0);
            Assert.IsTrue(totalsCount >= 0);

            int totalLevelCount = totalsCount > 1 ? levels + 1 : levels;

            GenerateGroups(0, aggregatesLevel, groups, totalLevelCount, 3, totalsCount, totalsPosition, showAggregatesValueInline);

            Layout.SetSource(groups, totalLevelCount, totalsPosition, aggregatesLevel, totalsCount, showAggregatesValueInline);

            return groups;
        }

        private static void GenerateGroups(int level, int aggregatesLevel, IList<TestGroup> groups, int totalLevelCount, int childCount, int totalsCount, TotalsPosition totalsPosition, bool showAggregatesValueInline)
        {
            if (level < totalLevelCount)
            {
                aggregatesLevel = totalsCount > 1 ? aggregatesLevel : -1;
                int end = (level == aggregatesLevel && totalsCount > 1) ? totalsCount : childCount;
                for (int i = 1; i <= end; i++)
                {
                    var group = new TestGroup(string.Format("Level: {0}, Group: {1}", level, i));
                    if (level == (totalLevelCount - 1))
                    {
                        group.Type = GroupType.BottomLevel;
                    }
                    else if (level < (totalLevelCount - 1))
                    {
                        group.Type = GroupType.Subheading;
                    }

                    groups.Add(group);

                    GenerateGroups(level + 1, aggregatesLevel, group, totalLevelCount, childCount, totalsCount, totalsPosition, showAggregatesValueInline);
                }

                TestGroup tg = groups as TestGroup;
                if (tg != null && tg.Type == GroupType.Subheading && (level - 1) != aggregatesLevel)
                {
                    if ((!showAggregatesValueInline || level != aggregatesLevel) && !(totalLevelCount - 1 == aggregatesLevel && level == totalLevelCount - 1))
                    {
                        int aggCount = totalsCount;
                        if (aggregatesLevel == -1)
                        {
                            aggCount = 1;
                        }
                        else if (aggregatesLevel < level)
                        {
                            aggCount = 1;
                        }

                        for (int i = 1; i <= aggCount; i++)
                        {
                            var type = level == totalLevelCount - 1 ? GroupType.BottomLevel : GroupType.Subtotal;
                            var subTotal = new TestGroup(string.Format("Level: {0}, SubTotal: {1}", level, i)) { Type = type };
                            if (totalsPosition == TotalsPosition.First || (totalsPosition == TotalsPosition.Inline && aggCount > 1))
                            {
                                tg.Insert(0, subTotal);
                            }
                            else if (totalsPosition == TotalsPosition.Last)
                            {
                                tg.Add(subTotal);
                            }
                        }
                    }
                }
            }
        }

        private void TestGetGroupsAtSlot(ReadOnlyList<TestGroup> root, int expectedVisibleSlots)
        {
            string testCase = string.Format("FAILED - Case: AggregatesLevels: {0}, TotalsCount: {1}, ShowAggregateValuesInline: {2}", Layout.AggregatesLevel, Layout.TotalsCount, Layout.ShowAggregateValuesInline);
            Assert.AreEqual(expectedVisibleSlots, Layout.VisibleLineCount, testCase);

            for (int i = 0; i < Layout.VisibleLineCount; i++)
            {
                var expectedGroups = GetExpectedGroups(root, i);

                var actual = Layout.GetLines(i, true).First().Select(itemInfo => itemInfo.Item);
                Assert.IsTrue(expectedGroups.ItemsEqual(actual), "Groups mismatch for slot {0}\r\n{1}.", i, testCase);
            }
        }

        private static IEnumerable<IGroup> GetExpectedGroups(ReadOnlyList<TestGroup> root, int slot)
        {
            var expectedGroups = new List<IGroup>();

            var group = root.Flatten().ElementAt(slot);
            expectedGroups.Add(group);

            return expectedGroups;
        }

        private IEnumerable<ItemInfo> GetItems()
        {
            foreach (var lines in this.Layout.GetLines(0, true))
            {
                foreach (var line in lines)
                {
                    yield return line;
                }
            }
        }
    }
}