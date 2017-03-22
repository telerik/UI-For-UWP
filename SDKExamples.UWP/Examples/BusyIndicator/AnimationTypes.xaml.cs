using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.BusyIndicator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimationTypes : ExamplePageBase
    {
        public AnimationTypes()
        {
            this.InitializeComponent();

            this.DataContext = Enum.GetValues(typeof(AnimationStyle));
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.indicator.AnimationStyle = (AnimationStyle)(sender as ListView).SelectedItem;
        }
    }
}
