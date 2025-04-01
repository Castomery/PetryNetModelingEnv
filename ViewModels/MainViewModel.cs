using PetryNet.Commands;
using PetryNet.Enums;
using PetryNet.Models;
using PetryNet.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PetryNet.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public PetryNetViewModel PetryNetViewModel { get; private set; }

        public  ApplicationMode _currentMode;
        public ApplicationMode CurrentMode
        {
            get => _currentMode;
            set
            {
                if (_currentMode != value)
                {
                    _currentMode = value;
                    SetProperty( ref _currentMode,value);
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
            CurrentMode = ApplicationMode.Select;
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
    }
}
