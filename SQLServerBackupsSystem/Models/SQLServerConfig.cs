namespace SQLServerBackupsSystem.Models
{
    internal class SQLServerConfig
    {
        public string serverName { get; set; }
        public string password { get; set; }
        public string userName { get; set; }

        public List<string> databaseNames { get; set; }
        public string SQLServerBackupDir { get; set; }
    }
}
