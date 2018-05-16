using System.Linq;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Grid.View;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        internal VisualStateService visualStateService;
        internal HitTestService hitTestService;
        internal CommandService commandService;
        private Storyboard cellFlyoutShowTimeOutAnimationBoard;
        private DoubleAnimation cellFlyoutShowTimeOutAnimation;
            
        /// <summary>
        /// Gets the <see cref="HitTestService"/> instance that provides methods for retrieving rows and cells from a given physical location.
        /// </summary>
        public HitTestService HitTestService
        {
            get
            {
                return this.hitTestService;
            }
        }

        internal ColumnHeaderTapContext GenerateColumnHeaderTapContext(DataGridColumn column, PointerDeviceType deviceType)
        {
            bool multiple = false;
            bool canSort = column.CanSort;

            if (canSort)
            {
                switch (this.UserSortMode)
                {
                    case DataGridUserSortMode.Auto:
                        if (deviceType == PointerDeviceType.Touch)
                        {
                            multiple = true;
                        }
                        else
                        {
                            multiple = KeyboardHelper.IsModifierKeyDown(VirtualKey.Control);
                        }
                        break;
                    case DataGridUserSortMode.Multiple:
                        multiple = true;
                        break;
                    case DataGridUserSortMode.None:
                        canSort = false;
                        break;
                }
            }

            var context = new ColumnHeaderTapContext()
            {
                Column = column,
                CanSort = canSort,
                IsMultipleSortAllowed = multiple
            };

            return context;
        }

        internal FilterButtonTapContext GenerateFilterButtonTapContext(DataGridColumnHeader header)
        {
            var context = new FilterButtonTapContext()
            {
                FirstFilterControl = header.Column.CreateFilterControl(),
                Column = header.Column,
                AssociatedDescriptor = this.FilterDescriptors.FirstOrDefault(d => d.DescriptorPeer == header.Column)
            };

            if (header.Column.SupportsCompositeFilter)
            {
                context.SecondFilterControl = header.Column.CreateFilterControl();
            }

            return context;
        }

        internal void ExecuteFilter(DataGridColumnHeader header)
        {
            var context = this.GenerateFilterButtonTapContext(header);
            this.commandService.ExecuteCommand(CommandId.FilterButtonTap, context);
        }

        internal void OnColumnHeaderTap(DataGridColumnHeader headerCell, TappedRoutedEventArgs e)
        {
            var columnHeaderPeer = FrameworkElementAutomationPeer.FromElement(headerCell) as DataGridColumnHeaderAutomationPeer;
            if (columnHeaderPeer != null)
            {
                columnHeaderPeer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
            }

            var context = this.GenerateColumnHeaderTapContext(headerCell.Column, e.PointerDeviceType);
            context.IsFlyoutOpen = this.ContentFlyout.IsOpen;
            this.ContentFlyout.Hide(DataGridFlyoutId.All);
            this.commandService.ExecuteCommand(CommandId.ColumnHeaderTap, context);
        }

        internal void OnFilterButtonTap(DataGridColumnHeader header)
        {
            this.ExecuteFilter(header);
        }

        internal void OnGroupHeaderTap(DataGridGroupHeader header)
        {
            var context = header.DataContext as GroupHeaderContext;
            context.IsExpanded = header.IsExpanded;

            this.commandService.ExecuteCommand(CommandId.GroupHeaderTap, context);

            // get the value from the context as the command may have it toggled
            header.IsExpanded = context.IsExpanded;

            this.CurrencyService.RefreshCurrentItem(false);

            this.TryFocus(FocusState.Pointer, false);

            this.ResetSelectedHeader();

            this.ContentFlyout.Hide(DataGridFlyoutId.All);
        }

        internal void OnCellsPanelPointerOver(PointerRoutedEventArgs e)
        {
            this.cellFlyoutShowTimeOutAnimationBoard.Completed -= this.CellFlyoutTimerAnimationBoardCompleted;
            this.cellFlyoutShowTimeOutAnimationBoard.Stop();

            var hitPoint = e.GetCurrentPoint(this.cellsPanel).Position;

            var cell = this.hitTestService.GetCellFromPoint(hitPoint.ToRadPoint());

            if (cell != null)
            {
                this.commandService.ExecuteCommand(CommandId.CellPointerOver, new DataGridCellInfo(cell));

                if (cell.Column.IsCellFlyoutEnabled && !(this.ContentFlyout.IsOpen && this.ContentFlyout.Id != DataGridFlyoutId.Cell))
                {
                    this.cellFlyoutShowTimeOutAnimationBoard.Completed += this.CellFlyoutTimerAnimationBoardCompleted;
                    this.hoveredCell = cell;
                    this.cellFlyoutShowTimeOutAnimationBoard.Begin();
                }
            }
            else
            {
                this.visualStateService.UpdateHoverDecoration(null);
            }
        }

        internal void OnCellsPanelPointerExited()
        {
            // clear the hover effect (if any)
            this.visualStateService.UpdateHoverDecoration(null);

            this.cellFlyoutShowTimeOutAnimationBoard.Completed -= this.CellFlyoutTimerAnimationBoardCompleted;
            this.cellFlyoutShowTimeOutAnimationBoard.Stop();
        }

        internal void OnCellsPanelPointerPressed()
        {
            this.TryFocus(FocusState.Pointer, false);
        }

        internal void OnCellsPanelHolding(HoldingRoutedEventArgs e)
        {
            var cell = this.hitTestService.GetCellFromPoint(e.GetPosition(this.cellsPanel).ToRadPoint());

            if (cell != null)
            {
                this.commandService.ExecuteCommand(CommandId.CellHolding, new CellHoldingContext(new DataGridCellInfo(cell), e.HoldingState));
            }

            this.TryFocus(FocusState.Pointer, false);
        }

        internal void OnCellsPanelTapped(TappedRoutedEventArgs e)
        {
            var cell = this.hitTestService.GetCellFromPoint(e.GetPosition(this.cellsPanel).ToRadPoint());
            if (cell != null)
            {
                this.commandService.ExecuteCommand(CommandId.CellTap, new DataGridCellInfo(cell));
            }

            this.ResetSelectedHeader();

            this.TryFocus(FocusState.Pointer, false, e.OriginalSource as FrameworkElement);

            if (this.contentFlyout.IsOpen)
            {
                this.ContentFlyout.Hide(DataGridFlyoutId.All);
            }
        }

        internal void OnCellsPanelDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            var cell = this.hitTestService.GetCellFromPoint(e.GetPosition(this.cellsPanel).ToRadPoint());
            if (cell != null)
            {
                this.commandService.ExecuteCommand(CommandId.CellDoubleTap, new DataGridCellInfo(cell));
            }
        }

        internal void OnCellDoubleTap(DataGridCellInfo cell)
        {
            this.BeginEdit(cell, ActionTrigger.DoubleTap, null);
        }

        internal void OnCellTap(DataGridCellInfo cellInfo)
        {
            if (this.editService.IsEditing)
            {
                if (this.UserEditMode == DataGridUserEditMode.External)
                {
                    this.CancelEdit();
                }
                else if (!this.CommitEdit(new DataGridCellInfo(this.CurrentItem, null), ActionTrigger.Tap, null))
                {
                    return;
                }
            }

            this.selectionService.Select(cellInfo.Cell);
            this.CurrencyService.ChangeCurrentItem(cellInfo.RowItemInfo.Item, true, true);

            if (cellInfo.Column != null)
            {
                cellInfo.Column.TryFocusCell(cellInfo, FocusState.Pointer);
            }
        }

        internal void OnCellHolding(DataGridCellInfo cellInfo, HoldingState holdingState)
        {
            if (cellInfo.Column.IsCellFlyoutEnabled && holdingState == HoldingState.Started)
            {
                this.CommandService.ExecuteCommand(Telerik.UI.Xaml.Controls.Grid.Commands.CommandId.CellFlyoutAction, new CellFlyoutActionContext(cellInfo, true, CellFlyoutGesture.Holding));
            }
        }

        internal void OnCellsPanelKeyDown(KeyRoutedEventArgs e)
        {
            this.ExecuteKeyDown(e);
        }

        internal void TryFocus(FocusState state, bool force, FrameworkElement tappedElement = null)
        {
            if (!this.IsTabStop)
            {
                return;
            }

            if (force)
            {
                this.Focus(state);
            }
            else
            {
                var focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
                if (focusedElement == null || (ElementTreeHelper.FindVisualAncestor<DataGridCellsPanel>(focusedElement) == null 
                    && ElementTreeHelper.FindVisualAncestor<DataGridCellsPanel>(tappedElement) == null))
                {
                    this.Focus(state);
                }
            }
        }

        internal void OnGroupIsExpandedChanged()
        {
            this.CurrencyService.OnGroupExpandStateChanged();

            // TODO: Decide whether to support expand/collapse of groups while editing
            this.CancelEdit();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal void HandleKeyDown(KeyRoutedEventArgs e)
        {
            if (e == null || e.Handled)
            {
                return;
            }

            ItemInfo? info = null;

            switch (e.Key)
            {
                case VirtualKey.Escape:
                    if (this.editService.IsEditing)
                    {
                        e.Handled = true;
                        this.CancelEdit(ActionTrigger.Keyboard, e.Key);
                    }
                    break;
                case VirtualKey.F2:
                    if (!this.editService.IsEditing)
                    {
                        e.Handled = true;
                        this.BeginEdit(this.CurrentItem, ActionTrigger.Keyboard, e.Key);
                    }
                    break;
                case VirtualKey.Tab:
                    if (e.OriginalSource is RadDataGrid)
                    {
                        if (!this.editService.IsEditing)
                        {
                            e.Handled = true;
                            info = this.CurrencyService.CurrentItemInfo == null ?
                                this.model.FindFirstDataItemInView()
                                : this.CurrencyService.CurrentItemInfo;

#pragma warning disable CS4014 
                            Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Low, 
                                () =>
                                {
                                    var xamlVisualStateLayer = this.visualStateLayerCache as XamlVisualStateLayer;
                                    if (xamlVisualStateLayer != null)
                                    {
                                        var currencyVisual = xamlVisualStateLayer.CurrencyVisual as DataGridCurrencyControl;
                                        if (currencyVisual != null && currencyVisual.Visibility == Visibility.Visible)
                                        {
                                            currencyVisual.Focus(FocusState.Keyboard);
                                            this.RaiseCellPeerFocusChangedEvent(info);
                                        }
                                    }
                                });
#pragma warning restore CS4014
                        }
                    }
                    break;

                case VirtualKey.Down:
                    if (!this.editService.IsEditing)
                    {
                        e.Handled = true;
                        info = this.model.FindPreviousOrNextDataItem(this.CurrentItem, true);
                    }
                    break;
                case VirtualKey.Up:
                    if (!this.editService.IsEditing)
                    {
                        e.Handled = true;
                        info = this.model.FindPreviousOrNextDataItem(this.CurrentItem, false);
                    }
                    break;
                case VirtualKey.PageDown:
                    if (!this.editService.IsEditing)
                    {
                        e.Handled = true;
                        info = this.model.FindPageUpOrDownDataItem(this.CurrentItem, true);
                    }
                    break;
                case VirtualKey.PageUp:
                    if (!this.editService.IsEditing)
                    {
                        e.Handled = true;
                        info = this.model.FindPageUpOrDownDataItem(this.CurrentItem, false);
                    }
                    break;
                case VirtualKey.Home:
                    if (!this.editService.IsEditing && KeyboardHelper.IsModifierKeyDown(VirtualKey.Control))
                    {
                        e.Handled = true;
                        info = this.model.FindFirstDataItemInView();
                    }
                    break;
                case VirtualKey.End:
                    if (!this.editService.IsEditing && KeyboardHelper.IsModifierKeyDown(VirtualKey.Control))
                    {
                        e.Handled = true;
                        info = this.model.FindLastDataItemInView();
                    }
                    break;
                case VirtualKey.Enter:
                    e.Handled = true;
                    if (this.editService.IsEditing)
                    {
                        this.CommitEdit(new DataGridCellInfo(this.CurrentItem, null), ActionTrigger.Keyboard, e.Key);
                    }
                    else
                    {
                        bool shiftPressed = KeyboardHelper.IsModifierKeyDown(VirtualKey.Shift);
                        info = this.model.FindPreviousOrNextDataItem(this.CurrentItem, !shiftPressed);
                    }
                    break;
                case VirtualKey.Space:
                    if (e.OriginalSource is RadDataGrid)
                    {
                        if (this.SelectionUnit == DataGridSelectionUnit.Row)
                        {
                            info = this.model.FindItemInfo(this.CurrentItem);
                            if (info != null)
                            {
                                var cell = this.model.CellsController.GetCellsForRow(info.Value.Slot).First();
                                if (cell != null)
                                {
                                    this.OnCellTap(new DataGridCellInfo(cell));
                                }
                            }
                        }
                    }
                    break;
            }

            if (info != null)
            {
                this.CurrencyService.ChangeCurrentItem(info.Value.Item, true, true);

                if (e.Key != VirtualKey.Tab)
                {
                    this.RaiseCellPeerFocusChangedEvent(info);
                }
            }
        }
        
        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            this.ExecuteKeyDown(e);
        }

        private void CellFlyoutTimerAnimationBoardCompleted(object sender, object e)
        {
            this.cellFlyoutShowTimeOutAnimationBoard.Completed -= this.CellFlyoutTimerAnimationBoardCompleted;

            // If another flyout is opened it should prevent showing the cell tooltip by design.
            if (this.ContentFlyout.IsOpen && this.ContentFlyout.Id != DataGridFlyoutId.Cell)
            {
                return;
            }
            this.CommandService.ExecuteCommand(CommandId.CellFlyoutAction, new CellFlyoutActionContext(new DataGridCellInfo(this.hoveredCell), true, CellFlyoutGesture.PointerOver));
        }

        private void OnScrollViewerKeyDown(object sender, KeyRoutedEventArgs e)
        {
            this.ExecuteKeyDown(e);
        }

        private void ExecuteKeyDown(KeyRoutedEventArgs e)
        {
            this.commandService.ExecuteCommand(CommandId.KeyDown, e);
        }

        private void SubscribeToFrozenHostEvents()
        {
            this.FrozenContentHost.Tapped += this.FrozenContentHost_Tapped;
            this.FrozenContentHost.DoubleTapped += this.FrozenContentHost_DoubleTapped;
            this.FrozenContentHost.PointerMoved += this.FrozenContentHost_PointerMoved;
            this.FrozenContentHost.PointerExited += this.FrozenContentHost_PointerExited;
            this.FrozenContentHost.PointerPressed += this.FrozenContentHost_PointerPressed;
            this.FrozenContentHost.KeyDown += this.FrozenContentHost_KeyDown;
        }

        private void UnsubscribeFromFrozenHostEvents()
        {
            var frozenHost = this.FrozenContentHost;

            if (frozenHost != null)
            {
                frozenHost.Tapped -= this.FrozenContentHost_Tapped;
                frozenHost.DoubleTapped -= this.FrozenContentHost_DoubleTapped;
                frozenHost.PointerMoved -= this.FrozenContentHost_PointerMoved;
                frozenHost.PointerExited -= this.FrozenContentHost_PointerExited;
                frozenHost.PointerPressed -= this.FrozenContentHost_PointerPressed;
                frozenHost.KeyDown -= this.FrozenContentHost_KeyDown;
            }
        }

        private void RaiseCellPeerFocusChangedEvent(ItemInfo? info)
        {
            var dataGridPeer = FrameworkElementAutomationPeer.FromElement(this) as RadDataGridAutomationPeer;
            if (dataGridPeer != null && dataGridPeer.childrenCache != null)
            {
                if (dataGridPeer.childrenCache.Count == 0)
                {
                    dataGridPeer.GetChildren();
                }

                var cellPeer = dataGridPeer.childrenCache.FirstOrDefault(a => a.Row == info.Value.Slot && a.Column == 0) as DataGridCellInfoAutomationPeer;
                if (cellPeer != null && cellPeer.ChildTextBlockPeer != null)
                {
                    cellPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
                }
            }
        }

        private void FrozenContentHost_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            this.OnCellsPanelKeyDown(e);
        }

        private void FrozenContentHost_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.OnCellsPanelPointerPressed();
        }

        private void FrozenContentHost_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.OnCellsPanelPointerExited();
        }

        private void FrozenContentHost_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            this.OnCellsPanelPointerOver(e);
        }

        private void FrozenContentHost_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.OnCellsPanelDoubleTapped(e);
        }

        private void FrozenContentHost_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnCellsPanelTapped(e);
        }
    }
}
