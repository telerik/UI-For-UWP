using System;
using System.Collections;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// Provides infrastructure for using <see cref="RadAutoCompleteBox"/> with a Web Service
    /// for providing suggestions.
    /// </summary>
    public sealed class WebServiceTextSearchProvider : TextSearchProvider
    {
        private IEnumerable filteredItems;
        private bool dataRequestIssued;
        private bool requestScheduled;

        /// <summary>
        /// Fires when the input string of the <see cref="WebServiceTextSearchProvider"/> changes.
        /// </summary>
        public event EventHandler InputChanged;

        /// <summary>
        /// Gets a value indicating whether there
        /// are filtered suggestion items for the current input.
        /// </summary>
        public override bool HasItems
        {
            get
            {
                if (this.filteredItems == null)
                {
                    return false;
                }

                foreach (object item in this.filteredItems)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets an array containing a subset of the <see cref="TextSearchProvider.ItemsSource"/> which
        /// represents all filtered suggestion items for the current input.
        /// </summary>
        public override IEnumerable FilteredItems
        {
            get
            {
                return this.filteredItems;
            }
        }

        /// <summary>
        /// Resets the current input and sets the <see cref="TextSearchProvider" /> to its
        /// initial state.
        /// </summary>
        public override void Reset()
        {
            this.filteredItems = null;

            base.Reset();
        }

        /// <summary>
        /// Loads the acquired items based on the current input.
        /// </summary>
        /// <param name="items">An <see cref="IEnumerable"/> implementation that contains the available suggestions.</param>
        public void LoadItems(IEnumerable items)
        {
            this.filteredItems = items;
            this.OnPropertyChanged(nameof(this.FilteredItems));
            this.dataRequestIssued = false;

            if (this.requestScheduled)
            {
                this.requestScheduled = false;
                this.OnInputChanged();
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
            string previousInput = this.InputString;

            base.Input(start, selectionLength, inputText);

            if (previousInput != this.InputString)
            {
                this.OnInputChanged();
            }
        }

        private void OnInputChanged()
        {
            if (this.InputChanged != null)
            {
                if (this.dataRequestIssued)
                {
                    this.requestScheduled = true;
                }
                else
                {
                    this.dataRequestIssued = true;
                }

                this.InputChanged(this, EventArgs.Empty);
            }
        }
    }
}
