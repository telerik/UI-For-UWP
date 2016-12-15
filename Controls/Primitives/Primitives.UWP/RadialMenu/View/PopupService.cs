using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal static class PopupService
    {
        private static Popup popup;

        private static Panel overlay;

        internal static RadRadialMenu CurrentAttachedMenu { get; private set; }

        internal static void PositionMenu(RadRadialMenu menu, Point positionPoint)
        {
            Canvas.SetLeft(menu, positionPoint.X);
            Canvas.SetTop(menu, positionPoint.Y);
        }

        internal static void DisplayOverlay()
        {
            if (popup == null)
            {
                popup = new Popup();
                overlay = new Canvas() { Width = Window.Current.Bounds.Width, Height = Window.Current.Bounds.Height };
                popup.Child = overlay;
            }

            overlay.Background = new SolidColorBrush(Colors.Transparent);

            overlay.PointerPressed += PopupService.Overlay_PointerPressed;
        }

        internal static void HideOverlay()
        {
            if (overlay != null)
            {
                overlay.ClearValue(Panel.BackgroundProperty);
                overlay.PointerPressed -= Overlay_PointerPressed;
            }
        }

        internal static void Attach(RadRadialMenu menu)
        {
            if (popup == null)
            {
                popup = new Popup();
                overlay = new Canvas() { Width = Window.Current.Bounds.Width, Height = Window.Current.Bounds.Height };
                popup.Child = overlay;
            }

            if (PopupService.CurrentAttachedMenu != null)
            {
                Detach();
            }

            PopupService.CurrentAttachedMenu = menu;
            overlay.Children.Add(menu);
            popup.IsOpen = true;
        }

        internal static void Detach()
        {
            if (popup != null)
            {
                popup.IsOpen = false;
            }

            if (PopupService.CurrentAttachedMenu != null)
            {
                if (PopupService.CurrentAttachedMenu.TargetElement != null)
                {
                    var behavior = RadRadialContextMenu.GetBehavior(PopupService.CurrentAttachedMenu.TargetElement);
                    if (behavior != null)
                    {
                        behavior.DetachFromTargetElement();
                    }
                }

                overlay.Children.Remove(PopupService.CurrentAttachedMenu);
                PopupService.CurrentAttachedMenu = null;
            }
        }

        private static void Overlay_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (CurrentAttachedMenu == null || !CurrentAttachedMenu.hitTestService.HitTest(e.GetCurrentPoint(CurrentAttachedMenu).Position).Any())
            {
                CurrentAttachedMenu.IsOpen = false;
                HideOverlay();
            }
        }
    }
}
