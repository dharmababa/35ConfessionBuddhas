using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace _35ConfessionBuddhas
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Close the background audio player in case it 
                // was running from a previous debugging session.
                BackgroundAudioPlayer.Instance.Close();

                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            // Copy media to isolated storage.
            CopyToIsolatedStorage();

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            BackgroundAudioPlayer.Instance.Close();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        private void CopyToIsolatedStorage()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string[] files = new string[43] { "Liberating Prayer.mp3", "Homage.mp3", "Refuge.mp3", "Prostration-1.mp3",
                    "Prostration-2.mp3", "Prostration-3.mp3", "Prostration-4.mp3", "Prostration-5.mp3", "Prostration-6.mp3", 
                    "Prostration-7.mp3", "Prostration-8.mp3", "Prostration-9.mp3", "Prostration-10.mp3", "Prostration-11.mp3", 
                    "Prostration-12.mp3", "Prostration-13.mp3", "Prostration-14.mp3", "Prostration-15.mp3", "Prostration-16.mp3", 
                    "Prostration-17.mp3", "Prostration-18.mp3", "Prostration-19.mp3", "Prostration-20.mp3", "Prostration-21.mp3", 
                    "Prostration-22.mp3", "Prostration-23.mp3", "Prostration-24.mp3", "Prostration-25.mp3", "Prostration-26.mp3", 
                    "Prostration-27.mp3", "Prostration-28.mp3", "Prostration-29.mp3", "Prostration-30.mp3", "Prostration-31.mp3", 
                    "Prostration-32.mp3", "Prostration-33.mp3", "Prostration-34.mp3", "Prostration-35.mp3", "Confession.mp3",
                    "Dedication.mp3", "Conclusion.mp3", "Prayers for the Virtuous Tradition.mp3", "Nine-line Migtsema Prayer.mp3" };

                foreach (var _fileName in files)
                {
                    if (!storage.FileExists(_fileName))
                    {
                        string _filePath = "Audio/" + _fileName;
                        StreamResourceInfo resource = Application.GetResourceStream(new Uri(_filePath, UriKind.Relative));

                        using (IsolatedStorageFileStream file = storage.CreateFile(_fileName))
                        {
                            int chunkSize = 4096;
                            byte[] bytes = new byte[chunkSize];
                            int byteCount;

                            while ((byteCount = resource.Stream.Read(bytes, 0, chunkSize)) > 0)
                            {
                                file.Write(bytes, 0, byteCount);
                            }
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("Finished copying files to isolated storage");
        } // End of CopyToIsolatedStorage()

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}