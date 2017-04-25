using System.Collections;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides an interface for specifying a value options.
    /// </summary>
    public interface IValueOptionsProvider
    {
        /// <summary>
        /// Gets a list of options using a passed <see cref="EntityProperty"/>.
        /// </summary>
        IList GetOptions(EntityProperty property);
    }
}
