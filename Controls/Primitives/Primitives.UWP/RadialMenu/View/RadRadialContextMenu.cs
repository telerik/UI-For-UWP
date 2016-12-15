using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a class that is used to attached <see cref="RadRadialMenu"/> to a <see cref="FrameworkElement"/>.
    /// </summary>
    public static class RadRadialContextMenu
    {
        /// <summary>
        /// Identifies the <see cref="Menu"/> attached property.
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.RegisterAttached("Menu", typeof(RadRadialMenu), typeof(RadRadialContextMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Behavior attached property.
        /// </summary>
        public static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached("Behavior", typeof(RadialMenuTriggerBehavior), typeof(RadRadialContextMenu), new PropertyMetadata(null));

        /// <summary>
        /// Returns the instance of current <see cref="RadRadialMenu"/> that is attached to specific <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="obj">The target <see cref="FrameworkElement"/>.</param>
        /// <returns>The attached <see cref="RadRadialMenu"/> control.</returns>
        public static RadRadialMenu GetMenu(DependencyObject obj)
        {
            if (obj != null)
            {
                return (RadRadialMenu)obj.GetValue(MenuProperty);
            }

            return null;
        }

        /// <summary>
        /// Attaches an instance of <see cref="RadRadialMenu"/> to a specific <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="obj">The target <see cref="FrameworkElement"/>.</param>
        /// <param name="value">The <see cref="RadRadialMenu"/> instance to be attached to the target element.</param>
        public static void SetMenu(DependencyObject obj, RadRadialMenu value)
        {
            if (obj != null)
            {
                obj.SetValue(MenuProperty, value);
            }
        }

        /// <summary>
        /// Returns the instance of current <see cref="RadialMenuTriggerBehavior"/> that is attached to specific <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="obj">The target <see cref="FrameworkElement"/>.</param>
        /// <returns><see cref="RadialMenuTriggerBehavior"/> instance.</returns>
        public static RadialMenuTriggerBehavior GetBehavior(DependencyObject obj)
        {
            if (obj != null)
            {
                return (RadialMenuTriggerBehavior)obj.GetValue(BehaviorProperty);
            }

            return null;
        }

        /// <summary>
        /// Attaches an instance of <see cref="RadialMenuTriggerBehavior"/> to a specific <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="obj">The target <see cref="FrameworkElement"/>.</param>
        /// <param name="value">The <see cref="RadialMenuTriggerBehavior"/> instance to be attached to the target element.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetBehavior(DependencyObject obj, RadialMenuTriggerBehavior value)
        {
            var oldBehavior = RadRadialContextMenu.GetBehavior(obj);
            var element = obj as FrameworkElement;
            if (oldBehavior != null && element != null)
            {
                oldBehavior.Owner = null;
            }

            if (obj != null)
            {
                obj.SetValue(BehaviorProperty, value);
            }

            if (value != null && element != null)
            {
                value.Owner = element;
            }
        }
    }
}