using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace _35ConfessionBuddhas
{
    public partial class SessionPage : PhoneApplicationPage
    {
        private int _latestTrack = 0;

        private Image _prevDeity;
        private Image _currDeity;
        private Image _nextDeity;

        public SessionPage()
        {
            InitializeComponent();

            // Add the handler for changes from background audio player.
            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetImageControlPositions();

            // If it is a new session, reset the images to the beginning.
            if ((NavigationContext.QueryString.ContainsKey("SessionType")) && (NavigationContext.QueryString["SessionType"] == "New"))
            {
                // Set up the initial image and next.
                SetImageForTrack(0, _currDeity);
                SetImageForTrack(1, _nextDeity);
            }
            // Else we are resuming a session, so need to update the images according to current track.
            else
            {
                int currTrack = Convert.ToInt32(BackgroundAudioPlayer.Instance.Track.Tag);

                SetImageForTrack(currTrack, _currDeity);

                // Only set the left off-screen placeholder if we are not at the first track.
                if (currTrack != 0)
                    SetImageForTrack(currTrack - 1, _prevDeity);

                // Only set the right off-screen placeholder if we are not at the last track.
                if (currTrack != 42)
                    SetImageForTrack(currTrack + 1, _nextDeity);
            }

            // Start the audio if needed
            if (BackgroundAudioPlayer.Instance.PlayerState != PlayState.Playing)
                BackgroundAudioPlayer.Instance.Play();

            base.OnNavigatedTo(e);
        }
        
        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("APP RECEIVED PLAYSTATE CHANGE: " + BackgroundAudioPlayer.Instance.PlayerState.ToString());
            if (null != BackgroundAudioPlayer.Instance.Track)
            {
                switch (BackgroundAudioPlayer.Instance.PlayerState)
                {
                    case PlayState.Playing:                        
                        short currTrack = Convert.ToInt16(BackgroundAudioPlayer.Instance.Track.Tag);

                        if (currTrack > _latestTrack) // Current track is newer so play the forward animation
                        {
                            // Re-set prev/curr/next pointers to figure out which image controls to move
                            SetImageControlPositions();

                            // Stop storyboard if it is running
                            sbNextDeity.Stop();

                            // Attach storyboard to current and next images and then start animation
                            Storyboard.SetTarget(animCurrLeft, _currDeity);
                            Storyboard.SetTarget(animNextLeft, _nextDeity);
                            sbNextDeity.Begin();

                            // Move the old "previous" image back go the right and set it up with next image.
                            _prevDeity.SetValue(Canvas.LeftProperty, 480d);
                            SetImageForTrack(currTrack + 1, _prevDeity);
                            
                            // Update record of the latest track played
                            _latestTrack = currTrack;
                        }
                        else if (currTrack < _latestTrack) // Current track is older so play backwards animation
                        {
                            SetImageControlPositions();
                            sbPrevDeity.Stop();

                            Storyboard.SetTarget(animCurrRight, _currDeity);
                            Storyboard.SetTarget(animPrevRight, _prevDeity);
                            sbPrevDeity.Begin();

                            _nextDeity.SetValue(Canvas.LeftProperty, -480d);
                            SetImageForTrack(currTrack - 1, _nextDeity);

                            _latestTrack = currTrack;
                        } // If not, track didn't change so do nothing.
                        break;
                }
            }
        }
        /// <summary>
        /// Looks at the current positions of all the image controls and determines which is in the previous,
        /// current and next slots based on their canvas position.
        /// </summary>
        private void SetImageControlPositions()
        {
            double[] arrLeftPos = new double[3] { 
                Convert.ToDouble(imgDeity1.GetValue(Canvas.LeftProperty)),
                Convert.ToDouble(imgDeity2.GetValue(Canvas.LeftProperty)),
                Convert.ToDouble(imgDeity3.GetValue(Canvas.LeftProperty))
            };

            int minIndex = 0, maxIndex = 0;

            for (int i = 0; i < 3; i++)
            {
                if (arrLeftPos[i] < arrLeftPos[minIndex]) minIndex = i;
                if (arrLeftPos[i] > arrLeftPos[maxIndex]) maxIndex = i;
            }

            switch (minIndex)
            {
                case 0:
                    _prevDeity = imgDeity1;
                    break;
                case 1:
                    _prevDeity = imgDeity2;
                    break;
                case 2:
                    _prevDeity = imgDeity3;
                    break;
            }

            switch (maxIndex)
            {
                case 0:
                    _nextDeity = imgDeity1;
                    break;
                case 1:
                    _nextDeity = imgDeity2;
                    break;
                case 2:
                    _nextDeity = imgDeity3;
                    break;
            }

            switch (minIndex + maxIndex)
            {
                case 1:
                    _currDeity = imgDeity3;
                    break;
                case 2:
                    _currDeity = imgDeity2;
                    break;
                case 3:
                    _currDeity = imgDeity1;
                    break;
            }
        }

        private void SetImageForTrack(int track, Image imgPlaceholder)
        {
            try
            {
                string imageName = String.Empty;

                // Figure out image based on track number
                if (track >= 0 && track <= 3)
                    imageName = "Buddha Shakyamuni";
                else if (track >= 4 && track <= 37)
                    imageName = (track - 2).ToString() + "-35CB";
                else if (track >= 38 && track <= 40)
                    imageName = "Buddha Shakyamuni";
                else if (track >= 41 && track <= 42)
                    imageName = "Buddha Shakyamuni"; // TODO: get Je Tsongkhapa image

                // Set image
                imgPlaceholder.Source = new BitmapImage(new Uri("/Images/" + imageName + ".png", UriKind.Relative));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SetImageForTrack: " + e.Message);
            }
        }       
    }
}