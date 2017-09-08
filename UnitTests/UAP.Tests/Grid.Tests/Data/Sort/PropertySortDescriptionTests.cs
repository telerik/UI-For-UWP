using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    /// <summary>
    /// Test class that test all supported aggregate descriptions (e.g. Sum, Count, Var, ...).
    /// </summary>
    [TestClass]
    public class SortDescriptionTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void PropertySortDescription_DefaultValues()
        {
            PropertySortDescription psd = new PropertySortDescription();
            Assert.AreEqual(SortOrder.Ascending, psd.SortOrder);
            Assert.IsTrue(string.IsNullOrWhiteSpace(psd.CustomName));
            Assert.IsTrue(string.IsNullOrWhiteSpace(psd.PropertyName));
            Assert.IsNull(psd.MemberAccess);
            Assert.AreEqual(psd.PropertyName, psd.GetMemberName());
        }

        [TestMethod]
        public void PropertySortDescription_Clone_Correctly()
        {
            PropertySortDescription psd = new PropertySortDescription();
            psd.PropertyName = "Test";
            Assert.AreEqual(psd.PropertyName, psd.DisplayName);
            psd.SortOrder = SortOrder.Descending;
            psd.CustomName = "Custom";
            Assert.AreEqual(psd.CustomName, psd.DisplayName);

            var copy = psd.Clone() as PropertySortDescription;
            Assert.IsNotNull(copy);
            Assert.AreNotSame(psd, copy);
            Assert.IsInstanceOfType(copy, typeof(PropertySortDescription));
            Assert.AreEqual(psd.CustomName, copy.CustomName);
            Assert.AreEqual(psd.PropertyName, copy.PropertyName);
            Assert.AreEqual(psd.DisplayName, copy.DisplayName);
            Assert.AreEqual(psd.SortOrder, copy.SortOrder);
            Assert.AreEqual(psd.PropertyName, psd.GetMemberName());
            Assert.AreEqual(copy.PropertyName, copy.GetMemberName());
        }

        [TestMethod]
        public void PropertySortDescription_PropertyName_Raise_DisplayName_PropertyChanged()
        {
            PropertySortDescription psd = new PropertySortDescription();
            psd.AssertPropertyChanged("Setting PropertyName", () => psd.PropertyName = "Test", "PropertyName", "DisplayName");
        }

        [TestMethod]
        public void PropertySortDescription_CustomName_Raise_DisplayName_PropertyChanged()
        {
            PropertySortDescription psd = new PropertySortDescription();
            psd.AssertPropertyChanged("Setting CustomName", () => psd.CustomName = "Test", "CustomName", "DisplayName");
        }

        [TestMethod]
        public void PropertySortDescription_SortOrder_Raise_PropertyChanged()
        {
            PropertySortDescription psd = new PropertySortDescription();
            psd.AssertPropertyChanged("Setting SortOrder", () => psd.SortOrder = SortOrder.Descending, "SortOrder");
        }
    }
}