using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Behavior class that is used to control the triggers caused by the target element will attach and display the menu.
    /// </summary>
    [Bindable]
    public class RadialMenuTriggerBehavior : AttachableObject<FrameworkElement>
    {
        /// <summary>
        /// Identifies the <see cref="AttachTriggers"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty AttachTriggersProperty =
            DependencyProperty.Register(nameof(AttachTriggers), typeof(RadialMenuAttachTriggers), typeof(RadialMenuTriggerBehavior), new PropertyMetadata(RadialMenuAttachTriggers.PressedOrFocused));

        /// <summary>
        /// Gets or sets the attach triggers that will attach and display the menu.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;TextBlock Text="Some Text"&gt;
        ///     &lt;telerikPrimitives:RadRadialContextMenu.Behavior&gt;
        ///         &lt;telerikPrimitives:RadialMenuTriggerBehavior AttachTriggers="PointerOver"/&gt;
        ///     &lt;/telerikPrimitives:RadRadialContextMenu.Behavior&gt;
        ///     &lt;telerikPrimitives:RadRadialContextMenu.Menu&gt;
        ///         &lt;telerikPrimitives:RadRadialMenu/&gt;
        ///     &lt;/telerikPrimitives:RadRadialContextMenu.Menu&gt;
        /// &lt;/TextBlock&gt;
        /// </code>
        /// </example>
        public RadialMenuAttachTriggers AttachTriggers
        {
            get
            {
                return (RadialMenuAttachTriggers)this.GetValue(AttachTriggersProperty);
            }
            set
            {
                this.SetValue(AttachTriggersProperty, value);
            }
        }

        /// <summary>
        /// Called when <see cref="RadRadialMenu"/> get its target element. It is used to unsubscribe the event actions that open the <see cref="RadRadialMenu"/> control.
        /// </summary>
        protected internal virtual void UnsubscribeFromTargetEvents(FrameworkElement element)
        {
            if (element != null)
            {
                if (this.AttachTriggers.HasFlag(RadialMenuAttachTriggers.Focused))
                {
                    element.GotFocus -= this.OnElement_GotFocus;
                }

                if (this.AttachTriggers.HasFlag(RadialMenuAttachTriggers.PointerOver))
                {
                    element.PointerEntered -= this.OnElement_PointerEntered;
                }

                if (this.AttachTriggers.HasFlag(RadialMenuAttachTriggers.PointerPressed))
                {
                    element.PointerPressed -= this.OnTarget_PointerPressed;
                }
            }
        }

        /// <summary>
        /// Called when <see cref="RadRadialMenu"/> get its target element. It is used to subscribe the desired event action that will open the <see cref="RadRadialMenu"/> control.
        /// </summary>
        protected internal virtual void SubscribeToTargetEvents(FrameworkElement element)
        {
            if (element != null)
            {
                if (this.AttachTriggers.HasFlag(RadialMenuAttachTriggers.Focused))
                {
                    element.GotFocus += this.OnElement_GotFocus;
                }

                if (this.AttachTriggers.HasFlag(RadialMenuAttachTriggers.PointerOver))
                {
                    element.PointerEntered += this.OnElement_PointerEntered;
                }

                if (this.AttachTriggers.HasFlag(RadialMenuAttachTriggers.PointerPressed))
                {
                    element.PointerPressed += this.OnTarget_PointerPressed;
                }
            }
        }

        /// <summary>
        /// It is used to position the <see cref="RadRadialMenu"/> and initiate menu position logic.
        /// </summary>
        protected internal virtual void AttachToTargetElement()
        {
            if (this.Owner == null)
            {
                return;
            }

            var menu = RadRadialContextMenu.GetMenu(this.Owner);

            if (menu != null && (PopupService.CurrentAttachedMenu != menu ||
                (PopupService.CurrentAttachedMenu != null && PopupService.CurrentAttachedMenu.TargetElement != this.Owner)))
            {
                menu.model.actionService.PushAction(
                    new DelegateAction(() =>
                    {
                        if (menu.TargetElement != null)
                        {
                            var behavior = RadRadialContextMenu.GetBehavior(menu.TargetElement);

                            if (behavior != null)
                            {
                                behavior.DetachFromTargetElement();
                            }
                            else
                            {
                                this.DetachFromTargetElement();
                            }
                        }

                        menu.TargetElement = this.Owner;

                        PopupService.Attach(menu);

                        if (menu.DesiredSize.Height == 0 || menu.DesiredSize.Width == 0)
                        {
                            menu.LayoutUpdated += this.Owner_LayoutUpdated;
                        }
                        else
                        {
                            this.PositionMenu(menu);                        
                        }
                    }));
            }
        }       

        /// <summary>
        /// It is used to clear the <see cref="RadRadialMenu"/> from the visual scene and detach it from the target element.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        protected internal virtual void DetachFromTargetElement()
        {
            var menu = RadRadialContextMenu.GetMenu(this.Owner);

            if (menu != null)
            {
                menu.TargetElement = null;
            }

            PopupService.Detach();
        }

        /// <summary>
        /// Gets the desired menu position in global coordinates.
        /// </summary>
        /// <param name="menu">The context menu.</param>
        protected virtual Point GetMenuPosition(RadRadialMenu menu)
        {
            if (menu != null)
            {
                return menu.GetPositionPoint();
            }
            return new Point();
        }

        /// <summary>
        /// It is used to detach the previous <see cref="RadRadialMenu"/> owner control.
        /// </summary>
        protected override void OnDetached(FrameworkElement previousOwner)
        {
            if (previousOwner != null)
            {
                this.UnsubscribeFromTargetEvents(previousOwner);

                var menu = RadRadialContextMenu.GetMenu(previousOwner);

                if (menu != null)
                {
                    menu.LayoutUpdated -= this.Owner_LayoutUpdated;
                }

                base.OnDetached(previousOwner);
            }
        }

        /// <summary>
        /// Performs the core logic behind the Attach routine. Allows inheritors to provide additional implementation.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.SubscribeToTargetEvents(this.Owner);
        }

        private void OnElement_GotFocus(object sender, RoutedEventArgs e)
        {
            this.AttachToTargetElement();
        }

        private void OnElement_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.AttachToTargetElement();
        }

        private void OnTarget_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.AttachToTargetElement();
        }

        private void Owner_LayoutUpdated(object sender, object e)
        {
            var menu = RadRadialContextMenu.GetMenu(this.Owner);

            if (menu != null)
            {
                menu.LayoutUpdated -= this.Owner_LayoutUpdated;
                this.PositionMenu(menu);
            }
        }

        private void PositionMenu(RadRadialMenu menu)
        {
            var point = this.GetMenuPosition(menu);
            PopupService.PositionMenu(menu, point);
        }
    }
}
