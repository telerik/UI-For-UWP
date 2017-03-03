using Windows.System.Profile;
using Windows.UI.ViewManagement;

// ReSharper disable once CheckNamespace
namespace Telerik.Core
{
    /// <summary>
    /// Helper class to get device type
    /// </summary>
    public static class DeviceTypeHelper
    {
        /// <summary>
        /// Get the device type (XBox, Continuum, Tablet, Desktop, Iot...)
        /// </summary>
        /// <returns></returns>
        public static DeviceType GetDeviceType()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse ? DeviceType.Continuum : DeviceType.Phone;
                case "Windows.Xbox":
                    return DeviceType.Xbox;
                case "Windows.Desktop":
                    return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse ? DeviceType.Desktop : DeviceType.Tablet;
                case "Windows.Universal":
                    return DeviceType.IoT;
                case "Windows.Team":
                    return DeviceType.SurfaceHub;
                default:
                    return DeviceType.Other;
            }
        }
    }

    /// <summary>
    /// Device type
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Phone
        /// </summary>
        Phone,
        /// <summary>
        /// Desktop
        /// </summary>
        Desktop,
        /// <summary>
        /// Tablet
        /// </summary>
        Tablet,
        /// <summary>
        /// IOT
        /// </summary>
        IoT,
        /// <summary>
        /// Xbox
        /// </summary>
        Xbox,
        /// <summary>
        /// SurfaceHub
        /// </summary>
        SurfaceHub,
        /// <summary>
        /// Continuum
        /// </summary>
        Continuum,
        /// <summary>
        /// Other
        /// </summary>
        Other
    }
}
