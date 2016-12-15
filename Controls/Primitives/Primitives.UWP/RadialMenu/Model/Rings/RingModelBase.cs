using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal abstract class RingModelBase : IHitTestArea
    {
        public RingModelBase(Layout layout)
        {
            this.Layout = layout;

            this.HitTestVisible = true;
        }

        public virtual bool IsVisible { get; protected set; }
        public bool HitTestVisible { get; set; }

        internal double InnerRadius { get; set; }
        internal double OuterRadius { get; set; }

        protected virtual Layout Layout { get; private set; }

        public abstract RadialSegment HitTest(RadPolarPoint point);
    }
}
