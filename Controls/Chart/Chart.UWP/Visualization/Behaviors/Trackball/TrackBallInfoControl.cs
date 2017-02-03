using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Chart.Primitives
{
    /// <summary>
    /// Represents the control that displays the track information, provided by a <see cref="ChartTrackBallBehavior"/>.
    /// </summary>
    [TemplatePart(Name = "PART_Panel", Type = typeof(StackPanel))]
    public class TrackBallInfoControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="DataPointInfoTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty DataPointInfoTemplateProperty =
            DependencyProperty.Register(nameof(DataPointInfoTemplate), typeof(DataTemplate), typeof(TrackBallInfoControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DataPointInfoTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(TrackBallInfoControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(TrackBallInfoControl), new PropertyMetadata(null));

        private const string PanelPartName = "PART_Panel";

        private Panel panel;
        private ChartTrackBallBehavior owner;
        private TrackBallInfoEventArgs pendingUpdates;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBallInfoControl"/> class.
        /// </summary>
        internal TrackBallInfoControl(ChartTrackBallBehavior owner)
        {
            this.DefaultStyleKey = typeof(TrackBallInfoControl);
            this.owner = owner;
        }

        /// <summary>
        /// Gets or sets the object that represents the header content of the control.
        /// </summary>
        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> that defines the appearance of the information about a single <see cref="DataPointInfo"/>.
        /// </summary>
        public DataTemplate DataPointInfoTemplate
        {
            get
            {
                return this.GetValue(DataPointInfoTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(DataPointInfoTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> that defines the appearance of the header of this control.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return this.GetValue(HeaderTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the default z-index of the <see cref="TrackBallInfoControl"/>.
        /// </summary>
        public int DefaultZIndex
        {
            get
            {
                return RadChartBase.TrackBallInfoControlZIndex;
            }
        }

        internal void Update(TrackBallInfoEventArgs e)
        {
            if (this.panel == null)
            {
                this.pendingUpdates = e;
                return;
            }

            if (e.Header != null)
            {
                this.Header = e.Header;
            }

            DataTemplate defaultTemplate = this.DataPointInfoTemplate;

            // add presenter for each data point
            int index = 0;
            foreach (DataPointInfo info in e.Context.DataPoints.OrderBy(c => c.Priority))
            {
                ContentPresenter presenter;
                if (index >= this.panel.Children.Count)
                {
                    presenter = new ContentPresenter();
                    this.panel.Children.Add(presenter);
                }
                else
                {
                    presenter = this.panel.Children[index] as ContentPresenter;
                }

                DataTemplate seriesInfoTemplate = ChartTrackBallBehavior.GetTrackInfoTemplate(info.Series);

                presenter.Content = info;
                presenter.ContentTemplate = seriesInfoTemplate == null ? defaultTemplate : seriesInfoTemplate;

                index++;
            }

            for (int i = index; i < this.panel.Children.Count; i++)
            {
                this.panel.Children.RemoveAt(i);
            }
        }

        /// <summary>
        /// Resolves the PART_Panel template part.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.panel = this.GetTemplatePartField<Panel>(PanelPartName);

            return this.panel != null;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.pendingUpdates != null)
            {
                this.Update(this.pendingUpdates);
                this.pendingUpdates = null;
            }
        }
    }
}
