using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public partial class LocalDataSourceProviderTests
    {
        [TestMethod]
        public void GetAggregateDescriptionForFieldDescriptor_WhenFieldDescriptionIsNull_ThrowsArgumentNullException()
        {
            try
            {
                this.provider.GetAggregateDescriptionForFieldDescription(null);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void GetAggregateDescriptionForFieldDescriptor_ReturnsPropertyAggregateDescriptionWithCorrectProperty()
        {
            var expectedPropertyName = "P1";
            var fieldDescriptionInt = new MockedFieldInfo() { DataType = typeof(int), Name = expectedPropertyName };

            var description = (PropertyAggregateDescription)this.provider.GetAggregateDescriptionForFieldDescription(fieldDescriptionInt);

            Assert.AreEqual(expectedPropertyName, description.PropertyName);
        }

        [TestMethod]
        public void GetGroupDescriptionForFieldDescriptor_WhenFieldDescriptionIsNull_ThrowsArgumentNullException()
        {
            try
            {
                this.provider.GetGroupDescriptionForFieldDescription(null);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void GetSortDescriptionForFieldDescriptor_WhenFieldDescriptionIsNull_ThrowsArgumentNullException()
        {
            try
            {
                this.provider.GetSortDescriptionForFieldDescription(null);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void GetGroupDescriptionForFieldDescriptor_WhenFieldDescriptionTypeString_ReturnsPropertyGroupDescriptionWithCorrectPropertyName()
        {
            var expectedPropertyName = "P1";
            var fieldDescriptionInt = new MockedFieldInfo() { DataType = typeof(string), Name = expectedPropertyName };

            var description = (PropertyGroupDescription)this.provider.GetGroupDescriptionForFieldDescription(fieldDescriptionInt);

            Assert.AreEqual(expectedPropertyName, description.PropertyName);
        }
    }
}
