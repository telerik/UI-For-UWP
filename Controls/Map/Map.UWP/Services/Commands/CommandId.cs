namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines the commands, available in a <see cref="RadMap"/> instance.
    /// </summary>
    public enum CommandId
    {
        /// <summary>
        /// The command is not familiar to <see cref="RadMap"/>.
        /// </summary>
        Unknown,

        /// <summary>
        /// A command associated with a change either in the <see cref="RadMap.Center"/> or <see cref="RadMap.ZoomLevel"/> properties (or both) triggered by a user input through the <see cref="MapPanAndZoomBehavior"/>.
        /// </summary>
        ViewChanged,

        /// <summary>
        /// A command associated with a change in the SelectedShapes property of a <see cref="MapShapeSelectionBehavior"/> instance.
        /// </summary>
        ShapeSelectionChanged,

        /// <summary>
        /// A command that is triggered when a <see cref="IShapeDataSource"/> instance is specified as a source to a <see cref="MapShapeLayer"/> instance and the <see cref="IShapeDataSource.Shapes"/> are prepared.
        /// </summary>
        ShapeLayerSourceChanged
    }
}
