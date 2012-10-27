using System;
using System.Collections.Generic;
using System.Windows;
using System.IO.IsolatedStorage;
using Microsoft.Phone.BackgroundAudio;

namespace _35CB_AudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static volatile bool _classInitialized;

        private const string ARTIST = "New Kadampa Tradition";
        private const string ALBUM = "The Bodhisattva's Confessions of Moral Downfalls";

        static int currentTrackNumber = 0;
        private _35CB_SharedHelpers.AppSettings settings = new _35CB_SharedHelpers.AppSettings();
        
        // Initialize playlist - remaining tracks are added in the constructor.
        private static List<AudioTrack> _playlist = new List<AudioTrack>
        {
            new AudioTrack(new Uri("Liberating Prayer.mp3", UriKind.Relative), "Liberating Prayer", ARTIST, ALBUM, null, 
                "0", EnabledPlayerControls.All),
            new AudioTrack(new Uri("Homage.mp3", UriKind.Relative), "Homage", ARTIST, ALBUM, null, "1", EnabledPlayerControls.All),
            new AudioTrack(new Uri("Refuge.mp3", UriKind.Relative), "Refuge", ARTIST, ALBUM, null, "2", EnabledPlayerControls.All)
        };

        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += AudioPlayer_UnhandledException;
                });

                // Add prostration tracks
                for (int i = 0; i < 35; i++) {
                    _playlist.Add(new AudioTrack(new Uri("Prostration-" + (i+1).ToString() + ".mp3", UriKind.Relative), "Prostration", 
                        ARTIST, ALBUM, null, (i+3).ToString(), EnabledPlayerControls.All));
                }

                // Add final tracks
                _playlist.Add(new AudioTrack(new Uri("Confession.mp3", UriKind.Relative), "Confession", ARTIST, ALBUM, 
                    null, "38", EnabledPlayerControls.All));
                _playlist.Add(new AudioTrack(new Uri("Dedication.mp3", UriKind.Relative), "Dedication", ARTIST, ALBUM,
                    null, "39", EnabledPlayerControls.All));
                _playlist.Add(new AudioTrack(new Uri("Conclusion.mp3", UriKind.Relative), "Conclusion", ARTIST, ALBUM,
                    null, "40", EnabledPlayerControls.All));
                _playlist.Add(new AudioTrack(new Uri("Prayers for the Virtuous Tradition.mp3", UriKind.Relative), "Prayers for the Virtuous Tradition", 
                    ARTIST, ALBUM, null, "41", EnabledPlayerControls.All));
                _playlist.Add(new AudioTrack(new Uri("Nine-line Migtsema Prayer.mp3", UriKind.Relative), "Nine-line Migtsema Prayer", ARTIST, ALBUM,
                    null, "42", EnabledPlayerControls.All));
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        /// 
        /// Notable playstate events: 
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            System.Diagnostics.Debug.WriteLine("AGENT PLAY STATE CHANGED: " + playState.ToString());
            switch (playState)
            {
                case PlayState.TrackEnded:
                    player.Track = GetNextTrack();
                    break;
                case PlayState.TrackReady:
                    player.Play();
                    break;
                case PlayState.Shutdown:
                    SaveTrackAndTime(player, track);
                    break;
                case PlayState.Unknown:
                    break;
                case PlayState.Stopped:                    
                    break;
                case PlayState.Paused:
                    break;
                case PlayState.Playing:
                    break;
                case PlayState.BufferingStarted:
                    break;
                case PlayState.BufferingStopped:
                    break;
                case PlayState.Rewinding:
                    break;
                case PlayState.FastForwarding:
                    break;
            }

            NotifyComplete();
        }

        /// <summary>
        /// Saves the current track and position to isolated storage so that playback can be resumed later.
        /// </summary>
        /// <param name="player">The current background audio player instance.</param>
        /// <param name="track">The current track.</param>
        private void SaveTrackAndTime(BackgroundAudioPlayer player, AudioTrack track)
        {
            settings.LastTrackNumber = Convert.ToInt32(track.Tag);
            settings.LastTrackTime = player.Position.TotalSeconds;   
        }        

        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            System.Diagnostics.Debug.WriteLine("AGENT RECEIVED USER ACTION: " + action.ToString());
            switch (action)
            {
                case UserAction.Play:                    
                    PlayTrack(player);                        
                    break;
                case UserAction.Stop:
                    player.Stop();
                    break;
                case UserAction.Pause:
                    player.Pause();
                    break;
                case UserAction.FastForward:
                    player.FastForward();
                    break;
                case UserAction.Rewind:
                    player.Rewind();
                    break;
                case UserAction.Seek:
                    player.Position = (TimeSpan)param;
                    break;
                case UserAction.SkipNext:
                    player.Track = GetNextTrack();
                    break;
                case UserAction.SkipPrevious:
                    AudioTrack previousTrack = GetPreviousTrack();
                    if (previousTrack != null)
                    {
                        player.Track = previousTrack;
                    }
                    break;
            }

            NotifyComplete();
        }

        private void PlayTrack(BackgroundAudioPlayer player) {
            // First, check if we are supposed to start a new session.
            if ((player.Track != null) && (player.Track.Tag.EndsWith("_new")))
            {
                // Remove the "new" tag before we reset to the beginning.
                player.Track.BeginEdit();
                player.Track.Tag.Remove(player.Track.Tag.Length - 4);
                player.Track.EndEdit();
                
                // Reset to the beginning
                player.Track = _playlist[0];
                currentTrackNumber = 0;
            }
            else
                // Sets the track to play. When the TrackReady state is received, 
                // playback begins from the OnPlayStateChanged handler.
                player.Track = _playlist[currentTrackNumber];
        }

        /// <summary>
        /// Implements the logic to get the next AudioTrack instance.
        /// In a playlist, the source can be from a file, a web request, etc.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if the playback is completed</returns>
        private AudioTrack GetNextTrack()
        {
            return (++currentTrackNumber < _playlist.Count) ? _playlist[currentTrackNumber] : null;
        }


        /// <summary>
        /// Implements the logic to get the previous AudioTrack instance.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if previous track is not allowed</returns>
        private AudioTrack GetPreviousTrack()
        {
            return (--currentTrackNumber >= 0) ? _playlist[currentTrackNumber] : null;
        }

        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent 
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            if (isFatal)
            {
                Abort();
            }
            else
            {
                NotifyComplete();
            }

        }

        /// <summary>
        /// Called when the agent request is getting cancelled
        /// </summary>
        /// <remarks>
        /// Once the request is Cancelled, the agent gets 5 seconds to finish its work,
        /// by calling NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {

        }
    }
}
