
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/job")]
public class JobController : ControllerBase
{
    private readonly ILogger<JobController> m_logger;
    private readonly JobService m_jobService;
    private readonly IConfiguration m_configuration;

    public JobController(ILogger<JobController> logger, JobService job_service, IConfiguration configuration)
    {
        m_logger        = logger;
        m_jobService    = job_service;
        m_configuration = configuration;
    }

    [HttpPost("internal/run-daily-job")]
    public async Task<IActionResult> RunInternal(
        [FromHeader(Name = "X-API-KEY")] string apiKey)
    {
        if (apiKey != m_configuration["GALAXY_FOOTBALL_API_KEY"])
            return Unauthorized();

        await m_jobService.RunIfNeeded();
        return Ok();
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost("admin/run-daily-job")]
    public async Task<IActionResult> RunAdmin()
    {
        await m_jobService.ForceRun();
        return Ok();
    }
}
