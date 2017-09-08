using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Linq;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class EnumerableFieldDescriptionsExtractorTests
    {
        [TestMethod]
        public void Constructor_WhenSourceIsNull_ThrownArgumentNullException()
        {
            try
            {
                var extractor = new EnumerableFieldDescriptionsExtractor(null);

                Assert.Fail("ArgumentNullException expected");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void GetDescriptions_WhenNoSpecialInterfacesAreImplemented_WhenEmptyEnumerableIsPassed_WhenEnumerableIsGeneric_RetrievesDescriptionsFromGenericTypeArgument()
        {
            var items = EnumerableScenariosHelper.GetGenericEmptyEnumerableOfBaseType();
            var extractor = new EnumerableFieldDescriptionsExtractor(items);
            var expectedProperyNames = new[] { "NormalPropertyOne", "NormalPropertyTwo" };

            var descriptions = extractor.GetDescriptions();
            var actualPropertyNames = descriptions.Select(fd => fd.Name).ToList();

            CollectionAssert.AreEqual(expectedProperyNames, actualPropertyNames);
        }

        [TestMethod]
        public void GetDescriptions_WhenNoSpecialInterfacesAreImplemented_WhenEmptyEnumerableIsPassed_WhenEnumerableIsNotGeneric_RetrievesNoDescriptions()
        {
            var items = EnumerableScenariosHelper.GetNonGenericEmptyEnumerable();
            var extractor = new EnumerableFieldDescriptionsExtractor(items);
            var expectedProperyNames = new string[] { };

            var descriptions = extractor.GetDescriptions();
            var actualPropertyNames = descriptions.Select(fd => fd.Name).ToList();

            CollectionAssert.AreEqual(expectedProperyNames, actualPropertyNames);
        }

        [TestMethod]
        public void GetDescriptions_WhenNoSpecialInterfacesAreImplemented_WhenEnumerableIsNonGeneric_RetrievesDescriptionsFromTheTypeOfTheFirstItem()
        {
            var items = EnumerableScenariosHelper.GetNonGenericEnumerableWithItemsWithDifferentTypes();
            var extractor = new EnumerableFieldDescriptionsExtractor(items);
            var expectedProperyNames = new[] { "NormalPropertyOne", "NormalPropertyTwo" };

            var descriptions = extractor.GetDescriptions();
            var actualPropertyNames = descriptions.Select(fd => fd.Name).ToList();

            CollectionAssert.AreEqual(expectedProperyNames, actualPropertyNames);
        }

        [TestMethod]
        public void GetDescriptions_WhenNoSpecialInterfacesAreImplemented_WhenEnumerableIsGeneric_RetrievesDescriptionsFromTheTypeOfTheGenericArgument()
        {
            var items = EnumerableScenariosHelper.GetGenericEnumerableOfBaseTypeWithItemsThatInheritFromBaseType();
            var extractor = new EnumerableFieldDescriptionsExtractor(items);
            var expectedProperyNames = new[] { "NormalPropertyOne", "NormalPropertyTwo" };

            var descriptions = extractor.GetDescriptions();
            var actualPropertyNames = descriptions.Select(fd => fd.Name).ToList();

            CollectionAssert.AreEqual(expectedProperyNames, actualPropertyNames);
        }

        //TODO: ICustomTypeProvider (souce item) - WPF45 / SL5
    }
}
