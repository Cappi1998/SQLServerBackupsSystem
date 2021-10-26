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
                //string fileDir = "BrasilSKins.bak";


                if (File.Exists(fileDir))
                {
                    
                    bool status = _cloudStorage.UploadFileAsync(fileDir, FileName);

                    if (status)
                    {
                        _logger.LogInformation($"File: {fileDir} uploaded to Google Cloud successfully!");
                        File.Delete(fileDir);
                    }
                    else
                    {
                        _logger.LogError($"FAILED to Upload File: {fileDir}");
                    }
                }
                else
                {
                    _logger.LogError($"File: {fileDir} not Found!");
                }

            }

            return Task.CompletedTask;
        }
    }
}
