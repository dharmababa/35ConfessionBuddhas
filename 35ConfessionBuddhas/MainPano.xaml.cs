using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;

namespace _35ConfessionBuddhas
{
    public partial class MainPano : PhoneApplicationPage
    {
        private _35CB_SharedHelpers.AppSettings settings = new _35CB_SharedHelpers.AppSettings();

        public MainPano()
        {
            InitializeComponent();
        }

        private void lbiNewSession_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ((settings.WarnOnNew) && (App.IsResumeAvailable))
            {
                // If resume is available, prompt the user to make sure.
                MessageBoxResult result = MessageBox.Show(AppResources.WarnOnNewText, AppResources.WarnOnNewTitle, MessageBoxButton.OKCancel);

                // If user says ok, stop the BAP and set flag indicating that new session has been started.
                if (result == MessageBoxResult.OK)
                {
                    BackgroundAudioPlayer.Instance.Stop();
                    settings.IsNewSession = true;
                }
                else // Don't want to navigate so return if the user cancelled the prompt.
                    return;
            }

            this.NavigationService.Navigate(new Uri("/SessionPage.xaml?SessionType=New", UriKind.Relative));
        }

        private void lbiResumeSession_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.IsResumeAvailable)
                this.NavigationService.Navigate(new Uri("/SessionPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Enable resume if there is either a saved session from before or a current instance of the player.
            App.IsResumeAvailable = (settings.LastTrackNumber != 0) ||
                (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing) ||
                (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Paused);

            // Enable the resume UI if needed.
            if (App.IsResumeAvailable)
            {
                tbResumeSession.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                imgResumeSession.OpacityMask = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                lbiResumeSession.SetValue(TiltEffect.IsTiltEnabledProperty, true);
            }

            base.OnNavigatedTo(e);
        }
    }
}