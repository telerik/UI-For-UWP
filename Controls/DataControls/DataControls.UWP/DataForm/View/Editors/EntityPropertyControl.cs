using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents an EntityPropertyControl control.
    /// </summary>
    [TemplatePart(Name = "PART_Content", Type = typeof(Grid))]
    public class EntityPropertyControl : RadControl, IEntityPropertyEditor
    {
        /// <summary>
        /// Identifies the <see cref="RowCount"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(2, OnRowCountChanged));

        /// <summary>
        /// Identifies the <see cref="ColumnCount"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(2, OnColumnCountChanged));

        /// <summary>
        /// Identifies the <see cref="ViewRow"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ViewRowProperty =
            DependencyProperty.Register(nameof(ViewRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnViewRowChanged));

        /// <summary>
        /// Identifies the <see cref="ViewColumn"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ViewColumnProperty =
            DependencyProperty.Register(nameof(ViewColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnViewColumnChanged));

        /// <summary>
        /// Identifies the <see cref="LabelRow"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LabelRowProperty =
            DependencyProperty.Register(nameof(LabelRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnLabelRowChanged));

        /// <summary>
        /// Identifies the <see cref="LabelColumn"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LabelColumnProperty =
            DependencyProperty.Register(nameof(LabelColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnLabelColumnChanged));

        /// <summary>
        /// Identifies the <see cref="ErrorViewRow"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorViewRowProperty =
            DependencyProperty.Register(nameof(ErrorViewRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnErrorViewRowChanged));

        /// <summary>
        /// Identifies the <see cref="ErrorViewColumn"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorViewColumnProperty =
            DependencyProperty.Register(nameof(ErrorViewColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, OnErrorViewColumnChanged));

        /// <summary>
        /// Identifies the <see cref="PositiveMessageViewRow"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty PositiveMessageViewRowProperty =
            DependencyProperty.Register(nameof(PositiveMessageViewRow), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, PositiveMessageViewRowChanged));

        /// <summary>
        /// Identifies the <see cref="PositiveMessageViewColumn"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty PositiveMessageViewColumnProperty =
            DependencyProperty.Register(nameof(PositiveMessageViewColumn), typeof(int), typeof(EntityPropertyControl), new PropertyMetadata(0, PositiveMessageViewColumnChanged));
        
        internal Grid container;
        private EntityProperty property;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyControl"/> class.
        /// </summary>
        public EntityPropertyControl()
        {
            this.DefaultStyleKey = typeof(EntityPropertyControl);
        }

        /// <summary>
        /// Gets or sets number of rows in the control.
        /// </summary>
        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { this.SetValue(RowCountProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets number of columns in the control.
        /// </summary>
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { this.SetValue(ColumnCountProperty, value); }
        }

        /// <summary>
        /// Gets or sets the row index where the view is visualized.
        /// </summary>
        public int ViewRow
        {
            get { return (int)GetValue(ViewRowProperty); }
            set { this.SetValue(ViewRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the column index where the view is visualized.
        /// </summary>
        public int ViewColumn
        {
            get { return (int)GetValue(ViewColumnProperty); }
            set { this.SetValue(ViewColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the row index where the label is visualized.
        /// </summary>
        public int LabelRow
        {
            get { return (int)GetValue(LabelRowProperty); }
            set { this.SetValue(LabelRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the column index where the label is visualized.
        /// </summary>
        public int LabelColumn
        {
            get { return (int)GetValue(LabelColumnProperty); }
            set { this.SetValue(LabelColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the row index where the error is visualized.
        /// </summary>
        public int ErrorViewRow
        {
            get { return (int)GetValue(ErrorViewRowProperty); }
            set { this.SetValue(ErrorViewRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the column index where the error is visualized.
        /// </summary>
        public int ErrorViewColumn
        {
            get { return (int)GetValue(ErrorViewColumnProperty); }
            set { this.SetValue(ErrorViewColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the row index where the positive message is visualized.
        /// </summary>
        public int PositiveMessageViewRow
        {
            get { return (int)GetValue(PositiveMessageViewRowProperty); }
            set { this.SetValue(PositiveMessageViewRowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the column index where the positive message is visualized.
        /// </summary>
        public int PositiveMessageViewColumn
        {
            get { return (int)GetValue(PositiveMessageViewColumnProperty); }
            set { this.SetValue(PositiveMessageViewColumnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the view of the <see cref="EntityPropertyControl"/>
        /// </summary>
        public FrameworkElement View { get; set; }

        /// <summary>
        /// Gets or sets the label of the <see cref="EntityPropertyControl"/>
        /// </summary>
        public FrameworkElement Label { get; set; }

        /// <summary>
        /// Gets or sets the error view of the <see cref="EntityPropertyControl"/>
        /// </summary>
        public FrameworkElement ErrorView { get; set; }

        /// <summary>
        /// Gets or sets the positive message view of the <see cref="EntityPropertyControl"/>
        /// </summary>
        public FrameworkElement PositiveMessageView { get; set; }

        EntityProperty IEntityPropertyEditor.Property
        {
            get { return this.Property; }
        }

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

        /// <summary>
        /// Adds a view to the container of the control.
        /// </summary>
        public virtual void AddView(FrameworkElement view)
        {
            if (this.container != null)
            {
                this.container.Children.Add(view);
            }
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.container = this.GetTemplatePartField<Grid>("PART_Content");
            applied = applied && this.container != null;

            return applied;
        }

        /// <inheritdoc/>
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
        private static void OnColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            editor.UpdateColumnCount((int)e.NewValue);
        }

        private static void PositiveMessageViewColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.PositiveMessageView != null)
            {
                editor.PositiveMessageView.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }
        
        private static void OnRowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            editor.UpdateRowCount((int)e.NewValue);
        }

        private static void OnViewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;

            if (editor.View != null)
            {
                editor.View.SetValue(Grid.RowProperty, e.NewValue);
            }
        }

        private static void PositiveMessageViewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.PositiveMessageView != null)
            {
                editor.PositiveMessageView.SetValue(Grid.RowProperty, e.NewValue);
            }
        }

        private static void OnErrorViewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.ErrorView != null)
            {
                editor.ErrorView.SetValue(Grid.RowProperty, e.NewValue);
            }
        }

        private static void OnViewColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.View != null)
            {
                editor.View.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }

        private static void OnErrorViewColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.ErrorView != null)
            {
                editor.ErrorView.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }

        private static void OnLabelColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.Label != null)
            {
                editor.Label.SetValue(Grid.ColumnProperty, e.NewValue);
            }
        }

        private static void OnLabelRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as EntityPropertyControl;
            if (editor.Label != null)
            {
                editor.Label.SetValue(Grid.RowProperty, e.NewValue);
            }
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
    }
}
