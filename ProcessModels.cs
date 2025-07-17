// --------------- Models ----------------

public class State
{
    public string? StateId { get; set; }
    public string? Name { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
}

public class ActionDef
{
    public string ActionId { get; set; }
    public string Name { get; set; }
    public string FromState { get; set; }
    public string ToState { get; set; }
}

public record WorkflowDefinition(
    string Id,
    string Name,
    List<State> States,
    List<ActionDef> Actions
);

public class WorkflowInstance{
    public string Id { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentState { get; set; }
    public List<string> History { get; set; }

    public WorkflowInstance(string id, string workflowId, string currentState, List<string> history)
    {
        Id = id;
        WorkflowId = workflowId;
        CurrentState = currentState;
        History = history;
    }
}

public record ExecuteActionRequest(string ActionId);
