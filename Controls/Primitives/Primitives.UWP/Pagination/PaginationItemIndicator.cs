using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Pagination
{
    /// <summary>
    /// A special control, use to indicate item in a <see cref="RadPaginationControl"/> when no suitable content is provided.
    /// </summary>
    public class PaginationItemIndicator : RadControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationItemIndicator"/> class.
        /// </summary>
        public PaginationItemIndicator()
        {
            this.DefaultStyleKey = typeof(PaginationItemIndicator);
        }
    }
}
