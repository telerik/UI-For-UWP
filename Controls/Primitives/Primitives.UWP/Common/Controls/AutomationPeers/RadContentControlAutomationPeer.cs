using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadContentControl class.
    /// </summary>
    public class RadContentControlAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadContentControlAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RadContentControlAutomationPeer(RadContentControl owner) 
            : base(owner)
        {
        }

        private RadContentControl Control
        {
            get
            {
                return this.Owner as RadContentControl;
            }
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            if (this.Control.Content != null)
            {
                var textBlock = ElementTreeHelper.FindVisualDescendant<Windows.UI.Xaml.Controls.TextBlock>(this.Control);
                if (textBlock != null)
                {
                    return textBlock.Text;
                }
            }

            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad content control";
        }
    }
}
