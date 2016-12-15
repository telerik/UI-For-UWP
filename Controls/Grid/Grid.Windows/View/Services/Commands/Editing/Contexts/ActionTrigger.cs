namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Enumeration that indicated the initiator of an action.
    /// </summary>
    public enum ActionTrigger
    {
        /// <summary>
        /// The action is triggered programmatically.
        /// </summary>
        Programmatic,

        /// <summary>
        /// The action is triggered by a DoubleTap event.
        /// </summary>
        DoubleTap,

        /// <summary>
        /// The action is triggered by a Tap event.
        /// </summary>
        Tap,

        /// <summary>
        /// The action is triggered by a KeyDown event.
        /// </summary>
        Keyboard,

        /// <summary>
        /// The action is triggered by an External Editor.
        /// </summary>
        ExternalEditor,
    }
}
