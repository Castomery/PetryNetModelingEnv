using PetryNet.ViewModels.Core;
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
    /// Interaction logic for IncidenceMatrixWindow.xaml
    /// </summary>
    public partial class IncidenceMatrixWindow : Window
    {
        public IncidenceMatrixWindow(PetryNetViewModel model)
        {
            InitializeComponent();
            DataContext = new IncidenceMatrixViewModel(model);
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (IncidenceMatrixViewModel)DataContext;

            GenerateColumns(ForwardMatrixGrid, vm);
            GenerateColumns(BackwardMatrixGrid, vm);
            GenerateColumns(CombinedMatrixGrid, vm);
        }

        private void GenerateColumns(DataGrid grid, IncidenceMatrixViewModel vm)
        {
            if (vm?.Model?.Transitions == null) return;

            grid.Columns.Clear();

            // Add Place column
            grid.Columns.Add(new DataGridTextColumn
            {
                Header = "",
                Binding = new Binding("PlaceName"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Auto)
            });

            // Add transition columns
            foreach (var transition in vm.Model.Transitions)
            {
                grid.Columns.Add(new DataGridTextColumn
                {
                    Header = transition.Name,
                    Binding = new Binding($"Weights[{transition.Name}]"),
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
            }
        }
    }
}
