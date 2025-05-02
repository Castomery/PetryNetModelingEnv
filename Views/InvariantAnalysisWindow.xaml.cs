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
            DataContext = new InvariantAnalysisViewModel(petrynetViewModel);
        }
    }
}
