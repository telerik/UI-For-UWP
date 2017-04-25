using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the SlideControl class.
    /// </summary>
    public class SlideControlAutomationPeer : RadControlAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the SlideControlAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public SlideControlAutomationPeer(SlideControl owner) 
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                if (this.SlideControl.ExpandedState == Xaml.Controls.Primitives.SlideTileExpandedState.Normal)
                {
                    return ExpandCollapseState.Collapsed;
                }
                else if (this.SlideControl.ExpandedState == Xaml.Controls.Primitives.SlideTileExpandedState.SemiExpanded)
                {
                    return ExpandCollapseState.PartiallyExpanded;
                }

                return ExpandCollapseState.Expanded;
            }
        }

        private SlideControl SlideControl
        {
            get
            {
                return (SlideControl)this.Owner;
            }
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Collapse()
        {
            this.SlideControl.ExpandedState = Xaml.Controls.Primitives.SlideTileExpandedState.Normal;
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Expand()
        {
            this.SlideControl.ExpandedState = Xaml.Controls.Primitives.SlideTileExpandedState.Expanded;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(SlideTileExpandedState oldValue, SlideTileExpandedState newValue)
        {
            ExpandCollapseState oldState = this.SlideTileExpandedStateIntoExpandCollapse(oldValue);
            ExpandCollapseState newState = this.SlideTileExpandedStateIntoExpandCollapse(newValue);
            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldState, newState);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "SlideControl";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "slide control";
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }

            return null;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return "SlideControl";
        }

        private ExpandCollapseState SlideTileExpandedStateIntoExpandCollapse(SlideTileExpandedState state)
        {
            if (state == Xaml.Controls.Primitives.SlideTileExpandedState.Normal)
            {
                return ExpandCollapseState.Collapsed;
            }
            else if (state == Xaml.Controls.Primitives.SlideTileExpandedState.SemiExpanded)
            {
                return ExpandCollapseState.PartiallyExpanded;
            }

            return ExpandCollapseState.Expanded;
        }
    }
}
