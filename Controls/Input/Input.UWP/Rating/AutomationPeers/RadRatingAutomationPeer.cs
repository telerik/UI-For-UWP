using System;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation peer class for RadRating.
    /// </summary>
    public class RadRatingAutomationPeer : RadControlAutomationPeer, IValueProvider, IRangeValueProvider
    {
        /// <summary>
        ///  Initializes a new instance of the RadRatingAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadRating that is associated with this RadRatingAutomationPeer.</param>
        public RadRatingAutomationPeer(RadRating owner) : base(owner)
        {
        }

        /// <inheritdoc />
        public double LargeChange => 1.0;

        /// <inheritdoc />
        public double Maximum => this.Rating.Items.Count;

        /// <inheritdoc />
        public double Minimum => 0.0;

        /// <inheritdoc />
        public double SmallChange => 1.0;

        /// <inheritdoc />
        double IRangeValueProvider.Value
        {
            get
            {
                return this.Rating.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Rating is in read-only state.
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
                var localizedRatingString = InputLocalizationManager.Instance.GetString("RatingValueString");
                if (!string.IsNullOrEmpty(localizedRatingString))
                {
                    return string.Format(localizedRatingString, this.Rating.Value, this.Maximum);
                }

                return "Rating unset";
            }
        }

        private RadRating Rating
        {
            get
            {
                return (RadRating)this.Owner;
            }
        }

        /// <summary>
        /// Tries to set the value of the Rating.
        /// </summary>
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

        /// <inheritdoc />
        public void SetValue(double value)
        {
            RadRating owner = this.Owner as RadRating;
            owner.Value = value;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value || patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Slider;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            if (this.Rating != null && !string.IsNullOrEmpty(this.Rating.Name))
            {
                return this.Rating.Name;
            }

            return "Ratings control";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "slider";
        }
    }
}
