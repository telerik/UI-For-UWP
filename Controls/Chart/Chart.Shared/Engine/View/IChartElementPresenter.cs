using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines a type which may visualize a logical chart element.
    /// </summary>
    public interface IChartElementPresenter : IElementPresenter
    {
        /// <summary>
        /// Forces re-evaluation of the palette of this instance.
        /// </summary>
        void InvalidatePalette();
    }
}
