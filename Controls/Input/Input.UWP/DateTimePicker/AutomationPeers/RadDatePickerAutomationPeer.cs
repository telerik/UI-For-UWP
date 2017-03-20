using Telerik.UI.Xaml.Controls.Input;

namespace Telerik.UI.Automation.Peers
{
    public class RadDatePickerAutomationPeer : DateTimePickerAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadDatePickerAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadDatePicker that is associated with this RadDatePickerAutomationPeer.</param>
        public RadDatePickerAutomationPeer(RadDatePicker owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "RadDatePicker";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad date picker";
        }
    }
}
