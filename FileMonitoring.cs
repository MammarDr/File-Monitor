using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace File_Monitoring_Windows_Service
{
    public partial class FileMonitoring : ServiceBase
    {

        private FileSystemWatcher _watcher;
        private string targetDirectory;
        private string observableDirectory;

       public FileMonitoring()
        {
            InitializeComponent();
        
            observableDirectory = ConfigurationManager.AppSettings["observableDirectory"];
        
            targetDirectory = ConfigurationManager.AppSettings["targetDirectory"];
        
            if (string.IsNullOrEmpty(observableDirectory))
                observableDirectory = "C:\\CodingEnviorenment\\File Monitoring";
        
            if (string.IsNullOrEmpty(targetDirectory))
                targetDirectory = "C:\\CodingEnviorenment\\File Monitoring\\TargetDirectory";
        
            Directory.CreateDirectory(observableDirectory);
            Directory.CreateDirectory(targetDirectory);
        
            _watcher = new FileSystemWatcher(observableDirectory, "*.*");
            _watcher.NotifyFilter = NotifyFilters.FileName;
            _watcher.Created += OnNewFileDetected;
            _watcher.EnableRaisingEvents = true;
        }

        private void Log(string message, EventLogEntryType type)
        {
            string sourceName = "FileMonitoring";

            if(!EventLog.SourceExists(sourceName)) {
                try
                {
                    EventLog.CreateEventSource(sourceName, "Application");
                    Log("EventLog Source has been Created", EventLogEntryType.Information);
                } catch(Exception) {
                    return;
                }
            }

            EventLog.WriteEntry(sourceName, message, type);
        }

        private void OnNewFileDetected(object sender, FileSystemEventArgs e)
        {
            TransferFile(Path.GetFileName(e.FullPath));
        }

        private void TransferFile(string fileName)
        {
            string filePath = Path.Combine(observableDirectory, fileName);

            if (!File.Exists(filePath)) {
                Log($"Failed to Fetch the File named {fileName}", EventLogEntryType.Error);
                return;
            }

            string guid = Guid.NewGuid().ToString();

            string targetPath = Path.Combine(targetDirectory, guid + Path.GetExtension(fileName));

            try
            {
                File.Copy(filePath, targetPath, true);

                File.Delete(filePath);

                Log($"'{fileName}' File has been Transfered", EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                Log($"Error while Transfering a File {fileName} : " + e.Message, EventLogEntryType.Error);
            }

        }

        protected override void OnStart(string[] args)
        {
            
        }

        protected override void OnStop()
        {
            _watcher.Dispose();
        }

    }
}
