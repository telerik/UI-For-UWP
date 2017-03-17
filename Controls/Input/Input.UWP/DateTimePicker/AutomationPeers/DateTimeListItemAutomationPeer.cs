using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.DateTimePickers;

namespace Telerik.UI.Automation.Peers
{
    public class DateTimeListItemAutomationPeer : LoopingListItemAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeListItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">The DateTimeListItem that is associated with this DateTimeListItemAutomationPeer.</param>
        public DateTimeListItemAutomationPeer(DateTimeListItem owner)
            : base(owner)
        {
        }

        private DateTimeListItem DateTimeListItem
        {
            get
            {
                return (DateTimeListItem)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var dateTimeItem = this.DateTimeListItem.Content as DateTimeItem;
            if (dateTimeItem != null)
            {
                 var dateTimeList = this.DateTimeListItem.Panel.Owner as DateTimeList;
                if (dateTimeList != null && !string.IsNullOrEmpty(dateTimeItem.ContentText) 
                    && !string.IsNullOrEmpty(dateTimeItem.HeaderText))
                {
                    if (dateTimeList.ComponentType == DateTimeComponentType.Day || dateTimeList.ComponentType == DateTimeComponentType.Year)
                    {
                        return dateTimeItem.HeaderText;
                    }

                    return dateTimeItem.ContentText;
                }
            }

            return base.GetNameCore();
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "DateTimeListItem";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datetime list item";
        }
    }
}
