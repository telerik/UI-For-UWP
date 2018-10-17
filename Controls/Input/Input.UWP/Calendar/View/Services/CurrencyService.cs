using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class CurrencyService : ServiceBase<RadCalendar>
    {
        private List<CalendarCellModel> currentCellsToUpdate;
        private DateTime currentDate = DateTime.MinValue;

        internal CurrencyService(RadCalendar owner)
            : base(owner)
        {
        }

        public event EventHandler<CurrentSelectionChangedEventArgs> CurrentChanged;
        public event CurrentChangingEventHandler CurrentChanging;

        internal DateTime CurrentDate
        {
            get
            {
                return this.currentDate;
            }
            set
            {
                DateTime coercedValue = value;
                this.Owner.CoerceDateWithinDisplayRange(ref coercedValue);

                if (this.currentDate != coercedValue)
                {
                    bool cancel = this.PreviewCancelCurrentChanging();
                    if (cancel)
                    {
                        return;
                    }

                    DateTime oldCurrentDate = this.currentDate;
                    this.currentDate = coercedValue;

                    this.AsyncUpdateCurrencyPresenters(oldCurrentDate, this.currentDate);

                    this.OnCurrentChanged(new CurrentSelectionChangedEventArgs() { NewSelection = this.currentDate });
                }
            }
        }

        internal bool MoveCurrentToPrevious(bool isControlModifierDown)
        {
            bool handled = true;

            if (isControlModifierDown)
            {
                if (this.CurrentDate == DateTime.MinValue)
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByView(this.Owner.DisplayDate, -1, this.Owner.DisplayMode);
                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                }
                else
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByView(this.CurrentDate, -1, this.Owner.DisplayMode);

                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                    this.CurrentDate = dateToNavigate;
                }
            }
            else
            {
                if (this.CurrentDate == DateTime.MinValue)
                {
                    this.CurrentDate = CalendarMathHelper.GetFirstDateForCurrentDisplayUnit(this.Owner.DisplayDate, this.Owner.DisplayMode);
                }
                else
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByCell(this.CurrentDate, -1, this.Owner.DisplayMode);

                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                    this.CurrentDate = dateToNavigate;
                }
            }

            return handled;
        }

        internal bool MoveCurrentToNext(bool isControlModifierDown)
        {
            bool handled = true;

            if (isControlModifierDown)
            {
                if (this.CurrentDate == DateTime.MinValue)
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByView(this.Owner.DisplayDate, 1, this.Owner.DisplayMode);
                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                }
                else
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByView(this.CurrentDate, 1, this.Owner.DisplayMode);

                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                    this.CurrentDate = dateToNavigate;
                }
            }
            else
            {
                if (this.CurrentDate == DateTime.MinValue)
                {
                    this.CurrentDate = CalendarMathHelper.GetFirstDateForCurrentDisplayUnit(this.Owner.DisplayDate, this.Owner.DisplayMode);
                }
                else
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByCell(this.CurrentDate, 1, this.Owner.DisplayMode);

                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                    this.CurrentDate = dateToNavigate;
                }
            }

            return handled;
        }

        internal bool MoveCurrentUp(bool isControlModifierDown)
        {
            bool handled = false;

            if (isControlModifierDown)
            {
                if (this.Owner.DisplayMode != CalendarDisplayMode.CenturyView)
                {
                    this.Owner.RaiseMoveToUpperViewCommand();

                    if (this.CurrentDate != DateTime.MinValue)
                    {
                        this.AsyncUpdateCurrencyPresenters(this.CurrentDate, this.CurrentDate);
                    }

                    handled = true;
                }
            }
            else
            {
                if (this.CurrentDate == DateTime.MinValue)
                {
                    this.CurrentDate = CalendarMathHelper.GetFirstDateForCurrentDisplayUnit(this.Owner.DisplayDate, this.Owner.DisplayMode);
                }
                else
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByCell(this.CurrentDate, -this.Owner.Model.ColumnCount, this.Owner.DisplayMode);

                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                    this.CurrentDate = dateToNavigate;
                }

                handled = true;
            }

            return handled;
        }

        internal bool MoveCurrentDown(bool isControlModifierDown)
        {
            bool handled = false;

            if (isControlModifierDown)
            {
                if (this.CurrentDate != DateTime.MinValue && this.Owner.DisplayMode != CalendarDisplayMode.MonthView)
                {
                    this.Owner.RaiseMoveToLowerCommand(this.CurrentDate);

                    // CurrentDate is not changed but we need to update the visuals on view change. 
                    this.AsyncUpdateCurrencyPresenters(this.CurrentDate, this.CurrentDate);

                    handled = true;
                }
            }
            else
            {
                if (this.CurrentDate == DateTime.MinValue)
                {
                    this.CurrentDate = CalendarMathHelper.GetFirstDateForCurrentDisplayUnit(this.Owner.DisplayDate, this.Owner.DisplayMode);
                }
                else
                {
                    DateTime dateToNavigate = CalendarMathHelper.IncrementByCell(this.CurrentDate, this.Owner.Model.ColumnCount, this.Owner.DisplayMode);

                    this.Owner.RaiseMoveToDateCommand(dateToNavigate);
                    this.CurrentDate = dateToNavigate;
                }

                handled = true;
            }

            return handled;
        }

        internal bool MoveCurrentToFirst()
        {
            bool handled = true;

            this.CurrentDate = CalendarMathHelper.GetFirstDateForCurrentDisplayUnit(this.Owner.DisplayDate, this.Owner.DisplayMode);

            return handled;
        }

        internal bool MoveCurrentToLast()
        {
            bool handled = true;

            this.CurrentDate = CalendarMathHelper.GetLastDateForCurrentDisplayUnit(this.Owner.DisplayDate, this.Owner.DisplayMode);

            return handled;
        }

        internal bool HandleCellTap()
        {
            bool handled = false;

            if (this.CurrentDate != DateTime.MinValue)
            {
                if (this.Owner.DisplayMode != CalendarDisplayMode.MonthView)
                {
                    this.Owner.RaiseMoveToLowerCommand(this.CurrentDate);

                    // CurrentDate is not changed but we need to update the visuals on view change. 
                    this.AsyncUpdateCurrencyPresenters(this.CurrentDate, this.CurrentDate);
                }
                else
                {
                    // We are certain that for month view there is indeed calendar cell that represents the date in CurrentDate property.
                    CalendarCellModel currentCell = this.Owner.GetCellByDate(this.CurrentDate);
                    if (currentCell != null)
                    {
                        this.Owner.RaiseCellTapCommand(currentCell);
                    }
                }

                handled = true;
            }

            return handled;
        }

        internal void AsyncUpdateCurrencyPresenters(DateTime oldCurrentDate, DateTime newCurrentDate)
        {
            this.Owner.InvokeAsync(() => this.UpdatePresenters(oldCurrentDate, newCurrentDate));
        }

        private void UpdatePresenters(DateTime oldCurrentDate, DateTime newCurrentDate)
        {
            if (this.currentCellsToUpdate == null)
            {
                this.currentCellsToUpdate = new List<CalendarCellModel>();
            }
            else
            {
                this.currentCellsToUpdate.Clear();
            }

            if (oldCurrentDate != DateTime.MinValue && oldCurrentDate.Date != newCurrentDate.Date)
            {
                oldCurrentDate = CalendarMathHelper.GetCellDateForViewLevel(oldCurrentDate, this.Owner.DisplayMode);
                CalendarCellModel previousCell = this.Owner.GetCellByDate(oldCurrentDate);
                if (previousCell != null)
                {
                    previousCell.IsCurrent = false;
                    this.currentCellsToUpdate.Add(previousCell);
                }
            }

            if (newCurrentDate != DateTime.MinValue)
            {
                newCurrentDate = CalendarMathHelper.GetCellDateForViewLevel(newCurrentDate, this.Owner.DisplayMode);
                if (newCurrentDate == oldCurrentDate && this.currentCellsToUpdate.Count > 0)
                {
                    return;
                }
                
                CalendarCellModel currentCell = this.Owner.GetCellByDate(newCurrentDate);
                if (currentCell != null)
                {
                    currentCell.IsCurrent = true;
                    this.currentCellsToUpdate.Add(currentCell);
                }
            }

            this.Owner.UpdatePresenters(this.currentCellsToUpdate);
        }

        private bool PreviewCancelCurrentChanging()
        {
            var handler = this.CurrentChanging;
            if (handler == null)
            {
                return false;
            }

            var args = new CurrentChangingEventArgs();
            handler(this.Owner, args);

            return args.Cancel;
        }

        private void OnCurrentChanged(CurrentSelectionChangedEventArgs args)
        {
            this.Owner.OnPropertyChanged(new PropertyChangedEventArgs("CurrentDate"));

            var handler = this.CurrentChanged;
            if (handler != null)
            {
                handler(this.Owner, args);
            }
        }
    }
}
