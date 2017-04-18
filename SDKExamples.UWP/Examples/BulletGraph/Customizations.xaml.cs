using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.BulletGraph
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Customizations : ExamplePageBase
    {
        public Customizations()
        {
            this.InitializeComponent();

            this.orientationCombo.ItemsSource = Enum.GetNames(typeof(Orientation));
        }

        private void orientationCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var orientation = (sender as ComboBox).SelectedItem.ToString();
            if (orientation.Equals("Horizontal"))
            {
                this.bullet.Orientation = Orientation.Horizontal;
            }
            else
            {
                this.bullet.Orientation = Orientation.Vertical;
            }          
        }
    }
}
