using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal interface IArrangeChild
    {
        Rect LayoutSlot { get; }

        bool TryInvalidateOwner();
    }
}
