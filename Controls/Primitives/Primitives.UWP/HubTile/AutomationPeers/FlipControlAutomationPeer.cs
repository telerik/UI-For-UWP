using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the FlipControl class.
    /// </summary>
    public class FlipControlAutomationPeer : RadControlAutomationPeer, IMultipleViewProvider
    {
        /// <summary>
        /// Initializes a new instance of the FlipControlAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public FlipControlAutomationPeer(FlipControl owner)
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public int CurrentView
        {
            get
            {
                return this.FlipControl.IsFlipped ? 1 : 0;
            }
        }

        private FlipControl FlipControl
        {
            get
            {
                return (FlipControl)this.Owner;
            }
        }

        /// <inheritdoc/>
        public int[] GetSupportedViews()
        {
            List<int> supportedViews = new List<int>();
            if (this.FlipControl.FrontContent != null)
            {
                supportedViews.Add(0);
            }

            if (this.FlipControl.BackContent != null)
            {
                supportedViews.Add(1);
            }

            return supportedViews.ToArray();
        }

        /// <summary>
        /// IMultipleViewProvider implementation.
        /// </summary>
        public string GetViewName(int viewId)
        {
            return viewId == 0 ? "FrontContent" : "BackContent";
        }

        /// <summary>
        /// IMultipleViewProvider implementation.
        /// </summary>
        public void SetCurrentView(int viewId)
        {
            if (this.GetSupportedViews().Any(a => a == viewId))
            {
                if (viewId == 0)
                {
                    this.FlipControl.IsFlipped = false;
                }
                else if (viewId == 1)
                {
                    this.FlipControl.IsFlipped = true;
                }
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "FlipControl";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "flip control";
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.MultipleView)
            {
                return this;
            }

            return null;
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return "FlipControl";
        }
    }
}
