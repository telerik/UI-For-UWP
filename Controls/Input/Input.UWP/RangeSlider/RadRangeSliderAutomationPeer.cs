using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    public class RadRangeSliderAutomationPeer : SliderBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadRangeSliderAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RadRangeSliderAutomationPeer(RadRangeSlider owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadRangeSlider);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad range slider";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            var children = base.GetChildrenCore().ToList();
            if (children != null && children.Count > 0)
            {
                children.RemoveAll(x => x.GetClassName() == nameof(ScalePrimitive));
            }

            return children;
        }
    }
}
