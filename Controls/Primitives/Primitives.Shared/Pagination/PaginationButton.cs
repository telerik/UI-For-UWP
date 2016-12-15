using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Telerik.Universal.UI.Xaml.Controls.Primitives.Pagination
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
    }
}
