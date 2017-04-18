using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.LoopingList
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Customizations : ExamplePageBase
    {
        public Customizations()
        {
            this.InitializeComponent();
            this.DataContext = new List<int>() { 1, 2, 4, 5, 6, 7, 8, 9, 10 };
            this.orientationCombo.ItemsSource = new List<string>() {"Horizontal", "Vertical" };
            this.orientationCombo.SelectedIndex = 1;
            this.positionCombo.ItemsSource = Enum.GetValues(typeof(LoopingListItemSnapPosition));
            this.positionCombo.SelectedIndex = 0;
            this.loopCombo.ItemsSource = new List<string>() {"true", "false" };
            this.loopCombo.SelectedIndex = 0;
        }

        private void OrientationModeClick(object sender, SelectionChangedEventArgs e)
        {
            var orientation = (sender as ComboBox).SelectedItem.ToString();

            if (orientation.Equals("Horizontal"))
            {
                this.loopingList.Orientation = Orientation.Horizontal;
            }
            else
            {
                this.loopingList.Orientation = Orientation.Vertical;
            }
        }

        private void CenteredItemSnapPositionClick(object sender, SelectionChangedEventArgs e)
        {
            var position = (LoopingListItemSnapPosition)(sender as ComboBox).SelectedItem;

            this.loopingList.CenteredItemSnapPosition = position;
        }

        private void LoopModeClicked(object sender, SelectionChangedEventArgs e)
        {
            var mode = (sender as ComboBox).SelectedItem.ToString();
            if (mode.Equals("true"))
            {
                this.loopingList.IsLoopingEnabled = true;
            }
            else
            {
                this.loopingList.IsLoopingEnabled = false;
            }
        }

        private void HeightChagned(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            this.loopingList.ItemHeight = e.NewValue;
        }

        private void SpacingChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            this.loopingList.ItemSpacing = e.NewValue;
        }
    }
}
