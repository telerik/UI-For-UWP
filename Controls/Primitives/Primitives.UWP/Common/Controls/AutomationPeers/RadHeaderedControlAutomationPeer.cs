using Telerik.UI.Xaml.Controls;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    public class RadHeaderedControlAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadHeaderedControlAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RadHeaderedControlAutomationPeer(RadHeaderedControl owner) 
            : base(owner)
        {

        }

        private RadHeaderedControl HeaderedControl
        {
            get
            {
                return this.Owner as RadHeaderedControl;
            }
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Header;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// GetNameCore will return a value matching (in priority order)
        ///
        /// 1. Automation.Name
        /// 2. Header 
        /// 3. FrameworkElements.GetNameCore()
        /// 4. String.Empty
        /// 
        /// The priority mimics the behavior in AutoSuggestBox / ComboBox.
        /// </summary>
        protected override string GetNameCore()
        {
            string result = AutomationProperties.GetName(this.HeaderedControl);
            if (!string.IsNullOrEmpty(result))
                return result;

            result = this.HeaderedControl.Header as string;
            if (!string.IsNullOrEmpty(result))
                return result;

            result = base.GetNameCore();
            if (!string.IsNullOrEmpty(result))
                return result;

            return string.Empty;
        }
        
        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad headered control";
        }
    }
}
