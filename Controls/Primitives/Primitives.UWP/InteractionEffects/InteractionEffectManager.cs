using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Implements an infrastructure that allows for applying interaction effects to elements.
    /// </summary>
    public static class InteractionEffectManager
    {
        /// <summary>
        /// Identifies the IsTiltEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInteractionEnabledProperty =
            DependencyProperty.RegisterAttached("IsInteractionEnabled", typeof(bool), typeof(InteractionEffectManager), new PropertyMetadata(false, OnIsInteractionEnabledChanged));

        /// <summary>
        /// Identifies the Interaction dependency property.
        /// </summary>
        public static readonly DependencyProperty InteractionProperty =
            DependencyProperty.RegisterAttached("Interaction", typeof(InteractionEffectBase), typeof(InteractionEffectManager), new PropertyMetadata(new TiltInteractionEffect()));

        /// <summary>
        /// Identifies the ApplyInteractionExplicitly dependency property.
        /// </summary>
        public static readonly DependencyProperty ApplyInteractionExplicitlyProperty =
            DependencyProperty.RegisterAttached("ApplyInteractionExplicitly", typeof(bool), typeof(InteractionEffectManager), new PropertyMetadata(false));

        private static List<Type> allowedTypes = new List<Type>(8);
        private static List<Type> excludedTypes = new List<Type>(4);

        /// <summary>
        /// Gets a collection of types
        /// to which the tilt effect can be applied.
        /// </summary>
        public static List<Type> AllowedTypes
        {
            get
            {
                return allowedTypes;
            }
        }

        /// <summary>
        /// Gets a collection of types
        /// to which the tilt effect cannot be applied.
        /// </summary>
        public static List<Type> ExcludedTypes
        {
            get
            {
                return excludedTypes;
            }
        }

        /// <summary>
        /// Sets a boolean value determining whether an interaction effect will be applied to a given element explicitly
        /// without checking the AllowedTypes and ExcludedTypes values.
        /// </summary>
        public static void SetApplyInteractionExplicitly(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(ApplyInteractionExplicitlyProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether an interaction effect will be applied to a given element explicitly
        /// without checking the AllowedTypes and ExcludedTypes values.
        /// </summary>
        public static bool GetApplyInteractionExplicitly(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool)element.GetValue(ApplyInteractionExplicitlyProperty);
        }

        /// <summary>
        /// Sets a boolean value indicating whether the tilt effect will be enabled for the given element.
        /// </summary>
        public static void SetIsInteractionEnabled(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(IsInteractionEnabledProperty, value);
        }

        /// <summary>
        /// Gets a boolean value indicating whether the tilt effect is enabled for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        public static bool GetIsInteractionEnabled(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool)element.GetValue(IsInteractionEnabledProperty);
        }

        /// <summary>
        /// Sets the interaction effect for a given element..
        /// </summary>
        public static void SetInteraction(DependencyObject element, InteractionEffectBase value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(InteractionProperty, value);
        }

        /// <summary>
        /// Gets the interaction effect assigned to the given element.
        /// </summary>
        public static InteractionEffectBase GetInteraction(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (InteractionEffectBase)element.GetValue(InteractionProperty);
        }

        /// <summary>
        /// Cancels the effect currently applied to the given element.
        /// </summary>
        public static void CancelEffect(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            InteractionEffectBase effect = GetInteraction(element);
            effect.CancelEffect();
        }

        private static bool IsTypeAllowed(Type type)
        {
            foreach (Type t in AllowedTypes)
            {
                if (t.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTypeExcluded(Type type)
        {
            foreach (Type t in ExcludedTypes)
            {
                if (t.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    return true;
                }
            }

            return false;
        }

        private static FrameworkElement GetInteractionElement(FrameworkElement element)
        {
            FrameworkElement parent = element;
            FrameworkElement interactionElement = null;
            do
            {
                if (GetApplyInteractionExplicitly(parent))
                {
                    return parent;
                }

                Type elementType = parent.GetType();

                if (!IsTypeExcluded(elementType) && IsTypeAllowed(elementType))
                {
                    interactionElement = parent;
                    break;
                }

                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            } 
            while (parent != null);

            return interactionElement;
        }

        private static void OnIsInteractionEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UIElement element = sender as UIElement;

            if (element == null)
            {
                return;
            }

            bool isTiltEnabled = (bool)args.NewValue;
            if (isTiltEnabled)
            {
                RegisterInteractionElement(element);
            }
            else
            {
                UnregisterInteractionElement(element);
            }
        }

        private static void OnElementPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            FrameworkElement interactionElement = GetInteractionElement(args.OriginalSource as FrameworkElement);
            if (interactionElement == null)
            {
                return;
            }

            BeginInteractionEffect(interactionElement, args);
        }

        private static void BeginInteractionEffect(FrameworkElement target, PointerRoutedEventArgs args)
        {
            InteractionEffectBase effect = GetInteraction(target);
            if (effect == null)
            {
                return;
            }

            effect.OnPointerDown(target, args);
        }

        private static void RegisterInteractionElement(UIElement element)
        {
            element.PointerPressed += OnElementPointerPressed;
        }

        private static void UnregisterInteractionElement(UIElement element)
        {
            element.PointerPressed -= OnElementPointerPressed;
        }
    }
}
