using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TestBenchTarget.UWP.Models;
using TestBenchTarget.UWP.Services;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestBenchTarget.UWP.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        // Properties // Vlastnosti
        private CustomObservableCollection<DataItem> _dataItems = new CustomObservableCollection<DataItem>();
        public CustomObservableCollection<DataItem> DataItems
        {
            get => _dataItems;
            set => SetProperty(ref _dataItems, value);
        }

        private DataItem? _selectedItem = null;
        public DataItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                System.Diagnostics.Debug.WriteLine($"SelectedItem setter called, new value is null: {value == null}");
                if (SetProperty(ref _selectedItem, value))
                {
                    // Notification of command about possible state change // Notifikácia príkazu o možnej zmene stavu
                    DeleteCommand.NotifyCanExecuteChanged();
                    System.Diagnostics.Debug.WriteLine("NotifyCanExecuteChanged called on DeleteCommand");
                }
            }
        }

        private string _dateFormat = "dd.MM.yyyy";
        public string DateFormat
        {
            get => _dateFormat;
            set
            {
                if (SetProperty(ref _dateFormat, value))
                {
                    // Ak sa zmenil formát, preparsujeme existujúci dátum a naformátujeme ho na nový formát
                    if (DateTime.TryParseExact(_selectedDateString, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date) ||
                        DateTime.TryParseExact(_selectedDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out date) ||
                        DateTime.TryParseExact(_selectedDateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out date))
                    {
                        SelectedDateString = date.ToString(value);
                    }
                }
            }
        }

        private string _selectedDateString = DateTime.Now.ToString("dd.MM.yyyy");
        public string SelectedDateString
        {
            get => _selectedDateString;
            set => SetProperty(ref _selectedDateString, value);
        }

        private string _procedureText = string.Empty;
        public string ProcedureText
        {
            get => _procedureText;
            set => SetProperty(ref _procedureText, value);
        }

        private string _pointsText = "0";
        public string PointsText
        {
            get => _pointsText;
            set => SetProperty(ref _pointsText, value);
        }

        private string _delegateText = string.Empty;
        public string DelegateText
        {
            get => _delegateText;
            set => SetProperty(ref _delegateText, value);
        }

        // Events // Deklarácie udalosti
        public event EventHandler? ListClearedSuccessfully;
        public event EventHandler? DataSavedSuccessfully;


        // Commands // Príkazy
        public IRelayCommand AddCommand { get; }
        public IRelayCommand LoadCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand DeleteCommand { get; }
        public IRelayCommand OpenFolderCommand { get; }
        public IRelayCommand ClearFormCommand { get; }
        public IRelayCommand ClearListCommand { get; }
        public IRelayCommand ExportDataCommand { get; }

        public MainViewModel(DataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

            // Loading data during initialization // Načítanie dát pri inicializácii 
            DataItems = _dataService.DataList;

            // Initialization of commands
            AddCommand = new RelayCommand(AddData);
            LoadCommand = new RelayCommand(LoadData);
            SaveCommand = new RelayCommand(SaveData);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            ClearFormCommand = new RelayCommand(ClearForm);
            ClearListCommand = new RelayCommand(ClearList);
            ExportDataCommand = new RelayCommand(ExportData);

            // Initialization of data loading
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                await _dataService.LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing data: {ex.Message}");
            }
        }

        private void AddData()
        {
            try
            {
                DateTime selectedDate;

                // Skúsime konvertovať dátum podľa aktuálneho formátu
                if (!DateTime.TryParseExact(_selectedDateString, _dateFormat, null,
                    System.Globalization.DateTimeStyles.None, out selectedDate))
                {
                    selectedDate = DateTime.Now.Date;
                }

                // Create new DataItem from input values // Vytvorenie nového DataItem z vstupných hodnôt
                var newItem = new DataItem
                {
                    DateColumnValue = selectedDate,
                    ProcedureColumnValue = ProcedureText,
                    // Parse PointsText to integer
                    PointsColumnValue = string.IsNullOrEmpty(PointsText) ? 0 : int.Parse(PointsText),
                    DelegateColumnValue = DelegateText
                };

                // Add new item to the collection // Pridanie položky do kolekcie
                DataItems.Add(newItem);

                // Reset and clear input fields // Vynulovanie vstupných polí
                //ProcedureText = string.Empty;
                //PointsText = "0";
                //DelegateText = string.Empty;

                SelectedItem = newItem;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding item: {ex.Message}");
            }
        }

        private async void LoadData()
        {
            try
            {
                await _dataService.LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                await ShowErrorDialog("Error loading data", ex.Message);
            }
        }

        private async void SaveData()
        {
            try
            {
                bool success = await _dataService.SaveDataAsync();
                if (success)
                {
                    // Event for successful save - instead of showing a dialog - SaveSuccessTeachingTip
                    // Namiesto zobrazenia dialógu vyvolajte event, ktorý XAML stránka zachytí
                    // TeachingTip
                    System.Diagnostics.Debug.WriteLine("Data saved successfully.");
                    DataSavedSuccessfully?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await ShowErrorDialog("Error saving data", "Failed to save data.");
                }
            }
            catch (Exception ex)
            {
                // if happens exception in SaveDataAsync run TeachingTip SaveErrorNotification
                // Ak nastane výnimka v SaveDataAsync, spustí sa SaveErrorNotification
                // TeachingTip

                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
                await ShowErrorDialog("Error saving data", ex.Message);
            }
        }

        private void Delete()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Delete called, SelectedItem: {SelectedItem != null}");

                if (SelectedItem != null)
                {
                    DataItems.Remove(SelectedItem);
                    SelectedItem = null;
                    System.Diagnostics.Debug.WriteLine("Item deleted successfully");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting item: {ex.Message}");
            }
        }

        private bool CanDelete()
        {
            return SelectedItem != null;
        }

        private async void OpenFolder()
        {
            try
            {
                var folder = await _dataService.GetLocalFolderAsync();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening folder: {ex.Message}");
                await ShowErrorDialog("Error opening folder", ex.Message);
            }
        }

        private async Task ShowErrorDialog(string title, string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "OK"
            };

            await errorDialog.ShowAsync();
        }

        //SaveDataAsync
        public async Task<bool> SaveDataAsync(string filePath)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync(filePath,
                                    CreationCollisionOption.ReplaceExisting);
                string jsonData = JsonConvert.SerializeObject(DataItems, Formatting.Indented);
                await FileIO.WriteTextAsync(file, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveDataAsync: {ex.Message}");
                return false;
            }
        }

        private void ClearForm()
        {
            ProcedureText = string.Empty;
            PointsText = "0";
            DelegateText = string.Empty;

            // SelectedDateString = DateTime.Now.ToString("dd.MM.yyyy"); // Today date set?
        }

        private async void ClearList()
        {

            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirm deletion",
                Content = "Are you sure you want to clear the entire list? This action cannot be undone when data is not saved in JSON file.",
                PrimaryButtonText = "Yes, clear list",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await confirmDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                DataItems.Clear(); // clear the colection of DataItems
                SelectedItem = null; // set SelectedItem to null, now it is not selected
                System.Diagnostics.Debug.WriteLine("List cleared");
                DeleteCommand.NotifyCanExecuteChanged();
                ListClearedSuccessfully?.Invoke(this, EventArgs.Empty);
            }
        }

        private async void ExportData()
        {
            var page = Window.Current.Content as Frame;
            var mainPage = page?.Content as MainPage;
            if (mainPage != null)
            {
                await mainPage.ExportDataAsync();
            }
        }
    }
}
