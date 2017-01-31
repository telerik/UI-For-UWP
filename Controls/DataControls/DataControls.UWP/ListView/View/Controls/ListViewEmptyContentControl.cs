using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Primitives
{
    public class ListViewEmptyContentControl : RadContentControl, IArrangeChild
    {
        private Rect layoutSlot;
        public Rect LayoutSlot
        {
            get
            {
                return this.layoutSlot;
            }
        }

        public bool TryInvalidateOwner()
        {
            return false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.layoutSlot = new Rect(new Point(0, 0), finalSize);
            return base.ArrangeOverride(finalSize);
        }
    }
}
