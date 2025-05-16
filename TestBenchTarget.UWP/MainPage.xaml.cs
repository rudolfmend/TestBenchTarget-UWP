using System;
using System.IO;
using System.Collections.Generic;
using TestBenchTarget.UWP.Models;
using TestBenchTarget.UWP.Services;
using TestBenchTarget.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;                                                                                                       
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace TestBenchTarget.UWP
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;
        private MainViewModel _viewModel;
        private CustomObservableCollection<DataItem> _dataItems = new CustomObservableCollection<DataItem>();

        public MainPage()
        {
            this.InitializeComponent();
                        
            //TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");

            //_timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromSeconds(1);
            //_timer.Tick += (s, e) => {
            //    TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            //};
            //_timer.Start();

            // Inicializácia ViewModelu a nastavenie DataContext
            _viewModel = new MainViewModel(new DataService());
            this.DataContext = _viewModel;

            InitializeDateSelector(); // - ComboBox for date selection initialization

            this.Unloaded += MainPage_Unloaded; // - Unloading event registration (for cleaning resources)
             
            _viewModel.DataSavedSuccessfully += ViewModel_DataSavedSuccessfully!;
            _viewModel.ListClearedSuccessfully += ViewModel_ListClearedSuccessfully!;
        }

        private async void ViewModel_ListClearedSuccessfully(object sender, EventArgs e)
        {
            // Display TeachingTip  
            ClearListSuccessNotification.IsOpen = true;

            // Auto-close after 3 seconds  
            await Task.Run(() =>
            {
                Task.Delay(3000).Wait();
            });

            ClearListSuccessNotification.IsOpen = false;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Zastavenie a vyčistenie časovača // Stop and clean the timer
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null!;
            }
        }

        private void InitializeDateSelector()
        {
            // Fill ComBoBox with dates // Naplnenie ComboBoxu dátumami
            var dateItems = new List<string>();
            for (int i = -367; i <= 367; i++)
            {
                dateItems.Add(DateTime.Now.AddDays(i).ToString("dd.MM.yyyy"));
            }

            DateSelector.ItemsSource = dateItems;
            DateSelector.SelectedIndex = 367; // Today´s date is in the middle // Dnešný dátum je v strede
        }

        // Method for loading data from JSON file // Metóda pre uloženie (načítanie) dát zo JSON súboru 
        private async void SaveDataToJsonFile()
        {
            try
            {
                string jsonFilePath = GetDefaultJsonFilePath();
                bool result = await _viewModel.SaveDataAsync(jsonFilePath);
                if (result)
                {
                    System.Diagnostics.Debug.WriteLine("Data saved successfully.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to save data.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
            }
        }
        /// <summary>
        /// EN - Get the default path to the JSON file
        /// SK - Získanie predvolenej cesty k JSON súboru 
        /// </summary>
        /// <returns></returns>
        private string GetDefaultJsonFilePath()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return System.IO.Path.Combine(documentsFolder, "TestBenchTarget.json");
        }

        private async void ViewModel_DataSavedSuccessfully(object sender, EventArgs e)
        {
            // Display TeachingTip  
            SaveSuccessNotification.IsOpen = true;

            // Auto-close after 5 seconds  
            await Task.Run(() =>
            {
                Task.Delay(5000).Wait();
            });

            SaveSuccessNotification.IsOpen = false;
        }

        private void PointsInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;

            // Cursor position preservation // Zachovanie pozície kurzora
            int cursorPosition = textBox.SelectionStart;

            // Remove all characters except digits  // Odstránenie všetkých znakov okrem čísel
            StringBuilder cleanedText = new StringBuilder();
            foreach (char c in textBox.Text)
            {
                if (char.IsDigit(c))
                {
                    cleanedText.Append(c);
                }
            }

            // If is text empty, set 0 // Ak je text prázdny, nastavíme 0
            if (string.IsNullOrEmpty(cleanedText.ToString()))
            {
                textBox.Text = "0";
                cursorPosition = 1; // cursor after the number
            }
            // If text has changed, update it // Ak sa text zmenil, aktualizujeme ho
            else if (cleanedText.ToString() != textBox.Text)
            {
                textBox.Text = cleanedText.ToString();

                // Preserve cursor position // Obnovenie - Zachovanie pozície kurzora
                if (cursorPosition <= textBox.Text.Length)
                {
                    textBox.SelectionStart = cursorPosition;
                }
                else
                {
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

        private void PointsInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void MainListView_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Delete)
            {
                if (_viewModel.DeleteCommand.CanExecute(null))
                {
                    _viewModel.DeleteCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        private void MainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = MainListView.SelectedItem as DataItem;
            System.Diagnostics.Debug.WriteLine($"XAML Selection changed: {item != null}");

            // Manual synchronization if binding does not work // Manuálna synchronizácia, ak by binding nefungoval
            if (item != null && _viewModel.SelectedItem != item)
            {
                _viewModel.SelectedItem = item;
                System.Diagnostics.Debug.WriteLine($"ViewModel SelectedItem manually updated");
            }
        }

        private void TextBlock_Holding(object sender, HoldingRoutedEventArgs e)
        {

        }
    }

    // Converter for date formatting // Konverter pre formátovanie dátumu
    public sealed partial class DateFormatConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is DateTime dateTime)
                {
                    return dateTime.ToString("dd.MM.yyyy");
                }
                //return value?.ToString() ?? string.Empty;
                return value != null ? value.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in date conversion: {ex.Message}");
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is string dateString && DateTime.TryParse(dateString, out DateTime result))
                {
                    return result;
                }
                return DateTime.Now;
            }
            catch
            {
                return DateTime.Now;
            }
        }
    }
}
