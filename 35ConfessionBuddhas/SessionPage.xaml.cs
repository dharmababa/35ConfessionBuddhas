using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace _35ConfessionBuddhas
{
    public partial class SessionPage : PhoneApplicationPage
    {
        public SessionPage()
        {
            InitializeComponent();

            // Add the handler for changes from background audio player.
            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);

            // Start the audio
            BackgroundAudioPlayer.Instance.Play();

            //imgDeity.Source = new BitmapImage(new Uri("/images/Buddha Shakyamuni.png", UriKind.Relative));
        }

        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            if (null != BackgroundAudioPlayer.Instance.Track)
            {
                SetImageForTrack(BackgroundAudioPlayer.Instance.Track.Tag);
            }
        }

        private void SetImageForTrack(string trackTag)
        {
            try
            {
                int track = Convert.ToInt16(trackTag);
                string imageName = String.Empty;

                // Figure out image based on track number
                if (track >= 1 && track <= 4)
                    imageName = "Buddha Shakyamuni";
                else if (track >= 5 && track <= 38)
                    imageName = (track - 3).ToString() + "-35CB";
                else if (track >= 39 && track <= 41)
                    imageName = "Buddha Shakyamuni";
                else if (track >= 42 && track <= 43)
                    imageName = "Buddha Shakyamuni"; // TODO: get Je Tsongkhapa image

                // Set image
                imgDeity.Source = new BitmapImage(new Uri("/Images/" + imageName + ".png", UriKind.Relative));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SetImageForTrack: " + e.Message);
            }
        }       
    }
}