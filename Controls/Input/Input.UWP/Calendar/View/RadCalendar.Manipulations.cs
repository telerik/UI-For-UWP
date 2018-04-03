using System;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input
{
    public partial class RadCalendar
    {
        /// <summary>
        /// Moves the calendar to a specific date and updates the <see cref="DisplayDate" /> value in the process.
        /// If the new date belongs to a different calendar view, the calendar performs the navigation to the correct view with default animation.
        /// </summary>
        /// <param name="date">The date to move to.</param>
        public void MoveToDate(DateTime date)
        {
            this.RaiseMoveToDateCommand(date);
        }

        /// <summary>
        /// Moves the calendar to the previous view on the same view level (e.g. from March 2013 to February 2013). 
        /// The action is performed with default animation.
        /// </summary>
        public void MoveToPreviousView()
        {
            int navigationStep = 1;
            if (this.displayModeCache == CalendarDisplayMode.MultiDayView)
            {
                navigationStep = this.MultiDayViewSettings.VisibleDays;
            }

            this.RaiseMoveToPreviousViewCommand(navigationStep);
        }

        /// <summary>
        /// Moves the calendar to the next view on the same view level (e.g. from March 2013 to April 2013). 
        /// The action is performed with default animation.
        /// </summary>
        public void MoveToNextView()
        {
            int navigationStep = 1;
            if (this.displayModeCache == CalendarDisplayMode.MultiDayView)
            {
                navigationStep = this.MultiDayViewSettings.VisibleDays;
            }

            this.RaiseMoveToNextViewCommand(navigationStep);
        }

        /// <summary>
        /// Moves the calendar to the upper view level (e.g. from Month view to Year view) updating the <see cref="DisplayMode"/> property in the process. 
        /// The action is performed with default animation.
        /// </summary>
        public void MoveToUpperView()
        {
            this.RaiseMoveToUpperViewCommand();
        }

        /// <summary>
        /// Moves the calendar to the lower view level if possible (e.g. from Year view to Month view) updating the <see cref="DisplayMode" /> property in the process.
        /// The action is performed with default animation.
        /// </summary>
        /// <param name="date">The date to move to on the lower view level.</param>
        public void MoveToLowerView(DateTime date)
        {
            this.RaiseMoveToLowerCommand(date);
        }

        internal void RaiseMoveToDateCommand(DateTime date)
        {
            Storyboard animation = null;
            if (date.Date < this.DisplayDate.Date)
            {
                animation = this.CreateMoveToPreviousViewAnimationStoryboard();
            }
            else
            {
                animation = this.CreateMoveToNextViewAnimationStoryboard();
            }

            CalendarViewChangeContext context = new CalendarViewChangeContext()
            {
                AnimationStoryboard = animation,
                Date = date
            };

            this.CommandService.ExecuteCommand(CommandId.MoveToDate, context);
        }

        internal void RaiseMoveToPreviousViewCommand(int navigatioStep)
        {
            CalendarViewChangeContext context = new CalendarViewChangeContext();
            if (this.displayModeCache != CalendarDisplayMode.MultiDayView)
            {
                context.AnimationStoryboard = this.CreateMoveToPreviousViewAnimationStoryboard();
            }
            context.navigationStep = navigatioStep;

            this.CommandService.ExecuteCommand(CommandId.MoveToPreviousView, context);
        }

        internal void RaiseMoveToNextViewCommand(int navigatioStep)
        {
            CalendarViewChangeContext context = new CalendarViewChangeContext();
            if (this.displayModeCache != CalendarDisplayMode.MultiDayView)
            {
                context.AnimationStoryboard = this.CreateMoveToNextViewAnimationStoryboard();
            }
            context.navigationStep = navigatioStep;

            this.CommandService.ExecuteCommand(CommandId.MoveToNextView, context);
        }

        internal void RaiseMoveToUpperViewCommand()
        {
            CalendarViewChangeContext context = new CalendarViewChangeContext()
            {
                AnimationStoryboard = this.CreateMoveToUpperOrLowerViewAnimationStoryboard()
            };

            this.CommandService.ExecuteCommand(CommandId.MoveToUpperView, context);
        }

        internal void RaiseMoveToLowerCommand(DateTime date)
        {
            CalendarViewChangeContext context = new CalendarViewChangeContext()
            {
                AnimationStoryboard = this.CreateMoveToUpperOrLowerViewAnimationStoryboard(),
                Date = date
            };

            this.CommandService.ExecuteCommand(CommandId.MoveToLowerView, context);
        }

        internal void RaiseCellTapCommand(CalendarCellModel cellModel)
        {
            this.CommandService.ExecuteCommand(CommandId.CellTap, new CalendarCellTapContext(cellModel));
        }

        internal void RaiseCellPointerOverCommand(CalendarCellModel cellModel)
        {
            this.CommandService.ExecuteCommand(CommandId.CellPointerOver, cellModel);
        }

        internal void OnContentPanelPointerOver(PointerRoutedEventArgs e)
        {
            var hitPoint = e.GetCurrentPoint(this.calendarViewHost).Position;
            var cellModel = HitTestService.GetCellFromPoint(hitPoint, this.model.CalendarCells);

            if (cellModel != null && e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                this.RaiseCellPointerOverCommand(cellModel);
            }
            else if (this.DisplayMode != CalendarDisplayMode.MultiDayView)
            {
                this.VisualStateService.UpdateHoverDecoration(null);
            }
        }

        internal bool OnContentPanelHolding(HoldingRoutedEventArgs e)
        {
            var hitPoint = e.GetPosition(this.calendarViewHost);
            var cellModel = HitTestService.GetCellFromPoint(hitPoint, this.model.CalendarCells);

            if (cellModel != null && this.SelectionMode == CalendarSelectionMode.Multiple && this.DisplayMode == CalendarDisplayMode.MonthView)
            {
                this.VisualStateService.UpdateHoldDecoration(cellModel);
                return true;
            }

            return false;
        }

        internal void OnDragEnded(bool success)
        {
            this.SelectionService.EndSelection(success);
        }

        internal void OnContentPanelTapped(TappedRoutedEventArgs e)
        {
            if (this.displayModeCache != CalendarDisplayMode.MultiDayView)
            {
                CalendarCellModel cellModel = HitTestService.GetCellFromPoint(e.GetPosition(this.contentLayer.VisualElement), this.Model.CalendarCells);
                if (cellModel == null)
                {
                    return;
                }

                this.RaiseCellTapCommand(cellModel);
            }
            else
            {
                Point hitPoint = e.GetPosition(this.timeRulerLayer.contentPanel);
                CalendarMultiDayViewModel multiDayViewModel = this.model.multiDayViewModel;
                hitPoint.X += multiDayViewModel.timeRulerWidth;
                Slot slot = HitTestService.GetSlotFromPoint(hitPoint, multiDayViewModel.timeRulerItems, this.model.CalendarCells, this.MultiDayViewSettings.VisibleDays);
                if (slot != null)
                {
                    this.commandService.ExecuteCommand(CommandId.TimeSlotTap, slot);
                }
            }
        }

        internal void OnContentPanelPointerMoved(PointerRoutedEventArgs e, bool isDragging)
        {
            if (!isDragging)
            {
                this.OnContentPanelPointerOver(e);
            }
            else if (this.SelectionMode == CalendarSelectionMode.Multiple)
            {
                CalendarCellModel cellModel = this.GetCellModelByPosition(e);
                if (cellModel != null)
                {
                    this.SelectionService.ModifySelection(cellModel);
                }
            }
        }

        internal void OnContentPanelPointerExited()
        {
            if (this.DisplayMode != CalendarDisplayMode.MultiDayView)
            {
                this.VisualStateService.UpdateHoverDecoration(null);
            }
        }

        internal void OnDragStarted(Point lastPointPressed)
        {
            this.SelectionService.StartSelection(lastPointPressed);
        }

        internal CalendarCellModel GetCellModelByPosition(PointerRoutedEventArgs e)
        {
            Point currentPoint = e.GetCurrentPoint(this.contentLayer.VisualElement).Position;

            var hitTestPoint = TruncateToBounds(this.contentLayer.VisualElement as FrameworkElement, currentPoint);
            CalendarCellModel cellModel = HitTestService.GetCellFromPoint(hitTestPoint, this.Model.CalendarCells);

            return cellModel;
        }

        internal CalendarCellModel GetCellModelByPoint(Point lastPointPosition)
        {
            var hitTestPoint = TruncateToBounds(this.contentLayer.VisualElement as FrameworkElement, lastPointPosition);
            CalendarCellModel cellModel = HitTestService.GetCellFromPoint(hitTestPoint, this.Model.CalendarCells);

            return cellModel;
        }

        internal bool CanStartDrag(Point point)
        {
            CalendarCellModel cell = this.GetCellModelByPoint(point);
            if ((cell != null && !cell.IsBlackout) &&
                this.SelectionMode == CalendarSelectionMode.Multiple &&
                this.DisplayMode == CalendarDisplayMode.MonthView)
            {
                return true;
            }

            return false;
        }

        internal void MoveToDate(DateTime date, Storyboard animationStoryboard)
        {
            DateTime newDisplayDate = date;

            this.CoerceDateWithinDisplayRange(ref newDisplayDate);

            DateTime oldDisplayDate = this.DisplayDate;
            this.DisplayDate = newDisplayDate;

            if (CalendarMathHelper.IsCalendarViewChanged(oldDisplayDate, newDisplayDate, this.DisplayMode))
            {
                if (animationStoryboard != null)
                {
                    animationStoryboard.Begin();
                }
            }
        }

        private static Point TruncateToBounds(FrameworkElement dragHost, Point point)
        {
            if (dragHost != null)
            {
                var x = Math.Max(0, point.X);
                var y = Math.Max(0, point.Y);

                x = Math.Min(x, dragHost.ActualWidth);
                y = Math.Min(y, dragHost.ActualHeight);

                return new Point(x, y);
            }

            return point;
        }

        private void TryFocus(FocusState state)
        {
            if (!this.IsTabStop)
            {
                return;
            }

            var focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            if (focusedElement == null || ElementTreeHelper.FindVisualAncestor<CalendarViewHost>(focusedElement) == null)
            {
                this.Focus(state);
            }
        }

        private Storyboard CreateMoveToPreviousViewAnimationStoryboard()
        {
            return CalendarAnimationHelper.CreateMoveToPreviousViewStoryboard(this.contentLayer.AnimatableContainer);
        }

        private Storyboard CreateMoveToNextViewAnimationStoryboard()
        {
            return CalendarAnimationHelper.CreateMoveToNextViewStoryboard(this.contentLayer.AnimatableContainer);
        }

        private Storyboard CreateMoveToUpperOrLowerViewAnimationStoryboard()
        {
            return CalendarAnimationHelper.CreateMoveToUpperOrLowerViewStoryboard(this.calendarViewHost);
        }
    }
}
