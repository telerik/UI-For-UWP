using System;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class LineDecorationPresenter : IDecorationPresenter<LineDecorationModel>
    {
        private DecorationLayer owner;

        internal LineDecorationPresenter()
        {
        }

        public DecorationLayer Owner
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

        void IDecorationPresenter<LineDecorationModel>.MakeVisible(LineDecorationModel element)
        {
            if (this.owner != null)
            {
                this.owner.MakeVisible(element);
            }
        }

        void IDecorationPresenter<LineDecorationModel>.Collapse(LineDecorationModel element)
        {
            if (this.owner != null)
            {
                this.owner.Collapse(element);
            }
        }

        bool IDecorationPresenter<LineDecorationModel>.HasDecorations(DecorationType decoratorElementType, ItemInfo itemInfo)
        {
            if (this.owner != null)
            {
                return this.owner.HasDecorations(decoratorElementType, itemInfo);
            }

            return false;
        }

        object IDecorationPresenter<LineDecorationModel>.GenerateContainerForDecorator()
        {
            if (this.owner != null)
            {
                return this.owner.GenerateContainerForLineDecorator();
            }

            return null;
        }

        void IDecorationPresenter<LineDecorationModel>.ApplyDecoratorProperties(LineDecorationModel lineDecorator, DecorationType decoratorElementType)
        {
            if (this.owner != null)
            {
                this.owner.ApplyLineDecoratorProperties(lineDecorator, decoratorElementType);
            }
        }

        RadRect IDecorationPresenter<LineDecorationModel>.ArrangeDecorator(LineDecorationModel decorator, RadRect radRect)
        {
            if (this.owner != null)
            {
                return this.owner.ArrangeLineDecorator(decorator, radRect);
            }

            return RadRect.Empty;
        }
    }
}
