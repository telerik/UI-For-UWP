using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class XamlAllDayAreaLayer : CalendarLayer
    {
        internal bool shouldArrange;

        private static SolidColorBrush DefaultBackground = new SolidColorBrush(Colors.Transparent);

        private ScrollViewer allDayAreaScrollViewer;
        private Canvas allDayAreaPanel;
        private RadRect allDayClipArea;
        private List<AppointmentControl> realizedAllDayAppointmentDefaultPresenters;

        public XamlAllDayAreaLayer()
        {
            this.allDayAreaPanel = new Canvas();
            this.allDayAreaScrollViewer = new ScrollViewer();
            this.allDayAreaScrollViewer.Content = this.allDayAreaPanel;

            this.realizedAllDayAppointmentDefaultPresenters = new List<AppointmentControl>();
        }

        protected internal override UIElement VisualElement
        {
            get
            {
                return this.allDayAreaScrollViewer;
            }
        }

        internal void UpdateAllDayAreaUI()
        {
            if (this.shouldArrange)
            {
                this.ArrangeVisualElement();
                this.shouldArrange = false;
            }

            var model = this.Owner.Model.multiDayViewModel;
            List<CalendarAppointmentInfo> allDayAppointmentInfos = model.allDayAppointmentInfos;
            if (allDayAppointmentInfos == null || allDayAppointmentInfos.Count == 0)
            {
                this.allDayAreaScrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.allDayAreaScrollViewer.ClearValue(FrameworkElement.VisibilityProperty);

                this.allDayClipArea = new RadRect(this.allDayAreaScrollViewer.HorizontalOffset, this.allDayAreaScrollViewer.VerticalOffset, this.allDayAreaScrollViewer.Width, model.totalAllDayAreaHeight);
                this.UpdateAllDayAppointments(allDayAppointmentInfos);
            }
        }

        internal void RecycleAppointments()
        {
            foreach (AppointmentControl appControl in this.realizedAllDayAppointmentDefaultPresenters)
            {
                appControl.Visibility = Visibility.Collapsed;
            }
        }

        internal void ArrangeVisualElement()
        {
            RadCalendar calendar = this.Owner;
            CalendarMultiDayViewModel multiDayViewModel = calendar.Model.multiDayViewModel;

            if (multiDayViewModel.allDayAppointmentInfos != null && multiDayViewModel.allDayAppointmentInfos.Count > 0)
            {
                Size availableCalendarViewSize = calendar.CalendarViewSize;

                double timeRulerWidth = multiDayViewModel.timeRulerWidth;
                double totalWidth = (availableCalendarViewSize.Width - timeRulerWidth) * 2;
                double allDayAreaHeight = multiDayViewModel.totalAllDayAreaHeight;

                this.allDayAreaScrollViewer.Width = 2 * totalWidth;
                this.allDayAreaScrollViewer.Height = allDayAreaHeight;

                int allDayAreaRowCount = multiDayViewModel.allDayAreaRowCount;
                this.allDayAreaPanel.Height = multiDayViewModel.allDayAppointmentInfos.First().layoutSlot.Bottom * allDayAreaRowCount
                    + (allDayAreaRowCount - 1) * calendar.MultiDayViewSettings.AllDayAppointmentSpacing;

                double cellHeight = multiDayViewModel.dayViewLayoutSlot.Height / multiDayViewModel.SpecificColumnCount;
                double topOffset = Math.Abs(cellHeight + multiDayViewModel.dayViewLayoutSlot.Y + calendar.GridLinesThickness);
                Canvas.SetTop(this.allDayAreaScrollViewer, topOffset);
                this.UpdatePanelBackground(calendar.MultiDayViewSettings.AllDayAreaBackground);
            }
            else
            {
                this.allDayAreaPanel.Height = 0;
                this.allDayAreaScrollViewer.Width = 0;
                this.allDayAreaScrollViewer.Height = 0;
            }
        }

        internal void UpdatePanelBackground(Brush background)
        {
            if (this.allDayAreaPanel.Background != background)
            {
                if (background != null)
                {
                    this.allDayAreaPanel.Background = background;
                }
                else
                {
                    this.allDayAreaPanel.Background = XamlAllDayAreaLayer.DefaultBackground;
                }
            }
        }

        protected internal override void DetachUI(Panel parent)
        {
            base.DetachUI(parent);
            this.allDayAreaScrollViewer.ViewChanged -= this.OnAllDayAreaScrollViewerViewChanged;
        }

        protected internal override void AttachUI(Panel parent)
        {
            base.AttachUI(parent);
            this.allDayAreaScrollViewer.ViewChanged += this.OnAllDayAreaScrollViewerViewChanged;
        }

        protected internal override void AddVisualChild(UIElement child)
        {
            this.allDayAreaPanel.Children.Add(child);
        }

        protected internal override void RemoveVisualChild(UIElement child)
        {
            this.allDayAreaPanel.Children.Remove(child);
        }

        private void UpdateAllDayAppointments(List<CalendarAppointmentInfo> allDayAppointmentInfos)
        {
            int index = 0;
            RadCalendar calendar = this.Owner;
            foreach (var appInfo in allDayAppointmentInfos)
            {
                if (!this.allDayClipArea.IntersectsWith(appInfo.layoutSlot))
                {
                    continue;
                }

                AppointmentControl appointmentControl = this.GetDefaultAllDayAppointmentVisual(index);
                if (appointmentControl != null)
                {
                    RadRect layoutSlot = appInfo.layoutSlot;
                    appointmentControl.Header = appInfo.Subject;
                    appointmentControl.Background = appInfo.Brush;
                    appointmentControl.appointmentInfo = appInfo;

                    StyleSelector styleSelector = calendar.AppointmentStyleSelector;
                    if (styleSelector != null)
                    {
                        var style = styleSelector.SelectStyle(appInfo, appointmentControl);
                        if (style != null)
                        {
                            appointmentControl.Style = style;
                        }
                    }
                    else if (appointmentControl.Style != null)
                    {
                        appointmentControl.ClearValue(AppointmentControl.StyleProperty);
                    }

                    AppointmentTemplateSelector headerTemplateSelector = calendar.AppointmentHeaderTemplateSelector;
                    if (headerTemplateSelector != null)
                    {
                        DataTemplate template = headerTemplateSelector.SelectTemplate(appInfo, appInfo.cell);
                        if (template != null)
                        {
                            appointmentControl.HeaderTemplate = template;
                        }
                    }

                    XamlContentLayer.ArrangeUIElement(appointmentControl, layoutSlot, true);
                    index++;
                }
            }

            while (index < this.realizedAllDayAppointmentDefaultPresenters.Count)
            {
                this.realizedAllDayAppointmentDefaultPresenters[index].Visibility = Visibility.Collapsed;
                index++;
            }
        }

        private AppointmentControl GetDefaultAllDayAppointmentVisual(int virtualIndex)
        {
            AppointmentControl visual;

            if (virtualIndex < this.realizedAllDayAppointmentDefaultPresenters.Count)
            {
                visual = this.realizedAllDayAppointmentDefaultPresenters[virtualIndex];
                visual.ClearValue(AppointmentControl.VisibilityProperty);
                visual.ClearValue(Canvas.ZIndexProperty);
                visual.ClearValue(Canvas.LeftProperty);
            }
            else
            {
                visual = this.CreateDefaultAllDayAppointmentVisual();
            }

            return visual;
        }

        private AppointmentControl CreateDefaultAllDayAppointmentVisual()
        {
            AppointmentControl appointmentControl = new AppointmentControl();
            appointmentControl.calendar = this.Owner;
            this.realizedAllDayAppointmentDefaultPresenters.Add(appointmentControl);
            this.AddVisualChild(appointmentControl);

            return appointmentControl;
        }

        private void OnAllDayAreaScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate)
            {
                this.UpdateAllDayAreaUI();
            }
        }
    }
}