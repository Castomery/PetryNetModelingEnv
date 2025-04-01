using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PetryNet.ViewModels.Core
{
    public class ArcViewModel : NodeViewModel
    {
        public ArcModel Model { get; private set; }
        private readonly NodeViewModel _source;
        private readonly NodeViewModel _target;

        public NodeViewModel Source {  get { return _source; } }
        public NodeViewModel Target { get { return _target; } }

        public ArcViewModel(ArcModel arc) : base()
        {
            Model = arc;
        }

        public ArcViewModel(ArcModel arc, NodeViewModel source, NodeViewModel target)
        {
            _source = source;
            _target = target;
            Model = arc;
            _source.NodePositionChanged += UpdateCoords;
            _target.NodePositionChanged += UpdateCoords;
            CalculateCoordinates();
        }

        private double _startX;
        private double _startY;
        private double _endX;
        private double _endY;

        private double offsetY = 10;

        public double StartX
        {
            get => _startX;
            set => SetProperty(ref _startX, value);
        }
        public double StartY
        {
            get => _startY;
            set => SetProperty(ref _startY, value);
        }
        public double EndX
        {
            get => _endX;
            set => SetProperty(ref _endX, value);
        }
        public double EndY
        {
            get => _endY;
            set => SetProperty(ref _endY, value);
        }

        private Point _endPoint;

        public Point EndPoint
        {
            get => _endPoint;
            set => SetProperty(ref _endPoint, value);
        }

        private Point _arrowPoint1;
        public Point ArrowPoint1
        {
            get => _arrowPoint1;
            set => SetProperty(ref _arrowPoint1, value);
        }

        private Point _arrowPoint2;
        public Point ArrowPoint2
        {
            get => _arrowPoint2;
            set => SetProperty(ref _arrowPoint2, value);
        }

        private Point CalculateArrowPoint(double length, double angle)
        {
            double lineAngle = Math.Atan2(EndY - StartY, EndX - StartX);
            double arrowAngle = lineAngle + angle * Math.PI / 180;
            return new Point(EndX + length * Math.Cos(arrowAngle), EndY + length * Math.Sin(arrowAngle));
        }

        private void CalculateCoordinates()
        {
            if (_source is PlaceViewModel sourcePlace && _target is TransitionViewModel targetTransition)
            {
                double halfRadius = sourcePlace.Radius / 2f;
                double halfWidth = targetTransition.Width / 2f;
                double halfHeight = targetTransition.Height / 2f;

                double sourceCenterX = sourcePlace.X + halfRadius;
                double sourceCenterY = sourcePlace.Y + halfRadius;
                double targetCenterX = targetTransition.X + halfWidth;
                double targetCenterY = targetTransition.Y + halfHeight;


                double angle = Math.Atan2(targetCenterY - sourceCenterY, targetCenterX - sourceCenterX);

                (StartX, StartY) = (sourceCenterX + halfRadius * Math.Cos(angle), sourceCenterY + halfRadius * Math.Sin(angle));
                (EndX, EndY) = CalculateRectangleEdgePoint(targetTransition, targetCenterX, targetCenterY, angle + Math.PI);


            }
            else if (_source is TransitionViewModel sourceTransition && _target is PlaceViewModel targetPlace)
            {
                double halfRadius = targetPlace.Radius / 2f;
                double halfWidth = sourceTransition.Width / 2f;
                double halfHeight = sourceTransition.Height / 2f;

                double sourceCenterX = sourceTransition.X + halfWidth;
                double sourceCenterY = sourceTransition.Y + halfHeight;
                double targetCenterX = targetPlace.X + halfRadius;
                double targetCenterY = targetPlace.Y + halfRadius;

                // Corrected angle calculation:
                double angle = Math.Atan2(targetCenterY - sourceCenterY, targetCenterX - sourceCenterX);

                // Calculate the edge points from source (Transition) to target (Place)
                (StartX, StartY) = CalculateRectangleEdgePoint(sourceTransition, sourceCenterX, sourceCenterY, angle);
                (EndX, EndY) = (targetCenterX + halfRadius * Math.Cos(angle + Math.PI),
                                targetCenterY + halfRadius * Math.Sin(angle + Math.PI));
            }

            EndPoint = new Point(EndX, EndY);
            ArrowPoint1 = CalculateArrowPoint(-20, 10);
            ArrowPoint2 = CalculateArrowPoint(-20, -10);

        }

        private (double X, double Y) CalculateRectangleEdgePoint(TransitionViewModel transition, double centerX, double centerY, double angle)
        {
            double halfWidth = transition.Width / 2;
            double halfHeight = transition.Height / 2;

            double dx = Math.Cos(angle);
            double dy = Math.Sin(angle);

            double scaleX = halfWidth / Math.Abs(dx);
            double scaleY = halfHeight / Math.Abs(dy);

            // Determine which edge the line intersects first
            if (scaleX < scaleY)
            {
                // Line intersects left or right edge first
                double edgeX = (dx > 0) ? (centerX + halfWidth) : (centerX - halfWidth);
                double edgeY = centerY + dy * scaleX;
                return (edgeX, edgeY);
            }
            else
            {
                // Line intersects top or bottom edge first
                double edgeY = (dy > 0) ? (centerY + halfHeight) : (centerY - halfHeight);
                double edgeX = centerX + dx * scaleY;
                return (edgeX, edgeY);
            }
        }

        public void UpdateCoords()
        {
            CalculateCoordinates();   
        }
    }
}
