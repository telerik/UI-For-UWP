using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="DataGridColumnHeader"/>.
    /// </summary>
    public class DataGridColumnHeaderAutomationPeer : RadControlAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the DataGridColumnHeaderAutomationPeer class.
        /// </summary>
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
        public void Invoke()
        {
            if (this.OwnerDataGridColumnHeader.Owner != null)
            {
                this.OwnerDataGridColumnHeader.Owner.OnColumnHeaderTap(this.OwnerDataGridColumnHeader, new Windows.UI.Xaml.Input.TappedRoutedEventArgs());
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
            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.DataGridColumnHeader);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.DataGridColumnHeader);
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

            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.DataGridColumnHeader);
        }

        /// <inheritdoc />
        protected override bool IsContentElementCore()
        {
            return false;
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
    }
}
