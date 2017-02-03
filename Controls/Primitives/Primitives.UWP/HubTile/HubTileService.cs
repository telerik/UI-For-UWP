using System;
using System.Linq;
using Telerik.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// HubTileService provides the ability to group hub tiles with a group tag and to freeze or unfreeze groups of hub tiles.
    /// </summary>
    public static class HubTileService
    {
        /// <summary>
        /// Identifies the GroupTag attached property.
        /// </summary>
        public static readonly DependencyProperty GroupTagProperty =
            DependencyProperty.RegisterAttached("GroupTag", typeof(string), typeof(HubTileService), new PropertyMetadata(null));

        private static WeakReferenceList<HubTileBase> tiles = new WeakReferenceList<HubTileBase>();

        internal static WeakReferenceList<HubTileBase> Tiles
        {
            get
            {
                return tiles;
            }
        }

        /// <summary>
        /// Gets the group tag of the provided hub tile.
        /// </summary>
        /// <param name="hubTile">The hub tile to get the group tag from.</param>
        /// <returns>Returns the group tag of the provided hub tile.</returns>
        /// <example>
        /// <code language="c#">
        ///    RadHubTile hubtile = new RadHubTile();
        ///    HubTileService.SetGroupTag(hubtile, "first");
        ///    var currentTag = HubTileService.GetGroupTag(hubtile);
        /// </code> 
        /// </example>
        public static string GetGroupTag(DependencyObject hubTile)
        {
            if (hubTile == null)
            {
                throw new ArgumentNullException(nameof(hubTile));
            }

            return (string)hubTile.GetValue(HubTileService.GroupTagProperty);
        }

        /// <summary>
        /// Sets the group tag of the specified hub tile to the specified value.
        /// </summary>
        /// <example>
        /// <code language="c#">
        /// RadHubTile hubtile = new RadHubTile();
        /// HubTileService.SetGroupTag(hubtile, "first");
        /// </code> 
        /// </example>
        /// <param name="hubTile">The hub tile to set the group tag to.</param>
        /// <param name="value">The tile's new group tag.</param>
        public static void SetGroupTag(DependencyObject hubTile, string value)
        {
            if (hubTile == null)
            {
                throw new ArgumentNullException(nameof(hubTile));
            }

            hubTile.SetValue(HubTileService.GroupTagProperty, value);
        }

        /// <summary>
        /// Freezes a group of hub tiles.
        /// </summary>
        /// <param name="groupTag">The groupTag which will be used when searching for hub tiles to freeze.</param>
        public static void FreezeGroup(string groupTag)
        {
            UpdateIsFrozen(groupTag, true);
        }

        /// <summary>
        /// Unfreezes a group of hub tiles.
        /// </summary>
        /// <param name="groupTag">The groupTag which will be used when searching for hub tiles to unfreeze.</param>
        public static void UnfreezeGroup(string groupTag)
        {
            UpdateIsFrozen(groupTag, false);
        }

        private static void UpdateIsFrozen(string groupKey, bool isFrozen)
        {
            foreach (HubTileBase tile in tiles)
            {
                if (GetGroupTag(tile) == groupKey)
                {
                    tile.IsFrozen = isFrozen;
                }
            }
        }
    }
}
