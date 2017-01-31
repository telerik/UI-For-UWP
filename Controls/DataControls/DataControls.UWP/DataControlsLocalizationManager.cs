using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Core;
using Windows.ApplicationModel.Resources.Core;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Encapsulates the localization logic for controls within the <see cref="Telerik.UI.Xaml.Controls.Data"/> assembly.
    /// </summary>
    public sealed class DataControlsLocalizationManager : LocalizationManager
    {
        private static DataControlsLocalizationManager instance = new DataControlsLocalizationManager();

        private DataControlsLocalizationManager()
        {
#if WINDOWS_PHONE_APP || WINDOWS_APP
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Controls.Data/Neutral");
#else
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Controls.Data.UWP/Neutral");
#endif
        }

        /// <summary>
        /// Gets the only instance of the <see cref="DataControlsLocalizationManager"/> class.
        /// </summary>
        public static DataControlsLocalizationManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
