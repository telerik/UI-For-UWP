using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class ChartAreaModel : RootElement
    {
        internal ChartPlotAreaModel plotArea;
        internal IChartView view;

        private byte suspendUpdate;

        public ChartAreaModel()
        {
            this.plotArea = new ChartPlotAreaModel();
            this.children.Add(this.plotArea);
        }

        /// <summary>
        /// Gets the area where data points are plotted.
        /// </summary>
        public ChartPlotAreaModel PlotArea
        {
            get
            {
                return this.plotArea;
            }
        }

        /// <summary>
        /// Gets the collection with all the series currently plotted by this instance.
        /// </summary>
        public ElementCollection<ChartSeriesModel> Series
        {
            get
            {
                return this.plotArea.series;
            }
        }

        public override IElementPresenter Presenter
        {
            get
            {
                return this.view;
            }
        }

        public void OnZoomChanged()
        {
            if (this.IsTreeLoaded)
            {
                this.ProcessZoomChanged();
            }
        }

        public void OnPlotOriginChanged()
        {
            if (this.IsTreeLoaded)
            {
                this.ProcessPlotOriginChanged();
            }
        }

        public void BeginUpdate()
        {
            this.suspendUpdate++;
        }

        public void EndUpdate()
        {
            this.EndUpdate(false);
        }

        public void EndUpdate(bool refresh)
        {
            if (this.suspendUpdate == 0)
            {
                return;
            }

            this.suspendUpdate--;
            if (this.suspendUpdate == 0 && refresh)
            {
                this.InvalidateNode(this);
            }
        }

        public void LoadElementTree(IChartView chartView)
        {
            if (this.IsTreeLoaded)
            {
                return;
            }

            this.view = chartView;
            this.View = chartView;

            this.Load(this);
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            this.BeginUpdate();

            this.plotArea.Arrange(rect);
            this.ApplyLayoutRounding();

            this.EndUpdate(false);

            return rect;
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child == this.plotArea)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void PreviewMessage(Message message)
        {
            if (!this.IsTreeLoaded)
            {
                message.StopDispatch = true;
            }
            else
            {
                if (message.Id == Node.PropertyChangedMessage)
                {
                    ChartAreaInvalidateFlags flags = PropertyKeys.GetPropertyFlags<ChartAreaInvalidateFlags>((message.Data as RadPropertyEventArgs).Key);
                    this.Invalidate(flags);
                }
                else if (message.Id == ChartSeriesModel.DataPointsModifiedMessageKey)
                {
                    this.Invalidate(ChartAreaInvalidateFlags.All);
                }
            }
        }

        internal void Invalidate(ChartAreaInvalidateFlags flags)
        {
            if (this.IsTreeLoaded)
            {
                this.InvalidateCore(flags);
            }
        }

        internal virtual void InvalidateCore(ChartAreaInvalidateFlags flags)
        {
            if ((flags & ChartAreaInvalidateFlags.InvalidateSeries) == ChartAreaInvalidateFlags.InvalidateSeries)
            {
                foreach (ChartSeriesModel series in this.plotArea.series)
                {
                    series.Invalidate();
                }
            }
        }

        internal virtual IEnumerable<string> GetNotLoadedReasons()
        {
            yield break;
        }

        protected virtual void ProcessZoomChanged()
        {
            this.Invalidate();
        }

        protected virtual void ProcessPlotOriginChanged()
        {
            this.Invalidate();
        }
    }
}
