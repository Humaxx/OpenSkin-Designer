using System;
using System.IO;
using System.Threading;

public static class Logger
{
    private static readonly object _lock = new object();
    private static string _logFilePath = @"C:\log\log.txt";
    private static bool _isLoggingEnabled = true; // Variable zum Ein-/Ausschalten des Loggers

    static Logger()
    {
        // Überprüfen, ob die Logdatei bereits existiert
        if (File.Exists(_logFilePath))
        {
            // Alte Logdatei löschen
            File.Delete(_logFilePath);
            Console.WriteLine("Alte Logdatei wurde gelöscht.");
        }

        // Ordner für Logdatei erstellen, wenn nötig
        string logFolderPath = Path.GetDirectoryName(_logFilePath);
        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
            Console.WriteLine($"Ordner '{logFolderPath}' wurde erstellt.");

            // Startmeldung in die Datei schreiben
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine("---- Logging gestartet ----");
                Console.WriteLine("Startmeldung in Logdatei geschrieben.");
            }
        }
    }

    // Methode zum Aktivieren/Deaktivieren des Loggings
    public static void SetLoggingEnabled(bool isEnabled)
    {
        _isLoggingEnabled = isEnabled;
        Console.WriteLine($"Logging ist jetzt {(isEnabled ? "aktiviert" : "deaktiviert")}.");
    }

    public static void LogMessage(string message)
    {
        if (!_isLoggingEnabled) return; // Wenn Logging deaktiviert ist, abbrechen

        // Nachricht in die Datei schreiben
        lock (_lock)
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine(message);
                Console.WriteLine($"Logmeldung '{message}' in Datei geschrieben.");
            }
        }
    }
}