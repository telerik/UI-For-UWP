using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input
{
    [Bindable]
    public class CalendarAppointmentContentStyle : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="SubjectFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubjectFontSizeProperty =
            DependencyProperty.Register(nameof(SubjectFontSize), typeof(int), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(14));

        /// <summary>
        /// Identifies the <see cref="SubjectForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubjectForegroundProperty =
            DependencyProperty.Register(nameof(SubjectForeground), typeof(Brush), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DetailsTextForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTextForegroundProperty =
            DependencyProperty.Register(nameof(DetailsTextForeground), typeof(Brush), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DetailsTextFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTextFontSizeProperty =
            DependencyProperty.Register(nameof(DetailsTextFontSize), typeof(int), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(13));

        /// <summary>
        /// Identifies the <see cref="DetailsTextHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTextHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(DetailsTextHorizontalAlignment), typeof(HorizontalAlignment), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// Identifies the <see cref="DetailsTextVerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTextVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(DetailsTextVerticalAlignment), typeof(VerticalAlignment), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the <see cref="SubjectHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubjectHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(SubjectHorizontalAlignment), typeof(HorizontalAlignment), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// Identifies the <see cref="SubjectVerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubjectVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(SubjectVerticalAlignment), typeof(VerticalAlignment), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the <see cref="SubjectFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubjectFontFamilyProperty =
            DependencyProperty.Register(nameof(SubjectFontFamily), typeof(FontFamily), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(new FontFamily("Arial")));

        /// <summary>
        /// Identifies the <see cref="DetailsTextFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTextFontFamilyProperty =
            DependencyProperty.Register(nameof(DetailsTextFontFamily), typeof(FontFamily), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(new FontFamily("Arial")));

        /// <summary>
        /// Identifies the <see cref="SubjectFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubjectFontStyleProperty =
            DependencyProperty.Register(nameof(SubjectFontStyle), typeof(FontStyle), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(FontStyle.Normal));

        /// <summary>
        /// Identifies the <see cref="DetailsTextFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTextFontStyleProperty =
            DependencyProperty.Register(nameof(DetailsTextFontStyle), typeof(FontStyle), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(FontStyle.Normal));

        /// <summary>
        /// Identifies the <see cref="SubjectFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubjectFontWeightProperty =
            DependencyProperty.Register(nameof(SubjectFontWeight), typeof(FontWeight), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(FontWeights.Normal));

        /// <summary>
        /// Identifies the <see cref="DetailsTextFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailsTextFontWeightProperty =
            DependencyProperty.Register(nameof(DetailsTextFontWeight), typeof(FontWeight), typeof(CalendarAppointmentContentStyle), new PropertyMetadata(FontWeights.Normal));

        /// <summary>
        /// Gets or sets the size of the font applied to the subject text visualized in the Appointment.
        /// </summary>
        public int SubjectFontSize
        {
            get
            {
                return (int)this.GetValue(SubjectFontSizeProperty);
            }
            set
            {
                this.SetValue(SubjectFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Foreground color applied to the subject text visualized in the Appointment.
        /// </summary>
        public Brush SubjectForeground
        {
            get
            {
                return (Brush)this.GetValue(SubjectForegroundProperty);
            }
            set
            {
                this.SetValue(SubjectForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Foreground color applied to the details text visualized in the Appointment.
        /// </summary>
        public Brush DetailsTextForeground
        {
            get
            {
                return (Brush)this.GetValue(DetailsTextForegroundProperty);
            }
            set
            {
                this.SetValue(DetailsTextForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the font applied to the details text visualized in the Appointment.
        /// </summary>
        public int DetailsTextFontSize
        {
            get
            {
                return (int)this.GetValue(DetailsTextFontSizeProperty);
            }
            set
            {
                this.SetValue(DetailsTextFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the HorizontalAlignment of the details text visualized in the Appointment.
        /// </summary>
        public HorizontalAlignment DetailsTextHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(DetailsTextHorizontalAlignmentProperty);
            }
            set
            {
                this.SetValue(DetailsTextHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the VerticalAlignment of the details text visualized in the Appointment.
        /// </summary>
        public VerticalAlignment DetailsTextVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(DetailsTextVerticalAlignmentProperty);
            }
            set
            {
                this.SetValue(DetailsTextVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the HorizontalAlignment of the subject visualized in the Appointment.
        /// </summary>
        public HorizontalAlignment SubjectHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(SubjectHorizontalAlignmentProperty);
            }
            set
            {
                this.SetValue(SubjectHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the VerticalAlignment of the subject visualized in the Appointment.
        /// </summary>
        public VerticalAlignment SubjectVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(SubjectVerticalAlignmentProperty);
            }
            set
            {
                this.SetValue(SubjectVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the FontFamily of the subject visualized in the Appointment.
        /// </summary>
        public FontFamily SubjectFontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(SubjectFontFamilyProperty);
            }
            set
            {
                this.SetValue(SubjectFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the FontFamily of the details text visualized in the Appointment.
        /// </summary>
        public FontFamily DetailsTextFontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(DetailsTextFontFamilyProperty);
            }
            set
            {
                this.SetValue(DetailsTextFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the FontStyle of the subject visualized in the Appointment.
        /// </summary>
        public FontStyle DetailsTextFontStyle
        {
            get
            {
                return (FontStyle)this.GetValue(DetailsTextFontStyleProperty);
            }
            set
            {
                this.SetValue(DetailsTextFontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the FontStyle of the details text visualized in the Appointment.
        /// </summary>
        public FontStyle SubjectFontStyle
        {
            get
            {
                return (FontStyle)this.GetValue(SubjectFontStyleProperty);
            }
            set
            {
                this.SetValue(SubjectFontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the FontWeight of the subject visualized in the Appointment.
        /// </summary>
        public FontWeight SubjectFontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(SubjectFontWeightProperty);
            }
            set
            {
                this.SetValue(SubjectFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the FontWeight of the details text visualized in the Appointment.
        /// </summary>
        public FontWeight DetailsTextFontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(DetailsTextFontWeightProperty);
            }
            set
            {
                this.SetValue(DetailsTextFontWeightProperty, value);
            }
        }
    }
}
