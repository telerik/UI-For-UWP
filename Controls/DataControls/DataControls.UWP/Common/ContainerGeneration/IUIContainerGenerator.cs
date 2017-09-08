namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal interface IUIContainerGenerator<D, I>
    {
        void PrepareContainerForItem(D element);

        void ClearContainerForItem(D element);

        object GetContainerTypeForItem(I info);

        object GenerateContainerForItem(I info, object containerType);

        void MakeVisible(D element);

        void MakeHidden(D element);

        void SetOpacity(D element, byte opacity);
    }
}
