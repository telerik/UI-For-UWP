using System;
using Telerik.Core;
using Windows.ApplicationModel.Resources.Core;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Encapsulates the localization logic for controls within the <see cref="Telerik.UI.Xaml.Controls.Input"/> assembly.
    /// </summary>
    public sealed class InputLocalizationManager : LocalizationManager
    {
        private static InputLocalizationManager instance = new InputLocalizationManager();

        private InputLocalizationManager()
        {
#if WINDOWS_PHONE_APP || WINDOWS_APP
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Input/Neutral");
#else
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Input.UWP/Neutral");
#endif
        }

        /// <summary>
        /// Gets the only instance of the <see cref="InputLocalizationManager"/> class.
        /// </summary>
        public static InputLocalizationManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
