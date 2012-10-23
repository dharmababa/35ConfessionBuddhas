using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;

namespace _35ConfessionBuddhas
{
    public class AppSettings
    {
        IsolatedStorageSettings settings;

        // Key names for settings
        private const string LAST_TRACK_NUMBER_KEYNAME = "LastTrackNumber";
        private const string LAST_TRACK_TIME_KEYNAME = "LastTrackTime";
        private const string WARN_ON_NEW_KEYNAME = "WarnOnNewSession";
        private const string BG_PLAYBACK_ENABLED_KEYNAME = "BackgroundPlaybackEnabled";

        // Default values for settings
        private const short LAST_TRACK_NUMBER_DEFAULT = 0;
        private const double LAST_TRACK_TIME_DEFAULT = 0;
        private const bool WARN_ON_NEW_DEFAULT = true;
        private const bool BG_PLAYBACK_ENABLED_DEFAULT = true;

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                // Get the settings for this application.
                settings = IsolatedStorageSettings.ApplicationSettings;
            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }

        public short LastTrackNumber
        {
            get
            {
                return GetValueOrDefault<short>(LAST_TRACK_NUMBER_KEYNAME, LAST_TRACK_NUMBER_DEFAULT);
            }
            set
            {
                if (AddOrUpdateValue(LAST_TRACK_NUMBER_KEYNAME, value)) Save();
            }
        }

        public double LastTrackTime
        {
            get
            {
                return GetValueOrDefault<double>(LAST_TRACK_TIME_KEYNAME, LAST_TRACK_TIME_DEFAULT);
            }
            set
            {
                if (AddOrUpdateValue(LAST_TRACK_TIME_KEYNAME, value)) Save();
            }
        }

        public bool WarnOnNew
        {
            get
            {
                return GetValueOrDefault<bool>(WARN_ON_NEW_KEYNAME, WARN_ON_NEW_DEFAULT);
            }
            set
            {
                if (AddOrUpdateValue(WARN_ON_NEW_KEYNAME, value)) Save();
            }
        }

        public bool BackgroundPlaybackEnabled
        {
            get
            {
                return GetValueOrDefault<bool>(BG_PLAYBACK_ENABLED_KEYNAME, BG_PLAYBACK_ENABLED_DEFAULT);
            }
            set
            {
                if (AddOrUpdateValue(BG_PLAYBACK_ENABLED_KEYNAME, value)) Save();
            }
    }
}
