using System;
using System.IO;
using System.Timers;
using Timer = System.Timers.Timer;

namespace FolderSync
{
    class Program
    {
        private static Timer syncTimer;
        
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: FolderSync <source> <replica> <interval> <logFile>");
                return;
            }

            string sourcePath = args[0];
            string replicaPath = args[1];
            if (!int.TryParse(args[2], out int interval))
            {
                Console.WriteLine("Invalid interval. It should be an integer representing seconds.");
                return;
            }

            string logFilePath = args[3];

            Console.WriteLine($"Source Path: {sourcePath}");
            Console.WriteLine($"Replica Path: {replicaPath}");
            Console.WriteLine($"Interval: {interval} seconds");
            Console.WriteLine($"Log File Path: {logFilePath}");

            // Ensure the source folder exists
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine($"Source folder '{sourcePath}' does not exist.");
                return;
            }

            // Ensure the replica folder exists, or create it
            if (!Directory.Exists(replicaPath))
            {
                Directory.CreateDirectory(replicaPath);
                Log($"Created replica directory: {replicaPath}", logFilePath);
            }

            SyncFolders(sourcePath, replicaPath, logFilePath);
            
            // Set up timer for periodic synchronization
            syncTimer = new Timer(interval * 1000); // interval in milliseconds
            syncTimer.Elapsed += (sender, e) => SyncFolders(sourcePath, replicaPath, logFilePath);
            syncTimer.AutoReset = true;
            syncTimer.Start();

            Console.WriteLine("Press Enter to stop the program...");
            Console.ReadLine();

            // Clean up resources
            syncTimer.Stop();
            syncTimer.Dispose();
        }

        static void SyncFolders(string sourcePath, string replicaPath, string logFilePath)
        {
            try
            {
                // Copy new and updated files
                foreach (var sourceFile in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(sourcePath, sourceFile);
                    string replicaFile = Path.Combine(replicaPath, relativePath);

                    // Ensure the directory exists in the replica
                    string replicaDir = Path.GetDirectoryName(replicaFile);
                    if (!Directory.Exists(replicaDir))
                    {
                        Directory.CreateDirectory(replicaDir);
                        Log($"Created directory: {replicaDir}", logFilePath);
                    }

                    // Copy file if it does not exist or if it has been modified
                    if (!File.Exists(replicaFile) ||
                        File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(replicaFile))
                    {
                        File.Copy(sourceFile, replicaFile, true);
                        Log($"Copied file: {sourceFile} to {replicaFile}", logFilePath);
                    }
                }

                // Delete files not in source
                foreach (var replicaFile in Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(replicaPath, replicaFile);
                    string sourceFile = Path.Combine(sourcePath, relativePath);

                    if (!File.Exists(sourceFile))
                    {
                        File.Delete(replicaFile);
                        Log($"Deleted file: {replicaFile}", logFilePath);
                    }
                }

                // Delete directories not in source
                foreach (var replicaDir in Directory.GetDirectories(replicaPath, "*", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(replicaPath, replicaDir);
                    string sourceDir = Path.Combine(sourcePath, relativePath);

                    if (!Directory.Exists(sourceDir))
                    {
                        Directory.Delete(replicaDir, true);
                        Log($"Deleted directory: {replicaDir}", logFilePath);
                    }
                }

                Log("Synchronization completed successfully.", logFilePath);
            }
            catch (Exception ex)
            {
                Log($"Error during synchronization: {ex.Message}", logFilePath);
            }
        }


        static void Log(string message, string logFilePath)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"{timestamp} - {message}";
            Console.WriteLine(logEntry);
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
    }
}