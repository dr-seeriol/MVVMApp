using MVVMApp.Data;
using MVVMApp.Models;
using MVVMApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using SQLitePCL;
using System.Collections.ObjectModel; // made me do this for observablecollection
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Batteries.Init(); //sqlite stuff don't touch it

            var viewModel = new MainViewModel();
            DataContext = viewModel;

            viewModel.PersonAdded += (s, person) =>
            {
                PeopleGrid.ScrollIntoView(person);
                FirstNameTextBox.Focus();
                FirstNameTextBox.SelectAll();
            };
        }
    }

}
  
    