using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class DataGridFlyoutGroupHeaderAutomationPeer : RadControlAutomationPeer, IInvokeProvider
    {
        public DataGridFlyoutGroupHeaderAutomationPeer(DataGridFlyoutGroupHeader owner) 
            : base(owner)
        {
        }

        private DataGridFlyoutGroupHeader DataGridFlyoutGroupHeader
        {
            get
            {
                return this.Owner as DataGridFlyoutGroupHeader;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridFlyoutGroupHeader);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid flyout group header";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return nameof(DataGridFlyoutGroupHeader);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <summary>
        /// IInvokeProvider implementation.
        /// </summary>
        public void Invoke()
        {
            this.DataGridFlyoutGroupHeader.RaiseDescriptorContentTap();
        }
    }
}
