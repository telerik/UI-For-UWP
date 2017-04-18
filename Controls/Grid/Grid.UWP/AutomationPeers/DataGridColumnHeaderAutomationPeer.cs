using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class DataGridColumnHeaderAutomationPeer : RadControlAutomationPeer, IInvokeProvider
    {
        public DataGridColumnHeaderAutomationPeer(DataGridColumnHeader owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        private DataGridColumnHeader OwnerDataGridColumnHeader
        {
            get
            {
                return (DataGridColumnHeader)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            if (this.OwnerDataGridColumnHeader != null && this.OwnerDataGridColumnHeader.Content != null)
            {
                return this.OwnerDataGridColumnHeader.Content.ToString();
            }

            return string.Empty;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.HeaderItem;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridColumnHeader);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(DataGridColumnHeader);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid column header";
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(DataGridColumnHeader);
        }

        /// <inheritdoc />
        protected override bool IsContentElementCore()
        {
            return false;
        }

        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        public void Invoke()
        {
            if (this.OwnerDataGridColumnHeader.Owner != null)
            {
                this.OwnerDataGridColumnHeader.Owner.OnColumnHeaderTap(this.OwnerDataGridColumnHeader, new Windows.UI.Xaml.Input.TappedRoutedEventArgs());
            }
        }
    }
}
