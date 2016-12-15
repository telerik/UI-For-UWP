using System;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class ScrollIntoViewOperation<T>
    {
        internal static readonly int MaxScrollAttempts = 10;

        public ScrollIntoViewOperation(T item, double scrollPosition)
        {
            this.RequestedItem = item;
            this.InitialScrollOffset = scrollPosition;
        }

        public int ScrollAttempts
        {
            get;
            set;
        }

        public double InitialScrollOffset
        {
            get;
            private set;
        }

        public T RequestedItem
        {
            get;
            private set;
        }

        public Action CompletedAction
        {
            get;
            set;
        }
    }
}
