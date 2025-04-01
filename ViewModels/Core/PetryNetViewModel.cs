using PetryNet.Commands;
using PetryNet.Models;
using PetryNet.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection.Metadata;

namespace PetryNet.ViewModels.Core
{
    public class PetryNetViewModel : BaseViewModel, IPetryNetViewModel
    {
        private readonly PetryNetModel _petryNetModel;

        private List<SelectableElementViewModelBase> itemsToRemove;

        public ObservableCollection<PlaceViewModel> Places { get; } = new();
        public ObservableCollection<TransitionViewModel> Transitions { get; } = new();
        public ObservableCollection<ArcViewModel> Arcs { get; } = new();
        private ObservableCollection<SelectableElementViewModelBase> _allItems = new();

        private List<TokenViewModel> _allTokens = new();
        public RelayCommand AddPlaceItemCommand => new RelayCommand(AddPlace);

        public RelayCommand RemoveItemCommand => throw new NotImplementedException();

        public RelayCommand ClearSelectedItemsCommand => throw new NotImplementedException();

        public ObservableCollection<SelectableElementViewModelBase> Items
        {
            get { return _allItems; }
        }

        public List<SelectableElementViewModelBase> SelectedItems
        {
            get { return Items.Where(x => x.IsSelected).ToList(); }
        }

        public void AddSelectedItem(SelectableElementViewModelBase item)
        {
            _allItems.Add(item);
        }

        public RelayCommand AddTransitionItemCommand => new RelayCommand(AddTransition);

        public RelayCommand AddArcItemCommand => new RelayCommand(AddArc);

        private void AddArc(object obj)
        {
            if (obj is Tuple<SelectableElementViewModelBase, SelectableElementViewModelBase> arcData)
            {
                var firstClickedElement = arcData.Item1;
                var secondClickedElement = arcData.Item2;

                if (IsArcAlreadyExist(firstClickedElement, secondClickedElement)) return;

                if (CanBeLinked(firstClickedElement, secondClickedElement))
                {
                    PlaceModel placeModel;
                    TransitionModel transitionModel;
                    ArcModel arc = null;

                    if (firstClickedElement is PlaceViewModel && secondClickedElement is TransitionViewModel)
                    {
                        placeModel = (firstClickedElement as PlaceViewModel).Model;
                        transitionModel = (secondClickedElement as TransitionViewModel).Model;
                        arc = _petryNetModel.CreateAndGetArc(Arcs.Count, placeModel, transitionModel);
                        
                    }
                    else if (firstClickedElement is TransitionViewModel && secondClickedElement is PlaceViewModel)
                    {
                        transitionModel = (firstClickedElement as TransitionViewModel).Model;
                        placeModel = (secondClickedElement as PlaceViewModel).Model;
                        arc = _petryNetModel.CreateAndGetArc(Arcs.Count, transitionModel, placeModel);
                    }

                    if(arc != null)
                    {
                        ArcViewModel arcViewModel = new ArcViewModel(arc, (NodeViewModel)firstClickedElement, (NodeViewModel)secondClickedElement);
                        arcViewModel.Parent = this;
                        Arcs.Add(arcViewModel);
                        _allItems.Add(arcViewModel);
                    }
                }
            }
        }

        private bool IsArcAlreadyExist(SelectableElementViewModelBase firstClickedElement, SelectableElementViewModelBase secondClickedElement)
        {
            ArcViewModel arcViewModel = Arcs.Where(arc=> arc.Source == firstClickedElement && arc.Target == secondClickedElement).FirstOrDefault();

            if(arcViewModel == null)
            {
                return false;
            }
            else
            {
                arcViewModel.Model.IncreaseWeightByOne();
                return true;
            }
        }

        //private PlaceModel GetPlaceModel(PlaceViewModel clickedElement)
        //{
        //    return Places.Where(x => x.Name == clickedElement.Name).First().Model;
        //}

        //private TransitionModel GetTransitionModel(SelectableElementViewModelBase clickeElement)
        //{
        //    return Transitions.Where(x => x.Id == clickeElement.Id).First().Model;
        //}

        private bool IsPlaceToTransition(SelectableElementViewModelBase source, SelectableElementViewModelBase target) => source is PlaceViewModel && target is TransitionViewModel;
        private bool IsTransitionToPlace(SelectableElementViewModelBase source, SelectableElementViewModelBase target) => source is TransitionViewModel && target is PlaceViewModel;

        private bool CanBeLinked(SelectableElementViewModelBase source, SelectableElementViewModelBase target)
        {
            return IsPlaceToTransition(source, target) || IsTransitionToPlace(source, target);
        }

        private void AddPlace(object position)
        {
            Point itemPos = (Point)position; 
            var place = new PlaceViewModel(_petryNetModel.CreatAndGetPlace(Places.Count, "p" + Places.Count), itemPos.X, itemPos.Y, this);
            Places.Add(place);
            _allItems.Add(place);
        }

        private void AddTransition(object position)
        {
            Point itemPos = (Point)position;
            var transition = new TransitionViewModel(_petryNetModel.CreateAndGetTransition(Transitions.Count, "t" + Transitions.Count));
            transition.X = itemPos.X - transition.Width / 2f;
            transition.Y = itemPos.Y - transition.Height / 2f;
            transition.Parent = this;
            Transitions.Add(transition);
            _allItems.Add(transition);
        }

        public void ExecuteClearSelectedItemsCommand()
        {
            foreach (SelectableElementViewModelBase item in Items)
            {
                item.IsSelected = false;
            }
        }

        internal void AddToken(PlaceViewModel placeViewModel)
        {
            var token = new TokenModel();
            placeViewModel.AddToken(token);
        }

        internal void RemoveToken(PlaceViewModel placeViewModel)
        {
            placeViewModel.RemoveToken();
        }


        public PetryNetViewModel(PetryNetModel petriNetModel)
        {

        }

        public PetryNetViewModel()
        {
            _petryNetModel = new PetryNetModel();
        }

        public void StartSimulations()
        {
            CheckTransitionsState();
        }

        private void CheckTransitionsState()
        {
            foreach (var transition in Transitions)
            {
                transition.IsEnabled = IsTransitionEnabled(transition.Model);
            }
        }

        public void MakeStep()
        {
            List<TransitionViewModel> enableTransitions = Transitions.Where(transition => transition.IsEnabled).ToList();
            Random rnd = new Random();
            TransitionViewModel transitionToFire = enableTransitions[rnd.Next(0, enableTransitions.Count)];
            List<PlaceViewModel> places = GetConnectedPlacesToTransition(transitionToFire);
            _petryNetModel.FireTransition(transitionToFire.Model);

            foreach(var place in places)
            {
                place.SyncTokensWithModel();
            }

            CheckTransitionsState();
        }

        private List<PlaceViewModel> GetConnectedPlacesToTransition(TransitionViewModel transition)
        {
            List<PlaceViewModel> placeViewModels = new List<PlaceViewModel>();
            List<ArcViewModel> arcViews = Arcs.Where(arc => arc.Target == transition || arc.Source == transition).ToList();

            foreach (var arcView in arcViews)
            {
                if(arcView.Source is PlaceViewModel placeViewModel)
                {
                    placeViewModels.Add(placeViewModel);
                }
                else if (arcView.Target is PlaceViewModel place) 
                {
                    placeViewModels.Add(place);
                }
            }
            return placeViewModels;
        }

        public bool IsTransitionEnabled(TransitionModel transition)
        {
            return _petryNetModel.IsTransitionEnabled(transition);
        }

        //private void InitializeViewModels()
        //{
        //    foreach (var place in _petriNetModel.Places)
        //        Places.Add(new PlaceViewModel(place));

        //    foreach (var transition in _petriNetModel.Transitions)
        //        Transitions.Add(new TransitionViewModel(transition));

        //    foreach (var arc in _petriNetModel.Arcs)
        //        Arcs.Add(new ArcViewModel(arc));
        //}

        //private void CreateArc(NodeViewModel source, NodeViewModel target)
        //{
        //    if (source != null && target != null)
        //    {
        //        if (source is PlaceViewModel placeSoures && target is TransitionViewModel transitionTarget)
        //        {
        //            var arc = new ArcModel(_petriNetModel.Arcs.Count + 1, placeSoures.Model, transitionTarget.Model);
        //            // Add the Arc to the Model
        //            _petriNetModel.Arcs.Add(arc);
        //            // Create the ArcViewModel and add it to the ViewModel collection
        //            var arcViewModel = new ArcViewModel(arc, source, target);
        //            Arcs.Add(arcViewModel);
        //        }
        //        else if (source is TransitionViewModel transitionSoures && target is PlaceViewModel placeTarget)
        //        {
        //            var arc = new ArcModel(_petriNetModel.Arcs.Count + 1, transitionSoures.Model, placeTarget.Model);
        //            // Add the Arc to the Model
        //            _petriNetModel.Arcs.Add(arc);
        //            // Create the ArcViewModel and add it to the ViewModel collection
        //            var arcViewModel = new ArcViewModel(arc, source, target);
        //            Arcs.Add(arcViewModel);
        //        }

        //    }
        //}
    }
}
