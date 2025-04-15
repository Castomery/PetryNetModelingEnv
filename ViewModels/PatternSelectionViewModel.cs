using PetryNet.Commands;
using PetryNet.DTOS;
using PetryNet.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PetryNet.ViewModels
{
    public class PatternSelectionViewModel : BaseViewModel
    {
        const string PATH = "C:\\Інше\\темп\\Project\\PetryNet\\PetryNet\\Patterns";
        public ObservableCollection<PatternPreview> Patterns { get; set; }
        public PatternPreview SelectedPattern { get; set; }

        public PatternSelectionViewModel()
        {
            Patterns = new ObservableCollection<PatternPreview>(
                PatternLibrary.LoadAllPatternPreviews(PATH));
        }

        public ICommand SelectPatternCommand => new RelayCommand(p =>
        {
            SelectedPattern = p as PatternPreview;
            OnPropertyChanged(nameof(SelectedPattern));
        });
    }
}
