using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Charting;
using Telerik.Core;

namespace Telerik.Windows.Controls.Tests
{
    [TestClass]
    public class PropertyBagTests
    {
        private PropertyBagObject testObject;

        [TestInitialize]
        public void TestInitialize()
        {
            this.testObject = new TestPropertyBagObject();
        }

        [TestMethod]
        public void TestInitialPropertyState()
        {
            var propertyValue = this.testObject.GetValue(TestPropertyBagObject.TestDoublePropertyKey);
            var isLocalValue = this.testObject.IsLocalValue(TestPropertyBagObject.TestDoublePropertyKey);
            
            Assert.AreEqual(null, propertyValue);
            Assert.AreEqual(false, isLocalValue);
        }

        [TestMethod]
        public void TestGetSetValue()
        {
            Assert.AreEqual(false, this.testObject.IsLocalValue(TestPropertyBagObject.TestDoublePropertyKey));

            double testValue = 10d;
            this.testObject.SetValue(TestPropertyBagObject.TestDoublePropertyKey, testValue);

            Assert.AreEqual(true, this.testObject.IsLocalValue(TestPropertyBagObject.TestDoublePropertyKey));
            Assert.AreEqual(testValue, this.testObject.GetValue(TestPropertyBagObject.TestDoublePropertyKey));
        }

        [TestMethod]
        public void TestClearValue()
        {
            Assert.AreEqual(false, this.testObject.IsLocalValue(TestPropertyBagObject.TestDoublePropertyKey));

            double testValue = 10d;
            this.testObject.SetValue(TestPropertyBagObject.TestDoublePropertyKey, testValue);
            Assert.AreEqual(true, this.testObject.IsLocalValue(TestPropertyBagObject.TestDoublePropertyKey));
            Assert.AreEqual(testValue, this.testObject.GetValue(TestPropertyBagObject.TestDoublePropertyKey));

            this.testObject.ClearValue(TestPropertyBagObject.TestDoublePropertyKey);
            Assert.AreEqual(false, this.testObject.IsLocalValue(TestPropertyBagObject.TestDoublePropertyKey));
            Assert.AreEqual(null, this.testObject.GetValue(TestPropertyBagObject.TestDoublePropertyKey));
        }
    }
}
