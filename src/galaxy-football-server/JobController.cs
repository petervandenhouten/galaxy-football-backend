
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/job")]
public class JobController : ControllerBase
{
    private readonly ILogger<JobController> m_logger;
    private readonly JobService m_jobService;

    public JobController(ILogger<JobController> logger, JobService job_service)
    {
        m_logger     = logger;
        m_jobService = job_service;
    }

    [HttpPost("internal/run-daily-job")]
    // public async Task<IActionResult> RunInternal((Name = "X-API-KEY")] string key)
    public async Task<IActionResult> RunInternal()
    {
        //if (key != _config["InternalApiKey"])
        //    return Unauthorized();

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
