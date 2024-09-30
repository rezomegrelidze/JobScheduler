// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using JobScheduler.Models;
using JobScheduler.Services;
using JobScheduler.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceProvider = new ServiceCollection()
    .AddLogging(config => config.AddConsole())
    .AddSingleton<IJobScheduler, JobScheduler.Services.JobScheduler>()
    .AddSingleton<TaskRunner>()
    .BuildServiceProvider();


var scheduler = serviceProvider.GetService<IJobScheduler>();

var reportJob = new Job
{
    JobId = Guid.NewGuid(),
    Name = "Daily Report",
    TimeOfExecution = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(1)), // execute job 1 minute after service starts,
    TaskType = TaskTypes.DailyReport,
    Occurrences = null // Unlimited
};

var backupJob = new Job
{
    JobId = Guid.NewGuid(),
    Name = "Daily Backup",
    TimeOfExecution = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(2)), // execute job 2 minute after service starts
    TaskType = TaskTypes.DailyBackup,
    Occurrences = 5 // Run the job 5 times
};

Job[] jobs = [reportJob, backupJob];
foreach (var job in jobs)
{
    await scheduler.RegisterJobAsync(job);
}

var cts = new CancellationTokenSource();

Task.Run(async () => await scheduler.RunScheduledJobsAsync(cts.Token));

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input == "get_state")
    {
        var state = await scheduler.GetJobsAsync();

        Console.WriteLine("The current jobs state: ");

        Console.WriteLine(JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true }));
    }
}