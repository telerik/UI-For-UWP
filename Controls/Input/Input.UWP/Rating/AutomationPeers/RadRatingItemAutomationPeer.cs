using System.Globalization;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation peer class for RadRatingItem.
    /// </summary>
    public class RadRatingItemAutomationPeer : RadContentControlAutomationPeer, IInvokeProvider
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
        
        /// <inheritdoc />	
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />	
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
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

        public void Invoke()
        {
            RadRating rating = this.RatingItem.Owner;
            RadRatingItem selectedRatingItem = rating.Items.LastOrDefault(item => rating.GetIndexOf(item) < rating.Value);
            if (selectedRatingItem != null && selectedRatingItem == this.RatingItem)
            {
                this.RatingItem.Owner.Value = 0d;
            }
            else
            {
                this.RatingItem.Select();
            }
        }
    }
}
