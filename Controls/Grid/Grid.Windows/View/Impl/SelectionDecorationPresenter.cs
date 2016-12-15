using System;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class SelectionDecorationPresenter : IDecorationPresenter<SelectionRegionModel>
    {
        private SelectionLayer owner;

        internal SelectionDecorationPresenter()
        {
        }

        public SelectionLayer Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                if (this.owner == value)
                {
                    return;
                }

                // TODO:Update/Recycle the UI in the layer
                this.owner = value;
            }
        }

        void IDecorationPresenter<SelectionRegionModel>.MakeVisible(SelectionRegionModel element)
        {
            if (this.owner != null)
            {
                this.owner.MakeVisible(element);
            }
        }

        void IDecorationPresenter<SelectionRegionModel>.Collapse(SelectionRegionModel element)
        {
            if (this.owner != null)
            {
                this.owner.Collapse(element);
            }
        }

        bool IDecorationPresenter<SelectionRegionModel>.HasDecorations(DecorationType decoratorElementType, ItemInfo itemInfo)
        {
            return true;
        }

        object IDecorationPresenter<SelectionRegionModel>.GenerateContainerForDecorator()
        {
            if (this.owner != null)
            {
                return this.owner.GenerateContainerForDecorator();
            }

            return null;
        }

        void IDecorationPresenter<SelectionRegionModel>.ApplyDecoratorProperties(SelectionRegionModel decorator, DecorationType decoratorElementType)
        {
            if (this.owner != null)
            {
                this.owner.ApplyDecoratorProperties(decorator, decoratorElementType);
            }
        }

        RadRect IDecorationPresenter<SelectionRegionModel>.ArrangeDecorator(SelectionRegionModel decorator, RadRect radRect)
        {
            if (this.owner != null)
            {
                return this.owner.ArrangeDecorator(decorator, radRect);
            }

            return RadRect.Empty;
        }
    }
}
