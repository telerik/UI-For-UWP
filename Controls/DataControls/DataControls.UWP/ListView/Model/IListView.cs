using Telerik.Core;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal interface IListView : IOrientedParentView, ICommandService, ISelectionService, ISupportItemAnimation, IView, ICurrencyService, IAnimatingService, IItemCheckBoxService
    {
        UpdateServiceBase<ListView.UpdateFlags> UpdateService { get; }

        IUIContainerGenerator<GeneratedItemModel, ItemGenerationContext> ContainerGenerator { get; }

        void OnDataStatusChanged(DataProviderStatus status);

        void ScrollToTop();

        void SetScrollPosition(RadPoint point, bool updateUI, bool updateScrollViewer);
    }
}
