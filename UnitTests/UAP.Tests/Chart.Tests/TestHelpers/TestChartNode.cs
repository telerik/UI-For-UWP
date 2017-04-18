using Telerik.Charting;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Chart.Tests
{
    internal class TestChartNode : Node
    {
        internal bool parentChangedFired = false;
        internal bool processMessageCalled = false;
        internal bool stopMessage = false;

        internal bool tunnelMessageReceived;
        internal bool bubbleMessageReceived;

        internal override void ProcessMessage(Message message)
        {
            base.ProcessMessage(message);
            this.processMessageCalled = true;

            if (message.DispatchPhase == MessageDispatchPhase.Tunnel)
            {
                this.tunnelMessageReceived = true;
            }
            else
            {
                this.bubbleMessageReceived = true;
            }

            message.StopDispatch = this.stopMessage;
        }

        internal override void OnParentChanged(Element oldParent)
        {
            this.parentChangedFired = true;
            base.OnParentChanged(oldParent);
        }
    }
}
