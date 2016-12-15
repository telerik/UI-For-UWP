using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// This class represents the tool tip for <see cref="RadRadialMenu"/>.
    /// </summary>
    public class MenuToolTip : ContentControl
    {
        private RadRadialMenu owner;
        private DispatcherTimer delayTimer;

        /// <summary>
        /// Initializes a new instance of the MenuToolTip class.
        /// </summary>
        public MenuToolTip()
        {
            this.DefaultStyleKey = typeof(MenuToolTip);

            this.delayTimer = new DispatcherTimer();
            this.delayTimer.Interval = TimeSpan.FromMilliseconds(800);
            this.SizeChanged += this.MenuToolTipSizeChanged;
        }

        internal RadRadialMenu Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        internal void ShowToolTip(RadialSegment itemControlSegment)
        {
            var menuItemControl = itemControlSegment.Visual as RadialMenuItemControl;

            if (this.Owner.ShowToolTip && !this.Owner.tooltip.IsOpen)
            {
                if (this.IsToolTipContentApplied(menuItemControl.Header, itemControlSegment.TargetItem.ToolTipContent))
                {
                    this.Owner.tooltip.IsOpen = true;
                }

                this.delayTimer.Stop();
            }
        }

        internal void HideToolTip(bool withDelay = false)
        {
            if (this.Owner.ShowToolTip && this.Owner.tooltip.IsOpen)
            {
                if (withDelay)
                {
                    this.delayTimer.Start();
                    this.delayTimer.Tick += (e, a) =>
                    {
                        delayTimer.Stop();
                        this.Owner.tooltip.IsOpen = false;
                    };
                }
                else
                {
                    this.Owner.tooltip.IsOpen = false;
                }
            }
        }

        private void MenuToolTipSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var parent = this.Parent as Popup;

            if (parent == null || this.Owner == null)
            {
                return;
            }

            var radialMenuTransformed = this.Owner.TransformToVisual(Window.Current.Content);
            Point radialMenuPosition = radialMenuTransformed.TransformPoint(new Point(0, 0));

            if (radialMenuPosition.Y - this.DesiredSize.Height < 0)
            {
                parent.VerticalOffset = this.Owner.ActualHeight / 2;
            }
            else
            {
                parent.VerticalOffset = (-this.Owner.ActualHeight / 2) - this.DesiredSize.Height;
            }

            parent.HorizontalOffset = -this.DesiredSize.Width / 2;
        }

        private bool IsToolTipContentApplied(object header, object toolTipContent)
        {
            if (toolTipContent != null)
            {
                this.Content = toolTipContent;
                return true;
            }

            var headerElement = header as FrameworkElement;

            if (headerElement == null && header != null)
            {
                this.Content = header;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
