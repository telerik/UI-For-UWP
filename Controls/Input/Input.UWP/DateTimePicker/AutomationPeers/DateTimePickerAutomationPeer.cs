using System;
using System.Collections.Generic;
using System.Globalization;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the DateTimePicker class.
    /// </summary>
    public class DateTimePickerAutomationPeer : RadHeaderedControlAutomationPeer, IValueProvider, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the DateTimePickerAutomationPeer class.
        /// </summary>
        /// <param name="owner">The DateTimePicker that is associated with this DateTimePickerAutomationPeer.</param>
        public DateTimePickerAutomationPeer(DateTimePicker owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get
            {
                return this.DateTimePicker.IsEnabled ? this.DateTimePicker.IsReadOnly : true;
            }
        }

        /// <inheritdoc />
        public string Value
        {
            get
            {
                return this.DateTimePicker.ValueString;
            }
        }

        /// <inheritdoc />
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return this.DateTimePicker.IsOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        private DateTimePicker DateTimePicker
        {
            get
            {
                return (DateTimePicker)this.Owner;
            }
        }
        
        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public void SetValue(string value)
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Read-only DateTimePicker");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            DateTime? parsedValue = DateTime.Parse(value);
            if (parsedValue != null)
            {
                var oldValue = this.DateTimePicker == null ? string.Empty : this.DateTimePicker.Value.ToString();
                this.DateTimePicker.Value = parsedValue;
            }
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Collapse()
        {
            this.DateTimePicker.IsOpen = false;
        }

        /// <summary>
        /// IExpandCollapseProvider implementation.
        /// </summary>
        public void Expand()
        {
            this.DateTimePicker.IsOpen = true;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseSelectionAutomationEvent(string oldValue, string newValue)
        {
            this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "DateTimePicker";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "datetime picker";
        }

        /// <summary>
        /// Gets the control type for the DateTimePicker that is associated with this DateTimePickerAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Calendar;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value 
                || (this.DateTimePicker.DisplayMode != DateTimePickerDisplayMode.Inline && patternInterface == PatternInterface.ExpandCollapse))
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            var result = new List<AutomationPeer>();
            var children = base.GetChildrenCore();
            if (children != null)
            {
                result.AddRange(children);
            }

            if (this.DateTimePicker.DisplayMode == DateTimePickerDisplayMode.Standard)
            {
                if (this.DateTimePicker.OKButton != null)
                {
                    var okButtonPeer = FrameworkElementAutomationPeer.CreatePeerForElement(this.DateTimePicker.OKButton) as FrameworkElementAutomationPeer;
                    if (okButtonPeer != null)
                    {
                        result.Add(okButtonPeer);
                    }
                }

                if (this.DateTimePicker.CancelButton != null)
                {
                    var cancelButton = FrameworkElementAutomationPeer.CreatePeerForElement(this.DateTimePicker.CancelButton) as FrameworkElementAutomationPeer;
                    if (cancelButton != null)
                    {
                        result.Add(cancelButton);
                    }
                }

                foreach (var dateTimeList in this.DateTimePicker.dateTimeLists)
                {
                    var dateTimeListPeer = FrameworkElementAutomationPeer.CreatePeerForElement(dateTimeList) as DateTimeListAutomationPeer;
                    if (dateTimeListPeer != null)
                    {
                        result.Add(dateTimeListPeer);
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return string.Empty;
        }
    }
}
