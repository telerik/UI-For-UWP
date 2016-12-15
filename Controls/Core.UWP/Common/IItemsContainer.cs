using Windows.UI.Xaml;

namespace Telerik.Core
{
    internal interface IItemsContainer
    {
        FrameworkElement[] ViewportItems
        {
            get;
        }
    }
}
