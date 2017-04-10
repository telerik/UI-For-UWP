using System;
using System.ComponentModel;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input
{
    public partial class RadCalendar : INotifyPropertyChanged
    {
        internal bool isCalendarViewFocused;

        /// <summary>
        /// Occurs when a property of the <see cref="RadCalendar"/> changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="CurrentDate"/> property is about to change. The event may be canceled to prevent the actual change.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event CurrentChangingEventHandler CurrentDateChanging
        {
            add
            {
                this.CurrencyService.CurrentChanging += value;
            }
            remove
            {
                this.CurrencyService.CurrentChanging -= value;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="CurrentDate"/> property has changed.
        /// </summary>
        public event EventHandler<CurrentSelectionChangedEventArgs> CurrentDateChanged
        {
            add
            {
                this.CurrencyService.CurrentChanged += value;
            }
            remove
            {
                this.CurrencyService.CurrentChanged -= value;
            }
        }

        /// <summary>
        /// Gets the date that represents the currently focused cell in the calendar view.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar x:Name="calendar"/&gt;
        /// </code>
        /// <code language="c#">
        /// var currentDate = this.calendar.CurrentDate;
        /// </code>
        /// </example>
        public DateTime CurrentDate
        {
            get
            {
                return this.CurrencyService.CurrentDate;
            }
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal bool ProcessKeyDown(VirtualKey key)
        {
            if (!this.isCalendarViewFocused)
            {
                return false;
            }

            bool handled = false;
            DateTime oldCurrentDate = this.CurrentDate;

            switch (key)
            {
                case VirtualKey.Left:
                    {
                        handled = this.CurrencyService.MoveCurrentToPrevious(KeyboardHelper.IsModifierKeyDown(VirtualKey.Control));
                        break;
                    }
                case VirtualKey.Right:
                    {
                        handled = this.CurrencyService.MoveCurrentToNext(KeyboardHelper.IsModifierKeyDown(VirtualKey.Control));
                        break;
                    }
                case VirtualKey.Up:
                    {
                        handled = this.CurrencyService.MoveCurrentUp(KeyboardHelper.IsModifierKeyDown(VirtualKey.Control));
                        break;
                    }
                case VirtualKey.Down:
                    {
                        handled = this.CurrencyService.MoveCurrentDown(KeyboardHelper.IsModifierKeyDown(VirtualKey.Control));
                        break;
                    }
                case VirtualKey.Home:
                    {
                        handled = this.CurrencyService.MoveCurrentToFirst();
                        break;
                    }
                case VirtualKey.End:
                    {
                        handled = this.CurrencyService.MoveCurrentToLast();
                        break;
                    }
                case VirtualKey.Enter:
                case VirtualKey.Space:
                    {
                        handled = this.CurrencyService.HandleCellTap();
                        break;
                    }
            }

            // Shift Selection in Multiple SelectionMode.
            if (key != VirtualKey.Enter && key != VirtualKey.Space && 
                KeyboardHelper.IsModifierKeyDown(VirtualKey.Shift) && !KeyboardHelper.IsModifierKeyDown(VirtualKey.Control) &&
                oldCurrentDate != this.CurrentDate && this.SelectionMode == CalendarSelectionMode.Multiple)
            {
                CalendarCellModel oldCurrentModel = this.GetCellByDate(oldCurrentDate);
                CalendarCellModel newCurrentModel = this.GetCellByDate(this.CurrentDate);                

                this.SelectionService.MakeRangeSelection(oldCurrentModel, newCurrentModel);
            }

            if (oldCurrentDate != this.CurrentDate)
            {
                this.FireAutomationFocusEvent();
            }

            if (handled && this.FocusState != FocusState.Keyboard)
            {
                this.TryFocus(FocusState.Keyboard);
            }

            return handled;
        }

        internal void FireAutomationFocusEvent()
        {
            RadCalendarAutomationPeer calendarPeer = FrameworkElementAutomationPeer.FromElement(this) as RadCalendarAutomationPeer;
            if (calendarPeer != null)
            {
                calendarPeer.RaiseAutomationFocusEvent(this.CurrentDate);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (e == null)
            {
                return;
            }

            // NOTE: We implicitly assume that focusing the RadCalendar control means that its calendar view (the panel holding the cells) is focused.
            if (e.OriginalSource is RadCalendar)
            {
                this.isCalendarViewFocused = true;

                this.CurrencyService.AsyncUpdateCurrencyPresenters(DateTime.MinValue, this.CurrentDate);
            }
            else
            {
                this.isCalendarViewFocused = false;

                this.CurrencyService.AsyncUpdateCurrencyPresenters(this.CurrentDate, DateTime.MinValue);
            }  
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (e == null)
            {
                return;
            }

            if (e.OriginalSource is RadCalendar)
            {
                this.isCalendarViewFocused = false;

                this.CurrencyService.AsyncUpdateCurrencyPresenters(this.CurrentDate, DateTime.MinValue);
            }
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            if (e == null)
            {
                return;
            }

            // HACK: Workaround for WinRT issue with KeyDown raised twice for VirtualKey.Enter
            if (e.Key == VirtualKey.Enter && e.KeyStatus.RepeatCount > 0)
            {
                return;
            }

            e.Handled = this.ProcessKeyDown(e.Key);
        }        
    }
}
