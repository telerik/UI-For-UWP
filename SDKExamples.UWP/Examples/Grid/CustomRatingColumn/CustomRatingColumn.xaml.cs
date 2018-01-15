// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataGrid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomRatingColumn : ExamplePageBase
    {
        public CustomRatingColumn()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel();
        }
    }
}