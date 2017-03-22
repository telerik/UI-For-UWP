using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace SDKExamples.UWP.Listview
{
    public sealed partial class ItemAnimations : ExamplePageBase, INotifyPropertyChanged
    {
        private ObservableCollection<int> source = new ObservableCollection<int>(Enumerable.Range(0, 10));

        public ObservableCollection<int> Source
        {
            get { return this.source; }
            set
            {
                if (this.source != value)
                {
                    this.source = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Source"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ItemAnimations()
        {
            this.InitializeComponent();
            this.DataContext = this; 

            this.combo.ItemsSource = Enum.GetValues(typeof(ItemAnimationMode));
            this.combo.SelectedItem = ItemAnimationMode.PlayAll;

            RadMoveAndFadeAnimation animation = new RadMoveAndFadeAnimation();
            animation.FadeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            animation.FadeAnimation.StartOpacity = 0.5;
            animation.FadeAnimation.EndOpacity = 1;
            animation.MoveAnimation.StartPoint = new Point(40, 0);
            animation.MoveAnimation.EndPoint = new Point(0, 0);
            animation.MoveAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            this.listView.ItemAddedAnimation = animation;

            RadMoveAndFadeAnimation animation2 = new RadMoveAndFadeAnimation();
            animation2.FadeAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            animation2.FadeAnimation.StartOpacity = 1;
            animation2.FadeAnimation.EndOpacity = 0.5;
            animation2.MoveAnimation.StartPoint = new Point(0, 0);
            animation2.MoveAnimation.EndPoint = new Point(40, 0);
            animation2.MoveAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            this.listView.ItemRemovedAnimation = animation2;
        }

        private void ModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.listView.ItemAnimationMode = (ItemAnimationMode)(sender as ComboBox).SelectedItem;
        }

        private void AddItemClick(object sender, RoutedEventArgs e)
        {
            if (this.Source == null)
            {
                this.Source = new ObservableCollection<int>();
            }

            this.Source.Add(this.source.Count);
        }

        private void RemoveItemClick(object sender, RoutedEventArgs e)
        {
            if (this.Source == null)
            {
                return;
            }

            if (this.Source.Count > 0)
            {
                this.Source.RemoveAt(this.source.Count - 1);
            }
        }

        private void ResetSourceClick(object sender, RoutedEventArgs e)
        {
            this.Source = new ObservableCollection<int>(Enumerable.Range(0, 10));
        }

        private void SetNullSourceClick(object sender, RoutedEventArgs e)
        {
            this.Source = null;
        }
    }
}
