using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    internal class ContainsTextSearchProvider : TextSearchProvider
    {
        private List<object> filteredItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainsTextSearchProvider" /> class.
        /// </summary>
        public ContainsTextSearchProvider()
        {
            this.filteredItems = new List<object>();
        }

        /// <summary>
        /// Gets a value indicating whether there
        /// are any suggestion items available based on the current
        /// input in the <see cref="ITextSearchProvider" />.
        /// </summary>
        public override bool HasItems
        {
            get
            {
                if (string.IsNullOrEmpty(this.InputString))
                {
                    return false;
                }

                return this.filteredItems.Count > 0;
            }
        }

        /// <summary>
        /// Gets an array containing a subset of the <see cref="TextSearchProvider.ItemsSource" /> that
        /// represents all filtered suggestion items based on the current input.
        /// </summary>
        public override IEnumerable FilteredItems
        {
            get
            {
                return this.filteredItems;
            }
        }

        /// <summary>
        /// Inputs a string in the <see cref="TextSearchProvider.InputString"/> starting at the given position by purging the text described
        /// by the <paramref name="start"/> with the given <paramref name="selectionLength"/>.
        /// </summary>
        /// <param name="start">The start index in the current input string where the <paramref name="inputText"/> is inserted.</param>
        /// <param name="selectionLength">The length of the text to be replaced.</param>
        /// <param name="inputText">The text that will be inserted at <paramref name="start"/> position.</param>
        public override void Input(int start, int selectionLength, string inputText)
        {
            bool clear = string.IsNullOrEmpty(this.InputString) || start < this.InputString.Length || selectionLength > 0;
            
            base.Input(start, selectionLength, inputText);

            this.BuildFilteredItems(clear);
            this.OnPropertyChanged(nameof(this.FilteredItems));
        }

        /// <summary>
        /// Resets the current input and sets the <see cref="TextSearchProvider" /> to its
        /// initial state.
        /// </summary>
        public override void Reset()
        {
            this.filteredItems.Clear();

            base.Reset();
        }

        internal override void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs a)
        {
            base.OnItemsSourceCollectionChanged(a);

            if (!string.IsNullOrEmpty(this.InputString))
            {
                this.BuildFilteredItems(true);
                this.OnPropertyChanged(nameof(this.FilteredItems));
            }
        }

        internal override void OnItemsSourceChanged()
        {
            base.OnItemsSourceChanged();

            if (!string.IsNullOrEmpty(this.InputString))
            {
                this.BuildFilteredItems(true);
                this.OnPropertyChanged(nameof(this.FilteredItems));
            }
        }

        private void BuildFilteredItems(bool clear)
        {
            string currentInput = this.InputString;
            IEnumerable<object> items = this.SortedItems;

            if (clear)
            {
                this.filteredItems = new List<object>();
            }
            else
            {
                items = this.filteredItems;
                this.filteredItems = new List<object>();
            }

            if (string.IsNullOrEmpty(currentInput))
            {
                return;
            }

            StringComparison stringComparisonMode = this.ComparisonMode;

            foreach (object item in items)
            {
                if (this.GetFilterKey(item).IndexOf(currentInput, 0, stringComparisonMode) != -1)
                {
                    this.filteredItems.Add(item);
                }
            }
        }
    }
}
