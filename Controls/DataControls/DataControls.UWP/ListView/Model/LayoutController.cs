using System.Collections.Generic;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Model
{
    internal partial class LayoutController
    {
        internal BaseLayoutStrategy strategy;

        private readonly ItemModelGenerator modelGenerator;

        private RadSize availableSize;

        private IListView owner;

        private ListViewModel model;

        public LayoutController(IListView owner, ListViewModel model)
        {
            this.owner = owner;

            this.modelGenerator = new ItemModelGenerator(this.owner.ContainerGenerator);

            this.strategy = new StackLayoutStrategy(this.modelGenerator, owner);

            this.model = model;
        }

        public BaseLayout Layout
        {
            get
            {
                return this.strategy.Layout;
            }
        }

        public double AvailableLength
        {
            get
            {
                return this.strategy.AvailableLength;
            }
        }

        public RadSize MeasureContent(RadSize newAvailableSize)
        {
            double heightToSubstract = 0;
            var headerSize = this.MeasureHeader(newAvailableSize);
            heightToSubstract += headerSize.Height;

            var footerSize = this.MeasureFooter(newAvailableSize);
            heightToSubstract += footerSize.Height;

            this.availableSize = new RadSize(newAvailableSize.Width, newAvailableSize.Height - heightToSubstract);
            var emptyContentSize = this.MeasureEmptyContent(this.availableSize);

            var resultSize = this.strategy.MeasureContent(this.availableSize, this.owner.ScrollOffset, this.model.BufferScale);
            this.strategy.RecycleAfterMeasure();

            var emptyContentPosition = new RadSize(0, this.headerModel.LayoutSlot.Bottom + resultSize.Height);
            this.UpdateEmptyContentLayoutSlotPosition(emptyContentPosition);

            var footerPosition = new RadSize(0, this.emptyContentModel.LayoutSlot.Bottom);
            this.UpdateFooterLayoutSlotPosition(footerPosition);

            this.model.View.ItemCheckBoxService.GenerateVisuals();

            this.strategy.GenerateFrozenContainers();
            if (this.strategy.IsHorizontal)
            {
                return new RadSize(resultSize.Width + this.emptyContentModel.LayoutSlot.Width, resultSize.Height + this.emptyContentModel.LayoutSlot.Height);
            }
            else
            {
                return new RadSize(resultSize.Width + this.emptyContentModel.LayoutSlot.Width, resultSize.Height + this.emptyContentModel.LayoutSlot.Height + heightToSubstract);
            }
        }

        public void ArrangeContent(RadSize adjustedfinalSize)
        {
            if (this.strategy.IsItemsSourceChanging && this.model.IsDataReady)
            {
                this.HandleNewSourceAnimations();
                this.strategy.IsItemsSourceChanging = false;
            }

            if (this.owner.AnimatingService.HasItemsForAnimation())
            {
                this.HandleItemAddedAnimations();
            }

            var rect = this.ArrangeHeader();

            var finalSize = new RadSize(adjustedfinalSize.Width, adjustedfinalSize.Height);
            var offset = this.strategy.IsHorizontal ? 0 : rect.Bottom;
            this.strategy.ArrangeContent(finalSize, offset);

            this.ArrangeFooter();

            this.ArrangeEmptyContent();

            this.model.View.CurrencyService.ArrangeVisual();

            this.model.View.ItemCheckBoxService.ArrangeVisuals();
        }

        internal void SetSource(IReadOnlyList<object> source, int groupDescriptionCount, bool keepCollapsedState)
        {
            this.strategy.SetSource(source, groupDescriptionCount, keepCollapsedState);
        }

        internal void OnLayoutDefinitionChanged(LayoutDefinitionBase oldLayoutDefinition, LayoutDefinitionBase newLayoutDefinition)
        {
            this.owner.ScrollToTop();

            if (oldLayoutDefinition != null)
            {
                oldLayoutDefinition.PropertyChanged -= this.LayoutDefinition_PropertyChanged;
            }

            if (newLayoutDefinition != null)
            {
                newLayoutDefinition.PropertyChanged += this.LayoutDefinition_PropertyChanged;
            }

            if (this.strategy != null)
            {
                this.strategy.FullyRecycle();
            }

            this.strategy = newLayoutDefinition.CreateStrategy(this.modelGenerator, this.owner);

            if (this.model.CurrentDataProvider != null && this.model.CurrentDataProvider.Status == DataProviderStatus.Ready)
            {
                this.model.SetLayoutSource();
            }

            this.strategy.FullyRecycle();

            this.owner.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);

            // TODO: add callback for orientation changed
        }

        internal void OnOrientationChanged(Orientation orientation)
        {
            this.strategy.IsHorizontal = orientation == Orientation.Horizontal;
            this.strategy.FullyRecycle();
            this.strategy.OnOrientationChanged();
            this.owner.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        internal AddRemoveLayoutResult AddItem(object changedItem, object addRemovedItem, int index)
        {
            return this.strategy.AddItem(changedItem, addRemovedItem, index);
        }

        internal AddRemoveLayoutResult RemoveItem(object changedItem, object addRemovedItem, int index)
        {
            return this.strategy.RemoveItem(changedItem, addRemovedItem, index);
        }

        internal void OnUpdate(UpdateFlags updateFlags)
        {
            if ((updateFlags & UpdateFlags.AffectsContent) == UpdateFlags.AffectsContent)
            {
                this.strategy.FullyRecycle();
            }
        }

        internal IEnumerable<KeyValuePair<int, List<GeneratedItemModel>>> GetDisplayedElements()
        {
            return this.strategy.GetDisplayedElements();
        }

        internal void ScheduleCleanUp()
        {
            if ((this.owner.ItemAnimationMode & ItemAnimationMode.PlayOnSourceReset) == ItemAnimationMode.PlayOnSourceReset && this.owner.ItemRemovedAnimation != null)
            {
                this.HandleNullSourceAnimations();
            }
            else
            {
                this.CleanUp();
            }
        }

        internal void CleanUp()
        {
            this.strategy.SetSource(null, 0, false);
            this.owner.UpdateService.RegisterUpdate((int)ListView.UpdateFlags.AllButData);
        }

        private void LayoutDefinition_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (sender as LayoutDefinitionBase).UpdateStrategy(this.strategy);
            this.strategy.FullyRecycle();
            this.owner.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }
    }
}