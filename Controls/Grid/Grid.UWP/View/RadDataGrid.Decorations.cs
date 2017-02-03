using Telerik.UI.Xaml.Controls.Grid.View;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        /// <summary>
        /// Identifies the <see cref="GridLinesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register(nameof(GridLinesVisibility), typeof(GridLinesVisibility), typeof(RadDataGrid), new PropertyMetadata(GridLinesVisibility.Both, OnGridLinesVisibilityChanged));

        /// <summary>
        /// Identifies the <see cref="GridLinesThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesThicknessProperty =
            DependencyProperty.Register(nameof(GridLinesThickness), typeof(double), typeof(RadDataGrid), new PropertyMetadata(2d, OnGridLinesThicknessChanged));

        /// <summary>
        /// Identifies the <see cref="GridLinesBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register(nameof(GridLinesBrush), typeof(Brush), typeof(RadDataGrid), new PropertyMetadata(null, OnGridLinesBrushChanged));

        /// <summary>
        /// Identifies the <see cref="AlternateRowBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register(nameof(RowBackground), typeof(Brush), typeof(RadDataGrid), new PropertyMetadata(null, OnRowBackgroundChanged));

        /// <summary>
        /// Identifies the <see cref="RowBackgroundSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowBackgroundSelectorProperty =
            DependencyProperty.Register(nameof(RowBackgroundSelector), typeof(ObjectSelector<Brush>), typeof(RadDataGrid), new PropertyMetadata(null, OnRowBackgroundSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="AlternateRowBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternateRowBackgroundProperty =
            DependencyProperty.Register(nameof(AlternateRowBackground), typeof(Brush), typeof(RadDataGrid), new PropertyMetadata(null, OnAlternateRowBackgroundChanged));

        /// <summary>
        /// Identifies the <see cref="AlternationStep"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternationStepProperty =
            DependencyProperty.Register(nameof(AlternationStep), typeof(int), typeof(RadDataGrid), new PropertyMetadata(0, OnAlternationStepChanged));

        /// <summary>
        /// Identifies the <see cref="AlternationStartIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternationStartIndexProperty =
            DependencyProperty.Register(nameof(AlternationStartIndex), typeof(int), typeof(RadDataGrid), new PropertyMetadata(0, OnAlternationStartIndexChanged));

        private double gridLinesThicknessCache = 2d;
        private GridLinesVisibility gridLinesVisibilityCache = GridLinesVisibility.Both;
        private Brush gridLinesBrushCache;
        private Brush alternateRowBackgroundCache;
        private Brush rowBackgroundCache;
        private ObjectSelector<Brush> rowBackgroundSelectorCache;
        private int alternationStepCache;
        private int alternationStartIndexCache;

        private LineDecorationPresenter lineDecorationsPresenter;
        private LineDecorationPresenter frozenLineDecorationsPresenter;

        private SelectionDecorationPresenter selectionDecorationsPresenter;
        private SelectionDecorationPresenter frozenSelectionDecorationsPresenter;

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that defines the fill of each row.
        /// </summary>
        public Brush RowBackground
        {
            get
            {
                return this.rowBackgroundCache;
            }
            set
            {
                this.SetValue(RowBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ObjectSelector{Brush}"/> that may be used to provide conditional fill for grid rows.
        /// </summary>
        public ObjectSelector<Brush> RowBackgroundSelector
        {
            get
            {
                return this.rowBackgroundSelectorCache;
            }
            set
            {
                this.SetValue(RowBackgroundSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that defines the fill of the alternate rows, as defined by the <see cref="AlternationStep"/> property.
        /// </summary>
        public Brush AlternateRowBackground
        {
            get
            {
                return this.alternateRowBackgroundCache;
            }
            set
            {
                this.SetValue(AlternateRowBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the step between each two alternate rows. The Modulus (%) operand is applied over this value.
        /// </summary>
        public int AlternationStep
        {
            get
            {
                return this.alternationStepCache;
            }
            set
            {
                this.SetValue(AlternationStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the row which is considered as alternation start.
        /// </summary>
        public int AlternationStartIndex
        {
            get
            {
                return this.alternationStartIndexCache;
            }
            set
            {
                this.SetValue(AlternationStartIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> value that defines the appearance of grid's horizontal lines.
        /// </summary>
        public Brush GridLinesBrush
        {
            get
            {
                return this.gridLinesBrushCache;
            }
            set
            {
                this.SetValue(GridLinesBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="GridLinesVisibility"/> value that defines which grid lines are currently visible (displayed).
        /// </summary>
        public GridLinesVisibility GridLinesVisibility
        {
            get
            {
                return this.gridLinesVisibilityCache;
            }
            set
            {
                this.SetValue(GridLinesVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the vertical grid lines and the height of the horizontal grid lines.
        /// </summary>
        public double GridLinesThickness
        {
            get
            {
                return this.gridLinesThicknessCache;
            }
            set
            {
                this.SetValue(GridLinesThicknessProperty, value);
            }
        }

        IDecorationPresenter<LineDecorationModel> IGridView.LineDecorationsPresenter
        {
            get
            {
                return this.lineDecorationsPresenter;
            }
        }

        IDecorationPresenter<SelectionRegionModel> IGridView.SelectionDecorationsPresenter
        {
            get
            {
                return this.selectionDecorationsPresenter;
            }
        }

        IDecorationPresenter<LineDecorationModel> IGridView.FrozenLineDecorationsPresenter
        {
            get
            {
                return this.frozenLineDecorationsPresenter;
            }
        }

        IDecorationPresenter<SelectionRegionModel> IGridView.FrozenSelectionDecorationsPresenter
        {
            get
            {
                return this.frozenSelectionDecorationsPresenter;
            }
        }

        internal bool HasHorizontalGridLines
        {
            get
            {
                return (this.gridLinesVisibilityCache & GridLinesVisibility.Horizontal) == GridLinesVisibility.Horizontal;
            }
        }

        internal bool HasVerticalGridLines
        {
            get
            {
                return (this.gridLinesVisibilityCache & GridLinesVisibility.Vertical) == GridLinesVisibility.Vertical;
            }
        }

        private static void OnGridLinesVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.gridLinesVisibilityCache = (GridLinesVisibility)e.NewValue;

            // the grid lines visibility will need cells re-arrange
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnGridLinesThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.gridLinesThicknessCache = (double)e.NewValue;

            // the grid lines thickness will need cells re-arrange
            grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
        }

        private static void OnGridLinesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.gridLinesBrushCache = e.NewValue as Brush;

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);
        }

        private static void OnRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.rowBackgroundCache = e.NewValue as Brush;

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);
        }

        private static void OnRowBackgroundSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.rowBackgroundSelectorCache = e.NewValue as ObjectSelector<Brush>;

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);
        }

        private static void OnAlternateRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.alternateRowBackgroundCache = e.NewValue as Brush;

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);
        }

        private static void OnAlternationStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.alternationStepCache = (int)e.NewValue;

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);
        }

        private static void OnAlternationStartIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.alternationStartIndexCache = (int)e.NewValue;

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);
        }
    }
}
