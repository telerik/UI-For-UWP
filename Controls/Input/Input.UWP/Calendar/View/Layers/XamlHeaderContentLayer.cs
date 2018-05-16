using System.Collections.Generic;
using Telerik.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class XamlHeaderContentLayer : CalendarLayer
    {
        internal List<TextBlock> realizedCalendarCellDefaultPresenters;

        private TextBlock measurementPresenter;
        private Canvas contentPanel;

        public XamlHeaderContentLayer()
        {
            this.contentPanel = new Canvas();

            this.realizedCalendarCellDefaultPresenters = new List<TextBlock>();
        }

        internal Panel VisualContainer
        {
            get
            {
                return this.contentPanel;
            }
        }

        protected internal override UIElement VisualElement
        {
            get
            {
                return this.contentPanel;
            }
        }

        internal void UpdateUI()
        {
            if (this.Owner.Model.CalendarHeaderCells == null)
            {
                this.VisualElement.Visibility = Visibility.Collapsed;
                return;
            }

            this.VisualElement.ClearValue(FrameworkElement.VisibilityProperty);

            int index = 0;
            foreach (CalendarHeaderCellModel cell in this.Owner.Model.CalendarHeaderCells)
            {
                if (!cell.layoutSlot.IsSizeValid())
                {
                    continue;
                }

                FrameworkElement element = this.GetDefaultVisual(cell, index);

                if (element != null)
                {
                    RadRect layoutSlot = cell.layoutSlot;
                    layoutSlot = XamlContentLayerHelper.ApplyLayoutSlotAlignment(element, layoutSlot);
                    XamlContentLayer.ArrangeUIElement(element, layoutSlot, false);

                    index++;
                }
            }

            while (index < this.realizedCalendarCellDefaultPresenters.Count)
            {
                this.realizedCalendarCellDefaultPresenters[index].Visibility = Visibility.Collapsed;
                index++;
            }
        }

        internal RadSize MeasureContent(object owner, object content)
        {
            this.EnsureMeasurementPresenter();

            if (owner is CalendarHeaderCellType)
            {
                CalendarHeaderCellType headerType = (CalendarHeaderCellType)owner;
                if (headerType == CalendarHeaderCellType.DayName)
                {
                    if (this.Owner.DayNameCellStyleSelector != null)
                    {
                        this.measurementPresenter.Style = this.Owner.DayNameCellStyleSelector.SelectStyle(new CalendarCellStyleContext() { Label = content.ToString() }, this.Owner);
                    }
                    else
                    {
                        this.measurementPresenter.Style = this.Owner.DayNameCellStyle != null 
                            ? this.Owner.DayNameCellStyle.ContentStyle 
                            : this.Owner.defaultDayNameCellStyle.ContentStyle;
                    }
                }
                else
                {
                    if (this.Owner.WeekNumberCellStyleSelector != null)
                    {
                        this.measurementPresenter.Style = this.Owner.WeekNumberCellStyleSelector.SelectStyle(new CalendarCellStyleContext() { Label = content.ToString() }, this.Owner);
                    }
                    else
                    {
                        this.measurementPresenter.Style = this.Owner.WeekNumberCellStyle.ContentStyle;
                    }
                }

                this.measurementPresenter.Text = content.ToString();

                return XamlContentLayerHelper.MeasureVisual(this.measurementPresenter);
            }

            return RadSize.Empty;
        }

        private static void ApplyStyleToDefaultVisual(TextBlock visual, CalendarCellModel cell)
        {
            Style cellStyle = cell.Context.GetEffectiveCellContentStyle();
            if (cellStyle == null)
            {
                return;
            }

            visual.Style = cellStyle;
        }

        private FrameworkElement GetDefaultVisual(CalendarNode cell, int virtualIndex)
        {
            TextBlock visual;

            if (virtualIndex < this.realizedCalendarCellDefaultPresenters.Count)
            {
                visual = this.realizedCalendarCellDefaultPresenters[virtualIndex];

                visual.ClearValue(TextBlock.VisibilityProperty);
                visual.ClearValue(TextBlock.StyleProperty);
            }
            else
            {
                visual = this.CreateDefaultVisual();
            }

            XamlContentLayerHelper.PrepareDefaultVisual(visual, cell);

            return visual;
        }

        private TextBlock CreateDefaultVisual()
        {
            TextBlock textBlock = new TextBlock();

            this.AddVisualChild(textBlock);
            this.realizedCalendarCellDefaultPresenters.Add(textBlock);

            return textBlock;
        }

        private void EnsureMeasurementPresenter()
        {
            if (this.measurementPresenter == null)
            {
                this.measurementPresenter = new TextBlock();
                this.measurementPresenter.Opacity = 0;
                this.measurementPresenter.IsHitTestVisible = false;

                this.AddVisualChild(this.measurementPresenter);
            }
        }
    }
}
