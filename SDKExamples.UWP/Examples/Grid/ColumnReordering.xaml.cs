using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataGrid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ColumnReordering : ExamplePageBase
    {
        public ColumnReordering()
        {
            this.InitializeComponent();

            this.DataContext = new List<Student>(Enumerable.Range(9,10).Select(i=> new Student { Name = "Name" + i.ToString(), Age = 9 + i }));

            this.combo.ItemsSource = Enum.GetValues(typeof(DataGridUserColumnReorderMode));
            this.combo.SelectedItem = DataGridUserColumnReorderMode.Interactive;
        }

        public class Student
        {
            public string Name { get; set; }
            public double Age { get; set; }
        }
 
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.grid.UserColumnReorderMode = (DataGridUserColumnReorderMode)(sender as ComboBox).SelectedItem;
        }
    }
}
