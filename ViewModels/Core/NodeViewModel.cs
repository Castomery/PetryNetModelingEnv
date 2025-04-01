using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.ViewModels.Core
{
    public abstract class NodeViewModel : SelectableElementViewModelBase
    {
        private string _color;
        private Point dragStartPosition;
        private double initialX;
        private double initialY;

        private double _x;
        private double _y;

        public event Action NodePositionChanged;

        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X)); // Notify UI
                    NodePositionChanged?.Invoke();
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y)); // Notify UI
                    NodePositionChanged?.Invoke();
                }
            }
        }

        public NodeViewModel():base()
        {
            
        }

        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public void DragStarted(Point currentPosition)
        {
            dragStartPosition = currentPosition;
            initialX = X;
            initialY = Y;

            Drag(currentPosition);
        }

        // Called as drag proceeds
        public void Drag(Point currentPosition)
        {
            // Calculate the offset from the start position
            double deltaX = currentPosition.X - dragStartPosition.X;
            double deltaY = currentPosition.Y - dragStartPosition.Y;

            // Update the position
            X = initialX + deltaX;
            Y = initialY + deltaY;
        }

        // Called when drag operation completes
        public void DragCompleted(Point finalPosition)
        {
            Drag(finalPosition);
        }
    }
}
