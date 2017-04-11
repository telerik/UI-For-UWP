using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Core;
using Windows.UI.Xaml.Controls;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.SegmentedControl
{
    [TestClass]
    [Tag("Input")]
    [Tag("SegmentedControl")]
    public class SegmentedControlSelectionTests : SegmentedControlTestsBase
    {
        [TestMethod]
        public void SegmentedControlTests_SelectionInitialSetup_SetSelectedItem()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;
            this.segmentedControl.SelectedItem = source[3];

            Assert.AreEqual(source[3], segmentedControl.SelectedItem);
            Assert.AreEqual(3, segmentedControl.SelectedIndex);
            Assert.AreEqual(source[3], segmentedControl.SelectedValue);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    foreach (var item in source)
                    {
                        var segment = this.segmentedControl.ItemsControl.ContainerFromItem(item) as Segment;

                        Assert.AreEqual(source[3].Equals(item), segment.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void SegmentedControlTests_SelectionInitialSetup_SetSelectedIndex()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;
            this.segmentedControl.SelectedIndex = 3;

            Assert.AreEqual(source[3], segmentedControl.SelectedItem);
            Assert.AreEqual(3, segmentedControl.SelectedIndex);
            Assert.AreEqual(source[3], segmentedControl.SelectedValue);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    foreach (var item in source)
                    {
                        var segment = this.segmentedControl.ItemsControl.ContainerFromItem(item) as Segment;

                        Assert.AreEqual(source[3].Equals(item), segment.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void SegmentedControlTests_SelectionInitialSetup_SetSelectedValue_NoValuePath()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;
            this.segmentedControl.SelectedValue = source[3];

            Assert.AreEqual(source[3], segmentedControl.SelectedItem);
            Assert.AreEqual(3, segmentedControl.SelectedIndex);
            Assert.AreEqual(source[3], segmentedControl.SelectedValue);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    foreach (var item in source)
                    {
                        var segment = this.segmentedControl.ItemsControl.ContainerFromItem(item) as Segment;

                        Assert.AreEqual(source[3].Equals(item), segment.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void SegmentedControlTests_SelectionInitialSetup_SetSelectedValue_ValuePath()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;

            this.segmentedControl.SelectedValuePath = "Value";
            this.segmentedControl.SelectedValue = source[3].Value;

            Assert.AreEqual(source[3], segmentedControl.SelectedItem);
            Assert.AreEqual(3, segmentedControl.SelectedIndex);
            Assert.AreEqual(source[3].Value, segmentedControl.SelectedValue);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    foreach (var item in source)
                    {
                        var segment = this.segmentedControl.ItemsControl.ContainerFromItem(item) as Segment;

                        Assert.AreEqual(source[3].Equals(item), segment.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void SegmentedControlTests_SelectionInitialSetup_ChangePropertiesBeforeControlLoaded()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;

            this.segmentedControl.SelectedItem = source[2];
            Assert.AreEqual(source[2], segmentedControl.SelectedValue);
            Assert.AreEqual(2, segmentedControl.SelectedIndex);

            this.segmentedControl.SelectedIndex = 1;
            Assert.AreEqual(source[1], segmentedControl.SelectedItem);
            Assert.AreEqual(source[1], segmentedControl.SelectedValue);

            this.segmentedControl.SelectedValue = source[5];
            Assert.AreEqual(source[5], segmentedControl.SelectedItem);
            Assert.AreEqual(5, segmentedControl.SelectedIndex);
        }

        [TestMethod]
        public void SegmentedControlTests_SelectionInitialSetup_SetValuePathAfterValue()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;

            this.segmentedControl.SelectedValue = source[5];
            this.segmentedControl.SelectedValuePath = "Value";

            Assert.AreEqual(null, segmentedControl.SelectedItem);
            Assert.AreEqual(null, segmentedControl.SelectedValue);
            Assert.AreEqual(-1, segmentedControl.SelectedIndex);

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    foreach (var item in source)
                    {
                        var segment = this.segmentedControl.ItemsControl.ContainerFromItem(item) as Segment;
                        Assert.IsFalse(segment.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void SegmentedControlTests_SelectionInitialSetup_SetValuePathBeforeValue()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;

            this.segmentedControl.SelectedValuePath = "Value";
            this.segmentedControl.SelectedValue = 5;

            Assert.AreEqual(source[5], segmentedControl.SelectedItem);
            Assert.AreEqual(5, segmentedControl.SelectedValue);
            Assert.AreEqual(5, segmentedControl.SelectedIndex);
        }

        [TestMethod]
        public void SegmentedControlTests_ClearSelection()
        {
            var source = GetDataSource(10);
            segmentedControl.ItemsSource = source;
            segmentedControl.SelectedValuePath = "Value";

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    var segment = this.segmentedControl.ItemsControl.ContainerFromIndex(3) as Segment;
                    segment.IsSelected = true;

                }, () =>
                {
                    Assert.AreEqual(source[3], segmentedControl.SelectedItem);
                    Assert.AreEqual(3, segmentedControl.SelectedIndex);
                    Assert.AreEqual(source[3].Value, segmentedControl.SelectedValue);

                    segmentedControl.SelectedItem = null;
                }, () =>
                {
                    Assert.AreEqual(null, segmentedControl.SelectedItem);
                    Assert.AreEqual(-1, segmentedControl.SelectedIndex);
                    Assert.AreEqual(null, segmentedControl.SelectedValue);

                    foreach (Segment segment in this.segmentedControl.ItemsControl.ItemsPanelRoot.Children)
                    {
                        Assert.IsFalse(segment.IsSelected);
                    }
                });
        }

        [TestMethod]
        public void SegmentedControlTests_CheckItem()
        {
            var source = GetDataSource(10);
            segmentedControl.ItemsSource = source;
            segmentedControl.SelectedValuePath = "Value";

            this.CreateAsyncTest(this.segmentedControl,
                () =>
                {
                    var segment = this.segmentedControl.ItemsControl.ContainerFromIndex(3) as Segment;
                    segment.IsSelected = true;

                }, () =>
                {
                    Assert.AreEqual(source[3], segmentedControl.SelectedItem);
                    Assert.AreEqual(3, segmentedControl.SelectedIndex);
                    Assert.AreEqual(source[3].Value, segmentedControl.SelectedValue);

                    var segment = this.segmentedControl.ItemsControl.ContainerFromIndex(5) as Segment;
                    segment.IsSelected = true;
                }, () =>
                {
                    var segment = this.segmentedControl.ItemsControl.ContainerFromIndex(3) as Segment;

                    Assert.IsFalse(segment.IsSelected);

                    Assert.AreEqual(source[5], segmentedControl.SelectedItem);
                    Assert.AreEqual(5, segmentedControl.SelectedIndex);
                    Assert.AreEqual(source[5].Value, segmentedControl.SelectedValue);
                });
        }
    }
}
