using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TestBenchTarget.UWP.Models;
using TestBenchTarget.UWP.Services;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace TestBenchTarget.UWP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private string currentTime = string.Empty;
        private DateTimeOffset selectedDate = DateTimeOffset.Now;
        private string procedureText = string.Empty;
        private string pointsText = "0";
        private string delegateText = string.Empty;
        private DataItem? selectedItem = null;
        private DispatcherTimer timer;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(DataService dataService)
        {
            _dataService = dataService;
            
            // Nastavenie časovača
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");

            // Inicializácia príkazov
            AddCommand = new RelayCommand(AddItem);
            DeleteCommand = new RelayCommand(DeleteItem, CanDeleteItem);
            SaveCommand = new RelayCommand(async () => await SaveDataAsync());
            LoadCommand = new RelayCommand(async () => await LoadDataAsync());
            OpenFolderCommand = new RelayCommand(async () => await OpenFolderAsync());
            
            // Načítanie existujúcich údajov
            _ = LoadDataAsync();
        }

        private void Timer_Tick(object? sender, object? e)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
        }

        // Properties
        public string CurrentTime 
        { 
            get => currentTime!; 
            set 
            { 
                currentTime = value; 
                OnPropertyChanged(nameof(CurrentTime)); 
            } 
        }

        public DateTimeOffset SelectedDate 
        { 
            get => selectedDate; 
            set 
            { 
                selectedDate = value; 
                OnPropertyChanged(nameof(SelectedDate)); 
            } 
        }

        public string ProcedureText 
        { 
            get => procedureText; 
            set 
            { 
                procedureText = value; 
                OnPropertyChanged(nameof(ProcedureText)); 
            } 
        }

        public string PointsText 
        { 
            get => pointsText; 
            set 
            { 
                // Validácia - prijíma iba číslice
                string cleanedText = string.Empty;
                foreach (char c in value)
                {
                    if (char.IsDigit(c))
                    {
                        cleanedText += c;
                    }
                }
                
                if (string.IsNullOrEmpty(cleanedText))
                {
                    cleanedText = "0";
                }
                
                pointsText = cleanedText;
                OnPropertyChanged(nameof(PointsText));
            } 
        }

        public string DelegateText 
        { 
            get => delegateText; 
            set 
            { 
                delegateText = value; 
                OnPropertyChanged(nameof(DelegateText)); 
            } 
        }

        public CustomObservableCollection<DataItem> DataItems => _dataService.DataList;

        public DataItem SelectedItem 
        { 
            get => selectedItem!; 
            set 
            { 
                selectedItem = value; 
                OnPropertyChanged(nameof(SelectedItem));
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            } 
        }

        // Príkazy
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand OpenFolderCommand { get; }

        // Implementácia príkazov
        private void AddItem()
        {
            // Konverzia a validácia hodnôt
            int pointsValue = 0;
            if (!string.IsNullOrEmpty(PointsText))
            {
                if (!int.TryParse(PointsText, out pointsValue))
                {
                    pointsValue = 0;
                }
            }

            // Vytvorenie a pridanie nového záznamu
            DataItem newItem = new DataItem
            {
                DateColumnValue = SelectedDate.DateTime,
                ProcedureColumnValue = ProcedureText,
                PointsColumnValue = pointsValue,
                DelegateColumnValue = DelegateText
            };

            // Pridanie do kolekcie (vloží na začiatok)
            _dataService.DataList.Add(newItem);

            // Reset formulárových polí
            ProcedureText = string.Empty;
            PointsText = "0";
            DelegateText = string.Empty;
        }

        private bool CanDeleteItem()
        {
            return SelectedItem != null;
        }

        private void DeleteItem()
        {
            if (SelectedItem != null)
            {
                _dataService.DataList.Remove(SelectedItem);
                SelectedItem = null!;
            }
        }

        private async Task SaveDataAsync()
        {
            bool success = await _dataService.SaveDataAsync();
            if (success)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Data saved successfully.");
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Failed to save data.");
                await dialog.ShowAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            await _dataService.LoadDataAsync();
        }

        private async Task OpenFolderAsync()
        {
            try
            {
                StorageFolder folder = await _dataService.GetLocalFolderAsync();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch (Exception)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Could not open folder.");
                await dialog.ShowAsync();
            }
        }
    }

    // Pomocná trieda pre príkazy
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null!)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => 
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
