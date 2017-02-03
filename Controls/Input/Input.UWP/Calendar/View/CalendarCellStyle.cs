using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Defines the appearance settings applied to a calendar cell instance.
    /// </summary>
    [Bindable]
    public class CalendarCellStyle : DependencyObject
    {
        //// TODO: Consider supporting runtime changes of these properties?

        /// <summary>
        /// Identifies the <see cref="ContentStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentStyleProperty =
            DependencyProperty.Register(nameof(ContentStyle), typeof(Style), typeof(CalendarCellStyle), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DecorationStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecorationStyleProperty =
            DependencyProperty.Register(nameof(DecorationStyle), typeof(Style), typeof(CalendarCellStyle), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style applied to the default content visual (TextBlock) for the respective calendar cell.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.SelectedCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///                 &lt;Style TargetType="TextBlock"&gt;
        ///                     &lt;Setter Property="Margin" Value="7,0,4,4"/&gt;
        ///                     &lt;Setter Property="FontSize" Value="18" /&gt;
        ///                     &lt;Setter Property="FontWeight" Value="Bold" /&gt;
        ///                     &lt;Setter Property="Foreground" Value="RoyalBlue" /&gt;
        ///                     &lt;Setter Property="TextAlignment" Value="Center" /&gt;
        ///                     &lt;Setter Property="VerticalAlignment" Value="Center" /&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.ContentStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.SelectedCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <remarks>
        /// The target type for this <see cref="Style"/> instance should be <see cref="TextBlock"/>.
        /// </remarks>
        public Style ContentStyle
        {
            get
            {
                return (Style)this.GetValue(ContentStyleProperty);
            }
            set
            {
                this.SetValue(ContentStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the default decoration visual (Rectangle) for the respective calendar cell.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar&gt;
        ///     &lt;telerikInput:RadCalendar.SelectedCellStyle&gt;
        ///         &lt;telerikInput:CalendarCellStyle&gt;
        ///             &lt;telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///                 &lt;Style TargetType="Border"&gt;
        ///                     &lt;Setter Property="Background" Value="PaleGreen" /&gt;
        ///                     &lt;Setter Property="BorderBrush" Value="SkyBlue"/&gt;
        ///                 &lt;/Style&gt;
        ///             &lt;/telerikInput:CalendarCellStyle.DecorationStyle&gt;
        ///         &lt;/telerikInput:CalendarCellStyle&gt;
        ///     &lt;/telerikInput:RadCalendar.SelectedCellStyle&gt;
        /// &lt;/telerikInput:RadCalendar&gt;
        /// </code>
        /// </example>
        /// <remarks>
        /// The target type for this <see cref="Style"/> instance should be <see cref="Border"/>.
        /// </remarks>
        public Style DecorationStyle
        {
            get
            {
                return (Style)this.GetValue(DecorationStyleProperty);
            }
            set
            {
                this.SetValue(DecorationStyleProperty, value);
            }
        }
    }
}
