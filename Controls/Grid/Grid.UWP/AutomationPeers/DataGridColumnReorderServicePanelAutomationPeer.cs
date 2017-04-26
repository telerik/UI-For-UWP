using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="DataGridColumnReorderServicePanel"/>.
    /// </summary>
    public class DataGridColumnReorderServicePanelAutomationPeer : RadControlAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the DataGridColumnReorderServicePanelAutomationPeer class.
        /// </summary>
        public DataGridColumnReorderServicePanelAutomationPeer(DataGridColumnReorderServicePanel owner) 
            : base(owner)
        {
        }

        private DataGridColumnReorderServicePanel DataGridColumnReorderServicePanel
        {
            get
            {
                return this.Owner as DataGridColumnReorderServicePanel;
            }
        }

        /// <summary>
        /// IInvokeProvider implementation.
        /// </summary>
        public void Invoke()
        {
            this.DataGridColumnReorderServicePanel.OpenColumnsFlyout();
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(DataGridColumnReorderServicePanel);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datagrid column reorder service panel";
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

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return nameof(DataGridColumnReorderServicePanel);
        }
    }
}
