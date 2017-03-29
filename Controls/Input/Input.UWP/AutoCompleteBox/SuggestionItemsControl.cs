using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// Represents a specialized <see cref="ItemsControl"/> that
    /// is integrated with <see cref="RadAutoCompleteBox"/> to display
    /// available auto-complete suggestions.
    /// </summary>
    public class SuggestionItemsControl : ListBox
    {
        internal RadAutoCompleteBox owner;

        private double desiredHeight;
        private bool isDesiredHeightDirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuggestionItemsControl" /> class.
        /// </summary>
        public SuggestionItemsControl()
        {
            this.DefaultStyleKey = typeof(SuggestionItemsControl);
        }

        internal event EventHandler<SuggestionItemTappedEventArgs> ItemTapped;

        internal double DesiredHeight
        {
            get
            {
                if (this.isDesiredHeightDirty)
                {
                    if (!this.HasItems)
                    {
                        this.desiredHeight = 0d;
                    }
                    else
                    {
                        FrameworkElement firstItem = this.ContainerFromIndex(0) as FrameworkElement;
                        if (firstItem == null)
                        {
                            this.desiredHeight = 0d;
                        }
                        else
                        {
                            this.desiredHeight = this.Items.Count * firstItem.ActualHeight;
                            this.isDesiredHeightDirty = false;
                        }
                    }
                }

                return this.desiredHeight;
            }
        }

        private bool HasItems
        {
            get
            {
                return this.Items.Count > 0;
            }
        }

        internal void OnItemTapped(SuggestionItem item)
        {
            if (this.ItemTapped != null)
            {
                this.ItemTapped(this, new SuggestionItemTappedEventArgs() { Item = item.DataItem });
            }
        }

        internal void SelectOrScrollToFirstItem()
        {
            if (!this.HasItems)
            {
                return;
            }

            if (this.owner.AutosuggestFirstItem)
            {
                this.SelectedIndex = 0;
            }

            this.ScrollIntoView(this.Items[0]);
        }

        internal void SelectPreviousItem()
        {
            if (!this.HasItems)
            {
                return;
            }

            if (this.SelectedIndex < 0 && this.owner.AutosuggestFirstItem)
            {
                return;
            }

            if (this.SelectedIndex > 0)
            {
                this.SelectedIndex--;
            }
            else
            {
                this.SelectedIndex = this.Items.Count - 1;
            }

            this.ScrollIntoView(this.SelectedItem);

            this.RaiseAutomationFocusChangedEvent();
        }

        internal void SelectNextItem()
        {
            if (!this.HasItems)
            {
                return;
            }

            if (this.SelectedIndex < 0 && this.owner.AutosuggestFirstItem)
            {
                return;
            }

            if (this.SelectedIndex < this.Items.Count - 1)
            {
                this.SelectedIndex++;
            }
            else
            {
                this.SelectedIndex = 0;
            }

            this.ScrollIntoView(this.SelectedItem);

            this.RaiseAutomationFocusChangedEvent();
        }

        private void RaiseAutomationFocusChangedEvent()
        {
            var suggestionItem = this.ContainerFromIndex(this.SelectedIndex) as ListBoxItem;
            if (suggestionItem != null)
            {
                var peer = FrameworkElementAutomationPeer.FromElement(suggestionItem) as ListBoxItemAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
        }

        internal DependencyObject GetContainerForSuggestionItem()
        {
            return new SuggestionItem();
        }

        internal void PrepareContainerForSuggestionItem(DependencyObject element, object item)
        {
            SuggestionItem suggestionItem = element as SuggestionItem;
            suggestionItem.Attach(this, item);

            suggestionItem.HighlightInput(this.owner.suggestionsProvider.InputString);

        }

        internal void ClearContainerForSuggestionItem(DependencyObject element, object item)
        {
            (element as SuggestionItem).Detach();
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of the layout cycle. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects or based on other considerations such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size result = base.MeasureOverride(availableSize);
            this.isDesiredHeightDirty = true;

            return result;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return this.owner.GetContainerForSuggestionItem();
        }

        /// <summary>
        /// Determines whether the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// True if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SuggestionItem;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element that's used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            this.owner.PrepareContainerForSuggestionItem(element, item);  
        }

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            this.owner.ClearContainerForSuggestionItem(element, item);

            base.ClearContainerForItemOverride(element, item);
        }
    }
}
