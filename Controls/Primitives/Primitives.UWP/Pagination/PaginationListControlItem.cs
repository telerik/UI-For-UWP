using System;
using System.Linq;
using System.Windows.Input;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives.Pagination
{
    /// <summary>
    /// Represents visual container, used within a <see cref="PaginationListControl"/> to visualize thumbnail items.
    /// </summary>
    public class PaginationListControlItem : ListBoxItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationListControlItem"/> class.
        /// </summary>
        public PaginationListControlItem()
        {
            this.DefaultStyleKey = typeof(PaginationListControlItem);
        }

        internal bool ShouldDisplayIndicator
        {
            get;
            set;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding
        /// layout pass) call ApplyTemplate. In simplest terms, this means the method is
        /// called just before a UI element displays in your app. Override this method to
        /// influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.ShouldDisplayIndicator)
            {
                VisualStateManager.GoToState(this, "Unbound", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "BoundToItem", true);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PaginationListControlItemAutomationPeer(this);
        }
    }
}
