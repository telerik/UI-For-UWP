using System.ComponentModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Base class used in filtering.
    /// </summary>
    internal abstract class Condition : SettingsNode, IFilter
    {
        /// <inheritdoc />
        public abstract bool PassesFilter(object item);

        bool IFilter.PassesFilter(object item)
        {
            return this.PassesFilter(item);
        }
    }
}