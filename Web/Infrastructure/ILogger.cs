using System;

namespace MediaCommMvc.Web.Infrastructure
{
    public interface ILogger
    {
        void Debug(string message);

        void Debug(string message, params object[] data);

        void Error(string message);

        void Error(string message, params object[] data);

        void Error(string message, Exception innerException);

        void Info(string message);

        void Info(string message, params object[] data);

        void Warn(string message);

        void Warn(string message, params object[] data);
    }
}