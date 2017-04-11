using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.UI.Xaml.Controls.Input.Tests;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using WinRT.Testing;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Tests
{
    [TestClass]
    [Tag("LoopingList")]
    [Tag("Input")]
    public class LoopingListTests_Ported : LoopingListTestsBase
    {
        [TestMethod]
        public void LoopingList_BindingAndItemPopulationTest()
        {
            this.loopingList.ItemsSource = this.CreateSource(10);
            double itemHeight = this.loopingList.ItemHeight + this.loopingList.ItemSpacing * 2;

            this.CreateAsyncTest(this.loopingList, () =>
            {
                var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                var visibleItems = (int)Math.Floor(loopingPanel.AvailableLength / itemHeight + 2);

                Assert.AreEqual<int>(loopingPanel.Children.Count, visibleItems, "Incorrect loopingPanel child count.");
                Assert.AreEqual<int>(loopingPanel.VisualCount, visibleItems, "Incorrect loopingPanel VisualCount.");
            });
        }

        [TestMethod]
        public void LoopingList_BindingAndItemTextTest()
        {
            this.loopingList.IsCentered = false;
            this.loopingList.ItemsSource = this.CreateSource(10);

            this.CreateAsyncTest(this.loopingList, () =>
            {
                var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                int counter = 0;
                var itemsCount = this.loopingList.ItemsSource.Count;

                for (var i = 0; i < loopingPanel.VisualIndexChain.Count; i++)
                {
                    int index = counter++ % itemsCount;
                    Assert.AreEqual<int>(index, i);
                }
            });
        }

        [TestMethod]
        public void LoopingList_ScrollingAndItemMappingTest()
        {
            this.loopingList.IsCentered = false;
            this.loopingList.ItemsSource = this.CreateSource(10);
            var itemLength = this.loopingList.ItemHeight + this.loopingList.ItemSpacing * 2;
            this.CreateAsyncTest(this.loopingList, () =>
            {
                var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                for (int i = 0; i < 100; i++)
                {
                    int index = Math.Abs(i % this.loopingList.ItemsSource.Count);
                    Assert.AreEqual<int>(index, loopingPanel.TopLogicalIndex);

                    loopingPanel.UpdateWheel(this.loopingList.VerticalOffset - itemLength, false);
                }

                loopingPanel.UpdateWheel(0, false);

                for (int i = 0; i > -100; i--)
                {
                    int index = Math.Abs(i % this.loopingList.ItemsSource.Count);
                    Assert.AreEqual<int>(index, loopingPanel.TopLogicalIndex);

                    loopingPanel.UpdateWheel(this.loopingList.VerticalOffset - itemLength, false);
                }
            });
        }

        [TestMethod]
        public void LoopingList_OffsetClampingTest_IsLoopingEnabledFalseIsCenteredFalse()
        {
            this.loopingList.IsCentered = false;
            this.loopingList.IsLoopingEnabled = false;
            this.loopingList.ItemsSource = this.CreateSource(10);

            var itemLength = this.loopingList.ItemHeight + this.loopingList.ItemSpacing * 2;
            var wheelHeight = this.loopingList.ItemsSource.Count * itemLength;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    loopingPanel.Scroll(10);
                },
                () =>
                {
                    Assert.AreEqual<double>(0.0, this.loopingList.VerticalOffset);
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);

                    var scrollableHeight = loopingPanel.AvailableLength - wheelHeight;
                    loopingPanel.Scroll(scrollableHeight - 0.5);
                },
                () =>
                {
                    var scrollableHeight = this.loopingList.itemsPanel.AvailableLength - wheelHeight;
                    Assert.AreEqual<double>(scrollableHeight, this.loopingList.VerticalOffset);
                });
        }

        [TestMethod]
        public void LoopingList_IsCenteredAndSelectedItemPositionTest()
        {
            this.loopingList.IsCentered = true;
            this.loopingList.IsLoopingEnabled = true;
            this.loopingList.ItemsSource = this.CreateSource(10);

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    for (var i = 0; i < 5; i++)
                    {
                        this.loopingList.SelectedIndex = i;
                        foreach (LoopingListItem item in loopingPanel.Children)
                        {
                            if (item.IsSelected)
                            {
                                var verticalOffset = item.VerticalOffset;
                                Assert.AreEqual<double>((loopingPanel.AvailableLength - loopingPanel.ItemLength) / 2, verticalOffset);
                            }
                        }
                    }
                });
        }

        [TestMethod]
        public void LoopingList_IsCenteredAndLoopingDisabledTest()
        {
            this.loopingList.IsCentered = true;
            this.loopingList.IsLoopingEnabled = false;
            this.loopingList.ItemsSource = this.CreateSource(3);
            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    var middleItem = loopingPanel.FindMiddleItem();
                    var itemOffset = middleItem.VerticalOffset;
                    Assert.AreEqual<double>((loopingPanel.AvailableLength - loopingPanel.ItemLength) / 2, itemOffset);
                });
        }

        [TestMethod]
        public void LoopingList_MaxOffsetTest_IsCenteredLogicalLengthSmallerThanAvailable()
        {
            this.loopingList.IsCentered = true;
            this.loopingList.IsLoopingEnabled = false;
            this.loopingList.ItemsSource = this.CreateSource(3);
            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    var children = loopingPanel.Children;
                    var firstItemFound = false;
                    loopingPanel.UpdateWheel(double.MinValue, false);
                    foreach (var itemIndex in loopingPanel.VisualIndexChain)
                    {
                        var item = children[itemIndex] as LoopingListItem;
                        if (item.IsEmpty)
                        {
                            continue;
                        }
                        else if (!firstItemFound)
                        {
                            var itemOffset = item.VerticalOffset;
                            Assert.AreEqual<double>(loopingPanel.MaxOffset, itemOffset);
                            firstItemFound = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                });
        }

        [TestMethod]
        public void LoopingList_MaxOffsetTest_NotCenteredLogicalLengthSmallerThenAvailable()
        {
            this.loopingList.IsCentered = false;
            this.loopingList.IsLoopingEnabled = false;
            this.loopingList.ItemsSource = this.CreateSource(3);

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    loopingPanel.UpdateWheel(double.MinValue, false);
                    Assert.AreEqual<double>(loopingPanel.MaxOffset, loopingPanel.visualOffset);
                    Assert.AreEqual<double>(loopingPanel.MinOffset, loopingPanel.visualOffset);
                });
        }

        [TestMethod]
        public void LoopingList_MaxOffsetTest_IsCenteredLogicalLengthBiggerThenAvailable()
        {
            this.loopingList.IsCentered = true;
            this.loopingList.IsLoopingEnabled = false;
            this.loopingList.ItemsSource = this.CreateSource(30);
            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    loopingPanel.UpdateWheel(double.MinValue, false);
                    var middleItem = loopingPanel.FindMiddleItem();
                    Assert.AreEqual<int>(29, middleItem.LogicalIndex);
                });
        }

        [TestMethod]
        public void LoopingList_EmptyItemsAreHiddenTest()
        {
            this.loopingList.IsCentered = false;
            this.loopingList.IsLoopingEnabled = false;
            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    foreach (LoopingListItem item in loopingPanel.Children)
                    {
                        if (item.IsEmpty)
                        {
                            Assert.IsTrue(item.Visibility == Visibility.Collapsed);
                        }
                        else
                        {
                            Assert.IsTrue(item.Visibility == Visibility.Visible);
                        }
                    }
                });
        }

        [TestMethod]
        public void LoopingList_SelectedIndex_IsDeferred()
        {
            this.loopingList.ItemsSource = this.CreateSource(10);
            this.loopingList.SelectedIndex = 8;

            Assert.AreEqual<int>(8, this.loopingList.SelectedIndex);

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<int>(8, this.loopingList.SelectedIndex);
                });
        }

        //[TestMethod]
        //[Asynchronous]
        //public void LoopingList_InfiniteVerticalSpaceAndSizeTest()
        //{
        //    this.TestPanel.Children.Clear();
        //    this.loopingList.ItemsSource = this.CreateSource(10);
        //    StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
        //    stackPanel.Children.Add(this.loopingList);
        //    this.TestPanel.Children.Add(stackPanel);
        //    this.EnqueueCallback(()=>
        //        {
        //            Assert.AreEqual<double>(this.loopingList.DesiredSize.Height, (this.loopingList.ItemHeight + this.loopingList.ItemSpacing * 2) * 3);
        //            this.TestComplete();
        //        });
        //}

        //[TestMethod]
        //[Asynchronous]
        //public void InfiniteHorizontalSpaceAndSizeTest()
        //{
        //    this.TestPanel.Children.Clear();
        //    this.loopingList.Orientation = Orientation.Horizontal;
        //    this.loopingList.DataSource = this.dataSource;
        //    StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        //    stackPanel.Children.Add(this.loopingList);
        //    this.TestPanel.Children.Add(stackPanel);
        //    this.EnqueueCallback(() =>
        //    {
        //        Assert.AreEqual<double>(this.loopingList.DesiredSize.Width, (this.loopingList.ItemWidth + this.loopingList.ItemSpacing * 2) * 3);
        //        this.TestComplete();
        //    });
        //}
        [TestMethod]
        public void LoopingList_TestSettingIsLoopingEnabledWhenLoadedWithLessItemsThanViewport()
        {
            this.loopingList.ItemsSource = this.CreateSource(3);
            this.loopingList.IsLoopingEnabled = false;
            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    Assert.AreEqual<int>(2, this.loopingList.SelectedIndex);

                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    for (var i = 0; i < loopingPanel.Children.Count; i++)
                    {
                        var item = loopingPanel.Children[i] as LoopingListItem;
                        if (i > loopingPanel.LogicalCount - 1)
                        {
                            Assert.IsTrue(item.IsEmpty);
                            Assert.AreEqual<Visibility>(Visibility.Collapsed, item.Visibility);
                        }
                        else
                        {
                            Assert.IsFalse(item.IsEmpty);
                            Assert.AreEqual<Visibility>(Visibility.Visible, item.Visibility);
                        }
                    }

                    this.loopingList.IsLoopingEnabled = true;
                },
                () =>
                {
                    Assert.AreEqual<int>(2, this.loopingList.SelectedIndex);

                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    for (var i = 0; i < loopingPanel.Children.Count; i++)
                    {
                        var item = loopingPanel.Children[i] as LoopingListItem;
                        Assert.IsFalse(item.IsEmpty);
                        Assert.AreEqual<Visibility>(Visibility.Visible, item.Visibility);
                    }

                    loopingPanel.IsLoopingEnabled = false;
                }, () =>
                {
                    Assert.AreEqual<int>(2, this.loopingList.SelectedIndex);

                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    Assert.AreEqual<double>(-75.0, loopingPanel.visualOffset);

                    for (var i = 0; i < loopingPanel.Children.Count; i++)
                    {
                        var item = loopingPanel.Children[i] as LoopingListItem;
                        if (i > loopingPanel.LogicalCount - 1)
                        {
                            Assert.IsTrue(item.IsEmpty);
                            Assert.AreEqual<Visibility>(Visibility.Collapsed, item.Visibility);
                        }
                        else
                        {
                            Assert.IsFalse(item.IsEmpty);
                            Assert.AreEqual<Visibility>(Visibility.Visible, item.Visibility);
                        }
                    }
                });
        }

        [TestMethod]
        public void LoopingList_TestTogglingIsCenteredWhenLogicalLengthSmallerThanAvailable()
        {
            this.loopingList.ItemsSource = this.CreateSource(3);
            this.loopingList.IsLoopingEnabled = false;

            this.CreateAsyncTest(this.loopingList,
                () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);
                    for (var i = 0; i < loopingPanel.Children.Count; i++)
                    {
                        var item = loopingPanel.Children[i] as LoopingListItem;
                        if (i > loopingPanel.LogicalCount - 1)
                        {
                            Assert.IsTrue(item.IsEmpty);
                            Assert.AreEqual<Visibility>(Visibility.Collapsed, item.Visibility);
                        }
                        else
                        {
                            Assert.IsFalse(item.IsEmpty);
                            Assert.AreEqual<Visibility>(Visibility.Visible, item.Visibility);
                        }
                    }

                    this.loopingList.IsCentered = true;
                }, () =>
                {
                    var loopingPanel = ElementTreeHelper.FindVisualDescendant<LoopingPanel>(this.loopingList);

                    for (var i = 0; i < loopingPanel.Children.Count; i++)
                    {
                        var item = loopingPanel.Children[i] as LoopingListItem;
                        if (this.loopingList.SelectedIndex == item.LogicalIndex)
                        {
                            var expectedOffset = (loopingPanel.AvailableLength - loopingPanel.ItemLength) / 2;
                            Assert.AreEqual<double>(expectedOffset, item.VerticalOffset);
                        }
                    }

                    this.loopingList.IsCentered = false;
                }, () =>
                {
                    Assert.AreEqual<double>(0, this.loopingList.itemsPanel.visualOffset);
                });
        }
    }
}