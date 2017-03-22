using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PointTemplate : ExamplePageBase
    {
        public PointTemplate()
        {
            this.InitializeComponent();
        }
    }

    public class CustomPointPointTemplate
    {
        public double Value { get; set; }
        public string Category { get; set; }
    }

    public class ViewModelPointTemplate
    {
        public ViewModelPointTemplate()
        {
            this.Source = new List<CustomPointPointTemplate>()
            {
                new CustomPointPointTemplate{ Category = "first", Value = 10},
                new CustomPointPointTemplate{ Category = "second", Value = 32},
                new CustomPointPointTemplate{ Category = "third", Value = 15},
            };
        }
        public List<CustomPointPointTemplate> Source { get; set; }
    }

}
