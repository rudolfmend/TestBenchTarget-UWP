using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestBenchTarget.UWP.Models;
using TestBenchTarget.UWP.Services;
using TestBenchTarget.UWP.ViewModels;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace TestBenchTarget.UWP
{
    public sealed partial class MainPage : Page
    {
        private DateTime _currentDate = DateTime.Now.Date;
        private DispatcherTimer? _timer;
        private MainViewModel _viewModel;
        public MainViewModel ViewModel => _viewModel;
        private CustomObservableCollection<DataItem> _dataItems = new CustomObservableCollection<DataItem>();
        private int _dateOffset = 0; // Offset from today's date for the DateNumberBox selection

        public MainPage()
        {
            this.InitializeComponent();

            _viewModel = new MainViewModel(new DataService());
            this.DataContext = _viewModel;

            DateFormatSelector.SelectionChanged += DateFormatSelector_SelectionChanged; // Set event handler for DateFormatSelector after initialization _viewModel

            // Inicializácia _currentDate aktuálnym dátumom
            _currentDate = DateTime.Now.Date;

            // Ak je SelectedDateString nastavený, skúste ho parsovať
            if (!string.IsNullOrEmpty(_viewModel.SelectedDateString))
            {
                var converter = new DateFormatConverter();

                //var result = converter.ConvertBack(_viewModel.SelectedDateString, typeof(DateTime), null!, null!);
                //var result = converter.ConvertBack(_viewModel.SelectedDateString, typeof(DateTime), default, default);
                var result = converter.ConvertBack(_viewModel.SelectedDateString, typeof(DateTime), null, null);

                if (result is DateTime dateTime)
                {
                    _currentDate = dateTime;
                }
            }

            this.Unloaded += MainPage_Unloaded; // - Unloading event registration (for cleaning resources)
             
            _viewModel.DataSavedSuccessfully += ViewModel_DataSavedSuccessfully!;
            _viewModel.ListClearedSuccessfully += ViewModel_ListClearedSuccessfully!;

            // KeyboardAccelerator for button "Enter" (keyboard shortcut)
            KeyboardAccelerator enterAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Enter
            };
            enterAccelerator.Invoked += EnterAccelerator_Invoked;
            this.KeyboardAccelerators.Add(enterAccelerator);
                        

            // KeyboardAccelerator for left arrow (decrement date - same as down arrow) // KeyboardAccelerator pre šípku doľava (dekrementácia dátumu - rovnako ako šípka nadol)
            KeyboardAccelerator leftKeyAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Left
            };
            leftKeyAccelerator.Invoked += (s, args) => {
                ChangeDateByDays(-1);
                args.Handled = true;
            };
            this.KeyboardAccelerators.Add(leftKeyAccelerator);

            // KeyboardAccelerator for right arrow (increment date - same as up arrow) // KeyboardAccelerator pre šípku doprava (inkrementácia dátumu - rovnako ako šípka nahor)
            KeyboardAccelerator rightKeyAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Right
            };
            rightKeyAccelerator.Invoked += (s, args) => {
                ChangeDateByDays(1);
                args.Handled = true;
            };
            this.KeyboardAccelerators.Add(rightKeyAccelerator);

            // KeyboardAccelerator pre šípku nahor (inkrementácia dátumu)
            KeyboardAccelerator upKeyAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Up
            };
            upKeyAccelerator.Invoked += (s, args) => {
                ChangeDateByDays(1);
                args.Handled = true;
            };
            this.KeyboardAccelerators.Add(upKeyAccelerator);

            // KeyboardAccelerator pre šípku nadol (dekrementácia dátumu)
            KeyboardAccelerator downKeyAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Down
            };
            downKeyAccelerator.Invoked += (s, args) => {
                ChangeDateByDays(-1);
                args.Handled = true;
            };
            this.KeyboardAccelerators.Add(downKeyAccelerator);

            // KeyboardAccelerator pre PageUp (posun o 30 dní dopredu)
            // KeyboardAccelerator for PageUp (increment date by 30 days)
            KeyboardAccelerator pageUpAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.PageUp
            };
            pageUpAccelerator.Invoked += (s, args) => {
                ChangeDateByDays(30);
                args.Handled = true;
            };
            this.KeyboardAccelerators.Add(pageUpAccelerator);

            // KeyboardAccelerator pre PageDown (posun o 30 dní dozadu)
            // KeyboardAccelerator for PageDown (shift 30 days back)
            KeyboardAccelerator pageDownAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.PageDown
            };
            pageDownAccelerator.Invoked += (s, args) => {
                ChangeDateByDays(-30);
                args.Handled = true;
            };
            this.KeyboardAccelerators.Add(pageDownAccelerator);
        }

        private void DateFormatSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ComboBoxItem selectedItem && selectedItem.Tag is string formatString)
            {
                // Aktualizácia formátu dátumu vo ViewModeli
                _viewModel.DateFormat = formatString;

                // Aktualizácia zobrazenia dátumu
                if (DateTime.TryParse(_viewModel.SelectedDateString, out DateTime date))
                {
                    _currentDate = date;
                    _viewModel.SelectedDateString = _currentDate.ToString(formatString);
                }
            }
        }

        private void IncrementDate_Click(object sender, RoutedEventArgs e)
        {
            ChangeDateByDays(1);
        }

        private void DecrementDate_Click(object sender, RoutedEventArgs e)
        {
            ChangeDateByDays(-1);
        }

        private void DateDisplay_PointerWheelChanged(object sender, PointerRoutedEventArgs e)        // handler for PointerWheelChanged in the TextBlock
        {
            int delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;
            ChangeDateByDays(delta > 0 ? 1 : -1);
            e.Handled = true;
        }

        private void ChangeDateByDays(int days)
        {
            _currentDate = _currentDate.AddDays(days);
            _viewModel.SelectedDateString = _currentDate.ToString(_viewModel.DateFormat);
        }

        //private void DateNumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        //{
        //    if (args.NewValue == null) return;

        //    // Výpočet rozdielu dní
        //    int newValue = (int)args.NewValue;
        //    int oldValue = args.OldValue != null ? (int)args.OldValue : 0;
        //    int change = newValue - oldValue;

        //    if (change != 0)
        //    {
        //        // Zmena dátumu o daný počet dní
        //        ChangeDateByDays(change);

        //        // Nastavíme hodnotu späť na 0, aby sme zabezpečili konzistentné správanie
        //        DateNumberBox.ValueChanged -= DateNumberBox_ValueChanged;  // Dočasne odpojíme event handler
        //        DateNumberBox.Value = 0;
        //        DateNumberBox.ValueChanged += DateNumberBox_ValueChanged;  // Znova pripojíme event handler
        //    }
        //}

        //private void DateNumberBox_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        //{
        //    int delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;
        //    DateNumberBox.Value += (delta > 0) ? 1 : -1;
        //    e.Handled = true;
        //}


        // When the user presses Enter to add an item to the table, the focus is set to the first field of the form.
        private void EnterAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            // Ensure _viewModel is of the correct type  
            if (_viewModel is MainViewModel mainViewModel)
            {
                // Execute the command when Enter is pressed  
                if (mainViewModel.AddCommand.CanExecute(null))
                {
                    mainViewModel.AddCommand.Execute(null);
                    ProcedureInput.Focus(FocusState.Programmatic); // Set focus to the first field of the form ("ProcedureInput" TextBox)
                }
            }
            args.Handled = true; // Prevent further processing of the Enter key  
        }

        private async void ViewModel_ListClearedSuccessfully(object sender, EventArgs e)
        {
            // Display TeachingTip  
            ClearListSuccessNotification.IsOpen = true;
            // Auto-close after 5 seconds  
            await Task.Delay(5000);

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

        /// <summary>
        /// EN - Get the default path to the JSON file
        /// SK - Získanie predvolenej cesty k JSON súboru 
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDefaultJsonFilePath()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync("TestBenchTarget.json", CreationCollisionOption.OpenIfExists);
            return file.Path;
        }

        public async Task ExportDataAsync()
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("JSON File", new List<string>() { ".json" });
            savePicker.SuggestedFileName = "TestBenchTarget";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await _viewModel.SaveDataAsync(file.Path);
            }
        }

        private async void ViewModel_DataSavedSuccessfully(object sender, EventArgs e)
        {
            // Display TeachingTip  
            SaveSuccessNotification.IsOpen = true;

            // Auto-close after 5 seconds  
            await Task.Delay(5000);

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

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true; // Zabrániť TextBoxu spracovať Enter

                // Ak je tlačidlo AddCommand povolené, vykonať ho
                if (_viewModel.AddCommand.CanExecute(null))
                {
                    _viewModel.AddCommand.Execute(null);
                }
            }
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
                    // Použiť formát z aplikácie, ak je dostupný
                    var page = Window.Current.Content as Frame;
                    var mainPage = page?.Content as MainPage;
                    if (mainPage?.ViewModel != null)
                    {
                        return dateTime.ToString(mainPage.ViewModel.DateFormat);
                    }

                    // Záložný formát
                    return dateTime.ToString("dd.MM.yyyy");
                }
                return value != null ? value.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in date conversion: {ex.Message}");
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object? parameter, string? language)
        {
            try
            {
                if (value is string dateString)
                {
                    // Skúsiť parseovať podľa rôznych formátov
                    foreach (var format in new[] { "dd.MM.yyyy", "MM/dd/yyyy", "yyyy-MM-dd" })
                    {
                        if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                        {
                            return parsedDate;
                        }
                    }

                    // Skúsiť obyčajný parsing ako poslednú možnosť
                    if (DateTime.TryParse(dateString, out DateTime result))
                    {
                        return result;
                    }
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
