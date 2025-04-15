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
    /// Interaction logic for EditArcWindow.xaml
    /// </summary>
    public partial class EditArcWindow : Window
    {
        public EditArcWindow(ArcViewModel arcViewModel)
        {
            InitializeComponent();
            DataContext = arcViewModel;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
