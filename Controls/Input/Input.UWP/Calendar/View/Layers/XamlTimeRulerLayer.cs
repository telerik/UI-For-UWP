using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class XamlTimeRulerLayer : CalendarLayer
    {
        internal Canvas contentPanel;
        internal Canvas topHeader;
        internal bool shouldArrange;

        private const int DefaultTopLeftHeaderZIndex = 1000;
        private const int DefaultToptHeaderZIndex = 500;
        private const int DefaultLeftHeaderZIndex = 750;
        private const int DefaultSlotZIndex = 250;
        private const int DefaultTodaySlotZIndex = 200;
        private const int DefaultLineZIndex = 500;
        private const int DefaultCurrentTimeIndicatorZIndex = 750;
        private const int DefaultAppointmentZIndex = 1000;
        private static SolidColorBrush DefaultBackground = new SolidColorBrush(Colors.White);

        private ScrollViewer scrollViewer;
        private Canvas leftHeaderPanel;
        private Canvas topLeftHeaderPanel;

        private double leftOffset;
        private RadRect viewPortArea;
        private RadRect bufferedViewPortArea;
        private TextBlock measurementPresenter;
        private Border currentTimeIndicatorBorder;
        private Border horizontalGridLineBorder;
        private Border verticalGridLineBorder;
        private Border todaySlotBorder;

        private Dictionary<CalendarTimeRulerItem, TextBlock> realizedTimerRulerItemsPresenters;
        private Dictionary<CalendarGridLine, Border> realizedTimerRulerLinePresenters;
        private Dictionary<Slot, Border> realizedSlotPresenters;

        private Queue<TextBlock> recycledTimeRulerItems;
        private Queue<Border> recycledTimeRulerLines;
        private Queue<Border> recycledSlots;
        private List<AppointmentControl> realizedAppointmentDefaultPresenters;

        private Point scrollMousePosition;
        private Storyboard offsetStoryboard;
        private DoubleAnimation offsetAnimation;
        private TranslateTransform translateTransform;
        private double prevHorizontalOffset;
        private double currHorizontalOffset;
        private bool isAnimationOngoing;

        public XamlTimeRulerLayer()
        {
            this.scrollViewer = new ScrollViewer();
            this.scrollViewer.HorizontalScrollMode = ScrollMode.Disabled;

            this.translateTransform = new TranslateTransform();

            this.contentPanel = new Canvas();
            this.contentPanel.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;

            this.contentPanel.RenderTransform = this.translateTransform;
            this.contentPanel.HorizontalAlignment = HorizontalAlignment.Left;
            this.contentPanel.VerticalAlignment = VerticalAlignment.Top;

            this.leftHeaderPanel = new Canvas();
            Canvas.SetZIndex(this.leftHeaderPanel, DefaultLeftHeaderZIndex);

            this.topLeftHeaderPanel = new Canvas();
            this.topLeftHeaderPanel.Background = XamlTimeRulerLayer.DefaultBackground;

            Canvas.SetZIndex(this.topLeftHeaderPanel, DefaultTopLeftHeaderZIndex);

            this.topHeader = new Canvas();
            this.topHeader.Background = XamlTimeRulerLayer.DefaultBackground;
            this.topHeader.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;
            Canvas.SetZIndex(this.topHeader, DefaultToptHeaderZIndex);

            this.scrollViewer.Content = this.contentPanel;
            this.scrollViewer.TopHeader = this.topHeader;
            this.scrollViewer.LeftHeader = this.leftHeaderPanel;
            this.scrollViewer.TopLeftHeader = this.topLeftHeaderPanel;

            this.realizedTimerRulerItemsPresenters = new Dictionary<CalendarTimeRulerItem, TextBlock>();
            this.realizedTimerRulerLinePresenters = new Dictionary<CalendarGridLine, Border>();
            this.realizedSlotPresenters = new Dictionary<Slot, Border>();

            this.recycledTimeRulerItems = new Queue<TextBlock>();
            this.recycledTimeRulerLines = new Queue<Border>();
            this.recycledSlots = new Queue<Border>();

            this.realizedAppointmentDefaultPresenters = new List<AppointmentControl>();

            this.CreateOffsetAnimation();
        }

        protected internal override UIElement VisualElement
        {
            get
            {
                return this.scrollViewer;
            }
        }

        internal RadSize MeasureContent(object content)
        {
            this.EnsureMeasurementPresenter();
            this.measurementPresenter.Text = content.ToString();

            return XamlContentLayerHelper.MeasureVisual(this.measurementPresenter);
        }

        internal async void UpdateUI()
        {
            if (this.shouldArrange)
            {
                this.ArrangeVisualElement();
                this.shouldArrange = false;
            }

            this.viewPortArea = new RadRect(this.scrollViewer.HorizontalOffset, this.scrollViewer.VerticalOffset, this.scrollViewer.Width, this.scrollViewer.Height);

            double xOfsset = this.scrollViewer.HorizontalOffset + this.leftHeaderPanel.Width;
            this.bufferedViewPortArea = new RadRect(xOfsset, this.scrollViewer.VerticalOffset, this.leftOffset * 3, this.scrollViewer.Height);

            CalendarModel model = this.Owner.Model;
            ElementCollection<CalendarTimeRulerItem> timeRulerItems = model.multiDayViewModel.timeRulerItems;
            ElementCollection<CalendarGridLine> timeRulerLines = model.multiDayViewModel.timerRulerLines;
            List<CalendarAppointmentInfo> appointmentInfos = model.multiDayViewModel.appointmentInfos;
            IEnumerable<Slot> slots = model.multiDayViewSettings.SpecialSlotsSource;

            this.UpdateTimeRulerDecorations(model.multiDayViewModel.horizontalRulerGridLine, model.multiDayViewModel.verticalRulerGridLine);
            this.UpdateTimeRulerItems(timeRulerItems);
            this.UpdateTimerRulerLines(timeRulerLines);
            this.UpdateAppointments(appointmentInfos);
            if (slots != null)
            {
                this.UpdateSlots(slots);
            }

            this.UpdateTodaySlot();
            this.UpdateCurrentTimeIndicator();

            // If there is a pending Appointment that needs to be scrolled we are dispatching it.
            IAppointment pendingToScrollApp = this.Owner.pendingScrollToAppointment;
            if (pendingToScrollApp != null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                    () => 
                    {
                        this.ScrollAppointmentIntoView(pendingToScrollApp);
                        this.Owner.pendingScrollToAppointment = null;
                    });
            }
        }

        internal void ScrollAppointmentIntoView(IAppointment appointment)
        {
            RadCalendar calendar = this.Owner;
            if (calendar != null)
            {
                var appInfos = calendar.Model.multiDayViewModel.appointmentInfos;
                if (appInfos != null && appInfos.Count > 0)
                {
                    DateTime startDate = calendar.DisplayDate;
                    DateTime endDate = calendar.DisplayDate.AddDays(calendar.MultiDayViewSettings.WeekStep);
                    CalendarAppointmentInfo appInfo = appInfos.FirstOrDefault(a => a.childAppointment == appointment && startDate <= a.Date && endDate >= a.Date);
                    if (appInfo != null)
                    {
                        RadRect layoutSlot = appInfo.layoutSlot;
                        this.scrollViewer.ChangeView(0.0f, layoutSlot.Y, 1.0f);
                    }
                }
            }
        }

        internal void RecycleTimeRulerItems(ElementCollection<CalendarTimeRulerItem> timeRulerItems)
        {
            foreach (var item in timeRulerItems)
            {
                TextBlock visual;
                if (this.realizedTimerRulerItemsPresenters.TryGetValue(item, out visual))
                {
                    this.realizedTimerRulerItemsPresenters.Remove(item);
                    this.recycledTimeRulerItems.Enqueue(visual);
                }
            }
        }

        internal void RecycleTimeRulerLines(ElementCollection<CalendarGridLine> timeRulerLines)
        {
            foreach (var line in timeRulerLines)
            {
                Border visual;
                if (this.realizedTimerRulerLinePresenters.TryGetValue(line, out visual))
                {
                    this.realizedTimerRulerLinePresenters.Remove(line);
                    this.recycledTimeRulerLines.Enqueue(visual);
                }
            }
        }

        internal void RecycleSlots(IEnumerable<Slot> slots)
        {
            foreach (Slot slot in slots)
            {
                Border visual;
                if (this.realizedSlotPresenters.TryGetValue(slot, out visual))
                {
                    this.realizedSlotPresenters.Remove(slot);
                    this.recycledSlots.Enqueue(visual);
                }
            }
        }

        internal void UpdateCurrentTimeIndicator()
        {
            CalendarModel model = this.Owner.Model;
            CalendarGridLine currentTimeIndicator = model.CurrentTimeIndicator;
            if (currentTimeIndicator != null)
            {
                if (this.viewPortArea.IntersectsWith(currentTimeIndicator.layoutSlot) && currentTimeIndicator.layoutSlot.IsSizeValid())
                {
                    if (this.currentTimeIndicatorBorder == null)
                    {
                        this.currentTimeIndicatorBorder = new Border();
                        this.AddVisualChild(this.currentTimeIndicatorBorder);
                        Canvas.SetZIndex(this.currentTimeIndicatorBorder, XamlTimeRulerLayer.DefaultCurrentTimeIndicatorZIndex);
                    }
                    else if (this.currentTimeIndicatorBorder.Visibility == Visibility.Collapsed)
                    {
                        this.currentTimeIndicatorBorder.Visibility = Visibility.Visible;
                        Canvas.SetZIndex(this.currentTimeIndicatorBorder, XamlTimeRulerLayer.DefaultCurrentTimeIndicatorZIndex);
                    }

                    MultiDayViewSettings settings = model.multiDayViewSettings;
                    Style currentTimeIndicatorStyle = settings.CurrentTimeIndicatorStyle ?? settings.defaultCurrentTimeIndicatorStyle;
                    if (currentTimeIndicatorStyle != null)
                    {
                        this.currentTimeIndicatorBorder.Style = currentTimeIndicatorStyle;
                    }

                    XamlTimeRulerLayer.ArrangeUIElement(this.currentTimeIndicatorBorder, currentTimeIndicator.layoutSlot);
                    Canvas.SetLeft(this.currentTimeIndicatorBorder, -this.leftOffset);
                }
                else if (this.currentTimeIndicatorBorder?.Visibility == Visibility.Visible)
                {
                    this.currentTimeIndicatorBorder.Visibility = Visibility.Collapsed;
                    this.currentTimeIndicatorBorder.ClearValue(Canvas.ZIndexProperty);
                    this.currentTimeIndicatorBorder.ClearValue(Border.StyleProperty);
                }
            }
        }

        internal void UpdateSlots(IEnumerable<Slot> slots)
        {
            this.RecycleSlots(slots);
            foreach (var slot in slots)
            {
                if (!this.bufferedViewPortArea.IntersectsWith(slot.layoutSlot))
                {
                    continue;
                }

                Border slotVisual = this.GetDefaultSlotVisual(slot);
                if (slotVisual != null)
                {
                    MultiDayViewSettings settings = this.Owner.MultiDayViewSettings;
                    StyleSelector specialSlotStyleSelector = settings.SpecialSlotStyleSelector ?? settings.defaultSpecialSlotStyleSelector;
                    if (specialSlotStyleSelector != null)
                    {
                        var style = specialSlotStyleSelector.SelectStyle(slot, slotVisual);
                        if (style != null)
                        {
                            slotVisual.Style = style;
                        }
                    }

                    XamlContentLayer.ArrangeUIElement(slotVisual, slot.layoutSlot, true);
                    Canvas.SetZIndex(slotVisual, XamlTimeRulerLayer.DefaultSlotZIndex);
                    Canvas.SetLeft(slotVisual, slot.layoutSlot.X - this.leftOffset + this.leftHeaderPanel.Width);
                }
            }

            foreach (Border slot in this.recycledSlots)
            {
                slot.Visibility = Visibility.Collapsed;
            }
        }

        internal void UpdateTodaySlot()
        {
            Slot todaySlot = this.Owner.Model.multiDayViewModel.todaySlot;
            if (this.todaySlotBorder == null)
            {
                this.todaySlotBorder = new Border();
                this.contentPanel.Children.Add(this.todaySlotBorder);
            }
            else
            {
                this.todaySlotBorder.ClearValue(Border.VisibilityProperty);
                this.todaySlotBorder.ClearValue(Border.StyleProperty);
                this.todaySlotBorder.ClearValue(Canvas.ZIndexProperty);
                this.todaySlotBorder.ClearValue(Canvas.LeftProperty);
            }

            if (this.bufferedViewPortArea.IntersectsWith(todaySlot.layoutSlot))
            {
                MultiDayViewSettings settings = this.Owner.MultiDayViewSettings;
                Style todaySlotStyle = settings.TodaySlotStyle ?? settings.defaulTodaySlotStyle;
                if (todaySlotStyle != null)
                {
                    this.todaySlotBorder.Style = todaySlotStyle;
                }

                RadRect layoutSlot = todaySlot.layoutSlot;
                XamlTimeRulerLayer.ArrangeUIElement(this.todaySlotBorder, layoutSlot, true);

                Canvas.SetZIndex(this.todaySlotBorder, XamlTimeRulerLayer.DefaultTodaySlotZIndex);
                Canvas.SetLeft(this.todaySlotBorder, layoutSlot.X - this.leftOffset + this.leftHeaderPanel.Width);
            }
            else if (this.todaySlotBorder.Visibility == Visibility.Visible)
            {
                this.todaySlotBorder.Visibility = Visibility.Collapsed;
            }
        }

        internal void UpdateAppointments(List<CalendarAppointmentInfo> appointmentInfos)
        {
            int index = 0;
            RadCalendar calendar = this.Owner;
            foreach (var appInfo in appointmentInfos)
            {
                if (!this.bufferedViewPortArea.IntersectsWith(appInfo.layoutSlot))
                {
                    continue;
                }

                AppointmentControl appointmentControl = this.GetDefaultAppointmentVisual(index);
                if (appointmentControl != null)
                {
                    RadRect layoutSlot = appInfo.layoutSlot;
                    appointmentControl.Content = appInfo.DetailText;
                    appointmentControl.Header = appInfo.Subject;
                    appointmentControl.Background = appInfo.Brush;
                    appointmentControl.appointmentInfo = appInfo;

                    StyleSelector contentStyleSelector = calendar.AppointmentStyleSelector;
                    if (contentStyleSelector != null)
                    {
                        var style = contentStyleSelector.SelectStyle(appInfo, appointmentControl);
                        if (style != null)
                        {
                            appointmentControl.Style = style;
                        }
                    }

                    AppointmentTemplateSelector templateSelector = calendar.AppointmentTemplateSelector;
                    if (templateSelector != null)
                    {
                        DataTemplate template = templateSelector.SelectTemplate(appInfo, appInfo.cell);
                        if (template != null)
                        {
                            appointmentControl.ContentTemplate = template;
                        }
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
                    Canvas.SetZIndex(appointmentControl, XamlTimeRulerLayer.DefaultAppointmentZIndex);
                    Canvas.SetLeft(appointmentControl, layoutSlot.X - this.leftOffset + this.leftHeaderPanel.Width);
                    index++;
                }
            }

            while (index < this.realizedAppointmentDefaultPresenters.Count)
            {
                this.realizedAppointmentDefaultPresenters[index].Visibility = Visibility.Collapsed;
                index++;
            }
        }

        internal void UpdateTimeRulerDecorations(CalendarGridLine horizontalLine, CalendarGridLine verticalLine)
        {
            if (this.horizontalGridLineBorder == null)
            {
                this.horizontalGridLineBorder = new Border();
                this.topLeftHeaderPanel.Children.Add(this.horizontalGridLineBorder);
            }

            if (this.verticalGridLineBorder == null)
            {
                this.verticalGridLineBorder = new Border();
                this.topLeftHeaderPanel.Children.Add(this.verticalGridLineBorder);
            }

            this.ApplyTimeRulerStyle(horizontalLine, this.horizontalGridLineBorder);

            if (this.horizontalGridLineBorder.BorderBrush != null && !XamlDecorationLayer.IsStrokeThicknessExplicitlySet(this.horizontalGridLineBorder.Style))
            {
                this.horizontalGridLineBorder.BorderThickness = new Thickness(this.Owner.GridLinesThickness);
            }

            RadRect layoutSlot = horizontalLine.layoutSlot;
            XamlTimeRulerLayer.ArrangeUIElement(this.horizontalGridLineBorder, layoutSlot, true);

            this.ApplyTimeRulerStyle(verticalLine, this.verticalGridLineBorder);

            if (this.verticalGridLineBorder.BorderBrush != null && !XamlDecorationLayer.IsStrokeThicknessExplicitlySet(this.verticalGridLineBorder.Style))
            {
                this.verticalGridLineBorder.BorderThickness = new Thickness(this.Owner.GridLinesThickness);
            }

            layoutSlot = verticalLine.layoutSlot;
            XamlTimeRulerLayer.ArrangeUIElement(this.verticalGridLineBorder, layoutSlot, true);
        }

        internal void ArrangeVisualElement()
        {
            RadCalendar calendar = this.Owner;
            Size availableCalendarViewSize = calendar.CalendarViewSize;
            CalendarMultiDayViewModel multiDayViewModel = calendar.Model.multiDayViewModel;

            double allDayAreaHeight = multiDayViewModel.totalAllDayAreaHeight;
            double timeRulerWidth = multiDayViewModel.timeRulerWidth;
            double totalWidth = (availableCalendarViewSize.Width - timeRulerWidth) * 2;
            this.leftOffset = totalWidth / 2 + timeRulerWidth - calendar.GridLinesThickness / 2;
            foreach (var topHeaderChild in this.topHeader.Children)
            {
                if (topHeaderChild.RenderTransform != this.translateTransform)
                {
                    topHeaderChild.RenderTransform = this.translateTransform;
                }
                Canvas.SetLeft(topHeaderChild, -this.leftOffset);
            }

            Canvas.SetLeft(this.topHeader, -this.leftOffset);

            this.scrollViewer.Width = availableCalendarViewSize.Width;
            this.scrollViewer.Height = availableCalendarViewSize.Height;

            this.contentPanel.Width = totalWidth;

            double cellHeight = multiDayViewModel.dayViewLayoutSlot.Height / multiDayViewModel.SpecificColumnCount;
            double topOffset = Math.Abs(cellHeight + multiDayViewModel.dayViewLayoutSlot.Y + calendar.GridLinesThickness);

            this.topLeftHeaderPanel.Height = topOffset + allDayAreaHeight;
            this.topLeftHeaderPanel.Width = timeRulerWidth;

            this.topHeader.Height = topOffset + allDayAreaHeight;
            this.topHeader.Width = totalWidth + this.leftOffset;

            if (multiDayViewModel.timeRulerItems != null && multiDayViewModel.timeRulerItems.Count > 0)
            {
                RadRect lastItemSlot = multiDayViewModel.timeRulerItems[multiDayViewModel.timeRulerItems.Count - 1].layoutSlot;
                this.contentPanel.Height = lastItemSlot.Y + lastItemSlot.Height;
                this.contentPanel.Width = this.scrollViewer.Width;

                this.leftHeaderPanel.Height = lastItemSlot.Y + lastItemSlot.Height;
                this.leftHeaderPanel.Width = multiDayViewModel.timeRulerWidth;
            }
        }

        internal void UpdatePanelsBackground(Brush background)
        {
            if (background != null)
            {
                this.contentPanel.Background = background;
                this.leftHeaderPanel.Background = background;
            }
            else
            {
                this.contentPanel.Background = XamlTimeRulerLayer.DefaultBackground;
                this.leftHeaderPanel.Background = XamlTimeRulerLayer.DefaultBackground;
            }
        }

        protected internal override void DetachUI(Panel parent)
        {
            base.DetachUI(parent);
            this.scrollViewer.ViewChanged -= this.OnScrollViewerViewChanged;

            this.scrollViewer.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(this.OnScrollViewerPointerPressed));
            this.scrollViewer.RemoveHandler(UIElement.PointerMovedEvent, new PointerEventHandler(this.OnScrollViewerPointerMoved));
            this.scrollViewer.ManipulationCompleted -= this.OnScrollViewerManipulationCompleted;

            if (this.offsetStoryboard != null)
            {
                this.offsetStoryboard.Completed -= this.OnOffsetStoryboardCompleted;
            }
        }

        protected internal override void AttachUI(Panel parent)
        {
            base.AttachUI(parent);

            RadCalendar calendar = this.Owner;
            if (calendar != null)
            {
                MultiDayViewSettings settings = calendar.MultiDayViewSettings;
                this.UpdatePanelsBackground(settings.TimelineBackground);
            }

            this.scrollViewer.ViewChanged += this.OnScrollViewerViewChanged;
            this.scrollViewer.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(this.OnScrollViewerPointerPressed), true);
            this.scrollViewer.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(this.OnScrollViewerPointerMoved), true);
            this.scrollViewer.ManipulationCompleted += this.OnScrollViewerManipulationCompleted;

            if (this.offsetStoryboard != null)
            {
                this.offsetStoryboard.Completed += this.OnOffsetStoryboardCompleted;
            }
        }

        protected internal override void AddVisualChild(UIElement child)
        {
            this.contentPanel.Children.Add(child);
        }

        protected internal override void RemoveVisualChild(UIElement child)
        {
            this.contentPanel.Children.Remove(child);
        }

        private void UpdateTimeRulerItems(ElementCollection<CalendarTimeRulerItem> timeRulerItems)
        {
            this.RecycleTimeRulerItems(timeRulerItems);
            foreach (var item in timeRulerItems)
            {
                if (!this.viewPortArea.IntersectsWith(item.layoutSlot))
                {
                    continue;
                }

                FrameworkElement element = this.GetDefaultTimeRulerItemVisual(item);
                if (element != null)
                {
                    this.ApplyTimeRulerStyle(item, element);
                    RadRect layoutSlot = item.layoutSlot;
                    
                    XamlContentLayer.ArrangeUIElement(element, layoutSlot, true);
                }
            }

            foreach (TextBlock item in this.recycledTimeRulerItems)
            {
                item.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateTimerRulerLines(ElementCollection<CalendarGridLine> timeRulerLines)
        {
            this.RecycleTimeRulerLines(timeRulerLines);
            foreach (var gridLine in timeRulerLines)
            {
                if (!this.viewPortArea.IntersectsWith(gridLine.layoutSlot))
                {
                    continue;
                }

                Border border = this.GetTimerRulerLine(gridLine);
                if (border != null)
                {
                    this.ApplyTimeRulerStyle(gridLine, border);

                    if (border.BorderBrush != null && !XamlDecorationLayer.IsStrokeThicknessExplicitlySet(border.Style))
                    {
                        border.BorderThickness = new Thickness(this.Owner.GridLinesThickness);
                    }

                    RadRect layoutSlot = gridLine.layoutSlot;
                    XamlContentLayer.ArrangeUIElement(border, layoutSlot, true);
                    Canvas.SetLeft(border, -this.leftOffset);
                    Canvas.SetZIndex(border, XamlTimeRulerLayer.DefaultLineZIndex);
                }
            }

            foreach (var recycledLine in this.recycledTimeRulerLines)
            {
                recycledLine.Visibility = Visibility.Collapsed;
            }
        }

        private void EnsureMeasurementPresenter()
        {
            if (this.measurementPresenter == null)
            {
                this.measurementPresenter = new TextBlock();
                this.measurementPresenter.Margin = new Thickness(5, 0, 5, 0);
                this.measurementPresenter.Opacity = 0;
                this.measurementPresenter.IsHitTestVisible = false;
            }
        }

        private FrameworkElement GetDefaultTimeRulerItemVisual(CalendarTimeRulerItem timeRulerItem)
        {
            TextBlock visual;
            if (this.recycledTimeRulerItems.Count > 0)
            {
                visual = this.recycledTimeRulerItems.Dequeue();

                visual.ClearValue(TextBlock.VisibilityProperty);
                visual.ClearValue(TextBlock.StyleProperty);
            }
            else
            {
                visual = this.CreateDefaultTimeRulerItemVisual();
            }

            visual.Text = timeRulerItem.Label;

            this.realizedTimerRulerItemsPresenters.Add(timeRulerItem, visual);
            return visual;
        }

        private Border GetTimerRulerLine(CalendarGridLine line)
        {
            Border visual;
            if (this.recycledTimeRulerLines.Count > 0)
            {
                visual = this.recycledTimeRulerLines.Dequeue();

                visual.ClearValue(Border.VisibilityProperty);
                visual.ClearValue(Border.StyleProperty);
                visual.ClearValue(Border.BorderThicknessProperty);
                visual.ClearValue(Canvas.ZIndexProperty);
                visual.ClearValue(Canvas.LeftProperty);
            }
            else
            {
                visual = this.CreateBorderVisual();
            }

            this.realizedTimerRulerLinePresenters.Add(line, visual);
            return visual;
        }

        private Border GetDefaultSlotVisual(Slot slot)
        {
            Border visual;
            if (this.recycledSlots.Count > 0)
            {
                visual = this.recycledSlots.Dequeue();

                visual.ClearValue(Border.VisibilityProperty);
                visual.ClearValue(Border.StyleProperty);
                visual.ClearValue(Border.BorderThicknessProperty);
                visual.ClearValue(Canvas.ZIndexProperty);
            }
            else
            {
                visual = this.CreateBorderVisual();
            }

            this.realizedSlotPresenters.Add(slot, visual);
            return visual;
        }

        private AppointmentControl GetDefaultAppointmentVisual(int virtualIndex)
        {
            AppointmentControl visual;

            if (virtualIndex < this.realizedAppointmentDefaultPresenters.Count)
            {
                visual = this.realizedAppointmentDefaultPresenters[virtualIndex];
                visual.ClearValue(AppointmentControl.VisibilityProperty);
                visual.ClearValue(Canvas.ZIndexProperty);
                visual.ClearValue(Canvas.LeftProperty);
            }
            else
            {
                visual = this.CreateDefaultAppointmentVisual();
            }

            return visual;
        }

        private TextBlock CreateDefaultTimeRulerItemVisual()
        {
            TextBlock textBlock = new TextBlock();
            this.leftHeaderPanel.Children.Add(textBlock);

            return textBlock;
        }

        private Border CreateBorderVisual()
        {
            Border slot = new Border();
            this.AddVisualChild(slot);

            return slot;
        }

        private AppointmentControl CreateDefaultAppointmentVisual()
        {
            AppointmentControl appointmentControl = new AppointmentControl();
            appointmentControl.calendar = this.Owner;
            this.realizedAppointmentDefaultPresenters.Add(appointmentControl);
            this.AddVisualChild(appointmentControl);

            return appointmentControl;
        }

        private void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            this.UpdateUI();
        }

        private void OnScrollViewerPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!this.isAnimationOngoing)
            {
                this.scrollMousePosition = e.GetCurrentPoint(this.scrollViewer).Position;
                this.prevHorizontalOffset = this.translateTransform.X;
                this.scrollViewer.CapturePointer(e.Pointer);
            }
        }

        private void OnScrollViewerPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!this.isAnimationOngoing && this.scrollViewer.PointerCaptures != null && this.scrollViewer.PointerCaptures.Count > 0)
            {
                this.currHorizontalOffset = this.prevHorizontalOffset + (this.scrollMousePosition.X - e.GetCurrentPoint(this.scrollViewer).Position.X);
                double viewPortWidth = this.scrollViewer.Width - this.leftHeaderPanel.Width;
                if (viewPortWidth > this.currHorizontalOffset && -viewPortWidth  < this.currHorizontalOffset)
                {
                    this.translateTransform.X = -this.currHorizontalOffset;
                }
            }
        }

        private void OnScrollViewerManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!this.isAnimationOngoing)
            {
                this.scrollViewer.ReleasePointerCaptures();

                RadCalendar calendar = this.Owner;
                double cellWidth = (this.scrollViewer.Width - this.leftHeaderPanel.Width) / calendar.Model.ColumnCount;
                double navigationWidth = cellWidth * calendar.MultiDayViewSettings.NavigationStep - cellWidth / 2;

                double currentPosition = this.translateTransform.X + this.currHorizontalOffset;
                if (this.currHorizontalOffset > 0)
                {
                    if (navigationWidth < this.currHorizontalOffset)
                    {
                        currentPosition -= (navigationWidth + cellWidth / 2);
                    }
                }
                else
                {
                    if (navigationWidth < -this.currHorizontalOffset)
                    {
                        currentPosition += (navigationWidth + cellWidth / 2);
                    }
                }

                this.offsetAnimation.From = this.translateTransform.X;
                this.offsetAnimation.To = currentPosition;

                this.currHorizontalOffset = currentPosition;
                this.isAnimationOngoing = true;
                this.offsetStoryboard.Begin();
            }
        }

        private void CreateOffsetAnimation()
        {
            this.offsetStoryboard = new Storyboard();

            this.offsetAnimation = new DoubleAnimation();
            this.offsetAnimation.EnableDependentAnimation = true;
            this.offsetAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            ExponentialEase easing = new ExponentialEase();
            easing.EasingMode = EasingMode.EaseInOut;
            this.offsetAnimation.EasingFunction = easing;

            this.offsetStoryboard.Children.Add(this.offsetAnimation);

            Storyboard.SetTarget(this.offsetStoryboard, this.translateTransform);
            Storyboard.SetTargetProperty(this.offsetStoryboard, "X");
        }

        private void OnOffsetStoryboardCompleted(object sender, object e)
        {
            this.offsetStoryboard.Stop();
            this.isAnimationOngoing = false;
            if (this.currHorizontalOffset != this.prevHorizontalOffset)
            {
                RadCalendar calendar = this.Owner;
                int navigationStep = calendar.MultiDayViewSettings.NavigationStep;
                if (this.translateTransform.X < 0)
                {
                    calendar.RaiseMoveToNextViewCommand(navigationStep);
                }
                else
                {
                    calendar.RaiseMoveToPreviousViewCommand(navigationStep);
                }
            }

            this.translateTransform.X = 0;
        }

        private void ApplyTimeRulerStyle(object item, FrameworkElement container)
        {
            MultiDayViewSettings settings = this.Owner.MultiDayViewSettings;
            CalendarTimeRulerItemStyleSelector itemStyleSelector = settings.TimeRulerItemStyleSelector ?? settings.defaultTimeRulerItemStyleSelector;
            if (itemStyleSelector != null)
            {
                var style = itemStyleSelector.SelectStyle(item, container);
                if (style != null)
                {
                    container.Style = style;
                }
            }
        }
    }
}