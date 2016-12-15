using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Defines the filtering User Interface used to allow users to visually filter a <see cref="DataGridColumn"/> instance.
    /// </summary>
    public interface IFilterControl
    {
        /// <summary>
        /// Gets or sets the <see cref="FilterDescriptorBase"/> instance associated with the control upon its loading into the visual tree.
        /// </summary>
        FilterDescriptorBase AssociatedDescriptor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is the first filtering control is the User Interface. 
        /// This flag is used to properly resolve the <see cref="AssociatedDescriptor"/> value when composite filtering is applied.
        /// </summary>
        bool IsFirst
        {
            get;
            set;
        }

        /// <summary>
        /// Builds the <see cref="FilterDescriptorBase"/> that will be added to the <see cref="RadDataGrid.FilterDescriptors"/> 
        /// collection after a successful user interaction with the control.
        /// </summary>
        FilterDescriptorBase BuildDescriptor();
    }
}
