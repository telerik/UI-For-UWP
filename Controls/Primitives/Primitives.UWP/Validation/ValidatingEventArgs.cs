using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Arguments used to raise Validating event of <see cref="ValidationControl"/>.
    /// </summary>
    public class ValidatingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingEventArgs" /> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public ValidatingEventArgs(IList<object> errors)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Gets the errors displayed in validation control.
        /// </summary>
        /// <value>The errors.</value>
        public IList<object> Errors { get; private set; }
    }
}
