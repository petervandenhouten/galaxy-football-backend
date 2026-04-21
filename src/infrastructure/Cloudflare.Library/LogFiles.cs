using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace GalaxyFootball.Infrastructure.Cloudflare
{
    public class LogFiles 
    {
        private readonly IAmazonS3 _s3;
       
        public LogFiles(string access_key, string secret_key, string service_url)
        {
            var s3Config = new AmazonS3Config
            {
                ServiceURL = service_url,
                ForcePathStyle = false
            };
            _s3 = new AmazonS3Client(access_key, secret_key, s3Config);
        }

        public async Task<List<string>> ListLogs(string bucket_name)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucket_name,
                Prefix = "logs/" // adjust if your logs are in a subfolder
            };
            var response = await _s3.ListObjectsV2Async(request);
            // Return only the filename (remove folder prefix)
            var files = response.S3Objects
                .Select(o => o.Key.StartsWith("logs/") ? o.Key.Substring("logs/".Length) : o.Key)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
            return files;
        }

        public async Task<string> GetLog(string bucket_name, string key)
        {
            // Prepend the folder prefix to the key
            var s3Key = $"logs/{key}";
            var request = new GetObjectRequest
            {
                BucketName = bucket_name,
                Key = s3Key
            };
            using var response = await _s3.GetObjectAsync(request);
            using var reader = new StreamReader(response.ResponseStream);
            var content = await reader.ReadToEndAsync();
            // Return as plain text for better client compatibility
            return content;
        }
    }
}