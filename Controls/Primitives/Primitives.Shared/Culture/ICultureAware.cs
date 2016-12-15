using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines an object instance which behavior/state depends on its current CultureInfo.
    /// </summary>
    internal interface ICultureAware
    {
        /// <summary>
        /// Gets the current <see cref="CultureInfo"/> object used by this instance.
        /// </summary>
        CultureInfo CurrentCulture
        {
            get;
        }

        /// <summary>
        /// A callback, used by the <see cref="CultureService"/> class to notify for a change in the CultureName property.
        /// </summary>
        void OnCultureChanged(CultureInfo oldValue, CultureInfo newValue);
    }
}
