using Telerik.UI.Xaml.Controls.Input;

namespace Telerik.UI.Automation.Peers
{
    public class RadTimePickerAutomationPeer : DateTimePickerAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadTimePickerAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadTimePicker that is associated with this RadTimePickerAutomationPeer.</param>
        public RadTimePickerAutomationPeer(RadTimePicker owner) 
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
            return "RadTimePicker";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad time picker";
        }
    }
}
