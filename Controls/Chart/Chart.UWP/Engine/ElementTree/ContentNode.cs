using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class ContentNode : Node
    {
        internal static readonly int ContentPropertyKey = PropertyKeys.Register(typeof(ContentNode), "Content", ChartAreaInvalidateFlags.All);

        internal RadSize desiredSize = RadSize.Empty;

        /// <summary>
        /// Gets or sets the content associated with the node.
        /// </summary>
        public object Content
        {
            get
            {
                return this.GetValue(ContentPropertyKey);
            }
            set
            {
                this.SetValue(ContentPropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e.Key == ContentPropertyKey)
            {
                this.desiredSize = RadSize.Empty;
            }

            base.OnPropertyChanged(e);
        }
    }
}
