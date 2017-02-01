using System;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IUIContainerGenerator<D, I>
    {
        void PrepareContainerForItem(D decorator);
        void ClearContainerForItem(D decorator);
        object GetContainerTypeForItem(I info);
        object GenerateContainerForItem(I info, object containerType);

        void MakeVisible(D decorator);
        void MakeHidden(D decorator);

        void SetOpacity(D decorator, byte opacity);
    }
}
