using System;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IGridView :
        IView,
        IUpdateService<UpdateFlags>,
        ICommandService,
        ISelectionService,
        ICurrencyService
    {
        double PhysicalVerticalOffset { get; }

        double PhysicalHorizontalOffset { get; }

        IDecorationPresenter<LineDecorationModel> LineDecorationsPresenter { get; }

        IDecorationPresenter<SelectionRegionModel> SelectionDecorationsPresenter { get; }

        IDecorationPresenter<LineDecorationModel> FrozenLineDecorationsPresenter { get; }

        IDecorationPresenter<SelectionRegionModel> FrozenSelectionDecorationsPresenter { get; }

        RowDetailsService RowDetailsService { get; }

        IDataView GetDataView();

        void RebuildUI();

        void InvalidateHeadersPanelArrange();

        void InvalidateHeadersPanelMeasure();

        void InvalidateCellsPanelArrange();

        void InvalidateCellsPanelMeasure();

        void Arrange(Node node);

        void SetScrollPosition(RadPoint point, bool updateUI, bool updateScrollViewer);

        //// TODO: Find suitable way to rafactor this, so that update service is used (consider if needed).
        void ProcessDataChangeFlags(DataChangeFlags flags);

        void OnDataStatusChanged(DataProviderStatus status);

        void ApplyLayersClipping(RadRect clip, RadRect frozenRect);
    }
}