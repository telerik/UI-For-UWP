using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SDKExamples.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            if (MainPage.Source == null)
            {
                this.LoadData();
            }
        }

        public static IEnumerable Source { get; set; }

        private async void LoadData()
        {
            var text = await Windows.Storage.PathIO.ReadTextAsync("ms-appx:///Data/Examples.xml");
            var doc = XDocument.Parse(text);

            this.DataContext = MainPage.Source = this.GetControls(doc);
        }

        private IEnumerable GetControls(XDocument doc)
        {

            return from control in doc.Descendants("Control")
                                     select new ControlData
                                     (
                                         control.Attribute("Name").Value,
                                         from example in control.Descendants("Example")
                                         select new Example(example.Attribute("ClassName").Value, example.Attribute("DisplayName").Value)
                                      );
        }

      //  private static ControlData CurrentItem { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var currentView = SystemNavigationManager.GetForCurrentView();

            var controlData = e.Parameter as ControlData;

            if (controlData == null)
            {
                this.DataContext = MainPage.Source;
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            else
            {
                this.DataContext = controlData.Examples;
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            this.DataContext = MainPage.Source;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            var selectedItem = (sender as ListView).SelectedItem;
            if (selectedItem != null)
            {
                var dataContext = (sender as FrameworkElement).DataContext;

                var control = selectedItem as ControlData;

                var example = selectedItem as Example;

                if (control != null)
                {
                    var listType = typeof(MainPage);
                    Frame.Navigate(listType, control);
                }
                else if (!string.IsNullOrEmpty(example.TypeName))
                {
                    var exampleType = Type.GetType(string.Format(example.TypeName));
                    if (exampleType != null)
                    {
                        Frame.Navigate(exampleType, example.DisplayName);
                        currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    }
                }
            }

        }
    }
}
