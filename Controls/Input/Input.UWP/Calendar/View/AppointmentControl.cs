using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents the custom <see cref="AppointmentControl"/> implementation used to visualize the UI of the appointments in a cell.
    /// </summary>
    public class AppointmentControl : RadHeaderedContentControl
    {
        /// <summary>
        /// Identifies the <see cref="LeftIndicatorVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftIndicatorVisibilityProperty =
            DependencyProperty.Register(nameof(LeftIndicatorVisibility), typeof(Visibility), typeof(AppointmentControl), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <see cref="RightIndicatorVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightIndicatorVisibilityProperty =
            DependencyProperty.Register(nameof(RightIndicatorVisibility), typeof(Visibility), typeof(AppointmentControl), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <see cref="IndicatorColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register(nameof(IndicatorColor), typeof(Brush), typeof(AppointmentControl), new PropertyMetadata(null));

        internal RadCalendar calendar;
        internal CalendarAppointmentInfo appointmentInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentControl"/> class.
        /// </summary>
        public AppointmentControl()
        {
            this.DefaultStyleKey = typeof(AppointmentControl);
        }

        /// <summary>
        /// Gets or sets the visibility of the left arrow visualized when the appointment is several days long.
        /// </summary>
        public Visibility LeftIndicatorVisibility
        {
            get
            {
                return (Visibility)this.GetValue(LeftIndicatorVisibilityProperty);
            }
            set
            {
                this.SetValue(LeftIndicatorVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the right arrow visualized when the appointment is several days long.
        /// </summary>
        public Visibility RightIndicatorVisibility
        {
            get
            {
                return (Visibility)this.GetValue(RightIndicatorVisibilityProperty);
            }
            set
            {
                this.SetValue(RightIndicatorVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the indicators.
        /// </summary>
        public Brush IndicatorColor
        {
            get
            {
                return (Brush)this.GetValue(IndicatorColorProperty);
            }
            set
            {
                this.SetValue(IndicatorColorProperty, value);
            }
        }

        /// <inheritdoc/>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (this.calendar != null)
            {
                this.calendar.CommandService.ExecuteCommand(Commands.CommandId.AppointmentTap, this.appointmentInfo.childAppointment);
                e.Handled = true;
            }

            base.OnTapped(e);
        }
    }
}