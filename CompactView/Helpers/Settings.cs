using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CompactView.Helpers
{
    class AppSettings
    {
        // Create a new localsettings container
        static ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;

        #region Key names
        //The storage key names of the settings
        const string UriKeyName = "Uri";

        #endregion
        #region Default value
        // The default value of the settings
        string UriDefault = "https://www.netflix.com";
        #endregion
        #region StoreSettings
        public string Uri
        {
            get
            {
                return GetValueOrDefault<string>(UriKeyName, UriDefault);
            }
            set
            {
                AddOrUpdateValue(UriKeyName, value);
            }
        }

        const string ADFKeyName = "ADF";
        const int ADFDefault = 0;

        public int ADF
        {
            get
            {
                return GetValueOrDefault<int>(ADFKeyName, ADFDefault);
            }
            set
            {
                AddOrUpdateValue(ADFKeyName, value);
            }
        }
        #endregion
        //Don't Touch!
        #region Basics
        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            // Get the settings for this application.
            settings = ApplicationData.Current.LocalSettings;
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
            if (settings.Values.ContainsKey(Key))
            {
                // If the value has changed
                if (settings.Values[Key] != value)
                {
                    // Store the new value
                    settings.Values[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Values.Add(Key, value);
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
            if (settings.Values.ContainsKey(Key))
            {
                value = (T)settings.Values[Key];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }
        #endregion
    }
}
