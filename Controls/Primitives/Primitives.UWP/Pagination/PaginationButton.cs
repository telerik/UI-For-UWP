using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives.Pagination
{
    /// <summary>
    /// Represents a templated button control used in a <see cref="RadPaginationControl"/> for navigation.
    /// </summary>
    public class PaginationButton : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationButton"/> class.
        /// </summary>
        public PaginationButton()
        {
            this.DefaultStyleKey = typeof(PaginationButton);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PaginationButtonAutomationPeer(this);
        }
    }
}
