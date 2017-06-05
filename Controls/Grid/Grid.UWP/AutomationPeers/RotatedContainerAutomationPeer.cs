using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RotatedContainer"/>.
    /// </summary>
    public class RotatedContainerAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RotatedContainerAutomationPeer class.
        /// </summary>
        public RotatedContainerAutomationPeer(RotatedContainer owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.RotatedContainer);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rotated container";
        }
        
        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return nameof(Telerik.UI.Xaml.Controls.Grid.Primitives.RotatedContainer);
        }
    }
}
