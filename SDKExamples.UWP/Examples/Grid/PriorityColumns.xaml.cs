using System;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataGrid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PriorityColumns : ExamplePageBase
    {
        public PriorityColumns()
        {
            this.InitializeComponent();

            ObservableCollection<Item> source = new ObservableCollection<Item>();
            source.Add(new Item() { FirstName = "Ivaylo", MiddleName = "Plamenov", LastName = "Gergov", Age = 25 });
            source.Add(new Item() { FirstName = "Rosi", MiddleName = "Nechiq", LastName = "Topchiyska", Age = 27 });
            source.Add(new Item() { FirstName = "Ivan", MiddleName = "Vladimirov", LastName = "Simeonov", Age = 26 });
            source.Add(new Item() { FirstName = "Kaloyan", MiddleName = "Smilenov", LastName = "Petrov", Age = 30});

            this.DataContext = source;
        }


        public class Item
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public double Age { get; set; }         
        }

        private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            this.grid.Width = (double)e.NewValue;
        }
    }

    public class GridAdaptiveTrigger : StateTriggerBase
    {
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(double), typeof(GridAdaptiveTrigger), new PropertyMetadata(0d));


        public RadDataGrid Owner
        {
            get { return (RadDataGrid)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Owner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(RadDataGrid), typeof(GridAdaptiveTrigger), new PropertyMetadata(null, OnOwnerChanged));

        private static void OnOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = e.NewValue as RadDataGrid;
            var oldGrid = e.OldValue as RadDataGrid;
            if(oldGrid != null)
            {
                oldGrid.SizeChanged -= (d as GridAdaptiveTrigger).Grid_SizeChanged;
            }
            if (grid != null)
            {
                grid.SizeChanged += (d as GridAdaptiveTrigger).Grid_SizeChanged;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < this.MinWidth)
            {
                this.SetActive(true);
            }
            else
            {
                this.SetActive(false);
            }
        }
    }
}
