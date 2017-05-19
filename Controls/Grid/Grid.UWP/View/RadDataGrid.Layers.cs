using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Grid.View;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        internal static readonly DependencyProperty DecorationLayerProperty =
            DependencyProperty.Register(nameof(DecorationLayer), typeof(DecorationLayer), typeof(RadDataGrid), new PropertyMetadata(null, OnDecorationLayerChanged));

        internal DecorationLayer frozenDecorationLayer;
        internal VisualStateLayer visualStateLayerCache;
        internal VisualStateLayer frozenVisualStateLayerCache;
        internal XamlScrollableAdornerLayer scrolalbleAdornerLayerCache;
        internal XamlOverlayAdornerLayer overlayAdornerLayerCache;
        internal ContentLayer FrozenColumnsContentLayer;
        internal ContentLayer GroupHeadersContentLayer;

        internal Panel FrozenDecorationsHost;
        internal Panel GroupHeadersHost;

        private XamlDragAdornerLayer dragAdornerLayerCache;
        private DecorationLayer decorationLayerCache;
        private SelectionLayer selectionLayerCache;
        private ObservableCollection<ContentLayer> contentLayers;
        private SelectionLayer frozenSelectionLayerCache;
        private Panel decorationsHost;

        /// <summary>
        /// Gets the collection that stores all the available content layers within the grid. The default <see cref="XamlContentLayer"/> is added if no user-defined layers exist.
        /// </summary>
        internal ObservableCollection<ContentLayer> ContentLayers
        {
            get
            {
                return this.contentLayers;
            }
        }

        /// <summary>
        /// Gets or sets the grid layer that is responsible for cell and row decorations.
        /// </summary>
        internal DecorationLayer DecorationLayer
        {
            get
            {
                return this.decorationLayerCache;
            }
            set
            {
                this.SetValue(DecorationLayerProperty, value);
            }
        }

        /// <summary>
        /// Gets the grid layer that is responsible for selection decorations.
        /// </summary>
        internal SelectionLayer SelectionLayer
        {
            get
            {
                return this.selectionLayerCache;
            }
        }

        /// <summary>
        /// Gets the grid layer that is responsible for Positioning drag visuals.
        /// </summary>
        internal XamlDragAdornerLayer DragAdornerLayer
        {
            get
            {
                return this.dragAdornerLayerCache;
            }
        }

        internal ContentLayer GetContentLayerForColumn(DataGridColumn columnDefinition)
        {
            if (columnDefinition != null && columnDefinition.IsFrozen)
            {
                return this.FrozenColumnsContentLayer;
            }

            for (int i = 0; i < this.contentLayers.Count; i++)
            {
                if (this.contentLayers[i].CanProcessColumnDefinition(columnDefinition))
                {
                    return this.contentLayers[i];
                }
            }

            return null;
        }

        internal void UpdateSelectedHeader(DataGridColumnHeader header, bool isSelected)
        {
            if (this.LastSelectedColumn != null && this.LastSelectedColumn.HeaderControl != null && this.LastSelectedColumn != header.Column)
            {
                this.LastSelectedColumn.HeaderControl.IsSelected = false;
            }

            if (header != null)
            {
                header.IsSelected = isSelected;
                this.LastSelectedColumn = header.Column;
            }
        }

        private static void OnDecorationLayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;

            // TODO: Recycle/Update layer UI
            grid.decorationLayerCache = e.NewValue as DecorationLayer;
            grid.lineDecorationsPresenter.Owner = grid.decorationLayerCache;

            if (!grid.IsTemplateApplied)
            {
                return;
            }

            var oldLayer = e.OldValue as DecorationLayer;
            if (oldLayer != null)
            {
                RemoveLayer(oldLayer, grid.decorationsHost);
            }

            if (grid.decorationLayerCache != null)
            {
                grid.AddLayer(grid.decorationLayerCache, grid.decorationsHost);
            }
        }

        private static void RemoveLayer(DataGridLayer layer, Panel parent)
        {
            layer.DetachUI(parent);
            layer.Owner = null;
        }

        private void UpdateLayersOnTemplateApplied()
        {
            // ensure the default XamlContentLayer
            if (this.contentLayers.Count == 0)
            {
                this.contentLayers.Add(new XamlContentLayer());
            }
            foreach (var layer in this.contentLayers)
            {
                this.AddLayer(layer, this.cellsPanel);
            }

            this.editRowLayer = new GridEditRowLayer();

            this.AddLayer(this.EditRowLayer, this.ContentLayers[0].VisualElement as Panel);

            // ensure the default XamlDecorator layer
            if (this.decorationLayerCache == null)
            {
                this.DecorationLayer = new XamlDecorationLayer();
            }

            this.AddLayer(this.decorationLayerCache, this.decorationsHost);

            if (this.frozenDecorationLayer == null)
            {
                // ensure the frozen XamlDecorator layer
                this.frozenDecorationLayer = new XamlDecorationLayer();
                this.frozenLineDecorationsPresenter.Owner = this.frozenDecorationLayer;
            }

            this.AddLayer(this.frozenDecorationLayer, this.FrozenDecorationsHost);

            // ensure the default XamlSelection layer
            if (this.selectionLayerCache == null)
            {
                this.selectionLayerCache = new XamlSelectionLayer();
                this.selectionDecorationsPresenter.Owner = this.selectionLayerCache;
            }

            this.AddLayer(this.selectionLayerCache, this.decorationsHost);

            if (this.frozenSelectionLayerCache == null)
            {
                this.frozenSelectionLayerCache = new XamlSelectionLayer();
                this.frozenSelectionDecorationsPresenter.Owner = this.frozenSelectionLayerCache;
            }

            this.AddLayer(this.frozenSelectionLayerCache, this.FrozenDecorationsHost);

            // ensure the default XamlVisualState layer
            if (this.visualStateLayerCache == null)
            {
                this.visualStateLayerCache = new XamlVisualStateLayer();
            }

            this.AddLayer(this.visualStateLayerCache, this.decorationsHost);

            if (this.dragAdornerLayerCache == null)
            {
                this.dragAdornerLayerCache = new XamlDragAdornerLayer();
            }

            if (this.frozenVisualStateLayerCache == null)
            {
                this.frozenVisualStateLayerCache = new XamlVisualStateLayer();
            }

            this.AddLayer(this.frozenVisualStateLayerCache, this.FrozenDecorationsHost);

            this.AddLayer(this.dragAdornerLayerCache, this.adornerHostPanel);

            if (this.overlayAdornerLayerCache == null)
            {
                this.overlayAdornerLayerCache = new XamlOverlayAdornerLayer();

                this.visualStateService.RegisterDataLoadingListener(this.overlayAdornerLayerCache);
            }

            this.AddLayer(this.overlayAdornerLayerCache, this.adornerHostPanel);

            if (this.scrolalbleAdornerLayerCache == null)
            {
                this.scrolalbleAdornerLayerCache = new XamlScrollableAdornerLayer();
            }

            this.AddLayer(this.scrolalbleAdornerLayerCache, this.scrollableAdornerHostPanel);

            if (this.FrozenColumnsContentLayer == null)
            {
                this.FrozenColumnsContentLayer = new XamlContentLayer();
            }

            this.AddLayer(this.FrozenColumnsContentLayer, this.frozenColumnsHost);

            if (this.GroupHeadersContentLayer == null)
            {
                this.GroupHeadersContentLayer = new XamlContentLayer();
            }

            this.AddLayer(this.GroupHeadersContentLayer, this.GroupHeadersHost);

            this.frozenEditRowLayer = new GridEditRowLayer();

            this.AddLayer(this.FrozenEditRowLayer, this.FrozenColumnsContentLayer.VisualElement as Panel);
        }

        private void UpdateLayersOnTemplateUnapplied()
        {
            foreach (var layer in this.contentLayers)
            {
                RemoveLayer(layer, this.cellsPanel);
            }

            RemoveLayer(this.EditRowLayer, this.ContentLayers[0].VisualElement as Panel);

            RemoveLayer(this.decorationLayerCache, this.decorationsHost);

            RemoveLayer(this.frozenDecorationLayer, this.FrozenDecorationsHost);

            RemoveLayer(this.selectionLayerCache, this.decorationsHost);

            RemoveLayer(this.frozenSelectionLayerCache, this.FrozenDecorationsHost);

            RemoveLayer(this.visualStateLayerCache, this.decorationsHost);

            RemoveLayer(this.frozenVisualStateLayerCache, this.FrozenDecorationsHost);

            RemoveLayer(this.dragAdornerLayerCache, this.adornerHostPanel);

            RemoveLayer(this.overlayAdornerLayerCache, this.adornerHostPanel);

            RemoveLayer(this.scrolalbleAdornerLayerCache, this.scrollableAdornerHostPanel);

            RemoveLayer(this.FrozenColumnsContentLayer, this.frozenColumnsHost);

            RemoveLayer(this.FrozenEditRowLayer, this.FrozenColumnsContentLayer.VisualElement as Panel);

            RemoveLayer(this.GroupHeadersContentLayer, this.GroupHeadersHost);
        }

        private void AddLayer(DataGridLayer layer, Panel parent)
        {
            layer.DetachUI(parent);
            layer.Owner = this;
            layer.AttachUI(parent);
        }

        private void OnContentLayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
}