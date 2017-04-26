using System.Text;
using Telerik.UI.Xaml.Controls.Primitives.Pagination;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the PaginationIndexLabelControl class.
    /// </summary>
    public class PaginationIndexLabelControlAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the PaginationIndexLabelControlAutomationPeer class.
        /// </summary>
        /// <param name="owner">The PaginationIndexLabelControl that is associated with this PaginationIndexLabelControlAutomationPeer.</param>
        public PaginationIndexLabelControlAutomationPeer(PaginationIndexLabelControl owner) 
            : base(owner)
        {
        }
        
        private PaginationIndexLabelControl PaginationIndexLabelControlOwner
        {
            get
            {
                return this.Owner as PaginationIndexLabelControl;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.Pagination.PaginationIndexLabelControl);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.Pagination.PaginationIndexLabelControl);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "pagination index label control";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var indexLabelText = new StringBuilder();
            if (!string.IsNullOrEmpty(this.PaginationIndexLabelControlOwner.CurrentIndexDisplayValue))
            {
                indexLabelText.Append(this.PaginationIndexLabelControlOwner.CurrentIndexDisplayValue);
            }

            if (!string.IsNullOrEmpty(this.PaginationIndexLabelControlOwner.Separator))
            {
                indexLabelText.Append(this.PaginationIndexLabelControlOwner.Separator);
            }

            if (!string.IsNullOrEmpty(this.PaginationIndexLabelControlOwner.ItemCountDisplayValue))
            {
                indexLabelText.Append(this.PaginationIndexLabelControlOwner.ItemCountDisplayValue);
            }

            return indexLabelText.ToString();
        }
    }
}
