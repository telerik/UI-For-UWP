using System;
using System.Windows.Input;
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
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(ListViewLoadDataControl), new PropertyMetadata(null, OnContentChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewLoadDataControl" /> class.
        /// </summary>
        public ListViewLoadDataControl()
        {
            this.DefaultStyleKey = typeof(ListViewLoadDataControl);
            this.LoadDataUICommand = new ListViewLoadDataUICommand(this);
        }

        private ContentControl contentControl;

        /// <summary>
        /// Gets or sets the commond invoked by the LoadDataUI.
        /// </summary>
        public ICommand LoadDataUICommand { get; set; }

        internal RadListView Owner { get; set; }

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
            set { SetValue(ContentProperty, value); }
        }

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

    /// <summary>
    /// Represents a command that can perform a given action.
    /// </summary>
    public class ListViewLoadDataUICommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewLoadDataUICommand"/> class.
        /// </summary>
        public ListViewLoadDataUICommand(ListViewLoadDataControl owner)
        {
            this.LoadDataControl = owner;
        }

        /// <summary>
        /// Gets or sets the <see cref="ListViewLoadDataControl"/>.
        /// </summary>
        public ListViewLoadDataControl LoadDataControl { get; set; }

#pragma warning disable 0067
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        /// <returns>
        /// Returns a value indicating whether this command can be executed.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        public void Execute(object parameter)
        {
            this.LoadDataControl.Owner.CommandService.ExecuteCommand(Commands.CommandId.LoadMoreData, new LoadMoreDataContext());
        }
    }
}
