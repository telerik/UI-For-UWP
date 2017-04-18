using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class GetDescriptionsDataCompletedEventArgsTests
    {
        [TestMethod]
        public void Constructor_WhenDescriptionsDataIsNull_ThrowsArgumentNullException()
        {
            try
            {
                var args = new GetDescriptionsDataCompletedEventArgs(null, null, null);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {

            }
        }

        [TestMethod]
        public void Constructor_AssignsDescriptionsDataProperty()
        {
            var expectedDescriptionsData = new FieldInfoData(new ContainerNode("", ContainerNodeRole.Dimension));
            var args = new GetDescriptionsDataCompletedEventArgs(null, null, expectedDescriptionsData);

            var actualDescriptionsData = args.DescriptionsData;

            Assert.AreSame(expectedDescriptionsData, actualDescriptionsData);
        }
    }
}
