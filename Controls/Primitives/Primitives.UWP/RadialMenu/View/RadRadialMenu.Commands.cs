using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Telerik.UI.Xaml.Controls.Primitives.Menu.Commands;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a control that enables a user to visualize a set of <see cref="RadialMenuItem"/>.
    /// </summary>
    public partial class RadRadialMenu
    {
        internal void RaiseNavigateCommand(RadialMenuItem targetMenuSegment, RadialMenuItem sourceMenuItem, double startAngle, bool isBackButtonPressed = false)
        {
            if (!isBackButtonPressed)
            {
                startAngle = GetNextLevelStartAngle(startAngle, targetMenuSegment.ChildItems.Count);
            }

            NavigateContext context = new NavigateContext()
            {
                MenuItemTarget = targetMenuSegment,
                MenuItemSource = sourceMenuItem,
                StartAngle = startAngle,
                IsBackButtonPressed = isBackButtonPressed
            };

            if (context != null)
            {
                this.CommandService.ExecuteCommand(CommandId.NavigateToView, context);
            }
        }
    }
}
