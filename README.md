# SQLServerBackupsSystem
Simple system to back up SQL Server and upload to Google Cloud Storage, .bak file created for each database and uploaded to Google Cloud Storage.


### Attentions:

* Remember to use a `user` with sufficient permission to access the created backup files.

* Use a "Google Cloud Storage Buckets" `only for this app` as it deletes old files as per the "MaxDaysToStorageBackup" rule

* `SQLServerBackupDir` - You need to inform the directory where the sql server is configured to create the backup files, if you do not enter the location correctly, the files will not be located by the program and therefore will not be sent to Google Cloud Storage.

* `MaxDaysToStorageBackup` - Is the maximum time (Days) that a backup will be saved to storage, when the time is reached, the file will be deleted.

## appsettings.json example
----

```json
{
  "GoogleCredentialFile": "KeyAuth.json",
  "GoogleCloudStorageBucketName": "backupsfilessql",
  "MaxDaysToStorageBackup": 90,

  "HoursOfDayToRunBackup": [5,20],

  "SQLServerConfig": {
    "serverName": "16.130.170.63",
    "password": "pKg42yK%GjJz",
    "userName": "sa",
    "SQLServerBackupDir": "/var/opt/mssql/data/",
    "databaseNames": [
      "GitHub_DB",
      "Database_Name02",
      "databasename_Etc"
    ]
  },
  
  "DiscordWebHook_NotificationsURL": "https://discordapp.com/api/webhooks/XXXXXXXXXX"
}
```


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
