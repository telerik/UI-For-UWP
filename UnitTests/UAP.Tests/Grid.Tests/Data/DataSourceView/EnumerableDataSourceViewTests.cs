using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class EnumerableDataSourceViewTests
    {
        [TestMethod]
        public void Count_ReturnsTheCountOfAllRows()
        {
            var data = Order.GetSmallData();
            var sourceView = new EnumerableDataSourceView(data);
            var expectedNumberOfRows = data.Count;

            var actualNumberOfRows = sourceView.InternalList.Count;

            Assert.AreEqual(expectedNumberOfRows, actualNumberOfRows);
        }

        [TestMethod]
        public void OriginalCollectionIsCopied_AndModificationsToTheOriginalCollectionDoNotChangeTheInternalCollectionOfTheView()
        {
            var data = Order.GetSmallData();
            var sourceView = new EnumerableDataSourceView(data);
            var expectedItemCount = data.Count;

            data.Add(new Order());
            var actualItemCount = sourceView.InternalList.Count;

            Assert.AreEqual(expectedItemCount, actualItemCount);
        }
    }
}