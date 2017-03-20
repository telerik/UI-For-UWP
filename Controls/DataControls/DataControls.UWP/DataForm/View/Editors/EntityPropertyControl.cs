using Telerik.Data.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    [TemplatePart(Name = "PART_Content", Type = typeof(Grid))]
    public class EntityPropertyControl : RadControl, IEntityPropertyEditor
    {
        private EntityProperty property;
        internal EntityProperty Property
        {
            get
            {
                return this.property;
            }
            set
            {
                this.property = value;
            }
        }


        internal Grid container;

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(2, OnRowCountChanged));

        private static void OnRowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            editor.UpdateRowCount((int)e.NewValue);

        }

        private void UpdateRowCount(int rows)
        {
            if (this.container != null)
            {
                this.container.RowDefinitions.Clear();
                for (int i = 0; i < rows; i++)
                {
                    this.container.RowDefinitions.Add(new RowDefinition());
                }
            }
        }

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(2, OnColumnCountChanged));

        private static void OnColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            editor.UpdateColumnCount((int)e.NewValue);
        }

        private void UpdateColumnCount(int columns)
        {
            if (this.container != null)
            {
                this.container.ColumnDefinitions.Clear();
                for (int i = 0; i < columns; i++)
                {
                    this.container.ColumnDefinitions.Add(new ColumnDefinition());
                }
            }
        }

        public int ViewRow
        {
            get { return (int)GetValue(ViewRowProperty); }
            set { SetValue(ViewRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewRowProperty =
            DependencyProperty.Register(nameof(ViewRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnViewRowChanged));

        private static void OnViewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;

            if (editor.View != null)
            {
                editor.View.SetValue(Grid.RowProperty, e.NewValue);
            }

        }

        public int ViewColumn
        {
            get { return (int)GetValue(ViewColumnProperty); }
            set { SetValue(ViewColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewColumnProperty =
            DependencyProperty.Register(nameof(ViewColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnViewColumnChanged));

        private static void OnViewColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.View != null)
            {
                editor.View.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }

        public int LabelRow
        {
            get { return (int)GetValue(LabelRowProperty); }
            set { SetValue(LabelRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelRowProperty =
            DependencyProperty.Register(nameof(LabelRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnLabelRowChanged));

        private static void OnLabelRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.Label != null)
            {
                editor.Label.SetValue(Grid.RowProperty, e.NewValue);
            }
        }

        public int LabelColumn
        {
            get { return (int)GetValue(LabelColumnProperty); }
            set { SetValue(LabelColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelColumnProperty =
            DependencyProperty.Register(nameof(LabelColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnLabelColumnChanged));

        private static void OnLabelColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.Label != null)
            {
                editor.Label.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }

        public int ErrorViewRow
        {
            get { return (int)GetValue(ErrorViewRowProperty); }
            set { SetValue(ErrorViewRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorViewRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorViewRowProperty =
            DependencyProperty.Register(nameof(ErrorViewRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnErrorViewRowChanged));

        private static void OnErrorViewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.ErrorView != null)
            {
                editor.ErrorView.SetValue(Grid.RowProperty, e.NewValue);
            }
        }

        public int ErrorViewColumn
        {
            get { return (int)GetValue(ErrorViewColumnProperty); }
            set { SetValue(ErrorViewColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorViewColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorViewColumnProperty =
            DependencyProperty.Register(nameof(ErrorViewColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnErrorViewColumnChanged));

        private static void OnErrorViewColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.ErrorView != null)
            {
                editor.ErrorView.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }

        public int PositiveMessageViewRow
        {
            get { return (int)GetValue(PositiveMessageViewRowProperty); }
            set { SetValue(PositiveMessageViewRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorViewRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositiveMessageViewRowProperty =
            DependencyProperty.Register(nameof(PositiveMessageViewRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, PositiveMessageViewRowChanged));

        private static void PositiveMessageViewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.PositiveMessageView != null)
            {
                editor.PositiveMessageView.SetValue(Grid.RowProperty, e.NewValue);
            }
        }

        public int PositiveMessageViewColumn
        {
            get { return (int)GetValue(PositiveMessageViewColumnProperty); }
            set { SetValue(PositiveMessageViewColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorViewColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositiveMessageViewColumnProperty =
            DependencyProperty.Register(nameof(PositiveMessageViewColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, PositiveMessageViewColumnChanged));

        private static void PositiveMessageViewColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.PositiveMessageView != null)
            {
                editor.PositiveMessageView.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }

        public EntityPropertyControl()
        {
            this.DefaultStyleKey = typeof(EntityPropertyControl);
        }

        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.container = this.GetTemplatePartField<Grid>("PART_Content");
            applied = applied && this.container != null;

            return applied;
        }

        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();
            if (this.View != null)
            {
                this.AddView(this.View);
            }
            if (this.Label != null)
            {
                this.AddView(this.Label);
            }

            if (this.ErrorView != null)
            {
                this.AddView(this.ErrorView);
            }

            if (this.PositiveMessageView != null)
            {
                this.AddView(this.PositiveMessageView);
            }

            this.UpdateColumnCount(this.ColumnCount);
            this.UpdateRowCount(this.RowCount);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new EntityPropertyControlAutomationPeer(this);
        }

        public FrameworkElement View { get; set; }
        public FrameworkElement Label { get; set; }
        public FrameworkElement ErrorView { get; set; }
        public FrameworkElement PositiveMessageView { get; set; }

        public virtual void AddView(FrameworkElement view)
        {
            if (this.container != null)
            {
                this.container.Children.Add(view);
            }
        }

        EntityProperty IEntityPropertyEditor.Property
        {
            get { return this.Property; }
        }
    }
}
