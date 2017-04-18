using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal partial class RadialMenuModel
    {
        internal MenuViewState viewState;
        internal ActionService actionService;

        internal RingModel contentRing;
        internal RingModel navigateRing;
        private Layout layout;
        private RingModel decorationItemsRing;
        private RingModel backgroundSectorItemsRing;
        private RadRadialMenu owner;
        private bool layersInitialized;

        public RadialMenuModel(RadRadialMenu owner)
        {
            this.layout = new FixedSegmentLayout(owner.startAngleCache);
            this.viewState = new MenuViewState();
            this.MenuItems = new MenuItemCollection<RadialMenuItem>() { Owner = this };

            this.owner = owner;

            this.actionService = new ActionService();

            this.contentRing = new RingModel(this.layout, new MenuItemLayer());
            this.decorationItemsRing = new RingModel(this.layout, new ItemVisualStateLayer()) { HitTestVisible = false };
            this.backgroundSectorItemsRing = new RingModel(this.layout, new BackgroundSectorItemLayer()) { HitTestVisible = false };
            this.navigateRing = new NavidateItemRingModel(this.layout);
            this.UpdateStartAngle(owner.startAngleCache);
        }

        public ObservableCollection<RadialMenuItem> MenuItems
        {
            get;
            private set;
        }

        internal Layout Layout
        {
            get
            {
                return this.layout;
            }

            set
            {
                this.layout = value;
            }
        }

        internal RadRadialMenu Owner
        {
            get
            {
                return this.owner;
            }
        }

        public void ShowView()
        {
            this.UpdateRingsRadius();

            IList<RadialMenuItem> items = this.MenuItems;

            var contentAction = this.contentRing.GetShowAction(items);
            var navigateAction = this.navigateRing.GetShowAction(items);
            var selectionItemsAction = this.decorationItemsRing.GetShowAction(items);
            var backgroundSectorItemsAction = this.backgroundSectorItemsRing.GetShowAction(items);

            if (contentAction == null || navigateAction == null || selectionItemsAction == null || backgroundSectorItemsAction == null)
            {
                return;
            }

            contentAction.IsDependant = false;
            navigateAction.IsDependant = false;
            selectionItemsAction.IsDependant = false;
            backgroundSectorItemsAction.IsDependant = false;

            this.actionService.PushAction(new CompositeAction(contentAction, navigateAction, selectionItemsAction, backgroundSectorItemsAction));
        }

        public void HideView()
        {
            List<ActionBase> actionsToExecute = new List<ActionBase>();

            var contentAction = this.contentRing.GetHideAction();
            if (contentAction != null)
            {
                contentAction.IsDependant = false;
                actionsToExecute.Add(contentAction);
            }

            var navigateAction = this.navigateRing.GetHideAction();
            if (navigateAction != null)
            {
                navigateAction.IsDependant = false;
                actionsToExecute.Add(navigateAction);
            }

            var backgroundSectorItemsAction = this.backgroundSectorItemsRing.GetHideAction();
            if (backgroundSectorItemsAction != null)
            {
                backgroundSectorItemsAction.IsDependant = false;
                actionsToExecute.Add(backgroundSectorItemsAction);
            }

            var selectedItemsAction = this.decorationItemsRing.GetHideAction();
            if (selectedItemsAction != null)
            {
                selectedItemsAction.IsDependant = false;
                actionsToExecute.Add(selectedItemsAction);
            }

            if (actionsToExecute.Count > 0)
            {
                this.actionService.PushAction(new CompositeAction(actionsToExecute));
            }
        }

        internal void NavigateToView(NavigateContext context, bool navigateBack = false)
        {
            var targetMenuItem = context.MenuItemTarget;

            if (!navigateBack)
            {
                this.viewState.MenuLevels.Push(targetMenuItem);
                this.viewState.StartAngleLevels.Add(context.StartAngle);
            }

            this.NavigateFromView();

            IList<RadialMenuItem> items = targetMenuItem == null ? this.MenuItems : targetMenuItem.ChildItems;

            List<ActionBase> actionsToExecute = new List<ActionBase>();

            actionsToExecute.Add(new DelegateAction(() =>
            {
                this.Layout.StartAngle = context.StartAngle;
                this.UpdateRingsRadius();
            }));

            var contentAction = this.contentRing.GetNavigateToAction(items);
            if (contentAction != null)
            {
                contentAction.IsDependant = false;
                actionsToExecute.Add(contentAction);
            }

            var navigateAction = this.navigateRing.GetNavigateToAction(items);
            if (navigateAction != null)
            {
                navigateAction.IsDependant = false;
                actionsToExecute.Add(navigateAction);
            }

            var backgroundSectorImtesAction = this.backgroundSectorItemsRing.GetNavigateToAction(items);
            if (backgroundSectorImtesAction != null)
            {
                backgroundSectorImtesAction.IsDependant = false;
                actionsToExecute.Add(backgroundSectorImtesAction);
            }

            var selectedItemsAction = this.decorationItemsRing.GetNavigateToAction(items);
            if (selectedItemsAction != null)
            {
                selectedItemsAction.IsDependant = false;
                actionsToExecute.Add(selectedItemsAction);
            }

            if (actionsToExecute.Count > 0)
            {
                this.actionService.PushAction(new CompositeAction(actionsToExecute));
            }
        }

        internal void UpdateStartAngle(double rootStartAngle)
        {
            if (this.viewState.StartAngleLevels.Count == 0)
            {
                this.viewState.StartAngleLevels.Add(rootStartAngle);
            }
            else
            {
                if (this.viewState.MenuLevels.Count == 0)
                {
                    this.viewState.StartAngleLevels[0] = rootStartAngle;
                }
            }
        }

        internal void UpdateRingsRadius()
        {
            if (!this.owner.IsTemplateApplied)
            {
                return;
            }

            this.contentRing.InnerRadius = this.owner.innerRadiusCache;
            this.contentRing.OuterRadius = this.owner.innerNavigationRadiusCache;
            this.navigateRing.InnerRadius = this.owner.innerNavigationRadiusCache;
            this.navigateRing.OuterRadius = this.owner.outerRadiusCache;

            this.decorationItemsRing.InnerRadius = this.contentRing.InnerRadius;
            this.decorationItemsRing.OuterRadius = this.contentRing.OuterRadius;

            this.backgroundSectorItemsRing.InnerRadius = this.contentRing.InnerRadius;
            this.backgroundSectorItemsRing.OuterRadius = this.contentRing.OuterRadius;

            this.UpdateRingsVisualPanel();
            this.UpdateRingsVisualItems();
        }

        internal void UpdateRingsVisualPanel()
        {
            if (this.layersInitialized)
            {
                this.backgroundSectorItemsRing.Layer.UpdateVisualPanel();
                this.contentRing.Layer.UpdateVisualPanel();
                this.navigateRing.Layer.UpdateVisualPanel();
                this.decorationItemsRing.Layer.UpdateVisualPanel();
            }
        }

        internal void UpdateRingsVisualItems()
        {
            if (this.layersInitialized)
            {
                this.backgroundSectorItemsRing.UpdateItems();
                this.decorationItemsRing.UpdateItems();
                this.contentRing.UpdateItems();
                this.navigateRing.UpdateItems();
                this.decorationItemsRing.UpdateItems();
            }
        }

        internal void InitializeLayers()
        {
            if (!this.layersInitialized)
            {
                this.owner.InitializeLayer(this.contentRing.Layer);
                this.owner.InitializeLayer(this.navigateRing.Layer);
                this.owner.InitializeLayer(this.decorationItemsRing.Layer);
                this.owner.InitializeLayer(this.backgroundSectorItemsRing.Layer);
                this.layersInitialized = true;
            }
        }

        internal void DestroyLayers()
        {
            this.owner.DestroyLayer(this.contentRing.Layer);
            this.owner.DestroyLayer(this.navigateRing.Layer);
        }

        internal RadialSegment GetDecorationSegment(RadialMenuItem menuItem)
        {
            return this.decorationItemsRing.GetSegmentByItem(menuItem);
        }

        internal RadialSegment GetContentSegment(RadialMenuItem menuItem)
        {
            return this.contentRing.GetSegmentByItem(menuItem);
        }

        internal void OnIsEnabledChanged(RadialMenuItem radialMenuItem)
        {
            var decorationSegment = this.decorationItemsRing.GetSegmentByItem(radialMenuItem);

            if (decorationSegment != null)
            {
                this.decorationItemsRing.UpdateItem(decorationSegment);
            }

            var itemSegment = this.contentRing.GetSegmentByItem(radialMenuItem);

            if (itemSegment != null)
            {
                this.contentRing.UpdateItem(itemSegment);
            }
        }

        internal RadialMenuItemContext GetCommandContext()
        {
            return new RadialMenuItemContext { TargetElement = this.owner.TargetElement };
        }

        internal void InvalidateRequerySuggested(ObservableCollection<RadialMenuItem> items)
        {
            foreach (var item in items)
            {
                item.UpdateIsEnabled();
                if (item.HasChildren)
                {
                    this.InvalidateRequerySuggested(item.ChildItems);
                }
            }
        }

        internal void ResetViewState()
        {
            double tempStartAngle = this.viewState.StartAngleLevels.FirstOrDefault();
            this.viewState.StartAngleLevels.Clear();
            this.UpdateStartAngle(tempStartAngle);

            this.viewState.MenuLevels.Clear();
        }

        private void NavigateFromView()
        {
            List<ActionBase> actionsToExecute = new List<ActionBase>();

            var contentAction = this.contentRing.GetNavigateFromAction();
            if (contentAction != null)
            {
                contentAction.IsDependant = false;
                actionsToExecute.Add(contentAction);
            }

            var navigateAction = this.navigateRing.GetNavigateFromAction();
            if (navigateAction != null)
            {
                navigateAction.IsDependant = false;
                actionsToExecute.Add(navigateAction);
            }

            var backgroundSectorItemsAction = this.backgroundSectorItemsRing.GetNavigateFromAction();
            if (backgroundSectorItemsAction != null)
            {
                backgroundSectorItemsAction.IsDependant = false;
                actionsToExecute.Add(backgroundSectorItemsAction);
            }

            var selectedItemsAction = this.decorationItemsRing.GetNavigateFromAction();
            if (selectedItemsAction != null)
            {
                selectedItemsAction.IsDependant = false;
                actionsToExecute.Add(selectedItemsAction);
            }

            if (!this.owner.menuButton.DisplayBackContent)
            {
                var showBackButton = new DelegateAction(() => this.owner.menuButton.TransformToBackButton());
                actionsToExecute.Add(showBackButton);
            }

            if (actionsToExecute.Count > 0)
            {
                this.actionService.PushAction(new CompositeAction(actionsToExecute));
            }
        }
    }
}