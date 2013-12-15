using System;

using log4net;

namespace MediaCommMvc.Web.Infrastructure
{
    public sealed class Log4NetLogger : ILogger
    {
        private readonly ILog log;

        public Log4NetLogger()
        {
            this.log = LogManager.GetLogger("Default");
        }

        public void Debug(string message)
        {
            if (this.log.IsDebugEnabled)
            {
                this.log.Debug(message);
            }
        }

        public void Debug(string message, params object[] data)
        {
            if (this.log.IsDebugEnabled)
            {
                this.log.Debug(this.CreateFormattedLogMessage(message, data));
            }
        }

        public void Error(string message)
        {
            if (this.log.IsErrorEnabled)
            {
                this.log.Error(message);
            }
        }

        public void Error(string message, params object[] data)
        {
            if (this.log.IsErrorEnabled)
            {
                this.log.Error(this.CreateFormattedLogMessage(message, data));
            }
        }

        public void Error(string message, Exception innerException)
        {
            if (this.log.IsErrorEnabled)
            {
                this.log.Error(message, innerException);
            }
        }

        public void Info(string message)
        {
            if (this.log.IsInfoEnabled)
            {
                this.log.Info(message);
            }
        }

        public void Info(string message, params object[] data)
        {
            if (this.log.IsInfoEnabled)
            {
                this.log.Info(this.CreateFormattedLogMessage(message, data));
            }
        }

        public void Warn(string message)
        {
            if (this.log.IsWarnEnabled)
            {
                this.log.Warn(message);
            }
        }

        public void Warn(string message, params object[] data)
        {
            if (this.log.IsWarnEnabled)
            {
                this.log.Warn(this.CreateFormattedLogMessage(message, data));
            }
        }

        private string CreateFormattedLogMessage(string message, object[] data)
        {
            string formattedLogMessage = message;
            try
            {
                formattedLogMessage = string.Format(message, data);
            }
            catch (FormatException ex)
            {
                string errorMessage = string.Format(
                    "The message to format does not fit the parameter count. Message: '{0}', Parameters: '{1}'", message, data);

                this.Error(errorMessage, ex);
            }

            return formattedLogMessage;
        }
    }
}