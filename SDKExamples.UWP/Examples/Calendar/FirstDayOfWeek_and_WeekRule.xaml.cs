using System;
using System.Globalization;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Calendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstDayOfWeek_and_WeekRule : ExamplePageBase
    {
        public FirstDayOfWeek_and_WeekRule()
        {
            this.InitializeComponent();

            CultureInfo culture = new CultureInfo("en-US");
            culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Wednesday;
            culture.DateTimeFormat.CalendarWeekRule = CalendarWeekRule.FirstFullWeek;
            CultureService.SetCulture(this.calendar, culture);
        }
    }
}
