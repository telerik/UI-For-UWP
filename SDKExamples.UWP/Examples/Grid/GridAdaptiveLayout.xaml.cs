using System;
using System.Collections.ObjectModel;
using Telerik.Core;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace SDKExamples.UWP.DataGrid
{
    public sealed partial class GridAdaptiveLayout : ExamplePageBase
    {
        public GridAdaptiveLayout()
        {
            this.InitializeComponent();

            ObservableCollection<Item> source = new ObservableCollection<Item>();
            source.Add(new Item { Name = "Ivaylo", Age = 25, Birthday = DateTime.Today.AddYears(-25) });
            source.Add(new Item { Name = "Rosi", Age = 27, Birthday = DateTime.Today.AddYears(-27) });
            source.Add(new Item { Name = "Ivan", Age = 26, Birthday = DateTime.Today.AddYears(-26) });
            source.Add(new Item { Name = "Kaloyan", Age = 30, Birthday = DateTime.Today.AddYears(-30) });

            this.DataContext = source;
        }

        public class Item : ViewModelBase
        {
            private string name;
            private int age;
            private DateTime birthday;
            private bool isMarried;

            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    if (this.name != value)
                    {
                        this.name = value;
                        OnPropertyChanged();
                    }
                }
            }

            public int Age
            {
                get
                {
                    return age;
                }
                set
                {
                    if (this.age != value)
                    {
                        this.age = value;
                        OnPropertyChanged();
                    }
                }
            }

            public DateTime Birthday
            {
                get
                {
                    return birthday;
                }
                set
                {
                    if (this.birthday != value)
                    {
                        this.birthday = value;
                        OnPropertyChanged();
                    }
                }
            }

            public bool IsMarried
            {
                get
                {
                    return isMarried;
                }
                set
                {
                    if (this.isMarried != value)
                    {
                        this.isMarried = value;
                        OnPropertyChanged();
                    }
                }
            }
        }
    }

    public class CustomTrigger : StateTriggerBase
    {
        private static string deviceFamily;

        public static readonly DependencyProperty DeviceFamilyProperty =
            DependencyProperty.Register("DeviceFamily", typeof(string), typeof(CustomTrigger), new PropertyMetadata(null, OnChanged));

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(double), typeof(CustomTrigger), new PropertyMetadata(0d, OnChanged));

        static CustomTrigger()
        {
            deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
        }

        public CustomTrigger()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        public string DeviceFamily
        {
            get { return (string)GetValue(DeviceFamilyProperty); }
            set { SetValue(DeviceFamilyProperty, value); }
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.UpdateIsActive();
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as CustomTrigger;
            trigger.UpdateIsActive();
        }

        private void UpdateIsActive()
        {
            if (this.DeviceFamily == deviceFamily)
            {
                if (deviceFamily == "Windows.Mobile")
                {
                    this.SetActive(true);
                }
                else
                {
                    var width = Window.Current.Bounds.Width;
                    var isActive = this.MinWidth <= width;
                    this.SetActive(isActive);
                }
            }
            else
            {
                this.SetActive(false);
            }
        }
    }
}
