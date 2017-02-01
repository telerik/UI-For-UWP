using System;
using System.Collections;
using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// An interface used by <see cref="RadAutoCompleteBox"/> that describes text search provider.
    /// </summary>
    public interface ITextSearchProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the mode used to compare 
        /// the current input string with the filter
        /// key of each <see cref="ItemsSource"/> item.
        /// </summary>
        StringComparison ComparisonMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a string representing a property path
        /// on a single object from the <see cref="ITextSearchProvider.ItemsSource"/>
        /// that is used to filter the suggestion items.
        /// </summary>
        string FilterMemberPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an <see cref="IEnumerable"/> implementation that contains all suggestion items.
        /// </summary>
        IEnumerable ItemsSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a function that is used to retrieve
        /// the key from the source object, 
        /// based on which suggestion items are filtered.
        /// </summary>
        Func<object, string> FilterMemberProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether there 
        /// are any suggestion items available based on the current
        /// input in the <see cref="ITextSearchProvider"/>.
        /// </summary>
        bool HasItems
        {
            get;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable"/> implementation containing the currently filtered suggestion items.
        /// </summary>
        IEnumerable FilteredItems
        {
            get;
        }

        /// <summary>
        /// Gets the current input string.
        /// </summary>
        string InputString
        {
            get;
        }

        /// <summary>
        /// Resets the state of the current provider.
        /// </summary>
        void Reset();

        /// <summary>
        /// Inputs a string in the <see cref="InputString"/> starting at the given position by purging the text described
        /// by the <paramref name="start"/> with the given <paramref name="selectionLength"/>.
        /// </summary>
        /// <param name="start">The start index in the current input string where the <paramref name="inputText"/> is inserted.</param>
        /// <param name="selectionLength">The length of the text to be replaced.</param>
        /// <param name="inputText">The text that will be inserted at <paramref name="start"/> position.</param>
        void Input(int start, int selectionLength, string inputText);

        /// <summary>
        /// Gets a string that represents the key based on which the
        /// corresponding item has been filtered as a valid suggestion item.
        /// </summary>
        /// <param name="item">The item for which to get the filter key.</param>
        /// <returns>The filter key.</returns>
        string GetFilterKey(object item);
    }
}
