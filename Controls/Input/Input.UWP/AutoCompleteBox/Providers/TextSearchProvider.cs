using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// This class provides a mechanism for filtering suggestion items based on a given input
    /// and a data source that contains a set of predefined suggestions.
    /// </summary>
    public abstract class TextSearchProvider : ITextSearchProvider, IWeakEventListener
    {
        private readonly List<object> sortedItems;

        private WeakEventHandler<NotifyCollectionChangedEventArgs> collectionChangedHandler;
        private Func<object, string> filterKeyProvider;
        private IEnumerable itemsSource;
        private StringBuilder currentInput = new StringBuilder();
        private Func<object, object> filterKeyGetter;
        private StringComparison textComparisonMode = StringComparison.CurrentCultureIgnoreCase;
        private string filterMemberPath = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSearchProvider"/> class.
        /// </summary>
        protected TextSearchProvider()
        {
            this.sortedItems = new List<object>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TextSearchProvider"/> class.
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TextSearchProvider"/> is reclaimed by garbage collection.
        /// </summary>
        ~TextSearchProvider()
        {
            if (this.collectionChangedHandler != null)
            {
                this.collectionChangedHandler.Unsubscribe();
                this.collectionChangedHandler = null;
            }
        }

        /// <summary>
        /// Fires when a property of this <see cref="TextSearchProvider"/> changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the mode used to compare 
        /// the current input string with the filter
        /// key of each <see cref="ItemsSource"/> item.
        /// </summary>
        public StringComparison ComparisonMode
        {
            get
            {
                return this.textComparisonMode;
            }
            set
            {
                this.textComparisonMode = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there
        /// are filtered suggestion items for the current input.
        /// </summary>
        public abstract bool HasItems
        {
            get;
        }

        /// <summary>
        /// Gets or sets a string representing a property path
        /// on a single object from the <see cref="TextSearchProvider.ItemsSource"/>
        /// that is used to filter the suggestion items.
        /// </summary>
        public string FilterMemberPath
        {
            get
            {
                return this.filterMemberPath;
            }
            set
            {
                this.filterMemberPath = value;
                this.OnPropertyChanged(nameof(this.FilterMemberPath));
            }
        }

        /// <summary>
        /// Gets or sets a function that is used to retrieve
        /// the key from the source object, 
        /// based on which suggestion items are filtered.
        /// </summary>
        public Func<object, string> FilterMemberProvider
        {
            get
            {
                return this.filterKeyProvider;
            }
            set
            {
                if (value != this.filterKeyProvider)
                {
                    this.filterKeyProvider = value;
                    this.OnPropertyChanged(nameof(this.FilterMemberProvider));
                }
            }
        }

        /// <summary>
        /// Gets an array containing a subset of the <see cref="ItemsSource"/> which
        /// represents all suggestion items filtered based on the current input.
        /// </summary>
        public abstract IEnumerable FilteredItems
        {
            get;
        }

        /// <summary>
        /// Gets or sets the source collection that provides
        /// the set of suggestion items.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
            set
            {
                if (this.itemsSource != value)
                {
                    this.itemsSource = value;
                    this.OnItemsSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets the current input string based on which the suggestion items are filtered.
        /// </summary>
        public string InputString
        {
            get
            {
                return this.currentInput.ToString();
            }
        }

        internal List<object> SortedItems
        {
            get
            {
                return this.sortedItems;
            }
        }

        /// <summary>
        /// Receives the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        public void ReceiveEvent(object sender, object args)
        {
            if (sender == this.itemsSource)
            {
                this.OnItemsSourceCollectionChanged(args as NotifyCollectionChangedEventArgs);
            }
        }

        /// <summary>
        /// Inputs a string in the <see cref="InputString"/> starting at the given position by purging the text described
        /// by the <paramref name="start"/> with the given <paramref name="selectionLength"/>.
        /// </summary>
        /// <param name="start">The start index in the current input string where the <paramref name="inputText"/> is inserted.</param>
        /// <param name="selectionLength">The length of the text to be replaced.</param>
        /// <param name="inputText">The text that will be inserted at <paramref name="start"/> position.</param>
        public virtual void Input(int start, int selectionLength, string inputText)
        {
            if (start < 0 || selectionLength < 0)
            {
                return;
            }

            this.currentInput.Remove(start, selectionLength);
            this.currentInput = this.currentInput.Insert(start, inputText);
        }

        /// <summary>
        /// Resets the current input and sets the <see cref="TextSearchProvider"/> to its
        /// initial state.
        /// </summary>
        public virtual void Reset()
        {
            this.currentInput.Clear();
            this.OnPropertyChanged(nameof(this.FilteredItems));
        }

        /// <summary>
        /// Gets a string that represents the key based on which the
        /// corresponding item has been filtered as a valid suggestion item.
        /// </summary>
        /// <param name="item">The item for which to get the filter key.</param>
        /// <returns>
        /// The filter key.
        /// </returns>
        public virtual string GetFilterKey(object item)
        {
            if (item == null)
            {
                return null;
            }

            if (this.filterKeyGetter == null && !string.IsNullOrEmpty(this.filterMemberPath))
            {
                this.ResetFilterKeyGetter(item);
            }

            if (this.filterKeyGetter != null)
            {
                return this.filterKeyGetter(item).ToString();
            }

            if (this.filterKeyProvider != null)
            {
                return this.filterKeyProvider(item);
            }

            return item.ToString();
        }

        internal virtual void OnItemsSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    this.sortedItems.Remove(args.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Add:
                    object newItem = args.NewItems[0];
                    int insertIndex = 0;

                    for (int i = 0; i < this.sortedItems.Count; i++)
                    {
                        if (string.Compare(this.GetFilterKey(this.sortedItems[i]), this.GetFilterKey(newItem), this.textComparisonMode) >= 0)
                        {
                            insertIndex = i;
                            break;
                        }
                        else
                        {
                            insertIndex = i + 1;
                        }
                    }
                    this.sortedItems.Insert(insertIndex, newItem);
                    break;
                default:
                    this.sortedItems.Clear();

                    foreach (object item in this.itemsSource)
                    {
                        this.sortedItems.Add(item);
                    }

                    this.sortedItems.Sort(new FilterKeyComparer(this));
                    break;
            }
        }

        internal virtual void OnItemsSourceChanged()
        {
            if (this.collectionChangedHandler != null)
            {
                this.collectionChangedHandler.Unsubscribe();
                this.collectionChangedHandler = null;
            }

            this.filterKeyGetter = null;
            this.sortedItems.Clear();

            if (this.itemsSource != null)
            {
                if (this.itemsSource is INotifyCollectionChanged)
                {
                    this.collectionChangedHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(this.itemsSource, this, KnownEvents.CollectionChanged);
                }

                foreach (object item in this.itemsSource)
                {
                    this.sortedItems.Add(item);
                }

                this.sortedItems.Sort(new FilterKeyComparer(this));
            }
            else
            {
                this.Reset();
            }
        }

        /// <summary>
        /// Called when a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "FilterMemberPath")
            {
                this.filterKeyGetter = null;
            }

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ResetFilterKeyGetter(object item)
        {
            this.filterKeyGetter = DynamicHelper.CreatePropertyValueGetter(item.GetType(), this.filterMemberPath);

            if (this.filterKeyGetter == null)
            {
                Debug.Assert(false, "Invalid FilterMemberPath defined.");
            }
        }

        private class FilterKeyComparer : IComparer<object>
        {
            private ITextSearchProvider provider;

            public FilterKeyComparer(ITextSearchProvider owner)
            {
                this.provider = owner;
            }

            public int Compare(object first, object second)
            {
                return string.Compare(this.provider.GetFilterKey(first), this.provider.GetFilterKey(second), this.provider.ComparisonMode);
            }
        }
    }
}