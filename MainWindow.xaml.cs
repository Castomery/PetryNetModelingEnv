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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PetryNet.ViewModels.Interfaces;
using PetryNet.DTOS;
using Microsoft.Win32;

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
        private PatternPreview _pendingPatternToPlace;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = e.OriginalSource as FrameworkElement;
            var itemVm = frameworkElement?.DataContext as SelectableElementViewModelBase;

            // If it's an arc and we're in select mode, call the Arc method first
            //if (mainViewModel.CurrentMode == ApplicationMode.Select && itemVm !=  && item)
            //{
            //    Arc_MouseLeftButtonDown(frameworkElement, e);

            //    // If Arc_MouseLeftButtonDown handled the event, return early
            //    if (e.Handled)
            //        return;
            //}
            

            if (mainViewModel.CurrentMode == ApplicationMode.Select && itemVm == null)
            {
                mainViewModel.ClearSelectedItems();
            }

            var position = e.GetPosition(PetriNetCanvas);

            if (_pendingPatternToPlace != null)
            {
                mainViewModel.AddPattern(_pendingPatternToPlace.Pattern,false, position);
                _pendingPatternToPlace = null;
                return;
            }

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
                //default:
                //    mainViewModel.AddPattern(position);
                //    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if(mainViewModel.PetryNetViewModel.SelectedItems.Count > 0)
                {
                    mainViewModel.DeleteElements();            
                }
            }
        }

        private void OpenPatternSelectionWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new PatternSelectionWindow("Patterns", "Images");
            if (window.ShowDialog() == true)
            {
                _pendingPatternToPlace = window.SelectedPattern;
            }
        }

        private void Arc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                e.Handled = true;
                var arcVm = ((FrameworkElement)sender).DataContext as ArcViewModel;
                if (arcVm != null)
                {
                    var editWindow = new EditArcWindow(arcVm); // a new Window
                    editWindow.ShowDialog(); // opens the window modally
                }
            }
        }

        private void SavePetriNet_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save Petri Net"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    (DataContext as MainViewModel)?.SavePetriNet(dialog.FileName);
                    MessageBox.Show("Petri net saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadPetriNet_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Load Petri Net"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    (DataContext as MainViewModel)?.LoadPetriNet(dialog.FileName);
                    MessageBox.Show("Petri net loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Invalid or corrupted Petri net file:\n{ex.Message}", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void OnNewNet_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.ClearCurrentNet();
        }
    }
}
