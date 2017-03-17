using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Automation.Peers
{
    public class RotatedContainerAutomationPeer : RadControlAutomationPeer
    {
        public RotatedContainerAutomationPeer(RotatedContainer owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RotatedContainer);
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
                return nameCore;

            return nameof(RotatedContainer);
        }
    }
}
