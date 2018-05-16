using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal static class DragDrop
    {
        public static readonly DependencyProperty AllowDragProperty =
            DependencyProperty.RegisterAttached("AllowDrag", typeof(bool), typeof(DragDrop), new PropertyMetadata(false, OnAllowDragChanged));

        public static readonly DependencyProperty AllowDropProperty =
            DependencyProperty.RegisterAttached("AllowDrop", typeof(bool), typeof(DragDrop), new PropertyMetadata(false, OnAllowDropChanged));

        public static readonly DependencyProperty DragPositionModeProperty =
            DependencyProperty.RegisterAttached("DragPositionMode", typeof(DragPositionMode), typeof(DragDrop), new PropertyMetadata(DragPositionMode.Free));

        private static readonly DependencyProperty dragDropOperationProperty =
            DependencyProperty.RegisterAttached("dragDropOperation", typeof(IDragDropOperation), typeof(DragDrop), null);

        private static DependencyProperty dragInitializerProperty =
            DependencyProperty.RegisterAttached("dragInitializer", typeof(DragInitializer), typeof(DragDrop), new PropertyMetadata(null));

        public static bool GetAllowDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDragProperty);
        }

        public static void SetAllowDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDragProperty, value);
        }

        public static bool GetAllowDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDropProperty);
        }

        public static void SetAllowDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDropProperty, value);
        }

        public static DragPositionMode GetDragPositionMode(DependencyObject obj)
        {
            return (DragPositionMode)obj.GetValue(DragPositionModeProperty);
        }

        public static void SetDragPositionMode(DependencyObject obj, DragPositionMode value)
        {
            obj.SetValue(DragPositionModeProperty, value);
        }

        public static IDragDropOperation GetRunningOperation(DependencyObject element)
        {
            IDragDropOperation operation = null;
            if (element != null)
            {
                operation = element.GetValue(DragDrop.dragDropOperationProperty) as IDragDropOperation;
            }

            return operation;
        }

        internal static void StartDrag(object sender, PointerRoutedEventArgs e, DragDropTrigger trigger, object initializeContext = null)
        {
            var dragDropElement = sender as IDragDropElement;
            var uiSource = sender as UIElement;
            if (dragDropElement == null)
            {
                dragDropElement = ElementTreeHelper.FindVisualAncestor<IDragDropElement>(uiSource);
            }

            if (GetRunningOperation(dragDropElement as DependencyObject) != null)
            {
                return;
            }

            UIElement uiDragDropElement = dragDropElement as UIElement;

            var frameworkElementSource = dragDropElement as FrameworkElement;
            double leftMargin = 0d;
            double topMargin = 0d;

            if (frameworkElementSource != null)
            {
                leftMargin = frameworkElementSource.Margin.Left;
                topMargin = frameworkElementSource.Margin.Top;
            }

            if (dragDropElement == null || !dragDropElement.CanStartDrag(trigger, initializeContext))
            {
                return;
            }

            var context = dragDropElement.DragStarting(trigger, initializeContext);

            if (context == null)
            {
                return;
            }

            var startDragPosition = e.GetCurrentPoint(context.DragSurface.RootElement).Position;
            var relativeStartDragPosition = e.GetCurrentPoint(uiDragDropElement).Position;
            var dragPositionMode = DragDrop.GetDragPositionMode(uiDragDropElement);

            AddOperation(new DragDropOperation(context, dragDropElement, dragPositionMode, e.Pointer, startDragPosition, relativeStartDragPosition));
        }

        internal static void StartDrag(object sender, HoldingRoutedEventArgs e, DragDropTrigger trigger, object initializeContext = null)
        {
            var dragDropElement = sender as IDragDropElement;

            if (GetRunningOperation(dragDropElement as DependencyObject) != null)
            {
                return;
            }

            var uiSource = sender as UIElement;
            var frameworkElementSource = sender as FrameworkElement;
            double leftMargin = 0d;
            double topMargin = 0d;

            if (frameworkElementSource != null)
            {
                leftMargin = frameworkElementSource.Margin.Left;
                topMargin = frameworkElementSource.Margin.Top;
            }

            if (dragDropElement == null || !dragDropElement.CanStartDrag(trigger, initializeContext))
            {
                return;
            }

            var context = dragDropElement.DragStarting(trigger, initializeContext);

            if (context == null)
            {
                return;
            }

            var startDragPosition = e.GetPosition(context.DragSurface.RootElement);
            var relativeStartDragPosition = e.GetPosition(uiSource);

            relativeStartDragPosition = new Point(relativeStartDragPosition.X + leftMargin, relativeStartDragPosition.Y + topMargin);
            var dragPositionMode = DragDrop.GetDragPositionMode(uiSource);

            AddOperation(new DragDropOperation(context, dragDropElement, dragPositionMode, null, startDragPosition, relativeStartDragPosition));
        }
        
        internal static void OnOperationFinished(IDragDropOperation dragDropOperation)
        {
            RemoveOperation(dragDropOperation);
        }

        // exposed for testing
        internal static void AddOperation(IDragDropOperation dragDropOperation)
        {
            (dragDropOperation.Source as DependencyObject).SetValue(DragDrop.dragDropOperationProperty, dragDropOperation);
        }

        private static void RemoveOperation(IDragDropOperation dragDropOperation)
        {
            (dragDropOperation.Source as DependencyObject).ClearValue(DragDrop.dragDropOperationProperty);
        }

        private static void OnAllowDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnAllowDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            if ((bool)e.NewValue)
            {
                element.SetValue(DragDrop.dragInitializerProperty, new DragInitializer(element));
            }
            else
            {
                element.ClearValue(DragDrop.dragInitializerProperty);
            }
        }
    }
}
