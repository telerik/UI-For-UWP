using Telerik.Core;
using Windows.ApplicationModel.Resources.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents the localization manager used for components defined in this assembly.
    /// </summary>
    public class PrimitivesLocalizationManager : LocalizationManager
    {
        /// <summary>
        /// Defines the key for looking-up the busy indicator content string for the <see cref="RadBusyIndicator"/> control.
        /// </summary>
        public static readonly string BusyIndicatorContentKey = "BusyIndicatorContent";

        private static readonly PrimitivesLocalizationManager InstanceField = new PrimitivesLocalizationManager();

        private PrimitivesLocalizationManager()
            : base()
        {
#if WINDOWS_PHONE_APP || WINDOWS_APP
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Primitives/Neutral");
#else
            this.DefaultResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Telerik.UI.Xaml.Primitives.UWP/Neutral");
#endif
        }

        /// <summary>
        /// Gets the only instance of the <see cref="PrimitivesLocalizationManager"/> class.
        /// </summary>
        /// <value>The instance.</value>
        public static PrimitivesLocalizationManager Instance
        {
            get
            {
                return InstanceField;
            }
        }

        /// <summary>
        /// Gets the content string displayed next to the <see cref="RadBusyIndicator"/> animation.
        /// </summary>
        public string BusyIndicatorContentString
        {
            get
            {
                return this.GetString(BusyIndicatorContentKey);
            }
        }
    }
}
