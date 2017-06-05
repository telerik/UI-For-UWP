using System.Collections;
using System.Collections.Generic;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Model
{
    internal partial class LayoutController
    {
        internal List<object> scheduledItemsForAnimation = new List<object>();
        internal List<GeneratedItemModel> animatingModels = new List<GeneratedItemModel>();

        private Dictionary<RadAnimation, FrameworkElement> scheduledAnimations = new Dictionary<RadAnimation, FrameworkElement>();
        private List<GeneratedItemModel> animatingItemsForRecycle = new List<GeneratedItemModel>();

        internal void StopAnimations()
        {
            this.owner.AnimatingService.StopAnimations();
        }

        internal void HandleNullSourceAnimations()
        {
            this.owner.AnimatingService.PlayNullSourceAnimation((item) =>
                {
                    this.CleanUp();
                });
        }

        internal void HandleSourceResetAnimations()
        {
            this.owner.AnimatingService.PlaySourceResetAnimation((item) =>
                {
                    this.model.ProcessPendingCollectionChange();
                });
        }

        internal void HandleItemRemoved(IList changedItems)
        {
            this.owner.AnimatingService.PlayItemRemovedAnimation(
                changedItems,
                (item) =>
                {
                    if (item != null)
                    {
                        this.strategy.RecycleAnimatedDecorator((GeneratedItemModel)item);
                    }

                    this.model.ProcessPendingCollectionChange();
                });
        }

        private void HandleNewSourceAnimations()
        {
            if ((this.owner.ItemAnimationMode & ItemAnimationMode.PlayOnNewSource) == ItemAnimationMode.PlayOnNewSource)
            {
                if (this.owner.ItemAddedAnimation != null)
                {
                    this.owner.AnimatingService.PlayNewSourceAnimation((item) =>
                        {
                        });
                }
            }
        }

        private void HandleItemAddedAnimations()
        {
            if ((this.owner.ItemAnimationMode & ItemAnimationMode.PlayOnAdd) == ItemAnimationMode.PlayOnAdd && this.owner.ItemAddedAnimation != null)
            {
                this.owner.AnimatingService.PlayItemAddedAnimations((item) =>
                    {
                        this.strategy.RecycleAnimatedDecorator((GeneratedItemModel)item);
                    });
            }
        }
    }
}