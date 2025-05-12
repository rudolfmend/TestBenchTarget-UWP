using System;
using System.Collections.Generic;
using TestBenchTarget.UWP.Models;
using TestBenchTarget.UWP.Services;
using TestBenchTarget.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace TestBenchTarget.UWP
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;
        private MainViewModel _viewModel;

        public MainPage()
        {
            this.InitializeComponent();

            // Inicializácia časovača pre aktualizáciu času
            TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => {
                TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            _timer.Start();

            // Inicializácia ViewModelu a nastavenie DataContext
            _viewModel = new MainViewModel(new DataService());
            this.DataContext = _viewModel;

            // Inicializácia ComboBoxu pre výber dátumu
            InitializeDateSelector();

            // Registrácia udalosti pre vyčistenie zdrojov
            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Zastavenie a vyčistenie časovača
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null!;
            }
        }

        private void InitializeDateSelector()
        {
            // Naplnenie ComboBoxu dátumami
            var dateItems = new List<string>();
            for (int i = -367; i <= 367; i++)
            {
                dateItems.Add(DateTime.Now.AddDays(i).ToString("dd.MM.yyyy"));
            }

            DateSelector.ItemsSource = dateItems;
            DateSelector.SelectedIndex = 367; // Dnešný dátum je v strede
        }

        private void PointsInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;

            // Zachovanie pozície kurzora
            int cursorPosition = textBox.SelectionStart;

            // Odstránenie všetkých znakov okrem čísel
            string cleanedText = "";
            foreach (char c in textBox.Text)
            {
                if (char.IsDigit(c))
                {
                    cleanedText += c;
                }
            }

            // Ak je text prázdny, nastavíme 0
            if (string.IsNullOrEmpty(cleanedText))
            {
                textBox.Text = "0";
                cursorPosition = 1; // kurzor za číslom
            }
            // Ak sa text zmenil, aktualizujeme ho
            else if (cleanedText != textBox.Text)
            {
                textBox.Text = cleanedText;
                // Obnovenie pozície kurzora
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

            // Manuálna synchronizácia, ak by binding nefungoval
            if (item != null && _viewModel.SelectedItem != item)
            {
                _viewModel.SelectedItem = item;
                System.Diagnostics.Debug.WriteLine($"ViewModel SelectedItem manually updated");
            }
        }
    }

    // Konverter pre formátovanie dátumu
    public sealed partial class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is DateTime dateTime)
                {
                    return dateTime.ToString("dd.MM.yyyy");
                }
                return value?.ToString() ?? string.Empty;
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
