using Newtonsoft.Json;
using PetryNet.Commands;
using PetryNet.DTOS;
using PetryNet.Enums;
using PetryNet.Models;
using PetryNet.Utils;
using PetryNet.ViewModels.Core;
using PetryNet.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PetryNet.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public PetryNetViewModel PetryNetViewModel { get; private set; }

        private PatternLoader _patternLoader;

        private bool _isSafetyNet;
        public bool IsSafetyNet
        {
            get => _isSafetyNet;
            set
            {
                if (_isSafetyNet != value)
                {
                    _isSafetyNet = value;
                    OnPropertyChanged();

                    PetryNetViewModel.SetIsSafeNet(_isSafetyNet);

                    // Apply token limit logic to existing places if needed
                    if (_isSafetyNet)
                    {
                        ApplyTokenLimits(1);
                    }
                    else
                    {
                        ApplyTokenLimits(_evaluationLimit);
                    }
                }
            }
        }

        private void ApplyTokenLimits(int placeLimit)
        {
            PetryNetViewModel.SetPlaceLimit(placeLimit);
        }

        // Optional: Global evaluation limit (used if not per-place)
        private int _evaluationLimit = 5;
        public int EvaluationLimit
        {
            get => _evaluationLimit;
            set
            {
                if (_evaluationLimit != value)
                {
                    _evaluationLimit = value;
                    OnPropertyChanged();
                    ApplyTokenLimits(_evaluationLimit);
                }
            }
        }

        public ApplicationMode _currentMode;
        public ApplicationMode CurrentMode
        {
            get => _currentMode;
            set
            {
                if (_currentMode != value)
                {
                    _currentMode = value;
                    SetProperty(ref _currentMode, value);
                }
            }
        }

        public event EventHandler<bool> IsSelectionModeChanged;

        private bool _isSelecting;
        public bool IsSelecting
        {
            get => _isSelecting;
            set
            {
                if (_isSelecting != value)
                {
                    _isSelecting = value;
                    OnPropertyChanged(nameof(IsSelecting));
                    IsSelectionModeChanged?.Invoke(this, _isSelecting);  // Notify listeners
                }
            }
        }

        public MainViewModel()
        {
            PetryNetViewModel = new PetryNetViewModel();
            _patternLoader = new PatternLoader();
            SetSelectModeCommand.Execute(true);
        }

        public ICommand SetSelectModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.Select; IsSelecting = true; ClearSelectedItems(); });
        public ICommand SetAddPlaceModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddPlace; IsSelecting = false; ClearSelectedItems(); });
        public ICommand SetAddTransitionModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddTransition; IsSelecting = false; ClearSelectedItems(); });
        public ICommand SetAddArcModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddArc; IsSelecting = false; ClearSelectedItems(); });
        public ICommand SetAddTokenModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddToken; IsSelecting = false; ClearSelectedItems(); });
        public ICommand SetRemoveTokenModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.RemoveToken; IsSelecting = false; ClearSelectedItems(); });
        public ICommand StartSimulationCommand => new RelayCommand(_ => { StartSimulation(); ClearSelectedItems(); } );
        public ICommand MakeStepCommand => new RelayCommand(_ => MakeStep());
        public ICommand ShowInvariantAnalysisCommand => new RelayCommand(_ => ExecuteShowInvariantAnalysis());

        public void AddPlace(Point position)
        {

            PetryNetViewModel.AddPlaceItemCommand.Execute(position);
        }

        public void AddTransition(Point position)
        {
            PetryNetViewModel.AddTransitionItemCommand.Execute(position);
        }

        public void AddPattern(PatternDTO patternDTO, bool useSavedCoords, bool isPattern, Point position)
        {
            PetryNetViewModel.ApplyPatternToCanvas(patternDTO, useSavedCoords, isPattern, position);
        }

        internal void ClearSelectedItems()
        {
            PetryNetViewModel.ExecuteClearSelectedItemsCommand();
        }

        internal void AddArc(SelectableElementViewModelBase firstClickedElement, SelectableElementViewModelBase secondClickedElement)
        {
            var arcData = Tuple.Create(firstClickedElement, secondClickedElement);

            PetryNetViewModel.AddArcItemCommand.Execute(arcData);
        }

        internal void AddToken(PlaceViewModel placeViewModel)
        {
            PetryNetViewModel.AddToken(placeViewModel);
        }

        internal void RemoveToken(PlaceViewModel placeViewModel)
        {
            PetryNetViewModel.RemoveToken(placeViewModel);
        }

        internal void StartSimulation()
        {
            PetryNetViewModel.StartSimulations();
        }

        private void MakeStep()
        {
            PetryNetViewModel.MakeStep();
        }

        public void SavePetriNet(string filePath)
        {
            var pattern = new PatternDTO
            {
                Places = PetryNetViewModel.Places.Select(p => new PlaceDTO
                {
                    Id = p.Name,
                    X = p.X,
                    Y = p.Y,
                    TokenLimit = p.Model.TokenLimit
                }).ToList(),

                Transitions = PetryNetViewModel.Transitions.Select(t => new TransitionDTO
                {
                    Id = t.Name,
                    X = t.X,
                    Y = t.Y
                }).ToList(),

                Arcs = PetryNetViewModel.Arcs.Select(a => new ArcDTO
                {
                    SourceId = a.Source switch
                    {
                        PlaceViewModel p => p.Name,
                        TransitionViewModel t => t.Name,
                    },

                    TargetId = a.Target switch
                    {
                        PlaceViewModel p => p.Name,
                        TransitionViewModel t => t.Name,
                    },
                    Weight = a.Weight
                }).ToList(),
                IsSafe = _isSafetyNet

            };

            var json = JsonConvert.SerializeObject(pattern, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void LoadPetriNet(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.");

            var json = File.ReadAllText(filePath);
            PatternDTO pattern;

            try
            {
                pattern = JsonConvert.DeserializeObject<PatternDTO>(json);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("File format is invalid.", ex);
            }

            if (pattern.Places == null || pattern.Transitions == null || pattern.Arcs == null)
                throw new InvalidDataException("Missing required parts of the Petri net.");

            ClearCurrentNet();
            AddPattern(pattern, true, false, new Point(0, 0));
            _isSafetyNet = pattern.IsSafe;
        }

        public ICommand ShowIncidenceMatrixCommand => new RelayCommand(_ => ShowIncidenceMatrix());

        private void ShowIncidenceMatrix()
        {
            var window = new IncidenceMatrixWindow(PetryNetViewModel);
            window.ShowDialog();
        }

        private void ExecuteShowInvariantAnalysis()
        {
            var window = new InvariantAnalysisWindow(PetryNetViewModel);
            window.ShowDialog();
        }

        public void ClearCurrentNet()
        {
            PetryNetViewModel.ClearNet();
        }

        internal void DeleteElements()
        {
            PetryNetViewModel.DeleteElements();
        }
    }
}
