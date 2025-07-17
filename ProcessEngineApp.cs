using Microsoft.AspNetCore.Http.Json;
using ProcessEngine.Core.Models;
using System.Text.Json;

namespace ProcessEngine.Api
{
    public class ProcessEngineApp
    {
        public static void Main(string[] args)
        {
            var applicationBuilder = WebApplication.CreateBuilder(args);
            
            // Configure JSON serialization
            applicationBuilder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            
            var processApp = applicationBuilder.Build();
            
            // Repository pattern for data storage
            var blueprintRepository = new List<ProcessBlueprint>();
            var executionRepository = new List<ProcessExecution>();
            
            // Configure API endpoints
            ConfigureProcessBlueprintEndpoints(processApp, blueprintRepository);
            ConfigureProcessExecutionEndpoints(processApp, blueprintRepository, executionRepository);
            
            processApp.Run();
        }
        
        private static void ConfigureProcessBlueprintEndpoints(WebApplication app, List<ProcessBlueprint> repository)
        {
            // Create a new process blueprint
            app.MapPost("/api/process-blueprints", (ProcessBlueprint blueprint) =>
            {
                if (repository.Any(b => b.ProcessId == blueprint.ProcessId))
                    return Results.Conflict("Process blueprint with this ID already exists.");
                
                repository.Add(blueprint);
                return Results.Created($"/api/process-blueprints/{blueprint.ProcessId}", blueprint);
            })
            .WithName("CreateProcessBlueprint")
            .WithTags("Process Management");
            
            // Retrieve all process blueprints
            app.MapGet("/api/process-blueprints", () =>
            {
                return Results.Ok(repository.Select(b => new
                {
                    b.ProcessId,
                    b.ProcessTitle,
                    b.CreatedAt,
                    StepCount = b.ProcessSteps.Count,
                    TransitionCount = b.ProcessTransitions.Count
                }));
            })
            .WithName("GetAllProcessBlueprints")
            .WithTags("Process Management");
            
            // Retrieve a specific process blueprint
            app.MapGet("/api/process-blueprints/{processId}", (string processId) =>
            {
                var blueprint = repository.FirstOrDefault(b => b.ProcessId == processId);
                return blueprint != null ? Results.Ok(blueprint) : Results.NotFound($"Process blueprint '{processId}' not found.");
            })
            .WithName("GetProcessBlueprint")
            .WithTags("Process Management");
        }
        
        private static void ConfigureProcessExecutionEndpoints(WebApplication app, List<ProcessBlueprint> blueprintRepo, List<ProcessExecution> executionRepo)
        {
            // Start a new process execution
            app.MapPost("/api/process-blueprints/{blueprintId}/executions", (string blueprintId) =>
            {
                var blueprint = blueprintRepo.FirstOrDefault(b => b.ProcessId == blueprintId);
                if (blueprint == null)
                    return Results.NotFound($"Process blueprint '{blueprintId}' not found.");
                
                var startingStep = blueprint.ProcessSteps.FirstOrDefault(s => s.IsStartingPoint);
                if (startingStep == null)
                    return Results.BadRequest("No starting step defined in the process blueprint.");
                
                var executionId = Guid.NewGuid().ToString("N")[..8]; // Short ID for better UX
                var execution = new ProcessExecution(executionId, blueprintId, startingStep.StepId, new List<string> { startingStep.StepId });
                
                executionRepo.Add(execution);
                return Results.Created($"/api/process-executions/{executionId}", execution);
            })
            .WithName("StartProcessExecution")
            .WithTags("Process Execution");
            
            // Get process execution details
            app.MapGet("/api/process-executions/{executionId}", (string executionId) =>
            {
                var execution = executionRepo.FirstOrDefault(e => e.ExecutionId == executionId);
                return execution != null ? Results.Ok(execution) : Results.NotFound($"Process execution '{executionId}' not found.");
            })
            .WithName("GetProcessExecution")
            .WithTags("Process Execution");
            
            // Execute a transition in a process
            app.MapPost("/api/process-executions/{executionId}/transitions", (string executionId, TransitionExecutionRequest request) =>
            {
                var execution = executionRepo.FirstOrDefault(e => e.ExecutionId == executionId);
                if (execution == null)
                    return Results.NotFound($"Process execution '{executionId}' not found.");
                
                var blueprint = blueprintRepo.FirstOrDefault(b => b.ProcessId == execution.BlueprintId);
                if (blueprint == null)
                    return Results.NotFound($"Process blueprint '{execution.BlueprintId}' not found.");
                
                var transition = blueprint.ProcessTransitions.FirstOrDefault(t => t.TransitionId == request.TransitionId);
                if (transition == null)
                    return Results.NotFound($"Transition '{request.TransitionId}' not found in process.");
                
                // Validate transition is allowed from current step
                if (!transition.SourceSteps.Contains(execution.ActiveStep))
                    return Results.BadRequest($"Transition '{request.TransitionId}' is not valid from current step '{execution.ActiveStep}'.");
                
                // Check if already in final step
                var currentStep = blueprint.ProcessSteps.FirstOrDefault(s => s.StepId == execution.ActiveStep);
                if (currentStep != null && currentStep.IsEndingPoint)
                    return Results.BadRequest("Process execution is already completed.");
                
                // Execute the transition
                execution.ActiveStep = transition.TargetStep;
                execution.StepTrace.Add(transition.TargetStep);
                
                // Mark as completed if reached final step
                var newStep = blueprint.ProcessSteps.FirstOrDefault(s => s.StepId == transition.TargetStep);
                if (newStep != null && newStep.IsEndingPoint)
                {
                    execution.MarkAsCompleted();
                }
                
                return Results.Ok(execution);
            })
            .WithName("ExecuteTransition")
            .WithTags("Process Execution");
        }
    }
}