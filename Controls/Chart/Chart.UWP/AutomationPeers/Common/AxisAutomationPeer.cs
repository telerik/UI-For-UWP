using Telerik.Core;
using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="Axis"/>.
    /// </summary>
    public class AxisAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the AxisAutomationPeer class.
        /// </summary>
        public AxisAutomationPeer(Axis owner) 
            : base(owner)
        {
        }

        private Axis Axis
        {
            get
            {
                return (Axis)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Axis);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Axis);
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            if (this.Axis.Title != null)
            {
                return this.Axis.Title.ToString();
            }

            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "axis";
        }
    }
}
