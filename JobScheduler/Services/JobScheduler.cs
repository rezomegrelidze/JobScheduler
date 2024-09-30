using JobScheduler.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace JobScheduler.Services;

public class JobScheduler : IJobScheduler
{
    private readonly SemaphoreSlim _fileSemaphore = new SemaphoreSlim(1,1); // use semaphore slim to allow locking in async/await
    private readonly ILogger<JobScheduler> _logger;
    private readonly TaskRunner _runner;
    private List<Job> _jobs = new List<Job>();

    public JobScheduler(ILogger<JobScheduler> logger,TaskRunner runner)
    {
        _logger = logger;
        _runner = runner;
        LoadJobsFromFileAsync();
    }

    public async Task RegisterJobAsync(Job job)
    {
        _jobs.Add(job);
        _logger.LogInformation($"Job '{job.Name}' registered for {job.TimeOfExecution}.");
        await SaveJobsToFileAsync();
    }

    public async Task<List<Job>> GetJobsAsync()
    {
        await LoadJobsFromFileAsync();
        return _jobs;
    }

    public async Task RunScheduledJobsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            foreach (var job in _jobs)
            {
                if (job.TimeOfExecution <= currentTime &&
                    (job.LastRun.Date != DateTime.Today) &&
                    (job.Occurrences == null || job.CompletedRuns < job.Occurrences))
                {
                    _logger.LogInformation($"Executing job '{job.Name}' at {currentTime}.");
                    var jobToRun = job;
                    Task.Run(async () =>
                    {
                        await _runner.RunTask(jobToRun.TaskType,token);
                        jobToRun.LastRun = DateTime.Now;
                        jobToRun.CompletedRuns++;
                        await SaveJobsToFileAsync();
                    },token);
                    
                }
            }
            await Task.Delay(TimeSpan.FromMinutes(1)); // Check every minute
        }
    }

    private async Task SaveJobsToFileAsync()
    {
        var json = JsonSerializer.Serialize(_jobs, new JsonSerializerOptions { WriteIndented = true });
        await _fileSemaphore.WaitAsync();
        try
        {
            await File.WriteAllTextAsync("jobs.json", json);
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    private async Task LoadJobsFromFileAsync()
    {
        if (File.Exists("jobs.json"))
        {
            await _fileSemaphore.WaitAsync();
            string json = null;
            try
            {
                json = await File.ReadAllTextAsync("jobs.json");
            }
            finally
            {
                _fileSemaphore.Release();
            }

            if (json != null)
            {
                _jobs = JsonSerializer.Deserialize<List<Job>>(json);
            }
        }
    }
}