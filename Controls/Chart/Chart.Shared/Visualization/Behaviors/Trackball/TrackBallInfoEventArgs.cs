using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Encapsulates the data associated with a <see cref="E:ChartTrackBallBehavior.TrackInfoUpdated"/> event.
    /// </summary>
    public class TrackBallInfoEventArgs : EventArgs
    {
        private ChartDataContext context;
        private object header;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBallInfoEventArgs"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public TrackBallInfoEventArgs(ChartDataContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets or sets the header of the info control.
        /// </summary>
        public object Header
        {
            get
            {
                return this.header;
            }
            set
            {
                this.header = value;
            }
        }

        /// <summary>
        /// Gets the context associated with the event.
        /// </summary>
        public ChartDataContext Context
        {
            get
            {
                return this.context;
            }
        }
    }
}
