using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation peer class for RadAutoCompleteBox.
    /// </summary>
    public class RadAutoCompleteBoxAutomationPeer : RadHeaderedControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadAutoCompleteBoxAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadAutoCompleteBox that is associated with this RadAutoCompleteBoxAutomationPeer.</param>
        public RadAutoCompleteBoxAutomationPeer(RadAutoCompleteBox owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad autocomplete box";
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadAutoCompleteBox);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadAutoCompleteBox);
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return nameof(RadAutoCompleteBox);
        }

        /// <summary>
        /// Gets the control type for the RadAutoCompleteBox that is associated with this RadAutoCompleteBoxAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }
    }
}
