using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataGrid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ColumnResizing : ExamplePageBase
    {
        public ColumnResizing()
        {
            this.InitializeComponent();
            List<Student> list = new List<Student>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new Student() { Name = "Name" + i.ToString(), Age = 9 + i });

            }
            this.DataContext = list;
            this.combo.ItemsSource = Enum.GetValues(typeof(DataGridColumnResizeHandleDisplayMode));
            this.combo.SelectedItem = DataGridColumnResizeHandleDisplayMode.None;
        }

        public class Student
        {
            public string Name { get; set; }
            public double Age { get; set; }
        }
 

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.grid.ColumnResizeHandleDisplayMode = (DataGridColumnResizeHandleDisplayMode)(sender as ComboBox).SelectedItem;
        }
    }
}
