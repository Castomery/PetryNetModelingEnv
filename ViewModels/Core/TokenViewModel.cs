using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.ViewModels.Core
{
    public class TokenViewModel : NodeViewModel
    {
        public TokenModel _model { get; private set; }

        //public PlaceViewModel _parentPlace { get; private set; }

        public TokenViewModel(TokenModel model)
        {
            _model = model;
            //_parentPlace = parentPlace;
        }

        public TokenViewModel(TokenModel model, double x, double y)
        {
            _model = model;
            //_parentPlace = parentPlace;
            X = x;
            Y = y;
        }

        public void UpdatePosition(double x, double y)
        {
            X = x;
            Y = y;
            OnPropertyChanged(nameof(X));
        }
    }
}
