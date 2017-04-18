using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.UI.Automation.Peers;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Primitives.Pagination
{
    /// <summary>
    /// A List control, used to visualize thumbnail data of <see cref="RadPaginationControl"/>.
    /// </summary>
    public class PaginationListControl : ListBox
    {
        private RadPaginationControl owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationListControl"/> class.
        /// </summary>
        public PaginationListControl()
        {
            this.DefaultStyleKey = typeof(PaginationListControl);

            this.SelectionChanged += this.OnSelectionChanged;
        }

        internal void OnOwnerSourceCollectionChanged(IVectorChangedEventArgs args)
        {
            this.OnItemsChanged(args);
        }

        internal void Attach(RadPaginationControl radPaginationControl)
        {
            this.owner = radPaginationControl;
        }

        /// <summary>Creates or identifies the element that is used to display the given item.</summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PaginationListControlItem();
        }

        /// <summary>
        /// Determines whether the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <returns>True if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        /// <param name="item">The item to check.</param>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
        }

        /// <summary>Prepares the specified element to display the specified item.</summary>
        /// <param name="element">The element that's used to display the specified item.
        /// </param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var paginationItem = element as PaginationListControlItem;
            var itemElement = item as FrameworkElement;

            var updatedElement = itemElement != null ? itemElement.DataContext : item;

            bool canApplyCustomTemplate = paginationItem != null && (paginationItem.ContentTemplate != null || paginationItem.ContentTemplateSelector != null);

            base.PrepareContainerForItemOverride(element, updatedElement);

            if (paginationItem != null && !canApplyCustomTemplate && !(paginationItem.Content is PaginationItemIndicator) &&
                (this.ItemTemplate == null && this.ItemTemplateSelector == null))
            {
                paginationItem.ShouldDisplayIndicator = true;
            }
            else
            {
                paginationItem.ShouldDisplayIndicator = false;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PaginationListControlAutomationPeer(this);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault();

            if (this.owner != null && this.owner.PageProvider != null)
            {
                this.owner.PageProvider.SelectedItem = selectedItem;
            }

            if (selectedItem != null)
            {
                this.ScrollIntoView(selectedItem);
            }
        }
    }
}
