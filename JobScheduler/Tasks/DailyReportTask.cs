namespace JobScheduler.Tasks;

public class DailyReportTask : ITask
{
    public string TaskType => "DailyReport";

    public Func<CancellationToken, Task> Task => async
        (token) => await System.Threading.Tasks.Task.Run(() =>
        {
            // Simulate generating a report
            Console.WriteLine("Generating daily report...");
        }, token);
}

public class DailyBackupTask : ITask
{
    public string TaskType => "DailyBackup";

    public Func<CancellationToken, Task> Task => async
        (token) => await System.Threading.Tasks.Task.Run(() =>
        {
            // Simulate performing a backup
            Console.WriteLine("Performing daily backup...");
        }, token);
}