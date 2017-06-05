using System.Collections.Generic;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadListView"/>.
    /// </summary>
    public class RadListViewAutomationPeer : RadControlAutomationPeer, ISelectionProvider
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="RadListViewAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="RadListView"/> that is associated with this <see cref="RadListViewAutomationPeer"/>.</param>
        public RadListViewAutomationPeer(RadListView owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets a value indicating whether that specifies whether the UI Automation provider allows more than one child element to be selected concurrently.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                return this.ListViewOwner.SelectionMode != DataControlsSelectionMode.None &&
                       this.ListViewOwner.SelectionMode != DataControlsSelectionMode.Single;
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

        private RadListView ListViewOwner
        {
            get
            {
                return (RadListView)this.Owner;
            }
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is selected.
        /// </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            List<IRawElementProviderSimple> providerSamples = new List<IRawElementProviderSimple>();

            foreach (object selected in this.ListViewOwner.SelectedItems)
            {
                ItemInfo? info = this.ListViewOwner.Model.FindItemInfo(selected);
                if (!info.HasValue)
                {
                    continue;
                }

                GeneratedItemModel generatedModel = this.ListViewOwner.Model.GetDisplayedElement(info.Value.Slot, info.Value.Id);
                RadListViewItem container = null;
                if (generatedModel != null)
                {
                    container = generatedModel.Container as RadListViewItem;
                }
                if (container == null)
                {
                    continue;
                }

                AutomationPeer itemPeer = (RadListViewItemAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(container);
                if (itemPeer != null)
                {
                    providerSamples.Add(this.ProviderFromPeer(itemPeer));
                }
            }

            return providerSamples.ToArray();
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
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> childPeers = new List<AutomationPeer>();

            foreach (GeneratedItemModel itemModel in this.ListViewOwner.Model.ForEachDisplayedElement())
            {
                if (itemModel.Container == null)
                {
                    continue;
                }

                RadListViewItem listItem = itemModel.Container as RadListViewItem;
                if (listItem != null)
                {
                    var peer = CreatePeerForElement(listItem);
                    childPeers.Add(peer);
                }
            }
            return childPeers;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad list view";
        }
    }
}
