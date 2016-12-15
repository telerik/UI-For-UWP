using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class NavidateItemRingModel : RingModel
    {
        public NavidateItemRingModel(Layout layout)
            : base(layout, new NavigateItemLayer())
        {
        }

        protected override void BuildSegments(IEnumerable<RadialMenuItem> menuItems)
        {
            this.Segments = menuItems.Select(c => new RadialNavigateItem { TargetItem = c }).ToList();
        }
    }
}
