using System.Linq;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for map shapes.
    /// </summary>
    public class MapShapeAutomationPeer : AutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapShapeAutomationPeer"/> class.
        /// </summary>
        /// <param name="layerPeer">The parent shape layer.</param>
        /// <param name="shapeModel">The underlying shape model.</param>
        public MapShapeAutomationPeer(MapShapeLayerAutomationPeer layerPeer, IMapShape shapeModel) : base()
        {
            this.LayerPeer = layerPeer;
            this.ShapeModel = shapeModel;
        }

        /// <summary>
        /// Gets a value indicating whether this shape is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.LayerPeer.SelectionBehavior.SelectedShapes.ToList().Contains(this.ShapeModel);
            }
        }

        /// <summary>
        /// Gets the parent ISelectionProvider performing the selection.
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                return this.ProviderFromPeer(this.LayerPeer);
            }
        }

        private MapShapeLayerAutomationPeer LayerPeer
        {
            get;
            set;
        }

        private IMapShape ShapeModel
        {
            get;
            set;
        }

        /// <summary>
        /// Selects the shape.
        /// </summary>
        public void AddToSelection()
        {
            this.Select();
        }

        /// <summary>
        /// Removes the shape from selection.
        /// </summary>
        public void RemoveFromSelection()
        {
            if (this.ShapeModel is MapShapeModel)
            {
                this.LayerPeer.SelectionBehavior.DeselectShape(this.ShapeModel as MapShapeModel);
                this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, true, false);
            }
        }

        /// <summary>
        /// Selects the shape.
        /// </summary>
        public void Select()
        {
            if (this.LayerPeer.SelectionBehavior != null)
            {
                D2DShape shape = this.LayerPeer.LayerOwner.Owner.FindShapeForModel(this.ShapeModel);
                if (shape != null)
                {
                    this.LayerPeer.SelectionBehavior.PerformSelection(shape);
                    this.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, false, true);
                }
            }
        }

        /// <inheritdoc/>
        protected override string GetLocalizedControlTypeCore()
        {
            return "map shape";
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            MapShapeModel model = this.ShapeModel as MapShapeModel;
            if (model != null)
            {
                string layerLabelAttribute = this.LayerPeer.LayerOwner.ShapeLabelAttributeName;
                if (!string.IsNullOrEmpty(layerLabelAttribute))
                {
                    if (model.Attributes[layerLabelAttribute] != null)
                    {
                        return model.Attributes[layerLabelAttribute].ToString();
                    }
                }                
                return model.UniqueId.ToString();
            }
            return this.GetHashCode().ToString();
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataItem;
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override string GetAcceleratorKeyCore()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetAccessKeyCore()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetItemTypeCore()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        protected override AutomationPeer GetLabeledByCore()
        {
            return null;
        }

        /// <inheritdoc />
        protected override AutomationOrientation GetOrientationCore()
        {
            return AutomationOrientation.None;
        }

        /// <inheritdoc />
        protected override bool IsContentElementCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool IsControlElementCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool IsEnabledCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool IsOffscreenCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override bool IsPasswordCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override bool IsRequiredForFormCore()
        {
            return false;
        }
    }
}
