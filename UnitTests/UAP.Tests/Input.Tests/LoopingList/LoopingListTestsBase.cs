using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Input.Tests;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;

namespace Telerik.UI.Xaml.Controls.Tests
{
    public abstract class LoopingListTestsBase : RadControlUITest
    {
        protected RadLoopingList loopingList;

        public override void TestInitialize()
        {
            base.TestInitialize();
            this.loopingList = new RadLoopingList();
        }

        protected List<int> CreateSource(int count)
        {
            var items = new List<int>();
            for (var i = 0; i < count; i++)
            {
                items.Add(i);
            }

            return items;
        }
    }
}
