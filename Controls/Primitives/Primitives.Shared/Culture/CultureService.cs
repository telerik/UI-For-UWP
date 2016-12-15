using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Exposes notation for culture support on a per DependencyObject instance basis.
    /// </summary>
    public sealed class CultureService
    {
        /// <summary>
        /// Identifies the CultureName attached property.
        /// </summary>
        public static readonly DependencyProperty CultureNameProperty =
            DependencyProperty.RegisterAttached("CultureName", typeof(string), typeof(CultureService), new PropertyMetadata(string.Empty, OnCultureNameChanged));

        /// <summary>
        /// Identifies the Culture attached property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.RegisterAttached("Culture", typeof(CultureInfo), typeof(CultureService), new PropertyMetadata(null, OnCultureChanged));       

        /// <summary>
        /// Prevents a default instance of the <see cref="CultureService" /> class from being created.
        /// </summary>
        private CultureService()
        {
        }

        /// <summary>
        /// Gets the CultureName value for the specified dependency object instance.
        /// </summary>
        public static string GetCultureName(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            return (string)instance.GetValue(CultureNameProperty);
        }

        /// <summary>
        /// Sets the specified CultureName value to the provided dependency object instance.
        /// </summary>
        public static void SetCultureName(DependencyObject instance, string cultureName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            instance.SetValue(CultureNameProperty, cultureName);
        }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> value for the specified dependency object instance.
        /// </summary>
        public static CultureInfo GetCulture(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            return (CultureInfo)instance.GetValue(CultureProperty);
        }

        /// <summary>
        /// Sets the specified <see cref="CultureInfo"/> value to the provided dependency object instance.
        /// </summary>
        public static void SetCulture(DependencyObject instance, CultureInfo value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            instance.SetValue(CultureProperty, value);
        }

        private static void OnCultureNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ICultureAware listener = d as ICultureAware;
            if (listener == null)
            {
                return;
            }

            var oldCultureName = (string)e.OldValue;
            var newCultureName = (string)e.NewValue;

            CultureInfo oldCulture = null;
            CultureInfo newCulture = null;

            try
            {
                if (!string.IsNullOrEmpty(oldCultureName))
                {
                    oldCulture = new CultureInfo(oldCultureName);
                }
                if (!string.IsNullOrEmpty(newCultureName))
                {
                    newCulture = new CultureInfo(newCultureName);
                }
            }
            catch (CultureNotFoundException)
            {
                if (DesignMode.DesignModeEnabled)
                {
                    // Prevent the exception at design-time while the user is typing.
                    return;
                }

                throw;
            }
            catch (ArgumentNullException)
            {
                if (DesignMode.DesignModeEnabled)
                {
                    // Prevent the exception at design-time while the user is typing.
                    return;
                }

                throw;
            }

            listener.OnCultureChanged(oldCulture, newCulture);
        }

        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ICultureAware listener = d as ICultureAware;
            if (listener == null)
            {
                return;
            }

            var oldCulture = (CultureInfo)e.OldValue;
            var newCulture = (CultureInfo)e.NewValue;

            listener.OnCultureChanged(oldCulture, newCulture);
        }
    }
}
