using System;

namespace Telerik.Core
{
    /// <summary>
    /// Represents <see cref="IElementPresenter"/> instance that is the root of the visual scene.
    /// </summary>
    public interface IView : IElementPresenter
    {
        /// <summary>
        /// Gets the visible width of the viewport.
        /// </summary>
        double ViewportWidth
        {
            get;
        }

        /// <summary>
        /// Gets the visible height of the viewport.
        /// </summary>
        double ViewportHeight
        {
            get;
        }
    }
}
