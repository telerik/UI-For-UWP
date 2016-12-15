using System;

namespace Telerik.UI.Xaml.Controls.Primitives.Pagination
{
    /// <summary>
    /// Specifies the different visual parts available within a <see cref="RadPaginationControl"/> instance.
    /// </summary>
    [Flags]
    public enum PaginationControlDisplayMode
    {
        /// <summary>
        /// Left and right arrows are visible.
        /// </summary>
        Arrows = 1,

        /// <summary>
        /// The ThumbnailsList part is visible.
        /// </summary>
        Thumbnails = Arrows << 1,

        /// <summary>
        /// The IndexControl part is visible.
        /// </summary>
        IndexLabel = Thumbnails << 1,

        /// <summary>
        /// Both arrows and thumbnails parts are visible.
        /// </summary>
        ArrowsAndThumbnails = Arrows | Thumbnails,

        /// <summary>
        /// Both arrows and index parts are visible.
        /// </summary>
        ArrowsAndIndex = Arrows | IndexLabel,

        /// <summary>
        /// Both thumbnails and index parts are visible.
        /// </summary>
        ThumbnailsAndIndex = Thumbnails | IndexLabel,

        /// <summary>
        /// All parts are visible.
        /// </summary>
        All = Arrows | Thumbnails | IndexLabel
    }
}
