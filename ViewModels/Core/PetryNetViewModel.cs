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
using PetryNet.DTOS;

namespace PetryNet.ViewModels.Core
{
    public class PetryNetViewModel : BaseViewModel, IPetryNetViewModel
    {
        private int indexForPlaces = 0;
        private int indexForTransitions = 0;
        private int indexForArcs = 0;
        private readonly PetryNetModel _petryNetModel;

        private List<SelectableElementViewModelBase> itemsToRemove;

        public ObservableCollection<PlaceViewModel> Places { get; } = new();
        public ObservableCollection<TransitionViewModel> Transitions { get; } = new();
        public ObservableCollection<ArcViewModel> Arcs { get; } = new();
        private ObservableCollection<SelectableElementViewModelBase> _allItems = new();

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
                        arc = _petryNetModel.CreateAndGetArc(indexForArcs, placeModel, transitionModel);

                    }
                    else if (firstClickedElement is TransitionViewModel && secondClickedElement is PlaceViewModel)
                    {
                        transitionModel = (firstClickedElement as TransitionViewModel).Model;
                        placeModel = (secondClickedElement as PlaceViewModel).Model;
                        arc = _petryNetModel.CreateAndGetArc(indexForArcs, transitionModel, placeModel);
                    }

                    if (arc != null)
                    {
                        ArcViewModel arcViewModel = new ArcViewModel(arc, (NodeViewModel)firstClickedElement, (NodeViewModel)secondClickedElement);
                        arcViewModel.Parent = this;
                        indexForArcs++;
                        Arcs.Add(arcViewModel);
                        _allItems.Add(arcViewModel);
                    }
                }
            }
        }

        private bool IsArcAlreadyExist(SelectableElementViewModelBase firstClickedElement, SelectableElementViewModelBase secondClickedElement)
        {
            ArcViewModel arcViewModel = Arcs.Where(arc => arc.Source == firstClickedElement && arc.Target == secondClickedElement).FirstOrDefault();

            if (arcViewModel == null)
            {
                return false;
            }
            else
            {
                arcViewModel.IncreaseWeightByOne();
                OnPropertyChanged("Weight");
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
            var place = new PlaceViewModel(_petryNetModel.CreatAndGetPlace(indexForPlaces, "p" + indexForPlaces), itemPos.X, itemPos.Y, this);
            indexForPlaces++;
            Places.Add(place);
            _allItems.Add(place);
        }

        private void AddTransition(object position)
        {
            Point itemPos = (Point)position;
            var transition = new TransitionViewModel(_petryNetModel.CreateAndGetTransition(indexForTransitions, "t" + indexForTransitions));
            transition.X = itemPos.X - transition.Width / 2f;
            transition.Y = itemPos.Y - transition.Height / 2f;
            transition.Parent = this;
            indexForTransitions++;
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

            foreach (var place in places)
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
                if (arcView.Source is PlaceViewModel placeViewModel)
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

        public void ApplyPatternToCanvas(PatternDTO pattern, bool useStoredCoordinates, Point? position = null)
        {
            var idToElementMap = new Dictionary<string, NodeViewModel>();

            //double minX = Math.Min(pattern.Places.Min(p => p.X), pattern.Transitions.Min(t => t.X));
            //double minY = Math.Min(pattern.Places.Min(p => p.Y), pattern.Transitions.Min(t => t.Y));

            var p0 = pattern.Places.FirstOrDefault(p => p.Id == "p0");
            if (p0 == null)
                throw new InvalidOperationException("Pattern must have a place with ID 'p0'.");

            //double originX = p0.X;
            //double originY = p0.Y;

            //// 2. Calculate offset from pattern origin to clicked position
            //double offsetX = position.X - originX;
            //double offsetY = position.Y - originY;

            double offsetX = 0;
            double offsetY = 0;

            if (!useStoredCoordinates && position.HasValue)
            {
                double minX = Math.Min(pattern.Places.Min(p => p.X), pattern.Transitions.Min(t => t.X));
                double minY = Math.Min(pattern.Places.Min(p => p.Y), pattern.Transitions.Min(t => t.Y));

                offsetX = position.Value.X - minX;
                offsetY = position.Value.Y - minY;
            }

            // Add places
            foreach (var placeDto in pattern.Places)
            {
                Point pos = new Point();
                pos.X = placeDto.X + offsetX;
                pos.Y = placeDto.Y + offsetY;

                AddPlace(pos);
                idToElementMap[placeDto.Id] = Places.Last();
            }

            // Add transitions
            foreach (var transitionDto in pattern.Transitions)
            {
                Point pos = new Point();
                pos.X = transitionDto.X + offsetX;
                pos.Y = transitionDto.Y + offsetY;
                AddTransition(pos);
                idToElementMap[transitionDto.Id] = Transitions.Last();
            }

            // Add arcs
            foreach (var arcDto in pattern.Arcs)
            {
                if (!idToElementMap.ContainsKey(arcDto.SourceId) || !idToElementMap.ContainsKey(arcDto.TargetId))
                    continue; // skip invalid arc

                var source = idToElementMap[arcDto.SourceId];
                var target = idToElementMap[arcDto.TargetId];

                ArcModel arcModel = null;

                if (source is PlaceViewModel && target is TransitionViewModel)
                {
                    arcModel = _petryNetModel.CreateAndGetArc(indexForArcs, ((PlaceViewModel)source).Model, ((TransitionViewModel)target).Model);
                }
                else if (source is TransitionViewModel && target is PlaceViewModel)
                {
                    arcModel = _petryNetModel.CreateAndGetArc(indexForArcs, ((TransitionViewModel)source).Model, ((PlaceViewModel)target).Model);
                }

                arcModel.ChangeWeight(arcDto.Weight);

                if (arcModel != null)
                {
                    var arcVm = new ArcViewModel(arcModel, source, target)
                    {
                        Parent = this
                    };
                    Arcs.Add(arcVm);
                    _allItems.Add(arcVm);
                    indexForArcs++;
                }
            }
        }

        internal void ClearNet()
        {
            _petryNetModel.ClearNet();
            Places.Clear();
            Transitions.Clear();
            Arcs.Clear();
            indexForPlaces = 0;
            indexForTransitions = 0;
            indexForArcs = 0;
            _allItems.Clear();
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
