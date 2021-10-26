using Coravel.Invocable;
using SQLServerBackupsSystem.Interfaces;
using SQLServerBackupsSystem.Utils;

namespace SQLServerBackupsSystem.Schedules
{
    internal class BucketStatusCheck : IInvocable
    {
        private readonly ILogger _logger;
        private readonly ICloudStorage _cloudStorage;
        public BucketStatusCheck(ILogger<AutomaticProcess> logger, ICloudStorage cloudStorage)
        {
            _logger = logger;
            _cloudStorage = cloudStorage;
        }

        public Task Invoke()
        {
            var Discord_MSG = $"MaxDaysToStorageBackup: {Program.MaxDaysToStorageBackup} Days!{Environment.NewLine}";

            var files = _cloudStorage.GetBucketListObjects();

            foreach(Google.Apis.Storage.v1.Data.Object obj in files)
            {
                TimeSpan? time = DateTime.Now - obj.Updated;
                if (time == null) continue;

                if(time.Value.TotalDays > Program.MaxDaysToStorageBackup)
                {
                    _cloudStorage.DeleteFileAsync(obj.Name);
                    string msg = $"{obj.Name} will be deleted as it has exceeded the maximum storage time allowed!{Environment.NewLine}";
                    _logger.LogInformation(msg);
                    Discord_MSG = Discord_MSG + msg;
                }
            }
            
            DiscordWebHook.SendMessage(Discord_MSG);
            return Task.CompletedTask;
        }
    }
}
