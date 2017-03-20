using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    public class EditRowHostPanelAutomationPeer : FrameworkElementAutomationPeer
    {
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
                return nameCore;

            return nameof(EditRowHostPanel);
        }
    }
}
