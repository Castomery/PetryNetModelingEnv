using PetryNet.Commands;
using PetryNet.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.ViewModels.Interfaces
{
    public interface IPetryNetViewModel
    {
        RelayCommand AddPlaceItemCommand { get; }
        RelayCommand AddTransitionItemCommand { get; }
        RelayCommand AddArcItemCommand { get; }
        RelayCommand RemoveItemCommand { get; }
        RelayCommand ClearSelectedItemsCommand { get; }
        List<SelectableElementViewModelBase> SelectedItems { get; }
        ObservableCollection<SelectableElementViewModelBase> Items { get; }
        ObservableCollection<PlaceViewModel> Places { get; }
        ObservableCollection<TransitionViewModel> Transitions { get; }
        ObservableCollection<ArcViewModel> Arcs { get; }
    }
}
