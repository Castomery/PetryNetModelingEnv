using PetryNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using PetryNet.ViewModels.Core;

namespace PetryNet.AttachedProperties
{
    internal class DragAndDropPros
    {
        #region EnabledForDragDrop

        public static readonly DependencyProperty EnabledForDragDropProperty =
            DependencyProperty.RegisterAttached("EnabledForDragDrop", typeof(bool), typeof(DragAndDropPros),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnEnabledForDragDropChanged)));

        public static bool GetEnabledForDragDrop(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledForDragDropProperty);
        }

        public static void SetEnabledForDragDrop(DependencyObject d, bool value)
        {
            d.SetValue(EnabledForDragDropProperty, value);
        }

        private static void OnEnabledForDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            if ((bool)e.NewValue)
            {
                fe.MouseMove += Element_MouseMove;
                fe.PreviewMouseLeftButtonUp += Element_MouseLeftButtonUp;
            }
            else
            {
                fe.MouseMove -= Element_MouseMove;
                fe.PreviewMouseLeftButtonUp -= Element_MouseLeftButtonUp;
            }
        }
        #endregion

        // Track whether we're currently dragging
        private static bool isDragging = false;
        private static Point startPoint;
        private static FrameworkElement draggedElement;

        // This method will be called after the selection handling
        private static void Element_MouseMove(object sender, MouseEventArgs e)
        {

            Point currentPosition = e.GetPosition(Application.Current.MainWindow);

            if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
            {
                // Get the current mouse position
              
                // Only start dragging if the mouse has moved a significant distance
                if (Math.Abs(currentPosition.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPosition.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    isDragging = true;
                    draggedElement = sender as FrameworkElement;

                    // Capture mouse to receive events even when outside the element
                    draggedElement.CaptureMouse();

                    // Start drag operation
                    StartDrag(draggedElement, currentPosition);

                }
            }
            else if (isDragging && draggedElement != null)
            {
                if(draggedElement.DataContext is NodeViewModel viewModel)
                {
                    viewModel.Drag(currentPosition);
                }
            }
        }

        private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && draggedElement != null)
            {
                // End drag operation
                EndDrag(draggedElement, e.GetPosition(Application.Current.MainWindow));

                // Release mouse capture
                draggedElement.ReleaseMouseCapture();

                isDragging = false;
                draggedElement = null;

                e.Handled = true;
            }
        }

        // Hook into SelectionProps to get the starting point for potential drag
        public static void SetStartDragPoint(Point point)
        {
            startPoint = point;
        }

        private static void StartDrag(FrameworkElement element, Point currentPosition)
        {
            // Get the element's viewmodel
            if (element.DataContext is NodeViewModel viewModel)
            {
                // Store original position for calculating offset
                viewModel.DragStarted(currentPosition);
            }
        }

        private static void EndDrag(FrameworkElement element, Point finalPosition)
        {
            // Update the element's position in the viewmodel
            if (element.DataContext is NodeViewModel viewModel)
            {
                viewModel.DragCompleted(finalPosition);
            }
        }
    }
}

