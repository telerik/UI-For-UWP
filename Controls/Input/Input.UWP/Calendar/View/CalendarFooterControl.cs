using Telerik.UI.Xaml.Controls.Input.Calendar.AutomationPeers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents the footer panel of the calendar control.
    /// </summary>
    public class CalendarFooterControl : RadControl
    {
        /// <summary>
        /// Identifies the <c cref="ButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(CalendarFooterControl), new PropertyMetadata(null));

        private Button calendarFooterButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarFooterControl"/> class.
        /// </summary>
        public CalendarFooterControl()
        {
            this.DefaultStyleKey = typeof(CalendarFooterControl);
        }

        /// <summary>
        /// Gets or sets the Style for the Button of the Footer control.
        /// </summary>
        public Style ButtonStyle
        {
            get
            {
                return (Style)this.GetValue(ButtonStyleProperty);
            }
            set
            {
                this.SetValue(ButtonStyleProperty, value);
            }
        }

        internal RadCalendar Owner { get; set; }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();
            this.calendarFooterButton = (Button)this.GetTemplateChild("calendarFooterButton");

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();
            if (this.calendarFooterButton != null)
            {
                this.calendarFooterButton.Click += this.CalendarFooterButtonClick;
            }
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.calendarFooterButton != null)
            {
                this.calendarFooterButton.Click -= this.CalendarFooterButtonClick;
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CalendarFooterControlAutomationPeer(this);
        }

        private void CalendarFooterButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Owner.OnCalendarButtonClicked();
        }
    }
}
