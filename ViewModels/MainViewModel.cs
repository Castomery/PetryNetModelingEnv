using Newtonsoft.Json;
using PetryNet.Commands;
using PetryNet.DTOS;
using PetryNet.Enums;
using PetryNet.Models;
using PetryNet.Utils;
using PetryNet.ViewModels.Core;
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

        public ICommand SetSelectModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.Select; IsSelecting = true; });
        public ICommand SetAddPlaceModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddPlace; IsSelecting = false; });
        public ICommand SetAddTransitionModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddTransition; IsSelecting = false; });
        public ICommand SetAddArcModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddArc; IsSelecting = false; });
        public ICommand SetAddTokenModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.AddToken; IsSelecting = false; });
        public ICommand SetRemoveTokenModeCommand => new RelayCommand(_ => { CurrentMode = ApplicationMode.RemoveToken; IsSelecting = false; });
        public ICommand StartSimulationCommand => new RelayCommand(_ => StartSimulation());
        public ICommand MakeStepCommand => new RelayCommand(_ => MakeStep());

        public void AddPlace(Point position)
        {

            PetryNetViewModel.AddPlaceItemCommand.Execute(position);
        }

        public void AddTransition(Point position)
        {
            PetryNetViewModel.AddTransitionItemCommand.Execute(position);
        }

        public void AddPattern(PatternDTO patternDTO, bool useSavedCoords, Point position)
        {
            PetryNetViewModel.ApplyPatternToCanvas(patternDTO, useSavedCoords, position);
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
                    Y = p.Y
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
                        _ => null
                    },

                    TargetId = a.Target switch
                    {
                        PlaceViewModel p => p.Name,
                        TransitionViewModel t => t.Name,
                        _ => null
                    },
                    Weight = a.Weight
                }).ToList()
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
            AddPattern(pattern, true, new Point(0, 0));
        }

        public void ClearCurrentNet()
        {
            PetryNetViewModel.ClearNet();
        }
    }
}
