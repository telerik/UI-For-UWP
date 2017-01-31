using System;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class BringIntoViewOperation
    {
        internal static readonly int MaxScrollAttempts = 10;

        public BringIntoViewOperation(object item)
        {
            this.RequestedItem = item;
        }

        public double LastAverageItemLength { get; set; }

        public int ScrollAttempts { get; set; }

        public object RequestedItem { get; private set; }

        public Action CompletedAction { get; set; }
    }
}