using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadSegmentedControl"/>.
    /// </summary>
    public class RadSegmentedControlAutomationPeer : RadControlAutomationPeer, ISelectionProvider
    {
        private static string SegmentedControlName = nameof(RadSegmentedControl);
        private RadSegmentedControl segmentedControlOwner;

        /// <summary>
        ///  Initializes a new instance of the <see cref="RadSegmentedControlAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="RadSegmentedControl"/> that is associated with this <see cref="RadSegmentedControlAutomationPeer"/>.</param>
        public RadSegmentedControlAutomationPeer(RadSegmentedControl owner) 
            : base(owner)
        {
            this.segmentedControlOwner = owner;
        }

        /// <summary>
        /// Gets a value indicating whether that specifies whether the UI Automation provider allows more than one child element to be selected concurrently.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether that specifies whether the UI Automation provider requires at least one child element to be selected.
        /// </summary>
        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is selected.
        /// </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            if (this.segmentedControlOwner.SelectedItem == null)
            {
                return new IRawElementProviderSimple[0];
            }

            Segment selectedSegment = (Segment)this.segmentedControlOwner.ItemsControl.ContainerFromItem(this.segmentedControlOwner.SelectedItem);
            AutomationPeer selectedItemPeer = (SegmentAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(selectedSegment);
            IRawElementProviderSimple selectedItemProvider = this.ProviderFromPeer(selectedItemPeer);

            return new IRawElementProviderSimple[] { selectedItemProvider };
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> segmentPeers = new List<AutomationPeer>();
            foreach (var segmentedItem in this.segmentedControlOwner.Items)
            {
                Segment segment = (Segment)this.segmentedControlOwner.ItemsControl.ContainerFromItem(segmentedItem);
                AutomationPeer segmentPeer = FrameworkElementAutomationPeer.CreatePeerForElement(segment);
                segmentPeers.Add(segmentPeer);
            }

            return segmentPeers;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return SegmentedControlName;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            return SegmentedControlName;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }
    }
}
