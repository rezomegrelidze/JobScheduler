using JobScheduler.Models;
using JobScheduler.Services;
using JobScheduler.Tasks;
using Microsoft.Extensions.Logging;

namespace JobScheduler.Tests
{
    public class JobSchedulerTests
    {
        private readonly IJobScheduler _scheduler;
        private readonly CancellationTokenSource _cts;

        public JobSchedulerTests()
        {
            var logger = new LoggerFactory().CreateLogger<Services.JobScheduler>();
            _scheduler = new Services.JobScheduler(logger,new TaskRunner());
            _cts = new CancellationTokenSource();
        }

        [Fact]
        public async Task TestReportJobExecution()
        {
            // Create a report job that runs daily at 02:00 AM with unlimited occurrences
            var reportJob = new Job
            {
                JobId = Guid.NewGuid(),
                Name = "Daily Report",
                TimeOfExecution = new TimeSpan(2, 0, 0), // 02:00 AM
                TaskType = "DailyReport",
                Occurrences = null // Unlimited
            };

            // Register the job
            await _scheduler.RegisterJobAsync(reportJob);

            // Simulate the scheduler running at 02:00 AM
            var currentTime = new TimeSpan(2, 0, 0); // Assume it's 02:00 AM
            reportJob.TimeOfExecution = currentTime;

            // Run the scheduler to simulate job execution
            await _scheduler.RunScheduledJobsAsync(_cts.Token);

            // Retrieve the registered jobs and check the expected output
            var jobs = await _scheduler.GetJobsAsync();
            Assert.Single(jobs);
            Assert.Equal("Daily Report", jobs.First().Name);
        }

        [Fact]
        public async Task TestBackupJobExecution()
        {
            // Create a backup job that runs daily at 01:00 AM with a limit of 5 occurrences
            var backupJob = new Job
            {
                JobId = Guid.NewGuid(),
                Name = "Daily Backup",
                TimeOfExecution = new TimeSpan(1, 0, 0), // 01:00 AM
                TaskType = "DailyBackup",
                Occurrences = 5 // Run the job 5 times
            };

            // Register the job
            await _scheduler.RegisterJobAsync(backupJob);

            // Simulate running the job for 5 days
            for (int i = 0; i < 5; i++)
            {
                var currentTime = new TimeSpan(1, 0, 0); // 01:00 AM each day
                backupJob.TimeOfExecution = currentTime;

                // Run the scheduler each day at 01:00 AM
                await _scheduler.RunScheduledJobsAsync(_cts.Token);
            }

            // Check that the job has run exactly 5 times
            var jobs = await _scheduler.GetJobsAsync();
            Assert.Single(jobs);
            Assert.Equal(5, jobs.First().CompletedRuns);
            Assert.Equal("Daily Backup", jobs.First().Name);
        }
    }
}