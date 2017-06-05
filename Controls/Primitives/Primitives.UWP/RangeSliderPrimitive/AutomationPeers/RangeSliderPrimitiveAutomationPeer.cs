using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RangeSliderPrimitive class.
    /// </summary>
    public class RangeSliderPrimitiveAutomationPeer : SliderBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RangeSliderPrimitiveAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RangeSliderPrimitiveAutomationPeer(RangeSliderPrimitive owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.RangeSliderPrimitive);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "range slider primitive";
        }
    }
}
