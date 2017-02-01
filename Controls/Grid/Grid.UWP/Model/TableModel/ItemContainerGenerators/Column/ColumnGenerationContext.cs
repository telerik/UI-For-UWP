using System;
using System.Linq;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class ColumnGenerationContext : IGenerationContext
    {
        public ColumnGenerationContext(ItemInfo info, bool isFrozen)
        {
            this.Info = info;
            this.IsFrozen = isFrozen;
        }

        public ItemInfo Info { get; set; }

        public bool IsFrozen { get; set; }
    }
}