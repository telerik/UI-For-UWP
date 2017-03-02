using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LabelDefinitions : ExamplePageBase
    {
        public LabelDefinitions()
        {
            this.InitializeComponent();
        }

    }

    public class CustomPointLabelDefinition
    {
        public string Category { get; set; }
        public double Value { get; set; }
        public string LabelProperty { get; set; }
    }

    public class ViewModelLabelDefinition
    {
        public ViewModelLabelDefinition()
        {
            List<CustomPointLabelDefinition> collection = new List<CustomPointLabelDefinition>();
            collection.Add(new CustomPointLabelDefinition { Category = "Apples", Value = 10, LabelProperty = "First Point" });
            collection.Add(new CustomPointLabelDefinition { Category = "Oranges", Value = 8, LabelProperty = "Second Point" });
            collection.Add(new CustomPointLabelDefinition { Category = "Pineapples", Value = 21, LabelProperty = "Third Point" });
            this.Source = collection;
        }
        public List<CustomPointLabelDefinition> Source { get; set; }
    }
}
