using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class SlideControlAutomationPeer : RadControlAutomationPeer, IExpandCollapseProvider
    {
        public SlideControlAutomationPeer(SlideControl owner) 
            : base(owner)
        {
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
                return nameCore;

            return "SlideControl";
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(SlideTileExpandedState oldValue, SlideTileExpandedState newValue)
        {
            ExpandCollapseState oldState = SlideTileExpandedStateIntoExpandCollapse(oldValue);
            ExpandCollapseState newState = SlideTileExpandedStateIntoExpandCollapse(newValue);
            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldState, newState);
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
