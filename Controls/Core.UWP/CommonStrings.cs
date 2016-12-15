namespace Telerik.Core
{
    internal static class CommonStrings
    {
#if WINDOWS_UWP
        public const string AssemblyProduct = "Telerik UI for UWP";
        public const string AssemblyCommonDescription = " for UWP";
#else
        public const string AssemblyProduct = "Telerik UI for Windows Universal";
        public const string AssemblyCommonDescription = " for Windows Universal applications";
#endif

#if TRIAL
        public const string TrialVersionTitleString = " (Trial Version)";
#else
        public const string TrialVersionTitleString = "";
#endif

#if WINDOWS_APP
		public const string FrameworkTitleString = "Windows Universal Applications";
#elif WINDOWS_PHONE_APP
		public const string FrameworkTitleString = "Windows Phone 8.1 Applications";
#elif WINDOWS_UWP
        public const string FrameworkTitleString = "UWP Applications";
#else
        public const string FrameworkTitleString = "";
#endif
    }
}
