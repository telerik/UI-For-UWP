using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ListView.Model.ContainerGeneration;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Model
{
    internal partial class LayoutController
    {
        private HeaderGeneratedModel headerModel = new HeaderGeneratedModel();
        private FooterGeneratedModel footerModel = new FooterGeneratedModel();
        private EmptyContentGeneratedModel emptyContentModel = new EmptyContentGeneratedModel();

        private RadSize MeasureHeader(RadSize size)
        {
            var resultSize = this.owner.Measure(this.headerModel, size);

            this.headerModel.LayoutSlot = new RadRect(0, 0, resultSize.Width, resultSize.Height);

            return resultSize;
        }

        private RadSize MeasureEmptyContent(RadSize size)
        {
            var resultSize = this.owner.Measure(this.emptyContentModel, size);

            this.emptyContentModel.LayoutSlot = new RadRect(0, 0, resultSize.Width, resultSize.Height);

            return resultSize;
        }

        private RadSize MeasureFooter(RadSize size)
        {
            var resultSize = this.owner.Measure(this.footerModel, size);

            this.footerModel.LayoutSlot = new RadRect(0, 0, resultSize.Width, resultSize.Height);

            return resultSize;
        }

        private RadRect ArrangeHeader()
        {
            this.owner.Arrange(this.headerModel);

            return this.headerModel.LayoutSlot;
        }

        private RadRect ArrangeEmptyContent()
        {
            this.owner.Arrange(this.emptyContentModel);

            return this.emptyContentModel.LayoutSlot;
        }

        private RadRect ArrangeFooter()
        {
            this.owner.Arrange(this.footerModel);

            return this.footerModel.LayoutSlot;
        }

        private void UpdateFooterLayoutSlotPosition(RadSize position)
        {
            this.footerModel.LayoutSlot = new RadRect(position.Width, position.Height, this.footerModel.LayoutSlot.Width, this.footerModel.LayoutSlot.Height);
        }

        private void UpdateEmptyContentLayoutSlotPosition(RadSize position)
        {
            this.emptyContentModel.LayoutSlot = new RadRect(position.Width, position.Height, this.emptyContentModel.LayoutSlot.Width, this.emptyContentModel.LayoutSlot.Height);
        }
    }
}
