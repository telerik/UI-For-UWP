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
            RadSize resultSize;

            if (this.strategy.IsHorizontal)
            {
                resultSize = this.MeasureHorizontal(newAvailableSize);
            }
            else
            {
                resultSize = this.MeasureVertical(newAvailableSize);
            }

            this.strategy.RecycleAfterMeasure();
            this.model.View.ItemCheckBoxService.GenerateVisuals();
            this.strategy.GenerateFrozenContainers();

            return resultSize;
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
            var offset = this.strategy.IsHorizontal ? rect.Right : rect.Bottom;
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

        private RadSize MeasureHorizontal(RadSize newAvailableSize)
        {
            var measureConstraints = newAvailableSize;

            measureConstraints.Width = double.PositiveInfinity;

            var headerSize = this.MeasureHeader(measureConstraints);
            var footerSize = this.MeasureFooter(measureConstraints);
            var emptyContentSize = this.MeasureEmptyContent(measureConstraints);
            var emptyContentAvailableWidth = newAvailableSize.Width - headerSize.Width - footerSize.Width;

            if (emptyContentSize.Width > 0 && emptyContentSize.Width < emptyContentAvailableWidth)
            {
                measureConstraints.Width = emptyContentAvailableWidth;
                emptyContentSize = this.MeasureEmptyContent(measureConstraints);
            }

            var availableSize = newAvailableSize;
            var startOffset = this.owner.ScrollOffset;

            if (startOffset < headerSize.Width)
            {
                availableSize.Width -= headerSize.Width - startOffset;

                if (availableSize.Width < 0)
                {
                    availableSize.Width = 0;
                }

                startOffset = 0;
            }
            else
            {
                startOffset -= headerSize.Width;
            }

            var resultSize = this.strategy.MeasureContent(availableSize, startOffset, this.model.BufferScale);
            var contentRemainingWidth = availableSize.Width - resultSize.Width + startOffset;

            if (contentRemainingWidth > 0 && footerSize.Width > 0)
            {
                availableSize.Width -= contentRemainingWidth;
                resultSize = this.strategy.MeasureContent(availableSize, startOffset, this.model.BufferScale);
            }

            var emptyContentPosition = new RadSize(this.headerModel.LayoutSlot.Right + resultSize.Width, 0);
            this.UpdateEmptyContentLayoutSlotPosition(emptyContentPosition);

            var footerPosition = new RadSize(this.emptyContentModel.LayoutSlot.Right, 0);
            this.UpdateFooterLayoutSlotPosition(footerPosition);

            resultSize.Width += emptyContentSize.Width + headerSize.Width + footerSize.Width;
            resultSize.Height += emptyContentSize.Height;

            return resultSize;
        }

        private RadSize MeasureVertical(RadSize newAvailableSize)
        {
            var measureConstraints = newAvailableSize;

            measureConstraints.Height = double.PositiveInfinity;

            var headerSize = this.MeasureHeader(measureConstraints);
            var footerSize = this.MeasureFooter(measureConstraints);
            var emptyContentSize = this.MeasureEmptyContent(measureConstraints);
            var emptyContentAvailableHeight = newAvailableSize.Height - headerSize.Height - footerSize.Height;

            if (emptyContentSize.Height > 0 && emptyContentSize.Height < emptyContentAvailableHeight)
            {
                measureConstraints.Height = emptyContentAvailableHeight;
                emptyContentSize = this.MeasureEmptyContent(measureConstraints);
            }

            var availableSize = newAvailableSize;
            var startOffset = this.owner.ScrollOffset;

            if (startOffset < headerSize.Height)
            {
                availableSize.Height -= headerSize.Height - startOffset;

                if (availableSize.Height < 0)
                {
                    availableSize.Height = 0;
                }

                startOffset = 0;
            }
            else
            {
                startOffset -= headerSize.Height;
            }

            var resultSize = this.strategy.MeasureContent(availableSize, startOffset, this.model.BufferScale);
            var contentRemainingHeight = availableSize.Height - resultSize.Height + startOffset;

            if (contentRemainingHeight > 0 && footerSize.Height > 0)
            {
                availableSize.Height -= contentRemainingHeight;
                resultSize = this.strategy.MeasureContent(availableSize, startOffset, this.model.BufferScale);
            }

            var emptyContentPosition = new RadSize(0, this.headerModel.LayoutSlot.Bottom + resultSize.Height);
            this.UpdateEmptyContentLayoutSlotPosition(emptyContentPosition);

            var footerPosition = new RadSize(0, this.emptyContentModel.LayoutSlot.Bottom);
            this.UpdateFooterLayoutSlotPosition(footerPosition);

            resultSize.Width += emptyContentSize.Width;
            resultSize.Height += emptyContentSize.Height + headerSize.Height + footerSize.Height;

            return resultSize;
        }
    }
}