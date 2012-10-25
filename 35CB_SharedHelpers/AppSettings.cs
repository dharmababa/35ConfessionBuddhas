using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;
using System.Threading;

namespace _35CB_SharedHelpers
{
    public class AppSettings
    {
        IsolatedStorageSettings settings;

        // Shared mutex name for accessing settings
        public const string STORAGE_MUTEX = "35CB_Storage_Mutex";

        // Key names for settings
        private const string LAST_TRACK_NUMBER_KEYNAME = "LastTrackNumber";
        private const string LAST_TRACK_TIME_KEYNAME = "LastTrackTime";
        private const string WARN_ON_NEW_KEYNAME = "WarnOnNewSession";
        private const string BG_PLAYBACK_ENABLED_KEYNAME = "BackgroundPlaybackEnabled";
        private const string IS_NEW_SESSION_KEYNAME = "IsNewSession";

        // Default values for settings
        private const int LAST_TRACK_NUMBER_DEFAULT = 0;
        private const double LAST_TRACK_TIME_DEFAULT = 0;
        private const bool WARN_ON_NEW_DEFAULT = true;
        private const bool BG_PLAYBACK_ENABLED_DEFAULT = true;
        private const bool IS_NEW_SESSION_DEFAULT = false;

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

            Mutex mutex = new Mutex(false, _35CB_SharedHelpers.AppSettings.STORAGE_MUTEX);
            try
            {
                mutex.WaitOne();

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
            catch (Exception)
            {
                // error handling, if applicable
                return false;
            }
            finally
            {
                try
                {
                    mutex.ReleaseMutex();
                }
                catch (Exception)
                {
                }
                mutex.Dispose();
            }            
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

            Mutex mutex = new Mutex(false, _35CB_SharedHelpers.AppSettings.STORAGE_MUTEX);
            try
            {
                mutex.WaitOne();

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
            catch (Exception)
            {
                // error handling, if applicable
                return default(T);
            }
            finally
            {
                try
                {
                    mutex.ReleaseMutex();
                }
                catch (Exception)
                {
                }
                mutex.Dispose();
            }            
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }

        public int LastTrackNumber
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

        public bool IsNewSession
        {
            get
            {
                return GetValueOrDefault<bool>(IS_NEW_SESSION_KEYNAME, IS_NEW_SESSION_DEFAULT);
            }
            set
            {
                if (AddOrUpdateValue(IS_NEW_SESSION_KEYNAME, value)) Save();
            }
        }
    }
}
