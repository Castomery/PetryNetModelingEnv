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
using System.Windows.Shapes;

namespace PetryNet.Views
{
    /// <summary>
    /// Interaction logic for InvariantAnalysisWindow.xaml
    /// </summary>
    public partial class InvariantAnalysisWindow : Window
    {
        public InvariantAnalysisWindow(PetryNetViewModel petrynetViewModel)
        {
            InitializeComponent();
            var vm = new InvariantAnalysisViewModel(petrynetViewModel);
            DataContext = vm;

            GenerateTables(vm);
        }

        private void GenerateTables(InvariantAnalysisViewModel vm)
        {
            var stackPanel = new StackPanel { Margin = new Thickness(5) };

            // --- T-Invariants ---
            stackPanel.Children.Add(new TextBlock
            {
                Text = "T-Invariants",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            foreach (var row in vm.TInvariants)
            {
                var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

                // Name
                rowPanel.Children.Add(new Border
                {
                    Background = Brushes.Blue,
                    Width = 40,
                    Height = 30,
                    Child = new TextBlock
                    {
                        Text = row.Name,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                });

                // Vector
                for (int j = 0; j < row.Vector.Count; j++)
                {
                    rowPanel.Children.Add(new Border
                    {
                        Width = 40,
                        Height = 30,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0, 1, 1, 1),
                        Background = vm.ZeroColumnsT.Contains(j) ? Brushes.Red : Brushes.Transparent,
                        Child = new TextBlock
                        {
                            Text = row.Vector[j].ToString(),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    });
                }

                stackPanel.Children.Add(rowPanel);
            }

            // --- P-Invariants ---
            stackPanel.Children.Add(new TextBlock
            {
                Text = "P-Invariants",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(0, 10, 0, 5)
            });

            foreach (var row in vm.PInvariants)
            {
                var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

                // Name
                rowPanel.Children.Add(new Border
                {
                    Background = Brushes.Blue,
                    Width = 40,
                    Height = 30,
                    Child = new TextBlock
                    {
                        Text = row.Name,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                });

                // Vector
                for (int j = 0; j < row.Vector.Count; j++)
                {
                    rowPanel.Children.Add(new Border
                    {
                        Width = 40,
                        Height = 30,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0, 1, 1, 1),
                        Background = vm.ZeroColumnsP.Contains(j) ? Brushes.Red : Brushes.Transparent,
                        Child = new TextBlock
                        {
                            Text = row.Vector[j].ToString(),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    });
                }

                stackPanel.Children.Add(rowPanel);
            }

            // Add to ScrollViewer
            InvariantsScrollViewer.Content = stackPanel;
        }


    }
}
