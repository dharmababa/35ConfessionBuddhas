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
        public MainPano()
        {
            InitializeComponent();

            if ((!App.IsResumeAvailable) && (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing))
                App.IsResumeAvailable = true;

            if (App.IsResumeAvailable)
            {
                tbResumeSession.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                imgResumeSession.OpacityMask = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
            }
        }

        private void lbiNewSession_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // If resume is available, prompt the user to make sure.

            this.NavigationService.Navigate(new Uri("/SessionPage.xaml", UriKind.Relative));
        }

        private void lbiResumeSession_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SessionPage.xaml", UriKind.Relative));
        }
    }
}