using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.Pagination;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the PaginationListControlItem class.
    /// </summary>
    public class PaginationListControlItemAutomationPeer : ListBoxItemAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the PaginationListControlItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">The PaginationListControlItem that is associated with this PaginationListControlItemAutomationPeer.</param>
        public PaginationListControlItemAutomationPeer(PaginationListControlItem owner) 
            : base(owner)
        {
        }

        private PaginationListControlItem PaginationListControlItem
        {
            get
            {
                return this.Owner as PaginationListControlItem;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(PaginationListControlItem);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(PaginationListControlItem);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "pagination list control item";
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            var children = base.GetChildrenCore().ToList();
            if (children != null && children.Count > 0)
            {
                children.RemoveAll(x => x.GetClassName() == nameof(Telerik.UI.Xaml.Controls.Primitives.Pagination.PaginationItemIndicator));
            }

            return children;
        }
    }
}
