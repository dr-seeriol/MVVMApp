using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DBApp.Data;
using DBApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DBApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private enum SortMode { Name, Age }

        private readonly PeopleContext database = new();
        private SortMode currentSort = SortMode.Name;

        private Person? selectedPerson;
        private string searchText = "";
        private string sortStatus = "";

        public ObservableCollection<Person> People { get; } = new();

        public Person? SelectedPerson
        {
            get => selectedPerson;
            set
            {
                if (selectedPerson == value) return;
                selectedPerson = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText == value) return;
                searchText = value;
                OnPropertyChanged();
                RefreshPeople();
            }
        }

        public string SortStatus
        {
            get => sortStatus;
            private set { sortStatus = value; OnPropertyChanged(); }
        }

        public ICommand AddPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand SortByNameCommand { get; }
        public ICommand SortByAgeCommand { get; }

        
        public event EventHandler<Person>? PersonAdded;

        public MainViewModel()
        {
            database.Database.EnsureCreated();
            database.People.Load();

            AddPersonCommand = new RelayCommand(_ => AddPerson());
            DeletePersonCommand = new RelayCommand(_ => DeletePerson(), _ => SelectedPerson != null);
            SaveChangesCommand = new RelayCommand(_ => database.SaveChanges());
            SortByNameCommand = new RelayCommand(_ => { currentSort = SortMode.Name; RefreshPeople(); });
            SortByAgeCommand = new RelayCommand(_ => { currentSort = SortMode.Age; RefreshPeople(); });

            RefreshPeople();
        }

        private void RefreshPeople()
        {
            Person? previouslySelected = SelectedPerson;

            IEnumerable<Person> query = database.People.Local;

            if (!string.IsNullOrEmpty(SearchText))
            {
                query = query.Where(person =>
                    person.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    person.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (currentSort == SortMode.Name)
            {
                query = query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
                SortStatus = "Sorting by: Last, First name";
            }
            else
            {
                query = query.OrderBy(p => p.Age);
                SortStatus = "Sorting by: Age";
            }

            var results = query.ToList();

            People.Clear();
            foreach (Person person in results)
            {
                People.Add(person);
            }

            if (previouslySelected != null && People.Contains(previouslySelected))
            {
                SelectedPerson = previouslySelected;
            }
        }

        private void AddPerson()
        {
            SearchText = "";

            Person person = new Person { FirstName = "New", LastName = "Person", Age = 18 };
            database.People.Add(person);
            RefreshPeople();

            SelectedPerson = person;
            PersonAdded?.Invoke(this, person);
        }

        private void DeletePerson()
        {
            if (SelectedPerson is Person person)
            {
                database.People.Remove(person);
                SelectedPerson = null;
                RefreshPeople();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}