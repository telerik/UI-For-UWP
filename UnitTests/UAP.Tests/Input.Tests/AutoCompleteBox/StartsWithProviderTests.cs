using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;

namespace Telerik.UI.Xaml.Controls.Input.Tests
{
    [TestClass]
    public class StartsWithProviderTests
    {
        private ObservableCollection<string> itemsSource;
        private StartsWithTextSearchProvider startsWithProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.startsWithProvider = new StartsWithTextSearchProvider();
            this.GenerateItemsSource();
            this.startsWithProvider.ItemsSource = this.itemsSource;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.startsWithProvider.Reset();
            this.startsWithProvider.ItemsSource = null;
            this.startsWithProvider = null;
        }

        [TestMethod]
        public void SequentialFilteringTest()
        {
            char[] inputSequence = new char[] {'B', 'u', 'l', 'g', 'a', 'r', 'i', 'a' };

            for(int i = 0; i< inputSequence.Length; i++)
            {
                this.startsWithProvider.Input(i, 0, inputSequence[i].ToString());
                Assert.IsTrue(this.AllSuggestionsStartWith(this.startsWithProvider.InputString), "Suggestion object does not pass the current filter.");
            }
        }

        [TestMethod]
        public void ImmediateFilteringTest()
        {
            string input = "Bulgaria";
            this.startsWithProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>(input, this.startsWithProvider.GetFilterKey(firstSuggestion));
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());
        }


        [TestMethod]
        public void SequentialWrongInputNoSuggestionsTest()
        {
            char[] inputSequence = new char[] { 'B', 'q', 'l', 'g', 'a', 'r', 'i', 'a' };

            for (int i = 0; i < inputSequence.Length; i++)
            {
                this.startsWithProvider.Input(i, 0, inputSequence[i].ToString());
                if (i == 0)
                {
                    Assert.IsTrue(this.AllSuggestionsStartWith(this.startsWithProvider.InputString), "Suggestion object does not pass the current filter.");
                }
                else
                {
                    Assert.IsTrue((this.startsWithProvider.FilteredItems as ICollection).Count == 0);
                }
            }
        }

        [TestMethod]
        public void WrongInputCorrectionTest1()
        {
            string input = "Brlgaria";
            this.startsWithProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());

            this.startsWithProvider.Input(1, 1, "u");
            filteredItemsEnumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>("Bulgaria", this.startsWithProvider.GetFilterKey(firstSuggestion));
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());
        }

        [TestMethod]
        public void WrongInputCorrectionTest2()
        {
            string input = "Bdhjskhfua";
            this.startsWithProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());

            this.startsWithProvider.Input(1, 8, "ulgari");
            filteredItemsEnumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>("Bulgaria", this.startsWithProvider.GetFilterKey(firstSuggestion));
            Assert.IsFalse(filteredItemsEnumerator.MoveNext());
        }

        [TestMethod]
        public void NoSuggestionsWhenNoInputTest()
        {
            string input = "Bulgaria";
            this.startsWithProvider.Input(0, 0, input);
            IEnumerator filteredItemsEnumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
            Assert.IsTrue(filteredItemsEnumerator.MoveNext());
            object firstSuggestion = filteredItemsEnumerator.Current;
            Assert.AreEqual<string>("Bulgaria", this.startsWithProvider.GetFilterKey(firstSuggestion));

            this.startsWithProvider.Input(0, 8, string.Empty);

            Assert.IsFalse(this.startsWithProvider.HasItems);
        }

        [TestMethod]
        public void BuildStepLengthCalculationTest()
        {
            string input = "qwerty";
            string[] itemsSource = new string[] { "qwerty1", "qwerty2", "qwerty3", "qwerty4" };
            this.startsWithProvider.ItemsSource = itemsSource;

            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                Assert.AreEqual<int>(4, (this.startsWithProvider.FilteredItems as ICollection).Count);
            }
        }

        [TestMethod]
        public void BuildStepStartIndexCalculationTest()
        {
            string input = "qwerty2";
            string[] itemsSource = new string[] { "qwerty1", "qwerty2", "qwerty3", "qwerty4" };
            this.startsWithProvider.ItemsSource = itemsSource;

            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                if (i + 1 == input.Length)
                {
                    IEnumerator e = this.startsWithProvider.FilteredItems.GetEnumerator();
                    e.MoveNext();
                    Assert.AreEqual<string>("qwerty2", e.Current.ToString());
                }
            }
        }

        [TestMethod]
        public void ItemsSourceCollectionChangedHandledTest1()
        {
            string input = "qwerty2";

            ObservableCollection<string> itemsSource = new ObservableCollection<string>();

            itemsSource.Add("qwerty1");
            itemsSource.Add("qwerty2");
            itemsSource.Add("qwerty3");
            itemsSource.Add("qwerty4");
            itemsSource.Add("qwerty5");

            this.startsWithProvider.ItemsSource = itemsSource;

            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                if (i + 1 == input.Length)
                {
                    IEnumerator enumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
                    enumerator.MoveNext();
                    Assert.AreEqual<string>("qwerty2", enumerator.Current.ToString());
                }
            }

            itemsSource[1] = "qwerty23";

            this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, "3");

            IEnumerator e = this.startsWithProvider.FilteredItems.GetEnumerator();

            e.MoveNext();

            Assert.AreEqual<string>("qwerty23", e.Current.ToString());

            itemsSource[1] = "querty7";

            Assert.AreEqual<int>(0, (this.startsWithProvider.FilteredItems as ICollection).Count);
        }

        [TestMethod]
        public void AddingNewSuggestionItemInSourceTest()
        {
            string input = "qwerty56";

            ObservableCollection<string> itemsSource = new ObservableCollection<string>();

            itemsSource.Add("qwerty1");
            itemsSource.Add("qwerty2");
            itemsSource.Add("qwerty3");
            itemsSource.Add("qwerty4");
            itemsSource.Add("qwerty5");

            this.startsWithProvider.ItemsSource = itemsSource;

            itemsSource.Add("qwerty56");

            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                if (i + 1 == input.Length)
                {
                    IEnumerator enumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
                    enumerator.MoveNext();
                    Assert.AreEqual<string>("qwerty56", enumerator.Current.ToString());
                }
            }

            this.startsWithProvider.Reset();
            input = "a";
            itemsSource.Add("abcd");

            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                if (i + 1 == input.Length)
                {
                    IEnumerator enumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
                    enumerator.MoveNext();
                    Assert.AreEqual<string>("abcd", enumerator.Current.ToString());
                }
            }

            this.startsWithProvider.Reset();
            input = "qwerty38";
            itemsSource.Add("qwerty38");

            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                if (i + 1 == input.Length)
                {
                    IEnumerator enumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
                    enumerator.MoveNext();
                    Assert.AreEqual<string>("qwerty38", enumerator.Current.ToString());
                }
            }
        }

        [TestMethod]
        public void RemovingSuggestionItemTest()
        {
            string input = "qwerty3";

            ObservableCollection<string> itemsSource = new ObservableCollection<string>();

            itemsSource.Add("qwerty1");
            itemsSource.Add("qwerty2");
            itemsSource.Add("qwerty33");
            itemsSource.Add("qwerty34");
            itemsSource.Add("qwerty4");
            itemsSource.Add("qwerty5");

            this.startsWithProvider.ItemsSource = itemsSource;

            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                if (i + 1 == input.Length)
                {
                    IEnumerator enumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
                    enumerator.MoveNext();
                    Assert.AreEqual<string>("qwerty33", enumerator.Current.ToString());
                }
            }
            this.startsWithProvider.Reset();
            itemsSource.RemoveAt(2);
            for (int i = 0; i < input.Length; i++)
            {
                this.startsWithProvider.Input(this.startsWithProvider.InputString.Length, 0, input[i].ToString());
                if (i + 1 == input.Length)
                {
                    IEnumerator enumerator = this.startsWithProvider.FilteredItems.GetEnumerator();
                    enumerator.MoveNext();
                    Assert.AreEqual<string>("qwerty34", enumerator.Current.ToString());
                }
            }
        }

        private bool AllSuggestionsStartWith(string start)
        {
            foreach (object o in this.startsWithProvider.FilteredItems)
            {
                string key = this.startsWithProvider.GetFilterKey(o);

                if (!key.StartsWith(start, this.startsWithProvider.ComparisonMode))
                {
                    return false;
                }
            }

            return true;
        }

        private void GenerateItemsSource()
        {
            this.itemsSource = CountriesHelper.GenerateItemsSourceCountries();
        }
    }
}
