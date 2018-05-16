using System.Collections.Generic;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class XamlDecorationLayer : CalendarLayer
    {
        private const int DefaultGridLineDecorationZIndex = -1;

        private Canvas decorationPanel;
        private Dictionary<CalendarNode, Border> realizedCalendarCellDecorationPresenters;
        private Queue<Border> recycledContainers;
        private Border pointerOverCellDecorationVisual, currentCellDecorationVisual;

        private RadRect dayNamesLineSlot, weekNumbersLineSlot;
        private CalendarHeaderCellModel dayNamesLineModel, weekNumbersLineModel;

        public XamlDecorationLayer()
        {
            this.decorationPanel = new Canvas();

            this.recycledContainers = new Queue<Border>();
            this.realizedCalendarCellDecorationPresenters = new Dictionary<CalendarNode, Border>();

            // We need dictionary keys for the additional line decorations we are drawing for the day names and week numbers;
            this.dayNamesLineModel = new CalendarHeaderCellModel() { Type = CalendarHeaderCellType.DayName };
            this.weekNumbersLineModel = new CalendarHeaderCellModel() { Type = CalendarHeaderCellType.WeekNumber };
        }

        internal Panel VisualContainer
        {
            get
            {
                return this.decorationPanel;
            }
        }

        protected internal override UIElement VisualElement
        {
            get
            {
                return this.decorationPanel;
            }
        }

        internal static bool IsStrokeThicknessExplicitlySet(Style style)
        {
            if (style != null)
            {
                foreach (Setter setter in style.Setters)
                {
                    if (setter.Property == Border.BorderThicknessProperty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool IsStrokeBrushExplicitlySet(Style style)
        {
            if (style != null)
            {
                foreach (Setter setter in style.Setters)
                {
                    if (setter.Property == Border.BorderBrushProperty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal void UpdateUI(IEnumerable<CalendarCellModel> cellsToUpdate = null)
        {
            bool fullUpdate = false;
            if (cellsToUpdate == null)
            {
                cellsToUpdate = this.Owner.Model.CalendarCells;
                fullUpdate = true;
            }

            if (fullUpdate)
            {
                this.RecycleAllVisuals();

                this.UpdateCalendarHeaderGridLineDecorations();
                this.UpdateCalendarGridLineDecorations();
                this.UpdateCalendarHeaderCellDecorations();
            }
            else
            {
                this.RecycleVisuals(cellsToUpdate);
            }

            this.UpdateCalendarCellDecorations(cellsToUpdate);

            foreach (FrameworkElement visual in this.recycledContainers)
            {
                visual.Visibility = Visibility.Collapsed;
                visual.Tag = null;
            }
        }

        private static bool ShouldRenderPrimaryCellDecorationVisual(CalendarNode cell)
        {
            Style effectiveStyle = cell.Context.GetEffectiveCellDecorationStyle();

            return effectiveStyle != null;
        }

        private static void RecycleAdditionalVisual(Border visual)
        {
            visual.ClearValue(Border.StyleProperty);
            visual.ClearValue(Canvas.ZIndexProperty);

            visual.Visibility = Visibility.Collapsed;
            visual.Tag = null;
        }

        private void UpdateCalendarHeaderGridLineDecorations()
        {
            if (this.Owner.Model.CalendarHeaderCells == null)
            {
                this.dayNamesLineSlot = RadRect.Empty;
                this.weekNumbersLineSlot = RadRect.Empty;

                return;
            }

            if (this.Owner.DisplayMode != CalendarDisplayMode.MultiDayView && this.Owner.DayNamesVisibility == Visibility.Visible)
            {
                Border dayNamesBottomDecorationLine = this.GetCalendarDecorationVisual(this.dayNamesLineModel, CalendarDecorationType.GridLine);
                if (dayNamesBottomDecorationLine != null)
                {
                    CalendarHeaderCellModel dayNameCell = this.GetFirstDayNameCell();
                    this.dayNamesLineSlot = new RadRect(0d, dayNameCell.LayoutSlot.Bottom - this.Owner.GridLinesThickness, this.Owner.CalendarViewSize.Width, this.Owner.GridLinesThickness);
                    CalendarLayer.ArrangeUIElement(dayNamesBottomDecorationLine, this.dayNamesLineSlot);
                    Canvas.SetZIndex(dayNamesBottomDecorationLine, DefaultGridLineDecorationZIndex);
                }
            }
            else
            {
                this.dayNamesLineSlot = RadRect.Empty;
            }

            if (this.Owner.DisplayMode != CalendarDisplayMode.MultiDayView && this.Owner.WeekNumbersVisibility == Visibility.Visible)
            {
                Border weekNumbersRightDecorationLine = this.GetCalendarDecorationVisual(this.weekNumbersLineModel, CalendarDecorationType.GridLine);
                if (weekNumbersRightDecorationLine != null)
                {
                    CalendarHeaderCellModel weekNumberCell = this.GetFirstWeekNumberCell();
                    this.weekNumbersLineSlot = new RadRect(weekNumberCell.LayoutSlot.Right - this.Owner.GridLinesThickness, 0d, this.Owner.GridLinesThickness, this.Owner.CalendarViewSize.Height);
                    CalendarLayer.ArrangeUIElement(weekNumbersRightDecorationLine, this.weekNumbersLineSlot);
                    Canvas.SetZIndex(weekNumbersRightDecorationLine, DefaultGridLineDecorationZIndex);
                }
            }
            else
            {
                this.weekNumbersLineSlot = RadRect.Empty;
            }
        }

        private void UpdateCalendarGridLineDecorations()
        {
            foreach (CalendarGridLine gridLine in this.Owner.Model.CalendarDecorations)
            {
                if (!gridLine.layoutSlot.IsSizeValid())
                {
                    continue;
                }

                FrameworkElement element = this.GetCalendarDecorationVisual(gridLine, CalendarDecorationType.GridLine);
                if (element != null)
                {
                    XamlDecorationLayer.ArrangeUIElement(element, gridLine.layoutSlot);
                    Canvas.SetZIndex(element, DefaultGridLineDecorationZIndex);
                }
            }
        }

        private void UpdateCalendarHeaderCellDecorations()
        {
            if (this.Owner.Model.CalendarHeaderCells == null)
            {
                return;
            }

            foreach (CalendarHeaderCellModel cell in this.Owner.Model.CalendarHeaderCells)
            {
                if (!cell.layoutSlot.IsSizeValid() || !ShouldRenderPrimaryCellDecorationVisual(cell))
                {
                    continue;
                }

                Border element = this.GetCalendarDecorationVisual(cell, CalendarDecorationType.Cell);
                if (element != null)
                {
                    RadRect decorationSlot = cell.layoutSlot;
                    decorationSlot = this.InflateCellDecorationHorizontally(decorationSlot, element);
                    decorationSlot = this.InflateCellDecorationVertically(decorationSlot, element);

                    CalendarLayer.ArrangeUIElement(element, decorationSlot);
                }
            }
        }

        private void UpdateCalendarCellDecorations(IEnumerable<CalendarCellModel> cellsToUpdate = null)
        {
            foreach (CalendarCellModel cell in cellsToUpdate)
            {
                this.UpdateCurrentCellDecoration(cell);
                this.UpdatePointerOverCellDecoration(cell);

                if (!ShouldRenderPrimaryCellDecorationVisual(cell))
                {
                    continue;
                }

                Border element = this.GetCalendarDecorationVisual(cell, CalendarDecorationType.Cell);
                this.ArrangeCellDecoration(element, cell.layoutSlot, cell.PrimaryDecorationZIndex);
            }
        }

        private void ArrangeCellDecoration(Border element, RadRect layoutSlot, int zIndex)
        {
            if (element != null)
            {
                RadRect decorationSlot = layoutSlot;
                decorationSlot = this.InflateCellDecorationHorizontally(decorationSlot, element);
                decorationSlot = this.InflateCellDecorationVertically(decorationSlot, element);

                XamlDecorationLayer.ArrangeUIElement(element, decorationSlot);
                Canvas.SetZIndex(element, zIndex);
            }
        }

        private void UpdateCurrentCellDecoration(CalendarCellModel cell)
        {
            if (!cell.IsCurrent || !this.Owner.ShouldRenderCurrentVisuals || this.Owner.CurrentCellStyle == null || this.Owner.CurrentCellStyle.DecorationStyle == null)
            {
                return;
            }

            if (this.currentCellDecorationVisual == null)
            {
                this.currentCellDecorationVisual = this.CreateDecorationVisual();
            }

            this.currentCellDecorationVisual.Tag = cell;
            this.currentCellDecorationVisual.ClearValue(Border.VisibilityProperty);

            this.ApplyStyleToCellDecorationVisual(this.currentCellDecorationVisual, this.Owner.CurrentCellStyle.DecorationStyle);

            this.ArrangeCellDecoration(this.currentCellDecorationVisual, cell.layoutSlot, cell.CurrentDecorationZIndex);
        }

        private void UpdatePointerOverCellDecoration(CalendarCellModel cell)
        {
            if (!cell.IsPointerOver || this.Owner.PointerOverCellStyle == null || this.Owner.PointerOverCellStyle.DecorationStyle == null)
            {
                return;
            }

            if (this.pointerOverCellDecorationVisual == null)
            {
                this.pointerOverCellDecorationVisual = this.CreateDecorationVisual();
            }

            this.pointerOverCellDecorationVisual.Tag = cell;
            this.pointerOverCellDecorationVisual.ClearValue(Border.VisibilityProperty);

            this.ApplyStyleToCellDecorationVisual(this.pointerOverCellDecorationVisual, this.Owner.PointerOverCellStyle.DecorationStyle);

            this.ArrangeCellDecoration(this.pointerOverCellDecorationVisual, cell.layoutSlot, cell.PointerOverDecorationZIndex);
        }

        private RadRect InflateCellDecorationVertically(RadRect layoutSlot, Border decoration)
        {
            if (decoration.BorderBrush == null)
            {
                return layoutSlot;
            }

            if (layoutSlot.Y < this.dayNamesLineSlot.Bottom)
            {
                layoutSlot.Y -= decoration.BorderThickness.Bottom;
                layoutSlot.Height += decoration.BorderThickness.Bottom;
            }
            else if (RadMath.AreClose(layoutSlot.Bottom, this.Owner.CalendarViewSize.Height))
            {
                layoutSlot.Y -= decoration.BorderThickness.Top;
                layoutSlot.Height += decoration.BorderThickness.Top + decoration.BorderThickness.Bottom;
            }
            else
            {
                layoutSlot.Y -= decoration.BorderThickness.Top;

                if (this.Owner.HasHorizontalGridLines)
                {
                    layoutSlot.Height += decoration.BorderThickness.Top + decoration.BorderThickness.Bottom;
                }
                else
                {
                    layoutSlot.Height += decoration.BorderThickness.Top;
                }
            }

            return layoutSlot;
        }

        private RadRect InflateCellDecorationHorizontally(RadRect layoutSlot, Border decoration)
        {
            if (decoration.BorderBrush == null)
            {
                return layoutSlot;
            }

            if (layoutSlot.X < this.weekNumbersLineSlot.Right)
            {
                layoutSlot.X -= decoration.BorderThickness.Right;
                layoutSlot.Width += decoration.BorderThickness.Right;
            }
            else if (RadMath.AreClose(layoutSlot.Right, this.Owner.CalendarViewSize.Width))
            {
                layoutSlot.X -= decoration.BorderThickness.Left;
                layoutSlot.Width += decoration.BorderThickness.Left + decoration.BorderThickness.Right;
            }
            else
            {
                layoutSlot.X -= decoration.BorderThickness.Left;

                if (this.Owner.HasVerticalGridLines)
                {
                    layoutSlot.Width += decoration.BorderThickness.Left + decoration.BorderThickness.Right;
                }
                else
                {
                    layoutSlot.Width += decoration.BorderThickness.Left;
                }
            }

            return layoutSlot;
        }

        private Border GetCalendarDecorationVisual(CalendarNode node, CalendarDecorationType decorationType)
        {
            Border visual;

            if (this.recycledContainers.Count > 0)
            {
                visual = this.recycledContainers.Dequeue();
                visual.ClearValue(Border.VisibilityProperty);

                this.realizedCalendarCellDecorationPresenters.Add(node, visual);
            }
            else
            {
                visual = this.CreateDecorationVisual();
                this.realizedCalendarCellDecorationPresenters.Add(node, visual);
            }

            this.PrepareDecorationVisual(visual, node, decorationType);

            return visual;
        }

        private void PrepareDecorationVisual(Border visual, CalendarNode node, CalendarDecorationType decorationType)
        {
            visual.Tag = node;

            if (decorationType == CalendarDecorationType.GridLine)
            {
                RadCalendar calendar = this.Owner;
                if (calendar.DisplayMode == CalendarDisplayMode.MultiDayView)
                {
                    MultiDayViewSettings settings = calendar.MultiDayViewSettings;
                    CalendarTimeRulerItemStyleSelector styleSelector = settings.TimeRulerItemStyleSelector ?? settings.defaultTimeRulerItemStyleSelector;
                    if (styleSelector != null)
                    {
                        Style style = styleSelector.SelectStyle(node, visual);
                        if (style != null)
                        {
                            visual.Style = style;

                            if (!XamlDecorationLayer.IsStrokeBrushExplicitlySet(visual.Style))
                            {
                                visual.BorderBrush = this.Owner.GridLinesBrush;
                            }

                            if (visual.BorderBrush != null && !XamlDecorationLayer.IsStrokeThicknessExplicitlySet(visual.Style))
                            {
                                visual.BorderThickness = new Thickness(this.Owner.GridLinesThickness);
                            }
                        }
                        else
                        {
                            visual.BorderBrush = this.Owner.GridLinesBrush;
                            visual.BorderThickness = new Thickness(this.Owner.GridLinesThickness);
                        }
                    }
                }
                else
                {
                    visual.BorderBrush = this.Owner.GridLinesBrush;
                    visual.BorderThickness = new Thickness(this.Owner.GridLinesThickness);
                }
            }
            else
            {
                this.ApplyStyleToCellDecorationVisual(visual, node);
            }
        }

        private void ApplyStyleToCellDecorationVisual(Border visual, CalendarNode cell)
        {
            if (cell == null)
            {
                return;
            }

            Style cellStyle = cell.Context.GetEffectiveCellDecorationStyle();
            this.ApplyStyleToCellDecorationVisual(visual, cellStyle);
        }

        private void ApplyStyleToCellDecorationVisual(Border visual, Style cellStyle)
        {
            if (cellStyle == null)
            {
                return;
            }

            visual.Style = cellStyle;

            if (visual.BorderBrush != null &&
                (!IsStrokeThicknessExplicitlySet(cellStyle) || this.Owner.GridLinesVisibility != GridLinesVisibility.None))
            {
                visual.BorderThickness = new Thickness(this.Owner.GridLinesThickness);
            }
        }

        private Border CreateDecorationVisual()
        {
            Border decoration = new Border();
            this.AddVisualChild(decoration);

            return decoration;
        }

        private CalendarHeaderCellModel GetFirstDayNameCell()
        {
            foreach (CalendarHeaderCellModel cell in this.Owner.Model.CalendarHeaderCells)
            {
                if (cell.Type == Calendar.CalendarHeaderCellType.DayName && cell.layoutSlot.IsSizeValid())
                {
                    return cell;
                }
            }

            return null;
        }

        private CalendarHeaderCellModel GetFirstWeekNumberCell()
        {
            foreach (CalendarHeaderCellModel cell in this.Owner.Model.CalendarHeaderCells)
            {
                if (cell.Type == Calendar.CalendarHeaderCellType.WeekNumber && cell.layoutSlot.IsSizeValid())
                {
                    return cell;
                }
            }

            return null;
        }

        private void RecycleAllVisuals()
        {
            foreach (Border visual in this.realizedCalendarCellDecorationPresenters.Values)
            {
                this.RecyclePrimaryVisual(visual);
            }

            this.realizedCalendarCellDecorationPresenters.Clear();

            if (this.currentCellDecorationVisual != null && this.currentCellDecorationVisual.Tag != null)
            {
                XamlDecorationLayer.RecycleAdditionalVisual(this.currentCellDecorationVisual);
            }

            if (this.pointerOverCellDecorationVisual != null && this.pointerOverCellDecorationVisual.Tag != null)
            {
                XamlDecorationLayer.RecycleAdditionalVisual(this.pointerOverCellDecorationVisual);
            }
        }

        private void RecycleVisuals(IEnumerable<CalendarCellModel> cellsToUpdate)
        {
            foreach (CalendarCellModel cell in cellsToUpdate)
            {
                this.RecyclePrimaryVisual(cell);

                if (this.currentCellDecorationVisual != null && this.currentCellDecorationVisual.Tag == cell)
                {
                    XamlDecorationLayer.RecycleAdditionalVisual(this.currentCellDecorationVisual);
                }

                if (this.pointerOverCellDecorationVisual != null && this.pointerOverCellDecorationVisual.Tag == cell)
                {
                    XamlDecorationLayer.RecycleAdditionalVisual(this.pointerOverCellDecorationVisual);
                }
            }
        }

        private void RecyclePrimaryVisual(CalendarNode node)
        {
            Border visual;

            if (this.realizedCalendarCellDecorationPresenters.TryGetValue(node, out visual))
            {
                this.realizedCalendarCellDecorationPresenters.Remove(node);

                this.RecyclePrimaryVisual(visual);
            }
        }

        private void RecyclePrimaryVisual(Border visual)
        {
            this.recycledContainers.Enqueue(visual);

            visual.ClearValue(Border.StyleProperty);
            visual.ClearValue(Canvas.ZIndexProperty);

            // NOTE: We are additionally clearing these properties for two reasons:
            // gridline styling and the "magic" compensation of the stroke thickness for cell decorations.
            visual.ClearValue(Border.BorderBrushProperty);
            visual.ClearValue(Border.BorderThicknessProperty);
        }
    }
}
