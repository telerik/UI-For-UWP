using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data.Common.PullToRefresh
{
    internal class PullToRefreshOperation
    {
        public double SwipeTheshold { get; set; }

        public bool ExceededTreshold { get; private set; }

        public FrameworkElement Root { get; private set; }
        public FrameworkElement ChildElement { get; private set; }



        public double GetOffset(bool isHorizontal)
        {
            if (this.Transform == null)
            {
                return 0;
            }

            var point = this.Transform.TransformPoint(new Point(0, 0));

            var offset = isHorizontal ? point.X : point.Y;

            if (offset > this.SwipeTheshold)
            {
                this.ExceededTreshold = true;
            }

            return offset;
        }
    }
}
