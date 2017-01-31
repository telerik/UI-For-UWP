using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal abstract class SelectionLayer : SharedUILayer
    {
        internal abstract void MakeVisible(GridElement element);

        internal abstract void Collapse(GridElement element);

        internal abstract object GenerateContainerForDecorator();

        internal abstract void ApplyDecoratorProperties(SelectionRegionModel decorator, DecorationType decoratorElementType);

        internal abstract RadRect ArrangeDecorator(SelectionRegionModel decorator, RadRect radRect);
    }
}
