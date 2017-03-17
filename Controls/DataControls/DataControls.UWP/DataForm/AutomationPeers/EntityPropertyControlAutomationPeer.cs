using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="EntityPropertyControl"/> class.
    /// </summary>
    public class EntityPropertyControlAutomationPeer : RadControlAutomationPeer
    {
        private EntityPropertyControl EntityPropertyControlOwner
        {
            get
            {
                return (EntityPropertyControl)this.Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyControlAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">EntityPropertyControl owner.</param>
        public EntityPropertyControlAutomationPeer(EntityPropertyControl owner) : base(owner)
        {

        }

        /// <inheritdoc />	
        protected override string GetNameCore()
        {
            if (this.EntityPropertyControlOwner.Property != null)
            {
                return this.EntityPropertyControlOwner.Property.Label;
            }
            return base.GetNameCore();
        }

        /// <inheritdoc />	
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />	
        protected override string GetLocalizedControlTypeCore()
        {
            return "Entity Property Control";
        }
    }
}
