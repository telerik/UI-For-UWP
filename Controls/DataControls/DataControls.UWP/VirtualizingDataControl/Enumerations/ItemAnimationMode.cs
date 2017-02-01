using System;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Defines the different modes for item animation support in
    /// <see cref="RadVirtualizingDataControl"/>.
    /// </summary>
    [Flags]
    public enum ItemAnimationMode
    {
        /// <summary>
        /// No item animations are played.
        /// </summary>
        None = 0,

        /// <summary>
        /// If the <see cref="RadVirtualizingDataControl.ItemAddedAnimation"/> property
        /// is defined, the control will animate all viewport items using this animation
        /// when it is initially bound to a data source.
        /// </summary>
        PlayOnNewSource = 1,

        /// <summary>
        /// If the <see cref="RadVirtualizingDataControl.ItemAddedAnimation"/> property
        /// is defined, the control will animate each new item that is added to the
        /// source collection and is realized in the viewport.
        /// </summary>
        PlayOnAdd = 2,

        /// <summary>
        /// If the <see cref="RadVirtualizingDataControl.ItemRemovedAnimation"/> property
        /// is defined, the control will animate each new item that is removed from the
        /// source collection and was realized in the viewport.
        /// </summary>
        PlayOnRemove = 4,

        /// <summary>
        /// If the <see cref="RadVirtualizingDataControl.ItemRemovedAnimation"/> property
        /// is defined, the control will animate all viewport items out of the viewport.
        /// </summary>
        PlayOnSourceReset = 8,

        /// <summary>
        /// Animations are played always when the control is bound to a collection, when the source is reset, or when
        /// items are added/removed from the source collection.
        /// </summary>
        PlayAll = PlayOnAdd | PlayOnNewSource | PlayOnRemove | PlayOnSourceReset
    }
}
