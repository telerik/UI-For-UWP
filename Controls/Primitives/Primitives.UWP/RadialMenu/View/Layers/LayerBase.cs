using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal abstract class LayerBase
    {
        private RingModelBase model;

        private Panel ownerPanel;

        public RadRadialMenu Owner { get; set; }

        public virtual RingModelBase Model
        {
            get
            {
                return this.model;
            }
            set
            {
                this.model = value;
            }
        }

        protected abstract Storyboard ShowLayerStoryboard { get; }

        protected abstract Storyboard HideLayerStoryboard { get; }

        protected abstract Storyboard NavigateFromStoryboard { get; }

        protected abstract Storyboard NavigateToStoryboard { get; }

        protected abstract Panel Visual { get; }

        public virtual void Arrange(Rect rect)
        {
            this.Visual.Arrange(rect);
        }

        public virtual void AttachToPanel(Panel panel)
        {
            this.ownerPanel = panel;
            this.ownerPanel.Children.Insert(0, this.Visual);
        }

        public virtual void DetachFromPanel(Panel panel)
        {
            if (this.ownerPanel == null)
            {
                return;
            }

            this.ownerPanel.Children.Remove(this.Visual);
            this.ownerPanel = null;
        }

        internal virtual ActionBase GetShowAction()
        {
            var action = new AnimationAction(this.ShowLayerStoryboard);

            var delegateAction = new DelegateAction(() =>
                {
                    if (this.Owner != null)
                    {
                        this.Owner.hitTestService.RegisterArea(this.Model);
                    }
                });

            return new CompositeAction(action, delegateAction);
        }

        internal virtual ActionBase GetHideAction()
        {
            var action = new AnimationAction(this.HideLayerStoryboard);

            var delegateAction = new DelegateAction(() =>
            {
                if (this.Owner != null)
                {
                    this.Owner.hitTestService.UnregisterArea(this.Model);
                }
            });

            return new CompositeAction(action, delegateAction);
        }

        internal virtual ActionBase GetNavigateFromAction()
        {
            var action = new AnimationAction(this.NavigateFromStoryboard);

            var delegateAction = new DelegateAction(() =>
            {
                if (this.Owner != null)
                {
                    this.Owner.hitTestService.UnregisterArea(this.Model);
                }
            });

            return new CompositeAction(action, delegateAction);
        }

        internal virtual ActionBase GetNavigateToAction()
        {
            var action = new AnimationAction(this.NavigateToStoryboard);

            var delegateAction = new DelegateAction(() =>
            {
                if (this.Owner != null)
                {
                    this.Owner.hitTestService.RegisterArea(this.Model);
                }
            });

            return new CompositeAction(action, delegateAction);
        }

        internal abstract void UpdateVisualPanel();
    }
}
