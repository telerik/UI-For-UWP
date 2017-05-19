using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadDataForm"/> class.
    /// </summary>
    public class RadDataFormAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadDataFormAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">RadDataForm owner.</param>
        public RadDataFormAutomationPeer(RadDataForm owner) : base(owner)
        {
        }

        private RadDataForm DataFormOwner
        {
            get
            {
                return (RadDataForm)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad data form";
        }
    }
}
