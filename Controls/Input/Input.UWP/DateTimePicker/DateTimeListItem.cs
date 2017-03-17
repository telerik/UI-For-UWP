using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Input.DateTimePickers
{
    /// <summary>
    /// Represents a special looping list item that is created within a date-time list, used internally by <see cref="RadDatePicker"/> and <see cref="RadTimePicker"/>.
    /// </summary>
    [TemplateVisualState(Name = "Collapsed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "NotFocused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    public class DateTimeListItem : LoopingListItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeListItem"/> class.
        /// </summary>
        public DateTimeListItem()
        {
            this.DefaultStyleKey = typeof(DateTimeListItem);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.DateTimeListItemAutomationPeer(this);
        }
    }
}