using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestBenchTarget.UWP
{
    public sealed partial class StartPage : Page
    {
        private DispatcherTimer _timer;

        public StartPage()
        {
            this.InitializeComponent();

            // Nastavenie času a dátumu
            TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            DateDisplay.Text = DateTime.Now.ToString("dd.MM.yyyy");

            // Nastavenie časovača
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                TimeDisplay.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            _timer.Start();

            // Registrovať udalosť pre vyčistenie zdrojov
            this.Unloaded += StartPage_Unloaded;
        }

        private void StartPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Zastaviť a vyčistiť časovač
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null!;
            }
        }

        private void OpenAppButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigácia na druhú stránku
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Vytvorenie dialógového okna About
            ContentDialog aboutDialog = new ContentDialog
            {
                //Title = "About TestBench Target",
                PrimaryButtonText = "Close"
            };

            // Obsah dialógového okna
            StackPanel contentPanel = new StackPanel { Margin = new Thickness(10) };

            // Verzia
            TextBlock versionBlock = new TextBlock
            {
                Text = $"Version: {Windows.ApplicationModel.Package.Current.Id.Version.Major}." +
                       $"{Windows.ApplicationModel.Package.Current.Id.Version.Minor}." +
                       $"{Windows.ApplicationModel.Package.Current.Id.Version.Build}." +
                       $"{Windows.ApplicationModel.Package.Current.Id.Version.Revision}",
                Margin = new Thickness(0, 10, 0, 20)
            };

            // Popis
            TextBlock descriptionBlock = new TextBlock
            {
                Text = "A sample application designed to serve as a testing subject for developers creating monitoring, " +
                       "accessibility, or UI automation tools. This app provides predictable user interface elements and " +
                       "behaviors that developers can use to test their monitoring solutions. For Windows 10 and newer.\n\n" +
                       "Main features:\n" +
                       "  - Small and fast application\n" +
                       "  - Tests opening a Windows directory\n" +
                       "  - Simulates adding defined items to a table\n" +
                       "  - Simple chronological display of data in a table format\n" +
                       "  - Provides a target app for trying out monitoring and testing tools\n\n" +
                       "Ideal for developers and testers who need a reliable target application when developing tools " +
                       "to monitor and test UI interactions.",
                TextWrapping = TextWrapping.Wrap
            };

            // Copyright
            TextBlock copyrightBlock = new TextBlock
            {
                Text = "Copyright © 2025 Rudolf Mendzezof",
                Margin = new Thickness(0, 20, 0, 0)
            };

            // Pridanie všetkých prvkov do panelu
            contentPanel.Children.Add(versionBlock);
            contentPanel.Children.Add(descriptionBlock);
            contentPanel.Children.Add(copyrightBlock);

            // Nastavenie obsahu dialógu
            aboutDialog.Content = contentPanel;

            // Zobrazenie dialógu
            await aboutDialog.ShowAsync();
        }
    }
}
