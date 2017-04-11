using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.UI.Xaml.Controls;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.SegmentedControl
{
    [TestClass]
    [Tag("Input")]
    [Tag("SegmentedControl")]
    public class SegmentedControlSelectionChangedEventTests : SegmentedControlTestsBase
    {
        private SelectionChangedEventArgs selectionChangedContext;

        [TestMethod]
        public void SegmentedControlTests_SelectionChanged_RemoveSelectedItem()
        {
            var source = GetDataSource(10);
            this.segmentedControl.ItemsSource = source;

            this.segmentedControl.SelectedItem = source[2];
            var removedItem = source[2];
            source.RemoveAt(2);

            Assert.AreEqual(removedItem, selectionChangedContext.RemovedItems[0]);
            Assert.AreEqual(0, selectionChangedContext.AddedItems.Count);

            this.segmentedControl.SelectedItem = source[2];

            CreateAsyncTest(segmentedControl, () =>
            {
                removedItem = source[2];
                source.RemoveAt(2);
            },
            () =>
            {
                Assert.AreEqual(removedItem, selectionChangedContext.RemovedItems[0]);
                Assert.AreEqual(0, selectionChangedContext.AddedItems.Count);
            });
        }

        [TestMethod]
        public void SegmentedControlTests_SelectionChanged_SetValueBeforeLoaded()
        {
            var source = GetDataSource(10);

            this.segmentedControl.ItemsSource = source;

            this.segmentedControl.SelectedValue = "wrong value";

            Assert.AreEqual(null, selectionChangedContext);

            this.segmentedControl.SelectedValuePath = "Value";
            this.segmentedControl.SelectedValue = source[5].Value;

            Assert.AreEqual(0, selectionChangedContext.RemovedItems.Count);
            Assert.AreEqual(source[5], selectionChangedContext.AddedItems[0]);

            this.segmentedControl.SelectedItem = source[2];

            Assert.AreEqual(source[5], selectionChangedContext.RemovedItems[0]);
            Assert.AreEqual(source[2], selectionChangedContext.AddedItems[0]);

            CreateAsyncTest(segmentedControl, () =>
            {
                (segmentedControl.ItemsControl.ContainerFromIndex(4) as Segment).IsSelected = true;
            },
            () =>
            {
                Assert.AreEqual(source[2], selectionChangedContext.RemovedItems[0]);
                Assert.AreEqual(source[4], selectionChangedContext.AddedItems[0]);

                segmentedControl.SelectedItem = null;
            },
            () =>
            {
                Assert.AreEqual(source[4], selectionChangedContext.RemovedItems[0]);
                Assert.AreEqual(0, selectionChangedContext.AddedItems.Count);
            });
        }

        public override void TestCleanup()
        {
            base.TestCleanup();
            segmentedControl.SelectionChanged -= SegmentedControl_SelectionChanged;
            selectionChangedContext = null;
        }

        public override void TestInitialize()
        {
            base.TestInitialize();
            segmentedControl.SelectionChanged += SegmentedControl_SelectionChanged;
        }

        private void SegmentedControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectionChangedContext = e;
        }
    }
}