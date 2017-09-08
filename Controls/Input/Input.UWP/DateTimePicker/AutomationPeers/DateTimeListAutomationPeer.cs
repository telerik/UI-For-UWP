using Telerik.UI.Xaml.Controls.Input.DateTimePickers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the DateTimeList class.
    /// </summary>
    public class DateTimeListAutomationPeer : RadLoopingListAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeListAutomationPeer class.
        /// </summary>
        /// <param name="owner">The DateTimeList that is associated with this DateTimeListAutomationPeer.</param>
        public DateTimeListAutomationPeer(DateTimeList owner) 
            : base(owner)
        {
        }
        
        private DateTimeList DateTimeList
        {
            get
            {
                return (DateTimeList)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            return this.DateTimeList.ComponentType.ToString();
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "DateTimeList";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datetime list";
        }
    }
}
