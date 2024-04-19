using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SiginBS.Common
{
    public class AppConfig
    {
        #region app.config config keys


        [Config("API Base Uri", "http://localhost:1280",false)]
        public string ApiBaseUri { get; private set; }

        [Config("UrlSchemas", "newsignvb", false)]
        public string UrlSchemas { get; private set; }


        [Config("SignPaddingButton_A5", "0", false)]
        public float SignPaddingButton_A5 { get; private set; }

        [Config("SHA265", "1", false)]
        public int SHA265 { get; private set; }

        [Config("SignPaddingLeft", "230", false)]
        public float SignPaddingLeft { get; private set; }

        /// <summary>
        /// Keep alive period in seconds
        /// </summary>

        //[Config("Assets Icon")]
        //public string IconApp { get; private set; }

        #endregion Web.app config keys

        #region Fields, Properties

        private static readonly AppConfig instance = new AppConfig();

        public static AppConfig Instance
        {
            get { return instance; }
        }

        #endregion Fields, Properties

        #region Methods

        /// <summary>
        /// Prevent new object of this class from outside
        /// </summary>
        private AppConfig()
        {
            LoadConfig();
        }

        /// <summary>
        /// Get all appSettings values in Web.config file
        /// </summary>
        /// <returns>A AppConfig object</returns>
        private void LoadConfig()
        {
            try
            {
                // Get objects's properties from memory cache
                var configProperties = typeof(AppConfig).GetProperties();

                foreach (var property in configProperties)
                {
                    var attr = property.GetCustomAttribute<ConfigAttribute>();
                    if (attr == null)
                    {
                        continue;
                    }

                    var configKey = attr.Key;
                    var configValueStr = attr.DefaultValue;

                    if (ConfigurationManager.AppSettings.AllKeys.Contains(configKey))
                    {
                        configValueStr = ConfigurationManager.AppSettings[configKey];
                    }
                    else if (attr.ThrowExceptionIfSourceNotExist)
                    {
                        throw new MissingFieldException(string.Format("Missing appSettings key [{0}] in Web.config file", configKey));
                    }

                    var configValue = ConvertData(property.PropertyType, configValueStr);
                    property.SetValue(this, configValue);
                }
            }
            catch (MissingFieldException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Convert a string value to another data type.
        /// </summary>
        /// <param name="propertyType">Destination data type</param>
        /// <param name="valueStr">Source value</param>
        /// <returns>New converted data in destination data type</returns>
        private static object ConvertData(Type propertyType, string valueStr)
        {
            object value = null;
            if (valueStr != null && typeof(string) != propertyType)
            {
                Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                value = System.Convert.ChangeType(valueStr, underlyingType);
            }
            else
            {
                value = valueStr;
            }

            return value;
        }

        #endregion Methods
    }
}
