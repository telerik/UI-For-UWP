using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources.Core;

namespace Telerik.Core
{
    /// <summary>
    /// Represents a singleton class that manages application string resources.
    /// </summary>
    public abstract class LocalizationManager
    {
        private static ResourceMap globalResourceMap;

        private ResourceMap defaultResourceMap;
        private ResourceMap userResourceMap;
        private IStringResourceLoader stringLoader;

        /// <summary>
        /// Gets or sets the <see cref="ResourceMap"/> instance that contains localized versions for all keys in each Telerik assembly.
        /// </summary>
        public static ResourceMap GlobalResourceMap
        {
            get
            {
                return globalResourceMap;
            }
            set
            {
                globalResourceMap = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ResourceMap"/> instance provided by the associated assembly.
        /// </summary>
        public ResourceMap DefaultResourceMap
        {
            get
            {
                return this.defaultResourceMap;
            }
            internal set
            {
                this.defaultResourceMap = value;
            }
        }

        /// <summary>
        /// Gets or sets the user-defined <see cref="ResourceMap"/> instance used to look-up localizable resources within the associated assembly.
        /// </summary>
        public ResourceMap UserResourceMap
        {
            get
            {
                return this.userResourceMap;
            }
            set
            {
                this.userResourceMap = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IStringResourceLoader"/> instance used to provide code-based localization per assembly.
        /// </summary>
        public IStringResourceLoader StringLoader
        {
            get
            {
                return this.stringLoader;
            }
            set
            {
                this.stringLoader = value;
            }
        }

        /// <summary>
        /// Gets the localized version of the string, associated with the specified key. 
        /// </summary>
        public string GetString(string key)
        {
            string localizedString;
            ResourceCandidate match;

            // 1. StringLoader is with highest priority
            if (this.stringLoader != null)
            {
                localizedString = this.stringLoader.GetString(key);
                if (!string.IsNullOrEmpty(localizedString))
                {
                    return localizedString;
                }
            }

            // 2. Local resource map is second.
            if (this.userResourceMap != null)
            {
                match = this.userResourceMap.GetValue(key, ResourceContext.GetForCurrentView());
                if (match != null)
                {
                    localizedString = match.ValueAsString;
                    if (!string.IsNullOrEmpty(localizedString))
                    {
                        return localizedString;
                    }
                }
            }

            // 3. Global ResourceMap is next
            if (globalResourceMap != null)
            {
                match = globalResourceMap.GetValue(key, ResourceContext.GetForCurrentView());
                if (match != null)
                {
                    localizedString = match.ValueAsString;
                    if (!string.IsNullOrEmpty(localizedString))
                    {
                        return localizedString;
                    }
                }
            }

            // 4. The default ResourceMap is last
            if (this.defaultResourceMap != null)
            {
                match = this.defaultResourceMap.GetValue(key, ResourceContext.GetForCurrentView());
                if (match != null)
                {
                    localizedString = match.ValueAsString;
                    if (!string.IsNullOrEmpty(localizedString))
                    {
                        return localizedString;
                    }
                }
            }

            Debug.Assert(false, string.Format("No entry found for key '{0}'.", key));
            return string.Empty;
        }
    }
}