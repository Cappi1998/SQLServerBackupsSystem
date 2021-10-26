using Coravel.Invocable;
using SQLServerBackupsSystem.Interfaces;
using SQLServerBackupsSystem.Utils;

namespace SQLServerBackupsSystem.Schedules
{
    class AutomaticProcess : IInvocable
    {
        private readonly ILogger _logger;
        private readonly ICloudStorage _cloudStorage;
        public AutomaticProcess(ILogger<AutomaticProcess> logger,ICloudStorage cloudStorage)
        {
            _logger = logger;
            _cloudStorage = cloudStorage;
        }

        public Task Invoke()
        {
            
            foreach(var db in Program.sqlConfig.databaseNames)
            {
                string FileName = $"{db} {DateTime.Now.ToString("g").Replace("/", ".").Replace(":",".")}.bak";


                _logger.LogInformation($"Start Create DB: {FileName}");
                try
                {
                    SQLServerRunBackup.BackupDatabase(db, FileName);
                    _logger.LogInformation($"Backup for {FileName} is done!");
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                string fileDir = Path.Combine(Program.sqlConfig.SQLServerBackupDir, FileName);

                if (File.Exists(fileDir))
                {
                    
                    bool status = _cloudStorage.UploadFileAsync(fileDir, FileName);

                    if (status)
                    {
                        string msg = $"File: {fileDir} uploaded to Google Cloud successfully!";
                        _logger.LogInformation(msg);
                        DiscordWebHook.SendMessage(msg);

                        FileInfo fileInfo = new System.IO.FileInfo(fileDir);
                        fileInfo.Delete();
                    }
                    else
                    {
                        string msg = $"FAILED to Upload File: {fileDir}";
                        _logger.LogError(msg);
                        DiscordWebHook.SendMessage(msg);
                    }
                }
                else
                {
                    string msg = $"File: {fileDir} not Found!";
                    _logger.LogError(msg);
                    DiscordWebHook.SendMessage(msg);
                }

            }

            return Task.CompletedTask;
        }
    }
}
