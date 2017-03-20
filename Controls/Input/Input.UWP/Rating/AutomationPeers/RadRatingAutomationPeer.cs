using System;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using System.Linq;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation peer class for RadRating.
    /// </summary>
    public class RadRatingAutomationPeer : RadControlAutomationPeer, IValueProvider, ISelectionProvider
    {
        private RadRating Rating
        {
            get
            {
                return (RadRating)this.Owner;
            }
        }

        /// <summary>
        ///  Initializes a new instance of the RadRatingAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadRating that is associated with this RadRatingAutomationPeer.</param>
        public RadRatingAutomationPeer(RadRating owner) : base(owner)
        {

        }

        /// <summary>
        /// Gets whether the Rating is in read-only state.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.Rating.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets the value of the Rating.
        /// </summary>
        public string Value
        {
            get
            {
                return this.Rating.Value.ToString();
            }
        }

        /// <summary>
        /// Tries to set the value of the Rating.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            double parsedDouble = 0;
            if (double.TryParse(value, out parsedDouble))
            {
                this.Rating.Value = parsedDouble;
            }
            else
            {
                throw new InvalidOperationException("Given string is not successfully parsed to double");
            }
        }

        // <summary>
        // Retrieves a UI Automation provider for each child element that is selected.
        // </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            RadRatingItem selectedRatingItem = this.Rating.Items.LastOrDefault(item => this.Rating.GetIndexOf(item) < this.Rating.Value);
            if (selectedRatingItem != null)
            {
                return new[] { ProviderFromPeer(FromElement(selectedRatingItem)) };
            }
            return new IRawElementProviderSimple[] { };
        }

        /// <summary>
        ///  Gets a value that specifies whether the UI Automation provider allows more than one child element to be selected concurrently.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                return false;
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
            if (patternInterface == PatternInterface.Value || patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />	
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Slider | AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            if (this.Rating != null && !string.IsNullOrEmpty(this.Rating.Name))
                return this.Rating.Name;

            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad rating";
        }          
    }
}
