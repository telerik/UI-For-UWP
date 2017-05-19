using System;
using System.Diagnostics.CodeAnalysis;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a custom control that represents the <see cref="DataGridFilteringFlyout.FirstFilterControl"/> or <see cref="DataGridFilteringFlyout.SecondFilterControl"/>.
    /// </summary>
    public abstract class DataGridFilterControlBase : RadControl, IFilterControl
    {
        internal FilterDescriptorBase associatedDescriptor;
        private bool isFirst;

        /// <summary>
        /// Gets or sets a value indicating whether this is the First filtering control in the UI.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IFilterControl.IsFirst
        {
            get
            {
                return this.isFirst;
            }
            set
            {
                this.isFirst = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="FilterDescriptorBase"/> associated with the filtering operation.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        FilterDescriptorBase IFilterControl.AssociatedDescriptor
        {
            get
            {
                return this.associatedDescriptor;
            }
            set
            {
                this.associatedDescriptor = value;
            }
        }

        /// <summary>
        /// Gets the actual associated descriptor, having in mind possible Composite filter.
        /// </summary>
        internal FilterDescriptorBase ActualAssociatedDescriptor
        {
            get
            {
                var compositeDescriptor = this.associatedDescriptor as CompositeFilterDescriptor;
                if (compositeDescriptor != null)
                {
                    if (compositeDescriptor.Descriptors.Count > 0 && this.isFirst)
                    {
                        return compositeDescriptor.Descriptors[0];
                    }

                    if (compositeDescriptor.Descriptors.Count > 1 && !this.isFirst)
                    {
                        return compositeDescriptor.Descriptors[1];
                    }
                }
                return this.associatedDescriptor;
            }
        }

        /// <summary>
        /// Builds the <see cref="FilterDescriptorBase"/> that describes the user input within this instance.
        /// </summary>
        public abstract FilterDescriptorBase BuildDescriptor();

        /// <summary>
        /// Initializes the control depending on the current <see cref="P:AssociatedDescriptor"/> value.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.Initialize();
        }
    }
}
