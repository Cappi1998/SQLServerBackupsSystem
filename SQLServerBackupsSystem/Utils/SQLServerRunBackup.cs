using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;


namespace SQLServerBackupsSystem.Utils
{
    public class SQLServerRunBackup
    {
        public static void BackupDatabase(string databaseName, string FileName)
        {

            ServerConnection serverConnection = new ServerConnection(Program.sqlConfig.serverName, Program.sqlConfig.userName, Program.sqlConfig.password);
            Server server = new Server(serverConnection);


            Backup backup = new Backup();
            backup.Action = BackupActionType.Database;
            backup.BackupSetDescription = $"{databaseName} - full backup";
            backup.BackupSetName = $"{databaseName} backup";
            backup.Database = databaseName;

            BackupDeviceItem deviceItem = new BackupDeviceItem(FileName, Microsoft.SqlServer.Management.Smo.DeviceType.File);

            backup.Devices.Add(deviceItem);
            backup.Incremental = false;
            backup.LogTruncation = BackupTruncateLogType.Truncate;
            backup.SqlBackup(server);
            
        }
    }
}
