namespace Telerik.UI.Xaml.Controls.Data
{
    internal class SingleItemAnimationContext
    {
        public RadVirtualizingDataControlItem AssociatedItem;
        public int RealizedIndex;
        public double RealizedLength;
        public bool CancelOffset = false;
    }
}
