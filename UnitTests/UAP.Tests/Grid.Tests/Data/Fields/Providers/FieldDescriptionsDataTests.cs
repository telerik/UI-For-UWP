using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class FieldDescriptionsDataTests
    {
        private FieldInfoData CreateDescriptionsDataWithFieldDescriptionsHierarchy()
        {
            var expectedRoot = new ContainerNode("Node", "Node", ContainerNodeRole.None);

            var fieldDescription = new MockedFieldInfo() { Name = "PropertyInDimension" };
            var fieldDescription2 = new MockedFieldInfo() { Name = "StandAloneProperty" };

            var dimensionInfo = new ContainerNode("Dimension1", "Dimension1", ContainerNodeRole.Dimension);
            var propertyInDimensionInfo = new FieldInfoNode(fieldDescription);

            dimensionInfo.Children.Add(propertyInDimensionInfo);

            var standAlonePropertyInfo = new FieldInfoNode(fieldDescription2);

            expectedRoot.Children.Add(dimensionInfo);
            expectedRoot.Children.Add(standAlonePropertyInfo);

            var descriptionsData = new FieldInfoData(expectedRoot);

            return descriptionsData;
        }

        [TestMethod]
        public void Constructor_WhenNullRootIsPassed_ThrowsArgumentNullException()
        {
            try
            {
                var descriptionsData = new FieldInfoData(null);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {

            }
        }

        [TestMethod]
        public void Constructor_AssignsRootFieldInfo()
        {
            var expectedRoot = new ContainerNode("Node", ContainerNodeRole.None);
            var descriptionsData = new FieldInfoData(expectedRoot);

            var actualRoot = descriptionsData.RootFieldInfo;

            Assert.AreSame(expectedRoot, actualRoot);
        }

        [TestMethod]
        public void GetFieldDescriptionByMember_WhenMemberNameIsNull_ReturnsNull()
        {
            var descriptionsData = this.CreateDescriptionsDataWithFieldDescriptionsHierarchy();

            var fieldDescription = descriptionsData.GetFieldDescriptionByMember(null);

            Assert.IsNull(fieldDescription);
        }

        [TestMethod]
        public void GetFieldDescriptionByMember_WhenMemberNameDoesNotExist_ReturnsNull()
        {
            var descriptionsData = this.CreateDescriptionsDataWithFieldDescriptionsHierarchy();

            var fieldDescription = descriptionsData.GetFieldDescriptionByMember("NonExistentProperty");

            Assert.IsNull(fieldDescription);
        }

        [TestMethod]
        public void GetFieldDescriptionByMember_WhenMemberExists_ReturnsCorrectFieldDescription()
        {
            var descriptionsData = this.CreateDescriptionsDataWithFieldDescriptionsHierarchy();
            var expectedFieldDescriptionName = "StandAloneProperty";

            var fieldDescription = descriptionsData.GetFieldDescriptionByMember("StandAloneProperty");
            var actualFieldDescrioptionName = fieldDescription.Name;
            
            Assert.AreEqual(expectedFieldDescriptionName, actualFieldDescrioptionName);
        }

        [TestMethod]
        public void GetFieldDescriptionByMember_WhenMemberExists_WhenFieldDescriptionIsContainedInAHierarchy_ReturnsCorrectFieldDescription()
        {
            var descriptionsData = this.CreateDescriptionsDataWithFieldDescriptionsHierarchy();
            var expectedFieldDescriptionName = "PropertyInDimension";

            var fieldDescription = descriptionsData.GetFieldDescriptionByMember("PropertyInDimension");
            var actualFieldDescrioptionName = fieldDescription.Name;

            Assert.AreEqual(expectedFieldDescriptionName, actualFieldDescrioptionName);
        }
    }
}
