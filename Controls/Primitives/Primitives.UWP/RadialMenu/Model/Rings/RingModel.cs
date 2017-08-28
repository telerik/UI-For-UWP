using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class RingModel : RingModelBase
    {
        private ElementLayerBase<RadialSegment> layer;

        public RingModel(Layout layout, ElementLayerBase<RadialSegment> layer)
            : base(layout)
        {
            this.layer = layer;

            // TODO find better wayt to pass the ringmodel to the layer.
            this.layer.Model = this;
        }

        public virtual LayerBase Layer
        {
            get
            {
                return this.layer;
            }
        }

        /// <summary>
        /// Gets or sets the segments. Exposed for testing purposes.
        /// </summary>
        internal IEnumerable<RadialSegment> Segments { get; set; }

        public override RadialSegment HitTest(RadPolarPoint point)
        {
            foreach (var item in this.Segments)
            {
                var layoutSlot = item.LayoutSlot;

                bool insideAngleRange = false;

                // TODO: consider refactor this.
                if (layoutSlot.StartAngle > 270 && layoutSlot.StartAngle + layoutSlot.SweepAngle > 360)
                {
                    var startAng = layoutSlot.StartAngle > 270 ? layoutSlot.StartAngle - 360 : layoutSlot.StartAngle;
                    var endAng = (layoutSlot.StartAngle + layoutSlot.SweepAngle) % 360;
                    var currentAngle = point.Angle > 270 ? point.Angle - 360 : point.Angle;

                    insideAngleRange = startAng <= currentAngle && currentAngle <= endAng;
                }
                else
                {
                    insideAngleRange = layoutSlot.StartAngle <= point.Angle &&
                                        layoutSlot.StartAngle + layoutSlot.SweepAngle >= point.Angle;
                }

                if (layoutSlot.InnerRadius <= point.Radius &&
                    layoutSlot.OuterRadius >= point.Radius &&
                    insideAngleRange)
                {
                    return item;
                }
            }

            return null;
        }

        internal virtual void UpdateItems()
        {
            if (this.Segments != null)
            {
                foreach (RadialSegment menuItem in this.Segments)
                {
                    this.UpdateItem(menuItem);
                }
            }
        }

        internal RadialSegment GetSegmentByItem(RadialMenuItem item)
        {
            if (this.Segments == null)
            {
                return null;
            }

            return this.Segments.FirstOrDefault(c => c.TargetItem == item);
        }

        internal virtual void UpdateItem(RadialSegment menuItem)
        {
            menuItem.LayoutSlot = this.Layout.GetLayoutSlotAtPosition(this, menuItem);
            this.layer.UpdateVisual(menuItem, this.Layout.StartAngle);
        }

        internal virtual void Prepare(IEnumerable<RadialMenuItem> menuItems)
        {
            this.BuildSegments(menuItems);

            foreach (var menuItem in this.Segments)
            {
                this.PrepareItem(menuItem);
            }
        }

        internal virtual void ClearItems()
        {
            if (this.Segments != null)
            {
                foreach (RadialSegment menuItem in this.Segments)
                {
                    this.ClearItem(menuItem);
                }
            }
        }

        internal virtual void ClearItem(RadialSegment menuItem)
        {
            this.layer.ClearVisual(menuItem);
        }

        internal virtual ActionBase GetShowAction(IEnumerable<RadialMenuItem> menuItems)
        {
            if (this.IsVisible)
            {
                return null;
            }

            var prepareAction = new DelegateAction(() =>
            {
                if (!this.IsVisible)
                {
                    this.Prepare(menuItems);
                    this.IsVisible = true;
                }
            });

            var action = this.Layer.GetShowAction();

            var delegateAction = new DelegateAction(
                () =>
                {
                    foreach (var item in this.Segments)
                    {
                        // TODO: Find better a way, so that we do not work with visuals in the model.
                        var menuVisual = item.Visual as RadialMenuItemControl;

                        if (menuVisual != null)
                        {
                            menuVisual.Loading = false;
                        }
                    }
                });
            return new CompositeAction(prepareAction, action, delegateAction);
        }

        internal virtual ActionBase GetHideAction()
        {
            if (!this.IsVisible)
            {
                return null;
            }

            var hideAction = this.Layer.GetHideAction();
            var delegateAction = new DelegateAction(
                () =>
                {
                    this.ClearItems();
                    this.IsVisible = false;
                });

            return new CompositeAction(hideAction, delegateAction);
        }

        internal virtual ActionBase GetNavigateFromAction()
        {
            if (!this.IsVisible)
            {
                return null;
            }

            this.IsVisible = false;

            var navigateAction = this.Layer.GetNavigateFromAction();
            var delegateAction = new DelegateAction(() => this.ClearItems());

            return new CompositeAction(navigateAction, delegateAction);
        }

        internal virtual ActionBase GetNavigateToAction(IEnumerable<RadialMenuItem> menuItems)
        {
            if (this.IsVisible)
            {
                return null;
            }

            var delegateAction = new DelegateAction(() =>
            {
                this.Prepare(menuItems);

                foreach (var item in this.Segments)
                {
                    // TODO: Find better a way, so that we do not work with visuals in the model.
                    var menuVisual = item.Visual as RadialMenuItemControl;

                    if (menuVisual != null)
                    {
                        menuVisual.Loading = false;
                    }
                }

                this.IsVisible = true;
            });

            return new CompositeAction(delegateAction, this.Layer.GetNavigateToAction());
        }

        protected virtual void BuildSegments(IEnumerable<RadialMenuItem> menuItems)
        {
            this.Segments = menuItems.Select(c => new RadialSegment { TargetItem = c }).ToList();
        }

        protected virtual void PrepareItem(RadialSegment menuItem)
        {
            menuItem.LayoutSlot = this.Layout.GetLayoutSlotAtPosition(this, menuItem);
            this.layer.ShowVisual(menuItem, this.Layout.StartAngle);
        }
    }
}
