using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    public partial class RadRadialMenu
    {
        internal void OnPointerMoved(Point relativePosition)
        {
            var radialSegments = this.hitTestService.HitTest(relativePosition);

            var item = radialSegments.OfType<RadialSegment>().FirstOrDefault();

            var itemToHover = item != null && item.TargetItem.IsEnabled ? item : null;

            this.visualstateService.UpdateItemHoverState(itemToHover);
        }

        internal void OnPointerTapped(Point relativePosition)
        {
            var radialSegments = this.hitTestService.HitTest(relativePosition).ToArray();

            var navigateItem = radialSegments.OfType<RadialNavigateItem>().FirstOrDefault();
            var contentItem = radialSegments.OfType<RadialSegment>().FirstOrDefault();

            if (navigateItem != null && navigateItem.TargetItem.CanNavigate)
            {
                this.RaiseNavigateCommand(navigateItem.TargetItem, this.model.viewState.MenuLevels.FirstOrDefault(), navigateItem.LayoutSlot.StartAngle);
            }
            else if (navigateItem == null && contentItem != null && contentItem.TargetItem.IsEnabled)
            {
                contentItem.TargetItem.IsSelected = !contentItem.TargetItem.IsSelected;
                contentItem.TargetItem.ExecuteCommand();
            }
        }
    }
}
