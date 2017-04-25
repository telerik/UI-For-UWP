using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents converter that can convert a property value.
    /// </summary>
    public interface IPropertyConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        object Convert(object value);

        /// <summary>
        /// Converts back the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        object ConvertBack(object value);
    }
}
