using DBApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBApp.Views
{
    /// <summary>
    /// Interaction logic for AltMainWindow.xaml
    /// </summary>
    public partial class AltMainWindow : Window
    {
        public AltMainWindow()
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            DataContext = viewModel;

            viewModel.PersonAdded += (s, person) =>
            {
                PeopleListBox.ScrollIntoView(person);
                FirstNameTextBox.Focus();
                FirstNameTextBox.SelectAll();
            };
        }
    }
}
