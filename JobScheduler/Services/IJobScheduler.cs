using JobScheduler.Models;

namespace JobScheduler.Services;

public interface IJobScheduler
{
    Task RegisterJobAsync(Job job);
    Task<List<Job>> GetJobsAsync();
    Task RunScheduledJobsAsync(CancellationToken token);
}