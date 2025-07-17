using ProcessEngine.Core.Models;

namespace ProcessEngine.Core.Models
{
    /// <summary>
    /// Represents a complete business process template
    /// </summary>
    public class ProcessBlueprint
    {
        public string ProcessId { get; set; }
        public string ProcessTitle { get; set; }
        public List<ProcessStep> ProcessSteps { get; set; }
        public List<ProcessTransition> ProcessTransitions { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public ProcessBlueprint(string processId, string processTitle, List<ProcessStep> processSteps, List<ProcessTransition> processTransitions)
        {
            ProcessId = processId;
            ProcessTitle = processTitle;
            ProcessSteps = processSteps ?? new List<ProcessStep>();
            ProcessTransitions = processTransitions ?? new List<ProcessTransition>();
            CreatedAt = DateTime.UtcNow;
        }
    }
    
    /// <summary>
    /// Represents a running instance of a business process
    /// </summary>
    public class ProcessExecution
    {
        public string ExecutionId { get; set; }
        public string BlueprintId { get; set; }
        public string ActiveStep { get; set; }
        public List<string> StepTrace { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; }
        
        public ProcessExecution(string executionId, string blueprintId, string activeStep, List<string> stepTrace)
        {
            ExecutionId = executionId;
            BlueprintId = blueprintId;
            ActiveStep = activeStep;
            StepTrace = stepTrace ?? new List<string>();
            StartedAt = DateTime.UtcNow;
            Status = "Running";
        }
        
        public void MarkAsCompleted()
        {
            CompletedAt = DateTime.UtcNow;
            Status = "Completed";
        }
    }
    
    /// <summary>
    /// Request model for executing a transition
    /// </summary>
    public class TransitionExecutionRequest
    {
        public string TransitionId { get; set; }
        public string RequestedBy { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        
        public TransitionExecutionRequest(string transitionId, string requestedBy = "System")
        {
            TransitionId = transitionId;
            RequestedBy = requestedBy;
            Parameters = new Dictionary<string, object>();
        }
    }

    public class ProcessTransition
    {
        public string TransitionId { get; set; }
        public List<string> SourceSteps { get; set; }
        public string TargetStep { get; set; }
        public string Description { get; set; }

        public ProcessTransition(string transitionId, List<string> sourceSteps, string targetStep, string description)
        {
            TransitionId = transitionId;
            SourceSteps = sourceSteps ?? new List<string>();
            TargetStep = targetStep;
            Description = description;
        }
    }
}