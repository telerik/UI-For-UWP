using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Pagination
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Customizations : ExamplePageBase
    {
        public Customizations()
        {
            this.InitializeComponent();

            List<ViewModel> pictures = new List<ViewModel>()
            {
                new ViewModel{ImagePath="../../Images/bee.jpg"},
                new ViewModel{ImagePath="../../Images/donkey.jpg"},
                new ViewModel{ImagePath="../../Images/butterfly.jpg"},
                new ViewModel{ImagePath="../../Images/walnuts.jpg"},
                new ViewModel{ImagePath="../../Images/elephant.jpg"}

            };

            MyFlipview.ItemsSource = pictures;

        }

        public class ViewModel
        {
            public object ImagePath { get; set; }
        }

    
        
    }
}
