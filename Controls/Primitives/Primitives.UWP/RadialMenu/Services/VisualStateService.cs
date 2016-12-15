using System;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class VisualStateService
    {
        private DecorationItemButton hoverVisual;
        private RadialSegment currentHoveredItem;
        private RadialSegment lastMenuItemControl;

        private RadRadialMenu owner;

        public VisualStateService(RadRadialMenu owner)
        {
            this.owner = owner;

            this.hoverVisual = new DecorationItemButton();
        }

        internal void UpdateItemHoverState(RadialSegment segment)
        {
            this.UpdateStateButtonState(segment);
            this.UpdateMenuItemControlState(segment);
        }

        internal void UpdateStateButtonState(RadialSegment segment)
        {
            if (this.currentHoveredItem != null && this.currentHoveredItem == segment)
            {
                return;
            }

            if (this.currentHoveredItem != null)
            {
                var visual = this.currentHoveredItem.Visual as DecorationItemButton;

                if (visual != null)
                {
                    visual.IsPointerOver = false;
                }
            }

            if (segment != null)
            {
                this.currentHoveredItem = this.owner.model.GetDecorationSegment(segment.TargetItem);

                var visual = this.currentHoveredItem.Visual as DecorationItemButton;

                if (visual != null)
                {
                    visual.IsPointerOver = true;
                }
            }
        }

        internal void UpdateMenuItemControlState(RadialSegment segment)
        {
            if (this.lastMenuItemControl != null && this.lastMenuItemControl == segment)
            {
                return;
            }

            if (this.lastMenuItemControl != null)
            {
                var lastMenuItem = this.lastMenuItemControl.Visual as RadialMenuItemControl;

                if (lastMenuItem != null)
                {
                    lastMenuItem.IsPointerOver = false;
                    this.owner.menuToolTipContent.HideToolTip();
                    this.lastMenuItemControl = null;
                }
            }

            if (segment != null)
            {
                this.lastMenuItemControl = segment;

                var lastMenuItem = this.lastMenuItemControl.Visual as RadialMenuItemControl;

                if (lastMenuItem != null)
                {
                    lastMenuItem.IsPointerOver = true;

                    if (this.lastMenuItemControl.TargetItem != null)
                    {
                        this.owner.menuToolTipContent.ShowToolTip(this.lastMenuItemControl);
                    }
                }
            }
        }
    }
}
