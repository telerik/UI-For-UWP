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

        /////// <summary>
        /////// Defines the key for looking-up the empty content string for the <see cref="RadDataBoundListBox"/> control.
        /////// </summary>
        ////public static readonly string DataBoundListBoxEmptyContentKey = "ListBoxEmptyContent";

        /////// <summary>
        /////// Defines the key for looking-up the list controls' pull-to-refresh time indicator string.
        /////// </summary>
        ////public static readonly string ListPullToRefreshTimeStringKey = "ListPullToRefreshTime";

        /////// <summary>
        /////// Defines the key for looking-up the list controls' pull-to-refresh loading string.
        /////// </summary>
        ////public static readonly string ListPullToRefreshLoadingStringKey = "ListPullToRefreshLoading";

        /////// <summary>
        /////// Defines the key for looking-up the list controls' pull-to-refresh normal state string.
        /////// </summary>
        ////public static readonly string ListPullToRefreshStringKey = "ListPullToRefresh";

        private static readonly PrimitivesLocalizationManager InstanceField = new PrimitivesLocalizationManager();

        private PrimitivesLocalizationManager()
            : base()
        {
            // TODO: LOCALIZATION
            // this.DefaultResourceManager = Windows.UI.Xaml.Resources.ResourceManager;
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

        /////// <summary>
        /////// Gets the default empty content for the <see cref="RadDataBoundListBox"/> control.
        /////// </summary>
        ////public string DataBoundListBoxEmptyContentString
        ////{
        ////    get
        ////    {
        ////        return this.GetString(DataBoundListBoxEmptyContentKey);
        ////    }
        ////}

        /////// <summary>
        /////// Gets the string displayed in a list control's pull-to-refresh time indicator when in refreshing state.
        /////// </summary>
        ////public string ListPullToRefreshTimeString
        ////{
        ////    get
        ////    {
        ////        return this.GetString(ListPullToRefreshTimeStringKey);
        ////    }
        ////}

        /////// <summary>
        /////// Gets the string displayed in a list control's pull-to-refresh time indicator when in refreshing state.
        /////// </summary>
        ////public string ListPullToRefreshLoadingString
        ////{
        ////    get
        ////    {
        ////        return this.GetString(ListPullToRefreshLoadingStringKey);
        ////    }
        ////}

        /////// <summary>
        /////// Gets the string displayed in a list control's pull-to-refresh time indicator when in normal state.
        /////// </summary>
        ////public string ListPullToRefreshString
        ////{
        ////    get
        ////    {
        ////        return this.GetString(ListPullToRefreshStringKey);
        ////    }
        ////}
    }
}
