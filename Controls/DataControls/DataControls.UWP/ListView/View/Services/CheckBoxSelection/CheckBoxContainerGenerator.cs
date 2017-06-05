using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class CheckBoxesContainerGenerator : IUIContainerGenerator<GeneratedItemModel, ItemInfo>
    {
        private ListViewItemCheckBoxService owner;

        internal CheckBoxesContainerGenerator(ListViewItemCheckBoxService owner)
        {
            this.owner = owner;
        }

        public void PrepareContainerForItem(GeneratedItemModel element)
        {
            this.owner.PrepareContainerForItem(element);
        }

        public void ClearContainerForItem(GeneratedItemModel element)
        {
            this.owner.ClearContainerForItem(element);
        }

        public object GetContainerTypeForItem(ItemInfo info)
        {
            return info.GetType();
        }

        public object GenerateContainerForItem(ItemInfo info, object containerType)
        {
            return this.owner.GenerateContainer();
        }

        public void MakeVisible(GeneratedItemModel element)
        {
            var visual = element.Container as UIElement;
            visual.ClearValue(CheckBox.VisibilityProperty);
        }

        public void MakeHidden(GeneratedItemModel element)
        {
            var visual = element.Container as UIElement;
            visual.Visibility = Visibility.Collapsed;
        }

        public void SetOpacity(GeneratedItemModel element, byte opacity)
        {
        }
    }
}
