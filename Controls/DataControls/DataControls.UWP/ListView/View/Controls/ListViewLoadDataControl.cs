using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// This control is used to explicitly load more data in the <see cref="RadListView"/> control.
    /// </summary>
    public class ListViewLoadDataControl : ListViewLoadDataControlBase
    {
        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ListViewLoadDataControl), new PropertyMetadata(null, OnContentChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewLoadDataControl" /> class.
        /// </summary>
        public ListViewLoadDataControl()
        {
            this.DefaultStyleKey = typeof(ListViewLoadDataControl);
            this.LoadDataUICommand = new ListViewLoadDataUICommand(this);
        }

        private ContentControl contentControl;

        public ICommand LoadDataUICommand { get; set; }

        internal RadListView Owner { get; set; }

        public string LoadMoreItemsText
        {
            get
            {
                return DataControlsLocalizationManager.Instance.GetString("LoadMoreItemsString");
            }
        }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.contentControl = this.GetTemplatePartField<ContentControl>("PART_Content");

            return applied && this.contentControl != null;
        }

        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.Content != null)
            {
                this.contentControl.Content = this.Content;
            }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // We use such approach instead of TemplateBinding because for some reason
            // WP8.1 has some restriction when binding to UIElement which causes an exception.
            var control = d as ListViewLoadDataControl;
            if (control != null && control.IsTemplateApplied)
            {
                control.contentControl.Content = e.NewValue;
            }
        }
    }

    public class ListViewLoadDataUICommand : ICommand
    {
        public ListViewLoadDataUICommand(ListViewLoadDataControl owner)
        {
            this.LoadDataControl = owner;
        }
        public ListViewLoadDataControl LoadDataControl { get; set; }
#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.LoadDataControl.Owner.CommandService.ExecuteCommand(Commands.CommandId.LoadMoreData, new LoadMoreDataContext());
        }
    }
}
