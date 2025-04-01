using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PetryNet.ViewModels.Core
{
    public class PlaceViewModel : NodeViewModel
    {
        public PlaceModel Model { get; private set; }
        public ObservableCollection<TokenViewModel> Tokens { get; private set; } = new ObservableCollection<TokenViewModel>();
        private double _radius = 40;

        private double CenterX = 16;
        private double CenterY = 16;
        private double RadiusForPlacement = 12;

        private double _tokenTextCoordX;
        private double _tokenTextCoordY;

        public double TokenTextCoordX { get => _tokenTextCoordX; set => SetProperty(ref _tokenTextCoordX, value); }
        public double TokenTextCoordY { get => _tokenTextCoordY; set => SetProperty(ref _tokenTextCoordY, value); }

        public int TokenCount { get => Model.Tokens.Count; }

        public string DisplayText => TokenCount > 5 ? TokenCount.ToString() : string.Empty;

        public PlaceViewModel(PlaceModel place, double x, double y, PetryNetViewModel parent) : base()
        {
            Model = place;
            this.NodePositionChanged += UpdateTokenPositions;
            Parent = parent;
            X = x - Radius / 2;
            Y = y - Radius / 2;
            _tokenTextCoordX = X;
            _tokenTextCoordY = Y;

            for(int i = 0; i < Model.Tokens.Count; i++)
            {
                Tokens.Add(CreateTokenViewModel(Model.Tokens[i], i + 1));
            }
        }
        public string Name => Model.Name;

        public double Radius
        {
            get => _radius;
            set => SetProperty(ref _radius, value);
        }

        public void AddToken(TokenModel token)
        {
            Model.AddToken(token);
            SyncTokensWithModel();
            //OnPropertyChanged(nameof(Tokens));
            //if (Tokens.Count < 5)
            //{
            //    Tokens.Add(CreateTokenViewModel(token));
            //}
            //OnPropertyChanged(nameof(TokenCount));
            //OnPropertyChanged(nameof(DisplayText));
        }

        private TokenViewModel CreateTokenViewModel(TokenModel token, int countOfToken)
        {

            double angle = (countOfToken / 5.0) * 2 * Math.PI; // Spread around a circle
            double x = X + CenterX + RadiusForPlacement * Math.Cos(angle);
            double y = Y + CenterY + RadiusForPlacement * Math.Sin(angle);

            return new TokenViewModel(token, x, y);
        }

        public void SyncTokensWithModel()
        {

            Tokens.Clear();

            int tokenCount = Model.Tokens.Count;

            OnPropertyChanged(nameof(TokenCount));

            int tokensToShow = Math.Min(tokenCount, 5); // Show at most 5 tokens

            for (int i = 0; i < tokensToShow; i++)
            {
                var tokenViewModel = CreateTokenViewModel(Model.Tokens[i], i+1);
                Tokens.Add(tokenViewModel);
            }
            OnPropertyChanged(nameof(Tokens));
            OnPropertyChanged(nameof(DisplayText));

        }

        private void UpdateTokenPositions()
        {
            if (TokenCount >= 6)
            {
                TokenTextCoordX = X + RadiusForPlacement;
                TokenTextCoordY = Y + RadiusForPlacement;
            }
            else
            {
                for (int i = 0; i < TokenCount; i++)
                {
                    double angle = (i / 5.0) * 2 * Math.PI; // Spread around a circle
                    double x = X + CenterX + RadiusForPlacement * Math.Cos(angle);
                    double y = Y + CenterY + RadiusForPlacement * Math.Sin(angle);

                    Tokens[i].UpdatePosition(x, y);
                }
            }
        }

        internal void RemoveToken()
        {
            if (Model.Tokens.Count > 0)
            {

                Model.RemoveToken(Model.Tokens.Last());
                SyncTokensWithModel();
            }
        }
    }
}
