using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="Segment"/>.
    /// </summary>
    public class SegmentAutomationPeer : FrameworkElementAutomationPeer, ISelectionItemProvider
    {
        private static string SegmentName = nameof(Segment);

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Segment"/> that is associated with this <see cref="SegmentAutomationPeer"/>.</param>
        public SegmentAutomationPeer(Segment owner) 
            : base(owner)
        {
        }

        /// <summary>
        /// Gets a value indicating whether an item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.SegmentOwner.IsSelected;
            }
        }

        /// <summary>
        /// Gets the provider that implements ISelectionProvider and acts as the container for the calling object.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                RadSegmentedControl segmentedControl = this.SegmentOwner.owner;
                if (segmentedControl != null)
                {
                    return this.ProviderFromPeer(FrameworkElementAutomationPeer.CreatePeerForElement(segmentedControl));
                }

                return null;
            }
        }

        private Segment SegmentOwner
        {
            get
            {
                return (Segment)this.Owner;
            }
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void AddToSelection()
        {
            this.SelectSegment();
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void RemoveFromSelection()
        {
            if (!this.IsSelected)
            {
                return;
            }

            this.SegmentOwner.owner.SelectedItem = null;
            this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, true, false);
            this.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void Select()
        {
            this.SelectSegment();
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return SegmentName;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return SegmentName;
        }

        private void SelectSegment()
        {
            if (this.IsSelected)
            {
                return;
            }

            RadSegmentedControl segmentedControl = this.SegmentOwner.owner;
            segmentedControl.SelectedItem = segmentedControl.ItemsControl.ItemFromContainer(this.SegmentOwner);
            this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, false, true);
            this.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
        }
    }
}
