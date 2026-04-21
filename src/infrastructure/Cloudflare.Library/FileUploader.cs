using Amazon.S3;
using Amazon.S3.Transfer;

namespace GalaxyFootball.Infrastructure.Cloudflare
{
    public class FileUploader 
    {
        private readonly IAmazonS3 _s3;
       
        public FileUploader(string access_key, string secret_key, string service_url)
        {
            var s3Config = new AmazonS3Config
            {
                ServiceURL = service_url,
                ForcePathStyle = false
            };
            _s3 = new AmazonS3Client(access_key, secret_key, s3Config);
        }

        public async Task<bool> UploadFile(string bucket_name, string filepath, string destination_folder)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_s3);
                var destinationPath = Path.Combine(destination_folder, Path.GetFileName(filepath));

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucket_name,
                    Key = destinationPath, // Store in specified folder in the bucket
                    FilePath = filepath,
                    ContentType = "text/plain",
                    DisablePayloadSigning = true, // Critical for Cloudflare R2
                };
                await fileTransferUtility.UploadAsync(uploadRequest);
            }
            catch (Exception ex)
            {
                // Log the error (consider using a logging framework)
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return false;
            }
            return true;
        }
    }
}