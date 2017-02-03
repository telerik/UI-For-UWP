using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents the visual element that is displayed on top of the scrollable content in <see cref="RadListView"/>
    /// that indicates the user to pull down to refresh the content.
    /// </summary>
    public class PullToRefreshIndicator : RadControl
    {
        private const string VerticalState = "Vertical";
        private const string HorizontalState = "Horizontal";

        private const string ContentAndIndicatorState = "ContentAndIndicator";
        private const string IndicatorOnlyState = "IndicatorOnly";

        private const string pullLabelName = "PART_PullLabel";
        private const string refreshLabelName = "PART_RefreshLabel";

        /// <summary>
        /// Gets or sets the current pull offset applied to the control.
        /// </summary>
        public double CurrentPullOffset
        {
            get { return (double)GetValue(CurrentPullOffsetProperty); }
            set { SetValue(CurrentPullOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CurrentPullOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPullOffsetProperty =
            DependencyProperty.Register(nameof(CurrentPullOffset), typeof(double), typeof(PullToRefreshIndicator), new PropertyMetadata(0d));

        /// <summary>
        /// Initializes a new instance of the <see cref="PullToRefreshIndicator" /> class.
        /// </summary>
        public PullToRefreshIndicator()
        {
            this.DefaultStyleKey = typeof(PullToRefreshIndicator);
        }

        internal void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        internal void SetOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                this.GoToState(false, HorizontalState);
            }
            else
            {
                this.GoToState(false, VerticalState);
            }
        }

        internal void SetDisplayMode(PullToRefreshScrollMode mode)
        {
            if (mode == PullToRefreshScrollMode.ContentAndIndicator)
            {
                this.GoToState(false, ContentAndIndicatorState);
            }
            else
            {
                this.GoToState(false, IndicatorOnlyState);
            }
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            var applied = true;

            return base.ApplyTemplateCore() && applied;
        }

    }
}
