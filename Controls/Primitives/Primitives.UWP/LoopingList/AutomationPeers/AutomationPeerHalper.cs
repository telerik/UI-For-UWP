using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    internal static class AutomationPeerHalper
    {
        internal static void RaiseAutomationEvent(this LoopingListItem loopingListItem)
        {
            if (loopingListItem.IsSelected)
            {
                loopingListItem.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
            }
        }

        internal static void RaiseAutomationEvent(this FrameworkElement target, params AutomationEvents[] events)
        {
            foreach (AutomationEvents eventId in events)
            {
                if (eventId.ListenerExists())
                {
                    var peer = FrameworkElementAutomationPeer.FromElement(target);
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(eventId);
                    }
                }
            }
        }

        internal static bool ListenerExists(this AutomationEvents eventId)
        {
            return AutomationPeer.ListenerExists(eventId);
        }
    }
}
