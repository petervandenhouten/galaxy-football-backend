using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GalaxyFootball.Infrastructure
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly IAmazonS3 _s3;
       
        private readonly IConfiguration m_configuration;

        public LogsController(IConfiguration configuration)
        {
             m_configuration = configuration;

            var access_key = m_configuration["CLOUDFLARE:ACCESS_KEY"];
            var secret_key = m_configuration["CLOUDFLARE:SECRET_KEY"];
            var service_url = m_configuration["CLOUDFLARE:S3_URL"];

            var s3Config = new AmazonS3Config
            {
                ServiceURL = service_url,
                ForcePathStyle = false
            };
            _s3 = new AmazonS3Client(access_key, secret_key, s3Config);
        }


        [HttpGet]
        public async Task<IActionResult> ListLogs()
        {
            var bucket_name = m_configuration["CLOUDFLARE:BUCKET_NAME"];

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
            return Ok(files);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetLog([FromRoute] string key)
        {
            var bucket_name = m_configuration["CLOUDFLARE:BUCKET_NAME"];

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
            return Content(content, "text/plain");
        }
    }
}