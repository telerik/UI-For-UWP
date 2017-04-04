using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.UI.Automation.Peers
{
    public class PolarCustomAnnotationAutomationPeer : ChartAnnotationAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the PolarCustomAnnotationAutomationPeer class.
        /// </summary>
        public PolarCustomAnnotationAutomationPeer(PolarCustomAnnotation owner)
                : base(owner)
        {
        }

        private PolarCustomAnnotation PolarCustomAnnotation
        {
            get
            {
                return (PolarCustomAnnotation)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            if (this.PolarCustomAnnotation.Content != null)
            {
                return this.PolarCustomAnnotation.Content.ToString();
            }

            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return nameof(PolarCustomAnnotation);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(PolarCustomAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(PolarCustomAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "polar custom annotation";
        }
    }
}
