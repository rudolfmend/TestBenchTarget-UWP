using System;
using System.Collections.Generic;
using TestBenchTarget.UWP.Models;
using TestBenchTarget.UWP.Services;
using TestBenchTarget.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TestBenchTarget.UWP
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel _viewModel;

        public MainPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("MainPage constructor beginning...");

                // Inicializácia XAML pred inicializáciou ViewModelu
                this.InitializeComponent();

                // Teraz inicializujeme ViewModel
                _viewModel = new MainViewModel(new DataService());

                // Nastavenie časovača pre aktualizáciu času
                var timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += (s, e) => {
                    TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
                };
                timer.Start();
                TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");

                // Manuálne nastavenie dátového zdroja pre ListView
                SetupListView();

                // Manuálne pripojenie ovládacích prvkov k ViewModelu
                ConnectControls();

                System.Diagnostics.Debug.WriteLine("MainPage initialization completed successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MainPage constructor: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        private void SetupListView()
        {
            // Nastavenie ItemTemplate manuálne
            MainListView.ItemTemplate = (DataTemplate)Resources["ItemTemplate"];

            // Nastavenie ItemsSource
            try
            {
                MainListView.ItemsSource = _viewModel.DataItems;

                // Pridanie handlera pre SelectionChanged
                MainListView.SelectionChanged += (s, e) => {
                    if (MainListView.SelectedItem is DataItem item)
                    {
                        _viewModel.SelectedItem = item;
                    }
                    else
                    {
                        _viewModel.SelectedItem = null;
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting ListView ItemsSource: {ex.Message}");
            }
        }

        private void ConnectControls()
        {
            try
            {
                // Prepojenie DatePicker s ViewModel
                DateSelector.Date = _viewModel.SelectedDate;
                DateSelector.DateChanged += (s, e) => {
                    _viewModel.SelectedDate = DateSelector.Date;
                };

                // Prepojenie TextBoxov s ViewModel
                ProcedureInput.Text = _viewModel.ProcedureText;
                ProcedureInput.TextChanged += (s, e) => {
                    _viewModel.ProcedureText = ProcedureInput.Text;
                };

                PointsInput.Text = _viewModel.PointsText;
                PointsInput.TextChanged += (s, e) => {
                    _viewModel.PointsText = PointsInput.Text;
                };

                DelegateInput.Text = _viewModel.DelegateText;
                DelegateInput.TextChanged += (s, e) => {
                    _viewModel.DelegateText = DelegateInput.Text;
                };

                // Prepojenie tlačidiel s príkazmi ViewModelu
                AddButton.Click += (s, e) => _viewModel.AddCommand.Execute(null);
                LoadButton.Click += (s, e) => _viewModel.LoadCommand.Execute(null);
                SaveButton.Click += (s, e) => _viewModel.SaveCommand.Execute(null);
                DeleteButton.Click += (s, e) => _viewModel.DeleteCommand.Execute(null);
                OpenFolderButton.Click += (s, e) => _viewModel.OpenFolderCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error connecting controls: {ex.Message}");
            }
        }
    }

    // Konverter pre formátovanie dátumu
    public partial class DateFormatConverter : IValueConverter
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
