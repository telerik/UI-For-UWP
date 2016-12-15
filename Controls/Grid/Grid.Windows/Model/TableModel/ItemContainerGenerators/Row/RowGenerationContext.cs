using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class RowGenerationContext : IGenerationContext
    {
        public RowGenerationContext(ItemInfo info, bool isFrozen)
        {
            this.Info = info;
            this.IsFrozen = isFrozen;
        }

        public ItemInfo Info
        {
            get;
            set;
        }

        public bool IsFrozen
        {
            get;

            private set;
        }
    }
}
