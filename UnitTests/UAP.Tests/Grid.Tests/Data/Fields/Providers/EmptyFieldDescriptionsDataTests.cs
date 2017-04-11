using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class EmptyFieldDescriptionsDataTests
    {
        [TestMethod]
        public void RootFieldInfo_ContainsNoChildren()
        {
            var expectedNumberOfChildren = 0;
            var descriptionsData = new EmptyFieldInfoData();

            var actualNumberOfChildren = descriptionsData.RootFieldInfo.Children.Count;

            Assert.AreEqual(expectedNumberOfChildren, actualNumberOfChildren);
        }

        [TestMethod]
        public void GetFieldDescriptionByMember_ReturnsNull()
        {
            var descriptionsData = new EmptyFieldInfoData();

            var fieldDescription = descriptionsData.GetFieldDescriptionByMember("ff");

            Assert.IsNull(fieldDescription);
        }
    }
}
