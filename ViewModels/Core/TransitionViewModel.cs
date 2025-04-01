using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.ViewModels.Core
{
    public class TransitionViewModel : NodeViewModel
    {
        public TransitionModel Model { get; private set; }
        private double _width = 20; // Default width
        private double _height = 40; // Default height
        private bool _isEnabled = false;

        public TransitionViewModel(TransitionModel transition) : base()
        {
            Model = transition;
        }

        public string Name => Model.Name;

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
    }
}
