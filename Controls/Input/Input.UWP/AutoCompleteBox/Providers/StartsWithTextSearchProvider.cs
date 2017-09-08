using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    internal class StartsWithTextSearchProvider : TextSearchProvider
    {
        private Stack<FilterStep> filterSteps;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartsWithTextSearchProvider" /> class.
        /// </summary>
        public StartsWithTextSearchProvider()
        {
            this.filterSteps = new Stack<FilterStep>();
        }

        /// <summary>
        /// Gets an array containing a subset of the <see cref="TextSearchProvider.ItemsSource" /> which
        /// represents all suggestion items filtered based on the current input.
        /// </summary>
        public override IEnumerable FilteredItems
        {
            get
            {
                if (this.filterSteps.Count == 0)
                {
                    return new object[0];
                }

                FilterStep lastFilterStep = this.filterSteps.Peek();
                object[] result = new object[lastFilterStep.Length];
                this.SortedItems.CopyTo(lastFilterStep.Index, result, 0, lastFilterStep.Length);

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there
        /// are filtered suggestion items for the current input.
        /// </summary>
        public override bool HasItems
        {
            get
            {
                return this.filterSteps.Count > 0 ? this.filterSteps.Peek().Length > 0 : false;
            }
        }

        /// <summary>
        /// Resets the state of the current provider.
        /// </summary>
        public override void Reset()
        {
            this.filterSteps.Clear();

            base.Reset();
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
            string previousInput = this.InputString;

            base.Input(start, selectionLength, inputText);

            string currentInput = this.InputString;

            if (previousInput.StartsWith(currentInput, this.ComparisonMode))
            {
                while (this.filterSteps.Count > 0)
                {
                    FilterStep currentStep = this.filterSteps.Peek();
                    if (currentStep.Input == currentInput)
                    {
                        break;
                    }

                    this.filterSteps.Pop();
                }
            }
            else if (!currentInput.StartsWith(previousInput, this.ComparisonMode))
            {
                this.filterSteps.Clear();
            }

            if (!string.IsNullOrEmpty(currentInput))
            {
                this.BuildFilterSteps();
            }

            this.OnPropertyChanged(nameof(this.FilteredItems));
        }

        internal override void OnItemsSourceCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs a)
        {
            base.OnItemsSourceCollectionChanged(a);

            if (!string.IsNullOrEmpty(this.InputString))
            {
                this.filterSteps.Clear();
                this.BuildFilterSteps();
                this.OnPropertyChanged(nameof(this.FilteredItems));
            }
        }

        internal override void OnItemsSourceChanged()
        {
            base.OnItemsSourceChanged();

            if (!string.IsNullOrEmpty(this.InputString))
            {
                this.filterSteps.Clear();
                this.BuildFilterSteps();
                this.OnPropertyChanged(nameof(this.FilteredItems));
            }
        }

        private void BuildFilterSteps()
        {
            if (this.filterSteps.Count > 0 && this.filterSteps.Peek().Input == this.InputString)
            {
                return;
            }

            int filteredRangeStart = this.filterSteps.Count == 0 ? 0 : this.filterSteps.Peek().Index;
            int filteredRangeEnd = filteredRangeStart;
            bool startInitialized = false;
            string currentInput = this.InputString;
            List<object> sortedSuggestions = this.SortedItems;
            int itemCount = sortedSuggestions.Count;

            StringComparison stringComparisonMode = this.ComparisonMode;

            for (int i = filteredRangeStart; i < itemCount; i++)
            {
                if (this.GetFilterKey(sortedSuggestions[i]).StartsWith(currentInput, stringComparisonMode))
                {
                    if (!startInitialized)
                    {
                        filteredRangeStart = i;
                        startInitialized = true;
                    }

                    if (i + 1 == itemCount && startInitialized)
                    {
                        filteredRangeEnd = i;
                    }
                }
                else
                {
                    if (startInitialized)
                    {
                        filteredRangeEnd = i - 1;
                        break;
                    }
                }
            }

            this.filterSteps.Push(new FilterStep(filteredRangeStart, startInitialized ? filteredRangeEnd - filteredRangeStart + 1 : 0, currentInput));
        }
    }
}
