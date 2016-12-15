using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Report <see cref="FilterDescription"/> implementation.
    /// </summary>
    internal sealed class PropertyFilterDescription : PropertyFilterDescriptionBase
    {
        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new PropertyFilterDescription();
        }

        /// <inheritdoc />
        protected override void CloneOverride(Cloneable source)
        {
        }
    }
}