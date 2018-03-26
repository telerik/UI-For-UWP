using System;
using System.Collections.Generic;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class XamlAppointmentLayer : CalendarLayer
    {
        internal Canvas appointmentPanel;
        internal List<AppointmentControl> realizedCalendarCellDefaultPresenters;

        internal XamlAppointmentLayer()
        {
            this.appointmentPanel = new Canvas();
            this.appointmentPanel.Background = new SolidColorBrush(Colors.Transparent);

            this.realizedCalendarCellDefaultPresenters = new List<AppointmentControl>();
        }

        protected internal override Windows.UI.Xaml.UIElement VisualElement
        {
            get
            {
                return this.appointmentPanel;
            }
        }

        internal void UpdateUI(IEnumerable<CalendarCellModel> cellsToUpdate = null)
        {
            if (this.Owner.AppointmentSource == null)
            {
                return;
            }
            if (cellsToUpdate == null)
            {
                cellsToUpdate = this.Owner.Model.CalendarCells;
            }

            int index = 0;
            RadCalendar calendar = this.Owner;
            foreach (CalendarCellModel cell in cellsToUpdate)
            {
                CalendarAppointmentInfo info = new CalendarAppointmentInfo();
                info.Date = cell.Date;
                info.Appointments = this.Owner.AppointmentSource.GetAppointments((IAppointment appointment) =>
                {
                    return cell.Date.Date >= appointment.StartDate.Date && cell.Date.Date <= appointment.EndDate.Date;
                });

                if (info.Appointments.Count > 0)
                {
                    AppointmentControl element = new AppointmentControl();

                    foreach (var appointment in info.Appointments)
                    {
                        info.Subject += (info.Subject != null ? Environment.NewLine : string.Empty) + appointment.Subject;
                    }

                    element = this.GetDefaultVisual(index);
                    element.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, cell.LayoutSlot.Width, cell.LayoutSlot.Height) };
                    element.Header = info.Subject;
                    element.Background = info.Brush;
                    element.appointmentInfo = info;

                    XamlContentLayerHelper.MeasureVisual(element);
                    if (element != null)
                    {
                        StyleSelector styleSelector = calendar.AppointmentStyleSelector;
                        if (styleSelector != null)
                        {
                            var style = styleSelector.SelectStyle(info, element);
                            if (style != null)
                            {
                                element.Style = style;
                            }
                        }

                        AppointmentTemplateSelector headerTemplateSelector = calendar.AppointmentHeaderTemplateSelector;
                        if (headerTemplateSelector != null)
                        {
                            DataTemplate template = headerTemplateSelector.SelectTemplate(info, info.cell);
                            if (template != null)
                            {
                                element.HeaderTemplate = template;
                            }
                        }

                        RadRect layoutSlot = cell.layoutSlot;
                        layoutSlot = XamlContentLayerHelper.ApplyLayoutSlotAlignment(element, layoutSlot);
                        XamlContentLayer.ArrangeUIElement(element, layoutSlot, false);

                        index++;
                    }
                }
            }

            while (index < this.realizedCalendarCellDefaultPresenters.Count)
            {
                this.realizedCalendarCellDefaultPresenters[index].Visibility = Visibility.Collapsed;
                index++;
            }
        }

        protected internal override void AddVisualChild(UIElement child)
        {
            if (this.appointmentPanel != null)
            {
                this.appointmentPanel.Children.Add(child);
            }
        }

        protected internal override void RemoveVisualChild(UIElement child)
        {
            if (this.appointmentPanel != null)
            {
                this.appointmentPanel.Children.Remove(child);
            }
        }

        private AppointmentControl GetDefaultVisual(int virtualIndex)
        {
            AppointmentControl visual;

            if (virtualIndex < this.realizedCalendarCellDefaultPresenters.Count)
            {
                visual = this.realizedCalendarCellDefaultPresenters[virtualIndex];
                visual.ClearValue(AppointmentControl.VisibilityProperty);
                visual.ClearValue(AppointmentControl.ContentTemplateProperty);
                visual.ClearValue(AppointmentControl.HeaderTemplateProperty);
                visual.ClearValue(AppointmentControl.StyleProperty);
            }
            else
            {
                visual = this.CreateDefaultVisual();
            }

            return visual;
        }

        private AppointmentControl CreateDefaultVisual()
        {
            AppointmentControl appointmentControl = new AppointmentControl();

            this.realizedCalendarCellDefaultPresenters.Add(appointmentControl);
            this.AddVisualChild(appointmentControl);

            return appointmentControl;
        }
    }
}