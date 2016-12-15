using System.Reflection;
using Telerik.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Encapsulates the localization logic for controls within the <see cref="Telerik.UI.Xaml.Controls.Grid"/> assembly.
    /// </summary>
    public sealed class GridLocalizationManager : LocalizationManager
    {
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(GridLocalizationManager), new PropertyMetadata(null, OnPropertyNameChanged));

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(GridLocalizationManager), new PropertyMetadata(null, OnKeyChanged));

        private static GridLocalizationManager instance = new GridLocalizationManager();

        private GridLocalizationManager()
        {
#if WINDOWS_PHONE_APP || WINDOWS_APP
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Grid/Neutral");
#else
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Grid.UWP/Neutral");
#endif
        }

        /// <summary>
        /// Gets the only instance of the <see cref="GridLocalizationManager"/> class.
        /// </summary>
        public static GridLocalizationManager Instance
        {
            get
            {
                return instance;
            }
        }

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var key = e.NewValue as string;
            var propertyName = GetPropertyName(d);

            SetPropertyToValue(d, propertyName, key);
        }

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var key = GetKey(d);
            var propertyName = e.NewValue as string;

            SetPropertyToValue(d, propertyName, key);
        }

        private static void SetPropertyToValue(object obj, string propertyName, string key)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(propertyName))
            {
                var property = obj.GetType().GetRuntimeProperty(propertyName);
                if (property != null)
                {
                    property.SetValue(obj, GridLocalizationManager.Instance.GetString(key));
                }
            }
        }

        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }
    }        
}
