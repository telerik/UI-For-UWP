using Telerik.UI.Xaml.Controls.Input.Calendar.AutomationPeers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    public class CalendarFooterControl : RadControl
    {
        /// <summary>
        /// Identifies the <c cref=FooterControlButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FooterControlButtonStyleProperty =
            DependencyProperty.Register(nameof(FooterControlButtonStyle), typeof(Style), typeof(CalendarFooterControl), new PropertyMetadata(null));

        private Button calendarFooterButton;

        public CalendarFooterControl()
        {
            this.DefaultStyleKey = typeof(CalendarFooterControl);
        }

        /// <summary>
        /// Gets or sets the Style for the Button of the Footer control.
        /// </summary>
        public Style FooterControlButtonStyle
        {
            get
            {
                return (Style)this.GetValue(FooterControlButtonStyleProperty);
            }
            set
            {
                this.SetValue(FooterControlButtonStyleProperty, value);
            }
        }

        internal RadCalendar Owner { get; set; }

        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();
            this.calendarFooterButton = (Button)this.GetTemplateChild("calendarFooterButton");

            return applied;
        }

        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();
            if (this.calendarFooterButton != null)
            {
                this.calendarFooterButton.Click += CalendarFooterButtonClick;
            }
        }

        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.calendarFooterButton != null)
            {
                this.calendarFooterButton.Click -= CalendarFooterButtonClick;
            }
        }

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
