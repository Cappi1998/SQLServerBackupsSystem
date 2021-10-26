using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace SQLServerBackupsSystem.Interfaces
{
    internal class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly string bucketName;

        public GoogleCloudStorage(IConfiguration configuration)
        {
            googleCredential = GoogleCredential.FromFile(Path.Combine(Program.Base_Path, configuration.GetValue<string>("GoogleCredentialFile")));
            storageClient = StorageClient.Create(googleCredential);
            bucketName = configuration.GetValue<string>("GoogleCloudStorageBucketName");
        }

        public List<Google.Apis.Storage.v1.Data.Object> GetBucketListObjects()
        {
            List<Google.Apis.Storage.v1.Data.Object> storageObjects = storageClient.ListObjects(bucketName).ToList();

            return storageObjects;
        }
      

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }

        public bool UploadFileAsync(string Filapath, string fileNameForStorage)
        {

            using var fileStream = File.OpenRead(Filapath);
            var dataObject = storageClient.UploadObject(bucketName, fileNameForStorage, null, fileStream);

            if (dataObject.Id != null)
            {
                return true;
            }
            else return false;
        }
    }

    public interface ICloudStorage
    {
        bool UploadFileAsync(string Filapath, string fileNameForStorage);
        Task DeleteFileAsync(string fileNameForStorage);
        List<Google.Apis.Storage.v1.Data.Object> GetBucketListObjects();
    }
}
