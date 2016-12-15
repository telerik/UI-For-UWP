using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Allows for pluggable customization of the appearance and layout of data point labels within a <see cref="ChartSeries"/> instance.
    /// </summary>
    public abstract class ChartSeriesLabelStrategy
    {
        /// <summary>
        /// Gets the functionality this strategy handles.
        /// </summary>
        public abstract LabelStrategyOptions Options
        {
            get;
        }

        /// <summary>
        /// Creates a <see cref="FrameworkElement"/> instance that will represent the label for the provided data point.
        /// </summary>
        /// <param name="point">The data point a label is needed for.</param>
        /// <param name="labelIndex">The index of the label. More than one label is supported per data point.</param>
        public virtual FrameworkElement CreateDefaultVisual(DataPoint point, int labelIndex)
        {
            return null;
        }

        /// <summary>
        /// Gets the content for the label at the specified index, associated with the provided data point.
        /// </summary>
        /// <param name="point">The data point the label is associated with.</param>
        /// <param name="labelIndex">The parameter is not used.</param>
        public virtual object GetLabelContent(DataPoint point, int labelIndex)
        {
            return point != null ? point.Label : null;
        }

        /// <summary>
        /// Sets the content of the label visual at the specified label index associated with the provided data point.
        /// </summary>
        /// <param name="point">The data point the label is associated with.</param>
        /// <param name="visual">The <see cref="FrameworkElement"/> instance that represents the label.</param>
        /// <param name="labelIndex">The label index.</param>
        public virtual void SetLabelContent(DataPoint point, FrameworkElement visual, int labelIndex)
        {
        }

        /// <summary>
        /// Gets the <see cref="RadRect"/> structure that defines the layout slot of the label at the specified label index, associated with the provided data point.
        /// </summary>
        public virtual RadRect GetLabelLayoutSlot(DataPoint point, FrameworkElement visual, int labelIndex)
        {
            return RadRect.Empty;
        }

        /// <summary>
        /// Gets the <see cref="RadSize"/> structure that is the desired size of the specified label visual, associated with the provided data point.
        /// </summary>
        public virtual RadSize GetLabelDesiredSize(DataPoint point, FrameworkElement visual, int labelIndex)
        {
            return RadSize.Empty;
        }
    }
}
