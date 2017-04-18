using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class PropertyFilterDescriptionTests
    {
        #region DefaultValues
        [TestMethod]
        public void PropertyFilterDescription_DefaultValues()
        {
            var gd = new PropertyFilterDescription();
            Assert.IsNull(gd.CustomName, "CustomName default value is wrong.");
            Assert.IsNull(gd.Condition, "Condition default value is wrong.");
            Assert.IsNull(gd.PropertyName, "PropertyName default value is wrong.");
        }
        #endregion

        #region PropertyChangedNotifications
        [TestMethod]
        public void PropertyFilterDescription_PropertyChangedNotifications_CustomName()
        {
            var gd = new PropertyFilterDescription() { CustomName = string.Empty };
            gd.AssertPropertyChanged("Setting CustomName", () => gd.CustomName = "UserSetCustomName", "CustomName", "DisplayName");
        }

        [TestMethod]
        public void PropertyFilterDescription_PropertyChangedNotifications_PropertyName()
        {
            var gd = new PropertyFilterDescription() { PropertyName = string.Empty };
            gd.AssertPropertyChanged("Setting PropertyName", () => gd.PropertyName = "CustomProperty", "PropertyName", "DisplayName");
        }

        //[TestMethod]
        //public void PropertyFilterDescription_PropertyChangedNotifications_Condition()
        //{
        //    var gd = new PropertyFilterDescription() { PropertyName = string.Empty };
        //    gd.AssertPropertyChanged("Setting Condition", () => gd.Condition = new SetCondition(), "Condition");
        //}
        #endregion

        #region Proper DisplayName
        [TestMethod]
        public void PropertyFilterDescription_DisplayName_From_PropertyName()
        {
            var gd = new PropertyFilterDescription() { PropertyName = "PropertyName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("PropertyName", gd.DisplayName);
        }

        [TestMethod]
        public void PropertyFilterDescription_DisplayName_From_CustomName()
        {
            var gd = new PropertyFilterDescription() { CustomName = "CustomName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("CustomName", gd.DisplayName);
        }

        [TestMethod]
        public void PropertyFilterDescription_DisplayName_From_PropertyName_and_CustomName()
        {
            var gd = new PropertyFilterDescription() { CustomName = "CustomName", PropertyName = "PropertyName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("CustomName", gd.DisplayName);
        }

        [TestMethod]
        public void PropertyFilterDescription_DisplayName_CorrectOnCleanUp()
        {
            var gd = new PropertyFilterDescription() { CustomName = "CustomName", PropertyName = "PropertyName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("CustomName", gd.DisplayName);
            gd.CustomName = null;
            Assert.AreEqual("PropertyName", gd.DisplayName);
            gd.PropertyName = null;
            Assert.IsNull(null, gd.DisplayName);
        }
        #endregion
    }
}
