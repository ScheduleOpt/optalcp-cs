# OptalCP C# API

A C# API for the OptalCP constraint programming solver.

> [!WARNING]
> **Disclaimer**: This is an unofficial C# API and is not affiliated with, supported by, or endorsed by the OptalCP team. Neither the maintainers of this project nor the OptalCP team assume any liability for bugs, errors, or issues arising from its use. This software is provided "as is" without warranty of any kind. You are free to use this package, but you do so entirely at your own risk.

> 📚 **Full API Documentation**: For complete API reference with detailed descriptions, see the [OptalCP Python API Documentation](http://dev.vilim.eu/python-api/index.html) - the C# API provides identical functionality with C#-idiomatic syntax.

---

## Installation

```bash
dotnet add package OptalCP
```

## Prerequisites

You need the OptalCP solver executable. Set the path via environment variable:

```bash
export OPTALCP_SOLVER=/path/to/optalcp
```

Or pass it directly when solving:

```csharp
var result = model.Solve("/path/to/optalcp");
```

## Quick Start

```csharp
using OptalCP;

var model = new Model();

// Create two tasks
var task1 = model.IntervalVar(length: 10, name: "Task1");
var task2 = model.IntervalVar(length: 20, name: "Task2");

// Task2 must start after Task1 ends (constraints auto-register)
(task1.End() <= task2.Start()).Enforce();

// Minimize completion time
model.Minimize(task2.End());

// Solve
var result = model.Solve();
Console.WriteLine($"Objective: {result.Objective}");
```

## Examples

### 1. Basic Modeling

Create interval variables, add constraints, and minimize an objective:

```csharp
using OptalCP;

var model = new Model();

// Create tasks (optional and required)
var x = model.IntervalVar(length: 10, name: "x", optional: true);
var y = model.IntervalVar(length: 20, name: "y");

// Add constraints (use .Enforce() to add boolean expressions)
(x.Start() >= 0).Enforce();
(y.End() <= 100).Enforce();

// Precedence: x ends before y starts
(x.End() <= y.Start()).Enforce();

// Minimize completion time
model.Minimize(y.End());

var result = model.Solve();
Console.WriteLine($"Objective: {result.Objective}");

if (result.Solution != null)
{
    // For optional intervals, check if present
    var xStart = result.Solution.GetStart(x);
    Console.WriteLine($"x starts at: {xStart}");
    Console.WriteLine(result.Solution.IsPresent(x) ? $"x is Present" : "x is Absent");
}
```

### 2. NoOverlap Constraint

Ensure tasks don't overlap in time (e.g., on the same machine):

```csharp
using OptalCP;

var model = new Model(name: "NoOverlapExample");

// Create tasks
var tasks = new List<IntervalVar>
{
    model.IntervalVar(length: 10, name: "Task1"),
    model.IntervalVar(length: 20, name: "Task2"),
    model.IntervalVar(length: 15, name: "Task3")
};

// Transition times between tasks (setup times)
var transitions = new int[][]
{
    new[] { 0, 5, 10 },
    new[] { 5, 0, 5 },
    new[] { 10, 5, 0 }
};

// Tasks cannot overlap, with transition times (constraints auto-register)
model.NoOverlap(tasks, transitions);

// Minimize makespan
var makespan = model.IntVar(min: 0, max: 100, name: "Makespan");
foreach (var task in tasks)
    (task.End() <= makespan).Enforce();
model.Minimize(makespan);

var result = model.Solve(parameters: new Parameters { logLevel = 0 });
Console.WriteLine($"Found solution: {result.NbSolutions > 0}");
Console.WriteLine($"Duration: {result.Duration:F3}s");

if (result.NbSolutions > 0)
{
    foreach (var task in tasks)
    {
        var start = result.Solution!.GetStart(task);
        var end = result.Solution!.GetEnd(task);
        Console.WriteLine($"{task.Name}: Start={start}, End={end}");
    }
    Console.WriteLine($"\nObjective (Makespan): {result.Solution!.GetValue(makespan)}");
}
```

### 3. Cumulative Constraints

Model resource capacity constraints:

```csharp
using OptalCP;

var model = new Model(name: "CumulativeExample");

var task1 = model.IntervalVar(length: 10, name: "task1");
var task2 = model.IntervalVar(length: 15, name: "task2");
var task3 = model.IntervalVar(length: 20, name: "task3", optional: true);

// Variable resource usage for task1
var usage1 = model.IntVar(min: 3, max: 5, name: "usage1");

// Create cumulative expression
var cumul = model.Sum(new[]
{
    model.Pulse(task1, usage1),
    model.Pulse(task2, model.IntVar(min: 2, max: 2)),  // Fixed usage
    model.Pulse(task3, model.IntVar(min: 4, max: 4))
});

// Total resource usage must not exceed capacity of 8
// the constraint is registered just by var _ = cumul <= 8 but we are calling Enforce() for clarity
model.Enforce(cumul <= 8);

var result = model.Solve(parameters: new Parameters { solutionLimit = 2, logLevel = 0 });
Console.WriteLine($"Found {result.NbSolutions} solutions.");
```

### 4. Integer Variables and Expressions

Work with integer variables and arithmetic expressions:

```csharp
using OptalCP;

var model = new Model(name: "IntegerExample");

var x = model.IntVar(min: 0, max: 10, name: "x");
var y = model.IntVar(min: 0, max: 10, name: "y");
var z = model.IntVar(min: 0, max: 20, name: "z");

// z = x + y (using operator overloading)
(z == x + y).Enforce();

// x != y
(x != y).Enforce();

// Maximize z
model.Maximize(z);

var result = model.Solve();
Console.WriteLine($"Objective: {result.Objective}");
```

### 5. Step Functions (Calendars)

Model time-based availability and costs:

```csharp
using OptalCP;

var model = new Model(name: "StepFunctionExample");

// Create a calendar: 0=forbidden (weekend), 1=allowed (weekday)
var calendar = model.StepFunction(new[]
{
    (0, 1),    // Allowed from Monday (day 0)
    (5, 0),    // Forbidden from Saturday (day 5)
    (7, 1),    // Allowed from next Monday (day 7)
    (12, 0)    // Forbidden from next Saturday (day 12)
});

// Create a task that can only start on weekdays
var task = model.IntervalVar(length: 3, name: "task", start: (5, 20));

// Task cannot start during forbidden times (constraints auto-register)
model.ForbidStart(task, calendar);
model.Minimize(task.End());

var result = model.Solve(parameters: new Parameters { logLevel = 0 });
Console.WriteLine($"Objective: {result.Objective}");

if (result.Solution != null)
{
    // The first feasible start is Monday (day 7)
    Console.WriteLine($"Task starts at: {result.Solution.GetStart(task)}");
}
```

### 6. Boolean Variables and Logic

Model logical constraints:

```csharp
using OptalCP;

var model = new Model(name: "BooleanExample");

var a = model.BoolVar(name: "a");
var b = model.BoolVar(name: "b");
var c = model.BoolVar(name: "c");

// At least one must be true
(a | b | c).Enforce();

// Implication: if a then b
a.Implies(b).Enforce();

// XOR: exactly one of b or c
((b & !c) | (!b & c)).Enforce();

// Maximize the number of true variables
model.Maximize(a + b + c);

var result = model.Solve();
Console.WriteLine($"Objective: {result.Objective}");
```

### 7. Alternative Constraint

Model a task that can run on one of several machines:

```csharp
using OptalCP;

var model = new Model(name: "AlternativeExample");

// Main task that must be performed
var mainTask = model.IntervalVar(length: 5, name: "MainTask");

// Alternative executions on different machines
var onMachine1 = model.IntervalVar(length: 5, optional: true, name: "OnMachine1");
var onMachine2 = model.IntervalVar(length: 3, optional: true, name: "OnMachine2");
var onMachine3 = model.IntervalVar(length: 7, optional: true, name: "OnMachine3");

// Main task runs on exactly one machine (constraints auto-register)
model.Alternative(mainTask, new[] { onMachine1, onMachine2, onMachine3 });

// Minimize end time
model.Minimize(mainTask.End());

var result = model.Solve();
Console.WriteLine($"Optimal end time: {result.Objective}");
Console.WriteLine($"onMachine1: {result.Solution!.IsPresent(onMachine1)}");
Console.WriteLine($"onMachine2: {result.Solution!.IsPresent(onMachine2)}");
Console.WriteLine($"onMachine3: {result.Solution!.IsPresent(onMachine3)}");
```

### 8. Solver with Parameters

Configure solver behavior:

```csharp
using OptalCP;

var model = new Model();
var x = model.IntervalVar(length: 10, name: "x");
model.Minimize(x.Start());

var parameters = new Parameters
{
    timeLimit = 60.0,
    solutionLimit = 10,
    logLevel = 1,
    nbWorkers = 4,
    searchType = "LNS"
};

var result = model.Solve(parameters: parameters);
Console.WriteLine($"Found {result.NbSolutions} solutions");
```

> **⚠ Note on naming:** `Parameters`, `WorkerParameters`, and `BenchmarkParameters` use **camelCase** property names (`timeLimit`, `nbWorkers`, `searchType`) — the only place in this library that deviates from standard C# PascalCase. This is intentional: those classes are serialized directly to the solver's JSON handshake, and camelCase matches the wire format shared with the Python and TypeScript APIs. Result types (`SolveResult`, `SolutionEvent`, `ObjectiveEntry`, `ObjectiveBoundEntry`) use standard PascalCase (`result.NbSolutions`, `evt.SolveTime`). Stylecop / `.editorconfig` may flag the `Parameters` classes — you can safely suppress that warning.

### 10. Job Shop Scheduling

Classic job shop problem:

```csharp
using OptalCP;

var model = new Model("JobShop");

int nJobs = 3;
int nMachines = 3;

// Processing times [job, operation]
var durations = new int[,] {
    { 3, 2, 2 },  // Job 0: operations on machines 0, 1, 2
    { 2, 1, 4 },  // Job 1
    { 4, 3, 3 }   // Job 2
};

// Machine assignments [job, operation]
var machines = new int[,] {
    { 0, 1, 2 },
    { 0, 2, 1 },
    { 1, 2, 0 }
};

// Create interval variables
var tasks = new IntervalVar[nJobs, nMachines];
for (int j = 0; j < nJobs; j++)
    for (int o = 0; o < nMachines; o++)
        tasks[j, o] = model.IntervalVar(length: durations[j, o], name: $"J{j}O{o}");

// Precedence within each job
for (int j = 0; j < nJobs; j++)
    for (int o = 0; o < nMachines - 1; o++)
        (tasks[j, o].End() <= tasks[j, o + 1].Start()).Enforce();

// No overlap on each machine (constraints auto-register)
for (int m = 0; m < nMachines; m++)
{
    var machineOps = new List<IntervalVar>();
    for (int j = 0; j < nJobs; j++)
        for (int o = 0; o < nMachines; o++)
            if (machines[j, o] == m)
                machineOps.Add(tasks[j, o]);
    model.NoOverlap(machineOps);
}

// Minimize makespan
var ends = new List<IntExpr>();
for (int j = 0; j < nJobs; j++)
    ends.Add(tasks[j, nMachines - 1].End());
var makespan = model.IntVar(min: 0, max: 1000, name: "makespan");
foreach (var end in ends)
    (end <= makespan).Enforce();
model.Minimize(makespan);

var result = model.Solve(parameters: new Parameters { logLevel = 1 });

if (result.NbSolutions > 0)
{
    Console.WriteLine($"\nMakespan: {result.Objective}");
    for (int j = 0; j < nJobs; j++)
    {
        Console.Write($"Job {j}: ");
        for (int o = 0; o < nMachines; o++)
        {
            var (start, end) = result.Solution!.GetValue(tasks[j, o])!.Value;
            Console.Write($"[{start}-{end}] ");
        }
        Console.WriteLine();
    }
}
```

### 11. Warm Start

Provide initial solution to speed up solving:

```csharp
using OptalCP;

var model = new Model();
var task1 = model.IntervalVar(length: 10, name: "task1");
var task2 = model.IntervalVar(length: 20, name: "task2");
(task1.End() <= task2.Start()).Enforce();
model.Minimize(task2.End());

// Create warm start solution
var warmStart = new Solution();
warmStart.SetValue(task1, start: 0, end: 10);
warmStart.SetValue(task2, start: 10, end: 30);

var result = model.Solve(warmStart: warmStart);
Console.WriteLine($"Objective: {result.Objective}");
```

### 12. Model Serialization

Save and load models:

```csharp
using OptalCP;

// Create and serialize
var model = new Model("SerializationExample");
var x = model.IntervalVar(length: 10, name: "x");
model.Minimize(x.End());

// Serialize model with optional parameters
var parameters = new Parameters { timeLimit = 60 };
string json = model.ToJson(parameters);
File.WriteAllText("problem.json", json);

// Later: load and solve
string loaded = File.ReadAllText("problem.json");
var (loadedModel, loadedParams, _) = Model.FromJson(loaded);

var result = loadedModel.Solve(parameters: loadedParams);
Console.WriteLine($"Objective: {result.Objective}");
```

---

## Other Topics

### Constants

```csharp
Model.IntVarMax      // 1073741823 - max integer variable value
Model.IntVarMin      // -1073741823
Model.IntervalMax    // 715827882 - max interval time
Model.IntervalMin    // -715827882
Model.LengthMax      // max interval length
```

### IntBound for Flexible Bounds

```csharp
// Single value (fixed)
model.IntervalVar(start: 5);                    // start = 5

// Tuple (range)
model.IntervalVar(start: (0, 100));            // 0 <= start <= 100

// Nullable tuple (partial bounds)
model.IntervalVar(start: (null, 100));         // start <= 100, no min
model.IntervalVar(length: (5, null));          // length >= 5, no max
```

## Resources

- 📚 **Full Python API Reference**: [OptalCP Python API Documentation](http://dev.vilim.eu/python-api/index.html)
- 📚 **Full Javascript API Reference**: [OptalCP JavaScript API Documentation](https://optalcp.com/docs/api/)
- 🌐 **Website**: [optalcp.com](https://optalcp.com)

---
