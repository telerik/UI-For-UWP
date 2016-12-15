using System;
using System.Collections.Generic;
using System.Globalization;
using Telerik.Core;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Encapsulates the localization logic for controls within the <see cref="Telerik.UI.Xaml.Controls.Chart"/> assembly.
    /// </summary>
    public sealed class ChartLocalizationManager : LocalizationManager
    {
        private static ChartLocalizationManager instance = new ChartLocalizationManager();

        private ChartLocalizationManager()
        {
#if WINDOWS_PHONE_APP || WINDOWS_APP
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Chart/Neutral");
#else
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Chart.UWP/Neutral");
#endif
        }

        /// <summary>
        /// Gets the only instance of the <see cref="ChartLocalizationManager"/> class.
        /// </summary>
        public static ChartLocalizationManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
