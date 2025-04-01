using PetryNet.Enums;
using PetryNet.Models;
using PetryNet.ViewModels;
using PetryNet.ViewModels.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PetryNet.ViewModels.Interfaces;

namespace PetryNet.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        private MainViewModel mainViewModel = new MainViewModel();
        private SelectableElementViewModelBase firstClickedElement;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled)
                return;

            

            if (mainViewModel.CurrentMode == ApplicationMode.Select && !(sender is SelectableElementViewModelBase))
            {
                mainViewModel.ClearSelectedItems();
            }

            var position = e.GetPosition(PetriNetCanvas);

            switch (mainViewModel.CurrentMode)
            {
                case ApplicationMode.AddPlace:
                    mainViewModel.AddPlace(position);
                    break;
                case ApplicationMode.AddTransition:
                    mainViewModel.AddTransition(position);
                    break;
                case ApplicationMode.AddArc:

                    var clickedElement = (e.OriginalSource as FrameworkElement)?.DataContext as SelectableElementViewModelBase;
                    // Handle AddArc mode here
                    if (clickedElement != null)
                    {
                        if (firstClickedElement == null)
                        {
                            // First click: store the source element
                            firstClickedElement = clickedElement;
                        }
                        else
                        {
                            // Second click: store the target element and create the arc
                            SelectableElementViewModelBase secondClickedElement = clickedElement;
                            mainViewModel.AddArc(firstClickedElement, secondClickedElement);
                            firstClickedElement = null;
                        }
                    }
                    break;
                case ApplicationMode.AddToken:
                    var clickedPlace = (e.OriginalSource as FrameworkElement)?.DataContext as PlaceViewModel;
                    if (clickedPlace != null)
                    {
                        mainViewModel.AddToken(clickedPlace);
                    }
                    break;

                case ApplicationMode.RemoveToken:
                    var clickedP = (e.OriginalSource as FrameworkElement)?.DataContext as PlaceViewModel;
                    if (clickedP != null)
                    {
                        mainViewModel.RemoveToken(clickedP);
                    }
                    break;
            }
        }
    }
}
