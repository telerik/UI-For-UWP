using System;
using System.Diagnostics;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop.Reorder;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class DataGridColumn
    {
        /// <summary>
        /// Identifies the <see cref="IsCellFlyoutEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCellFlyoutEnabledProperty =
            DependencyProperty.Register(nameof(IsCellFlyoutEnabled), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="CellDecorationStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellDecorationStyleProperty =
            DependencyProperty.Register(nameof(CellDecorationStyle), typeof(Style), typeof(DataGridColumn), new PropertyMetadata(null, OnCellDecorationStyleChanged));

        /// <summary>
        /// Identifies the <see cref="CellDecorationStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellDecorationStyleSelectorProperty =
            DependencyProperty.Register(nameof(CellDecorationStyleSelector), typeof(StyleSelector), typeof(DataGridColumn), new PropertyMetadata(null, OnCellDecorationStyleSelectorChanged));

        private Style cellDecorationStyleCache;
        private StyleSelector cellDecorationStyleSelectorCache;

        /// <summary>
        /// Gets or sets a value indicating whether a flyout will be shown over a cell after holding.
        /// </summary>
        public bool IsCellFlyoutEnabled
        {
            get { return (bool)GetValue(IsCellFlyoutEnabledProperty); }
            set { SetValue(IsCellFlyoutEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> object that defines the background of each cell associated with this column.
        /// The TargetType property of the Style object is <see cref="Rectangle"/>.
        /// </summary>
        public Style CellDecorationStyle
        {
            get
            {
                return this.cellDecorationStyleCache;
            }
            set
            {
                this.SetValue(CellDecorationStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> instance that allows for dynamic decoration on a per cell basis.
        /// </summary>
        public StyleSelector CellDecorationStyleSelector
        {
            get
            {
                return this.cellDecorationStyleSelectorCache;
            }
            set
            {
                this.SetValue(CellDecorationStyleSelectorProperty, value);
            }
        }

        internal virtual bool CanEdit
        {
            get
            {
                return this.CanUserEdit;
            }
        }

        internal DataGridColumnHeader HeaderControl
        {
            get
            {
                if (this.headerControl == null)
                {
                    return null;
                }

                DataGridColumnHeader header;
                if (this.headerControl.TryGetTarget(out header))
                {
                    return header;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the type of the editor that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public abstract object GetEditorType(object item);

        /// <summary>
        /// Creates an instance of the control visualized when the column is not edited.
        /// </summary>
        /// <returns>An instance of the control.</returns>
        public abstract object CreateContainer(object rowItem);

        /// <summary>
        /// Gets the type of the control visualized when the cell is not currently edited.
        /// </summary>
        /// <returns>The type of the control.</returns>
        public abstract object GetContainerType(object rowItem);

        /// <summary>
        /// Prepares the control visualized by the cell when it is not edited.
        /// </summary>
        /// <param name="container">The container visualizing the cell data when it is not edited.</param>
        public abstract void PrepareCell(object container, object value, object item);

        /// <summary>
        /// Clears the bindings, data etc. of the control visualized by the cell when it is not edited.
        /// </summary>
        /// <param name="container">The container visualizing the cell data when it is not edited.</param>
        public virtual void ClearCell(object container)
        {
        }

        internal void PrepareCellDecoration(GridCellModel cell)
        {
            var style = this.ComposeCellDecorationStyle(cell);
            var decorationContainer = cell.DecorationContainer as FrameworkElement;

            if (style != null)
            {
                if (decorationContainer == null)
                {
                    var rowItem = cell.ParentRow == null ? null : cell.ParentRow.ItemInfo.Item;
                    decorationContainer = this.CreateDecorationContainer(rowItem);
                    cell.DecorationContainer = decorationContainer;
                }

                decorationContainer.Style = style;
            }
            else if (decorationContainer != null)
            {
                decorationContainer.ClearValue(FrameworkElement.StyleProperty);
            }
        }

        internal void PrepareCell(GridCellModel cell)
        {
            this.PrepareCell(cell.Container, cell.Value, cell.ParentRow.ItemInfo.Item);
        }

        internal void ClearCell(GridCellModel cell)
        {
            this.ClearCell(cell.Container);
        }

        internal virtual RadSize MeasureCell(GridCellModel cell, double availableWidth)
        {
            // This method is called by the XamlContentLayer
            RadSize size = RadSize.Empty;

            if (this.sizeModeCache == DataGridColumnSizeMode.Fixed)
            {
                availableWidth = this.widthCache;
            }

            UIElement container = cell.Container as UIElement;
            if (container != null)
            {
                size = this.MeasureCellContainer(availableWidth, container).ToRadSize();
            }

            if (this.sizeModeCache == DataGridColumnSizeMode.Fixed)
            {
                size.Width = this.widthCache;
            }

            return size;
        }

        internal virtual RadSize MeasureEditCell(GridCellEditorModel cell, double availableWidth)
        {
            if (!this.CanEdit)
            {
                return this.MeasureCell(cell, availableWidth);
            }

            // This method is called by the XamlContentLayer
            RadSize size = RadSize.Empty;

            if (this.sizeModeCache == DataGridColumnSizeMode.Fixed)
            {
                availableWidth = this.widthCache;
            }

            var pair = cell.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;
            if (pair != null)
            {
                size = this.MeasureCellContainer(availableWidth, pair.Item1).ToRadSize();

                pair.Item2.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            if (this.sizeModeCache == DataGridColumnSizeMode.Fixed)
            {
                size.Width = this.widthCache;
            }

            return size;
        }

        internal virtual Size MeasureCellContainer(double availableWidth, UIElement container)
        {
            container.Measure(new Size(availableWidth, double.PositiveInfinity));

            return container.DesiredSize;
        }

        internal virtual void PrepareColumnHeaderCell(GridHeaderCellModel cell)
        {
            var header = cell.Container as DataGridColumnHeader;
            if (header == null)
            {
                Debug.Assert(false, "Unknown column decorator");
                return;
            }

            this.headerControl = new WeakReference<DataGridColumnHeader>(header);

            header.Column = this;
            header.Content = this.Header;
            header.IsFiltered = this.isFiltered;

            ((IReorderItem)header).LogicalIndex = cell.ItemInfo.LayoutInfo.Line;

            // Force update of all bindings. This is needed since any binding to dependency property does not recieve notification when property is changed.
            header.DataContext = null;
            header.DataContext = cell.ItemInfo.Item;
            header.Owner = this.Model.GridView as RadDataGrid;
            header.FilterGlyphVisibility = this.CanFilter && header.Owner.ColumnDataOperationsMode == ColumnDataOperationsMode.Inline ? Visibility.Visible : Visibility.Collapsed;

            header.ResizeHandleVisiblity = this.CanUserResize && header.Owner.ColumnResizeHandleDisplayMode == DataGridColumnResizeHandleDisplayMode.Always ? Visibility.Visible : Visibility.Collapsed;

            header.IsSelected = this == header.Owner.LastSelectedColumn;

            if (this.headerStyleCache != null)
            {
                header.Style = this.headerStyleCache;
            }
            else
            {
                header.ClearValue(FrameworkElement.StyleProperty);
            }

            if (cell.ItemInfo.LayoutInfo.Line == this.Model.ColumnPool.Layout.ItemsSource.Count - 1)
            {
                // Apply padding when last column to offset from column chooser button.
                if (header.Owner.CanUserChooseColumns)
                {
                    if (this.headerStyleCache != null && this.headerStyleCache.Setters.OfType<Setter>().Any(c => c.Property == DataGridColumnHeader.PaddingProperty))
                    {
                        var oldPadding = (Thickness)this.headerStyleCache.Setters.OfType<Setter>().First(c => c.Property == DataGridColumnHeader.PaddingProperty).Value;
                        header.Padding = new Thickness(oldPadding.Left, oldPadding.Top, oldPadding.Right + header.Owner.ColumnReorderServicePanel.ActualWidth, oldPadding.Bottom);
                    }
                    else
                    {
                        header.Padding = new Thickness(0, 0, header.Owner.ColumnReorderServicePanel.ActualWidth, 0);
                    }
                }
                else
                {
                    if (this.headerStyleCache != null && this.headerStyleCache.Setters.OfType<Setter>().Any(c => c.Property == DataGridColumnHeader.PaddingProperty))
                    {
                        var oldPadding = (Thickness)this.headerStyleCache.Setters.OfType<Setter>().First(c => c.Property == DataGridColumnHeader.PaddingProperty).Value;
                        header.Padding = oldPadding;
                    }
                    else
                    {
                        header.Padding = new Thickness(0);
                    }
                }
            }
        }

        internal virtual void ClearColumnHeaderCell(GridHeaderCellModel cell)
        {
            this.headerControl = null;

            // TODO: Consider which properties should be cleared in order to avoid memory leaks.
            var header = cell.Container as DataGridColumnHeader;
            if (header == null)
            {
                Debug.Assert(false, "Unknown column decorator");
                return;
            }

            header.ClearValue(FrameworkElement.WidthProperty);
            header.ClearValue(FrameworkElement.HeightProperty);
            header.Column = null;
            header.Owner = null;
            header.Content = null;

            //// TODO: Is this the best place to reset the AutoWidth?
            //// TODO: Not resetting AutoWidth causes weird behavior upon editing - if you add long text value, the column grows and then if you delete the long text, the column stays wide
            ////this.AutoWidth = 0;
        }

        internal virtual RadRect ApplyLayoutSlotAlignment(GridCellModel cell)
        {
            FrameworkElement element = cell.Container as FrameworkElement;
            if (element == null)
            {
                return cell.layoutSlot;
            }

            RadSize desiredSize = cell.DesiredSize;
            RadRect layoutSlot = cell.layoutSlot;

            if (desiredSize.Width < layoutSlot.Width)
            {
                switch (element.HorizontalAlignment)
                {
                    case HorizontalAlignment.Center:
                        layoutSlot.X += (layoutSlot.Width - desiredSize.Width) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        layoutSlot.X = layoutSlot.Right - desiredSize.Width;
                        break;
                }
            }

            if (desiredSize.Height < layoutSlot.Height)
            {
                switch (element.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        layoutSlot.Y += (layoutSlot.Height - desiredSize.Height) / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        layoutSlot.Y = layoutSlot.Bottom - desiredSize.Height;
                        break;
                }
            }

            return layoutSlot;
        }

        internal abstract object CreateEditorContainer(object rowItem, object containerType);

        internal abstract void PrepareEditorContainer(GridCellEditorModel editor);

        internal abstract void ClearEditorContainer(GridCellEditorModel editor);

        internal virtual bool IsCellValid(GridCellEditorModel editor)
        {
            return true;
        }

        internal virtual bool IsCellValid(object item)
        {
            return true;
        }

        internal virtual void TryFocusCell(DataGridCellInfo cellInfo, FocusState state)
        {
        }

        /// <summary>
        /// Creates a <see cref="FrameworkElement"/> instance that will be used as a decoration container for the cell represented by the specified row item and the current column instance.
        /// </summary>
        /// <param name="rowItem">The row item the cell belongs to.</param>
        protected virtual FrameworkElement CreateDecorationContainer(object rowItem)
        {
            return new Rectangle();
        }

        private static void OnCellDecorationStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as DataGridColumn;
            definition.cellDecorationStyleCache = e.NewValue as Style;
            definition.OnPropertyChange(UpdateFlags.AffectsContent);
        }

        private static void OnCellDecorationStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as DataGridColumn;
            definition.cellDecorationStyleSelectorCache = e.NewValue as StyleSelector;
            definition.OnPropertyChange(UpdateFlags.AffectsContent);
        }

        private Style ComposeCellDecorationStyle(GridCellModel cell)
        {
            if (this.cellDecorationStyleCache != null)
            {
                return this.cellDecorationStyleCache;
            }

            if (this.cellDecorationStyleSelectorCache != null)
            {
                var parentRow = cell.parent as GridRowModel;
                var selectContext = new DataGridCellInfo(parentRow.ItemInfo.Item, cell.Column);
                return this.cellDecorationStyleSelectorCache.SelectStyle(selectContext, null);
            }

            return null;
        }
    }
}
