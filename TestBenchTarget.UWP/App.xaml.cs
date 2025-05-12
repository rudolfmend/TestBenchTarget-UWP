using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TestBenchTarget.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
    /// </summary>
    public sealed partial class App : Application
    {
        // Zámok pre jednorázové spustenie aplikácie
        private static Windows.ApplicationModel.AppInstance? _instance;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Pridanie obslužnej rutiny pre neodchytené výnimky
            this.UnhandledException += App_UnhandledException;

            InitializeComponent();
            Suspending += OnSuspending;

            // Nastavenie anglickej kultúry globálne
            var culture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        /// <summary>
        /// Spracováva neodchytené výnimky v aplikácii
        /// </summary>
        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Logovanie chyby
            System.Diagnostics.Debug.WriteLine($"Unhandled Exception: {e.Message}\nStackTrace: {e.Exception?.StackTrace}");

            // Označiť ako spracovanú, aby aplikácia nezlyhala
            e.Handled = true;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                // Kontrola, či je aplikácia už spustená
                if (!EnsureSingleInstance(e))
                {
                    // Aplikácia je už spustená, vrátime sa bez spustenia ďalšej inštancie
                    return;
                }

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active.
                if (Window.Current.Content is not Frame rootFrame)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        // TODO: Load state from previously suspended application
                    }
                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
                }

                if (e.PrelaunchActivated == false)
                {
                    if (rootFrame.Content == null)
                    {
                        // Navigácia na úvodnú stránku (Form1) namiesto MainPage
                        rootFrame.Navigate(typeof(StartPage), e.Arguments);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
            catch (Exception ex)
            {
                // Logovanie akejkoľvek výnimky pri spustení
                System.Diagnostics.Debug.WriteLine($"Exception during app launch: {ex.Message}\nStackTrace: {ex.StackTrace}");
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        private static bool EnsureSingleInstance(LaunchActivatedEventArgs args)
        {
            try
            {
                // Vytvorenie identifikátora pre túto inštanciu aplikácie
                string uniqueId = "TestBenchTargetSingleInstance";

                // Pokus o získanie primárnej inštancie
                var instance = AppInstance.FindOrRegisterInstanceForKey(uniqueId);

                // Skontrolujeme, či je táto inštancia primárna
                if (instance != null && instance.IsCurrentInstance)
                {
                    // Toto je prvá inštancia aplikácie
                    return true;
                }
                else if (instance != null)
                {
                    // Aplikácia je už spustená, aktivujeme ju
                    instance.RedirectActivationTo();
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Logovanie akejkoľvek výnimky pri kontrole jednorázového spustenia
                System.Diagnostics.Debug.WriteLine($"Exception during single instance check: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }

            // Default to true (allow this instance to run) if we can't determine the status 
            // or in case of an error, to avoid blocking the application from starting
            return true;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load page '{e.SourcePageType.FullName}'.");

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            throw new Exception($"Failed to load page '{e.SourcePageType.FullName}'.");
        }

        /// <summary>
        /// Invoked when application execution is being suspended. Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
