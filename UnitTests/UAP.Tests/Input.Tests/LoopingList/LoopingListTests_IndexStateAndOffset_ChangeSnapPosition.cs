using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Tests
{
    [TestClass]
    [Tag("Input")]
    [Tag("LoopingList")]
    public class LoopingListTests_IndexStateAndOffset_ChangeSnapPosition : LoopingListTestsBase
    {
        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_ChangeSnapPosition_BigSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(25);
            this.loopingList.IsCentered = true;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Near;
            this.loopingList.ItemHeight = 78;
            this.loopingList.SelectedIndex = 10;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, -900);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 10);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 2 && item.LogicalIndex == 10, item.IsSelected);
                    }

                    this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Far;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, -390);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 10);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 2 && item.LogicalIndex == 10, item.IsSelected);
                    }

                    this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Middle;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, -645);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 10);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 2 && item.LogicalIndex == 10, item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_ChangeSnapPosition_SmallSource()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = true;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Near;
            this.loopingList.ItemHeight = 78;
            this.loopingList.SelectedIndex = 2;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, -180);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 2);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 2 && item.LogicalIndex == 2, item.IsSelected);
                    }

                    this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Far;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 330);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 2);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 2 && item.LogicalIndex == 2, item.IsSelected);
                    }

                    this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Middle;
                },
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 75);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 2);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 2 && item.LogicalIndex == 2, item.IsSelected);
                    }
                });
        }
    }
}
