using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrackBallBehavior : ExamplePageBase
    {
        public TrackBallBehavior()
        {
            this.InitializeComponent();
        }  
    }

    public class ViewModelTrackBall
    {
        public ViewModelTrackBall()
        {
            this.Source = new List<CustomPointTrackBall>()
            {
                new CustomPointTrackBall{ Category = "first", Value = 10},
                new CustomPointTrackBall{ Category = "second", Value = 32},
                new CustomPointTrackBall{ Category = "third", Value = 15},
            };
        }


        public List<CustomPointTrackBall> Source { get; set; }
    }

    public class CustomPointTrackBall
    {
        public double Value { get; set; }
        public string Category { get; set; }
    }
}
