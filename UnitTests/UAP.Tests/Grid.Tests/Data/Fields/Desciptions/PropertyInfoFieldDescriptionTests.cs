using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class PropertyInfoFieldDescriptionTests
    {
        private IList<Order> data;

        [TestInitialize]
        public void TestInitialize()
        {
            this.data = Order.GetData();
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            var propertyInfo = typeof(Order).GetRuntimeProperty(propertyName);

            return propertyInfo;
        }

        private Func<object, object> GetPropertyAccess(string propertyName)
        {
            var propertyAcces = BindingExpressionHelper.CreateGetValueFunc(typeof(Order), propertyName);

            return propertyAcces;
        }

        [TestMethod]
        public void Constructor_WhenNullPropertyInfoIsPassed_ThrowsArgumentNullException()
        {
            try
            {
                var desciption = new PropertyInfoFieldInfo(null, (o) => o);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {

            }
        }

        [TestMethod]
        public void Constructor_WhenNullPropertyAccessIsPassed_ThrowsArgumentNullException()
        {
            try
            {
                var propertyInfo = this.GetPropertyInfo("Product");
                var desciption = new PropertyInfoFieldInfo(propertyInfo, null);

                Assert.Fail("ArgumentNullException expected.");
            }
            catch (ArgumentNullException)
            {

            }
        }

        [TestMethod]
        public void DisplayName_ReturnsTheNamePropertyOfThePropertyInfo()
        {
            var propertyInfo = this.GetPropertyInfo("Product");
            var desciption = new PropertyInfoFieldInfo(propertyInfo, (o) => o);
            var expectedDisplayName = propertyInfo.Name;

            var actualDisplayName = desciption.DisplayName;

            Assert.AreEqual(expectedDisplayName, actualDisplayName);
        }

        [TestMethod]
        public void DataType_ReturnsThePropertyTypeOfThePropertyInfo()
        {
            var propertyInfo = this.GetPropertyInfo("Product");
            var desciption = new PropertyInfoFieldInfo(propertyInfo, (o) => o);
            var expectedDataType = propertyInfo.PropertyType;

            var actualDataType = desciption.DataType;

            Assert.AreEqual(expectedDataType, actualDataType);
        }

        [TestMethod]
        public void Name_ReturnsTheNamePropertyOfThePropertyInfo()
        {
            var propertyInfo = this.GetPropertyInfo("Product");
            var desciption = new PropertyInfoFieldInfo(propertyInfo, (o) => o);
            var expectedName = propertyInfo.Name;

            var actualName = desciption.Name;

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void GetValue_WhenPropertyAccessIsSet_UsesThePropertyAccessToAccessTheValue()
        {
            var propertyInfo = this.GetPropertyInfo("Product");
            var desciption = new PropertyInfoFieldInfo(propertyInfo, (o) => "MyValue");
            var expectedValue = "MyValue";

            var actualValue = desciption.GetValue(this.data[0]);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void SetValue_WhenPropertySetterIsSet_ShouldUpdateTheValue()
        {
            var order = new Order();

            var propertyInfo = this.GetPropertyInfo("Product");
            var desciption = new PropertyInfoFieldInfo(propertyInfo, (o) => "MyValue", (s, e) => ((Order)s).Product = e.ToString());

            desciption.SetValue(order, "Some product");

            Assert.AreEqual("Some product", order.Product);

        }

        [TestMethod]
        public void GetValue_WhenPropertyAccessIsNotSet_UsesThePropertyInfoToAccessTheValue()
        {
            var propertyInfo = this.GetPropertyInfo("Product");
            var desciption = new PropertyInfoFieldInfo(propertyInfo);
            var expectedValue = propertyInfo.GetValue(this.data[0], null);

            var actualValue = desciption.GetValue(this.data[0]);

            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
