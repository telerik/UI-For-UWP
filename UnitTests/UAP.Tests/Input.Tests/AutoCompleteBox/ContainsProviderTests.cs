using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;

namespace Telerik.UI.Xaml.Controls.Input.Tests
{
    [TestClass]
    public class ContainsProviderTests
    {
        private ObservableCollection<string> itemsSource;
        private ContainsTextSearchProvider containsProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.containsProvider = new ContainsTextSearchProvider();
            this.GenerateItemsSource();
            this.containsProvider.ItemsSource = this.itemsSource;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.containsProvider.Reset();
            this.containsProvider.ItemsSource = null;
            this.containsProvider = null;
        }

        [TestMethod]
        public void RandomFilteringTest()
        {
            char[] inputSequence = new char[] { 'B', 'u', 'l', 'g', 'a', 'r', 'i', 'a'};

            for (int i = 0; i < inputSequence.Length; i++)
            {
                this.containsProvider.Input(0, 0, inputSequence[i].ToString());
                Assert.IsTrue(this.AllSuggestionsContain(this.containsProvider.InputString), "Suggestion object does not pass the current filter.");
                this.containsProvider.Reset();
            }
        }

        [TestMethod]
        public void ImmediateFilteringTest()
        {
            string input = "Bulgaria";
            this.containsProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.containsProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>(input, this.containsProvider.GetFilterKey(firstSuggestion));
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());
        }

        [TestMethod]
        public void SequentialWrongInputNoSuggestionsTest()
        {
            char[] inputSequence = new char[] { 'B', 'q', 'l', 'g', 'a', 'r', 'i', 'a' };

            for (int i = 0; i < inputSequence.Length; i++)
            {
                this.containsProvider.Input(i, 0, inputSequence[i].ToString());
                if (i == 0)
                {
                    Assert.IsTrue(this.AllSuggestionsContain(this.containsProvider.InputString), "Suggestion object does not pass the current filter.");
                }
                else
                {
                    Assert.IsTrue((this.containsProvider.FilteredItems as ICollection).Count == 0);
                }
            }
        }

        [TestMethod]
        public void WrongInputCorrectionTest1()
        {
            string input = "Brlgaria";
            this.containsProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.containsProvider.FilteredItems.GetEnumerator();
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());

            this.containsProvider.Input(1, 1, "u");
            filteredItemsEnumerator = this.containsProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>("Bulgaria", this.containsProvider.GetFilterKey(firstSuggestion));
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());
        }

        [TestMethod]
        public void WrongInputCorrectionTest2()
        {
            string input = "Bdhjskhfua";
            this.containsProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.containsProvider.FilteredItems.GetEnumerator();
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());

            this.containsProvider.Input(1, 8, "ulgari");
            filteredItemsEnumerator = this.containsProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>("Bulgaria", this.containsProvider.GetFilterKey(firstSuggestion));
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());
        }

        [TestMethod]
        public void NoSuggestionsWhenNoInputTest()
        {
            string input = "Bulgaria";
            this.containsProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.containsProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>("Bulgaria", this.containsProvider.GetFilterKey(firstSuggestion));

            this.containsProvider.Input(0, 8, string.Empty);

            Assert.IsFalse(this.containsProvider.HasItems);
        }

        private bool AllSuggestionsContain(string start)
        {
            bool hasFilteredItems = false;

            foreach (object o in this.containsProvider.FilteredItems)
            {
                hasFilteredItems = true;
                string key = this.containsProvider.GetFilterKey(o);

                if (!(key.IndexOf(start, 0, this.containsProvider.ComparisonMode) != -1))
                {
                    return false;
                }
            }

            return hasFilteredItems;
        }

        private void GenerateItemsSource()
        {
            this.itemsSource = CountriesHelper.GenerateItemsSourceCountries();
        }
    }
}
