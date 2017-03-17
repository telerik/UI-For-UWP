using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadDataBoundListBox"/>.
    /// </summary>
    public class RadDataBoundListBoxAutomationPeer : RadControlAutomationPeer, ISelectionProvider
    {
        private RadDataBoundListBox ListBoxOwner
        {
            get
            {
                return (RadDataBoundListBox)this.Owner;
            }
        }       

        /// <summary>
        ///  Initializes a new instance of the <see cref="RadDataBoundListBoxAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="RadDataBoundListBox"/> that is associated with this <see cref="RadDataBoundListBoxAutomationPeer"/>.</param>
        public RadDataBoundListBoxAutomationPeer(RadDataBoundListBox owner) : base(owner)
        {

        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is selected.
        /// </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            List<IRawElementProviderSimple> providerSamples = new List<IRawElementProviderSimple>();

            if (this.ListBoxOwner.IsCheckModeEnabled)
            {
                foreach (object selected in this.ListBoxOwner.CheckedItems)
                {
                    RadDataBoundListBoxItem container = this.ListBoxOwner.GetContainerForItem(selected) as RadDataBoundListBoxItem;
                    if (container == null)
                        continue;

                    AutomationPeer itemPeer = (RadDataBoundListBoxItemAutomationPeer)CreatePeerForElement(container);
                    if (itemPeer != null)
                    {
                        providerSamples.Add(this.ProviderFromPeer(itemPeer));
                    }
                }
            }
            else
            {
                RadDataBoundListBoxItem container = this.ListBoxOwner.GetContainerForItem(this.ListBoxOwner.SelectedItem) as RadDataBoundListBoxItem;
                if (container != null)
                {
                    AutomationPeer itemPeer = (RadDataBoundListBoxItemAutomationPeer)CreatePeerForElement(container);
                    if (itemPeer != null)
                    {
                        providerSamples.Add(this.ProviderFromPeer(itemPeer));
                    }
                }
            }

            return providerSamples.ToArray();
        }

        /// <summary>
        ///  Gets a value that specifies whether the UI Automation provider allows more than one child element to be selected concurrently.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                // By checking multiple items user can simulate multiple selection in RadDataBoundListBox.
                // When CheckMode is not enabled, user can use standard single selection mode.
                return this.ListBoxOwner.IsCheckModeEnabled;
            }
        }

        /// <summary>
        /// Gets a value that specifies whether the UI Automation provider requires at least one child element to be selected.
        /// </summary>
        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
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

        /// <inheritdoc />	
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad databound list box";
        }
    }
}
