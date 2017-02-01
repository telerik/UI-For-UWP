using System;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IDecorationPresenter<T> where T : GridElement
    {
        void MakeVisible(T element);
        void Collapse(T element);

        bool HasDecorations(DecorationType decoratorElementType, ItemInfo itemInfo);

        object GenerateContainerForDecorator();

        void ApplyDecoratorProperties(T decorator, DecorationType decoratorElementType);
        RadRect ArrangeDecorator(T decorator, RadRect radRect);
    }
}
