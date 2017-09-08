using System.Windows.Input;
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
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(ListViewLoadDataControl), new PropertyMetadata(null, OnContentChanged));

        private ContentControl contentControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewLoadDataControl" /> class.
        /// </summary>
        public ListViewLoadDataControl()
        {
            this.DefaultStyleKey = typeof(ListViewLoadDataControl);
            this.LoadDataUICommand = new ListViewLoadDataUICommand(this);
        }

        /// <summary>
        /// Gets or sets the command invoked by the LoadDataUI.
        /// </summary>
        public ICommand LoadDataUICommand { get; set; }

        /// <summary>
        /// Gets the text visualized when more items are loading.
        /// </summary>
        public string LoadMoreItemsText
        {
            get
            {
                return DataControlsLocalizationManager.Instance.GetString("LoadMoreItemsString");
            }
        }

        /// <summary>
        /// Gets or sets the content of the <see cref="ListViewLoadDataControl"/>
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        internal RadListView Owner { get; set; }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.contentControl = this.GetTemplatePartField<ContentControl>("PART_Content");

            return applied && this.contentControl != null;
        }

        /// <inheritdoc/>
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
}
