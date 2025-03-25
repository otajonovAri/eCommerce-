using Serilog;

namespace EC.SharedLibrary.Logs;

public static class LogException
{
    public static void LogExceptions(Exception exception)
    {
        LogToFile(exception.Message);
        LogToConsole(exception.Message);
        LogToDebugger(exception.Message);
    }

    static void LogToConsole(string message) => Log.Warning(message);

    static void LogToDebugger(string message) => Log.Debug(message);
    static void LogToFile(string message) => Log.Information(message);
}
