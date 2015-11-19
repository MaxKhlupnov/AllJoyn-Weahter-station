using WinRTXamlToolkit.Debugging;
using RemoteMonitoring.Logging;


namespace SensorClient.Common
{
    /// <summary>
    /// Default implementation of ILogger with the System.Diagnostics.Trace 
    /// object as the logger.
    /// </summary>
    public class TraceLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Debug.WriteLine(message);
        }

        public void LogInfo(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public void LogWarning(string message)
        {
            Debug.WriteLine(message);
        }

        public void LogWarning(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public void LogError(string message)
        {
            Debug.WriteLine(message);
        }

        public void LogError(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }
    }
}
