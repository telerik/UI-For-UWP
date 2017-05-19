using System;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    internal class ComboBoxEditor : ComboBox, ITypeEditor
    {
        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...    
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(ComboBoxEditor), new PropertyMetadata(null, OnSourceChanged));
        
        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public void BindEditor()
        {
            Binding b0 = new Binding();
            b0.Source = this;
            b0.Path = new PropertyPath("ItemsSource");
            this.SetBinding(ComboBoxEditor.SourceProperty, b0);

            Binding b1 = new Binding();
            b1.Path = new PropertyPath("ValueOptions");
            this.SetBinding(ComboBoxEditor.ItemsSourceProperty, b1);

            Binding b2 = new Windows.UI.Xaml.Data.Binding();
            b2.Path = new PropertyPath("Watermark");
            this.SetBinding(ComboBoxEditor.PlaceholderTextProperty, b2);

            Binding b3 = new Binding();
            b3.Converter = new BooleanNotConverter();
            b3.Path = new PropertyPath("IsReadOnly");
            this.SetBinding(ComboBoxEditor.IsEnabledProperty, b3);

            Binding b4 = new Binding();
            b4.Path = new PropertyPath("DisplayMemberPath");
            this.SetBinding(ComboBoxEditor.DisplayMemberPathProperty, b4);

            Binding b5 = new Binding();
            b5.Path = new PropertyPath("SelectedValuePath");
            this.SetBinding(ComboBoxEditor.SelectedValuePathProperty, b5);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var combo = d as ComboBox;
            if (combo != null)
            {
                Binding b1 = new Binding();
                b1.Mode = BindingMode.TwoWay;

                var property = combo.DataContext as GridFormEntityProperty;
                if (property != null)
                {
                    if (!string.IsNullOrEmpty(property.SelectedValuePath))
                    {
                        b1.Path = new PropertyPath("PropertyValue");
                        combo.SetBinding(ComboBoxEditor.SelectedValueProperty, b1);
                    }
                    else
                    {
                        b1.Path = new PropertyPath("PropertyValue");
                        combo.SetBinding(ComboBoxEditor.SelectedItemProperty, b1);
                    }
                }
            }
        }
    }
}
