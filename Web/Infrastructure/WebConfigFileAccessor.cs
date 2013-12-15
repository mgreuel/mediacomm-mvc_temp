using System.Configuration;

namespace MediaCommMvc.Web.Infrastructure
{
    public class FileConfigAccessor : IConfigAccessor
    {
        private readonly ILogger logger;

        public FileConfigAccessor(ILogger logger)
        {
            this.logger = logger;
        }

        public string GetConfigValue(string key)
        {
            this.logger.Debug("Getting configuration value for key '{0}'", key);

            string value = ConfigurationManager.AppSettings[key];

            if (value == null)
            {
                throw new ConfigurationErrorsException(string.Format("Configuration value with the key {0} does not exist.", key));
            }

            this.logger.Debug("Got '{0}' as configuration value for key '{1}'", value, key);

            return value;
        }
    }
}