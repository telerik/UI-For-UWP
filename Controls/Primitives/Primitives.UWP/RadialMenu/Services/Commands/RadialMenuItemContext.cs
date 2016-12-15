using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Represent a context, passed to the command that is assigned to the RadialMenuItem.
    /// </summary>
    public class RadialMenuItemContext
    {
        /// <summary>
        /// Gets the menu item that has triggered the command.
        /// </summary>
        /// <value>The menu item.</value>
        public RadialMenuItem MenuItem { get; internal set; }

        /// <summary>
        /// Gets the target element that <see cref="RadRadialMenu"/> is associated with when used as a context menu.
        /// </summary>
        /// <value>The target element.</value>
        public FrameworkElement TargetElement { get; internal set; }

        /// <summary>
        /// Gets the <see cref="RadialMenuItem.CommandParameter"/> defined in the <see cref="MenuItem"/>.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter { get; internal set; }
    }
}
