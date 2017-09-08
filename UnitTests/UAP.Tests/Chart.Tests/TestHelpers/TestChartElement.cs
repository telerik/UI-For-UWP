using Telerik.Charting;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Chart.Tests
{
    internal class TestChartElement : Element
    {
        internal bool tunnelMessageReceived;
        internal bool bubbleMessageReceived;
        internal bool stopMessage = false;

        public TestChartElement()
        {
            this.ElementCollection1 = new ElementCollection<Node>(this);
            this.ElementCollection2 = new ElementCollection<Node>(this);
        }

        internal override void ProcessMessage(Message message)
        {
            base.ProcessMessage(message);
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

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            return ModifyChildrenResult.Accept;
        }

        public ElementCollection<Node> ElementCollection1
        {
            get;
            private set;
        }

        public ElementCollection<Node> ElementCollection2
        {
            get;
            private set;
        }
    }
}
