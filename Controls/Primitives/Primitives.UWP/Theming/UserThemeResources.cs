using System;
using Windows.UI.Xaml.Resources;

namespace Telerik.UI.Xaml.Controls
{
    /// <summary>
    /// Defines an entry point allowing user-specified resources to replace the built-in theme resources.
    /// </summary>
    public sealed class UserThemeResources
    {
        internal const string DarkResourcesPathKeyName = "DarkResourcesPath";
        internal const string LightResourcesPathKeyName = "LightResourcesPath";
        internal const string HighContrastResourcesPathKeyName = "HighContrastResourcesPath";

        private static string lightResourcesPath;
        private static string darkResourcesPath;
        private static string highContrastResourcesPath;

        /// <summary>
        /// Initializes static members of the <see cref="UserThemeResources"/> class.
        /// </summary>
        static UserThemeResources()
        {
            EnsureCustomXamlResourceLoader();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserThemeResources"/> class.
        /// </summary>
        public UserThemeResources()
        {
            EnsureCustomXamlResourceLoader();
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> path to the resource dictionary containing theme resource definitions for the Dark theme.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string DarkResourcesPath
        {
            get
            {
                return darkResourcesPath;
            }
            set
            {
                darkResourcesPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> path to the resource dictionary containing theme resource definitions for the Light theme.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string LightResourcesPath
        {
            get
            {
                return lightResourcesPath;
            }
            set
            {
                lightResourcesPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> path to the resource dictionary containing theme resource definitions for the HighContrast theme.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string HighContrastResourcesPath
        {
            get
            {
                return highContrastResourcesPath;
            }
            set
            {
                highContrastResourcesPath = value;
            }
        }

        internal static Uri GetUriByPath(string resourceId)
        {
            if (resourceId == LightResourcesPathKeyName)
            {
                if (!string.IsNullOrEmpty(lightResourcesPath))
                {
                    return new Uri(lightResourcesPath);
                }
            }
            else if (resourceId == DarkResourcesPathKeyName)
            {
                if (!string.IsNullOrEmpty(darkResourcesPath))
                {
                    return new Uri(darkResourcesPath);
                }
            }
            else if (resourceId == HighContrastResourcesPathKeyName)
            {
                if (!string.IsNullOrEmpty(highContrastResourcesPath))
                {
                    return new Uri(highContrastResourcesPath);
                }
            }

            return null;
        }

        private static void EnsureCustomXamlResourceLoader()
        {
            if (CustomXamlResourceLoader.Current == null)
            {
                CustomXamlResourceLoader.Current = new UserThemeResourceLoader();
            }
        }
    }
}
