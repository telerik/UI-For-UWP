using System;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        private RadPoint lastScrollOffset;
        private ScrollViewer scrollViewer;

        double IGridView.PhysicalVerticalOffset
        {
            get
            {
                return this.lastScrollOffset.Y;
            }
        }

        double IGridView.PhysicalHorizontalOffset
        {
            get
            {
                return this.lastScrollOffset.X;
            }
        }

        /// <summary>
        /// Attempts to bring the data item at the specified zero-based index into view asynchronously.
        /// </summary>
        /// <param name="index">The zero-based index of the item to scroll.</param>
        public void ScrollIndexIntoView(int index)
        {
            this.ScrollIndexIntoView(index, null);
        }

        /// <summary>
        /// Attempts to bring the data item at the specified zero-based index into view asynchronously.
        /// </summary>
        /// <param name="index">The zero-based index of the item to scroll.</param>
        /// <param name="scrollCompletedAction">Arbitrary action that may be executed after the asynchronous update is executed.</param>
        public void ScrollIndexIntoView(int index, Action scrollCompletedAction)
        {
            if (!this.IsTemplateApplied)
            {
                this.updateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(() => this.ScrollIndexIntoView(index, scrollCompletedAction)));
                return;
            }

            var scrollOperation = new ScrollIntoViewOperation<int>(index, this.ScrollViewer.VerticalOffset) { CompletedAction = scrollCompletedAction };
            this.Model.ScrollIndexIntoViewCore(scrollOperation);
        }

        /// <summary>
        /// Attempts to bring the specified data item into view asynchronously.
        /// </summary>
        /// <param name="item">The data item to scroll to.</param>
        public void ScrollItemIntoView(object item)
        {
            this.ScrollItemIntoView(item, null);
        }

        /// <summary>
        /// Attempts to bring the specified data item into view asynchronously.
        /// </summary>
        /// <param name="item">The data item to scroll to.</param>
        /// <param name="scrollCompletedAction">Arbitrary action that may be executed after the asynchronous update is executed.</param>
        public void ScrollItemIntoView(object item, Action scrollCompletedAction)
        {
            if (!this.IsTemplateApplied)
            {
                this.updateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(() => this.ScrollItemIntoView(item, scrollCompletedAction)));
                return;
            }

            var info = this.model.FindItemInfo(item);

            if (info != null)
            {
                var scrollOperation = new ScrollIntoViewOperation<int>(info.Value.LayoutInfo.Line, this.ScrollViewer.VerticalOffset) { CompletedAction = scrollCompletedAction };
                this.Model.ScrollIndexIntoViewCore(scrollOperation);
            }
        }

        /// <summary>
        /// Attempts to bring the specified <see cref="DataGridColumn"/> into view asynchronously.
        /// </summary>
        /// <param name="column">The column to attempt to scroll to.</param>
        public void ScrollColumnIntoView(DataGridColumn column)
        {
            this.ScrollColumnIntoView(column, null);
        }

        /// <summary>
        /// Attempts to bring the specified <see cref="DataGridColumn" /> into view asynchronously.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="scrollCompletedAction">Arbitrary action that may be executed after the asynchronous update is executed.</param>
        public void ScrollColumnIntoView(DataGridColumn column, Action scrollCompletedAction)
        {
            if (!this.IsTemplateApplied)
            {
                this.updateService.RegisterUpdate(new DelegateUpdate<UpdateFlags>(() => this.ScrollColumnIntoView(column, scrollCompletedAction)));
                return;
            }

            // TODO: consider adding displayindex to the columns to improve this when this operation become a performance issue.
            var visibleCollumns = this.model.VisibleColumns.ToList();
            var columnIndex = visibleCollumns.IndexOf(column);
            if (columnIndex < 0)
            {
                return;
            }

            var scrollOperation = new ScrollIntoViewOperation<int>(columnIndex, this.ScrollViewer.VerticalOffset) { CompletedAction = scrollCompletedAction };
            this.Model.ScrollColumnIntoViewCore(scrollOperation);
        }

        private void SetHorizontalOffset(double physicalOffset, bool updateUI, bool updateScrollViewer)
        {
            var adjustedOffset = GridModel.DoubleArithmetics.Ceiling(physicalOffset);
            bool needUpdate = !LayoutDoubleUtil.AreClose(this.lastScrollOffset.X, physicalOffset) || physicalOffset < 0;
            this.lastScrollOffset.X = adjustedOffset;

            if (needUpdate)
            {
                // TODO: The updateUI flag should be removed and all the needed updates should be processed by the UpdateService
                if (updateUI)
                {
                    if (this.columnHeadersPanel != null)
                    {
                        this.columnHeadersPanel.Measure();
                        this.columnHeadersPanel.Arrange();
                    }

                    if (this.CellsPanel != null)
                    {
                        this.CellsPanel.Measure();
                        this.CellsPanel.Arrange();
                    }
                }

                if (updateScrollViewer)
                {
                    this.ScrollViewer.ChangeView(adjustedOffset, null, null, true);

                    // Ensure that scrollviewer has updated its position.
                    if (updateUI)
                    {
                        this.ScrollViewer.UpdateLayout();
                    }
                }
            }
        }

        private void SetVerticalOffset(double physicalOffset, bool updateUI, bool updateScrollViewer)
        {
            var adjustedOffset = GridModel.DoubleArithmetics.Ceiling(physicalOffset);
            bool needUpdate = !LayoutDoubleUtil.AreClose(this.lastScrollOffset.Y, physicalOffset) || physicalOffset < 0;
            this.lastScrollOffset.Y = adjustedOffset;

            if (needUpdate)
            {
                if (updateUI && this.CellsPanel != null)
                {
                    if ((this.model.pendingMeasureFlags & InvalidateMeasureFlags.Header) == InvalidateMeasureFlags.Header && this.columnHeadersPanel != null)
                    {
                        this.columnHeadersPanel.Measure();
                        this.columnHeadersPanel.Arrange();
                    }
                    this.CellsPanel.Measure();
                    this.CellsPanel.Arrange();
                }

                if (updateScrollViewer)
                {
                    this.ScrollViewer.ChangeView(null, adjustedOffset, null, true);

                    ////Ensure that scrollviewer has updated its position.
                    if (updateUI)
                    {
                        this.ScrollViewer.UpdateLayout();
                    }
                }
            }
        }

        private void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            this.SetHorizontalOffset(this.scrollViewer.HorizontalOffset, false, false);
            this.SetVerticalOffset(this.scrollViewer.VerticalOffset, false, false);
            this.InvalidatePanelsMeasure();
        }
    }
}