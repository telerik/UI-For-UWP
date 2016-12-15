using System;
using System.ComponentModel;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.RangeSlider
{
    /// <summary>
    /// This class represents the tool tip for RangeSliderPrimitive.
    /// </summary>
    public class RangeToolTip : RadControl
    {
        private RangeSliderPrimitive owner;
        private ToolTipContext context;
        private DispatcherTimer delayTimer;

        /// <summary>
        /// Initializes a new instance of the RangeToolTip class.
        /// </summary>
        public RangeToolTip()
        {
            this.DefaultStyleKey = typeof(RangeToolTip);
            this.context = new ToolTipContext();
            this.DataContext = this.context;

            this.delayTimer = new DispatcherTimer();
            this.delayTimer.Interval = TimeSpan.FromMilliseconds(800);
        }

        internal RangeSliderPrimitive Owner
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

        internal void UpdateTooltipPosition(Point position)
        {
            var parent = this.Parent as Popup;

            if (this.Owner == null || this.Parent == null || parent == null || !this.Owner.ShowRangeToolTip)
            {
                return;
            }

            if (this.Owner.Orientation == Orientation.Horizontal)
            {
                this.TruncateToBoundsHorizontal(parent, position);
            }
            else
            {
                this.TruncateToBoundsVertical(parent, position);
            }

            if (this.Owner.valueToolTip.IsEnabled)
            {
                this.Owner.valueToolTip.IsOpen = false;
            }
        }

        internal void UpdateToolTipContext()
        {
            this.context.StartValue = this.Owner.ReturnFormatedValue(this.owner.SelectionStart);
            this.context.EndValue = this.Owner.ReturnFormatedValue(this.Owner.SelectionEnd);

            this.context.Range = this.Owner.ReturnFormatedValue(this.Owner.SelectionEnd - this.Owner.SelectionStart);
        }

        internal void ShowToolTip()
        {
            if (this.Owner.ShowRangeToolTip && !this.Owner.rangeToolTip.IsOpen)
            {
                this.Owner.rangeToolTip.IsOpen = true;
                this.delayTimer.Stop();
            }
        }

        internal void HideToolTip(bool withDelay = false)
        {
            if (this.Owner.ShowRangeToolTip && this.Owner.rangeToolTip.IsOpen)
            {
                if (withDelay)
                {
                    this.delayTimer.Start();
                    this.delayTimer.Tick += (e, a) =>
                    {
                        delayTimer.Stop();
                        this.Owner.rangeToolTip.IsOpen = false;
                    };
                }
                else
                {
                    this.Owner.rangeToolTip.IsOpen = false;
                }
            }
        }

        private void TruncateToBoundsVertical(Popup parent, Point position)
        {
            if (this.Owner.sliderPrimitivePosition.X < Math.Abs(position.X))
            {
                parent.HorizontalOffset = this.Owner.DesiredSize.Width;
            }
            else
            {
                parent.HorizontalOffset = position.X;
            }

            if (this.Owner.sliderPrimitivePosition.Y + position.Y < 0)
            {
                parent.VerticalOffset = -this.Owner.sliderPrimitivePosition.Y;
            }
            else if (position.Y + this.DesiredSize.Height + this.Owner.sliderPrimitivePosition.Y > Window.Current.Bounds.Height)
            {
                double newOffset = position.Y + this.DesiredSize.Height + this.Owner.sliderPrimitivePosition.Y - Window.Current.Bounds.Height;
                parent.VerticalOffset = position.Y - newOffset;
            }
            else
            {
                parent.VerticalOffset = position.Y;
            }
        }

        private void TruncateToBoundsHorizontal(Popup parent, Point position)
        {
            if (this.Owner.sliderPrimitivePosition.Y < Math.Abs(position.Y))
            {
                parent.VerticalOffset = this.Owner.DesiredSize.Height;
            }
            else
            {
                parent.VerticalOffset = position.Y;
            }

            if (this.Owner.sliderPrimitivePosition.X + position.X < 0)
            {
                parent.HorizontalOffset = -this.Owner.sliderPrimitivePosition.X;
            }
            else if (position.X + this.DesiredSize.Width + this.Owner.sliderPrimitivePosition.X > Window.Current.Bounds.Width)
            {
                double newOffset = position.X + this.DesiredSize.Width + this.Owner.sliderPrimitivePosition.X - Window.Current.Bounds.Width;
                parent.HorizontalOffset = position.X - newOffset;
            }
            else
            {
                parent.HorizontalOffset = position.X;
            }
        }
    }
}
