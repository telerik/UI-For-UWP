using System;
using System.Collections.Generic;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a <see cref="DataGridFilterControlBase"/> that is may be used to filter data presented by a <see cref="DataGridTypedColumn"/> instance.
    /// </summary>
    [TemplatePart(Name = "PART_OperatorCombo", Type = typeof(ComboBox))]
    public abstract class DataGridTypedFilterControl : DataGridFilterControlBase
    {
        private ComboBox operatorCombo;

        /// <summary>
        /// Gets or sets the name of the property that will be used in the underlying <see cref="Telerik.Data.Core.PropertyFilterDescriptor"/>.
        /// </summary>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the PART_OperatorCombo template part.
        /// </summary>
        internal ComboBox OperatorCombo
        {
            get
            {
                return this.operatorCombo;
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.operatorCombo = this.GetTemplatePartField<ComboBox>("PART_OperatorCombo");
            applied = applied && this.operatorCombo != null;

            return applied;
        }

        /// <summary>
        /// Initializes the control depending on the current <see cref="P:AssociatedDescriptor"/> value.
        /// </summary>
        protected override void Initialize()
        {
            // populate the operator combo
            foreach (var op in this.GetOperators())
            {
                this.operatorCombo.Items.Add(op);
            }
        }

        /// <inheritdoc/>
        protected virtual IEnumerable<string> GetOperators()
        {
            foreach (var op in Enum.GetValues(typeof(TextOperator)))
            {
                yield return GridLocalizationManager.Instance.GetString(op.ToString());
            } 
        }
    }
}
