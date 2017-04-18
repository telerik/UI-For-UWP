using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class GenericDataSourceViewTests
    {
        [TestMethod]
        public void Count_WhenItemsSourceIsEnumerable_ReturnsTheCountOfAllRows()
        {
            var data = Order.GetSmallData();
            var sourceView = DataSourceViewFacotry.CreateDataSourceView(data);
            var expectedNumberOfRows = data.Count;

            var actualNumberOfRows = sourceView.InternalList.Count;

            Assert.AreEqual(expectedNumberOfRows, actualNumberOfRows);
        }

        [TestMethod]
        public void Indexer_WhenItemsSourceIsEnumerable_WhenCorrectIndexIsSpecified_ReturnsTheCorrectItem()
        {
            var data = Order.GetSmallData();
            var sourceView = DataSourceViewFacotry.CreateDataSourceView(data);
            var expectedItem = data[2];

            var actualItem = sourceView.InternalList[2];

            Assert.AreSame(expectedItem, actualItem);
        }

        [TestMethod]
        public void OriginalCollectionIsCopied_WhenItemsSourceIsEnumerable_AndModificationsToTheOriginalCollectionDoNotChangeTheInternalCollectionOfTheView()
        {
            var data = Order.GetSmallData();
            var sourceView = DataSourceViewFacotry.CreateDataSourceView(data);
            var expectedItemCount = data.Count;

            data.Add(new Order());
            var actualItemCount = sourceView.InternalList.Count;

            Assert.AreEqual(expectedItemCount, actualItemCount);
        }
    }
}