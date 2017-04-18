using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class ContainerNodeTests
    {
        [TestMethod]
        public void Contructor_SetsDisplayNameProperty()
        {
            var expectedDisplayName = "MyName";
            var infoNode = new ContainerNode("expectedDisplayName", expectedDisplayName, ContainerNodeRole.Dimension);

            var actualDisplayName = infoNode.Caption;

            Assert.AreEqual(expectedDisplayName, actualDisplayName);
        }

        [TestMethod]
        public void Contructor_SetsRoleProperty()
        {
            var expectedRole = ContainerNodeRole.Kpi;
            var infoNode = new ContainerNode("name", "name", expectedRole );

            var actualRole = infoNode.Role;

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void HasChildren_WhenNewInstanceIsCreated_ReturnsFalse()
        {
            var infoNode = new ContainerNode("MyName", "MyName", ContainerNodeRole.Dimension);

            Assert.IsFalse(infoNode.HasChildren);
        }

        [TestMethod]
        public void HasChildren_WhenItemHsBeenAddedToChildrenCollection_ReturnsTrue()
        {
            var infoNode = new ContainerNode("MyName", "MyName", ContainerNodeRole.Dimension);

            infoNode.Children.Add(new ContainerNode("child", "child", ContainerNodeRole.Kpi));

            Assert.IsTrue(infoNode.HasChildren);
        }
    }
}
