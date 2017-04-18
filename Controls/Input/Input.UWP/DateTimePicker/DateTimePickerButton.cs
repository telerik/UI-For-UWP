using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.DateTimePickers
{
    /// <summary>
    /// Represents an extended <see cref="Button"/> class that represents the "Picker" part in a <see cref="DateTimePicker"/> control.
    /// </summary>
    public class DateTimePickerButton : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimePickerButton"/> class.
        /// </summary>
        public DateTimePickerButton()
        {
            this.DefaultStyleKey = typeof(DateTimePickerButton);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DateTimePickerButtonAutomationPeer(this);
        }
    }
}