using System.Text.Json.Serialization;

namespace JobScheduler.Models;

public class Job
{
    public Guid JobId { get; set; }
    public string Name { get; set; }
    public TimeSpan TimeOfExecution { get; set; }
    public string TaskType { get; set; } 
    public int? Occurrences { get; set; } 
    public DateTime LastRun { get; set; } 
    public int CompletedRuns { get; set; } 
}