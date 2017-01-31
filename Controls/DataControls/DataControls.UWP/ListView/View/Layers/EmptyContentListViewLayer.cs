using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ListView.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class EmptyContentListViewLayer : ListViewLayer
    {
        private ListViewEmptyContentControl emptyContentControl;

        internal void UpdateEmptyContent()
        {
            if (this.emptyContentControl == null)
            {
                this.emptyContentControl = new ListViewEmptyContentControl()
                {
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };

                this.AddVisualChild(this.emptyContentControl);
            }

            this.emptyContentControl.Content = this.Owner.EmptyContent;
        }

        internal RadSize MeasureEmptyContent(RadSize availableSize, bool displayEmptyContent)
        {
            RadSize measuredSize = RadSize.Empty;

            if (this.emptyContentControl != null)
            {
                if (displayEmptyContent)
                {
                    this.emptyContentControl.Visibility = Visibility.Visible;

                    this.emptyContentControl.Measure(availableSize.ToSize());
                    var width = double.IsInfinity(availableSize.Width) ? this.emptyContentControl.Width : Math.Max(availableSize.Width, this.emptyContentControl.DesiredSize.Width);

                    if (double.IsNaN(width))
                    {
                        width = 0;
                    }

                    var height = 0d;
                    if (double.IsInfinity(availableSize.Height))
                    {
                        height = this.emptyContentControl.DesiredSize.Height;
                    }
                    else
                    {
                        height = Math.Max(availableSize.Height, this.emptyContentControl.DesiredSize.Height);
                    }

                    if (double.IsNaN(height))
                    {
                        height = 0;
                    }

                    measuredSize = new RadSize(width, height);
                }
                else
                {
                    this.emptyContentControl.Visibility = Visibility.Collapsed;
                }
            }

            return measuredSize;
        }

        internal void ArrangeEmptyContent(RadRect rect)
        {
            this.emptyContentControl.Arrange(rect.ToRect());
            Canvas.SetLeft(this.emptyContentControl, rect.X);
            Canvas.SetTop(this.emptyContentControl, rect.Y);
        }

        protected internal override void DetachUI(Panel parent)
        {
            if (parent == null)
            {
                return;
            }

            if (parent.Children.Contains(this.emptyContentControl))
            {
                parent.Children.Remove(this.emptyContentControl);
            }

            base.DetachUI(parent);
        }

        protected internal override void AttachUI(Panel parent)
        {
            base.AttachUI(parent);

            if (this.emptyContentControl != null && !parent.Children.Contains(this.emptyContentControl))
            {
                parent.Children.Add(this.emptyContentControl);
            }
        }
    }
}
