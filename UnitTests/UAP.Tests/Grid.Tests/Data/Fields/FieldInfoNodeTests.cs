using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class FieldInfoNodeTests
    {
        [TestMethod]
        public void Contructor_SetsFieldDescriptionProperty()
        {
            var expectedFileInfo = new MockedFieldInfo();
            var infoNode = new FieldInfoNode(expectedFileInfo);

            var actualFieldInfo = infoNode.FieldInfo;

            Assert.AreEqual(expectedFileInfo, actualFieldInfo);
        }
    }
}
