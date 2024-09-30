namespace JobScheduler.Tasks;

public interface ITask
{
    public string TaskType { get; }
    public Func<CancellationToken, Task> Task { get;  }
}