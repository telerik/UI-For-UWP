using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Tests
{
    [TestClass]
    [Tag("Input")]
    [Tag("LoopingList")]
    public class LoopingListTests_IndexStateAndOffset_SmallSource : LoopingListTestsBase
    {
        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredTrueNearSmallSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = true;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Near;
            this.loopingList.ItemHeight = 78;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 0);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 0);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 0 && item.LogicalIndex == 0, item.IsSelected);
                    }

                    this.loopingList.SelectedIndex = 3;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, -270);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 3);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 3 && item.LogicalIndex == 3, item.IsSelected);
                    }

                    this.loopingList.SelectedIndex = 1;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, -810);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 1);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 1 && item.LogicalIndex == 1, item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredTrueMiddleSmallSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = true;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Middle;
            this.loopingList.ItemHeight = 48;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, -30);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 1);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 5 && item.LogicalIndex == 1, item.IsSelected);
                    }

                    this.loopingList.SelectedIndex = 3;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 90);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 3);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 3 && item.LogicalIndex == 3, item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredTrueFarSmallSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = true;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Far;
            this.loopingList.ItemHeight = 66;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 54);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 2);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 6 && item.LogicalIndex == 2, item.IsSelected);
                    }

                    this.loopingList.SelectedIndex = 0;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 522);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 0);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 0 && item.LogicalIndex == 0, item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredFalseNearSmallSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = false;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Near;
            this.loopingList.ItemHeight = 83;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 0);
                    Assert.AreEqual(this.loopingList.SelectedIndex, -1);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.IsFalse(item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredFalseMiddleSmallSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = false;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Middle;
            this.loopingList.ItemHeight = 83;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 0);
                    Assert.AreEqual(this.loopingList.SelectedIndex, -1);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.IsFalse(item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredFalseFarSmallSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = false;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Far;
            this.loopingList.ItemHeight = 83;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 0);
                    Assert.AreEqual(this.loopingList.SelectedIndex, -1);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.IsFalse(item.IsSelected);
                    }
                });
        }
    }
}
