using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a class used for customizing the timer ruler part of the day view.
    /// </summary>
    [Bindable]
    [StyleTypedProperty(Property = "VerticalLineStyle", StyleTargetType = typeof(Border))]
    [StyleTypedProperty(Property = "HorizontalLineStyle", StyleTargetType = typeof(Border))]
    [StyleTypedProperty(Property = "TimeLabelStyle", StyleTargetType = typeof(TextBlock))]
    public class CalendarTimeRulerItemStyleSelector : StyleSelector
    {
        /// <summary>
        /// Gets or sets the vertical style of the Line.
        /// </summary>
        public Style VerticalLineStyle { get; set; }

        /// <summary>
        /// Gets or sets the horizontal style of the Line.
        /// </summary>
        public Style HorizontalLineStyle { get; set; }

        /// <summary>
        /// Gets or sets the vertical style of the Label.
        /// </summary>
        public Style TimeLabelStyle { get; set; }

        /// <summary>
        /// Returns a style for the TimeRulerItem based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>The style for the TimeRulerItem.</returns>
        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var gridLine = item as CalendarGridLine;
            if (gridLine != null)
            {
                if (gridLine.IsHorizontal)
                {
                    return this.HorizontalLineStyle;
                }
                else
                {
                    return this.VerticalLineStyle;
                }
            }
            else if (item is CalendarTimeRulerItem)
            {
                return this.TimeLabelStyle;
            }

            return base.SelectStyleCore(item, container);
        }
    }
}
