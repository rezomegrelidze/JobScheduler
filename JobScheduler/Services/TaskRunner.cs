using System.Reflection;
using JobScheduler.Tasks;

namespace JobScheduler.Services;

public class TaskRunner
{
    private Dictionary<string,ITask> tasks;
    public TaskRunner()
    {
        var taskTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsAssignableTo(typeof(ITask)) && t.IsClass);
        tasks = taskTypes.Select(type => Activator.CreateInstance(type) as ITask)
                         .ToDictionary(task => task.TaskType);
    }

    public async Task RunTask(string taskType,CancellationToken token)
    {
        if (tasks.TryGetValue(taskType, out var task))
        {
            task.Task(token);
        }
    }
}