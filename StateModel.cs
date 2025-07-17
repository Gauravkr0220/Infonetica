namespace ProcessEngine.Core.Models
{
    /// <summary>
    /// Represents a single step in a business process
    /// </summary>
    public class ProcessStep
    {
        public string StepId { get; set; }
        public string Description { get; set; }
        public bool IsStartingPoint { get; set; }
        public bool IsEndingPoint { get; set; }
        
        public ProcessStep(string stepId, string description, bool isStartingPoint = false, bool isEndingPoint = false)
        {
            StepId = stepId;
            Description = description;
            IsStartingPoint = isStartingPoint;
            IsEndingPoint = isEndingPoint;
        }
    }
}