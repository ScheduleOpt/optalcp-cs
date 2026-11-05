// ===========================================================================
// OptalCP C# API - Model Class
// Version: 2026.11.0
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OptalCP
{
    /// <summary>
    /// Central class for building optimization models. Creates variables, constraints, and objectives.
    /// </summary>
    /// <remarks>
    /// _Model_ captures the problem to be solved. It contains variables,
    /// constraints and objective function.
    ///
    /// To create an optimization model, you must first create a _Model_ object.
    /// Then you can use the methods of the _Model_ to create variables (e.g. <see cref="Model.IntervalVar">Model.IntervalVar</see>), the objective function (<see cref="Model.Minimize">Model.Minimize</see> or <see cref="Model.Maximize">Model.Maximize</see>)
    /// and constraints (e.g. <see cref="Model.NoOverlap">Model.NoOverlap</see>).
    /// Note that a boolean expression becomes a constraint only by passing it to
    /// <see cref="Model.Enforce">Model.Enforce</see>; otherwise, it is not enforced.
    ///
    /// To solve a model, pass it to function <see cref="Model.Solve">Model.Solve</see> or to <see cref="Solver">Solver</see>
    /// class.
    ///
    /// <b>Available modeling elements</b>
    ///
    /// <b>Variables</b>
    ///
    /// Interval variables can be created by function <see cref="Model.IntervalVar">Model.IntervalVar</see>, integer variables by function <see cref="Model.IntVar">Model.IntVar</see>.
    ///
    /// <b>Basic integer expressions</b>
    ///
    /// * <see cref="Model.Start">Model.Start</see>: start of an interval variable (optional integer expression).
    /// * <see cref="Model.End">Model.End</see>:   end of an interval variable (optional integer expression).
    /// * <see cref="Model.Length">Model.Length</see>: length of an interval variable (optional integer expression).
    /// * <see cref="Model.Guard">Model.Guard</see>: replaces *absent* value by a constant.
    ///
    /// <b>Integer arithmetics</b>
    ///
    /// Use standard arithmetic operators on integer expressions: <c>+</c>, <c>-</c>, unary <c>-</c>, <c>*</c>, <c>/</c>.
    ///
    /// * <see cref="Model.Abs">Model.Abs</see>:   absolute value.
    /// * <see cref="Model.Min2">Model.Min2</see>:  minimum of two integer expressions.
    /// * <see cref="Model.Min">Model.Min</see>:   minimum of an array of integer expressions.
    /// * <see cref="Model.Max2">Model.Max2</see>:  maximum of two integer expressions.
    /// * <see cref="Model.Max">Model.Max</see>:   maximum of an array of integer expressions.
    /// * <see cref="Model.Sum">Model.Sum</see>:   sum of an array of integer expressions.
    ///
    /// <b>Comparison operators for integer expressions</b>
    ///
    /// Use standard comparison operators on integer expressions: <c>&lt;</c>, <c>&lt;=</c>, <c>==</c>, <c>!=</c>, <c>&gt;</c>, <c>&gt;=</c>.
    ///
    /// * <see cref="Model.Identity">Model.Identity</see>: constraints two integer expressions to be equal, including the presence status.
    ///
    /// <b>Boolean operators</b>
    ///
    /// * <see cref="Model.Not">Model.Not</see>: negation.
    /// * <see cref="Model.And">Model.And</see>: conjunction.
    /// * <see cref="Model.Or">Model.Or</see>:  disjunction.
    /// * <see cref="Model.Implies">Model.Implies</see>: implication.
    ///
    /// <b>Functions returning <see cref="BoolExpr">BoolExpr</see></b>
    ///
    /// * <see cref="Model.Presence">Model.Presence</see>: whether the argument is *present* or *absent*.
    /// * <see cref="Model.InRange">Model.InRange</see>: whether an integer expression is within the given range
    ///
    /// <b>Basic constraints on interval variables</b>
    ///
    /// * <see cref="Model.Alternative">Model.Alternative</see>: an alternative between multiple interval variables.
    /// * <see cref="Model.Span">Model.Span</see>: span (cover) of interval variables.
    /// * <see cref="Model.EndBeforeEnd">Model.EndBeforeEnd</see>, <see cref="Model.EndBeforeStart">Model.EndBeforeStart</see>, <see cref="Model.StartBeforeEnd">Model.StartBeforeEnd</see>, <see cref="Model.StartBeforeStart">Model.StartBeforeStart</see>,
    ///   <see cref="Model.EndAtStart">Model.EndAtStart</see>, <see cref="Model.StartAtEnd">Model.StartAtEnd</see>: precedence constraints.
    ///
    /// <b>Disjunction (noOverlap)</b>
    ///
    /// * <see cref="Model.SequenceVar">Model.SequenceVar</see>: sequence variable over a set of interval variables.
    /// * <see cref="Model.NoOverlap">Model.NoOverlap</see>: constraints a set of interval variables to not overlap (possibly with transition times).
    /// * <see cref="Model.Position">Model.Position</see>: returns the position of an interval variable in a sequence.
    ///
    /// <b>Basic cumulative expressions</b>
    ///
    /// * <see cref="Model.Pulse">Model.Pulse</see>: changes value during the interval variable.
    /// * <see cref="Model.StepAtStart">Model.StepAtStart</see>: changes value at the start of the interval variable.
    /// * <see cref="Model.StepAtEnd">Model.StepAtEnd</see>: changes value at the end of the interval variable.
    /// * <see cref="Model.StepAt">Model.StepAt</see>: changes value at a given time.
    ///
    /// <b>Combining cumulative expressions</b>
    ///
    /// Use standard operators on cumulative expressions: <c>+</c>, <c>-</c>, unary <c>-</c>, and <see cref="Model.Sum">Model.Sum</see>.
    ///
    /// <b>Constraints on cumulative expressions</b>
    ///
    /// Use comparison operators on cumulative expressions: <c>&lt;=</c>, <c>&gt;=</c>.
    ///
    /// <b>Objective</b>
    ///
    /// * <see cref="Model.Minimize">Model.Minimize</see>: minimize an integer expression.
    /// * <see cref="Model.Maximize">Model.Maximize</see>: maximize an integer expression.
    ///
    /// <para>Example:</para>
    /// <example>
    /// Our goal is to schedule a set of tasks such that it is finished as soon as
    /// possible (i.e., the makespan is minimized).  Each task has a fixed duration, and
    /// cannot be interrupted.  Moreover, each task needs a certain number of
    /// workers to be executed, and the total number of workers is limited.
    /// The input data are generated randomly.
    ///
    /// <code>
    /// using System;
    /// using System.Linq;
    /// using OptalCP;
    ///
    /// // Constants for random problem generation:
    /// int nbTasks = 100;
    /// int nbWorkers = 5;
    /// int maxDuration = 100;
    /// var rng = new Random();
    ///
    /// // Start by creating the model:
    /// var model = new Model();
    ///
    /// // For each task we will have an interval variable and a cumulative expression:
    /// var tasks = new List&lt;IntervalVar&gt;();
    /// var workerUsage = new List&lt;CumulExpr&gt;();
    ///
    /// // Loop over the tasks:
    /// for (int i = 0; i &lt; nbTasks; i++) {
    ///     int taskLength = 1 + rng.Next(maxDuration - 1);
    ///     var task = model.IntervalVar(name: $"Task{i + 1}", length: taskLength);
    ///     tasks.Add(task);
    ///     int workersNeeded = 1 + rng.Next(nbWorkers - 1);
    ///     workerUsage.Add(task.Pulse(workersNeeded));
    /// }
    ///
    /// // Limit the sum of the pulses to the number of workers available:
    /// model.Enforce(model.Sum(workerUsage) &lt;= nbWorkers);
    /// // Minimize the maximum of the ends (makespan):
    /// model.Minimize(model.Max(tasks.Select(t =&gt; t.End())));
    ///
    /// // Solve the model:
    /// var result = model.Solve(new Parameters {
    ///     timeLimit = 3,
    ///     nbWorkers = 4,
    /// });
    ///
    /// if (result.nbSolutions == 0)
    ///     Console.WriteLine("No solution found.");
    /// else {
    ///     var solution = result.solution!;
    ///     Console.WriteLine($"Solution found with makespan {solution.GetObjective()}");
    ///     foreach (var task in tasks) {
    ///         var start = solution.GetStart(task);
    ///         if (start != null)
    ///             Console.WriteLine($"Task {task.Name} starts at {start}");
    ///         else
    ///             Console.WriteLine($"Task {task.Name} is absent (not scheduled).");
    ///     }
    /// }
    /// </code>
    ///
    /// </example>
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="Model.Solve">Model.Solve</see></description></item>
    /// <item><description><see cref="Solution">Solution</see></description></item>
    /// <item><description><see cref="Solver">Solver</see></description></item>
    /// </list>
    /// </remarks>
    public class Model
    {
        /// <remarks>
        /// Maximum value of a decision variable or a decision expression (such as <c>x+1</c> where <c>x</c> is a variable).
        ///
        /// Arithmetic expressions must stay within the range [<see cref="Model.IntVarMin">Model.IntVarMin</see>, <see cref="Model.IntVarMax">Model.IntVarMax</see>]. If an expression exceeds this range for all possible variable assignments, the model becomes infeasible.
        ///
        /// <para>Example:</para>
        /// <example>
        /// The following model is infeasible because <c>x*x</c> exceeds <c>IntVarMax</c> for all values in the variable's domain:
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntVar(min: 10000000, max: 20000000);
        /// // Constraint x*x &gt;= 1:
        /// model.Enforce(x * x &gt;= 1);
        /// var result = model.Solve();
        /// </code>
        ///
        /// For any value of <c>x</c> in the range [10000000, 20000000], the expression <c>x*x</c> exceeds <see cref="Model.IntVarMax">Model.IntVarMax</see> and cannot be computed, making the model infeasible.
        /// </example>
        /// </remarks>
        public const int IntVarMax = 1073741823;

        /// <remarks>
        /// Minimum value of a decision variable or a decision expression (such as <c>x+1</c> where <c>x</c> is a variable).
        /// The opposite of <see cref="Model.IntVarMax">Model.IntVarMax</see>.
        ///
        /// Use <c>IntVarMin</c> when you need to allow the full range of negative values for an integer variable.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// // IntVarMin is the minimum allowed value for integer variables
        /// var x = model.IntVar(min: Model.IntVarMin, max: 0, name: "x");
        /// </code>
        /// </example>
        /// </remarks>
        public const int IntVarMin = -IntVarMax;

        /// <remarks>
        /// Maximum value of start or end of an interval variable. The opposite of <see cref="Model.IntervalMin">Model.IntervalMin</see>.
        ///
        /// Use <c>IntervalMax</c> when you want to leave the end time of an interval variable unconstrained.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// // IntervalMax is the maximum allowed value for interval start/end
        /// var task = model.IntervalVar(end: (0, Model.IntervalMax), length: 10, name: "task");
        /// </code>
        /// </example>
        /// </remarks>
        public const int IntervalMax = 715827882;
        
        /// <remarks>
        /// Minimum value of start or end of an interval variable. The opposite of <see cref="Model.IntervalMax">Model.IntervalMax</see>.
        ///
        /// Use <c>IntervalMin</c> when you want to leave the start time of an interval variable unconstrained.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// // IntervalMin is the minimum allowed value for interval start/end
        /// var task = model.IntervalVar(start: (Model.IntervalMin, 0), length: 10, name: "task");
        /// </code>
        /// </example>
        /// </remarks>    
        public const int IntervalMin = -IntervalMax;

        /// <remarks>
        /// Maximum length of an interval variable. The maximum length can be achieved by
        /// an interval that starts at <see cref="Model.IntervalMin">Model.IntervalMin</see> and ends at <see cref="Model.IntervalMax">Model.IntervalMax</see>.
        ///
        /// Use <c>LengthMax</c> when you want to leave the length of an interval variable unconstrained.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// // LengthMax is the maximum allowed length for an interval variable
        /// var task = model.IntervalVar(length: (0, Model.LengthMax), name: "task");
        /// </code>
        /// </example>
        /// </remarks>
        public const int LengthMax = IntervalMax - IntervalMin;

        private string? _name;
        private readonly List<object> _model = new();
        private readonly List<Dictionary<string, object?>> _refs = new();
        private Dictionary<string, object?>? _objective;
        private readonly List<IntervalVar> _intervalVars = new();
        private readonly List<IntVar> _intVars = new();
        private readonly List<BoolVar> _boolVars = new();

        /// <summary>
        /// Creates an empty optimization model.
        /// </summary>
        /// <param name="name">Optional name for the model</param>
        /// <remarks>
        /// Creates an empty model with no variables, constraints, or objective.
        ///
        /// The optional <c>name</c> parameter can be used to identify the model in logs,
        /// debugging output, and benchmarking reports. When not specified, the model
        /// remains unnamed.
        ///
        /// After creating a model, use its methods to define:
        ///
        /// - Variables: <see cref="Model.IntervalVar">Model.IntervalVar</see>, <see cref="Model.IntVar">Model.IntVar</see>, <see cref="Model.BoolVar">Model.BoolVar</see>
        /// - Constraints: <see cref="Model.NoOverlap">Model.NoOverlap</see>, <see cref="Model.EndBeforeStart">Model.EndBeforeStart</see>, <see cref="Model.Enforce">Model.Enforce</see>, etc.
        /// - Objective: <see cref="Model.Minimize">Model.Minimize</see> or <see cref="Model.Maximize">Model.Maximize</see>
        ///
        /// <code>
        /// // Create an unnamed model
        /// var model = new Model();
        ///
        /// // Create a named model (useful for debugging)
        /// var namedModel = new Model(name: "JobShop");
        ///
        /// // Add variables and constraints
        /// var task = model.IntervalVar(length: 10, name: "task");
        /// model.Minimize(task.End());
        ///
        /// // Solve
        /// var result = model.Solve();
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model">Model</see> — for available modeling methods.</description></item>
        /// <item><description><see cref="Model.Solve">Model.Solve</see> — to solve the model.</description></item>
        /// </list>
        /// </remarks>
        public Model(string? name = null)
        {
            _name = name;
        }

        /// <summary>
        /// The name of the model.
        /// </summary>
        /// <remarks>
        /// The name is optional and primarily useful for distinguishing between different
        /// models during debugging and benchmarking. When set, it helps identify the model
        /// in logs and benchmark results.
        ///
        /// The name can be set either in the constructor or by assigning to this property.
        /// Assigning overwrites any name that was previously set.
        ///
        /// <code>
        /// // Set name in constructor
        /// var model = new Model(name: "MySchedulingProblem");
        /// Console.WriteLine(model.Name);  // "MySchedulingProblem"
        ///
        /// // Or set name later
        /// model = new Model();
        /// model.Name = "JobShop";
        /// Console.WriteLine(model.Name);  // "JobShop"
        ///
        /// // Clear the name
        /// model.Name = null;
        /// </code>
        /// </remarks>
        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Returns a list of all interval variables in the model.
        /// </summary>
        /// <returns>A list of all interval variables in the model</returns>
        /// <remarks>
        /// Returns a copy of the list containing all interval variables that have been
        /// created in this model using <see cref="Model.IntervalVar">Model.IntervalVar</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task1 = model.IntervalVar(length: 10, name: "task1");
        /// var task2 = model.IntervalVar(length: 20, name: "task2");
        ///
        /// var intervals = model.GetIntervalVars();
        /// Console.WriteLine(intervals.Count);  // 2
        /// foreach (var iv in intervals)
        ///     Console.WriteLine(iv.Name);  // "task1", "task2"
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.GetIntVars">Model.GetIntVars</see></description></item>
        /// <item><description><see cref="Model.GetBoolVars">Model.GetBoolVars</see></description></item>
        /// </list>
        /// </remarks>
        public IReadOnlyList<IntervalVar> GetIntervalVars() => _intervalVars;

        /// <summary>
        /// Returns a list of all integer variables in the model.
        /// </summary>
        /// <returns>A list of all integer variables in the model</returns>
        /// <remarks>
        /// Returns a copy of the list containing all integer variables that have been
        /// created in this model using <see cref="Model.IntVar">Model.IntVar</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.IntVar(min: 0, max: 10, name: "x");
        /// var y = model.IntVar(min: 0, max: 100, name: "y");
        ///
        /// var intVars = model.GetIntVars();
        /// Console.WriteLine(intVars.Count);  // 2
        /// foreach (var iv in intVars)
        ///     Console.WriteLine(iv.Name);  // "x", "y"
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.GetIntervalVars">Model.GetIntervalVars</see></description></item>
        /// <item><description><see cref="Model.GetBoolVars">Model.GetBoolVars</see></description></item>
        /// </list>
        /// </remarks>
        public IReadOnlyList<IntVar> GetIntVars() => _intVars;

        /// <summary>
        /// Returns a list of all boolean variables in the model.
        /// </summary>
        /// <returns>A list of all boolean variables in the model</returns>
        /// <remarks>
        /// Returns a copy of the list containing all boolean variables that have been
        /// created in this model using <see cref="Model.BoolVar">Model.BoolVar</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var useMachineA = model.BoolVar(name: "use_machine_a");
        /// var useMachineB = model.BoolVar(name: "use_machine_b");
        ///
        /// var boolVars = model.GetBoolVars();
        /// Console.WriteLine(boolVars.Count);  // 2
        /// foreach (var bv in boolVars)
        ///     Console.WriteLine(bv.Name);  // "use_machine_a", "use_machine_b"
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.GetIntervalVars">Model.GetIntervalVars</see></description></item>
        /// <item><description><see cref="Model.GetIntVars">Model.GetIntVars</see></description></item>
        /// </list>
        /// </remarks>
        public IReadOnlyList<BoolVar> GetBoolVars() => _boolVars;

        /// <summary>
        /// Creates a new interval variable and adds it to the model.
        /// </summary>
        /// <param name="start">Fixed start time or range [min, max]. Pass an int for a fixed value, a tuple (min, max) for a range, or null for the default range [0, Model.IntervalMax].</param>
        /// <param name="end">Fixed end time or range [min, max]. Pass an int for a fixed value, a tuple (min, max) for a range, or null for the default range [Model.IntervalMin, Model.IntervalMax].</param>
        /// <param name="length">Fixed length or range [min, max]. Pass an int for a fixed value, a tuple (min, max) for a range, or null for the default range [0, Model.LengthMax].</param>
        /// <param name="optional">If true, the interval can be absent in a solution. The default is false.</param>
        /// <param name="name">Optional name for debugging purposes.</param>
        /// <returns>The created interval variable.</returns>
        /// <remarks>
        /// An interval variable represents an unknown interval (a task, operation,
        /// action) that the solver assigns a value in such a way as to satisfy all
        /// constraints. An interval variable has a start, end, and length. In a
        /// solution, *start &lt;= end* and *length = end - start*.
        ///
        /// The interval variable can be optional. In this case, its value in a solution
        /// could be *absent*, meaning that the task/operation is not performed.
        ///
        /// Parameters <c>start</c>, <c>end</c>, and <c>length</c> accept an <c>IntBound</c>, which can be:
        ///
        /// - An <see langword="int"/> for a fixed value
        /// - A tuple <c>(int min, int max)</c> for a range (use <see langword="null"/> for either bound to keep the default)
        ///
        /// The default ranges are <c>0</c> to <see cref="Model.IntervalMax">Model.IntervalMax</see> for start, <see cref="Model.IntervalMin">Model.IntervalMin</see>
        /// to <see cref="Model.IntervalMax">Model.IntervalMax</see> for end, and <c>0</c> to <see cref="Model.LengthMax">Model.LengthMax</see> for length. So only
        /// the default start keeps the interval at non-negative times. When you give a
        /// negative start, the end can follow it without a range of its own.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        ///
        /// // Create an interval variable with a fixed start but unknown length:
        /// var x = model.IntervalVar(start: 0, length: (10, 20), name: "x");
        ///
        /// // Create an interval variable with start and end ranges:
        /// var y = model.IntervalVar(start: (0, 5), end: (10, 15), name: "y");
        ///
        /// // Create an optional interval variable with length range 5..10:
        /// var z = model.IntervalVar(length: (5, 10), optional: true, name: "z");
        ///
        /// // Set only startMax (startMin remains the default 0):
        /// var w = model.IntervalVar(start: (null, 100), name: "w");
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar">IntervalVar</see></description></item>
        /// </list>
        /// </remarks>
        public IntervalVar IntervalVar(
            IntBound? start = null,
            IntBound? end = null,
            IntBound? length = null,
            bool optional = false,
            string? name = null)
        {
            var props = new Dictionary<string, object?>
            {
                ["func"] = "intervalVar",
                ["args"] = new List<object>()
            };

            if (name != null) props["name"] = name;
            if (optional) props["status"] = (int)PresenceStatus.Optional;

            if (start.HasValue)
            {
                if (start.Value.Min.HasValue) props["startMin"] = start.Value.Min.Value;
                if (start.Value.Max.HasValue) props["startMax"] = start.Value.Max.Value;
            }
            if (end.HasValue)
            {
                if (end.Value.Min.HasValue) props["endMin"] = end.Value.Min.Value;
                if (end.Value.Max.HasValue) props["endMax"] = end.Value.Max.Value;
            }
            if (length.HasValue)
            {
                if (length.Value.Min.HasValue) props["lengthMin"] = length.Value.Min.Value;
                if (length.Value.Max.HasValue) props["lengthMax"] = length.Value.Max.Value;
            }

            var v = new IntervalVar(this, props);
            _intervalVars.Add(v);
            return v;
        }

        /// <summary>
        /// Creates a new integer variable and adds it to the model.
        /// </summary>
        /// <param name="min">Minimum value for the variable. Default is 0.</param>
        /// <param name="max">Maximum value for the variable. Default is <see cref="Model.IntVarMax">Model.IntVarMax</see>.</param>
        /// <param name="optional">If true, the variable can be absent in a solution. The default is false.</param>
        /// <param name="name">Optional name for debugging purposes.</param>
        /// <returns>The created integer variable.</returns>
        /// <remarks>
        /// An integer variable represents an unknown value the solver must find.
        /// The variable can be optional.
        /// In this case, its value in a solution could be *absent*, meaning that the solution does not use the variable at all.
        ///
        /// The default domain is <c>0</c> to <see cref="Model.IntVarMax">Model.IntVarMax</see>.
        /// If <c>min</c> or <c>max</c> is not specified (or null), the default value is used.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        ///
        /// // Create an integer variable with possible values 1..10:
        /// var x = model.IntVar(min: 1, max: 10, name: "x");
        ///
        /// // Create an optional integer variable with possible values 5..Model.IntVarMax:
        /// var y = model.IntVar(min: 5, optional: true, name: "y");
        ///
        /// // Create a non-negative integer variable:
        /// var z = model.IntVar(max: 100, name: "z");
        /// </code>
        /// </example>
        /// </remarks>
        public IntVar IntVar(int? min = null, int? max = null, bool optional = false, string? name = null)
        {
            var props = new Dictionary<string, object?>
            {
                ["func"] = "intVar",
                ["args"] = new List<object>()
            };

            if (min.HasValue) props["min"] = min.Value;
            if (max.HasValue) props["max"] = max.Value;
            if (optional) props["status"] = (int)PresenceStatus.Optional;
            if (name != null) props["name"] = name;

            var v = new IntVar(this, props);
            _intVars.Add(v);
            return v;
        }

        /// <summary>
        /// Creates a new boolean variable and adds it to the model.
        /// </summary>
        /// <param name="optional">If true, the variable can be absent in a solution. The default is false.</param>
        /// <param name="name">Optional name for the variable (useful for debugging).</param>
        /// <returns>The created boolean variable.</returns>
        /// <remarks>
        /// A boolean variable represents an unknown truth value (<see langword="true"/> or <see langword="false"/>) that the
        /// solver must find. Boolean variables are useful for modeling decisions, choices,
        /// or logical conditions in your problem.
        ///
        /// By default, a boolean variable must be assigned a value (<see langword="true"/> or <see langword="false"/>) in every
        /// solution. When <c>optional: true</c>, the variable can also be *absent*, meaning the
        /// solution does not use the variable at all. This is useful when the variable
        /// represents a decision that may not apply in all scenarios.
        ///
        /// Boolean variables support logical operators:
        ///
        /// - <c>!x</c> for logical NOT
        /// - <c>x | y</c> for logical OR
        /// - <c>x &amp; y</c> for logical AND
        ///
        /// Note: use <c>&amp;</c> and <c>|</c> (bitwise operators), not <c>&amp;&amp;</c> and <c>||</c> (short-circuit operators),
        /// as C# does not allow overloading <c>&amp;&amp;</c>/<c>||</c>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// Create boolean variables to model decisions:
        ///
        /// <code>
        /// var model = new Model();
        /// var useMachineA = model.BoolVar(name: "useMachineA");
        /// var useMachineB = model.BoolVar(name: "useMachineB");
        ///
        /// // Constraint: must use at least one machine
        /// model.Enforce(useMachineA | useMachineB);
        ///
        /// // Constraint: cannot use both machines
        /// model.Enforce(!(useMachineA &amp; useMachineB));
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.IntervalVar">Model.IntervalVar</see> — for the primary variable type for scheduling problems.</description></item>
        /// <item><description><see cref="Model.IntVar">Model.IntVar</see> — for numeric decisions.</description></item>
        /// </list>
        /// </remarks>
        public BoolVar BoolVar(bool optional = false, string? name = null)
        {
            var props = new Dictionary<string, object?>
            {
                ["func"] = "boolVar",
                ["args"] = new List<object>(),
                ["min"] = false,
                ["max"] = true
            };

            if (name != null) props["name"] = name;
            if (optional) props["status"] = (int)PresenceStatus.Optional;

            var v = new BoolVar(this, props);
            _boolVars.Add(v);
            return v;
        }

        /// <summary>
        /// Creates a sequence variable from the provided set of interval variables.
        /// </summary>
        /// <param name="intervals">Interval variables that will form the sequence in the solution</param>
        /// <param name="types">Types of the intervals, used in particular for transition times</param>
        /// <param name="name">Name assigned to the sequence variable</param>
        /// <returns>The created sequence variable</returns>
        /// <remarks>
        /// Sequence variable is used together with <see cref="SequenceVar.NoOverlap">SequenceVar.NoOverlap</see>
        /// constraint to model a set of intervals that cannot overlap and so they form
        /// a sequence in the solution. Sequence variable allows us to constrain the sequence further.
        /// For example, by specifying sequence-dependent minimum
        /// transition times.
        ///
        /// Types can be used to mark intervals with similar properties. In
        /// particular, they behave similarly in terms of transition times.
        /// Interval variable <c>intervals[0]</c> will have type <c>type[0]</c>, <c>intervals[1]</c>
        /// will have type <c>type[1]</c> and so on.
        ///
        /// If <c>types</c> are not specified then <c>intervals[0]</c> will have type 0,
        /// <c>intervals[1]</c> will have type 1, and so on.
        ///
        /// The length of the array <c>types</c> must be the same as the length of the array
        /// <c>intervals</c>. Types should be integer numbers in the range <c>0</c> to <c>n-1</c> where <c>n</c> is the
        /// number of types.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="SequenceVar.NoOverlap">SequenceVar.NoOverlap</see> — for an example of sequenceVar usage with transition times.</description></item>
        /// <item><description><see cref="SequenceVar">SequenceVar</see></description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see></description></item>
        /// </list>
        /// </remarks>
        public SequenceVar SequenceVar(
            IEnumerable<IntervalVar> intervals,
            IEnumerable<int>? types = null,
            string? name = null)
        {
            var intervalArgs = OptalCP.IntervalVar.WrapList(intervals);
            var outParams = new List<object?> { intervalArgs };

            if (types != null)
            {
                var typesList = types.ToList();
                if (typesList.Count != intervalArgs.Count)
                    throw new ArgumentException($"Length of types ({typesList.Count}) must equal length of intervals ({intervalArgs.Count})");
                outParams.Add(typesList.Cast<object>().ToList());
            }

            var result = new SequenceVar(this, "sequenceVar", outParams);
            if (name != null) result.Name = name;
            _model.Add(result.AsArg());
            return result;
        }

        /// <summary>
        /// Constrain a set of interval variables not to overlap.
        /// </summary>
        /// <param name="intervals">An array of interval variables or a sequence variable to constrain</param>
        /// <param name="transitions">A 2D square array of minimum transition times between the intervals</param>
        /// <returns>The no-overlap constraint.</returns>
        /// <remarks>
        /// This function constrains a set of interval variables so they do not overlap.
        /// That is, for each pair of interval variables <c>x</c> and <c>y</c>, one of the
        /// following must hold:
        ///
        /// 1. Interval variable <c>x</c> or <c>y</c> is *absent*. In this case, the absent interval
        /// is not scheduled (the task is not performed), so it cannot overlap
        /// with any other interval. Only *optional* interval variables can be *absent*.
        /// 2. Interval variable <c>x</c> is before <c>y</c>, that is, <c>x.end()</c> is less than or
        /// equal to <c>y.start()</c>.
        /// 3. The interval variable <c>y</c> is before <c>x</c>. That is, <c>y.end()</c> is less than or
        /// equal to <c>x.start()</c>.
        ///
        /// The function can also take a square array <c>transitions</c> of minimum
        /// transition times between the intervals. The transition time is the time
        /// that must elapse between the end of the first interval and the start of the
        /// second interval. The transition time cannot be negative. When transition
        /// times are specified, the above conditions 2 and 3 are modified as follows:
        ///
        /// 2. <c>x.end() + transitions[i][j]</c> is less than or equal to <c>y.start()</c>.
        /// 3. <c>y.end() + transitions[j][i]</c> is less than or equal to <c>x.start()</c>.
        ///
        /// Where <c>i</c> and <c>j</c> are types of <c>x</c> and <c>y</c>. When an array of intervals is passed,
        /// the type of each interval is its index in the array.
        ///
        /// Note that minimum transition times are enforced between all pairs of
        /// intervals, not only between direct neighbors.
        ///
        /// Instead of an array of interval variables, a <see cref="SequenceVar">SequenceVar</see> can be passed.
        /// See <see cref="Model.SequenceVar">Model.SequenceVar</see> for how to assign types to intervals in a sequence.
        ///
        /// <para>Example:</para>
        /// <example>
        /// The following example does not use transition times. For example with
        /// transition times see <see cref="SequenceVar.NoOverlap">SequenceVar.NoOverlap</see>.
        ///
        /// Let's consider a set of tasks that must be performed by a single machine.
        /// The machine can handle only one task at a time. Each task is
        /// characterized by its length and a deadline. The goal is to schedule the
        /// tasks on the machine so that the number of missed deadlines is minimized.
        ///
        /// <code>
        /// var tasks = new[] {
        ///     new { length = 10, deadline = 70 },
        ///     new { length = 20, deadline = 50 },
        ///     new { length = 15, deadline = 50 },
        ///     new { length = 30, deadline = 100 },
        ///     new { length = 20, deadline = 120 },
        ///     new { length = 25, deadline = 90 },
        ///     new { length = 30, deadline = 80 },
        ///     new { length = 10, deadline = 40 },
        ///     new { length = 20, deadline = 60 },
        ///     new { length = 25, deadline = 150 },
        /// };
        ///
        /// var model = new Model();
        ///
        /// // An interval variable for each task:
        /// var taskVars = new List&lt;IntervalVar&gt;();
        /// // A boolean expression that is true if the task is late:
        /// var isLate = new List&lt;BoolExpr&gt;();
        ///
        /// for (int i = 0; i &lt; tasks.Length; i++) {
        ///     var taskVar = model.IntervalVar(name: $"Task{i}", length: tasks[i].length);
        ///     taskVars.Add(taskVar);
        ///     isLate.Add(taskVar.End() &gt;= tasks[i].deadline);
        /// }
        ///
        /// // Tasks cannot overlap:
        /// model.NoOverlap(taskVars);
        /// // Minimize the number of late tasks:
        /// model.Minimize(model.Sum(isLate));
        ///
        /// var result = model.Solve();
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="SequenceVar.NoOverlap">SequenceVar.NoOverlap</see> — is the equivalent method on <see cref="SequenceVar">SequenceVar</see>.</description></item>
        /// </list>
        /// </remarks>
        public Constraint NoOverlap(
            IEnumerable<IntervalVar> intervals,
            IEnumerable<IEnumerable<int>>? transitions = null)
        {
            var sequence = AuxiliarySequenceVar(intervals);
            return sequence.NoOverlap(transitions);
        }

        /// <inheritdoc cref="NoOverlap(IEnumerable{IntervalVar}, IEnumerable{IEnumerable{int}}?)"/>
        public Constraint NoOverlap(
            SequenceVar sequence,
            IEnumerable<IEnumerable<int>>? transitions = null)
        {
            return sequence.NoOverlap(transitions);
        }

        private SequenceVar AuxiliarySequenceVar(IEnumerable<IntervalVar> intervals)
        {
            var sequence = SequenceVar(intervals);
            sequence.MakeAuxiliary();
            return sequence;
        }

        /// <summary>
        /// Creates a new integer step function.
        /// </summary>
        /// <param name="values">An array of points defining the step function in the form [[x0, y0], [x1, y1], ..., [xn, yn]], where xi and yi are integers. The array must be sorted by xi</param>
        /// <returns>The created step function</returns>
        /// <remarks>
        /// Integer step function is a piecewise constant function defined on integer
        /// values in range <see cref="Model.IntVarMin">Model.IntVarMin</see> to <see cref="Model.IntVarMax">Model.IntVarMax</see>.  The function is
        /// defined as follows:
        ///
        /// * $f(x) = 0$ for $x &lt; x_0$,
        /// * $f(x) = y_i$ for $x_i \leq x &lt; x_{i+1}$
        /// * $f(x) = y_n$ for $x \geq x_n$.
        ///
        /// Step functions can be used in the following ways:
        ///
        /// * Function <see cref="Model.Eval">Model.Eval</see> evaluates the function at the given point (given as <see cref="IntExpr">IntExpr</see>).
        /// * Function <see cref="Model.Integral">Model.Integral</see> computes a sum (integral) of the function over an <see cref="IntervalVar">IntervalVar</see>.
        /// * Constraint <see cref="Model.ForbidStart">Model.ForbidStart</see> forbids the start of an <see cref="IntervalVar">IntervalVar</see> to be in a zero-value interval of the function. Constraint <see cref="Model.ForbidEnd">Model.ForbidEnd</see> does the same for the last time point of the interval (end - 1).
        /// * Constraint <see cref="Model.ForbidExtent">Model.ForbidExtent</see> forbids the extent of an <see cref="IntervalVar">IntervalVar</see> to be in a zero-value interval of the function.
        /// </remarks>
        public IntStepFunction StepFunction(IEnumerable<(int, int)> values)
            => new IntStepFunction(this, values);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal StateFunction StateFunction(IEnumerable<IEnumerable<int>>? transitions = null)
            => new StateFunction(this, transitions);


        /// <summary>
        /// Creates a minimization objective for the provided expression.
        /// </summary>
        /// <param name="expr">The expression to minimize</param>
        /// <returns>An Objective that minimizes the expression.</returns>
        /// <remarks>
        /// Creates an <see cref="Objective">Objective</see> to minimize the given expression.
        /// A model can have at most one objective. New objective replaces the old one.
        ///
        /// The expression can be *absent* (see optional <see cref="IntExpr">IntExpr</see>). An absent
        /// value is worse than any present value. So the solver prefers a solution in
        /// which the expression is present.
        ///
        /// Equivalent of function <see cref="IntExpr.Minimize">IntExpr.Minimize</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following model, we search for a solution that minimizes the maximum
        /// end of the two intervals <c>x</c> and <c>y</c>:
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "x");
        /// var y = model.IntervalVar(length: 20, name: "y");
        /// model.Minimize(model.Max2(x.End(), y.End()));
        /// var result = model.Solve();
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Maximize">Model.Maximize</see></description></item>
        /// <item><description><see cref="IntExpr.Minimize">IntExpr.Minimize</see> — for fluent-style minimization.</description></item>
        /// </list>
        /// </remarks>
        public Objective Minimize(IntExpr expr)
            => new Objective(this, "minimize", new object[] { IntExpr.Wrap(expr) });

        /// <inheritdoc cref="Minimize(IntExpr)"/>
        public Objective Minimize(int value)
            => new Objective(this, "minimize", new object[] { value });

        /// <summary>
        /// Creates a maximization objective for the provided expression.
        /// </summary>
        /// <param name="expr">The expression to maximize</param>
        /// <returns>An Objective that maximizes the expression.</returns>
        /// <remarks>
        /// Creates an <see cref="Objective">Objective</see> to maximize the given expression.
        /// A model can have at most one objective. New objective replaces the old one.
        ///
        /// The expression can be *absent* (see optional <see cref="IntExpr">IntExpr</see>). An absent
        /// value is worse than any present value. So the solver prefers a solution in
        /// which the expression is present.
        ///
        /// Equivalent of function <see cref="IntExpr.Maximize">IntExpr.Maximize</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following model, we search for a solution that maximizes
        /// the length of the interval variable <c>x</c>:
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: (10, 20), name: "x");
        /// model.Maximize(x.Length());
        /// var result = model.Solve();
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Minimize">Model.Minimize</see></description></item>
        /// <item><description><see cref="IntExpr.Maximize">IntExpr.Maximize</see> — for fluent-style maximization.</description></item>
        /// </list>
        /// </remarks>
        public Objective Maximize(IntExpr expr)
            => new Objective(this, "maximize", new object[] { IntExpr.Wrap(expr) });

        /// <inheritdoc cref="Maximize(IntExpr)"/>
        public Objective Maximize(int value)
            => new Objective(this, "maximize", new object[] { value });

        internal void SetObjective(Objective objective)
        {
            _objective = objective.GetProps();
        }

        internal void AddConstraint(Constraint constraint)
        {
            _model.Add(constraint.AsArg());
        }

        internal void AddDirective(Directive directive)
        {
            _model.Add(directive.AsArg());
        }

        internal int GetNewRefId(Dictionary<string, object?> props)
        {
            int refId = _refs.Count;
            _refs.Add(props);
            return refId;
        }

        /// <summary>
        /// Creates an integer expression for the sum of the arguments.
        /// </summary>
        /// <param name="args">Array of integer expressions to sum.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// Absent arguments are ignored (treated as zeros). Therefore, the resulting expression is never *absent*.
        ///
        /// Note that the binary operator <c>+</c> handles absent values differently. For example, when <c>x</c> is *absent* then:
        ///
        /// * <c>x + 3</c> is *absent*.
        /// * <c>model.Sum(new[] { x, 3 })</c> is 3.
        ///
        /// <para>Example:</para>
        /// <example>
        /// Let's consider a set of optional tasks. Due to limited resources and time, only some of them can be executed. Every task has a profit, and we want to maximize the total profit from the executed tasks.
        ///
        /// <code>
        /// // Lengths and profits of the tasks:
        /// int[] lengths = { 10, 20, 15, 30, 20, 25, 30, 10, 20, 25 };
        /// int[] profits = {  5,  6,  7,  8,  9, 10, 11, 12, 13, 14 };
        ///
        /// var model = new Model();
        /// var tasks = new List&lt;IntervalVar&gt;();
        /// // Profits of individual tasks. The value will be zero if the task is not executed.
        /// var taskProfits = new List&lt;IntExpr&gt;();
        ///
        /// for (int i = 0; i &lt; lengths.Length; i++) {
        ///     // All tasks must finish before time 100:
        ///     var task = model.IntervalVar(name: $"Task{i}", optional: true, length: lengths[i], end: (null, 100));
        ///     tasks.Add(task);
        ///     taskProfits.Add(model.Presence(task) * profits[i]);
        /// }
        /// model.Maximize(model.Sum(taskProfits));
        /// // Tasks cannot overlap:
        /// model.NoOverlap(tasks);
        ///
        /// var result = model.Solve(new Parameters { searchType = "FDS" });
        /// </code>
        /// </example>
        /// </remarks>
        public IntExpr Sum(IEnumerable<IntExpr> args)
        {
            var wrapped = args.Select(a => IntExpr.Wrap(a)).ToList();
            if (wrapped.Count == 0) return new IntExpr(this, "intConst", new object[] { 0 });
            return new IntExpr(this, "intSum", new object[] { wrapped });
        }

        /// <inheritdoc cref="Sum(IEnumerable{IntExpr})"/>
        public IntExpr Sum(IEnumerable<object> args)
        {
            var wrapped = IntExpr.WrapMixedList(args);
            if (wrapped.Count == 0) return new IntExpr(this, "intConst", new object[] { 0 });
            return new IntExpr(this, "intSum", new object[] { wrapped });
        }

        /// <summary>
        /// Sum of cumulative expressions.
        /// </summary>
        /// <param name="args">Array of cumulative expressions to sum.</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// Computes the sum of cumulative functions. The sum can be used, e.g., to combine contributions of individual tasks to total resource consumption.
        ///
        /// Limitation: Currently, pulse-based and step-based cumulative expressions cannot be mixed. All expressions in the sum must be either pulse-based or step-based.
        /// </remarks>
        public CumulExpr Sum(IEnumerable<CumulExpr> args)
        {
            var wrapped = args.Select(a => CumulExpr.Wrap(a)).ToList();
            return new CumulExpr(this, "cumulSum", new object[] { wrapped });
        }

        /// <summary>
        /// Creates a boolean expression that is true if the given argument is present in the solution.
        /// </summary>
        /// <param name="arg">The argument to check for presence in the solution</param>
        /// <returns>A boolean expression that is true if the argument is present in the solution.</returns>
        /// <remarks>
        /// The value of the expression remains unknown until a solution is found.
        /// The expression can be used in a constraint to restrict possible solutions.
        ///
        /// The function is equivalent to <see cref="IntervalVar.Presence">IntervalVar.Presence</see>
        /// and <see cref="IntExpr.Presence">IntExpr.Presence</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, interval variables <c>x</c> and <c>y</c> must have the same presence status.
        /// I.e. they must either be both *present* or both *absent*.
        ///
        /// <code>
        /// var model = new Model();
        ///
        /// var x = model.IntervalVar(name: "x", optional: true, length: 10, start: (0, 100));
        /// var y = model.IntervalVar(name: "y", optional: true, length: 10, start: (0, 100));
        /// model.Enforce(model.Presence(x) == model.Presence(y));
        /// </code>
        ///
        /// <b>Simple constraints over presence</b>
        ///
        /// The solver treats binary constraints over presence in a special way: it
        /// uses them to better propagate other constraints over the same pairs of variables.
        /// Let's extend the previous example by a constraint that <c>x</c> must end before
        /// <c>y</c> starts:
        ///
        /// <code>
        /// var x = model.IntervalVar(name: "x", optional: true, length: 10, start: (0, 100));
        /// var y = model.IntervalVar(name: "y", optional: true, length: 10, start: (0, 100));
        /// model.Enforce(model.Presence(x) == model.Presence(y));
        /// // x.end &lt;= y.start:
        /// var precedence = x.End() &lt;= y.Start();
        /// model.Enforce(precedence);
        /// </code>
        ///
        /// In this example, the solver sees (propagates) that the minimum start time of
        /// <c>y</c> is 10 and maximum end time of <c>x</c> is 90.  Without the constraint over
        /// <c>presence</c>, the solver could not propagate that because one
        /// of the intervals can be *absent* and the other one *present* (and so the
        /// value of <c>precedence</c> would be *absent* and the constraint would be
        /// satisfied).
        ///
        /// To achieve good propagation, it is recommended to use binary
        /// constraints over <c>presence</c> when possible. For example, multiple binary
        /// constraints can be used instead of a single complicated constraint.
        /// </example>
        /// </remarks>
        public BoolExpr Presence(IntervalVar arg)
            => new BoolExpr(this, "intervalPresenceOf", new object[] { OptalCP.IntervalVar.Wrap(arg) });

        /// <inheritdoc cref="Presence(IntervalVar)"/>
        public BoolExpr Presence(IntExpr expr)
            => new BoolExpr(this, "intPresenceOf", new object[] { IntExpr.Wrap(expr) });

        /// <inheritdoc cref="Presence(IntervalVar)"/>
        public BoolExpr Presence(int value)
            => new BoolExpr(this, "intPresenceOf", new object[] { value });

        /// <summary>
        /// Enforces a boolean expression as a constraint in the model.
        /// </summary>
        /// <param name="constraint">The constraint, boolean expression, or iterable of these to enforce in the model</param>
        /// <remarks>
        /// This is the primary method for enforcing boolean expressions as constraints in the model.
        ///
        /// A constraint is satisfied if it is not <see langword="false"/>. In other words, a constraint is
        /// satisfied if it is <see langword="true"/> or *absent*.
        ///
        /// A boolean expression that is *not* enforced as a constraint can have
        /// arbitrary value in a solution (<see langword="true"/>, <see langword="false"/>, or *absent*). Once enforced
        /// as a constraint, it can only be <see langword="true"/> or *absent* in the solution.
        ///
        /// Note: Constraint objects are automatically registered when created.
        /// Passing them to <c>enforce()</c> is accepted but does nothing. For cumulative
        /// constraints (<c>cumul &lt;= capacity</c>, <c>cumul &gt;= min_level</c>), using <c>enforce()</c> is
        /// recommended for code clarity even though it's not required.
        ///
        /// <b>Accepted argument types</b>
        ///
        /// The <c>enforce</c> method accepts several types of arguments:
        ///
        /// 1. BoolExpr objects: Boolean expressions created from comparisons
        ///    (e.g., <c>x &lt;= 5</c>, <c>a == b</c>) or logical operations (e.g., <c>a &amp; b</c>, <c>~c</c>).
        /// 2. bool values: Python boolean constants <c>True</c> or <c>False</c>.
        /// 3. Constraint objects: Accepted but does nothing (constraints auto-register).
        ///    Use with cumulative constraints for clarity: <c>model.enforce(cumul &lt;= capacity)</c>.
        /// 4. Iterables: Lists, tuples, or generators of the above types.
        ///
        /// <para>Example:</para>
        /// <example>
        /// Basic usage with a boolean expression:
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "x");
        ///
        /// // Enforce boolean expressions as constraints
        /// model.Enforce(x.Start() &gt;= 0);
        /// model.Enforce(x.End() &lt;= 100);
        /// </code>
        /// </example><para>Example:</para>
        /// <example>
        /// Enforcing multiple constraints at once using an iterable:
        ///
        /// <code>
        /// var model = new Model();
        /// var tasks = new IntervalVar[5];
        /// for (int i = 0; i &lt; 5; i++)
        ///     tasks[i] = model.IntervalVar(length: 10, name: $"task_{i}");
        ///
        /// // Enforce multiple constraints at once
        /// model.Enforce(tasks.Select(task =&gt; task.Start() &gt;= 0));
        ///
        /// // Or inline
        /// model.Enforce(tasks.Select(task =&gt; task.End() &lt;= 100));
        /// </code>
        /// </example><para>Example:</para>
        /// <example>
        /// Enforcing multiple boolean expressions:
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntVar(min: 0, max: 100, name: "x");
        /// var y = model.IntVar(min: 0, max: 100, name: "y");
        ///
        /// // Enforce various boolean expressions
        /// model.Enforce(new BoolExpr[] {
        ///     x + y &lt;= 50,           // From comparison
        ///     x &gt;= 10,               // From comparison
        /// });
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Enforce">BoolExpr.Enforce</see> — for the fluent-style alternative.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for creating no-overlap constraints.</description></item>
        /// <item><description><see cref="Model.Minimize">Model.Minimize</see> — for creating minimization objectives.</description></item>
        /// <item><description><see cref="Model.Maximize">Model.Maximize</see> — for creating maximization objectives.</description></item>
        /// </list>
        /// </remarks>
        public void Enforce(Constraint constraint)
        {
            // No-op: Constraints are auto-registered in constructor
        }

        /// <inheritdoc cref="Enforce(Constraint)"/>
        public void Enforce(BoolExpr expr)
        {
            _model.Add(expr.AsArg());
        }

        /// <inheritdoc cref="Enforce(Constraint)"/>
        public void Enforce(bool value)
        {
            _model.Add(BoolExpr.Wrap(value));
        }

        /// <inheritdoc cref="Enforce(Constraint)"/>
        public void Enforce(IEnumerable<Constraint> constraints)
        {
            // No-op: Constraints are auto-registered in constructor
        }

        /// <inheritdoc cref="Enforce(Constraint)"/>
        public void Enforce(IEnumerable<BoolExpr> expressions)
        {
            foreach (var expr in expressions)
                _model.Add(expr.AsArg());
        }

        internal Dictionary<string, object?> ToDict()
        {
            var result = new Dictionary<string, object?>
            {
                ["refs"] = _refs,
                ["model"] = _model
            };

            if (!string.IsNullOrEmpty(_name)) result["name"] = _name;
            if (_objective != null) result["objective"] = _objective;

            return result;
        }

        /// <summary>
        /// Exports the model to JSON format.
        /// </summary>
        /// <param name="parameters">Optional solver parameters to include</param>
        /// <param name="warmStart">Optional initial solution to include</param>
        /// <returns>A string containing the model in JSON format.</returns>
        /// <remarks>
        /// The result can be stored in a file for later use. The model can be
        /// converted back from JSON format using <see cref="Model.FromJSON">Model.FromJSON</see>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "task_x");
        /// var y = model.IntervalVar(length: 20, name: "task_y");
        /// x.EndBeforeStart(y);
        /// model.Minimize(y.End());
        ///
        /// // Export to JSON
        /// var jsonStr = model.ToJSON();
        ///
        /// // Save to file
        /// File.WriteAllText("model.json", jsonStr);
        ///
        /// // Later, load from JSON
        /// var (model2, params2, warmStart2) = Model.FromJSON(jsonStr);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.FromJSON">Model.FromJSON</see> — to import from JSON.</description></item>
        /// <item><description><see cref="Model.ToText">Model.ToText</see> — to export as text format.</description></item>
        /// <item><description><see cref="Model.ToJS">Model.ToJS</see> — to export as JavaScript code.</description></item>
        /// </list>
        /// </remarks>
        public string ToJSON(Parameters? parameters = null, Solution? warmStart = null)
        {
            var msg = ToDict();

            if (parameters != null)
                msg["parameters"] = parameters.ToDict();

            if (warmStart != null)
                msg["warmStart"] = warmStart.ToDict();

            return JsonSerializer.Serialize(msg);
        }

        /// <summary>
        /// Creates a model from JSON format.
        /// </summary>
        /// <param name="jsonStr">A string containing the model in JSON format</param>
        /// <returns>A tuple containing the model, optional parameters, and optional warm start solution.</returns>
        /// <remarks>
        /// Creates a new Model instance from a JSON string that was previously
        /// exported using <see cref="Model.ToJSON">Model.ToJSON</see>.
        ///
        /// The method returns a tuple with three elements:
        /// 1. The reconstructed Model
        /// 2. Parameters (if they were included in the JSON), or None
        /// 3. Warm start Solution (if it was included in the JSON), or None
        ///
        /// Variables in the new model can be accessed using methods like
        /// <see cref="Model.GetIntervalVars">Model.GetIntervalVars</see>, <see cref="Model.GetIntVars">Model.GetIntVars</see>, etc.
        ///
        /// <code>
        /// // Create and export a model
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "task_x");
        /// model.Minimize(x.End());
        ///
        /// var parameters = new Parameters { timeLimit = 60 };
        /// var jsonStr = model.ToJSON(parameters);
        ///
        /// // Save to file
        /// File.WriteAllText("model.json", jsonStr);
        ///
        /// // Later, load from file
        /// var loadedJson = File.ReadAllText("model.json");
        ///
        /// // Restore model, parameters, and warm start
        /// var (model2, params2, warmStart2) = Model.FromJSON(loadedJson);
        ///
        /// // Access variables
        /// var intervalVars = model2.GetIntervalVars();
        /// Console.WriteLine($"Loaded model with {intervalVars.Count} interval variables");
        ///
        /// // Solve with restored parameters
        /// var result = model2.Solve(params2);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ToJSON">Model.ToJSON</see> — to export to JSON.</description></item>
        /// </list>
        /// </remarks>
        public static (Model Model, Parameters? Parameters, Solution? WarmStart) FromJSON(string jsonStr)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonStr)
                ?? throw new ArgumentException("Invalid JSON");
            
            var model = FromDict(data);
            Parameters? parameters = null;
            Solution? warmStart = null;

            if (data.TryGetValue("parameters", out var paramsObj) && paramsObj is JsonElement paramsElem
                && paramsElem.ValueKind == JsonValueKind.Object)
            {
                var paramsDict = ParseJsonElementToDictPreserveElements(paramsElem);
                if (paramsDict != null)
                    parameters = Parameters.FromDict(paramsDict);
            }

            if (data.TryGetValue("warmStart", out var warmObj) && warmObj is JsonElement warmElem
                && warmElem.ValueKind == JsonValueKind.Object)
            {
                var warmDict = ParseJsonElementToDictPreserveElements(warmElem);
                if (warmDict != null)
                {
                    warmStart = new Solution();
                    warmStart.InitFromDict(warmDict);
                }
            }

            return (model, parameters, warmStart);
        }

        internal static Model FromDict(Dictionary<string, object?> data)
        {
            var model = new Model();

            // Parse model name
            if (data.TryGetValue("name", out var nameObj) && nameObj is JsonElement nameElem
                && nameElem.ValueKind == JsonValueKind.String)
            {
                model._name = nameElem.GetString();
            }

            // Parse refs array to recreate variables
            if (data.TryGetValue("refs", out var refsObj) && refsObj is JsonElement refsElem
                && refsElem.ValueKind == JsonValueKind.Array)
            {
                int refId = 0;
                foreach (var refItem in refsElem.EnumerateArray())
                {
                    if (refItem.ValueKind != JsonValueKind.Object) { refId++; continue; }

                    var props = ParseJsonElementToDict(refItem);
                    if (props == null) { refId++; continue; }

                    // Always add to refs list to preserve ID mapping
                    model._refs.Add(props);

                    var func = props.TryGetValue("func", out var funcObj)
                        ? funcObj?.ToString() : null;

                    // Convert absent->optional with empty domain (backward compat)
                    if (func == "intVar" || func == "_intVar")
                    {
                        if (props.TryGetValue("status", out var intStatus)
                            && Convert.ToInt32(intStatus) == (int)PresenceStatus.Absent)
                        {
                            props["status"] = (int)PresenceStatus.Optional;
                            props["min"] = IntVarMax;
                            props["max"] = IntVarMin;
                        }
                        if (func == "intVar")
                        {
                            var intV = new IntVar(model, props, refId);
                            model._intVars.Add(intV);
                        }
                    }
                    else if (func == "boolVar" || func == "_boolVar")
                    {
                        if (props.TryGetValue("status", out var boolStatus)
                            && Convert.ToInt32(boolStatus) == (int)PresenceStatus.Absent)
                        {
                            props["status"] = (int)PresenceStatus.Optional;
                            props["min"] = 1;
                            props["max"] = 0;
                        }
                        if (func == "boolVar")
                        {
                            var boolV = new BoolVar(model, props, refId);
                            model._boolVars.Add(boolV);
                        }
                    }
                    else if (func == "intervalVar" || func == "_intervalVar")
                    {
                        if (props.TryGetValue("status", out var ivStatus)
                            && Convert.ToInt32(ivStatus) == (int)PresenceStatus.Absent)
                        {
                            props["status"] = (int)PresenceStatus.Optional;
                            props["startMin"] = IntervalMax;
                            props["startMax"] = IntervalMin;
                            props["endMin"] = IntervalMax;
                            props["endMax"] = IntervalMin;
                            props["lengthMin"] = LengthMax;
                            props["lengthMax"] = 0;
                        }
                        if (func == "intervalVar")
                        {
                            var iv = new IntervalVar(model, props, refId);
                            model._intervalVars.Add(iv);
                        }
                    }

                    refId++;
                }
            }

            // Parse model array (constraints)
            if (data.TryGetValue("model", out var modelObj) && modelObj is JsonElement modelElem
                && modelElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in modelElem.EnumerateArray())
                {
                    var parsed = ParseJsonElement(item);
                    if (parsed != null)
                        model._model.Add(parsed);
                }
            }

            // Parse objective
            if (data.TryGetValue("objective", out var objObj) && objObj is JsonElement objElem
                && objElem.ValueKind == JsonValueKind.Object)
            {
                model._objective = ParseJsonElementToDict(objElem);
            }

            return model;
        }

        /// <summary>
        /// Converts JsonElement to Dictionary but preserves JsonElement values (for Parameters/Solution).
        /// </summary>
        private static Dictionary<string, object?>? ParseJsonElementToDictPreserveElements(JsonElement elem)
        {
            if (elem.ValueKind != JsonValueKind.Object) return null;

            var result = new Dictionary<string, object?>();
            foreach (var prop in elem.EnumerateObject())
            {
                result[prop.Name] = prop.Value;
            }
            return result;
        }

        private static Dictionary<string, object?>? ParseJsonElementToDict(JsonElement elem)
        {
            if (elem.ValueKind != JsonValueKind.Object) return null;

            var result = new Dictionary<string, object?>();
            foreach (var prop in elem.EnumerateObject())
            {
                result[prop.Name] = ParseJsonElement(prop.Value);
            }
            return result;
        }

        private static object? ParseJsonElement(JsonElement elem)
        {
            switch (elem.ValueKind)
            {
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Number:
                    if (elem.TryGetInt32(out var intVal))
                        return intVal;
                    if (elem.TryGetInt64(out var longVal))
                        return longVal;
                    return elem.GetDouble();
                case JsonValueKind.String:
                    return elem.GetString();
                case JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (var item in elem.EnumerateArray())
                        list.Add(ParseJsonElement(item));
                    return list;
                case JsonValueKind.Object:
                    return ParseJsonElementToDict(elem);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Solves the model and returns the result.
        /// </summary>
        /// <param name="parameters">The parameters for solving</param>
        /// <param name="warmStart">The solution to start with</param>
        /// <returns>The result of the solve.</returns>
        /// <remarks>
        /// Solves the model using the OptalCP solver and returns the result. This is the
        /// main entry point for solving constraint programming models.
        ///
        /// The solver searches for solutions that satisfy all constraints in the model.
        /// If an objective was specified (using <see cref="Model.Minimize">Model.Minimize</see> or
        /// <see cref="Model.Maximize">Model.Maximize</see>), the solver searches for optimal or near-optimal
        /// solutions within the given time limit.
        ///
        /// The returned <see cref="SolveResult">SolveResult</see> contains:
        ///
        /// * <c>solution</c> - The best solution found, or <c>None</c> if no solution was found.
        ///   Use this to query variable values via methods like <c>get_start()</c>, <c>get_end()</c>,
        ///   and <c>get_value()</c>.
        /// * <c>objective</c> - The objective value of the best solution (if an objective
        ///   was specified).
        /// * <c>nb_solutions</c> - The total number of solutions found during the search.
        /// * <c>proof</c> - Whether the solver proved optimality or infeasibility.
        /// * <c>duration</c> - The total time spent solving.
        /// * Statistics like <c>nb_branches</c>, <c>nb_fails</c>, and <c>nb_restarts</c>.
        ///
        /// When an error occurs (e.g., invalid model, solver not found), the function
        /// raises an exception.
        ///
        /// <b>Parameters</b>
        ///
        /// Solver behavior can be controlled via the <c>parameters</c> argument. Common parameters
        /// include:
        ///
        /// * <c>timeLimit</c> - Maximum solving time in seconds.
        /// * <c>solutionLimit</c> - Stop after finding this many solutions.
        /// * <c>nbWorkers</c> - Number of parallel threads to use.
        /// * <c>searchType</c> - Search strategy (<c>"LNS"</c>, <c>"FDS"</c>, etc.).
        ///
        /// See <see cref="Parameters">Parameters</see> for the complete list.
        ///
        /// <b>Warm start</b>
        ///
        /// If the <c>warm_start</c> parameter is specified, the solver will start with the
        /// given solution. The solution must be compatible with the model; otherwise,
        /// an error will be raised. The solver will take advantage of the
        /// solution to speed up the search: it will search only for better solutions
        /// (if it is a minimization or maximization problem). The solver may also try to
        /// improve the provided solution by Large Neighborhood Search.
        ///
        /// <b>Advanced usage</b>
        ///
        /// This is a simple blocking function for basic usage. For advanced features
        /// like event callbacks, progress monitoring, or async support, use the
        /// <see cref="Solver">Solver</see> class instead.
        ///
        /// This method works seamlessly in both regular Python scripts and Jupyter
        /// notebooks. In Jupyter (where an event loop is already running), it
        /// automatically handles nested event loops.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "task_x");
        /// var y = model.IntervalVar(length: 20, name: "task_y");
        /// x.EndBeforeStart(y);
        /// model.Minimize(y.End());
        ///
        /// // Basic solve
        /// var result = model.Solve();
        /// Console.WriteLine($"Objective: {result.Objective}");
        ///
        /// // Solve with parameters
        /// var parameters = new Parameters { timeLimit = 60, searchType = "LNS" };
        /// result = model.Solve(parameters);
        ///
        /// // Solve with warm start
        /// if (result.Solution != null) {
        ///     var result2 = model.Solve(parameters, result.Solution);
        /// }
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solver">Solver</see> — for async solving with event callbacks.</description></item>
        /// <item><description><see cref="Parameters">Parameters</see> — for available solver parameters.</description></item>
        /// <item><description><see cref="SolveResult">SolveResult</see> — for the result structure.</description></item>
        /// <item><description><see cref="Solution">Solution</see> — for working with solutions.</description></item>
        /// </list>
        /// </remarks>
        public SolveResult Solve(Parameters? parameters = null, Solution? warmStart = null)
        {
            using var solverInstance = new Solver();
            return solverInstance.SyncSolve(this, parameters, warmStart);
        }

        /// <summary>
        /// Converts the model to text format similar to IBM CP Optimizer file format.
        /// </summary>
        /// <param name="parameters">Optional solver parameters (mostly unused)</param>
        /// <param name="warmStart">Optional initial solution to include</param>
        /// <returns>Text representation of the model.</returns>
        /// <remarks>
        /// The output is human-readable and can be stored in a file. Unlike JSON format,
        /// there is no way to convert the text format back into a Model.
        ///
        /// The result is so similar to the file format used by IBM CP Optimizer that,
        /// under some circumstances, the result can be used as an input file for
        /// CP Optimizer. However, some differences between OptalCP and CP Optimizer
        /// make it impossible to guarantee the result is always valid for CP Optimizer.
        ///
        /// Known issues:
        ///
        /// * OptalCP supports optional integer expressions, while CP Optimizer does not.
        ///   If the model contains optional integer expressions, the result will not be
        ///   valid for CP Optimizer or may be badly interpreted.
        /// * For the same reason, prefer precedence constraints such as
        ///   <c>end_before_start()</c> over <c>model.enforce(x.end() &lt;= y.start())</c>.
        /// * Negative heights in cumulative expressions (e.g., in <c>step_at_start()</c>)
        ///   are not supported by CP Optimizer.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "task_x");
        /// var y = model.IntervalVar(length: 20, name: "task_y");
        /// x.EndBeforeStart(y);
        /// model.Minimize(y.End());
        ///
        /// // Convert to text format
        /// var text = model.ToText();
        /// Console.WriteLine(text);
        ///
        /// // Save to file
        /// File.WriteAllText("model.txt", text);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ToJS">Model.ToJS</see> — to export as JavaScript code.</description></item>
        /// <item><description><see cref="Model.ToJSON">Model.ToJSON</see> — to export as JSON (can be imported back).</description></item>
        /// </list>
        /// </remarks>
        public string ToText(Parameters? parameters = null, Solution? warmStart = null)
        {
            using var solver = new Solver();
            return solver.SyncToText(this, parameters, warmStart);
        }

        /// <summary>
        /// Converts the model to equivalent JavaScript code.
        /// </summary>
        /// <param name="parameters">Optional solver parameters (included in generated code)</param>
        /// <param name="warmStart">Optional initial solution to include</param>
        /// <returns>JavaScript code representing the model.</returns>
        /// <remarks>
        /// The output is human-readable, executable with Node.js, and can be stored
        /// in a file. It is meant as a way to export a model to a format that is
        /// executable, human-readable, editable, and independent of other libraries.
        ///
        /// This feature is experimental and the result is not guaranteed to be valid
        /// in all cases.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "task_x");
        /// var y = model.IntervalVar(length: 20, name: "task_y");
        /// x.EndBeforeStart(y);
        /// model.Minimize(y.End());
        ///
        /// // Convert to JavaScript code
        /// var jsCode = model.ToJS();
        /// Console.WriteLine(jsCode);
        ///
        /// // Save to file
        /// File.WriteAllText("model.js", jsCode);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ToText">Model.ToText</see> — to export as text format.</description></item>
        /// <item><description><see cref="Model.ToJSON">Model.ToJSON</see> — to export as JSON (can be imported back).</description></item>
        /// </list>
        /// </remarks>
        public string ToJS(Parameters? parameters = null, Solution? warmStart = null)
        {
            using var solver = new Solver();
            return solver.SyncToJS(this, parameters, warmStart);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr ReusableBoolExpr(BoolExpr value)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(value) };
            return new BoolExpr(this, "reusableBoolExpr", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr ReusableBoolExpr(bool value)
        {
            var outParams = new List<object?>() { value };
            return new BoolExpr(this, "reusableBoolExpr", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr ReusableIntExpr(IntExpr value)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(value) };
            return new IntExpr(this, "reusableIntExpr", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr ReusableIntExpr(int value)
        {
            var outParams = new List<object?>() { value };
            return new IntExpr(this, "reusableIntExpr", outParams);
        }

        /// <summary>
        /// Negation of the boolean expression <c>arg</c>.
        /// </summary>
        /// <param name="arg">The argument to negate (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If the argument has value *absent* then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="BoolExpr.Not">BoolExpr.Not</see>.
        /// </remarks>
        public BoolExpr Not(BoolExpr arg)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(arg) };
            return new BoolExpr(this, "boolNot", outParams);
        }

        /// <inheritdoc cref="Not(BoolExpr)"/>
        public BoolExpr Not(bool arg)
        {
            var outParams = new List<object?>() { arg };
            return new BoolExpr(this, "boolNot", outParams);
        }

        /// <summary>
        /// Logical _OR_ of boolean expressions <c>lhs</c> and <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If one of the arguments has value *absent*, then the resulting expression also has value *absent*.
        ///
        /// Same as <see cref="BoolExpr.Or">BoolExpr.Or</see>.
        /// </remarks>
        public BoolExpr Or(BoolExpr lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolOr", outParams);
        }

        /// <inheritdoc cref="Or(BoolExpr, BoolExpr)"/>
        public BoolExpr Or(bool lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolOr", outParams);
        }

        /// <inheritdoc cref="Or(BoolExpr, BoolExpr)"/>
        public BoolExpr Or(BoolExpr lhs, bool rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), rhs };
            return new BoolExpr(this, "boolOr", outParams);
        }

        /// <summary>
        /// Logical _AND_ of boolean expressions <c>lhs</c> and <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If one of the arguments has value *absent*, then the resulting expression also has value *absent*.
        ///
        /// Same as <see cref="BoolExpr.And">BoolExpr.And</see>.
        /// </remarks>
        public BoolExpr And(BoolExpr lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolAnd", outParams);
        }

        /// <inheritdoc cref="And(BoolExpr, BoolExpr)"/>
        public BoolExpr And(bool lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolAnd", outParams);
        }

        /// <inheritdoc cref="And(BoolExpr, BoolExpr)"/>
        public BoolExpr And(BoolExpr lhs, bool rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), rhs };
            return new BoolExpr(this, "boolAnd", outParams);
        }

        /// <summary>
        /// Logical implication of two boolean expressions, that is <c>lhs</c> implies <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If one of the arguments has value *absent*, then the resulting expression also has value *absent*.
        ///
        /// Same as <see cref="BoolExpr.Implies">BoolExpr.Implies</see>.
        /// </remarks>
        public BoolExpr Implies(BoolExpr lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolImplies", outParams);
        }

        /// <inheritdoc cref="Implies(BoolExpr, BoolExpr)"/>
        public BoolExpr Implies(bool lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolImplies", outParams);
        }

        /// <inheritdoc cref="Implies(BoolExpr, BoolExpr)"/>
        public BoolExpr Implies(BoolExpr lhs, bool rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), rhs };
            return new BoolExpr(this, "boolImplies", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Eq(BoolExpr lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolEq", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Eq(bool lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolEq", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Eq(BoolExpr lhs, bool rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), rhs };
            return new BoolExpr(this, "boolEq", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Ne(BoolExpr lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolNe", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Ne(bool lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolNe", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Ne(BoolExpr lhs, bool rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), rhs };
            return new BoolExpr(this, "boolNe", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Nand(BoolExpr lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolNand", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Nand(bool lhs, BoolExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(this, "boolNand", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Nand(BoolExpr lhs, bool rhs)
        {
            var outParams = new List<object?>() { OptalCP.BoolExpr.Wrap(lhs), rhs };
            return new BoolExpr(this, "boolNand", outParams);
        }

        /// <summary>
        /// Creates an expression that replaces value *absent* by a constant.
        /// </summary>
        /// <param name="arg">The argument to guard (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <param name="absentValue">The value to use when the expression is absent.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// The resulting expression is:
        ///
        /// * equal to <c>arg</c> if <c>arg</c> is *present*
        /// * and equal to <c>absentValue</c> otherwise (i.e. when <c>arg</c> is *absent*).
        ///
        /// The default value of <c>absentValue</c> is 0.
        ///
        /// The resulting expression is never *absent*.
        ///
        /// Same as <see cref="IntExpr.Guard">IntExpr.Guard</see>.
        /// </remarks>
        public IntExpr Guard(IntExpr arg, int absentValue = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(arg), absentValue };
            return new IntExpr(this, "intGuard", outParams);
        }

        /// <inheritdoc cref="Guard(IntExpr, int)"/>
        public IntExpr Guard(int arg, int absentValue = 0)
        {
            var outParams = new List<object?>() { arg, absentValue };
            return new IntExpr(this, "intGuard", outParams);
        }

        /// <summary>
        /// Constrains <c>lhs</c> and <c>rhs</c> to be identical, including their presence status.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <returns>The identity constraint.</returns>
        /// <remarks>
        /// Identity is different than equality. For example, if <c>x</c> is *absent*, then <c>eq(x, 0)</c> is *absent*, but <c>identity(x, 0)</c> is <see langword="false"/>.
        ///
        /// Same as <see cref="IntExpr.Identity">IntExpr.Identity</see>.
        /// </remarks>
        public Constraint Identity(IntExpr lhs, IntExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(lhs), OptalCP.IntExpr.Wrap(rhs) };
            return new Constraint(this, "intIdentity", outParams);
        }

        /// <inheritdoc cref="Identity(IntExpr, IntExpr)"/>
        public Constraint Identity(int lhs, IntExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.IntExpr.Wrap(rhs) };
            return new Constraint(this, "intIdentity", outParams);
        }

        /// <inheritdoc cref="Identity(IntExpr, IntExpr)"/>
        public Constraint Identity(IntExpr lhs, int rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(lhs), rhs };
            return new Constraint(this, "intIdentity", outParams);
        }

        /// <summary>
        /// Creates Boolean expression <c>lb</c> &amp;le; <c>arg</c> &amp;le; <c>ub</c>.
        /// </summary>
        /// <param name="arg">The argument to check (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <param name="lb">The lower bound of the range.</param>
        /// <param name="ub">The upper bound of the range.</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If <c>arg</c> has value *absent* then the resulting expression has also value *absent*.
        ///
        /// Use <see cref="Model.Enforce">Model.Enforce</see> to add this expression as a constraint to the model.
        ///
        /// Same as <see cref="IntExpr.InRange">IntExpr.InRange</see>.
        /// </remarks>
        public BoolExpr InRange(IntExpr arg, int lb, int ub)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(arg), lb, ub };
            return new BoolExpr(this, "intInRange", outParams);
        }

        /// <inheritdoc cref="InRange(IntExpr, int, int)"/>
        public BoolExpr InRange(int arg, int lb, int ub)
        {
            var outParams = new List<object?>() { arg, lb, ub };
            return new BoolExpr(this, "intInRange", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr NotInRange(IntExpr arg, int lb, int ub)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(arg), lb, ub };
            return new BoolExpr(this, "intNotInRange", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr NotInRange(int arg, int lb, int ub)
        {
            var outParams = new List<object?>() { arg, lb, ub };
            return new BoolExpr(this, "intNotInRange", outParams);
        }

        /// <summary>
        /// Creates an integer expression which is absolute value of <c>arg</c>.
        /// </summary>
        /// <param name="arg">The argument (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If <c>arg</c> has value *absent* then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="IntExpr.Abs">IntExpr.Abs</see>.
        /// </remarks>
        public IntExpr Abs(IntExpr arg)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(arg) };
            return new IntExpr(this, "intAbs", outParams);
        }

        /// <inheritdoc cref="Abs(IntExpr)"/>
        public IntExpr Abs(int arg)
        {
            var outParams = new List<object?>() { arg };
            return new IntExpr(this, "intAbs", outParams);
        }

        /// <summary>
        /// Creates an integer expression which is the minimum of <c>lhs</c> and <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If one of the arguments has value *absent*, then the resulting expression also has value *absent*.
        ///
        /// Same as <see cref="IntExpr.Min2">IntExpr.Min2</see>. See <see cref="Model.Min">Model.Min</see> for n-ary minimum.
        /// </remarks>
        public IntExpr Min2(IntExpr lhs, IntExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(lhs), OptalCP.IntExpr.Wrap(rhs) };
            return new IntExpr(this, "intMin2", outParams);
        }

        /// <inheritdoc cref="Min2(IntExpr, IntExpr)"/>
        public IntExpr Min2(int lhs, IntExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.IntExpr.Wrap(rhs) };
            return new IntExpr(this, "intMin2", outParams);
        }

        /// <inheritdoc cref="Min2(IntExpr, IntExpr)"/>
        public IntExpr Min2(IntExpr lhs, int rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(lhs), rhs };
            return new IntExpr(this, "intMin2", outParams);
        }

        /// <summary>
        /// Creates an integer expression which is the maximum of <c>lhs</c> and <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If one of the arguments has value *absent*, then the resulting expression also has value *absent*.
        ///
        /// Same as <see cref="IntExpr.Max2">IntExpr.Max2</see>. See <see cref="Model.Max">Model.Max</see> for n-ary maximum.
        /// </remarks>
        public IntExpr Max2(IntExpr lhs, IntExpr rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(lhs), OptalCP.IntExpr.Wrap(rhs) };
            return new IntExpr(this, "intMax2", outParams);
        }

        /// <inheritdoc cref="Max2(IntExpr, IntExpr)"/>
        public IntExpr Max2(int lhs, IntExpr rhs)
        {
            var outParams = new List<object?>() { lhs, OptalCP.IntExpr.Wrap(rhs) };
            return new IntExpr(this, "intMax2", outParams);
        }

        /// <inheritdoc cref="Max2(IntExpr, IntExpr)"/>
        public IntExpr Max2(IntExpr lhs, int rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(lhs), rhs };
            return new IntExpr(this, "intMax2", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr LearningSum(IEnumerable<IntExpr> args)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(args) };
            return new IntExpr(this, "learningSum", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr LearningSum(IEnumerable<object> args)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(args) };
            return new IntExpr(this, "learningSum", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the maximum of the arguments.
        /// </summary>
        /// <param name="args">Array of integer expressions to compute maximum of.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// Absent arguments are ignored as if they were not specified in the input array <c>args</c>. Maximum of an empty set (i.e. <c>max([])</c> is *absent*. The maximum is *absent* also if all arguments are *absent*.
        ///
        /// Note that binary function <see cref="Model.Max2">Model.Max2</see> handles absent values differently. For example, when <c>x</c> is *absent* then:
        ///
        /// * <c>max2(x, 5)</c> is *absent*.
        /// * <c>max([x, 5])</c> is 5.
        /// * <c>max([x])</c> is *absent*.
        ///
        /// <para>Example:</para>
        /// <example>
        /// A common use case is to compute _makespan_ of a set of tasks, i.e. the time when the last task finishes. In the following example, we minimize the makespan of a set of tasks (other parts of the model are omitted).
        ///
        /// <code>
        /// var model = new Model();
        /// var tasks = Enumerable.Range(0, 5)
        ///     .Select(i =&gt; model.IntervalVar(length: 10, name: $"task_{i}")).ToArray();
        /// // Create an array of end times of the tasks:
        /// var endTimes = tasks.Select(t =&gt; t.End());
        /// var makespan = model.Max(endTimes);
        /// model.Minimize(makespan);
        /// </code>
        /// Notice that when a task is *absent* (not executed), then its end time is *absent*. And therefore, the absent task is not included in the maximum.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Max2">Model.Max2</see></description></item>
        /// <item><description><see cref="Model.Span">Model.Span</see> — constraints interval variable to start and end at minimum and maximum of the given set of intervals.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Max(IEnumerable<IntExpr> args)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(args) };
            return new IntExpr(this, "intMax", outParams);
        }

        /// <inheritdoc cref="Max(IEnumerable{IntExpr})"/>
        public IntExpr Max(IEnumerable<object> args)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(args) };
            return new IntExpr(this, "intMax", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the minimum of the arguments.
        /// </summary>
        /// <param name="args">Array of integer expressions to compute minimum of.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// Absent arguments are ignored as if they were not specified in the input array <c>args</c>. Minimum of an empty set (i.e. <c>min([])</c>) is *absent*. The minimum is *absent* also if all arguments are *absent*.
        ///
        /// Note that binary function <see cref="Model.Min2">Model.Min2</see> handles absent values differently. For example, when <c>x</c> is *absent* then:
        ///
        /// * <c>min2(x, 5)</c> is *absent*.
        /// * <c>min([x, 5])</c> is 5.
        /// * <c>min([x])</c> is *absent*.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we compute the time when the first task of <c>tasks</c> starts, i.e. the minimum of the starting times.
        ///
        /// <code>
        /// var model = new Model();
        /// var tasks = Enumerable.Range(0, 5)
        ///     .Select(i =&gt; model.IntervalVar(length: 10, name: $"task_{i}")).ToArray();
        /// // Create an array of start times of the tasks:
        /// var startTimes = tasks.Select(t =&gt; t.Start());
        /// var firstStartTime = model.Min(startTimes);
        /// </code>
        /// Notice that when a task is *absent* (not executed), its end time is *absent*. And therefore, the absent task is not included in the minimum.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Min2">Model.Min2</see></description></item>
        /// <item><description><see cref="Model.Span">Model.Span</see> — constraints interval variable to start and end at minimum and maximum of the given set of intervals.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Min(IEnumerable<IntExpr> args)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(args) };
            return new IntExpr(this, "intMin", outParams);
        }

        /// <inheritdoc cref="Min(IEnumerable{IntExpr})"/>
        public IntExpr Min(IEnumerable<object> args)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(args) };
            return new IntExpr(this, "intMin", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr IntPresentLinearExpr(IEnumerable<int> coefficients, IEnumerable<IntExpr> expressions, int constantTerm = 0)
        {
            var outParams = new List<object?>() { ModelElement.WrapIntList(coefficients), OptalCP.IntExpr.WrapList(expressions), constantTerm };
            return new IntExpr(this, "intPresentLinearExpr", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr IntPresentLinearExpr(IEnumerable<int> coefficients, IEnumerable<object> expressions, int constantTerm = 0)
        {
            var outParams = new List<object?>() { ModelElement.WrapIntList(coefficients), OptalCP.IntExpr.WrapMixedList(expressions), constantTerm };
            return new IntExpr(this, "intPresentLinearExpr", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr IntOptionalLinearExpr(IEnumerable<int> coefficients, IEnumerable<IntExpr> expressions, int constantTerm = 0)
        {
            var outParams = new List<object?>() { ModelElement.WrapIntList(coefficients), OptalCP.IntExpr.WrapList(expressions), constantTerm };
            return new IntExpr(this, "intOptionalLinearExpr", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr IntOptionalLinearExpr(IEnumerable<int> coefficients, IEnumerable<object> expressions, int constantTerm = 0)
        {
            var outParams = new List<object?>() { ModelElement.WrapIntList(coefficients), OptalCP.IntExpr.WrapMixedList(expressions), constantTerm };
            return new IntExpr(this, "intOptionalLinearExpr", outParams);
        }

        /// <summary>
        /// Creates an integer expression for array element lookup by index.
        /// </summary>
        /// <param name="array">Array of integer values to select from.</param>
        /// <param name="subscript">Index into the array (0-based).</param>
        /// <returns>The resulting integer expression equal to <c>array[subscript]</c>.</returns>
        /// <remarks>
        /// The result of <c>element(array, subscript)</c> is an integer expression equal to <c>array[subscript]</c>.
        ///
        /// Creating the expression automatically adds a side constraint that restricts <c>subscript</c> to the valid index range <c>0</c> to <c>n - 1</c>, where <c>n</c> is the number of elements in <c>array</c>. This constraint must be satisfied in every solution, regardless of how the expression is used. For example, in <c>element(array, subscript) == 5 || x == 1</c>, the subscript must be a valid index even in solutions where <c>x == 1</c> makes the disjunction true.
        ///
        /// When the <c>subscript</c> is *absent*, the result is also *absent*.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, there are 3 machines with different processing times for a task. We use <c>element</c> to look up the processing time based on which machine is assigned.
        ///
        /// <code>
        /// var model = new Model();
        /// // Processing times for each machine:
        /// var durations = new[] { 5, 8, 3 };
        /// // Decision variable: which machine to use (0, 1, or 2):
        /// var machine = model.IntVar(min: 0, max: durations.Length - 1, name: "machine");
        /// // Look up the duration for the chosen machine:
        /// var duration = model.Element(durations, machine);
        /// var task = model.IntervalVar(length: duration, name: "task");
        /// model.Minimize(task.End());
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.IntVar">Model.IntVar</see> — to create the subscript variable.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Element(IEnumerable<int> array, IntExpr subscript)
        {
            var outParams = new List<object?>() { ModelElement.WrapIntList(array), OptalCP.IntExpr.Wrap(subscript) };
            return new IntExpr(this, "intElement", outParams);
        }

        /// <inheritdoc cref="Element(IEnumerable{int}, IntExpr)"/>
        public IntExpr Element(IEnumerable<int> array, int subscript)
        {
            var outParams = new List<object?>() { ModelElement.WrapIntList(array), subscript };
            return new IntExpr(this, "intElement", outParams);
        }

        /// <summary>
        /// Lexicographic less than or equal constraint: <c>lhs</c> ≤ <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side array of integer expressions.</param>
        /// <param name="rhs">The right-hand side array of integer expressions.</param>
        /// <returns>The lexicographic constraint.</returns>
        /// <remarks>
        /// Constrains <c>lhs</c> to be lexicographically less than or equal <c>rhs</c>.
        ///
        /// Both arrays must have the same length, and the length must be at least 1.
        ///
        /// Lexicographic ordering compares arrays element by element from the first
        /// position. The comparison <c>lhs</c> ≤ <c>rhs</c> holds if and only if:
        ///
        /// * all elements are equal (<c>lhs[i] == rhs[i]</c> for all <c>i</c>), or
        /// * there exists a position <c>k</c> where <c>lhs[k] &lt; rhs[k]</c> and all preceding elements are equal (<c>lhs[i] == rhs[i]</c> for all <c>i &lt; k</c>)
        ///
        /// Lexicographic constraints are useful for symmetry breaking. For example, when
        /// you have multiple equivalent solutions that differ only in the ordering of
        /// symmetric variables, adding a lexicographic constraint can eliminate redundant
        /// solutions.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        ///
        /// // Variables for a 3x3 matrix where rows should be lexicographically ordered
        /// var rows = Enumerable.Range(0, 3).Select(i =&gt;
        ///   Enumerable.Range(0, 3).Select(j =&gt; model.IntVar(min: 0, max: 9, name: $"x_{i}_{j}")).ToArray()
        /// ).ToArray();
        ///
        /// // Break row symmetry: row[0] ≤ row[1] ≤ row[2] lexicographically
        /// model.LexLe(rows[0], rows[1]);
        /// model.LexLe(rows[1], rows[2]);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.LexLt">Model.LexLt</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexGe">Model.LexGe</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexGt">Model.LexGt</see> — for other lexicographic comparisons.</description></item>
        /// </list>
        /// </remarks>
        public Constraint LexLe(IEnumerable<IntExpr> lhs, IEnumerable<IntExpr> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(lhs), OptalCP.IntExpr.WrapList(rhs) };
            return new Constraint(this, "intLexLe", outParams);
        }

        /// <inheritdoc cref="LexLe(IEnumerable{IntExpr}, IEnumerable{IntExpr})"/>
        public Constraint LexLe(IEnumerable<object> lhs, IEnumerable<object> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(lhs), OptalCP.IntExpr.WrapMixedList(rhs) };
            return new Constraint(this, "intLexLe", outParams);
        }

        /// <summary>
        /// Lexicographic strictly less than constraint: <c>lhs</c> &lt; <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side array of integer expressions.</param>
        /// <param name="rhs">The right-hand side array of integer expressions.</param>
        /// <returns>The lexicographic constraint.</returns>
        /// <remarks>
        /// Constrains <c>lhs</c> to be lexicographically strictly less than <c>rhs</c>.
        ///
        /// Both arrays must have the same length, and the length must be at least 1.
        ///
        /// Lexicographic ordering compares arrays element by element from the first
        /// position. The comparison <c>lhs</c> &lt; <c>rhs</c> holds if and only if:
        /// there exists a position <c>k</c> where <c>lhs[k] &lt; rhs[k]</c> and all preceding elements are equal (<c>lhs[i] == rhs[i]</c> for all <c>i &lt; k</c>)
        ///
        /// Lexicographic constraints are useful for symmetry breaking. For example, when
        /// you have multiple equivalent solutions that differ only in the ordering of
        /// symmetric variables, adding a lexicographic constraint can eliminate redundant
        /// solutions.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        ///
        /// // Variables for a 3x3 matrix where rows should be lexicographically ordered
        /// var rows = Enumerable.Range(0, 3).Select(i =&gt;
        ///   Enumerable.Range(0, 3).Select(j =&gt; model.IntVar(min: 0, max: 9, name: $"x_{i}_{j}")).ToArray()
        /// ).ToArray();
        ///
        /// // Break row symmetry: row[0] &lt; row[1] &lt; row[2] lexicographically
        /// model.LexLt(rows[0], rows[1]);
        /// model.LexLt(rows[1], rows[2]);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.LexLe">Model.LexLe</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexGe">Model.LexGe</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexGt">Model.LexGt</see> — for other lexicographic comparisons.</description></item>
        /// </list>
        /// </remarks>
        public Constraint LexLt(IEnumerable<IntExpr> lhs, IEnumerable<IntExpr> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(lhs), OptalCP.IntExpr.WrapList(rhs) };
            return new Constraint(this, "intLexLt", outParams);
        }

        /// <inheritdoc cref="LexLt(IEnumerable{IntExpr}, IEnumerable{IntExpr})"/>
        public Constraint LexLt(IEnumerable<object> lhs, IEnumerable<object> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(lhs), OptalCP.IntExpr.WrapMixedList(rhs) };
            return new Constraint(this, "intLexLt", outParams);
        }

        /// <summary>
        /// Lexicographic greater than or equal constraint: <c>lhs</c> ≥ <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side array of integer expressions.</param>
        /// <param name="rhs">The right-hand side array of integer expressions.</param>
        /// <returns>The lexicographic constraint.</returns>
        /// <remarks>
        /// Constrains <c>lhs</c> to be lexicographically greater than or equal <c>rhs</c>.
        ///
        /// Both arrays must have the same length, and the length must be at least 1.
        ///
        /// Lexicographic ordering compares arrays element by element from the first
        /// position. The comparison <c>lhs</c> ≥ <c>rhs</c> holds if and only if:
        ///
        /// * all elements are equal (<c>lhs[i] == rhs[i]</c> for all <c>i</c>), or
        /// * there exists a position <c>k</c> where <c>lhs[k] &gt; rhs[k]</c> and all preceding elements are equal (<c>lhs[i] == rhs[i]</c> for all <c>i &lt; k</c>)
        ///
        /// Lexicographic constraints are useful for symmetry breaking. For example, when
        /// you have multiple equivalent solutions that differ only in the ordering of
        /// symmetric variables, adding a lexicographic constraint can eliminate redundant
        /// solutions.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        ///
        /// // Variables for a 3x3 matrix where rows should be lexicographically ordered
        /// var rows = Enumerable.Range(0, 3).Select(i =&gt;
        ///   Enumerable.Range(0, 3).Select(j =&gt; model.IntVar(min: 0, max: 9, name: $"x_{i}_{j}")).ToArray()
        /// ).ToArray();
        ///
        /// // Break row symmetry: row[0] ≥ row[1] ≥ row[2] lexicographically
        /// model.LexGe(rows[0], rows[1]);
        /// model.LexGe(rows[1], rows[2]);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.LexLe">Model.LexLe</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexLt">Model.LexLt</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexGt">Model.LexGt</see> — for other lexicographic comparisons.</description></item>
        /// </list>
        /// </remarks>
        public Constraint LexGe(IEnumerable<IntExpr> lhs, IEnumerable<IntExpr> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(lhs), OptalCP.IntExpr.WrapList(rhs) };
            return new Constraint(this, "intLexGe", outParams);
        }

        /// <inheritdoc cref="LexGe(IEnumerable{IntExpr}, IEnumerable{IntExpr})"/>
        public Constraint LexGe(IEnumerable<object> lhs, IEnumerable<object> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(lhs), OptalCP.IntExpr.WrapMixedList(rhs) };
            return new Constraint(this, "intLexGe", outParams);
        }

        /// <summary>
        /// Lexicographic strictly greater than constraint: <c>lhs</c> &gt; <c>rhs</c>.
        /// </summary>
        /// <param name="lhs">The left-hand side array of integer expressions.</param>
        /// <param name="rhs">The right-hand side array of integer expressions.</param>
        /// <returns>The lexicographic constraint.</returns>
        /// <remarks>
        /// Constrains <c>lhs</c> to be lexicographically strictly greater than <c>rhs</c>.
        ///
        /// Both arrays must have the same length, and the length must be at least 1.
        ///
        /// Lexicographic ordering compares arrays element by element from the first
        /// position. The comparison <c>lhs</c> &gt; <c>rhs</c> holds if and only if:
        /// there exists a position <c>k</c> where <c>lhs[k] &gt; rhs[k]</c> and all preceding elements are equal (<c>lhs[i] == rhs[i]</c> for all <c>i &lt; k</c>)
        ///
        /// Lexicographic constraints are useful for symmetry breaking. For example, when
        /// you have multiple equivalent solutions that differ only in the ordering of
        /// symmetric variables, adding a lexicographic constraint can eliminate redundant
        /// solutions.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        ///
        /// // Variables for a 3x3 matrix where rows should be lexicographically ordered
        /// var rows = Enumerable.Range(0, 3).Select(i =&gt;
        ///   Enumerable.Range(0, 3).Select(j =&gt; model.IntVar(min: 0, max: 9, name: $"x_{i}_{j}")).ToArray()
        /// ).ToArray();
        ///
        /// // Break row symmetry: row[0] &gt; row[1] &gt; row[2] lexicographically
        /// model.LexGt(rows[0], rows[1]);
        /// model.LexGt(rows[1], rows[2]);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.LexLe">Model.LexLe</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexLt">Model.LexLt</see> — for other lexicographic comparisons.</description></item>
        /// <item><description><see cref="Model.LexGe">Model.LexGe</see> — for other lexicographic comparisons.</description></item>
        /// </list>
        /// </remarks>
        public Constraint LexGt(IEnumerable<IntExpr> lhs, IEnumerable<IntExpr> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(lhs), OptalCP.IntExpr.WrapList(rhs) };
            return new Constraint(this, "intLexGt", outParams);
        }

        /// <inheritdoc cref="LexGt(IEnumerable{IntExpr}, IEnumerable{IntExpr})"/>
        public Constraint LexGt(IEnumerable<object> lhs, IEnumerable<object> rhs)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(lhs), OptalCP.IntExpr.WrapMixedList(rhs) };
            return new Constraint(this, "intLexGt", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the start time of an interval variable.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the interval is absent, the resulting expression is also absent.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we constrain interval variable <c>y</c> to start after the end of <c>x</c> with a delay of at least 10. In addition, we constrain the length of <c>x</c> to be less or equal to the length of <c>y</c>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x");
        /// var y = model.IntervalVar(name: "y");
        /// model.Enforce(model.End(x) + 10 &lt;= model.Start(y));
        /// model.Enforce(model.Length(x) &lt;= model.Length(y));
        /// </code>
        ///
        /// When <c>x</c> or <c>y</c> is *absent* then value of both constraints above is *absent* and therefore they are satisfied.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Start(IntervalVar interval)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval) };
            return new IntExpr(this, "startOf", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the end time of an interval variable.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the interval is absent, the resulting expression is also absent.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we constrain interval variable <c>y</c> to start after the end of <c>x</c> with a delay of at least 10. In addition, we constrain the length of <c>x</c> to be less or equal to the length of <c>y</c>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x");
        /// var y = model.IntervalVar(name: "y");
        /// model.Enforce(model.End(x) + 10 &lt;= model.Start(y));
        /// model.Enforce(model.Length(x) &lt;= model.Length(y));
        /// </code>
        ///
        /// When <c>x</c> or <c>y</c> is *absent* then value of both constraints above is *absent* and therefore they are satisfied.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr End(IntervalVar interval)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval) };
            return new IntExpr(this, "endOf", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the duration (end - start) of an interval variable.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the interval is absent, the resulting expression is also absent.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we constrain interval variable <c>y</c> to start after the end of <c>x</c> with a delay of at least 10. In addition, we constrain the length of <c>x</c> to be less or equal to the length of <c>y</c>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x");
        /// var y = model.IntervalVar(name: "y");
        /// model.Enforce(model.End(x) + 10 &lt;= model.Start(y));
        /// model.Enforce(model.Length(x) &lt;= model.Length(y));
        /// </code>
        ///
        /// When <c>x</c> or <c>y</c> is *absent* then value of both constraints above is *absent* and therefore they are satisfied.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.Length">IntervalVar.Length</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Length(IntervalVar interval)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval) };
            return new IntExpr(this, "lengthOf", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr AlternativeCost(IntervalVar main, IEnumerable<IntervalVar> options, IEnumerable<int> weights)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(main), OptalCP.IntervalVar.WrapList(options), ModelElement.WrapIntList(weights) };
            return new IntExpr(this, "intAlternativeCost", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay &lt;= successor.End());
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be less than or equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.EndBeforeEnd">IntervalVar.EndBeforeEnd</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndBeforeEnd(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "endBeforeEnd", outParams);
        }

        /// <inheritdoc cref="EndBeforeEnd(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint EndBeforeEnd(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "endBeforeEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay &lt;= successor.Start());
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be less than or equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.EndBeforeStart">IntervalVar.EndBeforeStart</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndBeforeStart(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "endBeforeStart", outParams);
        }

        /// <inheritdoc cref="EndBeforeStart(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint EndBeforeStart(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "endBeforeStart", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay &lt;= successor.End());
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be less than or equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StartBeforeEnd">IntervalVar.StartBeforeEnd</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartBeforeEnd(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "startBeforeEnd", outParams);
        }

        /// <inheritdoc cref="StartBeforeEnd(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint StartBeforeEnd(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "startBeforeEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay &lt;= successor.Start());
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be less than or equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StartBeforeStart">IntervalVar.StartBeforeStart</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartBeforeStart(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "startBeforeStart", outParams);
        }

        /// <inheritdoc cref="StartBeforeStart(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint StartBeforeStart(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "startBeforeStart", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay == successor.End());
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.EndAtEnd">IntervalVar.EndAtEnd</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndAtEnd(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "endAtEnd", outParams);
        }

        /// <inheritdoc cref="EndAtEnd(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint EndAtEnd(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "endAtEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay == successor.Start());
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.EndAtStart">IntervalVar.EndAtStart</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndAtStart(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "endAtStart", outParams);
        }

        /// <inheritdoc cref="EndAtStart(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint EndAtStart(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "endAtStart", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay == successor.End());
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StartAtEnd">IntervalVar.StartAtEnd</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartAtEnd(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "startAtEnd", outParams);
        }

        /// <inheritdoc cref="StartAtEnd(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint StartAtEnd(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "startAtEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="predecessor">The predecessor interval variable.</param>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay == successor.Start());
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StartAtStart">IntervalVar.StartAtStart</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartAtStart(IntervalVar predecessor, IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(this, "startAtStart", outParams);
        }

        /// <inheritdoc cref="StartAtStart(IntervalVar, IntervalVar, IntExpr)"/>
        public Constraint StartAtStart(IntervalVar predecessor, IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(predecessor), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(this, "startAtStart", outParams);
        }

        /// <summary>
        /// Alternative constraint models a choice between different ways to execute an interval.
        /// </summary>
        /// <param name="main">The main interval variable.</param>
        /// <param name="options">Array of optional interval variables to choose from.</param>
        /// <returns>The alternative constraint.</returns>
        /// <remarks>
        /// Alternative constraint is a way to model various kinds of choices. For example, we can model a task that could be done by worker A, B, or C. To model such alternative, we use interval variable <c>main</c> that represents the task regardless the chosen worker and three interval variables <c>options = [A, B, C]</c> that represent the task when done by worker A, B, or C. Interval variables <c>A</c>, <c>B</c>, and <c>C</c> should be optional. This way, if e.g. option B is chosen, then <c>B</c> will be *present* and equal to <c>main</c> (they will start at the same time and end at the same time), the remaining options, A and C, will be *absent*.
        ///
        /// We may also decide not to execute the <c>main</c> task at all (if it is optional). Then <c>main</c> will be *absent* and all options <c>A</c>, <c>B</c> and <c>C</c> will be *absent* too.
        ///
        /// <b>Formal definition</b>
        ///
        /// The constraint <c>alternative(main, options)</c> is satisfied in the following two cases:
        /// 1. Interval <c>main</c> is *absent* and all <c>options[i]</c> are *absent* too.
        /// 2. Interval <c>main</c> is *present* and there is exactly one index <c>k</c> such that <c>options[k]</c> is *present* (the remaining options are *absent*).    Then <c>main.start() == options[k].start()</c> and <c>main.end() == options[k].end()</c>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// Let's consider task T, which can be done by workers A, B, or C. The length of the task and a cost associated with it depends on the chosen worker:
        ///
        /// * If done by worker A, then its length is 10, and the cost is 5.
        /// * If done by worker B, then its length is 20, and the cost is 2.
        /// * If done by worker C, then its length is 3, and the cost is 10.
        ///
        /// Each worker can execute only one task at a time. However, the remaining tasks are omitted in the model below. The objective could be, e.g., to minimize the total cost (also omitted in the model).
        ///
        /// <code>
        /// var model = new Model();
        ///
        /// var T = model.IntervalVar(name: "T");
        /// var T_A = model.IntervalVar(name: "T_A", optional: true, length: 10);
        /// var T_B = model.IntervalVar(name: "T_B", optional: true, length: 20);
        /// var T_C = model.IntervalVar(name: "T_C", optional: true, length: 3);
        ///
        /// // T_A, T_B and T_C are different ways to execute task T:
        /// model.Alternative(T, new[] { T_A, T_B, T_C });
        /// // The cost depends on the chosen option:
        /// var costOfT = model.Sum(new IntExpr[] {
        ///     model.Presence(T_A) * 5,
        ///     model.Presence(T_B) * 2,
        ///     model.Presence(T_C) * 10
        /// });
        ///
        /// // Each worker A can perform only one task at a time:
        /// model.NoOverlap(new[] { T_A /* ... other Worker A tasks */ });  // Worker A
        /// model.NoOverlap(new[] { T_B /* ... other Worker B tasks */ });  // Worker B
        /// model.NoOverlap(new[] { T_C /* ... other Worker C tasks */ });  // Worker C
        ///
        /// // Minimize the total cost:
        /// model.Minimize(model.Sum(new IntExpr[] { costOfT /* ... other costs */ }));
        /// </code>
        /// </example>
        /// </remarks>
        public Constraint Alternative(IntervalVar main, IEnumerable<IntervalVar> options)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(main), OptalCP.IntervalVar.WrapList(options) };
            return new Constraint(this, "alternative", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint IntervalVarElement(IEnumerable<IntervalVar> slots, IntExpr index, IntervalVar value)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(slots), OptalCP.IntExpr.Wrap(index), OptalCP.IntervalVar.Wrap(value) };
            return new Constraint(this, "intervalVarElement", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint IntervalVarElement(IEnumerable<IntervalVar> slots, int index, IntervalVar value)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(slots), index, OptalCP.IntervalVar.Wrap(value) };
            return new Constraint(this, "intervalVarElement", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint IncreasingIntervalVarElement(IEnumerable<IntervalVar> slots, IntExpr index, IntervalVar value)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(slots), OptalCP.IntExpr.Wrap(index), OptalCP.IntervalVar.Wrap(value) };
            return new Constraint(this, "increasingIntervalVarElement", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint IncreasingIntervalVarElement(IEnumerable<IntervalVar> slots, int index, IntervalVar value)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(slots), index, OptalCP.IntervalVar.Wrap(value) };
            return new Constraint(this, "increasingIntervalVarElement", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint ItvMapping(IEnumerable<IntervalVar> tasks, IEnumerable<IntervalVar> slots, IEnumerable<IntExpr> indices)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(tasks), OptalCP.IntervalVar.WrapList(slots), OptalCP.IntExpr.WrapList(indices) };
            return new Constraint(this, "itvMapping", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint ItvMapping(IEnumerable<IntervalVar> tasks, IEnumerable<IntervalVar> slots, IEnumerable<object> indices)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(tasks), OptalCP.IntervalVar.WrapList(slots), OptalCP.IntExpr.WrapMixedList(indices) };
            return new Constraint(this, "itvMapping", outParams);
        }

        /// <summary>
        /// Constrains an interval variable to span (cover) a set of other interval variables.
        /// </summary>
        /// <param name="main">The spanning interval variable.</param>
        /// <param name="covered">The set of interval variables to cover.</param>
        /// <returns>The span constraint.</returns>
        /// <remarks>
        /// Span constraint can be used to model, for example, a composite task that consists of several subtasks.
        ///
        /// The constraint makes sure that interval variable <c>main</c> starts with the first interval in <c>covered</c> and ends with the last interval in <c>covered</c>. Absent interval variables in <c>covered</c> are ignored.
        ///
        /// <b>Formal definition</b>
        ///
        /// Span constraint is satisfied in one of the following two cases:
        ///
        /// * Interval variable <c>main</c> is absent and all interval variables in <c>covered</c> are absent too.
        /// * Interval variable <c>main</c> is present, at least one interval in <c>covered</c> is present and:
        ///
        ///    * <c>main.start()</c> is equal to the minimum starting time of all present intervals in <c>covered</c>.
        ///    * <c>main.end()</c> is equal to the maximum ending time of all present intervals in <c>covered</c>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// Let's consider composite task <c>T</c>, which consists of 3 subtasks: <c>T1</c>, <c>T2</c>, and <c>T3</c>. Subtasks are independent, could be processed in any order, and may overlap. However, task T is blocking a particular location, and no other task can be processed there. The location is blocked as soon as the first task from <c>T1</c>, <c>T2</c>, <c>T3</c> starts, and it remains blocked until the last one of them finishes.
        ///
        /// <code>
        /// var model = new Model();
        ///
        /// // Subtasks have known lengths:
        /// var T1 = model.IntervalVar(name: "T1", length: 10);
        /// var T2 = model.IntervalVar(name: "T2", length: 5);
        /// var T3 = model.IntervalVar(name: "T3", length: 15);
        /// // The main task has unknown length though:
        /// var T = model.IntervalVar(name: "T");
        ///
        /// // T spans/covers T1, T2 and T3:
        /// model.Span(T, new[] { T1, T2, T3 });
        ///
        /// // Tasks requiring the same location cannot overlap.
        /// // Other tasks are not included in the example, therefore '...' below:
        /// model.NoOverlap(new[] { T /* ... other tasks sharing the same location */ });
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.Span">IntervalVar.Span</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// </list>
        /// </remarks>
        public Constraint Span(IntervalVar main, IEnumerable<IntervalVar> covered)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(main), OptalCP.IntervalVar.WrapList(covered) };
            return new Constraint(this, "span", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint SelectiveDisjunction(IEnumerable<IntervalVar> intervals, IEnumerable<int> types, IEnumerable<IEnumerable<int>>? transitions = null)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(intervals), ModelElement.WrapIntList(types) };
            if (transitions != null)
                outParams.Add(ModelElement.WrapIntMatrix(transitions));
            return new Constraint(this, "selectiveDisjunction", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint AlwaysEqual(StateFunction func, IntervalVar interval, int value)
        {
            var outParams = new List<object?>() { OptalCP.StateFunction.Wrap(func), OptalCP.IntervalVar.Wrap(interval), value };
            return new Constraint(this, "alwaysEqual", outParams);
        }

        /// <summary>
        /// Creates an expression equal to the position of the <c>interval</c> on the <c>sequence</c>.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <param name="sequence">The sequence variable.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// In the solution, the present interval that is scheduled first in the <c>sequence</c> has position 0, the second one has position 1, etc. Absent intervals are not counted. The position of an absent interval is <c>absent</c>.
        ///
        /// The <c>position</c> expression has the following requirements:
        ///
        /// * The <c>sequence</c> must be used in a <see cref="Model.NoOverlap">Model.NoOverlap</see> constraint.
        /// * The <c>interval</c> must appear exactly once in the <c>sequence</c>.
        /// * The <c>interval</c> cannot have zero length, i.e., its minimum length must be at least 1. Otherwise, the position of two simultaneous zero-length intervals would be undefined.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.Position">IntervalVar.Position</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for constraints on overlapping intervals.</description></item>
        /// <item><description><see cref="Model.SequenceVar">Model.SequenceVar</see> — for creating sequence variables.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Position(IntervalVar interval, SequenceVar sequence)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), OptalCP.SequenceVar.Wrap(sequence) };
            return new IntExpr(this, "position", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint SameSequence(SequenceVar sequence1, SequenceVar sequence2)
        {
            var outParams = new List<object?>() { OptalCP.SequenceVar.Wrap(sequence1), OptalCP.SequenceVar.Wrap(sequence2) };
            return new Constraint(this, "sameSequence", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint SameSequenceGroup(IEnumerable<SequenceVar> sequences)
        {
            var outParams = new List<object?>() { OptalCP.SequenceVar.WrapList(sequences) };
            return new Constraint(this, "sameSequenceGroup", outParams);
        }

        /// <summary>
        /// Creates cumulative function (expression) _pulse_ for the given interval variable and height.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <param name="height">The height value.</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// Pulse can be used to model a resource requirement during an interval variable. The given amount <c>height</c> of the resource is used throughout the interval (from start to end).
        ///
        /// Limitation: The <c>height</c> must be non-negative. Pulses with negative height are not supported. If you need negative contributions, use step functions instead (see <see cref="Model.StepAtStart">Model.StepAtStart</see> and <see cref="Model.StepAtEnd">Model.StepAtEnd</see>).
        ///
        /// <b>Formal definition</b>
        ///
        /// Pulse creates a cumulative function which has the value:
        ///
        /// * <c>0</c> before <c>interval.start()</c>,
        /// * <c>height</c> between <c>interval.start()</c> and <c>interval.end()</c>,
        /// * <c>0</c> after <c>interval.end()</c>
        ///
        /// If <c>interval</c> is absent, the pulse is <c>0</c> everywhere.
        ///
        /// If <c>interval</c> has length 0, the pulse is also <c>0</c> everywhere, whatever its <c>height</c>, so it always satisfies a maximum capacity.
        ///
        /// The <c>height</c> can be a constant value or an expression. In particular, the <c>height</c> can be given by an <see cref="IntVar">IntVar</see>. In such a case, the <c>height</c> is unknown at the time of the model creation but is determined during the search.
        ///
        /// Note that the <c>interval</c> and the <c>height</c> may have different presence statuses (when the <c>height</c> is given by a variable or an expression). In this case, the pulse is present only if both the <c>interval</c> and the <c>height</c> are present. Therefore, it is helpful to constrain the <c>height</c> to have the same presence status as the <c>interval</c>.
        ///
        /// Cumulative functions can be combined using operators (<c>+</c>, <c>-</c>, unary <c>-</c>) and <see cref="Model.Sum">Model.Sum</see>. A cumulative function's minimum and maximum height can be constrained using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>).
        ///
        /// <para>Example:</para>
        /// <example>
        /// Let us consider a set of tasks and a group of 3 workers. Each task requires a certain number of workers (<c>demand</c>). Our goal is to schedule the tasks so that the length of the schedule (makespan) is minimal.
        ///
        /// <code>
        /// // The input data:
        /// int nbWorkers = 3;
        /// var tasks = new[] {
        ///     new { length = 10, demand = 3 },
        ///     new { length = 20, demand = 2 },
        ///     new { length = 15, demand = 1 },
        ///     new { length = 30, demand = 2 },
        ///     new { length = 20, demand = 1 },
        ///     new { length = 25, demand = 2 },
        ///     new { length = 10, demand = 1 },
        /// };
        ///
        /// var model = new Model();
        /// // A set of pulses, one for each task:
        /// var pulses = new List&lt;CumulExpr&gt;();
        /// // End times of the tasks:
        /// var ends = new List&lt;IntExpr&gt;();
        ///
        /// for (int i = 0; i &lt; tasks.Length; i++) {
        ///     // Create a task:
        ///     var task = model.IntervalVar(name: $"T{i + 1}", length: tasks[i].length);
        ///     // Create a pulse for the task:
        ///     pulses.Add(model.Pulse(task, tasks[i].demand));
        ///     // Store the end of the task:
        ///     ends.Add(task.End());
        /// }
        ///
        /// // The number of workers used at any time cannot exceed nbWorkers:
        /// model.Enforce(model.Sum(pulses) &lt;= nbWorkers);
        /// // Minimize the maximum of the ends (makespan):
        /// model.Minimize(model.Max(ends));
        ///
        /// var result = model.Solve(new Parameters { searchType = "FDS" });
        /// </code>
        /// </example><para>Example:</para>
        /// <example>
        /// In the following example, we create three interval variables <c>x</c>, <c>y</c>, and <c>z</c> that represent some tasks. Variables <c>x</c> and <c>y</c> are present, but variable <c>z</c> is optional. Each task requires a certain number of workers. The length of the task depends on the assigned number of workers. The number of assigned workers is modeled using integer variables <c>wx</c>, <c>wy</c>, and <c>wz</c>.
        ///
        /// There are 7 workers. Therefore, at any time, the sum of the workers assigned to the running tasks must be less or equal to 7.
        ///
        /// If the task <c>z</c> is absent, then the variable <c>wz</c> has no meaning, and therefore, it should also be absent.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x");
        /// var y = model.IntervalVar(name: "y");
        /// var z = model.IntervalVar(name: "z", optional: true);
        ///
        /// var wx = model.IntVar(min: 1, max: 5, name: "wx");
        /// var wy = model.IntVar(min: 1, max: 5, name: "wy");
        /// var wz = model.IntVar(min: 1, max: 5, name: "wz", optional: true);
        ///
        /// // wz is present if and only if z is present:
        /// model.Enforce(z.Presence() == wz.Presence());
        ///
        /// var px = model.Pulse(x, wx);
        /// var py = model.Pulse(y, wy);
        /// var pz = model.Pulse(z, wz);
        ///
        /// // There are at most 7 workers at any time:
        /// model.Enforce(model.Sum(new[] { px, py, pz }) &lt;= 7);
        ///
        /// // Length of the task depends on the number of workers using the following formula:
        /// //    length * wx = 12
        /// model.Enforce(x.Length() * wx == 12);
        /// model.Enforce(y.Length() * wy == 12);
        /// model.Enforce(z.Length() * wz == 12);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.Pulse">IntervalVar.Pulse</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="Model.StepAtStart">Model.StepAtStart</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="Model.StepAtEnd">Model.StepAtEnd</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="Model.StepAt">Model.StepAt</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="CumulExpr">CumulExpr</see> — for constraining cumulative functions using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>).</description></item>
        /// </list>
        /// </remarks>
        public CumulExpr Pulse(IntervalVar interval, IntExpr height)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), OptalCP.IntExpr.Wrap(height) };
            return new CumulExpr(this, "pulse", outParams);
        }

        /// <inheritdoc cref="Pulse(IntervalVar, IntExpr)"/>
        public CumulExpr Pulse(IntervalVar interval, int height)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), height };
            return new CumulExpr(this, "pulse", outParams);
        }

        /// <summary>
        /// Creates cumulative function (expression) that changes value at start of the interval variable by the given height.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <param name="height">The height value.</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// Cumulative _step_ functions could be used to model a resource that is consumed or produced and, therefore, changes in amount over time. Examples of such a resource are a battery, an account balance, a product's stock, etc.
        ///
        /// A <c>stepAtStart</c> can change the amount of such resource at the start of a given variable. The amount is changed by the given <c>height</c>, which can be positive or negative.
        ///
        /// The <c>height</c> can be a constant value or an expression. In particular, the <c>height</c> can be given by an <see cref="IntVar">IntVar</see>. In such a case, the <c>height</c> is unknown at the time of the model creation but is determined during the search.
        ///
        /// Note that the <c>interval</c> and the <c>height</c> may have different presence statuses (when the <c>height</c> is given by a variable or an expression). In this case, the step is present only if both the <c>interval</c> and the <c>height</c> are present. Therefore, it is helpful to constrain the <c>height</c> to have the same presence status as the <c>interval</c>.
        ///
        /// Cumulative steps can be combined using operators (<c>+</c>, <c>-</c>, unary <c>-</c>) and <see cref="Model.Sum">Model.Sum</see>. A cumulative function's minimum and maximum height can be constrained using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>).
        ///
        /// <b>Formal definition</b>
        ///
        /// stepAtStart creates a cumulative function which has the value:
        ///
        /// * <c>0</c> before <c>interval.start()</c>,
        /// * <c>height</c> after <c>interval.start()</c>.
        ///
        /// If the <c>interval</c> or the <c>height</c> is *absent*, the created cumulative function is <c>0</c> everywhere.
        ///
        /// Note: Combining _pulses_ (<see cref="Model.Pulse">Model.Pulse</see>) and _steps_ (<see cref="Model.StepAtStart">Model.StepAtStart</see>, <see cref="Model.StepAtEnd">Model.StepAtEnd</see>, <see cref="Model.StepAt">Model.StepAt</see>) is not supported yet.
        ///
        /// <para>Example:</para>
        /// <example>
        /// Let us consider a set of tasks. Each task either costs a certain amount of money or makes some money. Money is consumed at the start of a task and produced at the end. We have an initial <c>budget</c>, and we want to schedule the tasks so that we do not run out of money (i.e., the amount is always non-negative).
        ///
        /// Tasks cannot overlap. Our goal is to find the shortest schedule possible.
        ///
        /// <code>
        /// // The input data:
        /// int budget = 100;
        /// var tasksData = new[] {
        ///     (length: 10, money: -150), (length: 20, money:   40),
        ///     (length: 15, money:   20), (length: 30, money:  -10),
        ///     (length: 20, money:   30), (length: 25, money:  -20),
        ///     (length: 10, money:   10), (length: 20, money:   50),
        /// };
        ///
        /// var model = new Model();
        /// var taskVars = new List&lt;IntervalVar&gt;();
        /// var steps = new List&lt;CumulExpr&gt;();
        ///
        /// for (int i = 0; i &lt; tasksData.Length; i++) {
        ///     var interval = model.IntervalVar(name: $"T{i + 1}", length: tasksData[i].length);
        ///     taskVars.Add(interval);
        ///     if (tasksData[i].money &lt; 0)
        ///         steps.Add(model.StepAtStart(interval, tasksData[i].money));
        ///     else
        ///         steps.Add(model.StepAtEnd(interval, tasksData[i].money));
        /// }
        ///
        /// // The initial budget increases the cumul at time 0:
        /// steps.Add(model.StepAt(0, budget));
        /// // The money must be non-negative at any time:
        /// model.Enforce(model.Sum(steps) &gt;= 0);
        /// // Only one task at a time:
        /// model.NoOverlap(taskVars);
        ///
        /// // Minimize the maximum of the ends (makespan):
        /// model.Minimize(model.Max(taskVars.Select(t =&gt; t.End())));
        ///
        /// var result = model.Solve(new Parameters { searchType = "FDS" });
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StepAtStart">IntervalVar.StepAtStart</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="Model.StepAtEnd">Model.StepAtEnd</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="Model.StepAt">Model.StepAt</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="Model.Pulse">Model.Pulse</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="CumulExpr">CumulExpr</see> — for constraining cumulative functions using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>).</description></item>
        /// </list>
        /// </remarks>
        public CumulExpr StepAtStart(IntervalVar interval, IntExpr height)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), OptalCP.IntExpr.Wrap(height) };
            return new CumulExpr(this, "stepAtStart", outParams);
        }

        /// <inheritdoc cref="StepAtStart(IntervalVar, IntExpr)"/>
        public CumulExpr StepAtStart(IntervalVar interval, int height)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), height };
            return new CumulExpr(this, "stepAtStart", outParams);
        }

        /// <summary>
        /// Creates cumulative function (expression) that changes value at end of the interval variable by the given height.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <param name="height">The height value.</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// Cumulative _step_ functions could be used to model a resource that is consumed or produced and, therefore, changes in amount over time. Examples of such a resource are a battery, an account balance, a product's stock, etc.
        ///
        /// A <c>stepAtEnd</c> can change the amount of such resource at the end of a given variable. The amount is changed by the given <c>height</c>, which can be positive or negative.
        ///
        /// The <c>height</c> can be a constant value or an expression. In particular, the <c>height</c> can be given by an <see cref="IntVar">IntVar</see>. In such a case, the <c>height</c> is unknown at the time of the model creation but is determined during the search.
        ///
        /// Note that the <c>interval</c> and the <c>height</c> may have different presence statuses (when the <c>height</c> is given by a variable or an expression). In this case, the step is present only if both the <c>interval</c> and the <c>height</c> are present. Therefore, it is helpful to constrain the <c>height</c> to have the same presence status as the <c>interval</c>.
        ///
        /// Cumulative steps can be combined using operators (<c>+</c>, <c>-</c>, unary <c>-</c>) and <see cref="Model.Sum">Model.Sum</see>. A cumulative function's minimum and maximum height can be constrained using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>).
        ///
        /// <b>Formal definition</b>
        ///
        /// stepAtEnd creates a cumulative function which has the value:
        ///
        /// * <c>0</c> before <c>interval.end()</c>,
        /// * <c>height</c> after <c>interval.end()</c>.
        ///
        /// If the <c>interval</c> or the <c>height</c> is *absent*, the created cumulative function is <c>0</c> everywhere.
        ///
        /// Note: Combining _pulses_ (<see cref="Model.Pulse">Model.Pulse</see>) and _steps_ (<see cref="Model.StepAtStart">Model.StepAtStart</see>, <see cref="Model.StepAtEnd">Model.StepAtEnd</see>, <see cref="Model.StepAt">Model.StepAt</see>) is not supported yet.
        ///
        /// <para>Example:</para>
        /// <example>
        /// Let us consider a set of tasks. Each task either costs a certain amount of money or makes some money. Money is consumed at the start of a task and produced at the end. We have an initial <c>budget</c>, and we want to schedule the tasks so that we do not run out of money (i.e., the amount is always non-negative).
        ///
        /// Tasks cannot overlap. Our goal is to find the shortest schedule possible.
        ///
        /// <code>
        /// // The input data:
        /// int budget = 100;
        /// var tasksData = new[] {
        ///     (length: 10, money: -150), (length: 20, money:   40),
        ///     (length: 15, money:   20), (length: 30, money:  -10),
        ///     (length: 20, money:   30), (length: 25, money:  -20),
        ///     (length: 10, money:   10), (length: 20, money:   50),
        /// };
        ///
        /// var model = new Model();
        /// var taskVars = new List&lt;IntervalVar&gt;();
        /// var steps = new List&lt;CumulExpr&gt;();
        ///
        /// for (int i = 0; i &lt; tasksData.Length; i++) {
        ///     var interval = model.IntervalVar(name: $"T{i + 1}", length: tasksData[i].length);
        ///     taskVars.Add(interval);
        ///     if (tasksData[i].money &lt; 0)
        ///         steps.Add(model.StepAtStart(interval, tasksData[i].money));
        ///     else
        ///         steps.Add(model.StepAtEnd(interval, tasksData[i].money));
        /// }
        ///
        /// // The initial budget increases the cumul at time 0:
        /// steps.Add(model.StepAt(0, budget));
        /// // The money must be non-negative at any time:
        /// model.Enforce(model.Sum(steps) &gt;= 0);
        /// // Only one task at a time:
        /// model.NoOverlap(taskVars);
        ///
        /// // Minimize the maximum of the ends (makespan):
        /// model.Minimize(model.Max(taskVars.Select(t =&gt; t.End())));
        ///
        /// var result = model.Solve(new Parameters { searchType = "FDS" });
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StepAtEnd">IntervalVar.StepAtEnd</see> — is equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="Model.StepAtStart">Model.StepAtStart</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="Model.StepAt">Model.StepAt</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="Model.Pulse">Model.Pulse</see> — for other basic cumulative functions.</description></item>
        /// <item><description><see cref="CumulExpr">CumulExpr</see> — for constraining cumulative functions using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>).</description></item>
        /// </list>
        /// </remarks>
        public CumulExpr StepAtEnd(IntervalVar interval, IntExpr height)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), OptalCP.IntExpr.Wrap(height) };
            return new CumulExpr(this, "stepAtEnd", outParams);
        }

        /// <inheritdoc cref="StepAtEnd(IntervalVar, IntExpr)"/>
        public CumulExpr StepAtEnd(IntervalVar interval, int height)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), height };
            return new CumulExpr(this, "stepAtEnd", outParams);
        }

        /// <summary>
        /// Creates a cumulative function that changes value at a given point.
        /// </summary>
        /// <param name="x">The point at which the cumulative function changes value.</param>
        /// <param name="height">The height value (can be positive, negative, constant, or expression).</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// This function is similar to <see cref="Model.StepAtStart">Model.StepAtStart</see> and <see cref="Model.StepAtEnd">Model.StepAtEnd</see>, but the time of the change is given by the constant value <c>x</c> instead of by the start/end of an interval variable. The height can be a constant or an expression (e.g., created by <see cref="Model.IntVar">Model.IntVar</see>).
        ///
        /// <b>Formal definition</b>
        ///
        /// <c>stepAt</c> creates a cumulative function which has the value:
        ///
        /// * 0 before <c>x</c>,
        /// * <c>height</c> after <c>x</c>.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.StepAtStart">Model.StepAtStart</see> — for an example with <c>stepAt</c>.</description></item>
        /// <item><description><see cref="Model.StepAtEnd">Model.StepAtEnd</see> — for an example with <c>stepAt</c>.</description></item>
        /// <item><description><see cref="CumulExpr">CumulExpr</see> — for constraining cumulative functions using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>).</description></item>
        /// </list>
        /// </remarks>
        public CumulExpr StepAt(int x, IntExpr height)
        {
            var outParams = new List<object?>() { x, OptalCP.IntExpr.Wrap(height) };
            return new CumulExpr(this, "stepAt", outParams);
        }

        /// <inheritdoc cref="StepAt(int, IntExpr)"/>
        public CumulExpr StepAt(int x, int height)
        {
            var outParams = new List<object?>() { x, height };
            return new CumulExpr(this, "stepAt", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint CumulMaxProfile(CumulExpr cumul, IntStepFunction profile)
        {
            var outParams = new List<object?>() { OptalCP.CumulExpr.Wrap(cumul), OptalCP.IntStepFunction.Wrap(profile) };
            return new Constraint(this, "cumulMaxProfile", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint CumulMinProfile(CumulExpr cumul, IntStepFunction profile)
        {
            var outParams = new List<object?>() { OptalCP.CumulExpr.Wrap(cumul), OptalCP.IntStepFunction.Wrap(profile) };
            return new Constraint(this, "cumulMinProfile", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal CumulExpr CumulStairs(IEnumerable<CumulExpr> atoms)
        {
            var outParams = new List<object?>() { OptalCP.CumulExpr.WrapList(atoms) };
            return new CumulExpr(this, "cumulStairs", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint PrecedenceEnergyBefore(IntervalVar main, IEnumerable<IntervalVar> others, IEnumerable<int> heights, int capacity)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(main), OptalCP.IntervalVar.WrapList(others), ModelElement.WrapIntList(heights), capacity };
            return new Constraint(this, "precedenceEnergyBefore", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint PrecedenceEnergyAfter(IntervalVar main, IEnumerable<IntervalVar> others, IEnumerable<int> heights, int capacity)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(main), OptalCP.IntervalVar.WrapList(others), ModelElement.WrapIntList(heights), capacity };
            return new Constraint(this, "precedenceEnergyAfter", outParams);
        }

        /// <summary>
        /// Computes sum of values of the step function <c>func</c> over the interval <c>interval</c>.
        /// </summary>
        /// <param name="func">The step function.</param>
        /// <param name="interval">The interval variable.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// The sum is computed over all points in range <c>interval.start()</c> .. <c>interval.end()-1</c>. The sum includes the function value at the start time but not the value at the end time. If the interval variable has zero length, then the result is 0. If the interval variable is absent, then the result is <c>absent</c>.
        ///
        /// Requirement: The step function <c>func</c> must be non-negative.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntStepFunction.Integral">IntStepFunction.Integral</see> — for the equivalent function on <see cref="IntStepFunction">IntStepFunction</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Integral(IntStepFunction func, IntervalVar interval)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), OptalCP.IntervalVar.Wrap(interval) };
            return new IntExpr(this, "intStepFunctionIntegral", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionIntegralInRange(IntStepFunction func, IntervalVar interval, int lb, int ub)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), OptalCP.IntervalVar.Wrap(interval), lb, ub };
            return new Constraint(this, "intStepFunctionIntegralInRange", outParams);
        }

        /// <summary>
        /// Evaluates a step function at a given point.
        /// </summary>
        /// <param name="func">The step function.</param>
        /// <param name="arg">The point at which to evaluate the step function.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// The result is the value of the step function <c>func</c> at the point <c>arg</c>. If the value of <c>arg</c> is <c>absent</c>, then the result is also <c>absent</c>.
        ///
        /// By constraining the returned value, it is possible to limit <c>arg</c> to be only within certain segments of the segmented function. In particular, functions <see cref="Model.ForbidStart">Model.ForbidStart</see> and <see cref="Model.ForbidEnd">Model.ForbidEnd</see> work that way.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntStepFunction.Eval">IntStepFunction.Eval</see> — for the equivalent function on <see cref="IntStepFunction">IntStepFunction</see>.</description></item>
        /// <item><description><see cref="Model.ForbidStart">Model.ForbidStart</see> — are convenience functions built on top of <c>eval</c>.</description></item>
        /// <item><description><see cref="Model.ForbidEnd">Model.ForbidEnd</see> — are convenience functions built on top of <c>eval</c>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Eval(IntStepFunction func, IntExpr arg)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), OptalCP.IntExpr.Wrap(arg) };
            return new IntExpr(this, "intStepFunctionEval", outParams);
        }

        /// <inheritdoc cref="Eval(IntStepFunction, IntExpr)"/>
        public IntExpr Eval(IntStepFunction func, int arg)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), arg };
            return new IntExpr(this, "intStepFunctionEval", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalInRange(IntStepFunction func, IntExpr arg, int lb, int ub)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), OptalCP.IntExpr.Wrap(arg), lb, ub };
            return new Constraint(this, "intStepFunctionEvalInRange", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalInRange(IntStepFunction func, int arg, int lb, int ub)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), arg, lb, ub };
            return new Constraint(this, "intStepFunctionEvalInRange", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalNotInRange(IntStepFunction func, IntExpr arg, int lb, int ub)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), OptalCP.IntExpr.Wrap(arg), lb, ub };
            return new Constraint(this, "intStepFunctionEvalNotInRange", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalNotInRange(IntStepFunction func, int arg, int lb, int ub)
        {
            var outParams = new List<object?>() { OptalCP.IntStepFunction.Wrap(func), arg, lb, ub };
            return new Constraint(this, "intStepFunctionEvalNotInRange", outParams);
        }

        /// <summary>
        /// Forbid the interval variable to overlap with segments of the function where the value is zero.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <param name="func">The step function.</param>
        /// <returns>The constraint forbidding the extent (entire interval).</returns>
        /// <remarks>
        /// This function prevents the specified interval variable from overlapping with segments of the step function where the value is zero. That is, if $[s, e)$ is a segment of the step function where the value is zero, then the interval variable either ends before $s$ ($\mathtt{interval.end()} \le s$) or starts after $e$ ($e \le \mathtt{interval.start()}$).
        ///
        /// @example A production task that cannot overlap with scheduled maintenance windows:
        ///
        /// <code>
        /// var model = new Model();
        /// var production = model.IntervalVar(length: 3, name: "production");
        /// var availability = model.StepFunction(new[] {
        ///     (0, 1), (8, 0), (10, 1),
        /// });
        /// model.ForbidExtent(production, availability);
        /// model.Minimize(production.End());
        /// var result = model.Solve();
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.ForbidExtent">IntervalVar.ForbidExtent</see> — for the equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="Model.ForbidStart">Model.ForbidStart</see> — for similar functions that constrain the start/end of an interval variable.</description></item>
        /// <item><description><see cref="Model.ForbidEnd">Model.ForbidEnd</see> — for similar functions that constrain the start/end of an interval variable.</description></item>
        /// <item><description><see cref="Model.Eval">Model.Eval</see> — for evaluation of a step function.</description></item>
        /// </list>
        /// </remarks>
        public Constraint ForbidExtent(IntervalVar interval, IntStepFunction func)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), OptalCP.IntStepFunction.Wrap(func) };
            return new Constraint(this, "forbidExtent", outParams);
        }

        /// <summary>
        /// Constrains the start of the interval variable to be outside of the zero-height segments of the step function.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <param name="func">The step function.</param>
        /// <returns>The constraint forbidding the start point.</returns>
        /// <remarks>
        /// This function is equivalent to:
        ///
        /// <code>
        /// model.Enforce(model.Eval(func, interval.Start()) != 0);
        /// </code>
        ///
        /// I.e., the function value at the start of the interval variable cannot be zero.
        ///
        /// @example A factory task that can only start during work hours (excluding breaks):
        ///
        /// <code>
        /// var model = new Model();
        ///
        /// // A 2-hour task on a machine
        /// var task = model.IntervalVar(length: 2, name: "task");
        ///
        /// // Allowed start times: 1 = allowed, 0 = forbidden
        /// var allowedStarts = model.StepFunction(new[] {
        ///     (0, 0),   // Before 6h: forbidden
        ///     (6, 1),   // 6h: shift starts, allowed
        ///     (10, 0),  // 10h: break, forbidden
        ///     (11, 1),  // 11h: break ends, allowed
        ///     (14, 0),  // 14h: shift ends, forbidden
        /// });
        ///
        /// // Task cannot start when allowedStarts is 0
        /// model.ForbidStart(task, allowedStarts);
        /// model.Minimize(task.Start());
        ///
        /// var result = model.Solve();
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.ForbidStart">IntervalVar.ForbidStart</see> — for the equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="Model.ForbidEnd">Model.ForbidEnd</see> — for similar function that constrains end an interval variable.</description></item>
        /// <item><description><see cref="Model.Eval">Model.Eval</see> — for evaluation of a step function.</description></item>
        /// </list>
        /// </remarks>
        public Constraint ForbidStart(IntervalVar interval, IntStepFunction func)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), OptalCP.IntStepFunction.Wrap(func) };
            return new Constraint(this, "forbidStart", outParams);
        }

        /// <summary>
        /// Constrains the last time point of the interval variable (end - 1) to be outside of the zero-height segments of the step function.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <param name="func">The step function.</param>
        /// <returns>The constraint forbidding the end point.</returns>
        /// <remarks>
        /// This function is equivalent to:
        ///
        /// <code>
        /// model.Enforce(model.Eval(func, interval.End() - 1) != 0);
        /// </code>
        ///
        /// I.e., the function value at <c>end - 1</c> cannot be zero. The end of an interval
        /// variable is exclusive, so <c>end - 1</c> is the last time point the interval covers.
        /// A zero-length interval covers no time point, but the constraint still checks
        /// <c>end - 1</c> for it.
        ///
        /// The constraint cannot be used inside a boolean expression. To combine the
        /// condition with other conditions (e.g. in a disjunction), use the <c>eval</c> expression
        /// above instead.
        ///
        /// @example A delivery task that must complete during business hours (not during lunch break):
        ///
        /// <code>
        /// var model = new Model();
        ///
        /// // A 1-hour delivery task
        /// var delivery = model.IntervalVar(length: 1, name: "delivery");
        ///
        /// // The last hour of the delivery: 1 = allowed, 0 = forbidden
        /// var allowedEnds = model.StepFunction(new[] {
        ///     (0, 0),   // Before 9h: forbidden
        ///     (9, 1),   // 9h: business opens, allowed
        ///     (12, 0),  // 12h: lunch break, forbidden
        ///     (13, 1),  // 13h: lunch ends, allowed
        ///     (17, 0),  // 17h: business closes, forbidden
        /// });
        ///
        /// // Delivery cannot end at t when allowedEnds is 0 at t - 1
        /// model.ForbidEnd(delivery, allowedEnds);
        /// model.Minimize(delivery.End());
        ///
        /// var result = model.Solve();
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.ForbidEnd">IntervalVar.ForbidEnd</see> — for the equivalent function on <see cref="IntervalVar">IntervalVar</see>.</description></item>
        /// <item><description><see cref="Model.ForbidStart">Model.ForbidStart</see> — for similar function that constrains start an interval variable.</description></item>
        /// <item><description><see cref="Model.Eval">Model.Eval</see> — for evaluation of a step function.</description></item>
        /// </list>
        /// </remarks>
        public Constraint ForbidEnd(IntervalVar interval, IntStepFunction func)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(interval), OptalCP.IntStepFunction.Wrap(func) };
            return new Constraint(this, "forbidEnd", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr DisjunctiveIsBefore(IntervalVar x, IntervalVar y)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(x), OptalCP.IntervalVar.Wrap(y) };
            return new BoolExpr(this, "disjunctiveIsBefore", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint ItvPresenceChain(IEnumerable<IntervalVar> intervals)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(intervals) };
            return new Constraint(this, "itvPresenceChain", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint ItvPresenceChainWithCount(IEnumerable<IntervalVar> intervals, IntExpr count)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(intervals), OptalCP.IntExpr.Wrap(count) };
            return new Constraint(this, "itvPresenceChainWithCount", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint ItvPresenceChainWithCount(IEnumerable<IntervalVar> intervals, int count)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(intervals), count };
            return new Constraint(this, "itvPresenceChainWithCount", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint EndBeforeStartChain(IEnumerable<IntervalVar> intervals)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(intervals) };
            return new Constraint(this, "endBeforeStartChain", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StartBeforeStartChain(IEnumerable<IntervalVar> intervals)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(intervals) };
            return new Constraint(this, "startBeforeStartChain", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint EndBeforeEndChain(IEnumerable<IntervalVar> intervals)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.WrapList(intervals) };
            return new Constraint(this, "endBeforeEndChain", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentIntVar(IntExpr variable, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(variable), isLeft };
            return new SearchDecision(this, "decisionPresentIntVar", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentIntVar(int variable, bool isLeft)
        {
            var outParams = new List<object?>() { variable, isLeft };
            return new SearchDecision(this, "decisionPresentIntVar", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionAbsentIntVar(IntExpr variable, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(variable), isLeft };
            return new SearchDecision(this, "decisionAbsentIntVar", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionAbsentIntVar(int variable, bool isLeft)
        {
            var outParams = new List<object?>() { variable, isLeft };
            return new SearchDecision(this, "decisionAbsentIntVar", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentIntervalVar(IntervalVar variable, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), isLeft };
            return new SearchDecision(this, "decisionPresentIntervalVar", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionAbsentIntervalVar(IntervalVar variable, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), isLeft };
            return new SearchDecision(this, "decisionAbsentIntervalVar", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentLE(IntExpr variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionPresentLE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentLE(int variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { variable, bound, isLeft };
            return new SearchDecision(this, "decisionPresentLE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalGT(IntExpr variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionOptionalGT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalGT(int variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { variable, bound, isLeft };
            return new SearchDecision(this, "decisionOptionalGT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentGE(IntExpr variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionPresentGE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentGE(int variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { variable, bound, isLeft };
            return new SearchDecision(this, "decisionPresentGE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalLT(IntExpr variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionOptionalLT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalLT(int variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { variable, bound, isLeft };
            return new SearchDecision(this, "decisionOptionalLT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentStartLE(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionPresentStartLE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalStartGT(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionOptionalStartGT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentStartGE(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionPresentStartGE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalStartLT(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionOptionalStartLT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentEndLE(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionPresentEndLE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalEndGT(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionOptionalEndGT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentEndGE(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionPresentEndGE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalEndLT(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionOptionalEndLT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionPresentLengthLE(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionPresentLengthLE", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal SearchDecision DecisionOptionalLengthGT(IntervalVar variable, int bound, bool isLeft)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(variable), bound, isLeft };
            return new SearchDecision(this, "decisionOptionalLengthGT", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint NoGood(IEnumerable<SearchDecision> decisions)
        {
            var outParams = new List<object?>() { OptalCP.SearchDecision.WrapList(decisions) };
            return new Constraint(this, "noGood", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Directive Related(IntervalVar x, IntervalVar y)
        {
            var outParams = new List<object?>() { OptalCP.IntervalVar.Wrap(x), OptalCP.IntervalVar.Wrap(y) };
            return new Directive(this, "related", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint Pack(IEnumerable<IntExpr> load, IEnumerable<IntExpr> where, IEnumerable<int> sizes)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapList(load), OptalCP.IntExpr.WrapList(where), ModelElement.WrapIntList(sizes) };
            return new Constraint(this, "pack", outParams);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint Pack(IEnumerable<object> load, IEnumerable<object> where, IEnumerable<int> sizes)
        {
            var outParams = new List<object?>() { OptalCP.IntExpr.WrapMixedList(load), OptalCP.IntExpr.WrapMixedList(where), ModelElement.WrapIntList(sizes) };
            return new Constraint(this, "pack", outParams);
        }


    }
}
