using Windows.System.Profile;
using Windows.UI.Xaml;

namespace Telerik.Core
{
    internal class DeviceFamilyStateTrigger : StateTriggerBase
    {
        private static string deviceFamily;

        public static readonly DependencyProperty DeviceFamilyProperty =
            DependencyProperty.Register("DeviceFamily", typeof(string), typeof(DeviceFamilyStateTrigger), new PropertyMetadata(null, OnDeviceTypePropertyChanged));

        static DeviceFamilyStateTrigger()
        {
            deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
        }

        public string DeviceFamily
        {
            get { return (string) GetValue(DeviceFamilyProperty); }
            set { SetValue(DeviceFamilyProperty, value); }
        }

        private static void OnDeviceTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (DeviceFamilyStateTrigger) d;
            var value = (string) e.NewValue;

            trigger.SetActive(value == deviceFamily);
        }
    }
}
