using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    public class CartesianCustomAnnotationAutomationPeer : ChartAnnotationAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the CartesianCustomAnnotationAutomationPeer class.
        /// </summary>
        public CartesianCustomAnnotationAutomationPeer(CartesianCustomAnnotation owner) 
            : base(owner)
        {
        }
        
        private CartesianCustomAnnotation CartesianCustomAnnotation
        {
            get
            {
                return (CartesianCustomAnnotation)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            if (this.CartesianCustomAnnotation.Content != null)
            {
                return this.CartesianCustomAnnotation.Content.ToString();
            }

            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            return nameof(CartesianCustomAnnotation);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(CartesianCustomAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(CartesianCustomAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "cartesian custom annotation";
        }
    }
}
