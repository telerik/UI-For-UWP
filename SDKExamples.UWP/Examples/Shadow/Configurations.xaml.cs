using System;
using System.Collections.Generic;
using System.Reflection;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SDKExamples.UWP.Shadow
{
    public sealed partial class Configurations : ExamplePageBase
    {
        private ViewModel viewModel;

        public Configurations()
        {
            this.InitializeComponent();

            this.viewModel = new ViewModel();
            this.DataContext = this.viewModel;

            this.content.ItemsSource = new List<ContentType>() 
            {
                new ContentType() { Name = "Ellipse", Type = typeof(Ellipse) },
                new ContentType() { Name = "Rectangle", Type = typeof(Rectangle) },
                new ContentType() { Name = "TextBlock", Type = typeof(TextBlock) },
                new ContentType() { Name = "RadCalendar", Type = typeof(RadCalendar) },
                new ContentType() { Name = "RadNumericBox", Type = typeof(RadNumericBox) },
                new ContentType() { Name = "RadDatePicker", Type = typeof(RadDatePicker) },
                new ContentType() { Name = "RadDataGrid", Type = typeof(RadDataGrid) },
                new ContentType() { Name = "RadListView", Type = typeof(RadListView) },
            };

            this.content.SelectionChanged += this.Content_SelectionChanged;
            this.content.SelectedIndex = 0;
        }

        private void Content_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var type = (ContentType)e.AddedItems[0];
                var instance = Activator.CreateInstance(type.Type);
                if (instance is RadCalendar)
                {
                    this.InitCalendar((RadCalendar)instance);
                }

                if (instance is RadDataGrid)
                {
                    this.InitDataGrid((RadDataGrid)instance);
                }

                if (instance is RadListView)
                {
                    this.InitListView((RadListView)instance);
                }

                if (instance is RadNumericBox)
                {
                    this.InitNumericBox((RadNumericBox)instance);
                }

                if (instance is RadDatePicker)
                {
                    this.InitDatePicker((RadDatePicker)instance);
                }

                if (instance is TextBlock)
                {
                    this.InitTextBlock((TextBlock)instance);
                }

                if (instance is Ellipse || instance is Rectangle)
                {
                    this.InitShape((Shape)instance);
                }

                this.shadow.Content = instance;
            }
        }

        private void InitDatePicker(RadDatePicker datePicker)
        {
            datePicker.Background = new SolidColorBrush(Colors.White);
            datePicker.Width = 200;
        }

        private void InitNumericBox(RadNumericBox numericBox)
        {
            numericBox.Width = 200;
        }

        private void InitCalendar(RadCalendar calendar)
        {
            calendar.Width = 250;
            calendar.Height = 250;
        }

        private void InitDataGrid(RadDataGrid grid)
        {
            grid.ItemsSource = new List<Item>
            {
                new Item() { Name = "Item 1" },
                new Item() { Name = "Item 2" },
                new Item() { Name = "Item 3" },
            };

            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Name",
                PropertyName = "Name",
            });

            grid.Width = 300;
            grid.Height = 300;
        }

        private void InitListView(RadListView listView)
        {
            listView.ItemsSource = new List<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
            };

            listView.Width = 300;
            listView.Height = 300;

            listView.ItemTemplate = this.Resources["ItemTemplate"] as DataTemplate;
            listView.Background = new SolidColorBrush(Colors.White);
        }

        private void InitTextBlock(TextBlock textBlock)
        {
            textBlock.FontSize = 24;
            textBlock.Text = "This is the content";
        }

        private void InitShape(Shape shape)
        {
            shape.Fill = new SolidColorBrush(Colors.Orange);
            shape.Width = 100;
            shape.Height = 100;
        }

        class ViewModel : ViewModelBase
        {
            private double opacity = 0.26;
            private double blur = 10.0;
            private double offsetX;
            private double offsetY;
            private Color color = Colors.Black;
            private double cornerRadius;

            public ViewModel()
            {
                this.ColorsSource = new List<Color>();
                foreach (var color in typeof(Colors).GetRuntimeProperties())
                {
                    this.ColorsSource.Add((Color)color.GetValue(null));
                }
            }

            public List<Color> ColorsSource { get; set; }

            public double Opacity
            {
                get
                {
                    return this.opacity;
                }
                set
                {
                    if (this.opacity != value)
                    {
                        this.opacity = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            public double Blur
            {
                get
                {
                    return this.blur;
                }
                set
                {
                    if (this.blur != value)
                    {
                        this.blur = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            public Color Color
            {
                get
                {
                    return this.color;
                }
                set
                {
                    if (this.color != value)
                    {
                        this.color = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            public double OffsetX
            {
                get
                {
                    return this.offsetX;
                }
                set
                {
                    if (this.offsetX != value)
                    {
                        this.offsetX = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            public double OffsetY
            {
                get
                {
                    return this.offsetY;
                }
                set
                {
                    if (this.offsetY != value)
                    {
                        this.offsetY = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            public double CornerRadius
            {
                get
                {
                    return this.cornerRadius;
                }
                set
                {
                    if (this.cornerRadius != value)
                    {
                        this.cornerRadius = value;
                        this.OnPropertyChanged();
                    }
                }
            }
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ContentType
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
    }
}