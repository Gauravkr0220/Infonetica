# WorkflowEngine State Machine

## Overview

This project implements a **generic state machine engine** for workflow automation. It allows you to define any process as a set of steps (states) and transitions, then execute and track process instances as they move through those steps. Typical use cases include approvals, onboarding, document flows, or any business process that follows a defined path.

---

## Features

- **Blueprints:** Define reusable process templates with steps and transitions.
- **Executions:** Start and track process instances based on blueprints.
- **Transitions:** Move executions between steps according to allowed transitions.
- **Traceability:** View the full history of each execution.
- **REST API:** Interact with the engine via simple HTTP endpoints.

---

## How It Works

1. **Define a Blueprint:**  
   Describe your process as steps (states) and transitions (allowed moves).
2. **Start an Execution:**  
   Create a new instance of the process, which begins at the starting step.
3. **Advance the Execution:**  
   Use transitions to move the execution through the process steps.
4. **Complete the Process:**  
   When the execution reaches an ending step, the process is complete.

---

## Example Use Case

Suppose you want to automate employee onboarding:

- **Steps:**  
  - Collect Documents  
  - HR Approval  
  - Setup Account

- **Transitions:**  
  - Docs to HR  
  - HR to Setup

You define this as a blueprint, then start an execution for each new employee.

---

## API Usage: Step-by-Step

### 1. Create a Process Blueprint

**POST** `/api/process-blueprints`

**Body:**
```json
{
  "processId": "onboarding",
  "processTitle": "Employee Onboarding",
  "processSteps": [
    { "stepId": "step1", "description": "Collect Docs", "isStartingPoint": true, "isEndingPoint": false },
    { "stepId": "step2", "description": "HR Approval", "isStartingPoint": false, "isEndingPoint": false },
    { "stepId": "step3", "description": "Setup Account", "isStartingPoint": false, "isEndingPoint": true }
  ],
  "processTransitions": [
    { "transitionId": "t1", "sourceSteps": ["step1"], "targetStep": "step2", "description": "Docs to HR" },
    { "transitionId": "t2", "sourceSteps": ["step2"], "targetStep": "step3", "description": "HR to Setup" }
  ]
}
```

---

### 2. List All Blueprints

**GET** `/api/process-blueprints`

---

### 3. Get a Specific Blueprint

**GET** `/api/process-blueprints/{processId}`

---

### 4. Start a New Execution

**POST** `/api/process-blueprints/{blueprintId}/executions`

---

### 5. Get Execution Details

**GET** `/api/process-executions/{executionId}`

---

### 6. Execute a Transition

**POST** `/api/process-executions/{executionId}/transitions`

**Body Example:**
```json
{ "transitionId": "t1" }
```

---

### 7. Repeat Step 6 Until Final State

Continue posting transitions until the execution reaches an ending step.

---

## How to Use

1. **Define your process** as a blueprint using the API.
2. **Start executions** for each real-world instance of your process.
3. **Advance executions** by posting transitions as work is completed.
4. **Query executions** to monitor progress and see history.

---

## Notes

- All endpoints use JSON.
- No authentication is required (for demo/testing).
- You can define any process, not just onboarding—this is a generic engine.

---

## Example Flow

1. Create blueprint for onboarding.
2. Start execution for a new employee.
3. Move from "Collect Docs" to "HR Approval" (`transitionId: t1`).
4. Move from "HR Approval" to "Setup Account" (`transitionId: t2`).
5. Execution is now complete.

---

For more details, see the code files