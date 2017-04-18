using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public partial class LocalDataSourceFieldDescriptionsProviderTests
    {
        private InheritedLocalDataSourceFieldDescriptionsProvider provider;
        private LocalDataSourceProvider dataProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dataProvider = new LocalDataSourceProvider();
            this.provider = new InheritedLocalDataSourceFieldDescriptionsProvider(this.dataProvider);
        }

        [TestMethod]
        public void GenerateDescriptionsData_WhenItemsSourceOfTheDataProviderIsNull_ReturnsDescriptionsDataWithNoFieldInfoNodes()
        {
            var expectedNumberOfChildren = 0;
            this.dataProvider.ItemsSource = null;

            var descriptionsData = this.provider.ExposedGenerateDescriptionsData();
            var actualNumberOfChildren = descriptionsData.RootFieldInfo.Children.Count;
            
            Assert.AreEqual(expectedNumberOfChildren, actualNumberOfChildren);
        }

        [TestMethod]
        public void GenerateDescriptionsData_WhenItemsSourceOfTheDataProviderIsEmptyEnumerable_WhenNoSpecialInterfacesAreImplemented_WhenEnumerableIsGeneric_ReturnsDescriptionsDataWithFieldInfoNodesForEveryPublicPropertyOfTheGenericTypeArgument()
        {
            var items = EnumerableScenariosHelper.GetGenericEmptyEnumerableOfBaseType();
            this.dataProvider.ItemsSource = items;
            var expectedFieldInfoNames = new[] { "NormalPropertyOne", "NormalPropertyTwo" };

            var infoData = this.provider.ExposedGenerateDescriptionsData();
            var actualFieldInfoNames = infoData.RootFieldInfo.Children.OfType<FieldInfoNode>().Select(fd => fd.FieldInfo.Name).ToList();

            CollectionAssert.AreEqual(expectedFieldInfoNames, actualFieldInfoNames);
        }

        [TestMethod]
        public void GenerateDescriptionsData_WhenItemsSourceOfTheDataProviderIsEmptyEnumerable_WhenNoSpecialInterfacesAreImplemented_WhenEnumerableIsNotGeneric_ReturnsDescriptionsDataWithNoFieldInfoNodes()
        {
            var items = EnumerableScenariosHelper.GetNonGenericEmptyEnumerable();
            this.dataProvider.ItemsSource = items;
            var expectedFieldInfoNames = new string[] { };

            var infoData = this.provider.ExposedGenerateDescriptionsData();
            var actualFieldInfoNames = infoData.RootFieldInfo.Children.OfType<FieldInfoNode>().Select(fd => fd.FieldInfo.Name).ToList();

            CollectionAssert.AreEqual(expectedFieldInfoNames, actualFieldInfoNames);
        }

        [TestMethod]
        public void GenerateDescriptionsData_WhenItemsSourceOfTheDataProviderIsEnumerable_WhenNoSpecialInterfacesAreImplemented_WhenEnumerableIsNonGeneric_ReturnsDescriptionsDataWithFieldInfoNodesForEveryPublicPropertyOfTheFirstItemType()
        {
            var items = EnumerableScenariosHelper.GetNonGenericEnumerableWithItemsWithDifferentTypes();
            this.dataProvider.ItemsSource = items;
            var expectedFieldInfoNames = new[] { "NormalPropertyOne", "NormalPropertyTwo" };

            var infoData = this.provider.ExposedGenerateDescriptionsData();
            var actualFieldInfoNames = infoData.RootFieldInfo.Children.OfType<FieldInfoNode>().Select(fd => fd.FieldInfo.Name).ToList();

            CollectionAssert.AreEqual(expectedFieldInfoNames, actualFieldInfoNames);
        }

        [TestMethod]
        public void GenerateDescriptionsData_WhenItemsSourceOfTheDataProviderIsEnumerable_WhenNoSpecialInterfacesAreImplemented_WhenEnumerableIsGeneric_ReturnsDescriptionsDataWithFieldInfoNodesForEveryPublicPropertyOfTheGenericTypeArgument()
        {
            var items = EnumerableScenariosHelper.GetGenericEnumerableOfBaseTypeWithItemsThatInheritFromBaseType();
            this.dataProvider.ItemsSource = items;
            var expectedFieldInfoNames = new[] { "NormalPropertyOne", "NormalPropertyTwo" };

            var infoData = this.provider.ExposedGenerateDescriptionsData();
            var actualFieldInfoNames = infoData.RootFieldInfo.Children.OfType<FieldInfoNode>().Select(fd => fd.FieldInfo.Name).ToList();

            CollectionAssert.AreEqual(expectedFieldInfoNames, actualFieldInfoNames);
        }

        [TestMethod]
        public void GenerateDescriptionsData_WhenItemsSourceOfTheDataProviderIsEnumerable_WhenNoSpecialInterfacesAreImplemented_WhenEnumerableIsGenericAndGenericTypeIsObject_ReturnsDescriptionsDataWithFieldInfoNodesForEveryPublicPropertyOfTheFirstItemType()
        {
            var items = EnumerableScenariosHelper.GetGenericEnumerableOfObjectWithItemsOfBaseType();
            this.dataProvider.ItemsSource = items;
            var expectedFieldInfoNames = new[] { "NormalPropertyOne", "NormalPropertyTwo" };

            var infoData = this.provider.ExposedGenerateDescriptionsData();
            var actualFieldInfoNames = infoData.RootFieldInfo.Children.OfType<FieldInfoNode>().Select(fd => fd.FieldInfo.Name).ToList();

            CollectionAssert.AreEqual(expectedFieldInfoNames, actualFieldInfoNames);
        }
    }
}
