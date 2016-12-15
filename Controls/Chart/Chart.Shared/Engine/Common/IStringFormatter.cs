using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines a type that can convert some input to its string representation.
    /// </summary>
    public interface IContentFormatter
    {
        /// <summary>
        /// Formats the content, using the provided owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="content">The content to be formatted.</param>
        /// <returns>The formatted content.</returns>
        object Format(object owner, object content);
    }
}
