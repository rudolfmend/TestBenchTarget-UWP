using System;
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

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
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
                        // When the navigation stack isn't restored navigate to the first page, configuring
                        // the new page by passing required information as a navigation parameter.
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
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
