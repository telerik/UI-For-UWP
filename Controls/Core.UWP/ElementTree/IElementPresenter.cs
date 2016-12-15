using System;
using System.Linq;

namespace Telerik.Core
{
    /// <summary>
    /// Represents an instance that may visualize a <see cref="Element"/> instance on the screen. Typically this interface is implemented by platform-specific types like the XAML Control class.
    /// </summary>
    public interface IElementPresenter
    {
        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Invalidates the visual representation of the specified logical node.
        /// </summary>
        void RefreshNode(object node);

        /// <summary>
        /// Retrieves the desired size of the specified logical node's content.
        /// </summary>
        RadSize MeasureContent(object owner, object content);
    }
}