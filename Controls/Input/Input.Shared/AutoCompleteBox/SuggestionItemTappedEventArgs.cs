using System;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// This class contains information about a <see cref="SuggestionItemsControl.ItemTapped"/> event.
    /// </summary>
    internal class SuggestionItemTappedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the data item that has been clicked.
        /// </summary>
        public object Item { get; set; }
    }
}
