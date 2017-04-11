using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Reflection;
using Telerik.Data.Core;

namespace DataControls.Tests.DataForm
{
    [TestClass]
    public class EntityPropertyTests
    {

        [TestMethod]
        public void ShouldSetDisplayMessage_ShouldBeFalse()
        {
            var data = new Data();
            var property = new RuntimeEntityProperty(typeof(Data).GetTypeInfo().GetDeclaredProperty("Property1"), data);

            Assert.IsFalse(property.DisplayPositiveMessage);
        }

        [TestMethod]
        public void SettingPropertyValue_ShouldSetDisplayMessageToFalse()
        {
            var data = new Data();
            var property = new RuntimeEntityProperty(typeof(Data).GetTypeInfo().GetDeclaredProperty("Property1"), data);

            property.DisplayPositiveMessage = true;
            property.PropertyValue = 1;

            Assert.IsFalse(property.DisplayPositiveMessage);
        }

        public class Data
        {
            public int Property1 { get; set; }
        }
    }


}
