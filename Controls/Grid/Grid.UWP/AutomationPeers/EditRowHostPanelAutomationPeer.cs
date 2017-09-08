using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="EditRowHostPanel"/>.
    /// </summary>
    public class EditRowHostPanelAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditRowHostPanelAutomationPeer"/> class.
        /// </summary>
        public EditRowHostPanelAutomationPeer(EditRowHostPanel owner)
            : base(owner)
        {
        }

        private EditRowHostPanel EditRowHostPanel
        {
            get
            {
                return this.Owner as EditRowHostPanel;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(EditRowHostPanel);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "editrow host panel";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Table;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return nameof(EditRowHostPanel);
        }
    }
}
