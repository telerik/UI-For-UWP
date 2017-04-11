using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.SegmentedControl
{
    [TestClass]
    [Tag("Input")]
    [Tag("SegmentedControl")]
    public class SegmentedControlSourceTests : SegmentedControlTestsBase
    {
        [TestMethod]
        public void SegmentedControlTests_AddItemToItems()
        {
            AddItems(items);

            this.segmentedControl.ItemsSource = GetDataSource(10);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    try
                    {
                        // Setting ItemsSource makes the Items collection readonly.
                        this.segmentedControl.Items.Add(new Data { });
                    }
                    catch (Exception ex)
                    {
                        Assert.AreEqual("Catastrophic failure (Exception from HRESULT: 0x8000FFFF (E_UNEXPECTED))", ex.Message);
                    }
                }, () =>
                {
                    this.segmentedControl.ItemsSource = null;
                    this.segmentedControl.Items.Add(1);
                }, () =>
                {
                    Assert.AreEqual(5, this.segmentedControl.Items.Count);
                });
        }

        [TestMethod]
        public void SegmentedControlTests_AddItemToItemsSource()
        {
            this.segmentedControl.ItemsSource = GetDataSource(10);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    (this.segmentedControl.ItemsSource as IList).Add(new Data { });
                }, () =>
                {
                    Assert.AreEqual(11, this.segmentedControl.Items.Count);
                });
        }

        [TestMethod]
        public void SegmentedControlTests_AddItemToItemsWhenNotReadonly_NoItemsInitially()
        {
            this.segmentedControl.ItemsSource = GetDataSource(10);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    try
                    {
                        // Setting ItemsSource makes the Items collection readonly.
                        this.segmentedControl.Items.Add(new Data { });
                    }
                    catch (Exception ex)
                    {
                        Assert.AreEqual("Catastrophic failure (Exception from HRESULT: 0x8000FFFF (E_UNEXPECTED))", ex.Message);
                    }
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SegmentedControlTests_AddNullToItems_NoItemsSource()
        {
            AddItems(items);
            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    this.segmentedControl.Items.Add(null);
                });
        }

        [TestMethod]
        public void SegmentedControlTests_GeneratedItems_SetItemsSourceInitially()
        {
            this.segmentedControl.ItemsSource = GetDataSource(10);

            AddItems(items);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    Assert.AreEqual(10, this.segmentedControl.ItemsControl.Items.Count);

                    var i = 0;
                    foreach (var item in this.segmentedControl.ItemsSource)
                    {
                        Assert.AreEqual(item, this.segmentedControl.ItemsControl.Items[i++]);
                    }

                    this.segmentedControl.ItemsSource = null;
                }, () =>
                {
                    Assert.AreEqual(4, this.segmentedControl.Items.Count);
                    this.segmentedControl.Items.Add("item");
                    // no exception should be thrown
                });
        }

        [TestMethod]
        public void SegmentedControlTests_GeneratedItems_SetItemsSourceLater()
        {
            AddItems(items);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    Assert.AreEqual(items.Count, this.segmentedControl.ItemsControl.Items.Count);

                    var i = 0;
                    foreach (var item in items)
                    {
                        var expectedItem = this.segmentedControl.ItemsControl.Items[i++];
                        Assert.AreEqual(item, expectedItem);
                    }

                    this.segmentedControl.ItemsSource = GetDataSource(10);
                }, () =>
                {
                    Assert.AreEqual(10, this.segmentedControl.Items.Count);
                    this.segmentedControl.ItemsSource = null;
                }, () =>
                {
                    Assert.AreEqual(4, this.segmentedControl.Items.Count);
                });
        }

        [TestMethod]
        public void SegmentedControlTests_NoItemsSource_AddItemToItems_NoException()
        {
            AddItems(items);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    this.segmentedControl.Items.Add("item");
                    // no exception should be thrown here
                });
        }
    }
}