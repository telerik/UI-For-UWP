using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.DateTimePickers;
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the DateTimePickerButton class.
    /// </summary>
    public class DateTimePickerButtonAutomationPeer : ButtonAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the DateTimePickerButtonAutomationPeer class.
        /// </summary>
        /// <param name="owner">The DateTimePickerButton that is associated with this DateTimePickerButtonAutomationPeer.</param>
        public DateTimePickerButtonAutomationPeer(DateTimePickerButton owner) 
            : base(owner)
        {
        }

        private DateTimePickerButton DateTimePickerButton
        {
            get
            {
                return (DateTimePickerButton)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "DateTimePickerButton";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datetime picker button";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();

            var dateTimePickerParent = ElementTreeHelper.FindVisualAncestor<DateTimePicker>(this.DateTimePickerButton);
            if (dateTimePickerParent != null)
            {
                var dateTimePickerPeer = FrameworkElementAutomationPeer.FromElement(dateTimePickerParent) as DateTimePickerAutomationPeer;
                if (dateTimePickerPeer != null)
                {
                    var datePickerHeader = dateTimePickerPeer.GetName();
                    if (!string.IsNullOrEmpty(datePickerHeader) && dateTimePickerParent.Header != null)
                    {
                        return string.Format("{0} {1}", datePickerHeader, nameCore);
                    }
                }
            }

            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return string.Empty;
        }
    }
}
