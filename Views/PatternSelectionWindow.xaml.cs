using PetryNet.DTOS;
using PetryNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PetryNet.Views
{
    /// <summary>
    /// Interaction logic for PatternSelectionWindow.xaml
    /// </summary>
    public partial class PatternSelectionWindow : Window
    {

        public PatternPreview SelectedPattern { get; private set; }

        public PatternSelectionWindow(string patternsDir, string imagesDir)
        {
            InitializeComponent();

            var viewModel = new PatternSelectionViewModel();
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedPattern))
                {
                    SelectedPattern = viewModel.SelectedPattern;
                    DialogResult = true;
                    Close();
                }
            };

            DataContext = viewModel;
        }
    }
}
