using System;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Encapsulates the parameters needed to evaluate a <see cref="LoopingListItem"/> visual state.
    /// </summary>
    internal struct VisualStateUpdateParams
    {
        private bool evaluateEnabled;
        private bool evaluateSelected;
        private bool animate;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateUpdateParams"/> struct.
        /// </summary>
        /// <param name="animate">True to use transitions if visual state changes, false otherwise.</param>
        public VisualStateUpdateParams(bool animate)
        {
            this.animate = animate;
            this.evaluateEnabled = false;
            this.evaluateSelected = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateUpdateParams"/> struct.
        /// </summary>
        /// <param name="animate">True to use transitions if visual state changes, false otherwise.</param>
        /// <param name="evaluateEnabled">True to evaluate the IsEnabled property of the visual item, false otherwise.</param>
        public VisualStateUpdateParams(bool animate, bool evaluateEnabled)
        {
            this.animate = animate;
            this.evaluateEnabled = evaluateEnabled;
            this.evaluateSelected = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateUpdateParams"/> struct.
        /// </summary>
        /// <param name="animate">True to use transitions if visual state changes, false otherwise.</param>
        /// <param name="evaluateEnabled">True to evaluate the IsEnabled property of the visual item, false otherwise.</param>
        /// <param name="evaluateSelected">True to evaluate the IsSelected property of the visual item, false otherwise.</param>
        public VisualStateUpdateParams(bool animate, bool evaluateEnabled, bool evaluateSelected)
        {
            this.animate = animate;
            this.evaluateEnabled = evaluateEnabled;
            this.evaluateSelected = evaluateSelected;
        }

        /// <summary>
        /// Gets or sets a value indicating whether VisualStateManager will use transitions upon visual state change.
        /// </summary>
        public bool Animate
        {
            get
            {
                return this.animate;
            }
            set
            {
                this.animate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsEnabled property will be evaluated.
        /// </summary>
        public bool EvaluateEnabled
        {
            get
            {
                return this.evaluateEnabled;
            }
            set
            {
                this.evaluateEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsSelected property will be evaluated.
        /// </summary>
        public bool EvaluateSelected
        {
            get
            {
                return this.evaluateSelected;
            }
            set
            {
                this.evaluateSelected = value;
            }
        }
    }
}
