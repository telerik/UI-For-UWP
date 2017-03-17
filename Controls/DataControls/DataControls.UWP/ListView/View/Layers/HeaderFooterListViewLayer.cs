using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class HeaderFooterListViewLayer : ListViewLayer
    {
        internal double topOffset;
        internal double bottomOffset;

        private ContentControl headerControl;
        private ContentControl footerControl;
        private bool displayHeader;
        private bool displayFooter;

        internal void UpdateHeader(bool displayHeader)
        {
            this.displayHeader = displayHeader;
            if (this.displayHeader)
            {
                if (this.headerControl == null)
                {
                    this.headerControl = new ContentControl()
                    {
                        VerticalContentAlignment = VerticalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        IsTabStop = false,
                    };

                    this.AddVisualChild(this.headerControl);
                }

                this.headerControl.Content = this.Owner.ListHeader;
            }
            else
            {
                // remove header
            }
        }

        internal void UpdateFooter(bool displayFooter)
        {
            this.displayFooter = displayFooter;
            if (this.displayFooter)
            {
                if (this.footerControl == null)
                {
                    this.footerControl = new ContentControl()
                    {
                        VerticalContentAlignment = VerticalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        IsTabStop = false,
                    };

                    this.AddVisualChild(this.footerControl);
                }

                this.footerControl.Content = this.Owner.ListFooter;
            }
            else
            {
                // remove footer
            }
        }

        internal RadSize MeasureHeader(RadSize availableSize)
        {
            if (this.displayHeader)
            {
                this.headerControl.Measure(availableSize.ToSize());

                return new RadSize(availableSize.Width, this.headerControl.DesiredSize.Height);
            }

            return RadSize.Empty;
        }

        internal RadSize MeasureFooter(RadSize availableSize)
        {
            if (this.displayFooter)
            {
                this.footerControl.Measure(availableSize.ToSize());

                return new RadSize(availableSize.Width, this.footerControl.DesiredSize.Height);
            }

            return RadSize.Empty;
        }

        internal void ArrangeHeader(RadRect rect)
        {
            if (this.displayHeader)
            {
                this.headerControl.Arrange(rect.ToRect());
                Canvas.SetLeft(this.headerControl, rect.X);
                Canvas.SetTop(this.headerControl, rect.Y);
                this.topOffset = rect.Height;
            }
        }

        internal void ArrangeFooter(RadRect rect)
        {
            if (this.displayFooter)
            {
                this.footerControl.Arrange(rect.ToRect());
                Canvas.SetLeft(this.footerControl, rect.X);
                Canvas.SetTop(this.footerControl, rect.Y);
                this.bottomOffset = rect.Height;
            }
        }

        protected internal override void DetachUI(Panel parent)
        {
            if (parent == null)
            {
                return;
            }

            if (this.displayHeader)
            {
                parent.Children.Remove(this.headerControl);
            }

            if (this.displayFooter)
            {
                parent.Children.Remove(this.footerControl);
            }

            base.DetachUI(parent);
        }

        protected internal override void AttachUI(Panel parent)
        {
            base.AttachUI(parent);

            if (this.headerControl != null && !parent.Children.Contains(this.headerControl))
            {
                parent.Children.Add(this.headerControl);
            }

            if (this.footerControl != null && !parent.Children.Contains(this.footerControl))
            {
                parent.Children.Add(this.footerControl);
            }
        }
    }
}