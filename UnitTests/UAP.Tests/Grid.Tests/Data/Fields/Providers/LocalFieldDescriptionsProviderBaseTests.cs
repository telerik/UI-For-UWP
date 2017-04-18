using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class LocalFieldDescriptionsProviderBaseTests
    {
        [TestMethod]
        public void GetFieldDescriptionHierarchy_WhenNullDescriptionsArePassed_ThrowsArgumentNullException()
        {
            var provider = new InheritedLocalFieldDescriptionsProviderBase();

            try
            {
                provider.ExposedGetFieldDescriptionHierarchy(null);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void GetFieldDescriptionHierarchy_WhenEmptyEnumerableIsPassed_ReturnsRootFieldNodeWithNoChildren()
        {
            var provider = new InheritedLocalFieldDescriptionsProviderBase();
            var expectedNumberOfChildren = 0;

            var rootNode = provider.ExposedGetFieldDescriptionHierarchy(new List<IDataFieldInfo>() { });
            var actualNumberOfChildren = rootNode.Children.Count;

            Assert.AreEqual(expectedNumberOfChildren, actualNumberOfChildren);
        }

        [TestMethod]
        public void GetFieldDescriptionHierarchy_WhenEnumerableWithValidDescriptionsIsPassed_ReturnsRootNodeWithFieldInfoNodeForEachFieldDescription()
        {
            var provider = new InheritedLocalFieldDescriptionsProviderBase();
            var expectedNumberOfChildren = 2;

            var rootNode = provider.ExposedGetFieldDescriptionHierarchy(new List<IDataFieldInfo>()
            {
                new MockedFieldInfo(),
                new MockedFieldInfo()
            });

            var actualNumberOfChildren = rootNode.Children.Count;

            Assert.AreEqual(expectedNumberOfChildren, actualNumberOfChildren);
        }

        [TestMethod]
        public void GetFieldDescriptionHierarchy_WhenEnumerableWithValidDescriptionsIsPassed_ReturnsRootNodeWithSelectableFieldInfoNodes()
        {
            var provider = new InheritedLocalFieldDescriptionsProviderBase();

            var rootNode = provider.ExposedGetFieldDescriptionHierarchy(new List<IDataFieldInfo>()
            {
                new MockedFieldInfo(),
                new MockedFieldInfo()
            });

            var allChildrenAreSelectable = rootNode.Children.All(inf => inf.Role == ContainerNodeRole.Selectable);
            Assert.IsTrue(allChildrenAreSelectable);
        }

        internal class InheritedLocalFieldDescriptionsProviderBase : LocalFieldDescriptionsProviderBase
        {
            public override bool IsReady
            {
                get
                {
                    return true;
                }
            }
           
            internal override bool IsDynamic
            {
                get
                {
                    return false;
                }
            }

            public ContainerNode ExposedGetFieldDescriptionHierarchy(IEnumerable<IDataFieldInfo> descriptions)
            {
                return this.GetFieldDescriptionHierarchy(descriptions);
            }

            protected override IFieldInfoData GenerateDescriptionsData()
            {
                throw new NotImplementedException();
            }
        }
    }
}