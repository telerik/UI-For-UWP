using System;
using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadialMenuItemControl class.
    /// </summary>
    public class RadialMenuItemControlAutomationPeer : RadControlAutomationPeer, IToggleProvider, IInvokeProvider, ISelectionItemProvider
    {
        private RadRadialMenu parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialMenuItemControlAutomationPeer"/> class.
        /// Automation Peer for the <see cref="Telerik.UI.Xaml.Controls.Primitives.Menu.RadialMenuItemControl"/> class.
        /// </summary>
        /// <param name="owner">The object that is associated with this AutomationPeer.</param>
        /// <param name="parent">The parent RadialMenu of the RadialMenuItem.</param>
        public RadialMenuItemControlAutomationPeer(RadialMenuItemControl owner, RadRadialMenu parent) 
            : base(owner)
        {
            this.parent = parent;
        }

        /// <inheritdoc/>
        public ToggleState ToggleState
        {
            get
            {
                if (this.OwnerAsRadialMenuItemControl == null && this.OwnerAsRadialMenuItemControl.Segment != null)
                {
                    return ToggleState.Indeterminate;
                }

                return this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected
                    ? ToggleState.On
                    : ToggleState.Off;
            }
        }

        /// <inheritdoc/>
        public bool IsSelected => this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected;

        /// <inheritdoc/>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                if (this.parent != null)
                {
                    return this.ProviderFromPeer(FrameworkElementAutomationPeer.CreatePeerForElement(this.parent));
                }
                return null;
            }
        }

        private RadialMenuItemControl OwnerAsRadialMenuItemControl
        {
            get
            {
                return this.Owner as RadialMenuItemControl;
            }
        }

        /// <summary>
        /// IInvokeProvider implementation.
        /// </summary>
        public void Invoke()
        {
            if (this.OwnerAsRadialMenuItemControl != null && this.OwnerAsRadialMenuItemControl.Segment != null
                && this.OwnerAsRadialMenuItemControl.Segment.TargetItem.CanNavigate && this.parent != null)
            {
                this.parent.RaiseNavigateCommand(
                    this.OwnerAsRadialMenuItemControl.Segment.TargetItem,
                    this.parent.model.viewState.MenuLevels.FirstOrDefault(),
                    this.OwnerAsRadialMenuItemControl.Segment.LayoutSlot.StartAngle);
            }
        }

        /// <summary>
        /// IToggleProvider implementation.
        /// </summary>
        public void Toggle()
        {
            if (this.OwnerAsRadialMenuItemControl != null && this.OwnerAsRadialMenuItemControl.Segment != null
                && this.OwnerAsRadialMenuItemControl.Segment.TargetItem.Selectable)
            {
                this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected = !this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected;
            }
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void AddToSelection()
        {
            if (this.OwnerAsRadialMenuItemControl.Segment != null && this.OwnerAsRadialMenuItemControl.Segment.TargetItem.Selectable
                && !this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected)
            {
                this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected = true;
            }
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void RemoveFromSelection()
        {
            if (this.OwnerAsRadialMenuItemControl.Segment != null && this.OwnerAsRadialMenuItemControl.Segment.TargetItem.Selectable
                 && this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected)
            {
                this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected = false;
            }
        }

        /// <summary>
        /// ISelectionItemProvider implementation.
        /// </summary>
        public void Select()
        {
            var radialMenuModel = this.parent.model;
            if (radialMenuModel != null && radialMenuModel.contentRing != null
                && radialMenuModel.contentRing.Segments != null)
            {
                var radialMenuItems = radialMenuModel.contentRing.Segments.OfType<RadialSegment>();
                if (radialMenuItems != null)
                {
                    foreach (var item in radialMenuItems)
                    {
                        if (item.TargetItem != null && item.TargetItem.IsSelected)
                        {
                            item.TargetItem.IsSelected = !item.TargetItem.IsSelected;
                        }
                    }
                }
            }

            this.AddToSelection();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseToggleStatePropertyChangedEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(
                TogglePatternIdentifiers.ToggleStateProperty,
                oldValue ? ToggleState.On : ToggleState.Off,
                newValue ? ToggleState.On : ToggleState.Off);
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke || patternInterface == PatternInterface.Toggle
                || patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.Menu.RadialMenuItemControl);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.Menu.RadialMenuItemControl);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radial menu item control";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore) && this.OwnerAsRadialMenuItemControl.Header != null
                && this.OwnerAsRadialMenuItemControl.Header is string)
            {
                nameCore = this.OwnerAsRadialMenuItemControl.Header.ToString();
            }

            return nameCore;
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(Telerik.UI.Xaml.Controls.Primitives.Menu.RadialMenuItemControl);
        }
    }
}
