namespace Telerik.UI.Xaml.Controls.Primitives.Menu.Commands
{
    /// <summary>
    /// Defines the known commands that are available within a <see cref="RadRadialMenu"/> control.
    /// </summary>
    public enum CommandId
    {
        /// <summary>
        /// The command is not familiar to the radial menu. 
        /// </summary>
        Unknown,

        /// <summary>
        /// A command associated with the action of opening the radial menu.
        /// </summary>
        Open,

        /// <summary>
        /// A command associated with the action of closing the radial menu.
        /// </summary>
        Close,

        /// <summary>
        /// A command associated with the action of navigating to specific <see cref="RadialMenuItem"/>.
        /// </summary>
        NavigateToView,

        /// <summary>
        /// A command associated with the action of navigating back to the previous <see cref="RadialMenuItem"/>.
        /// </summary>
        NavigateBack
    }
}