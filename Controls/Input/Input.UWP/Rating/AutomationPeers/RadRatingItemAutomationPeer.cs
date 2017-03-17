using System;
using System.Globalization;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using System.Linq;
using Windows.UI.Xaml.Automation;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation peer class for RadRatingItem.
    /// </summary>
    public class RadRatingItemAutomationPeer : RadContentControlAutomationPeer, ISelectionItemProvider
    {
        private RadRatingItem RatingItem
        {
            get
            {
                return (RadRatingItem)this.Owner;
            }
        }

        /// <summary>
        ///  Initializes a new instance of the RadRatingItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadRatingItem that is associated with this RadRatingItemAutomationPeer.</param>
        public RadRatingItemAutomationPeer(RadRatingItem owner) : base(owner)
        {

        }       

        /// <summary>
        /// Indicates whether an item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                RadRating rating = this.RatingItem.Owner;
                RadRatingItem selectedRatingItem = rating.Items.LastOrDefault(item => rating.GetIndexOf(item) < rating.Value);

                bool selected = selectedRatingItem != null && selectedRatingItem == this.RatingItem;
                return selected;
            }
        }

        /// <summary>
        /// Specifies the provider that implements ISelectionProvider and acts as the container for the calling object.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                RadRating parentControl = this.RatingItem.Owner;
                if (parentControl != null)
                {
                    return this.ProviderFromPeer(CreatePeerForElement(parentControl));
                }
                return null;
            }
        }

        /// <summary>
        /// Not implemented in this single-value control.
        /// </summary>
        public void AddToSelection()
        {
            if (!this.IsSelected)
            {
                RadRating rating = this.RatingItem.Owner;
                RadRatingItem selectedRatingItem = rating.Items.LastOrDefault(item => rating.GetIndexOf(item) < rating.Value);
                if (selectedRatingItem != null && selectedRatingItem != this.RatingItem)
                {
                    throw new ArgumentException("Rating supports only single selection.");
                }
                this.Select();
            }
            else
            {
                throw new InvalidOperationException("Element is already selected");
            }
        }

        /// <summary>
        /// Not implemented in this single-value control.
        /// </summary>
        public void RemoveFromSelection()
        {
            if (this.IsSelected)
            {
                this.RatingItem.Owner.Value = 0d;
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, true, false);
            }
        }

        /// <summary>
        /// Selects the current element.
        /// </summary>
        public void Select()
        {
            bool selectSuccess = this.RatingItem.Select();
            if (selectSuccess)
            {
                this.RaiseIsSelectedAutomationEventAndFocusChanged(false, true);
            }
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

        /// <summary>
        /// Returns the name of the RadRatingItem. 
        /// </summary>
        protected override string GetNameCore()
        {
            string baseName = base.GetNameCore();
            if (!string.IsNullOrEmpty(baseName))
                return baseName;

            int index = this.RatingItem.Owner.GetIndexOf(this.RatingItem);
            if (index != -1)
            {
                return (index + 1).ToString(CultureInfo.CurrentUICulture);
            }

            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad rating item";
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseIsSelectedAutomationEventAndFocusChanged(bool oldValue, bool newValue)
        {
            this.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, newValue);
        }
    }
}
