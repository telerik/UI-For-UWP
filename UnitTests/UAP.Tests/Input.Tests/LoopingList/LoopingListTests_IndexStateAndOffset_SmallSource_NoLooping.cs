using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Tests
{
    [TestClass]
    [Tag("Input")]
    [Tag("LoopingList")]
    public class LoopingListTests_IndexStateAndOffset_SmallSource_NoLooping : LoopingListTestsBase
    {
        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredTrueNearSmallSourceNoLooping()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = true;
            this.loopingList.IsLoopingEnabled = false;
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
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredTrueMiddleSmallSourceNoLooping()
        {
            this.loopingList.ItemsSource = this.CreateSource(3);
            this.loopingList.IsCentered = true;
            this.loopingList.IsLoopingEnabled = false;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Middle;
            this.loopingList.ItemHeight = 78;
            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 255);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 0);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 0 && item.LogicalIndex == 0, item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredTrueFarSmallSourceNoLooping()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = true;
            this.loopingList.IsLoopingEnabled = false;
            this.loopingList.CenteredItemSnapPosition = LoopingListItemSnapPosition.Far;
            this.loopingList.ItemHeight = 83;
            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<double>(this.loopingList.VerticalOffset, 505);
                    Assert.AreEqual(this.loopingList.SelectedIndex, 0);
                    foreach (LoopingListItem item in this.loopingList.itemsPanel.Children)
                    {
                        Assert.AreEqual(item.VisualIndex == 0 && item.LogicalIndex == 0, item.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestIndexStateAndOffset_IsCenteredFalseNearSmallSourceNoLooping()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = false;
            this.loopingList.IsLoopingEnabled = false;
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
        public void LoopingList_TestIndexStateAndOffset_IsCenteredFalseMiddleSmallSourceNoLooping()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = false;
            this.loopingList.IsLoopingEnabled = false;
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
        public void LoopingList_TestIndexStateAndOffset_IsCenteredFalseFarSmallSourceNoLooping()
        {
            this.loopingList.ItemsSource = this.CreateSource(4);
            this.loopingList.IsCentered = false;
            this.loopingList.IsLoopingEnabled = false;
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
