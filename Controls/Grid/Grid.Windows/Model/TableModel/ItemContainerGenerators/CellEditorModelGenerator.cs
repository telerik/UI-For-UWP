using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class CellEditorModelGenerator : ItemModelGenerator<GridCellEditorModel, CellGenerationContext>
    {
        public CellEditorModelGenerator(IUIContainerGenerator<GridCellEditorModel, CellGenerationContext> owner)
            : base(owner)
        {
        }

        internal override object ContainerTypeForItem(CellGenerationContext context)
        {
            return this.Owner.GetContainerTypeForItem(context);
        }

        internal override void PrepareContainerForItem(GridCellEditorModel decorator)
        {
            this.Owner.PrepareContainerForItem(decorator);
        }

        protected override void ClearContainerForItem(GridCellEditorModel decorator)
        {
            this.Owner.ClearContainerForItem(decorator);
        }

        protected override GridCellEditorModel GenerateContainerForItem(CellGenerationContext context, object containerType)
        {
            var element = this.Owner.GenerateContainerForItem(context, containerType);

            return new GridCellEditorModel { Container = element, ContainerType = containerType, IsFrozen = context.IsFrozen };
        }
    }
}
