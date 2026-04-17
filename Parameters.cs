// ===========================================================================
// OptalCP C# API - Parameters
// Version: 2026.3.0
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OptalCP
{
    /// <summary>
    /// Parameters specify how the solver should behave.
    /// </summary>
    /// <remarks>
    /// Parameters specify how the solver should behave.  For example, the
    /// number of workers (threads) to use, the time limit, etc.
    ///
    /// Parameters can be passed to the solver functions <see cref="Model.Solve">Model.Solve</see>
    /// and <see cref="Solver.Solve">Solver.Solve</see>.
    ///
    /// <para>Example:</para>
    /// <example>
    /// In the following example, we are using the _TimeLimit_ parameter to specify
    /// that the solver should stop after 5 minutes. We also specify that the solver
    /// should use 4 threads. Finally, we specify that the solver should use
    /// _FDS_ search (in all threads).
    ///
    /// <code>
    /// var parameters = new Parameters {
    ///     timeLimit = 300,    // In seconds, i.e. 5 minutes
    ///     nbWorkers = 4,     // Use 4 threads
    ///     searchType = "FDS",
    /// };
    /// var result = myModel.Solve(parameters);
    /// </code>
    ///
    /// <b>Worker-specific parameters</b>
    ///
    /// Some parameters can be specified differently for each worker.  For example,
    /// some workers may use _LNS_ search while others use _FDS_ search.  To specify
    /// worker-specific parameters, use the _workers_ parameter and pass an array
    /// of <see cref="WorkerParameters">WorkerParameters</see>.
    ///
    /// Not all parameters can be specified per worker. For example, _TimeLimit_ is a
    /// global parameter. See <see cref="WorkerParameters">WorkerParameters</see> for the list of parameters
    /// that can be specified per worker.
    ///
    /// If a parameter is not set specifically for a worker, the global value is used.
    /// </example><para>Example:</para>
    /// <example>
    /// In the following example, we are going to use 4 workers; two of them will run
    /// _FDS_ search and the remaining two will run _LNS_ search.  In addition, workers
    /// that use _FDS_ search will use increased propagation levels.
    ///
    /// <code>
    /// // Parameters for a worker that uses FDS search:
    /// var fdsWorker = new WorkerParameters {
    ///     searchType = "FDS",
    ///     noOverlapPropagationLevel = 4,
    ///     cumulPropagationLevel = 3,
    ///     reservoirPropagationLevel = 2,
    /// };
    /// // Global parameters:
    /// var parameters = new Parameters {
    ///     timeLimit = 60,
    ///     searchType = "LNS",
    ///     nbWorkers = 4,
    ///     workers = new List&lt;WorkerParameters&gt; { fdsWorker, fdsWorker },
    /// };
    /// var result = myModel.Solve(parameters);
    /// </code>
    ///
    /// </example>
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="WorkerParameters">WorkerParameters</see> — for worker-specific parameters.</description></item>
    /// </list>
    /// </remarks>
    public class Parameters
    {
        /// <summary>
        /// Parameters specify how the solver should behave.
        /// </summary>
        /// <remarks>
        /// Parameters specify how the solver should behave.  For example, the
        /// number of workers (threads) to use, the time limit, etc.
        ///
        /// Parameters can be passed to the solver functions <see cref="Model.Solve">Model.Solve</see>
        /// and <see cref="Solver.Solve">Solver.Solve</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we are using the _TimeLimit_ parameter to specify
        /// that the solver should stop after 5 minutes. We also specify that the solver
        /// should use 4 threads. Finally, we specify that the solver should use
        /// _FDS_ search (in all threads).
        ///
        /// <code>
        /// var parameters = new Parameters {
        ///     timeLimit = 300,    // In seconds, i.e. 5 minutes
        ///     nbWorkers = 4,     // Use 4 threads
        ///     searchType = "FDS",
        /// };
        /// var result = myModel.Solve(parameters);
        /// </code>
        ///
        /// <b>Worker-specific parameters</b>
        ///
        /// Some parameters can be specified differently for each worker.  For example,
        /// some workers may use _LNS_ search while others use _FDS_ search.  To specify
        /// worker-specific parameters, use the _workers_ parameter and pass an array
        /// of <see cref="WorkerParameters">WorkerParameters</see>.
        ///
        /// Not all parameters can be specified per worker. For example, _TimeLimit_ is a
        /// global parameter. See <see cref="WorkerParameters">WorkerParameters</see> for the list of parameters
        /// that can be specified per worker.
        ///
        /// If a parameter is not set specifically for a worker, the global value is used.
        /// </example><para>Example:</para>
        /// <example>
        /// In the following example, we are going to use 4 workers; two of them will run
        /// _FDS_ search and the remaining two will run _LNS_ search.  In addition, workers
        /// that use _FDS_ search will use increased propagation levels.
        ///
        /// <code>
        /// // Parameters for a worker that uses FDS search:
        /// var fdsWorker = new WorkerParameters {
        ///     searchType = "FDS",
        ///     noOverlapPropagationLevel = 4,
        ///     cumulPropagationLevel = 3,
        ///     reservoirPropagationLevel = 2,
        /// };
        /// // Global parameters:
        /// var parameters = new Parameters {
        ///     timeLimit = 60,
        ///     searchType = "LNS",
        ///     nbWorkers = 4,
        ///     workers = new List&lt;WorkerParameters&gt; { fdsWorker, fdsWorker },
        /// };
        /// var result = myModel.Solve(parameters);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="WorkerParameters">WorkerParameters</see> — for worker-specific parameters.</description></item>
        /// </list>
        /// </remarks>
        public Parameters() { }

        /// <summary>
        /// Whether to colorize output to the terminal
        /// </summary>
        /// <remarks>
        /// This parameter controls when terminal output is colorized. Possible values are:
        ///
        /// *  <c>Never</c>: don't colorize the output.
        /// *  <c>Auto</c>: colorize if the output is a supported terminal.
        /// *  <c>Always</c>: always colorize the output.
        ///
        /// The default value is <c>Auto</c>.
        /// </remarks>
        public string? color { get; set; }

        /// <summary>
        /// Number of threads dedicated to search
        /// </summary>
        /// <remarks>
        /// When this parameter is 0 (the default), the number of workers is determined the following way:
        ///
        ///  * If environment variable <c>OPTALCP_NB_WORKERS</c> is set, its value is used.
        ///  * Otherwise, all available cores are used.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? nbWorkers { get; set; }

        /// <summary>
        /// Number of helper threads
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _nbHelpers { get; set; }

        /// <summary>
        /// Preset configuration for solver parameters
        /// </summary>
        /// <remarks>
        /// Presets provide reasonable default values for multiple solver parameters at once. Instead of manually tuning individual parameters, you can select a preset that matches your problem characteristics. The solver will then configure search strategies and propagation levels appropriately.
        ///
        /// Available presets:
        ///
        /// - <c>Auto</c>: The solver automatically selects a preset based on problem size (the default). Problems with more than 100,000 variables use <c>Large</c>, otherwise <c>Default</c>.
        ///
        /// - <c>Default</c>: Balanced configuration for most problems. Uses maximum propagation levels and distributes workers across different search strategies: half use LNS, 3/8 use FDS, and the rest use FDSDual. This provides a good mix of exploration and exploitation.
        ///
        /// - <c>Large</c>: Optimized for big problems with more than 100,000 variables. Uses minimum propagation to reduce overhead, all workers use LNS search, and <see cref="Parameters.lnsMode">Parameters.lnsMode</see> is set to <c>Focused</c>. This trades propagation strength for scalability.
        ///
        /// Parameters affected by presets:
        ///
        /// The preset sets default values for the following parameters:
        ///
        /// - <see cref="Parameters.searchType">Parameters.searchType</see>: How workers are distributed across LNS, FDS, and FDSDual
        /// - <see cref="Parameters.lnsMode">Parameters.lnsMode</see>: LNS solution pool strategy (<c>Large</c> preset selects <c>Focused</c>)
        /// - <see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see>: Propagation strength for noOverlap constraints
        /// - <see cref="Parameters.cumulPropagationLevel">Parameters.cumulPropagationLevel</see>: Propagation strength for cumulative constraints
        ///
        /// When you explicitly set any of these parameters, your value takes precedence over the preset's default. This allows you to use a preset as a starting point and fine-tune specific parameters as needed.
        ///
        /// When to use presets:
        ///
        /// Presets are a good starting point for most problems. They are not guaranteed to be optimal for your specific problem, but they provide reasonable defaults that work well in practice. If you find that the default preset is not working well for your problem, consider:
        ///
        /// - Trying the <c>Large</c> preset for very big problems, even if they have fewer than 100,000 variables
        /// - Explicitly setting <see cref="Parameters.searchType">Parameters.searchType</see> to use a specific search strategy
        /// - Adjusting propagation levels based on your problem structure
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model ...
        ///
        /// // Use automatic preset selection
        /// var result = model.Solve();
        ///
        /// // Or explicitly select a preset for a large problem
        /// result = model.Solve(new Parameters { preset = "Large" });
        ///
        /// // Or use Default preset but override search type
        /// result = model.Solve(new Parameters { preset = "Default", searchType = "FDS" });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// <item><description><see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see> — for tuning noOverlap propagation.</description></item>
        /// <item><description><see cref="Parameters.cumulPropagationLevel">Parameters.cumulPropagationLevel</see> — for tuning cumulative propagation.</description></item>
        /// </list>
        /// </remarks>
        public string? preset { get; set; }

        /// <summary>
        /// Type of search to use
        /// </summary>
        /// <remarks>
        /// This parameter controls which search algorithm the solver uses. Different search types have different strengths:
        ///
        /// - <c>Auto</c>: Automatically determined based on the <see cref="Parameters.preset">Parameters.preset</see> (the default). With the <c>Default</c> preset, workers are distributed across LNS, FDS, and FDSDual. With the <c>Large</c> preset, all workers use LNS.
        ///
        /// - <c>LNS</c>: Large Neighborhood Search. Starts from an initial solution and iteratively improves it by relaxing and re-optimizing parts of the solution. Good for finding high-quality solutions quickly, especially on large problems. Works best when a good initial solution can be found.
        ///
        /// - <c>FDS</c>: Failure-Directed Search. A systematic search that learns from failures to guide exploration. Uses restarts with no-good learning. Often effective at proving optimality and works well with strong propagation.
        ///
        /// - <c>FDSDual</c>: Failure-Directed Search working on objective bounds. Similar to FDS but focuses on proving bounds on the objective value. Useful for optimization problems where you want to know how far from optimal your solutions are.
        ///
        /// - <c>SetTimes</c>: Depth-first set-times search (not restarted). A simple chronological search that assigns start times in order. Can be effective for tightly constrained problems but generally less robust than other methods.
        ///
        /// Interaction with presets:
        ///
        /// When <c>searchType</c> is set to <c>Auto</c>, the actual search type is determined by the <see cref="Parameters.preset">Parameters.preset</see>:
        ///
        /// - <c>Default</c> preset: Distributes workers across different search types. Half use LNS, 3/8 use FDS, and the rest use FDSDual. This portfolio approach provides robustness across different problem types.
        ///
        /// - <c>Large</c> preset: All workers use LNS. For very large problems, the overhead of systematic search methods like FDS becomes prohibitive, so LNS is used exclusively.
        ///
        /// If you explicitly set <c>searchType</c> to a specific value (not <c>Auto</c>), that value is used regardless of the preset.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model ...
        ///
        /// // Let the preset decide (default behavior)
        /// var result = model.Solve();
        ///
        /// // Or explicitly use FDS for systematic search
        /// result = model.Solve(new Parameters { searchType = "FDS" });
        ///
        /// // Or use LNS for quick solutions on large problems
        /// result = model.Solve(new Parameters { searchType = "LNS" });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of search and propagation.</description></item>
        /// <item><description><see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see> — which works well with FDS at higher levels.</description></item>
        /// </list>
        /// </remarks>
        public string? searchType { get; set; }

        /// <summary>
        /// Random seed
        /// </summary>
        /// <remarks>
        /// The solver breaks ties randomly using a pseudorandom number generator. This parameter sets the seed of the generator.
        ///
        /// Note that when <see cref="Parameters.nbWorkers">Parameters.nbWorkers</see> is more than 1 then there is also another source of randomness: the time it takes for a message to pass from one worker to another. Therefore with 1 worker the solver is deterministic (random behavior depends only on random seed). With more workers the solver is not deterministic.
        ///
        /// Even with the same random seed, the solver may behave differently on different platforms. This can be due to different implementations of certain functions such as <c>std::sort</c>.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>1</c>.
        ///
        /// <b>Environment variable <c>OPTALCP_RANDOM_SEED</c></b>
        ///
        /// When <c>randomSeed</c> is not explicitly set (i.e. it remains at its default value of 1), the solver checks the environment variable <c>OPTALCP_RANDOM_SEED</c>. If the variable is set:
        ///
        /// *  A numeric value (e.g. <c>42</c>) is used directly as the random seed.
        /// *  The special value <c>RANDOM</c> (case-insensitive) tells the solver to generate a time-based seed.
        ///
        /// This is useful for randomizing test runs without changing application code.
        /// </remarks>
        public long? randomSeed { get; set; }

        /// <summary>
        /// Level of the log
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of text the solver writes on standard output. The solver is completely silent when this option is set to 0.
        ///
        /// The parameter takes an integer value in range <c>0..3</c>.
        ///
        /// The default value is <c>2</c>.
        /// </remarks>
        public long? logLevel { get; set; }

        /// <summary>
        /// Level of warnings
        /// </summary>
        /// <remarks>
        /// This parameter controls the types of warnings the solver emits. When this parameter is set to 0 then no warnings are emitted.
        ///
        /// The parameter takes an integer value in range <c>0..3</c>.
        ///
        /// The default value is <c>2</c>.
        /// </remarks>
        public long? warningLevel { get; set; }

        /// <summary>
        /// How often to print log messages (in seconds)
        /// </summary>
        /// <remarks>
        /// When <see cref="Parameters.logLevel">Parameters.logLevel</see> &amp;ge; 2 then solver writes a log message every <c>logPeriod</c> seconds. The log message contains the current statistics about the solve: number of branches, number of fails, memory used, etc.
        ///
        /// The parameter takes a floating point value in range <c>0.01..Infinity</c>.
        ///
        /// The default value is <c>10</c>.
        /// </remarks>
        public double? logPeriod { get; set; }

        /// <summary>
        /// When on, the correctness of solutions is verified
        /// </summary>
        /// <remarks>
        /// Verification is an independent algorithm that checks whether all constraints in the model are satisfied (or absent), and that objective value was computed correctly. Verification is a somewhat redundant process as all solutions should be correct. Its purpose is to double-check and detect bugs in the solver.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? verifySolutions { get; set; }

        /// <summary>
        /// Whether to verify correctness of external solutions
        /// </summary>
        /// <remarks>
        /// External solutions can be passed to the solver as a warm start via <see cref="Model.Solve">Model.Solve</see>, or using <see cref="Solver.SendSolution">Solver.SendSolution</see> during the search. Normally, all external solutions are checked before they are used. However, the check may be time consuming, especially if too many external solutions are sent simultaneously. This parameter allows to turn the check off.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? verifyExternalSolutions { get; set; }

        /// <summary>
        /// The minimal amount of memory in kB for a single allocation
        /// </summary>
        /// <remarks>
        /// The solver allocates memory in blocks. This parameter sets the minimal size of a block. Larger blocks mean a higher risk of wasting memory. However, larger blocks may also lead to better performance, particularly when the size matches the page size supported by the operating system.
        ///
        /// The value of this parameter must be a power of 2.
        ///
        /// The default value of 2048 means 2MB, which means that up to ~12MB can be wasted per worker in the worst case.
        ///
        /// The parameter takes an integer value in range <c>4..1073741824</c>.
        ///
        /// The default value is <c>2048</c>.
        /// </remarks>
        public long? allocationBlockSize { get; set; }

        /// <summary>
        /// Timeout for solver process to exit after finishing
        /// </summary>
        /// <remarks>
        /// After the solver finishes, wait up to this many seconds for the process to exit. If it doesn't exit in time, it is silently killed.
        ///
        /// The parameter takes a floating point value in range <c>0.0..Infinity</c>.
        ///
        /// The default value is <c>3</c>.
        /// </remarks>
        public double? processExitTimeout { get; set; }

        /// <summary>
        /// Wall clock limit for execution in seconds
        /// </summary>
        /// <remarks>
        /// Caps the total wall-clock time spent by the solver. The timer starts as soon as the solve begins, and it includes presolve, search, and verification. When the limit is reached, all workers stop cooperatively. Leave it at the default <c>Infinity</c> to run without a time bound.
        ///
        /// The parameter takes a floating point value in range <c>0.0..Infinity</c>.
        ///
        /// The default value is <c>Infinity</c>.
        /// </remarks>
        public double? timeLimit { get; set; }

        /// <summary>
        /// Stop the search after the given number of solutions
        /// </summary>
        /// <remarks>
        /// Terminates the solve after the specified number of solutions have been found and reported.
        ///
        /// Automatic behavior (value 0):
        ///
        /// When set to 0 (the default), the limit is determined automatically based on the problem type:
        ///
        /// - Decision problems (no objective): The solver stops after finding the first solution. This is usually what you want for feasibility problems.
        ///
        /// - Optimization problems: No limit is applied. The solver continues searching for better solutions until it proves optimality, hits another limit (like <see cref="Parameters.timeLimit">Parameters.timeLimit</see>), or is stopped manually.
        ///
        /// Explicit values:
        ///
        /// You can set an explicit limit to control solution enumeration:
        ///
        /// - <c>1</c>: Stop after the first solution. Useful when you just need any feasible solution quickly, even for optimization problems.
        ///
        /// - <c>N &gt; 1</c>: Find up to N solutions. Useful for:
        ///    - Generating multiple alternative solutions for warm starts
        ///    - Enumerating all solutions to small problems
        ///    - Finding a diverse set of solutions for analysis
        ///
        /// Note on optimization problems:
        ///
        /// For optimization problems, only improving solutions are counted. If you set <c>solutionLimit=5</c>, the solver will stop after finding 5 solutions, each better than the previous. Non-improving solutions (which can occur during the search) are not counted toward the limit.
        ///
        /// Note on LNS and decision problems:
        ///
        /// When using LNS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) on decision problems (no objective), be aware that LNS may report duplicate solutions. LNS works by iteratively improving a solution, and for decision problems without an objective to guide the search, it may find the same solution multiple times. If you need unique solutions, consider using FDS search instead, or filter duplicates in your application code.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model ...
        ///
        /// // Automatic behavior (default)
        /// var result = model.Solve();
        ///
        /// // Stop after first solution
        /// result = model.Solve(new Parameters { solutionLimit = 1 });
        ///
        /// // Find up to 10 solutions for warm starts
        /// result = model.Solve(new Parameters { solutionLimit = 10 });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.timeLimit">Parameters.timeLimit</see> — for limiting solve time.</description></item>
        /// </list>
        /// </remarks>
        public long? solutionLimit { get; set; }

        /// <summary>
        /// Fail limit for each worker
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerFailLimit { get; set; }

        /// <summary>
        /// Branch limit for every worker
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerBranchLimit { get; set; }

        /// <summary>
        /// Stop the worker after the given number of solutions
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerSolutionLimit { get; set; }

        /// <summary>
        /// Stop the worker after given number of LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerLNSStepLimit { get; set; }

        /// <summary>
        /// Stop the worker after the given number of restarts
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerRestartLimit { get; set; }

        /// <summary>
        /// Stop the search when the gap is below the tolerance
        /// </summary>
        /// <remarks>
        /// The search is stopped if the absolute difference between the current solution
        /// value and current lower/upper bound is not bigger than the specified value.
        ///
        /// This parameter works together with <see cref="Parameters.relativeGapTolerance">Parameters.relativeGapTolerance</see> as an OR condition: the search stops when *either* the absolute gap or the relative gap is within tolerance.
        ///
        /// The parameter takes a floating point value.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public double? absoluteGapTolerance { get; set; }

        /// <summary>
        /// Stop the search when the gap is below the tolerance
        /// </summary>
        /// <remarks>
        /// The search is stopped if the relative difference between the current solution
        /// value and current lower/upper bound is not bigger than the specified value.
        ///
        /// This parameter works together with <see cref="Parameters.absoluteGapTolerance">Parameters.absoluteGapTolerance</see> as an OR condition: the search stops when *either* the absolute gap or the relative gap is within tolerance.
        ///
        /// The parameter takes a floating point value.
        ///
        /// The default value is <c>0.0001</c>.
        /// </remarks>
        public double? relativeGapTolerance { get; set; }

        /// <summary>
        /// How much to propagate noOverlap constraints
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for noOverlap constraints. Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
        ///
        /// Propagation levels:
        ///
        /// - Level 1: Basic timetable propagation only
        /// - Level 2: Adds detectable precedences algorithm
        /// - Level 3: Adds edge-finding reasoning
        /// - Level 4: Maximum propagation with all available algorithms
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the propagation level is determined automatically based on the <see cref="Parameters.preset">Parameters.preset</see>:
        ///
        /// - <c>Default</c> preset: Uses level 4 (maximum propagation)
        /// - <c>Large</c> preset: Uses level 1 (minimum propagation for scalability)
        ///
        /// Performance considerations:
        ///
        /// More propagation doesn't necessarily mean better overall performance. The trade-off depends on your problem:
        ///
        /// - Dense scheduling problems with many overlapping intervals often benefit from higher propagation levels because the extra pruning reduces the search space significantly.
        ///
        /// - Sparse problems or very large problems may perform better with lower propagation levels because the overhead of sophisticated algorithms outweighs the benefit.
        ///
        /// - FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) typically benefits from higher propagation levels because it relies on strong propagation to guide the search.
        ///
        /// If you're unsure, start with the automatic selection (level 0) and let the preset choose. You can then experiment with explicit levels if needed.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model with noOverlap constraints ...
        ///
        /// // Let the preset decide (default)
        /// var result = model.Solve();
        ///
        /// // Or use maximum propagation for dense problems
        /// result = model.Solve(new Parameters { noOverlapPropagationLevel = 4 });
        ///
        /// // Or use minimum propagation for very large problems
        /// result = model.Solve(new Parameters { noOverlapPropagationLevel = 1 });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for creating noOverlap constraints.</description></item>
        /// </list>
        /// </remarks>
        public long? noOverlapPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate constraints on cumul functions
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for cumulative constraints (e.g., <c>cumul &lt;= limit</c>) when used with a sum of <see cref="Model.Pulse">Model.Pulse</see> pulses.
        ///
        /// Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
        ///
        /// Propagation levels:
        ///
        /// - Level 1: Basic timetable propagation
        /// - Level 2: Adds time-table edge-finding
        /// - Level 3: Maximum propagation with all available algorithms
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the propagation level is determined automatically based on the <see cref="Parameters.preset">Parameters.preset</see>:
        ///
        /// - <c>Default</c> preset: Uses level 3 (maximum propagation)
        /// - <c>Large</c> preset: Uses level 1 (minimum propagation for scalability)
        ///
        /// Performance considerations:
        ///
        /// More propagation doesn't necessarily mean better overall performance. The trade-off depends on your problem:
        ///
        /// - Resource-constrained problems with tight capacity limits often benefit from higher propagation levels because cumulative reasoning can prune many infeasible assignments.
        ///
        /// - Problems with loose resource constraints may not benefit much from higher levels because the extra computation doesn't lead to significant pruning.
        ///
        /// - Very large problems may perform better with lower propagation levels because the overhead becomes prohibitive.
        ///
        /// - FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) typically benefits from higher propagation levels.
        ///
        /// If you're unsure, start with the automatic selection (level 0) and let the preset choose.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model with cumulative constraints ...
        ///
        /// // Let the preset decide (default)
        /// var result = model.Solve();
        ///
        /// // Or use maximum propagation for resource-constrained problems
        /// result = model.Solve(new Parameters { cumulPropagationLevel = 3 });
        ///
        /// // Or use minimum propagation for very large problems
        /// result = model.Solve(new Parameters { cumulPropagationLevel = 1 });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// <item><description><see cref="Model.Pulse">Model.Pulse</see> — for creating pulse contributions to cumulative functions.</description></item>
        /// </list>
        /// </remarks>
        public long? cumulPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate constraints on cumul functions
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for cumulative constraints (e.g., <c>cumul &lt;= limit</c>, <c>cumul &gt;= limit</c>) when used together with steps (<see cref="Model.StepAtStart">Model.StepAtStart</see>, <see cref="Model.StepAtEnd">Model.StepAtEnd</see>, <see cref="Model.StepAt">Model.StepAt</see>).
        /// The bigger the value, the more algorithms are used for propagation.
        /// It means that more time is spent by the propagation, and possibly more values are removed from domains.
        /// More propagation doesn't necessarily mean better performance.
        /// FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) usually benefits from higher propagation levels.
        ///
        /// The parameter takes an integer value in range <c>1..2</c>.
        ///
        /// The default value is <c>1</c>.
        /// </remarks>
        public long? reservoirPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate position expressions on noOverlap constraints
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for position expressions on noOverlap constraints.
        /// The bigger the value, the more algorithms are used for propagation.
        /// It means that more time is spent by the propagation, and possibly more values are removed from domains.
        /// However, more propagation doesn't necessarily mean better performance.
        /// FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) usually benefits from higher propagation levels.
        ///
        /// The parameter takes an integer value in range <c>1..3</c>.
        ///
        /// The default value is <c>2</c>.
        /// </remarks>
        public long? positionPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate integral expression
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for <see cref="Model.Integral">Model.Integral</see> expressions.
        /// In particular, it controls whether the propagation also affects the minimum and the maximum length of the associated interval variable:
        ///
        /// * <c>1</c>: The length is updated only once during initial constraint propagation.
        /// * <c>2</c>: The length is updated every time the expression is propagated.
        ///
        /// The parameter takes an integer value in range <c>1..2</c>.
        ///
        /// The default value is <c>1</c>.
        /// </remarks>
        public long? integralPropagationLevel { get; set; }

        /// <summary>
        /// Whether to use precedence energy propagation algorithm
        /// </summary>
        /// <remarks>
        /// Precedence energy algorithm improves propagation of precedence constraints when an interval has multiple predecessors (or successors) which use the same resource (noOverlap or cumulative constraint). In this case, the predecessors (or successors) may be in disjunction. Precedence energy algorithm can leverage this information and propagate the precedence constraint more aggressively.
        ///
        /// The parameter takes an integer value: <c>0</c> to disable, <c>1</c> to enable.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? usePrecedenceEnergy { get; set; }

        /// <summary>
        /// How much to propagate pack constraints
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _packPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate itvMapping constraint
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _itvMappingPropagationLevel { get; set; }

        /// <summary>
        /// Level of search trace
        /// </summary>
        /// <remarks>
        /// This parameter is available only in the development edition of the solver.
        ///
        /// When set to a value bigger than zero, the solver prints a trace of the search.
        /// The trace contains information about every choice taken by the solver.
        /// The higher the value, the more information is printed.
        ///
        /// The parameter takes an integer value in range <c>0..5</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? searchTraceLevel { get; set; }

        /// <summary>
        /// Level of propagation trace
        /// </summary>
        /// <remarks>
        /// This parameter is available only in the development edition of the solver.
        ///
        /// When set to a value bigger than zero, the solver prints a trace of the propagation,
        /// that is a line for every domain change.
        /// The higher the value, the more information is printed.
        ///
        /// The parameter takes an integer value in range <c>0..5</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? propagationTraceLevel { get; set; }

        /// <summary>
        /// Level of information trace
        /// </summary>
        /// <remarks>
        /// This parameter is available only in the development edition of the solver.
        ///
        /// When set to a value bigger than zero, the solver prints various high-level information.
        /// The higher the value, the more information is printed.
        ///
        /// The parameter takes an integer value in range <c>0..5</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? infoTraceLevel { get; set; }

        /// <summary>
        /// Initial rating for newly created choices
        /// </summary>
        /// <remarks>
        /// Default rating for newly created choices. Both left and right branches get the same rating.
        /// Choice is initially permuted so that bigger domain change is the left branch.
        ///
        /// The parameter takes a floating point value in range <c>0.0..2.0</c>.
        ///
        /// The default value is <c>0.5</c>.
        /// </remarks>
        public double? fdsInitialRating { get; set; }

        /// <summary>
        /// Weight of the reduction factor in rating computation
        /// </summary>
        /// <remarks>
        /// When computing the local rating of a branch, multiply reduction factor by the given weight.
        ///
        /// The parameter takes a floating point value in range <c>0.0..Infinity</c>.
        ///
        /// The default value is <c>1</c>.
        /// </remarks>
        public double? fdsReductionWeight { get; set; }

        /// <summary>
        /// Length of average rating computed for choices
        /// </summary>
        /// <remarks>
        /// For the computation of rating of a branch. Arithmetic average is used until the branch
        /// is taken at least FDSRatingAverageLength times. After that exponential moving average
        /// is used with parameter alpha = 1 - 1 / FDSRatingAverageLength.
        ///
        /// The parameter takes an integer value in range <c>0..254</c>.
        ///
        /// The default value is <c>25</c>.
        /// </remarks>
        public long? fdsRatingAverageLength { get; set; }

        /// <summary>
        /// When non-zero, alpha factor for rating updates
        /// </summary>
        /// <remarks>
        /// When this parameter is set to a non-zero, parameter FDSRatingAverageLength is ignored.
        /// Instead, the rating of a branch is computed as an exponential moving average with the given parameter alpha.
        ///
        /// The parameter takes a floating point value in range <c>0..1</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public double? fdsFixedAlpha { get; set; }

        /// <summary>
        /// Whether to compare the local rating with the average
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        ///  * <c>Off</c> (the default): No comparison is done.
        ///  * <c>Global</c>: Compare with the global average.
        ///  * <c>Depth</c>: Compare with the average on the current search depth
        ///
        /// Arithmetic average is used for global and depth averages.
        ///
        /// The default value is <c>Off</c>.
        /// </remarks>
        public string? fdsRatingAverageComparison { get; set; }

        /// <summary>
        /// Reduction factor R for rating computation
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        ///  * <c>Normal</c> (the default): Normal reduction factor.
        ///  * <c>Zero</c>: Factor is not used (it is 0 all the time).
        ///  * <c>Random</c>: A random number in the range [0,1] is used instead.
        ///
        /// The default value is <c>Normal</c>.
        /// </remarks>
        public string? fdsReductionFactor { get; set; }

        /// <summary>
        /// Whether always reuse closing choice
        /// </summary>
        /// <remarks>
        /// Most of the time, FDS reuses closing choice automatically. This parameter enforces it all the time.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? fdsReuseClosing { get; set; }

        /// <summary>
        /// Whether all initial choices have the same step length
        /// </summary>
        /// <remarks>
        /// When set, then initial choices generated on interval variables will have the same step size.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsUniformChoiceStep { get; set; }

        /// <summary>
        /// Choice step relative to average length
        /// </summary>
        /// <remarks>
        /// Ratio of initial choice step size to the minimum length of interval variable. When FDSUniformChoiceStep is set, this ratio is used to compute global choice step using the average of interval var length. When FDSUniformChoiceStep is not set, this ratio is used to compute the choice step for every interval var individually.
        ///
        /// The parameter takes a floating point value in range <c>0.0..Infinity</c>.
        ///
        /// The default value is <c>0.699999988079071</c>.
        /// </remarks>
        public double? fdsLengthStepRatio { get; set; }

        /// <summary>
        /// Maximum number of choices generated initially per a variable
        /// </summary>
        /// <remarks>
        /// Initial domains are often very large (e.g., <c>0..IntervalMax</c>). Therefore initial
        /// number of generated choices is limited: only choices near startMin are kept.
        ///
        /// The parameter takes an integer value in range <c>2..2147483647</c>.
        ///
        /// The default value is <c>90</c>.
        /// </remarks>
        public long? fdsMaxInitialChoicesPerVariable { get; set; }

        /// <summary>
        /// Domain split ratio when run out of choices
        /// </summary>
        /// <remarks>
        /// When all choices are decided, and a greedy algorithm cannot find a solution, then
        /// more choices are generated by splitting domains into the specified number of pieces.
        ///
        /// The parameter takes a floating point value in range <c>2.0..Infinity</c>.
        ///
        /// The default value is <c>7</c>.
        /// </remarks>
        public double? fdsAdditionalStepRatio { get; set; }

        /// <summary>
        /// Whether to generate choices on presence status
        /// </summary>
        /// <remarks>
        /// Choices on start time also include a choice on presence status. Therefore, dedicated choices on presence status only are not mandatory.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsPresenceStatusChoices { get; set; }

        /// <summary>
        /// Maximum number of initial choices on length of an interval variable
        /// </summary>
        /// <remarks>
        /// When non-zero, this parameter limits the number of initial choices generated on length of an interval variable.
        /// When zero (the default), no choices on length are generated.
        ///
        /// The parameter takes an integer value in range <c>0..2147483647</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? fdsMaxInitialLengthChoices { get; set; }

        /// <summary>
        /// Maximum step when generating initial choices for length of an interval variable
        /// </summary>
        /// <remarks>
        /// Steps between choices for length of an interval variable are never bigger than the specified value.
        ///
        /// The parameter takes an integer value in range <c>1..1073741823</c>.
        ///
        /// The default value is <c>1073741823</c>.
        /// </remarks>
        public long? fdsMinLengthChoiceStep { get; set; }

        /// <summary>
        /// Minimum step when generating choices for integer variables.
        /// </summary>
        /// <remarks>
        /// Steps between choices for integer variables are never smaller than the specified value.
        ///
        /// The parameter takes an integer value in range <c>1..1073741823</c>.
        ///
        /// The default value is <c>1073741823</c>.
        /// </remarks>
        public long? fdsMinIntVarChoiceStep { get; set; }

        /// <summary>
        /// Influence of event time to initial choice rating
        /// </summary>
        /// <remarks>
        /// When non-zero, the initial choice rating is influenced by the date of the choice.
        /// This way, very first choices in the search should be taken chronologically.
        ///
        /// The parameter takes a floating point value in range <c>0..1</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public double? fdsEventTimeInfluence { get; set; }

        /// <summary>
        /// How much to improve rating when both branches fail immediately
        /// </summary>
        /// <remarks>
        /// This parameter sets a bonus reward for a choice when both left and right branches fail immediately.
        /// Current rating of both branches is multiplied by the specified value.
        ///
        /// The parameter takes a floating point value in range <c>0..1</c>.
        ///
        /// The default value is <c>0.98</c>.
        /// </remarks>
        public double? fdsBothFailRewardFactor { get; set; }

        /// <summary>
        /// How often to chose a choice randomly
        /// </summary>
        /// <remarks>
        /// Probability that a choice is taken randomly. A randomly selected choice is not added to the search tree automatically. Instead, the choice is tried, its rating is updated,
        /// but it is added to the search tree only if one of the branches fails.
        /// The mechanism is similar to strong branching.
        ///
        /// The parameter takes a floating point value in range <c>0.0..0.99999</c>.
        ///
        /// The default value is <c>0.1</c>.
        /// </remarks>
        public double? fdsEpsilon { get; set; }

        /// <summary>
        /// Number of choices to try in strong branching
        /// </summary>
        /// <remarks>
        /// Strong branching means that instead of taking a choice with the best rating,
        /// we take the specified number (FDSStrongBranchingSize) of best choices,
        /// try them in dry-run mode, measure their local rating, and
        /// then chose the one with the best local rating.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>10</c>.
        /// </remarks>
        public long? fdsStrongBranchingSize { get; set; }

        /// <summary>
        /// Up-to what search depth apply strong branching
        /// </summary>
        /// <remarks>
        /// Strong branching is typically used in the root node. This parameter controls
        /// the maximum search depth when strong branching is used.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>6</c>.
        /// </remarks>
        public long? fdsStrongBranchingDepth { get; set; }

        /// <summary>
        /// How to choose the best choice in strong branching
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        /// * <c>Both</c>: Choose the the choice with best combined rating.
        /// * <c>Left</c> (the default): Choose the choice with the best rating of the left branch.
        /// * <c>Right</c>: Choose the choice with the best rating of the right branch.
        ///
        /// The default value is <c>Left</c>.
        /// </remarks>
        public string? fdsStrongBranchingCriterion { get; set; }

        /// <summary>
        /// Fail limit for the first restart
        /// </summary>
        /// <remarks>
        /// Failure-directed search is periodically restarted: explored part of the current search tree is turned into a no-good constraint, and the search starts again in the root node.
        /// This parameter specifies the size of the very first search tree (measured in number of failures).
        ///
        /// The parameter takes an integer value in range <c>1..9223372036854775807</c>.
        ///
        /// The default value is <c>100</c>.
        /// </remarks>
        public long? fdsInitialRestartLimit { get; set; }

        /// <summary>
        /// Restart strategy to use
        /// </summary>
        /// <remarks>
        /// This parameter specifies how the restart limit (maximum number of failures) changes from restart to restart.
        /// Possible values are:
        ///
        /// * <c>Geometric</c> (the default): After each restart, restart limit is multiplied by <see cref="Parameters.fdsRestartGrowthFactor">Parameters.fdsRestartGrowthFactor</see>.
        /// * <c>Nested</c>: Similar to <c>Geometric</c> but the limit is changed back to <see cref="Parameters.fdsInitialRestartLimit">Parameters.fdsInitialRestartLimit</see> each time a new maximum limit is reached.
        /// * <c>Luby</c>: Luby restart strategy is used. Parameter <see cref="Parameters.fdsRestartGrowthFactor">Parameters.fdsRestartGrowthFactor</see> is ignored.
        ///
        /// The default value is <c>Geometric</c>.
        /// </remarks>
        public string? fdsRestartStrategy { get; set; }

        /// <summary>
        /// Growth factor for fail limit after each restart
        /// </summary>
        /// <remarks>
        /// After each restart, the fail limit for the restart is multiplied by the specified factor.
        /// This parameter is ignored when <see cref="Parameters.fdsRestartStrategy">Parameters.fdsRestartStrategy</see> is <c>Luby</c>.
        ///
        /// The parameter takes a floating point value in range <c>1.0..Infinity</c>.
        ///
        /// The default value is <c>1.15</c>.
        /// </remarks>
        public double? fdsRestartGrowthFactor { get; set; }

        /// <summary>
        /// Truncate choice use counts after a restart to this value
        /// </summary>
        /// <remarks>
        /// The idea is that ratings learned in the previous restart are less valid in the new restart.
        /// Using this parameter, it is possible to truncate use counts on choices so that new local ratings will have bigger weights (when FDSFixedAlpha is not used).
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>255</c>.
        /// </remarks>
        public long? fdsMaxCounterAfterRestart { get; set; }

        /// <summary>
        /// Truncate choice use counts after a solution is found
        /// </summary>
        /// <remarks>
        /// Similar to <see cref="Parameters.fdsMaxCounterAfterRestart">Parameters.fdsMaxCounterAfterRestart</see>, this parameter allows truncating use counts on choices when a solution is found.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>255</c>.
        /// </remarks>
        public long? fdsMaxCounterAfterSolution { get; set; }

        /// <summary>
        /// Reset restart size after a solution is found (ignored in Luby)
        /// </summary>
        /// <remarks>
        /// When this parameter is set (the default), then restart limit is set back to <see cref="Parameters.fdsInitialRestartLimit">Parameters.fdsInitialRestartLimit</see> when a solution is found.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsResetRestartsAfterSolution { get; set; }

        /// <summary>
        /// Whether to use or not nogood constraints
        /// </summary>
        /// <remarks>
        /// By default, no-good constraint is generated after each restart. This parameter allows to turn no-good constraints off.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsUseNogoods { get; set; }

        /// <summary>
        /// Whether to freeze ratings after a proof is found
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsFreezeRatingsAfterProof { get; set; }

        /// <summary>
        /// Whether to continue after proof
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsContinueAfterProof { get; set; }

        /// <summary>
        /// How many times repeat the proof
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _fdsRepeatLimit { get; set; }

        /// <summary>
        /// Choose completely randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsCompletelyRandom { get; set; }

        /// <summary>
        /// Whether to generate choices for objective expression/variable
        /// </summary>
        /// <remarks>
        /// This option controls the generation of choices on the objective. It works regardless of the objective is given by an expression or a variable.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? fdsBranchOnObjective { get; set; }

        /// <summary>
        /// Whether to improve nogoods from FDS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsImproveNogoods { get; set; }

        /// <summary>
        /// Controls which side of a choice is explored first (considering the rating).
        /// </summary>
        /// <remarks>
        /// This option can take the following values:
        ///
        /// * <c>FailureFirst</c>: Explore the failure side first.
        /// * <c>FailureLast</c>: Explore the failure side last.
        /// * <c>Random</c>: Explore either side randomly.
        ///
        /// The default value is <c>FailureFirst</c>.
        /// </remarks>
        public string? fdsBranchOrdering { get; set; }

        /// <summary>
        /// Use SetTimes when out of choices
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsDiveBySetTimes { get; set; }

        /// <summary>
        /// A strategy to choose objective cuts during FDSDual search.
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        /// * <c>Minimum</c>: Always change the cut by the minimum amount.
        /// * <c>Random</c>: At each restart, randomly choose a value in range LB..UB. The default.
        /// * <c>Split</c>: Always split the current range LB..UB in half.
        ///
        /// The default value is <c>Random</c>.
        /// </remarks>
        public string? fdsDualStrategy { get; set; }

        /// <summary>
        /// Whether to reset ratings when a new LB is proved
        /// </summary>
        /// <remarks>
        /// When this parameter is on, and FDSDual proves a new lower bound, then all ratings are reset to default values.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? fdsDualResetRatings { get; set; }

        /// <summary>
        /// LNS solution pool strategy.
        /// </summary>
        /// <remarks>
        /// Controls how LNS manages its solution pool.
        ///
        /// - <c>Robust</c>: Maintain multiple solution tiers for diverse exploration (the default).
        ///   The solver keeps several solutions of varying quality and occasionally works on
        ///   improving worse solutions. This provides robustness against getting stuck in
        ///   local optima.
        ///
        /// - <c>Focused</c>: Concentrate all LNS effort on improving the best solution.
        ///   Secondary solution tiers are disabled, reducing memory and CPU overhead.
        ///   Recommended for large problems where maintaining multiple solutions is too
        ///   expensive.
        ///
        /// The <c>Large</c> <see cref="Parameters.preset">Parameters.preset</see> automatically selects <c>Focused</c> mode. If you
        /// explicitly set <c>lnsMode</c>, your value takes precedence over the preset.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model ...
        ///
        /// // Default: robust mode with multiple solution tiers
        /// var result = model.Solve();
        ///
        /// // Focused mode for large problems
        /// result = model.Solve(new Parameters { lnsMode = "Focused" });
        ///
        /// // Or use the Large preset, which enables Focused mode automatically
        /// result = model.Solve(new Parameters { preset = "Large" });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration based on problem size.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// </list>
        /// </remarks>
        public string? lnsMode { get; set; }

        /// <summary>
        /// NoOverlap propagation level for LNS initial phase
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsInitNoOverlapPropagationLevel { get; set; }

        /// <summary>
        /// Cumul propagation level for LNS initial phase
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsInitCumulPropagationLevel { get; set; }

        /// <summary>
        /// Initial fail limit in first solution phase (after 0 fails)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsFirstFailLimit { get; set; }

        /// <summary>
        /// How much to increase fail limit during first solution phase
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsFailLimitGrowthFactor { get; set; }

        /// <summary>
        /// Multiply fail limit for LNS steps by this number
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsFailLimitCoefficient { get; set; }

        /// <summary>
        /// Number of all-heuristic iterations after the first solution
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsIterationsAfterFirstSolution { get; set; }

        /// <summary>
        /// Whether to use aggressive SetTimes dominance during LNS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsAggressiveDominance { get; set; }

        /// <summary>
        /// How many times iterate on the current solution (unless new one is found)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsSameSolutionPeriod { get; set; }

        /// <summary>
        /// Number of best solutions that receive most attention
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTier1Size { get; set; }

        /// <summary>
        /// How many worse solutions to remember and sometimes work on
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTier2Size { get; set; }

        /// <summary>
        /// How many other solutions to remember
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTier3Size { get; set; }

        /// <summary>
        /// Probability to work on a solution from tier 2
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsTier2Effort { get; set; }

        /// <summary>
        /// Probability to work on a solution from tier 3
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsTier3Effort { get; set; }

        /// <summary>
        /// Multiplication factor to affect fail limit of every LNS step
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsStepFailLimitFactor { get; set; }

        /// <summary>
        /// How often apply objective cut in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsApplyCutProbability { get; set; }

        /// <summary>
        /// Probability to skip an LNS step when no improvement is possible
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsSkipNoImprovementProbability { get; set; }

        /// <summary>
        /// Maximum size of a model part considered for structure detection
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsSmallStructureLimit { get; set; }

        /// <summary>
        /// Whether to minimize resources in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsResourceOptimization { get; set; }

        /// <summary>
        /// Whether to restore absent interval variables in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsRestoreAbsentIntervals { get; set; }

        /// <summary>
        /// Whether to restore lengths of interval variables in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsRestoreIntervalLengths { get; set; }

        /// <summary>
        /// Always restore values of non-relaxed integer variables
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsRestoreIntVarValues { get; set; }

        /// <summary>
        /// Whether to always relax the objective variable in LNS neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsAlwaysRelaxObjective { get; set; }

        /// <summary>
        /// Use only the user-provided warm start as the initial solution in LNS
        /// </summary>
        /// <remarks>
        /// When this parameter is on, the solver will use only the user-specified warm start solution for the initial solution phase in LNS. If no warm start is provided, the solver will search for its own initial solution as usual.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? lnsUseWarmStartOnly { get; set; }

        /// <summary>
        /// Probability to chose heuristics randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsEpsilon { get; set; }

        /// <summary>
        /// Learning coefficient for heuristics
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsAlpha { get; set; }

        /// <summary>
        /// Temperature for heuristics SoftMax bandits
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsTemperature { get; set; }

        /// <summary>
        /// Whether to choose heuristics uniformly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsHeuristicsUniform { get; set; }

        /// <summary>
        /// Initial Q-values of all heuristics
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsInitialQ { get; set; }

        /// <summary>
        /// Probability to chose portions to relax randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionEpsilon { get; set; }

        /// <summary>
        /// Learning coefficient for portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionAlpha { get; set; }

        /// <summary>
        /// Temperature for portions SoftMax bandits
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionTemperature { get; set; }

        /// <summary>
        /// Whether to choose portions uniformly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsPortionUniform { get; set; }

        /// <summary>
        /// Initial Q-values of portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionInitialQ { get; set; }

        /// <summary>
        /// Limit for portions to relax that are considered big and used less often
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionHandicapLimit { get; set; }

        /// <summary>
        /// Relative probability to use handicapped portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionHandicapValue { get; set; }

        /// <summary>
        /// Initial Q-values of handicapped portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionHandicapInitialQ { get; set; }

        /// <summary>
        /// How to choose neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsNeighborhoodStrategy { get; set; }

        /// <summary>
        /// Probability to chose neighborhoods randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodEpsilon { get; set; }

        /// <summary>
        /// Learning coefficient for neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodAlpha { get; set; }

        /// <summary>
        /// Temperature for neighborhoods SoftMax bandits
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodTemperature { get; set; }

        /// <summary>
        /// Whether to choose neighborhoods uniformly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsNeighborhoodUniform { get; set; }

        /// <summary>
        /// Initial Q-values of neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodInitialQ { get; set; }

        /// <summary>
        /// How many dives before switching to normal LNS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsDivingLimit { get; set; }

        /// <summary>
        /// Diving fail limit compared to final first solution fail limit
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsDivingFailLimitRatio { get; set; }

        /// <summary>
        /// Dump learning data
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsLearningRun { get; set; }

        /// <summary>
        /// Don't accept better solutions (for learning)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsStayOnObjective { get; set; }

        /// <summary>
        /// Use FDS inside LNS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsFDS { get; set; }

        /// <summary>
        /// Freeze interval variables around the relaxed fragment
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string? _lnsFragmentFreezing { get; set; }

        /// <summary>
        /// How much to relax bounds of relaxed interval variables
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsRelaxSlack { get; set; }

        /// <summary>
        /// Size of the relaxed fragment is multiplied by this factor
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionMultiplier { get; set; }

        /// <summary>
        /// Which worker computes simple lower bound
        /// </summary>
        /// <remarks>
        /// Simple lower bound is a bound such that infeasibility of a better objective can be proved by propagation only (without the search). The given worker computes simple lower bound before it starts the normal search. If a worker with the given number doesn't exist, then the lower bound is not computed.
        ///
        /// The parameter takes an integer value in range <c>-1..2147483647</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? simpleLBWorker { get; set; }

        /// <summary>
        /// Maximum number of feasibility checks
        /// </summary>
        /// <remarks>
        /// Simple lower bound is computed by binary search for the best objective value that is not infeasible by propagation. This parameter limits the maximum number of iterations of the binary search. When the value is 0, then simple lower bound is not computed at all.
        ///
        /// The parameter takes an integer value in range <c>0..2147483647</c>.
        ///
        /// The default value is <c>2147483647</c>.
        /// </remarks>
        public long? simpleLBMaxIterations { get; set; }

        /// <summary>
        /// Number of shaving rounds
        /// </summary>
        /// <remarks>
        /// When non-zero, the solver shaves on variable domains to improve the lower bound. This parameter controls the number of shaving rounds.
        ///
        /// The parameter takes an integer value in range <c>0..2147483647</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? simpleLBShavingRounds { get; set; }

        /// <summary>
        /// Level of internal trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _debugTraceLevel { get; set; }

        /// <summary>
        /// Level of memory trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _memoryTraceLevel { get; set; }

        /// <summary>
        /// Addition internal propagation trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _propagationDetailTraceLevel { get; set; }

        /// <summary>
        /// Level of SetTimes trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _setTimesTraceLevel { get; set; }

        /// <summary>
        /// Trace messages between master and workers
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _communicationTraceLevel { get; set; }

        /// <summary>
        /// Level of presolve trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _presolveTraceLevel { get; set; }

        /// <summary>
        /// Level of conversion trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _conversionTraceLevel { get; set; }

        /// <summary>
        /// Trace creation of expressions in the engine
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _expressionBuilderTraceLevel { get; set; }

        /// <summary>
        /// Level of Memorization trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _memorizationTraceLevel { get; set; }

        /// <summary>
        /// Additional internal search trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _searchDetailTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for Failure-Directed Search
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _fdsTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for shaving
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _shavingTraceLevel { get; set; }

        /// <summary>
        /// Internal trace that dumps ratings of FDS choices
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _fdsRatingsTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for Large Neighborhood Search
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for heuristic replay
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _heuristicReplayTraceLevel { get; set; }

        /// <summary>
        /// Whether to allow (optimality) proofs by SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _allowSetTimesProofs { get; set; }

        /// <summary>
        /// Always use aggressive dominance rules in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _setTimesAggressiveDominance { get; set; }

        /// <summary>
        /// Coefficient for computation of the number of extends in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _setTimesExtendsCoef { get; set; }

        /// <summary>
        /// Strategy for selecting pulse and step heights in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string? _setTimesHeightStrategy { get; set; }

        /// <summary>
        /// Strategy for selecting itvMapping assignment in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string? _setTimesItvMappingStrategy { get; set; }

        /// <summary>
        /// Initial density for SetTimes (nb of intervals / time unit)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _setTimesInitDensity { get; set; }

        /// <summary>
        /// For how many iterations to use arithmetic average for density computation
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _setTimesDensityLength { get; set; }

        /// <summary>
        /// After how many iterations density is considered reliable
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _setTimesDensityReliabilityThreshold { get; set; }

        /// <summary>
        /// Bitset of disabled SetTimes heuristics
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsDisabledHeuristics { get; set; }

        /// <summary>
        /// Multiply nbRemainingExtends by this factor
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _setTimesNbExtendsFactor { get; set; }

        /// <summary>
        /// Maximum capacity to use low-capacity timetable algorithm
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _discreteLowCapacityLimit { get; set; }

        /// <summary>
        /// Objective value to start LNS training
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsTrainingObjectiveLimit { get; set; }

        /// <summary>
        /// Whether try to find related of absent intervals
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _posAbsentRelated { get; set; }

        /// <summary>
        /// Default size of CallbackBlock. Must be multiple of 8.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _defaultCallbackBlockSize { get; set; }

        /// <summary>
        /// Whether to peg reservoir during SetTimes search
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _useReservoirPegging { get; set; }

        /// <summary>
        /// Whether to use TimeNet
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _useTimeNet { get; set; }

        /// <summary>
        /// Number of variables to process after objective cut before processing TimeNet
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _timeNetVarsToPreprocess { get; set; }

        /// <summary>
        /// Number of last bits in priority for At precedences
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _timeNetSubPriorityBits { get; set; }

        /// <summary>
        /// Minimum chain length to replace precedences with chain constraint
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _minChainLength { get; set; }

        /// <summary>
        /// Sampling interval for the activity monitor (0 = disabled)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _monitorInterval { get; set; }

        /// <summary>
        /// Hide monitor report lines with global percentage below this threshold
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _monitorThreshold { get; set; }



        /// <summary>
        /// Where to write solver log output.
        /// </summary>
        /// <remarks>
        /// Controls where solver log messages, warnings, and errors are written during solving.
        ///
        /// - <see langword="null"/> (default): Write to console (<c>Console.Out</c>)
        /// - A <c>TextWriter</c> instance: Write to the provided writer
        ///
        /// Note that setting <c>printLog</c> to <see langword="null"/> only suppresses custom redirection; the solver
        /// still writes to console by default. The solver still emits <c>log</c>, <c>warning</c>, and <c>error</c>
        /// events that can be intercepted using callback properties (<see cref="Solver.OnLog">Solver.OnLog</see>,
        /// <see cref="Solver.OnWarning">Solver.OnWarning</see>, <see cref="Solver.OnError">Solver.OnError</see>). To reduce the amount of logging at
        /// the source, use <see cref="Parameters.logLevel">Parameters.logLevel</see>.
        ///
        /// ANSI colors: When writing to a stream, the solver automatically detects whether the stream supports colors by checking if it is a TTY. To override automatic detection, use the <see cref="Parameters.color">Parameters.color</see> parameter.
        ///
        /// If the output writer throws (e.g., a broken pipe), the solver stops as soon as possible.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// // Default - logs to console
        /// var result = model.Solve();
        ///
        /// // Write logs to a file
        /// using var logFile = new StreamWriter("solver.log");
        /// var result2 = model.Solve(new Parameters { printLog = logFile });
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.logLevel">Parameters.logLevel</see> — to control verbosity.</description></item>
        /// <item><description><see cref="Parameters.color">Parameters.color</see> — to override automatic color detection.</description></item>
        /// </list>
        /// </remarks>
        public TextWriter? printLog { get; set; }

        /// <summary>
        /// Path to the solver executable or WebSocket URL.
        /// </summary>
        /// <remarks>
        /// Specifies how to connect to the solver.
        ///
        /// The value should be a path to the <c>optalcp</c> executable (e.g., <c>/usr/bin/optalcp</c>).
        /// The API spawns the solver as a subprocess.
        ///
        /// If not specified, the solver is searched as described in <see cref="Solver.FindSolver">Solver.FindSolver</see>.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solver.FindSolver">Solver.FindSolver</see> — for solver discovery logic.</description></item>
        /// <item><description><see cref="Parameters.solverArgs">Parameters.solverArgs</see> — for additional subprocess arguments.</description></item>
        /// </list>
        /// </remarks>
        public string? solver { get; set; }

        /// <summary>
        /// Additional command-line arguments for the solver subprocess.
        /// </summary>
        /// <remarks>
        /// These arguments are passed directly to the solver subprocess when it is spawned.
        /// This parameter is only used in subprocess mode (not when connecting to a remote
        /// solver via WebSocket).
        ///
        /// This can be useful for debugging or passing special flags to the solver that are
        /// not exposed through the Parameters API.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build model ...
        ///
        /// // Pass custom arguments to the solver
        /// var result = model.Solve(new Parameters {
        ///     solverArgs = new[] { "--some-debug-flag" },
        ///     timeLimit = 60,
        /// });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.solver">Parameters.solver</see> — to specify a custom solver path.</description></item>
        /// </list>
        /// </remarks>
        public string[]? solverArgs { get; set; }

        /// <summary>
        /// Per-worker parameter overrides.
        /// </summary>
        /// <remarks>
        /// Each worker can have its own parameters. If a parameter is not specified
        /// for a worker, then the global value is used.
        ///
        /// Note that parameter <see cref="Parameters.nbWorkers">Parameters.nbWorkers</see> specifies the number of
        /// workers regardless of the length of this list.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="WorkerParameters">WorkerParameters</see> — for the list of parameters that can be set per worker.</description></item>
        /// </list>
        /// </remarks>
        public List<WorkerParameters>? workers { get; set; }

        internal Dictionary<string, object?> ToDict()
        {
            var result = new Dictionary<string, object?>();

            if (color != null)
            {
                var validValues = new[] { "Never", "Auto", "Always" };
                if (!validValues.Contains(color, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{color}' for parameter Color");
                result["color"] = color;
            }

            if (nbWorkers != null)
            {
                result["nbWorkers"] = nbWorkers;
            }

            if (_nbHelpers != null)
            {
                result["_nbHelpers"] = _nbHelpers;
            }

            if (preset != null)
            {
                var validValues = new[] { "Auto", "Default", "Large" };
                if (!validValues.Contains(preset, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{preset}' for parameter Preset");
                result["preset"] = preset;
            }

            if (searchType != null)
            {
                var validValues = new[] { "Auto", "LNS", "FDS", "FDSDual", "SetTimes", "FDSLB" };
                if (!validValues.Contains(searchType, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{searchType}' for parameter SearchType");
                result["searchType"] = searchType;
            }

            if (randomSeed != null)
            {
                result["randomSeed"] = randomSeed;
            }

            if (logLevel != null)
            {
                if (logLevel < 0 || logLevel > 3)
                    throw new ArgumentException($"Value {logLevel} for parameter LogLevel is out of range [0, 3]");
                result["logLevel"] = logLevel;
            }

            if (warningLevel != null)
            {
                if (warningLevel < 0 || warningLevel > 3)
                    throw new ArgumentException($"Value {warningLevel} for parameter WarningLevel is out of range [0, 3]");
                result["warningLevel"] = warningLevel;
            }

            if (logPeriod != null)
            {
                if (double.IsPositiveInfinity(logPeriod.Value))
                    result["logPeriod"] = "Infinity";
                else if (double.IsNegativeInfinity(logPeriod.Value))
                    result["logPeriod"] = "-Infinity";
                else
                    result["logPeriod"] = logPeriod;
            }

            if (verifySolutions != null)
            {
                result["verifySolutions"] = verifySolutions;
            }

            if (verifyExternalSolutions != null)
            {
                result["verifyExternalSolutions"] = verifyExternalSolutions;
            }

            if (allocationBlockSize != null)
            {
                if (allocationBlockSize < 4 || allocationBlockSize > 1073741824)
                    throw new ArgumentException($"Value {allocationBlockSize} for parameter AllocationBlockSize is out of range [4, 1073741824]");
                result["allocationBlockSize"] = allocationBlockSize;
            }

            if (processExitTimeout != null)
            {
                if (double.IsPositiveInfinity(processExitTimeout.Value))
                    result["processExitTimeout"] = "Infinity";
                else if (double.IsNegativeInfinity(processExitTimeout.Value))
                    result["processExitTimeout"] = "-Infinity";
                else
                    result["processExitTimeout"] = processExitTimeout;
            }

            if (timeLimit != null)
            {
                if (double.IsPositiveInfinity(timeLimit.Value))
                    result["timeLimit"] = "Infinity";
                else if (double.IsNegativeInfinity(timeLimit.Value))
                    result["timeLimit"] = "-Infinity";
                else
                    result["timeLimit"] = timeLimit;
            }

            if (solutionLimit != null)
            {
                result["solutionLimit"] = solutionLimit;
            }

            if (_workerFailLimit != null)
            {
                result["_workerFailLimit"] = _workerFailLimit;
            }

            if (_workerBranchLimit != null)
            {
                result["_workerBranchLimit"] = _workerBranchLimit;
            }

            if (_workerSolutionLimit != null)
            {
                result["_workerSolutionLimit"] = _workerSolutionLimit;
            }

            if (_workerLNSStepLimit != null)
            {
                result["_workerLNSStepLimit"] = _workerLNSStepLimit;
            }

            if (_workerRestartLimit != null)
            {
                result["_workerRestartLimit"] = _workerRestartLimit;
            }

            if (absoluteGapTolerance != null)
            {
                if (double.IsPositiveInfinity(absoluteGapTolerance.Value))
                    result["absoluteGapTolerance"] = "Infinity";
                else if (double.IsNegativeInfinity(absoluteGapTolerance.Value))
                    result["absoluteGapTolerance"] = "-Infinity";
                else
                    result["absoluteGapTolerance"] = absoluteGapTolerance;
            }

            if (relativeGapTolerance != null)
            {
                if (double.IsPositiveInfinity(relativeGapTolerance.Value))
                    result["relativeGapTolerance"] = "Infinity";
                else if (double.IsNegativeInfinity(relativeGapTolerance.Value))
                    result["relativeGapTolerance"] = "-Infinity";
                else
                    result["relativeGapTolerance"] = relativeGapTolerance;
            }

            if (noOverlapPropagationLevel != null)
            {
                if (noOverlapPropagationLevel < 0 || noOverlapPropagationLevel > 4)
                    throw new ArgumentException($"Value {noOverlapPropagationLevel} for parameter NoOverlapPropagationLevel is out of range [0, 4]");
                result["noOverlapPropagationLevel"] = noOverlapPropagationLevel;
            }

            if (cumulPropagationLevel != null)
            {
                if (cumulPropagationLevel < 0 || cumulPropagationLevel > 4)
                    throw new ArgumentException($"Value {cumulPropagationLevel} for parameter CumulPropagationLevel is out of range [0, 4]");
                result["cumulPropagationLevel"] = cumulPropagationLevel;
            }

            if (reservoirPropagationLevel != null)
            {
                if (reservoirPropagationLevel < 1 || reservoirPropagationLevel > 2)
                    throw new ArgumentException($"Value {reservoirPropagationLevel} for parameter ReservoirPropagationLevel is out of range [1, 2]");
                result["reservoirPropagationLevel"] = reservoirPropagationLevel;
            }

            if (positionPropagationLevel != null)
            {
                if (positionPropagationLevel < 1 || positionPropagationLevel > 3)
                    throw new ArgumentException($"Value {positionPropagationLevel} for parameter PositionPropagationLevel is out of range [1, 3]");
                result["positionPropagationLevel"] = positionPropagationLevel;
            }

            if (integralPropagationLevel != null)
            {
                if (integralPropagationLevel < 1 || integralPropagationLevel > 2)
                    throw new ArgumentException($"Value {integralPropagationLevel} for parameter IntegralPropagationLevel is out of range [1, 2]");
                result["integralPropagationLevel"] = integralPropagationLevel;
            }

            if (usePrecedenceEnergy != null)
            {
                if (usePrecedenceEnergy < 0 || usePrecedenceEnergy > 1)
                    throw new ArgumentException($"Value {usePrecedenceEnergy} for parameter UsePrecedenceEnergy is out of range [0, 1]");
                result["usePrecedenceEnergy"] = usePrecedenceEnergy;
            }

            if (_packPropagationLevel != null)
            {
                if (_packPropagationLevel < 1 || _packPropagationLevel > 2)
                    throw new ArgumentException($"Value {_packPropagationLevel} for parameter PackPropagationLevel is out of range [1, 2]");
                result["_packPropagationLevel"] = _packPropagationLevel;
            }

            if (_itvMappingPropagationLevel != null)
            {
                if (_itvMappingPropagationLevel < 1 || _itvMappingPropagationLevel > 2)
                    throw new ArgumentException($"Value {_itvMappingPropagationLevel} for parameter ItvMappingPropagationLevel is out of range [1, 2]");
                result["_itvMappingPropagationLevel"] = _itvMappingPropagationLevel;
            }

            if (searchTraceLevel != null)
            {
                if (searchTraceLevel < 0 || searchTraceLevel > 5)
                    throw new ArgumentException($"Value {searchTraceLevel} for parameter SearchTraceLevel is out of range [0, 5]");
                result["searchTraceLevel"] = searchTraceLevel;
            }

            if (propagationTraceLevel != null)
            {
                if (propagationTraceLevel < 0 || propagationTraceLevel > 5)
                    throw new ArgumentException($"Value {propagationTraceLevel} for parameter PropagationTraceLevel is out of range [0, 5]");
                result["propagationTraceLevel"] = propagationTraceLevel;
            }

            if (infoTraceLevel != null)
            {
                if (infoTraceLevel < 0 || infoTraceLevel > 5)
                    throw new ArgumentException($"Value {infoTraceLevel} for parameter InfoTraceLevel is out of range [0, 5]");
                result["infoTraceLevel"] = infoTraceLevel;
            }

            if (fdsInitialRating != null)
            {
                if (fdsInitialRating < 0.000000 || fdsInitialRating > 2.000000)
                    throw new ArgumentException($"Value {fdsInitialRating} for parameter FDSInitialRating is out of range [0.000000, 2.000000]");
                if (double.IsPositiveInfinity(fdsInitialRating.Value))
                    result["fdsInitialRating"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsInitialRating.Value))
                    result["fdsInitialRating"] = "-Infinity";
                else
                    result["fdsInitialRating"] = fdsInitialRating;
            }

            if (fdsReductionWeight != null)
            {
                if (double.IsPositiveInfinity(fdsReductionWeight.Value))
                    result["fdsReductionWeight"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsReductionWeight.Value))
                    result["fdsReductionWeight"] = "-Infinity";
                else
                    result["fdsReductionWeight"] = fdsReductionWeight;
            }

            if (fdsRatingAverageLength != null)
            {
                if (fdsRatingAverageLength < 0 || fdsRatingAverageLength > 254)
                    throw new ArgumentException($"Value {fdsRatingAverageLength} for parameter FDSRatingAverageLength is out of range [0, 254]");
                result["fdsRatingAverageLength"] = fdsRatingAverageLength;
            }

            if (fdsFixedAlpha != null)
            {
                if (fdsFixedAlpha < 0 || fdsFixedAlpha > 1)
                    throw new ArgumentException($"Value {fdsFixedAlpha} for parameter FDSFixedAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(fdsFixedAlpha.Value))
                    result["fdsFixedAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsFixedAlpha.Value))
                    result["fdsFixedAlpha"] = "-Infinity";
                else
                    result["fdsFixedAlpha"] = fdsFixedAlpha;
            }

            if (fdsRatingAverageComparison != null)
            {
                var validValues = new[] { "Off", "Global", "Depth" };
                if (!validValues.Contains(fdsRatingAverageComparison, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsRatingAverageComparison}' for parameter FDSRatingAverageComparison");
                result["fdsRatingAverageComparison"] = fdsRatingAverageComparison;
            }

            if (fdsReductionFactor != null)
            {
                var validValues = new[] { "Normal", "Zero", "Random" };
                if (!validValues.Contains(fdsReductionFactor, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsReductionFactor}' for parameter FDSReductionFactor");
                result["fdsReductionFactor"] = fdsReductionFactor;
            }

            if (fdsReuseClosing != null)
            {
                result["fdsReuseClosing"] = fdsReuseClosing;
            }

            if (fdsUniformChoiceStep != null)
            {
                result["fdsUniformChoiceStep"] = fdsUniformChoiceStep;
            }

            if (fdsLengthStepRatio != null)
            {
                if (double.IsPositiveInfinity(fdsLengthStepRatio.Value))
                    result["fdsLengthStepRatio"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsLengthStepRatio.Value))
                    result["fdsLengthStepRatio"] = "-Infinity";
                else
                    result["fdsLengthStepRatio"] = fdsLengthStepRatio;
            }

            if (fdsMaxInitialChoicesPerVariable != null)
            {
                if (fdsMaxInitialChoicesPerVariable < 2 || fdsMaxInitialChoicesPerVariable > 2147483647)
                    throw new ArgumentException($"Value {fdsMaxInitialChoicesPerVariable} for parameter FDSMaxInitialChoicesPerVariable is out of range [2, 2147483647]");
                result["fdsMaxInitialChoicesPerVariable"] = fdsMaxInitialChoicesPerVariable;
            }

            if (fdsAdditionalStepRatio != null)
            {
                if (double.IsPositiveInfinity(fdsAdditionalStepRatio.Value))
                    result["fdsAdditionalStepRatio"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsAdditionalStepRatio.Value))
                    result["fdsAdditionalStepRatio"] = "-Infinity";
                else
                    result["fdsAdditionalStepRatio"] = fdsAdditionalStepRatio;
            }

            if (fdsPresenceStatusChoices != null)
            {
                result["fdsPresenceStatusChoices"] = fdsPresenceStatusChoices;
            }

            if (fdsMaxInitialLengthChoices != null)
            {
                if (fdsMaxInitialLengthChoices < 0 || fdsMaxInitialLengthChoices > 2147483647)
                    throw new ArgumentException($"Value {fdsMaxInitialLengthChoices} for parameter FDSMaxInitialLengthChoices is out of range [0, 2147483647]");
                result["fdsMaxInitialLengthChoices"] = fdsMaxInitialLengthChoices;
            }

            if (fdsMinLengthChoiceStep != null)
            {
                if (fdsMinLengthChoiceStep < 1 || fdsMinLengthChoiceStep > 1073741823)
                    throw new ArgumentException($"Value {fdsMinLengthChoiceStep} for parameter FDSMinLengthChoiceStep is out of range [1, 1073741823]");
                result["fdsMinLengthChoiceStep"] = fdsMinLengthChoiceStep;
            }

            if (fdsMinIntVarChoiceStep != null)
            {
                if (fdsMinIntVarChoiceStep < 1 || fdsMinIntVarChoiceStep > 1073741823)
                    throw new ArgumentException($"Value {fdsMinIntVarChoiceStep} for parameter FDSMinIntVarChoiceStep is out of range [1, 1073741823]");
                result["fdsMinIntVarChoiceStep"] = fdsMinIntVarChoiceStep;
            }

            if (fdsEventTimeInfluence != null)
            {
                if (fdsEventTimeInfluence < 0 || fdsEventTimeInfluence > 1)
                    throw new ArgumentException($"Value {fdsEventTimeInfluence} for parameter FDSEventTimeInfluence is out of range [0, 1]");
                if (double.IsPositiveInfinity(fdsEventTimeInfluence.Value))
                    result["fdsEventTimeInfluence"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsEventTimeInfluence.Value))
                    result["fdsEventTimeInfluence"] = "-Infinity";
                else
                    result["fdsEventTimeInfluence"] = fdsEventTimeInfluence;
            }

            if (fdsBothFailRewardFactor != null)
            {
                if (fdsBothFailRewardFactor < 0 || fdsBothFailRewardFactor > 1)
                    throw new ArgumentException($"Value {fdsBothFailRewardFactor} for parameter FDSBothFailRewardFactor is out of range [0, 1]");
                if (double.IsPositiveInfinity(fdsBothFailRewardFactor.Value))
                    result["fdsBothFailRewardFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsBothFailRewardFactor.Value))
                    result["fdsBothFailRewardFactor"] = "-Infinity";
                else
                    result["fdsBothFailRewardFactor"] = fdsBothFailRewardFactor;
            }

            if (fdsEpsilon != null)
            {
                if (fdsEpsilon < 0.000000 || fdsEpsilon > 0.999990)
                    throw new ArgumentException($"Value {fdsEpsilon} for parameter FDSEpsilon is out of range [0.000000, 0.999990]");
                if (double.IsPositiveInfinity(fdsEpsilon.Value))
                    result["fdsEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsEpsilon.Value))
                    result["fdsEpsilon"] = "-Infinity";
                else
                    result["fdsEpsilon"] = fdsEpsilon;
            }

            if (fdsStrongBranchingSize != null)
            {
                result["fdsStrongBranchingSize"] = fdsStrongBranchingSize;
            }

            if (fdsStrongBranchingDepth != null)
            {
                result["fdsStrongBranchingDepth"] = fdsStrongBranchingDepth;
            }

            if (fdsStrongBranchingCriterion != null)
            {
                var validValues = new[] { "Both", "Left", "Right" };
                if (!validValues.Contains(fdsStrongBranchingCriterion, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsStrongBranchingCriterion}' for parameter FDSStrongBranchingCriterion");
                result["fdsStrongBranchingCriterion"] = fdsStrongBranchingCriterion;
            }

            if (fdsInitialRestartLimit != null)
            {
                result["fdsInitialRestartLimit"] = fdsInitialRestartLimit;
            }

            if (fdsRestartStrategy != null)
            {
                var validValues = new[] { "Geometric", "Nested", "Luby" };
                if (!validValues.Contains(fdsRestartStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsRestartStrategy}' for parameter FDSRestartStrategy");
                result["fdsRestartStrategy"] = fdsRestartStrategy;
            }

            if (fdsRestartGrowthFactor != null)
            {
                if (double.IsPositiveInfinity(fdsRestartGrowthFactor.Value))
                    result["fdsRestartGrowthFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsRestartGrowthFactor.Value))
                    result["fdsRestartGrowthFactor"] = "-Infinity";
                else
                    result["fdsRestartGrowthFactor"] = fdsRestartGrowthFactor;
            }

            if (fdsMaxCounterAfterRestart != null)
            {
                result["fdsMaxCounterAfterRestart"] = fdsMaxCounterAfterRestart;
            }

            if (fdsMaxCounterAfterSolution != null)
            {
                result["fdsMaxCounterAfterSolution"] = fdsMaxCounterAfterSolution;
            }

            if (fdsResetRestartsAfterSolution != null)
            {
                result["fdsResetRestartsAfterSolution"] = fdsResetRestartsAfterSolution;
            }

            if (fdsUseNogoods != null)
            {
                result["fdsUseNogoods"] = fdsUseNogoods;
            }

            if (_fdsFreezeRatingsAfterProof != null)
            {
                result["_fdsFreezeRatingsAfterProof"] = _fdsFreezeRatingsAfterProof;
            }

            if (_fdsContinueAfterProof != null)
            {
                result["_fdsContinueAfterProof"] = _fdsContinueAfterProof;
            }

            if (_fdsRepeatLimit != null)
            {
                result["_fdsRepeatLimit"] = _fdsRepeatLimit;
            }

            if (_fdsCompletelyRandom != null)
            {
                result["_fdsCompletelyRandom"] = _fdsCompletelyRandom;
            }

            if (fdsBranchOnObjective != null)
            {
                result["fdsBranchOnObjective"] = fdsBranchOnObjective;
            }

            if (_fdsImproveNogoods != null)
            {
                result["_fdsImproveNogoods"] = _fdsImproveNogoods;
            }

            if (fdsBranchOrdering != null)
            {
                var validValues = new[] { "FailureFirst", "FailureLast", "Random" };
                if (!validValues.Contains(fdsBranchOrdering, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsBranchOrdering}' for parameter FDSBranchOrdering");
                result["fdsBranchOrdering"] = fdsBranchOrdering;
            }

            if (_fdsDiveBySetTimes != null)
            {
                result["_fdsDiveBySetTimes"] = _fdsDiveBySetTimes;
            }

            if (fdsDualStrategy != null)
            {
                var validValues = new[] { "Minimum", "Random", "Split" };
                if (!validValues.Contains(fdsDualStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsDualStrategy}' for parameter FDSDualStrategy");
                result["fdsDualStrategy"] = fdsDualStrategy;
            }

            if (fdsDualResetRatings != null)
            {
                result["fdsDualResetRatings"] = fdsDualResetRatings;
            }

            if (lnsMode != null)
            {
                var validValues = new[] { "Robust", "Focused" };
                if (!validValues.Contains(lnsMode, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{lnsMode}' for parameter LNSMode");
                result["lnsMode"] = lnsMode;
            }

            if (_lnsInitNoOverlapPropagationLevel != null)
            {
                if (_lnsInitNoOverlapPropagationLevel < 0 || _lnsInitNoOverlapPropagationLevel > 4)
                    throw new ArgumentException($"Value {_lnsInitNoOverlapPropagationLevel} for parameter LNSInitNoOverlapPropagationLevel is out of range [0, 4]");
                result["_lnsInitNoOverlapPropagationLevel"] = _lnsInitNoOverlapPropagationLevel;
            }

            if (_lnsInitCumulPropagationLevel != null)
            {
                if (_lnsInitCumulPropagationLevel < 0 || _lnsInitCumulPropagationLevel > 4)
                    throw new ArgumentException($"Value {_lnsInitCumulPropagationLevel} for parameter LNSInitCumulPropagationLevel is out of range [0, 4]");
                result["_lnsInitCumulPropagationLevel"] = _lnsInitCumulPropagationLevel;
            }

            if (_lnsFirstFailLimit != null)
            {
                result["_lnsFirstFailLimit"] = _lnsFirstFailLimit;
            }

            if (_lnsFailLimitGrowthFactor != null)
            {
                if (double.IsPositiveInfinity(_lnsFailLimitGrowthFactor.Value))
                    result["_lnsFailLimitGrowthFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsFailLimitGrowthFactor.Value))
                    result["_lnsFailLimitGrowthFactor"] = "-Infinity";
                else
                    result["_lnsFailLimitGrowthFactor"] = _lnsFailLimitGrowthFactor;
            }

            if (_lnsFailLimitCoefficient != null)
            {
                if (double.IsPositiveInfinity(_lnsFailLimitCoefficient.Value))
                    result["_lnsFailLimitCoefficient"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsFailLimitCoefficient.Value))
                    result["_lnsFailLimitCoefficient"] = "-Infinity";
                else
                    result["_lnsFailLimitCoefficient"] = _lnsFailLimitCoefficient;
            }

            if (_lnsIterationsAfterFirstSolution != null)
            {
                if (_lnsIterationsAfterFirstSolution < -1 || _lnsIterationsAfterFirstSolution > 2147483647)
                    throw new ArgumentException($"Value {_lnsIterationsAfterFirstSolution} for parameter LNSIterationsAfterFirstSolution is out of range [-1, 2147483647]");
                result["_lnsIterationsAfterFirstSolution"] = _lnsIterationsAfterFirstSolution;
            }

            if (_lnsAggressiveDominance != null)
            {
                result["_lnsAggressiveDominance"] = _lnsAggressiveDominance;
            }

            if (_lnsSameSolutionPeriod != null)
            {
                if (_lnsSameSolutionPeriod < 1 || _lnsSameSolutionPeriod > 2147483647)
                    throw new ArgumentException($"Value {_lnsSameSolutionPeriod} for parameter LNSSameSolutionPeriod is out of range [1, 2147483647]");
                result["_lnsSameSolutionPeriod"] = _lnsSameSolutionPeriod;
            }

            if (_lnsTier1Size != null)
            {
                if (_lnsTier1Size < 1 || _lnsTier1Size > 1000)
                    throw new ArgumentException($"Value {_lnsTier1Size} for parameter LNSTier1Size is out of range [1, 1000]");
                result["_lnsTier1Size"] = _lnsTier1Size;
            }

            if (_lnsTier2Size != null)
            {
                if (_lnsTier2Size < 0 || _lnsTier2Size > 1000)
                    throw new ArgumentException($"Value {_lnsTier2Size} for parameter LNSTier2Size is out of range [0, 1000]");
                result["_lnsTier2Size"] = _lnsTier2Size;
            }

            if (_lnsTier3Size != null)
            {
                if (_lnsTier3Size < 0 || _lnsTier3Size > 1000)
                    throw new ArgumentException($"Value {_lnsTier3Size} for parameter LNSTier3Size is out of range [0, 1000]");
                result["_lnsTier3Size"] = _lnsTier3Size;
            }

            if (_lnsTier2Effort != null)
            {
                if (_lnsTier2Effort < 0.000000 || _lnsTier2Effort > 1.000000)
                    throw new ArgumentException($"Value {_lnsTier2Effort} for parameter LNSTier2Effort is out of range [0.000000, 1.000000]");
                if (double.IsPositiveInfinity(_lnsTier2Effort.Value))
                    result["_lnsTier2Effort"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsTier2Effort.Value))
                    result["_lnsTier2Effort"] = "-Infinity";
                else
                    result["_lnsTier2Effort"] = _lnsTier2Effort;
            }

            if (_lnsTier3Effort != null)
            {
                if (_lnsTier3Effort < 0.000000 || _lnsTier3Effort > 1.000000)
                    throw new ArgumentException($"Value {_lnsTier3Effort} for parameter LNSTier3Effort is out of range [0.000000, 1.000000]");
                if (double.IsPositiveInfinity(_lnsTier3Effort.Value))
                    result["_lnsTier3Effort"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsTier3Effort.Value))
                    result["_lnsTier3Effort"] = "-Infinity";
                else
                    result["_lnsTier3Effort"] = _lnsTier3Effort;
            }

            if (_lnsStepFailLimitFactor != null)
            {
                if (double.IsPositiveInfinity(_lnsStepFailLimitFactor.Value))
                    result["_lnsStepFailLimitFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsStepFailLimitFactor.Value))
                    result["_lnsStepFailLimitFactor"] = "-Infinity";
                else
                    result["_lnsStepFailLimitFactor"] = _lnsStepFailLimitFactor;
            }

            if (_lnsApplyCutProbability != null)
            {
                if (_lnsApplyCutProbability < 0 || _lnsApplyCutProbability > 1)
                    throw new ArgumentException($"Value {_lnsApplyCutProbability} for parameter LNSApplyCutProbability is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsApplyCutProbability.Value))
                    result["_lnsApplyCutProbability"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsApplyCutProbability.Value))
                    result["_lnsApplyCutProbability"] = "-Infinity";
                else
                    result["_lnsApplyCutProbability"] = _lnsApplyCutProbability;
            }

            if (_lnsSkipNoImprovementProbability != null)
            {
                if (_lnsSkipNoImprovementProbability < 0 || _lnsSkipNoImprovementProbability > 1)
                    throw new ArgumentException($"Value {_lnsSkipNoImprovementProbability} for parameter LNSSkipNoImprovementProbability is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsSkipNoImprovementProbability.Value))
                    result["_lnsSkipNoImprovementProbability"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsSkipNoImprovementProbability.Value))
                    result["_lnsSkipNoImprovementProbability"] = "-Infinity";
                else
                    result["_lnsSkipNoImprovementProbability"] = _lnsSkipNoImprovementProbability;
            }

            if (_lnsSmallStructureLimit != null)
            {
                if (_lnsSmallStructureLimit < 0 || _lnsSmallStructureLimit > 10)
                    throw new ArgumentException($"Value {_lnsSmallStructureLimit} for parameter LNSSmallStructureLimit is out of range [0, 10]");
                result["_lnsSmallStructureLimit"] = _lnsSmallStructureLimit;
            }

            if (_lnsResourceOptimization != null)
            {
                result["_lnsResourceOptimization"] = _lnsResourceOptimization;
            }

            if (_lnsRestoreAbsentIntervals != null)
            {
                result["_lnsRestoreAbsentIntervals"] = _lnsRestoreAbsentIntervals;
            }

            if (_lnsRestoreIntervalLengths != null)
            {
                result["_lnsRestoreIntervalLengths"] = _lnsRestoreIntervalLengths;
            }

            if (_lnsRestoreIntVarValues != null)
            {
                result["_lnsRestoreIntVarValues"] = _lnsRestoreIntVarValues;
            }

            if (_lnsAlwaysRelaxObjective != null)
            {
                result["_lnsAlwaysRelaxObjective"] = _lnsAlwaysRelaxObjective;
            }

            if (lnsUseWarmStartOnly != null)
            {
                result["lnsUseWarmStartOnly"] = lnsUseWarmStartOnly;
            }

            if (_lnsHeuristicsEpsilon != null)
            {
                if (_lnsHeuristicsEpsilon < 0 || _lnsHeuristicsEpsilon > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsEpsilon} for parameter LNSHeuristicsEpsilon is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsEpsilon.Value))
                    result["_lnsHeuristicsEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsEpsilon.Value))
                    result["_lnsHeuristicsEpsilon"] = "-Infinity";
                else
                    result["_lnsHeuristicsEpsilon"] = _lnsHeuristicsEpsilon;
            }

            if (_lnsHeuristicsAlpha != null)
            {
                if (_lnsHeuristicsAlpha < 0 || _lnsHeuristicsAlpha > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsAlpha} for parameter LNSHeuristicsAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsAlpha.Value))
                    result["_lnsHeuristicsAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsAlpha.Value))
                    result["_lnsHeuristicsAlpha"] = "-Infinity";
                else
                    result["_lnsHeuristicsAlpha"] = _lnsHeuristicsAlpha;
            }

            if (_lnsHeuristicsTemperature != null)
            {
                if (_lnsHeuristicsTemperature < -1 || _lnsHeuristicsTemperature > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsTemperature} for parameter LNSHeuristicsTemperature is out of range [-1, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsTemperature.Value))
                    result["_lnsHeuristicsTemperature"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsTemperature.Value))
                    result["_lnsHeuristicsTemperature"] = "-Infinity";
                else
                    result["_lnsHeuristicsTemperature"] = _lnsHeuristicsTemperature;
            }

            if (_lnsHeuristicsUniform != null)
            {
                result["_lnsHeuristicsUniform"] = _lnsHeuristicsUniform;
            }

            if (_lnsHeuristicsInitialQ != null)
            {
                if (_lnsHeuristicsInitialQ < 0 || _lnsHeuristicsInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsInitialQ} for parameter LNSHeuristicsInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsInitialQ.Value))
                    result["_lnsHeuristicsInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsInitialQ.Value))
                    result["_lnsHeuristicsInitialQ"] = "-Infinity";
                else
                    result["_lnsHeuristicsInitialQ"] = _lnsHeuristicsInitialQ;
            }

            if (_lnsPortionEpsilon != null)
            {
                if (_lnsPortionEpsilon < 0 || _lnsPortionEpsilon > 1)
                    throw new ArgumentException($"Value {_lnsPortionEpsilon} for parameter LNSPortionEpsilon is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionEpsilon.Value))
                    result["_lnsPortionEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionEpsilon.Value))
                    result["_lnsPortionEpsilon"] = "-Infinity";
                else
                    result["_lnsPortionEpsilon"] = _lnsPortionEpsilon;
            }

            if (_lnsPortionAlpha != null)
            {
                if (_lnsPortionAlpha < 0 || _lnsPortionAlpha > 1)
                    throw new ArgumentException($"Value {_lnsPortionAlpha} for parameter LNSPortionAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionAlpha.Value))
                    result["_lnsPortionAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionAlpha.Value))
                    result["_lnsPortionAlpha"] = "-Infinity";
                else
                    result["_lnsPortionAlpha"] = _lnsPortionAlpha;
            }

            if (_lnsPortionTemperature != null)
            {
                if (_lnsPortionTemperature < -1 || _lnsPortionTemperature > 1)
                    throw new ArgumentException($"Value {_lnsPortionTemperature} for parameter LNSPortionTemperature is out of range [-1, 1]");
                if (double.IsPositiveInfinity(_lnsPortionTemperature.Value))
                    result["_lnsPortionTemperature"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionTemperature.Value))
                    result["_lnsPortionTemperature"] = "-Infinity";
                else
                    result["_lnsPortionTemperature"] = _lnsPortionTemperature;
            }

            if (_lnsPortionUniform != null)
            {
                result["_lnsPortionUniform"] = _lnsPortionUniform;
            }

            if (_lnsPortionInitialQ != null)
            {
                if (_lnsPortionInitialQ < 0 || _lnsPortionInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsPortionInitialQ} for parameter LNSPortionInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionInitialQ.Value))
                    result["_lnsPortionInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionInitialQ.Value))
                    result["_lnsPortionInitialQ"] = "-Infinity";
                else
                    result["_lnsPortionInitialQ"] = _lnsPortionInitialQ;
            }

            if (_lnsPortionHandicapLimit != null)
            {
                if (_lnsPortionHandicapLimit < 0 || _lnsPortionHandicapLimit > 1)
                    throw new ArgumentException($"Value {_lnsPortionHandicapLimit} for parameter LNSPortionHandicapLimit is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionHandicapLimit.Value))
                    result["_lnsPortionHandicapLimit"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionHandicapLimit.Value))
                    result["_lnsPortionHandicapLimit"] = "-Infinity";
                else
                    result["_lnsPortionHandicapLimit"] = _lnsPortionHandicapLimit;
            }

            if (_lnsPortionHandicapValue != null)
            {
                if (_lnsPortionHandicapValue < 0 || _lnsPortionHandicapValue > 1)
                    throw new ArgumentException($"Value {_lnsPortionHandicapValue} for parameter LNSPortionHandicapValue is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionHandicapValue.Value))
                    result["_lnsPortionHandicapValue"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionHandicapValue.Value))
                    result["_lnsPortionHandicapValue"] = "-Infinity";
                else
                    result["_lnsPortionHandicapValue"] = _lnsPortionHandicapValue;
            }

            if (_lnsPortionHandicapInitialQ != null)
            {
                if (_lnsPortionHandicapInitialQ < 0 || _lnsPortionHandicapInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsPortionHandicapInitialQ} for parameter LNSPortionHandicapInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionHandicapInitialQ.Value))
                    result["_lnsPortionHandicapInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionHandicapInitialQ.Value))
                    result["_lnsPortionHandicapInitialQ"] = "-Infinity";
                else
                    result["_lnsPortionHandicapInitialQ"] = _lnsPortionHandicapInitialQ;
            }

            if (_lnsNeighborhoodStrategy != null)
            {
                if (_lnsNeighborhoodStrategy < 0 || _lnsNeighborhoodStrategy > 2)
                    throw new ArgumentException($"Value {_lnsNeighborhoodStrategy} for parameter LNSNeighborhoodStrategy is out of range [0, 2]");
                result["_lnsNeighborhoodStrategy"] = _lnsNeighborhoodStrategy;
            }

            if (_lnsNeighborhoodEpsilon != null)
            {
                if (_lnsNeighborhoodEpsilon < 0 || _lnsNeighborhoodEpsilon > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodEpsilon} for parameter LNSNeighborhoodEpsilon is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodEpsilon.Value))
                    result["_lnsNeighborhoodEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodEpsilon.Value))
                    result["_lnsNeighborhoodEpsilon"] = "-Infinity";
                else
                    result["_lnsNeighborhoodEpsilon"] = _lnsNeighborhoodEpsilon;
            }

            if (_lnsNeighborhoodAlpha != null)
            {
                if (_lnsNeighborhoodAlpha < 0 || _lnsNeighborhoodAlpha > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodAlpha} for parameter LNSNeighborhoodAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodAlpha.Value))
                    result["_lnsNeighborhoodAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodAlpha.Value))
                    result["_lnsNeighborhoodAlpha"] = "-Infinity";
                else
                    result["_lnsNeighborhoodAlpha"] = _lnsNeighborhoodAlpha;
            }

            if (_lnsNeighborhoodTemperature != null)
            {
                if (_lnsNeighborhoodTemperature < -1 || _lnsNeighborhoodTemperature > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodTemperature} for parameter LNSNeighborhoodTemperature is out of range [-1, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodTemperature.Value))
                    result["_lnsNeighborhoodTemperature"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodTemperature.Value))
                    result["_lnsNeighborhoodTemperature"] = "-Infinity";
                else
                    result["_lnsNeighborhoodTemperature"] = _lnsNeighborhoodTemperature;
            }

            if (_lnsNeighborhoodUniform != null)
            {
                result["_lnsNeighborhoodUniform"] = _lnsNeighborhoodUniform;
            }

            if (_lnsNeighborhoodInitialQ != null)
            {
                if (_lnsNeighborhoodInitialQ < 0 || _lnsNeighborhoodInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodInitialQ} for parameter LNSNeighborhoodInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodInitialQ.Value))
                    result["_lnsNeighborhoodInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodInitialQ.Value))
                    result["_lnsNeighborhoodInitialQ"] = "-Infinity";
                else
                    result["_lnsNeighborhoodInitialQ"] = _lnsNeighborhoodInitialQ;
            }

            if (_lnsDivingLimit != null)
            {
                result["_lnsDivingLimit"] = _lnsDivingLimit;
            }

            if (_lnsDivingFailLimitRatio != null)
            {
                if (double.IsPositiveInfinity(_lnsDivingFailLimitRatio.Value))
                    result["_lnsDivingFailLimitRatio"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsDivingFailLimitRatio.Value))
                    result["_lnsDivingFailLimitRatio"] = "-Infinity";
                else
                    result["_lnsDivingFailLimitRatio"] = _lnsDivingFailLimitRatio;
            }

            if (_lnsLearningRun != null)
            {
                result["_lnsLearningRun"] = _lnsLearningRun;
            }

            if (_lnsStayOnObjective != null)
            {
                result["_lnsStayOnObjective"] = _lnsStayOnObjective;
            }

            if (_lnsFDS != null)
            {
                result["_lnsFDS"] = _lnsFDS;
            }

            if (_lnsFragmentFreezing != null)
            {
                var validValues = new[] { "Off", "BeforeOnly", "On", "Unsafe" };
                if (!validValues.Contains(_lnsFragmentFreezing, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{_lnsFragmentFreezing}' for parameter LNSFragmentFreezing");
                result["_lnsFragmentFreezing"] = _lnsFragmentFreezing;
            }

            if (_lnsRelaxSlack != null)
            {
                if (_lnsRelaxSlack < 0.000000 || _lnsRelaxSlack > 1.000000)
                    throw new ArgumentException($"Value {_lnsRelaxSlack} for parameter LNSRelaxSlack is out of range [0.000000, 1.000000]");
                if (double.IsPositiveInfinity(_lnsRelaxSlack.Value))
                    result["_lnsRelaxSlack"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsRelaxSlack.Value))
                    result["_lnsRelaxSlack"] = "-Infinity";
                else
                    result["_lnsRelaxSlack"] = _lnsRelaxSlack;
            }

            if (_lnsPortionMultiplier != null)
            {
                if (_lnsPortionMultiplier < 0.000001 || _lnsPortionMultiplier > 10.000000)
                    throw new ArgumentException($"Value {_lnsPortionMultiplier} for parameter LNSPortionMultiplier is out of range [0.000001, 10.000000]");
                if (double.IsPositiveInfinity(_lnsPortionMultiplier.Value))
                    result["_lnsPortionMultiplier"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionMultiplier.Value))
                    result["_lnsPortionMultiplier"] = "-Infinity";
                else
                    result["_lnsPortionMultiplier"] = _lnsPortionMultiplier;
            }

            if (simpleLBWorker != null)
            {
                if (simpleLBWorker < -1 || simpleLBWorker > 2147483647)
                    throw new ArgumentException($"Value {simpleLBWorker} for parameter SimpleLBWorker is out of range [-1, 2147483647]");
                result["simpleLBWorker"] = simpleLBWorker;
            }

            if (simpleLBMaxIterations != null)
            {
                if (simpleLBMaxIterations < 0 || simpleLBMaxIterations > 2147483647)
                    throw new ArgumentException($"Value {simpleLBMaxIterations} for parameter SimpleLBMaxIterations is out of range [0, 2147483647]");
                result["simpleLBMaxIterations"] = simpleLBMaxIterations;
            }

            if (simpleLBShavingRounds != null)
            {
                if (simpleLBShavingRounds < 0 || simpleLBShavingRounds > 2147483647)
                    throw new ArgumentException($"Value {simpleLBShavingRounds} for parameter SimpleLBShavingRounds is out of range [0, 2147483647]");
                result["simpleLBShavingRounds"] = simpleLBShavingRounds;
            }

            if (_debugTraceLevel != null)
            {
                if (_debugTraceLevel < 0 || _debugTraceLevel > 5)
                    throw new ArgumentException($"Value {_debugTraceLevel} for parameter DebugTraceLevel is out of range [0, 5]");
                result["_debugTraceLevel"] = _debugTraceLevel;
            }

            if (_memoryTraceLevel != null)
            {
                if (_memoryTraceLevel < 0 || _memoryTraceLevel > 5)
                    throw new ArgumentException($"Value {_memoryTraceLevel} for parameter MemoryTraceLevel is out of range [0, 5]");
                result["_memoryTraceLevel"] = _memoryTraceLevel;
            }

            if (_propagationDetailTraceLevel != null)
            {
                if (_propagationDetailTraceLevel < 0 || _propagationDetailTraceLevel > 5)
                    throw new ArgumentException($"Value {_propagationDetailTraceLevel} for parameter PropagationDetailTraceLevel is out of range [0, 5]");
                result["_propagationDetailTraceLevel"] = _propagationDetailTraceLevel;
            }

            if (_setTimesTraceLevel != null)
            {
                if (_setTimesTraceLevel < 0 || _setTimesTraceLevel > 5)
                    throw new ArgumentException($"Value {_setTimesTraceLevel} for parameter SetTimesTraceLevel is out of range [0, 5]");
                result["_setTimesTraceLevel"] = _setTimesTraceLevel;
            }

            if (_communicationTraceLevel != null)
            {
                if (_communicationTraceLevel < 0 || _communicationTraceLevel > 5)
                    throw new ArgumentException($"Value {_communicationTraceLevel} for parameter CommunicationTraceLevel is out of range [0, 5]");
                result["_communicationTraceLevel"] = _communicationTraceLevel;
            }

            if (_presolveTraceLevel != null)
            {
                if (_presolveTraceLevel < 0 || _presolveTraceLevel > 5)
                    throw new ArgumentException($"Value {_presolveTraceLevel} for parameter PresolveTraceLevel is out of range [0, 5]");
                result["_presolveTraceLevel"] = _presolveTraceLevel;
            }

            if (_conversionTraceLevel != null)
            {
                if (_conversionTraceLevel < 0 || _conversionTraceLevel > 5)
                    throw new ArgumentException($"Value {_conversionTraceLevel} for parameter ConversionTraceLevel is out of range [0, 5]");
                result["_conversionTraceLevel"] = _conversionTraceLevel;
            }

            if (_expressionBuilderTraceLevel != null)
            {
                if (_expressionBuilderTraceLevel < 0 || _expressionBuilderTraceLevel > 5)
                    throw new ArgumentException($"Value {_expressionBuilderTraceLevel} for parameter ExpressionBuilderTraceLevel is out of range [0, 5]");
                result["_expressionBuilderTraceLevel"] = _expressionBuilderTraceLevel;
            }

            if (_memorizationTraceLevel != null)
            {
                if (_memorizationTraceLevel < 0 || _memorizationTraceLevel > 5)
                    throw new ArgumentException($"Value {_memorizationTraceLevel} for parameter MemorizationTraceLevel is out of range [0, 5]");
                result["_memorizationTraceLevel"] = _memorizationTraceLevel;
            }

            if (_searchDetailTraceLevel != null)
            {
                if (_searchDetailTraceLevel < 0 || _searchDetailTraceLevel > 5)
                    throw new ArgumentException($"Value {_searchDetailTraceLevel} for parameter SearchDetailTraceLevel is out of range [0, 5]");
                result["_searchDetailTraceLevel"] = _searchDetailTraceLevel;
            }

            if (_fdsTraceLevel != null)
            {
                if (_fdsTraceLevel < 0 || _fdsTraceLevel > 5)
                    throw new ArgumentException($"Value {_fdsTraceLevel} for parameter FDSTraceLevel is out of range [0, 5]");
                result["_fdsTraceLevel"] = _fdsTraceLevel;
            }

            if (_shavingTraceLevel != null)
            {
                if (_shavingTraceLevel < 0 || _shavingTraceLevel > 5)
                    throw new ArgumentException($"Value {_shavingTraceLevel} for parameter ShavingTraceLevel is out of range [0, 5]");
                result["_shavingTraceLevel"] = _shavingTraceLevel;
            }

            if (_fdsRatingsTraceLevel != null)
            {
                if (_fdsRatingsTraceLevel < 0 || _fdsRatingsTraceLevel > 5)
                    throw new ArgumentException($"Value {_fdsRatingsTraceLevel} for parameter FDSRatingsTraceLevel is out of range [0, 5]");
                result["_fdsRatingsTraceLevel"] = _fdsRatingsTraceLevel;
            }

            if (_lnsTraceLevel != null)
            {
                if (_lnsTraceLevel < 0 || _lnsTraceLevel > 5)
                    throw new ArgumentException($"Value {_lnsTraceLevel} for parameter LNSTraceLevel is out of range [0, 5]");
                result["_lnsTraceLevel"] = _lnsTraceLevel;
            }

            if (_heuristicReplayTraceLevel != null)
            {
                if (_heuristicReplayTraceLevel < 0 || _heuristicReplayTraceLevel > 5)
                    throw new ArgumentException($"Value {_heuristicReplayTraceLevel} for parameter HeuristicReplayTraceLevel is out of range [0, 5]");
                result["_heuristicReplayTraceLevel"] = _heuristicReplayTraceLevel;
            }

            if (_allowSetTimesProofs != null)
            {
                result["_allowSetTimesProofs"] = _allowSetTimesProofs;
            }

            if (_setTimesAggressiveDominance != null)
            {
                result["_setTimesAggressiveDominance"] = _setTimesAggressiveDominance;
            }

            if (_setTimesExtendsCoef != null)
            {
                if (double.IsPositiveInfinity(_setTimesExtendsCoef.Value))
                    result["_setTimesExtendsCoef"] = "Infinity";
                else if (double.IsNegativeInfinity(_setTimesExtendsCoef.Value))
                    result["_setTimesExtendsCoef"] = "-Infinity";
                else
                    result["_setTimesExtendsCoef"] = _setTimesExtendsCoef;
            }

            if (_setTimesHeightStrategy != null)
            {
                var validValues = new[] { "FromMax", "FromMin", "Random" };
                if (!validValues.Contains(_setTimesHeightStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{_setTimesHeightStrategy}' for parameter SetTimesHeightStrategy");
                result["_setTimesHeightStrategy"] = _setTimesHeightStrategy;
            }

            if (_setTimesItvMappingStrategy != null)
            {
                var validValues = new[] { "FromMax", "FromMin", "Random" };
                if (!validValues.Contains(_setTimesItvMappingStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{_setTimesItvMappingStrategy}' for parameter SetTimesItvMappingStrategy");
                result["_setTimesItvMappingStrategy"] = _setTimesItvMappingStrategy;
            }

            if (_setTimesInitDensity != null)
            {
                if (double.IsPositiveInfinity(_setTimesInitDensity.Value))
                    result["_setTimesInitDensity"] = "Infinity";
                else if (double.IsNegativeInfinity(_setTimesInitDensity.Value))
                    result["_setTimesInitDensity"] = "-Infinity";
                else
                    result["_setTimesInitDensity"] = _setTimesInitDensity;
            }

            if (_setTimesDensityLength != null)
            {
                result["_setTimesDensityLength"] = _setTimesDensityLength;
            }

            if (_setTimesDensityReliabilityThreshold != null)
            {
                result["_setTimesDensityReliabilityThreshold"] = _setTimesDensityReliabilityThreshold;
            }

            if (_lnsDisabledHeuristics != null)
            {
                result["_lnsDisabledHeuristics"] = _lnsDisabledHeuristics;
            }

            if (_setTimesNbExtendsFactor != null)
            {
                if (double.IsPositiveInfinity(_setTimesNbExtendsFactor.Value))
                    result["_setTimesNbExtendsFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(_setTimesNbExtendsFactor.Value))
                    result["_setTimesNbExtendsFactor"] = "-Infinity";
                else
                    result["_setTimesNbExtendsFactor"] = _setTimesNbExtendsFactor;
            }

            if (_discreteLowCapacityLimit != null)
            {
                if (_discreteLowCapacityLimit < 0 || _discreteLowCapacityLimit > 16)
                    throw new ArgumentException($"Value {_discreteLowCapacityLimit} for parameter DiscreteLowCapacityLimit is out of range [0, 16]");
                result["_discreteLowCapacityLimit"] = _discreteLowCapacityLimit;
            }

            if (_lnsTrainingObjectiveLimit != null)
            {
                if (double.IsPositiveInfinity(_lnsTrainingObjectiveLimit.Value))
                    result["_lnsTrainingObjectiveLimit"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsTrainingObjectiveLimit.Value))
                    result["_lnsTrainingObjectiveLimit"] = "-Infinity";
                else
                    result["_lnsTrainingObjectiveLimit"] = _lnsTrainingObjectiveLimit;
            }

            if (_posAbsentRelated != null)
            {
                result["_posAbsentRelated"] = _posAbsentRelated;
            }

            if (_defaultCallbackBlockSize != null)
            {
                result["_defaultCallbackBlockSize"] = _defaultCallbackBlockSize;
            }

            if (_useReservoirPegging != null)
            {
                result["_useReservoirPegging"] = _useReservoirPegging;
            }

            if (_useTimeNet != null)
            {
                result["_useTimeNet"] = _useTimeNet;
            }

            if (_timeNetVarsToPreprocess != null)
            {
                result["_timeNetVarsToPreprocess"] = _timeNetVarsToPreprocess;
            }

            if (_timeNetSubPriorityBits != null)
            {
                if (_timeNetSubPriorityBits < 4 || _timeNetSubPriorityBits > 20)
                    throw new ArgumentException($"Value {_timeNetSubPriorityBits} for parameter TimeNetSubPriorityBits is out of range [4, 20]");
                result["_timeNetSubPriorityBits"] = _timeNetSubPriorityBits;
            }

            if (_minChainLength != null)
            {
                result["_minChainLength"] = _minChainLength;
            }

            if (_monitorInterval != null)
            {
                if (double.IsPositiveInfinity(_monitorInterval.Value))
                    result["_monitorInterval"] = "Infinity";
                else if (double.IsNegativeInfinity(_monitorInterval.Value))
                    result["_monitorInterval"] = "-Infinity";
                else
                    result["_monitorInterval"] = _monitorInterval;
            }

            if (_monitorThreshold != null)
            {
                if (_monitorThreshold < 0 || _monitorThreshold > 100)
                    throw new ArgumentException($"Value {_monitorThreshold} for parameter MonitorThreshold is out of range [0, 100]");
                if (double.IsPositiveInfinity(_monitorThreshold.Value))
                    result["_monitorThreshold"] = "Infinity";
                else if (double.IsNegativeInfinity(_monitorThreshold.Value))
                    result["_monitorThreshold"] = "-Infinity";
                else
                    result["_monitorThreshold"] = _monitorThreshold;
            }



            // Serialize workers
            if (workers != null)
            {
                var workerDicts = new List<Dictionary<string, object?>>();
                foreach (var w in workers)
                    workerDicts.Add(w.ToDict());
                result["workers"] = workerDicts;
            }

            return result;
        }

        /// <summary>
        /// Creates Parameters from a dictionary representation.
        /// </summary>
        internal static Parameters FromDict(Dictionary<string, object?> data)
        {
            var parameters = new Parameters();

            // Parse infinities from JSON string representation (inline to avoid Serialization dependency)
            var keys = data.Keys.ToList();
            foreach (var key in keys)
            {
                if (data[key] is string s)
                {
                    if (s == "Infinity")
                        data[key] = double.PositiveInfinity;
                    else if (s == "-Infinity")
                        data[key] = double.NegativeInfinity;
                }
            }

            if (data.TryGetValue("color", out var colorVal) && colorVal != null)
            {
                if (colorVal is System.Text.Json.JsonElement colorElem)
                {
                    if (colorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.color = colorElem.GetString();
                }
            }

            if (data.TryGetValue("nbWorkers", out var nbWorkersVal) && nbWorkersVal != null)
            {
                if (nbWorkersVal is System.Text.Json.JsonElement nbWorkersElem)
                {
                    if (nbWorkersElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.nbWorkers = nbWorkersElem.GetInt64();
                }
            }

            if (data.TryGetValue("_nbHelpers", out var nbHelpersVal) && nbHelpersVal != null)
            {
                if (nbHelpersVal is System.Text.Json.JsonElement nbHelpersElem)
                {
                    if (nbHelpersElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._nbHelpers = nbHelpersElem.GetInt64();
                }
            }

            if (data.TryGetValue("preset", out var presetVal) && presetVal != null)
            {
                if (presetVal is System.Text.Json.JsonElement presetElem)
                {
                    if (presetElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.preset = presetElem.GetString();
                }
            }

            if (data.TryGetValue("searchType", out var searchTypeVal) && searchTypeVal != null)
            {
                if (searchTypeVal is System.Text.Json.JsonElement searchTypeElem)
                {
                    if (searchTypeElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.searchType = searchTypeElem.GetString();
                }
            }

            if (data.TryGetValue("randomSeed", out var randomSeedVal) && randomSeedVal != null)
            {
                if (randomSeedVal is System.Text.Json.JsonElement randomSeedElem)
                {
                    if (randomSeedElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.randomSeed = randomSeedElem.GetInt64();
                }
            }

            if (data.TryGetValue("logLevel", out var logLevelVal) && logLevelVal != null)
            {
                if (logLevelVal is System.Text.Json.JsonElement logLevelElem)
                {
                    if (logLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.logLevel = logLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("warningLevel", out var warningLevelVal) && warningLevelVal != null)
            {
                if (warningLevelVal is System.Text.Json.JsonElement warningLevelElem)
                {
                    if (warningLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.warningLevel = warningLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("logPeriod", out var logPeriodVal) && logPeriodVal != null)
            {
                if (logPeriodVal is System.Text.Json.JsonElement logPeriodElem)
                {
                    if (logPeriodElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.logPeriod = logPeriodElem.GetDouble();
                    else if (logPeriodElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = logPeriodElem.GetString();
                        if (str == "Infinity") parameters.logPeriod = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.logPeriod = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("verifySolutions", out var verifySolutionsVal) && verifySolutionsVal != null)
            {
                if (verifySolutionsVal is System.Text.Json.JsonElement verifySolutionsElem)
                {
                    if (verifySolutionsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.verifySolutions = true;
                    else if (verifySolutionsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.verifySolutions = false;
                }
            }

            if (data.TryGetValue("verifyExternalSolutions", out var verifyExternalSolutionsVal) && verifyExternalSolutionsVal != null)
            {
                if (verifyExternalSolutionsVal is System.Text.Json.JsonElement verifyExternalSolutionsElem)
                {
                    if (verifyExternalSolutionsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.verifyExternalSolutions = true;
                    else if (verifyExternalSolutionsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.verifyExternalSolutions = false;
                }
            }

            if (data.TryGetValue("allocationBlockSize", out var allocationBlockSizeVal) && allocationBlockSizeVal != null)
            {
                if (allocationBlockSizeVal is System.Text.Json.JsonElement allocationBlockSizeElem)
                {
                    if (allocationBlockSizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.allocationBlockSize = allocationBlockSizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("processExitTimeout", out var processExitTimeoutVal) && processExitTimeoutVal != null)
            {
                if (processExitTimeoutVal is System.Text.Json.JsonElement processExitTimeoutElem)
                {
                    if (processExitTimeoutElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.processExitTimeout = processExitTimeoutElem.GetDouble();
                    else if (processExitTimeoutElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = processExitTimeoutElem.GetString();
                        if (str == "Infinity") parameters.processExitTimeout = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.processExitTimeout = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("timeLimit", out var timeLimitVal) && timeLimitVal != null)
            {
                if (timeLimitVal is System.Text.Json.JsonElement timeLimitElem)
                {
                    if (timeLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.timeLimit = timeLimitElem.GetDouble();
                    else if (timeLimitElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = timeLimitElem.GetString();
                        if (str == "Infinity") parameters.timeLimit = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.timeLimit = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("solutionLimit", out var solutionLimitVal) && solutionLimitVal != null)
            {
                if (solutionLimitVal is System.Text.Json.JsonElement solutionLimitElem)
                {
                    if (solutionLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.solutionLimit = solutionLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerFailLimit", out var workerFailLimitVal) && workerFailLimitVal != null)
            {
                if (workerFailLimitVal is System.Text.Json.JsonElement workerFailLimitElem)
                {
                    if (workerFailLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._workerFailLimit = workerFailLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerBranchLimit", out var workerBranchLimitVal) && workerBranchLimitVal != null)
            {
                if (workerBranchLimitVal is System.Text.Json.JsonElement workerBranchLimitElem)
                {
                    if (workerBranchLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._workerBranchLimit = workerBranchLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerSolutionLimit", out var workerSolutionLimitVal) && workerSolutionLimitVal != null)
            {
                if (workerSolutionLimitVal is System.Text.Json.JsonElement workerSolutionLimitElem)
                {
                    if (workerSolutionLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._workerSolutionLimit = workerSolutionLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerLNSStepLimit", out var workerLNSStepLimitVal) && workerLNSStepLimitVal != null)
            {
                if (workerLNSStepLimitVal is System.Text.Json.JsonElement workerLNSStepLimitElem)
                {
                    if (workerLNSStepLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._workerLNSStepLimit = workerLNSStepLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerRestartLimit", out var workerRestartLimitVal) && workerRestartLimitVal != null)
            {
                if (workerRestartLimitVal is System.Text.Json.JsonElement workerRestartLimitElem)
                {
                    if (workerRestartLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._workerRestartLimit = workerRestartLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("absoluteGapTolerance", out var absoluteGapToleranceVal) && absoluteGapToleranceVal != null)
            {
                if (absoluteGapToleranceVal is System.Text.Json.JsonElement absoluteGapToleranceElem)
                {
                    if (absoluteGapToleranceElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.absoluteGapTolerance = absoluteGapToleranceElem.GetDouble();
                    else if (absoluteGapToleranceElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = absoluteGapToleranceElem.GetString();
                        if (str == "Infinity") parameters.absoluteGapTolerance = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.absoluteGapTolerance = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("relativeGapTolerance", out var relativeGapToleranceVal) && relativeGapToleranceVal != null)
            {
                if (relativeGapToleranceVal is System.Text.Json.JsonElement relativeGapToleranceElem)
                {
                    if (relativeGapToleranceElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.relativeGapTolerance = relativeGapToleranceElem.GetDouble();
                    else if (relativeGapToleranceElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = relativeGapToleranceElem.GetString();
                        if (str == "Infinity") parameters.relativeGapTolerance = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.relativeGapTolerance = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("noOverlapPropagationLevel", out var noOverlapPropagationLevelVal) && noOverlapPropagationLevelVal != null)
            {
                if (noOverlapPropagationLevelVal is System.Text.Json.JsonElement noOverlapPropagationLevelElem)
                {
                    if (noOverlapPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.noOverlapPropagationLevel = noOverlapPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("cumulPropagationLevel", out var cumulPropagationLevelVal) && cumulPropagationLevelVal != null)
            {
                if (cumulPropagationLevelVal is System.Text.Json.JsonElement cumulPropagationLevelElem)
                {
                    if (cumulPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.cumulPropagationLevel = cumulPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("reservoirPropagationLevel", out var reservoirPropagationLevelVal) && reservoirPropagationLevelVal != null)
            {
                if (reservoirPropagationLevelVal is System.Text.Json.JsonElement reservoirPropagationLevelElem)
                {
                    if (reservoirPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.reservoirPropagationLevel = reservoirPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("positionPropagationLevel", out var positionPropagationLevelVal) && positionPropagationLevelVal != null)
            {
                if (positionPropagationLevelVal is System.Text.Json.JsonElement positionPropagationLevelElem)
                {
                    if (positionPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.positionPropagationLevel = positionPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("integralPropagationLevel", out var integralPropagationLevelVal) && integralPropagationLevelVal != null)
            {
                if (integralPropagationLevelVal is System.Text.Json.JsonElement integralPropagationLevelElem)
                {
                    if (integralPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.integralPropagationLevel = integralPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("usePrecedenceEnergy", out var usePrecedenceEnergyVal) && usePrecedenceEnergyVal != null)
            {
                if (usePrecedenceEnergyVal is System.Text.Json.JsonElement usePrecedenceEnergyElem)
                {
                    if (usePrecedenceEnergyElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.usePrecedenceEnergy = usePrecedenceEnergyElem.GetInt64();
                }
            }

            if (data.TryGetValue("_packPropagationLevel", out var packPropagationLevelVal) && packPropagationLevelVal != null)
            {
                if (packPropagationLevelVal is System.Text.Json.JsonElement packPropagationLevelElem)
                {
                    if (packPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._packPropagationLevel = packPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_itvMappingPropagationLevel", out var itvMappingPropagationLevelVal) && itvMappingPropagationLevelVal != null)
            {
                if (itvMappingPropagationLevelVal is System.Text.Json.JsonElement itvMappingPropagationLevelElem)
                {
                    if (itvMappingPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._itvMappingPropagationLevel = itvMappingPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("searchTraceLevel", out var searchTraceLevelVal) && searchTraceLevelVal != null)
            {
                if (searchTraceLevelVal is System.Text.Json.JsonElement searchTraceLevelElem)
                {
                    if (searchTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.searchTraceLevel = searchTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("propagationTraceLevel", out var propagationTraceLevelVal) && propagationTraceLevelVal != null)
            {
                if (propagationTraceLevelVal is System.Text.Json.JsonElement propagationTraceLevelElem)
                {
                    if (propagationTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.propagationTraceLevel = propagationTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("infoTraceLevel", out var infoTraceLevelVal) && infoTraceLevelVal != null)
            {
                if (infoTraceLevelVal is System.Text.Json.JsonElement infoTraceLevelElem)
                {
                    if (infoTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.infoTraceLevel = infoTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsInitialRating", out var fdsInitialRatingVal) && fdsInitialRatingVal != null)
            {
                if (fdsInitialRatingVal is System.Text.Json.JsonElement fdsInitialRatingElem)
                {
                    if (fdsInitialRatingElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsInitialRating = fdsInitialRatingElem.GetDouble();
                    else if (fdsInitialRatingElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsInitialRatingElem.GetString();
                        if (str == "Infinity") parameters.fdsInitialRating = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsInitialRating = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsReductionWeight", out var fdsReductionWeightVal) && fdsReductionWeightVal != null)
            {
                if (fdsReductionWeightVal is System.Text.Json.JsonElement fdsReductionWeightElem)
                {
                    if (fdsReductionWeightElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsReductionWeight = fdsReductionWeightElem.GetDouble();
                    else if (fdsReductionWeightElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsReductionWeightElem.GetString();
                        if (str == "Infinity") parameters.fdsReductionWeight = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsReductionWeight = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsRatingAverageLength", out var fdsRatingAverageLengthVal) && fdsRatingAverageLengthVal != null)
            {
                if (fdsRatingAverageLengthVal is System.Text.Json.JsonElement fdsRatingAverageLengthElem)
                {
                    if (fdsRatingAverageLengthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsRatingAverageLength = fdsRatingAverageLengthElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsFixedAlpha", out var fdsFixedAlphaVal) && fdsFixedAlphaVal != null)
            {
                if (fdsFixedAlphaVal is System.Text.Json.JsonElement fdsFixedAlphaElem)
                {
                    if (fdsFixedAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsFixedAlpha = fdsFixedAlphaElem.GetDouble();
                    else if (fdsFixedAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsFixedAlphaElem.GetString();
                        if (str == "Infinity") parameters.fdsFixedAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsFixedAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsRatingAverageComparison", out var fdsRatingAverageComparisonVal) && fdsRatingAverageComparisonVal != null)
            {
                if (fdsRatingAverageComparisonVal is System.Text.Json.JsonElement fdsRatingAverageComparisonElem)
                {
                    if (fdsRatingAverageComparisonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.fdsRatingAverageComparison = fdsRatingAverageComparisonElem.GetString();
                }
            }

            if (data.TryGetValue("fdsReductionFactor", out var fdsReductionFactorVal) && fdsReductionFactorVal != null)
            {
                if (fdsReductionFactorVal is System.Text.Json.JsonElement fdsReductionFactorElem)
                {
                    if (fdsReductionFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.fdsReductionFactor = fdsReductionFactorElem.GetString();
                }
            }

            if (data.TryGetValue("fdsReuseClosing", out var fdsReuseClosingVal) && fdsReuseClosingVal != null)
            {
                if (fdsReuseClosingVal is System.Text.Json.JsonElement fdsReuseClosingElem)
                {
                    if (fdsReuseClosingElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.fdsReuseClosing = true;
                    else if (fdsReuseClosingElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.fdsReuseClosing = false;
                }
            }

            if (data.TryGetValue("fdsUniformChoiceStep", out var fdsUniformChoiceStepVal) && fdsUniformChoiceStepVal != null)
            {
                if (fdsUniformChoiceStepVal is System.Text.Json.JsonElement fdsUniformChoiceStepElem)
                {
                    if (fdsUniformChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.fdsUniformChoiceStep = true;
                    else if (fdsUniformChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.fdsUniformChoiceStep = false;
                }
            }

            if (data.TryGetValue("fdsLengthStepRatio", out var fdsLengthStepRatioVal) && fdsLengthStepRatioVal != null)
            {
                if (fdsLengthStepRatioVal is System.Text.Json.JsonElement fdsLengthStepRatioElem)
                {
                    if (fdsLengthStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsLengthStepRatio = fdsLengthStepRatioElem.GetDouble();
                    else if (fdsLengthStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsLengthStepRatioElem.GetString();
                        if (str == "Infinity") parameters.fdsLengthStepRatio = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsLengthStepRatio = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsMaxInitialChoicesPerVariable", out var fdsMaxInitialChoicesPerVariableVal) && fdsMaxInitialChoicesPerVariableVal != null)
            {
                if (fdsMaxInitialChoicesPerVariableVal is System.Text.Json.JsonElement fdsMaxInitialChoicesPerVariableElem)
                {
                    if (fdsMaxInitialChoicesPerVariableElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsMaxInitialChoicesPerVariable = fdsMaxInitialChoicesPerVariableElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsAdditionalStepRatio", out var fdsAdditionalStepRatioVal) && fdsAdditionalStepRatioVal != null)
            {
                if (fdsAdditionalStepRatioVal is System.Text.Json.JsonElement fdsAdditionalStepRatioElem)
                {
                    if (fdsAdditionalStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsAdditionalStepRatio = fdsAdditionalStepRatioElem.GetDouble();
                    else if (fdsAdditionalStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsAdditionalStepRatioElem.GetString();
                        if (str == "Infinity") parameters.fdsAdditionalStepRatio = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsAdditionalStepRatio = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsPresenceStatusChoices", out var fdsPresenceStatusChoicesVal) && fdsPresenceStatusChoicesVal != null)
            {
                if (fdsPresenceStatusChoicesVal is System.Text.Json.JsonElement fdsPresenceStatusChoicesElem)
                {
                    if (fdsPresenceStatusChoicesElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.fdsPresenceStatusChoices = true;
                    else if (fdsPresenceStatusChoicesElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.fdsPresenceStatusChoices = false;
                }
            }

            if (data.TryGetValue("fdsMaxInitialLengthChoices", out var fdsMaxInitialLengthChoicesVal) && fdsMaxInitialLengthChoicesVal != null)
            {
                if (fdsMaxInitialLengthChoicesVal is System.Text.Json.JsonElement fdsMaxInitialLengthChoicesElem)
                {
                    if (fdsMaxInitialLengthChoicesElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsMaxInitialLengthChoices = fdsMaxInitialLengthChoicesElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsMinLengthChoiceStep", out var fdsMinLengthChoiceStepVal) && fdsMinLengthChoiceStepVal != null)
            {
                if (fdsMinLengthChoiceStepVal is System.Text.Json.JsonElement fdsMinLengthChoiceStepElem)
                {
                    if (fdsMinLengthChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsMinLengthChoiceStep = fdsMinLengthChoiceStepElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsMinIntVarChoiceStep", out var fdsMinIntVarChoiceStepVal) && fdsMinIntVarChoiceStepVal != null)
            {
                if (fdsMinIntVarChoiceStepVal is System.Text.Json.JsonElement fdsMinIntVarChoiceStepElem)
                {
                    if (fdsMinIntVarChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsMinIntVarChoiceStep = fdsMinIntVarChoiceStepElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsEventTimeInfluence", out var fdsEventTimeInfluenceVal) && fdsEventTimeInfluenceVal != null)
            {
                if (fdsEventTimeInfluenceVal is System.Text.Json.JsonElement fdsEventTimeInfluenceElem)
                {
                    if (fdsEventTimeInfluenceElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsEventTimeInfluence = fdsEventTimeInfluenceElem.GetDouble();
                    else if (fdsEventTimeInfluenceElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsEventTimeInfluenceElem.GetString();
                        if (str == "Infinity") parameters.fdsEventTimeInfluence = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsEventTimeInfluence = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsBothFailRewardFactor", out var fdsBothFailRewardFactorVal) && fdsBothFailRewardFactorVal != null)
            {
                if (fdsBothFailRewardFactorVal is System.Text.Json.JsonElement fdsBothFailRewardFactorElem)
                {
                    if (fdsBothFailRewardFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsBothFailRewardFactor = fdsBothFailRewardFactorElem.GetDouble();
                    else if (fdsBothFailRewardFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsBothFailRewardFactorElem.GetString();
                        if (str == "Infinity") parameters.fdsBothFailRewardFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsBothFailRewardFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsEpsilon", out var fdsEpsilonVal) && fdsEpsilonVal != null)
            {
                if (fdsEpsilonVal is System.Text.Json.JsonElement fdsEpsilonElem)
                {
                    if (fdsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsEpsilon = fdsEpsilonElem.GetDouble();
                    else if (fdsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsEpsilonElem.GetString();
                        if (str == "Infinity") parameters.fdsEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsStrongBranchingSize", out var fdsStrongBranchingSizeVal) && fdsStrongBranchingSizeVal != null)
            {
                if (fdsStrongBranchingSizeVal is System.Text.Json.JsonElement fdsStrongBranchingSizeElem)
                {
                    if (fdsStrongBranchingSizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsStrongBranchingSize = fdsStrongBranchingSizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsStrongBranchingDepth", out var fdsStrongBranchingDepthVal) && fdsStrongBranchingDepthVal != null)
            {
                if (fdsStrongBranchingDepthVal is System.Text.Json.JsonElement fdsStrongBranchingDepthElem)
                {
                    if (fdsStrongBranchingDepthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsStrongBranchingDepth = fdsStrongBranchingDepthElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsStrongBranchingCriterion", out var fdsStrongBranchingCriterionVal) && fdsStrongBranchingCriterionVal != null)
            {
                if (fdsStrongBranchingCriterionVal is System.Text.Json.JsonElement fdsStrongBranchingCriterionElem)
                {
                    if (fdsStrongBranchingCriterionElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.fdsStrongBranchingCriterion = fdsStrongBranchingCriterionElem.GetString();
                }
            }

            if (data.TryGetValue("fdsInitialRestartLimit", out var fdsInitialRestartLimitVal) && fdsInitialRestartLimitVal != null)
            {
                if (fdsInitialRestartLimitVal is System.Text.Json.JsonElement fdsInitialRestartLimitElem)
                {
                    if (fdsInitialRestartLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsInitialRestartLimit = fdsInitialRestartLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsRestartStrategy", out var fdsRestartStrategyVal) && fdsRestartStrategyVal != null)
            {
                if (fdsRestartStrategyVal is System.Text.Json.JsonElement fdsRestartStrategyElem)
                {
                    if (fdsRestartStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.fdsRestartStrategy = fdsRestartStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("fdsRestartGrowthFactor", out var fdsRestartGrowthFactorVal) && fdsRestartGrowthFactorVal != null)
            {
                if (fdsRestartGrowthFactorVal is System.Text.Json.JsonElement fdsRestartGrowthFactorElem)
                {
                    if (fdsRestartGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsRestartGrowthFactor = fdsRestartGrowthFactorElem.GetDouble();
                    else if (fdsRestartGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsRestartGrowthFactorElem.GetString();
                        if (str == "Infinity") parameters.fdsRestartGrowthFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.fdsRestartGrowthFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsMaxCounterAfterRestart", out var fdsMaxCounterAfterRestartVal) && fdsMaxCounterAfterRestartVal != null)
            {
                if (fdsMaxCounterAfterRestartVal is System.Text.Json.JsonElement fdsMaxCounterAfterRestartElem)
                {
                    if (fdsMaxCounterAfterRestartElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsMaxCounterAfterRestart = fdsMaxCounterAfterRestartElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsMaxCounterAfterSolution", out var fdsMaxCounterAfterSolutionVal) && fdsMaxCounterAfterSolutionVal != null)
            {
                if (fdsMaxCounterAfterSolutionVal is System.Text.Json.JsonElement fdsMaxCounterAfterSolutionElem)
                {
                    if (fdsMaxCounterAfterSolutionElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.fdsMaxCounterAfterSolution = fdsMaxCounterAfterSolutionElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsResetRestartsAfterSolution", out var fdsResetRestartsAfterSolutionVal) && fdsResetRestartsAfterSolutionVal != null)
            {
                if (fdsResetRestartsAfterSolutionVal is System.Text.Json.JsonElement fdsResetRestartsAfterSolutionElem)
                {
                    if (fdsResetRestartsAfterSolutionElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.fdsResetRestartsAfterSolution = true;
                    else if (fdsResetRestartsAfterSolutionElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.fdsResetRestartsAfterSolution = false;
                }
            }

            if (data.TryGetValue("fdsUseNogoods", out var fdsUseNogoodsVal) && fdsUseNogoodsVal != null)
            {
                if (fdsUseNogoodsVal is System.Text.Json.JsonElement fdsUseNogoodsElem)
                {
                    if (fdsUseNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.fdsUseNogoods = true;
                    else if (fdsUseNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.fdsUseNogoods = false;
                }
            }

            if (data.TryGetValue("_fdsFreezeRatingsAfterProof", out var fdsFreezeRatingsAfterProofVal) && fdsFreezeRatingsAfterProofVal != null)
            {
                if (fdsFreezeRatingsAfterProofVal is System.Text.Json.JsonElement fdsFreezeRatingsAfterProofElem)
                {
                    if (fdsFreezeRatingsAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._fdsFreezeRatingsAfterProof = true;
                    else if (fdsFreezeRatingsAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._fdsFreezeRatingsAfterProof = false;
                }
            }

            if (data.TryGetValue("_fdsContinueAfterProof", out var fdsContinueAfterProofVal) && fdsContinueAfterProofVal != null)
            {
                if (fdsContinueAfterProofVal is System.Text.Json.JsonElement fdsContinueAfterProofElem)
                {
                    if (fdsContinueAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._fdsContinueAfterProof = true;
                    else if (fdsContinueAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._fdsContinueAfterProof = false;
                }
            }

            if (data.TryGetValue("_fdsRepeatLimit", out var fdsRepeatLimitVal) && fdsRepeatLimitVal != null)
            {
                if (fdsRepeatLimitVal is System.Text.Json.JsonElement fdsRepeatLimitElem)
                {
                    if (fdsRepeatLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._fdsRepeatLimit = fdsRepeatLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_fdsCompletelyRandom", out var fdsCompletelyRandomVal) && fdsCompletelyRandomVal != null)
            {
                if (fdsCompletelyRandomVal is System.Text.Json.JsonElement fdsCompletelyRandomElem)
                {
                    if (fdsCompletelyRandomElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._fdsCompletelyRandom = true;
                    else if (fdsCompletelyRandomElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._fdsCompletelyRandom = false;
                }
            }

            if (data.TryGetValue("fdsBranchOnObjective", out var fdsBranchOnObjectiveVal) && fdsBranchOnObjectiveVal != null)
            {
                if (fdsBranchOnObjectiveVal is System.Text.Json.JsonElement fdsBranchOnObjectiveElem)
                {
                    if (fdsBranchOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.fdsBranchOnObjective = true;
                    else if (fdsBranchOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.fdsBranchOnObjective = false;
                }
            }

            if (data.TryGetValue("_fdsImproveNogoods", out var fdsImproveNogoodsVal) && fdsImproveNogoodsVal != null)
            {
                if (fdsImproveNogoodsVal is System.Text.Json.JsonElement fdsImproveNogoodsElem)
                {
                    if (fdsImproveNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._fdsImproveNogoods = true;
                    else if (fdsImproveNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._fdsImproveNogoods = false;
                }
            }

            if (data.TryGetValue("fdsBranchOrdering", out var fdsBranchOrderingVal) && fdsBranchOrderingVal != null)
            {
                if (fdsBranchOrderingVal is System.Text.Json.JsonElement fdsBranchOrderingElem)
                {
                    if (fdsBranchOrderingElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.fdsBranchOrdering = fdsBranchOrderingElem.GetString();
                }
            }

            if (data.TryGetValue("_fdsDiveBySetTimes", out var fdsDiveBySetTimesVal) && fdsDiveBySetTimesVal != null)
            {
                if (fdsDiveBySetTimesVal is System.Text.Json.JsonElement fdsDiveBySetTimesElem)
                {
                    if (fdsDiveBySetTimesElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._fdsDiveBySetTimes = true;
                    else if (fdsDiveBySetTimesElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._fdsDiveBySetTimes = false;
                }
            }

            if (data.TryGetValue("fdsDualStrategy", out var fdsDualStrategyVal) && fdsDualStrategyVal != null)
            {
                if (fdsDualStrategyVal is System.Text.Json.JsonElement fdsDualStrategyElem)
                {
                    if (fdsDualStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.fdsDualStrategy = fdsDualStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("fdsDualResetRatings", out var fdsDualResetRatingsVal) && fdsDualResetRatingsVal != null)
            {
                if (fdsDualResetRatingsVal is System.Text.Json.JsonElement fdsDualResetRatingsElem)
                {
                    if (fdsDualResetRatingsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.fdsDualResetRatings = true;
                    else if (fdsDualResetRatingsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.fdsDualResetRatings = false;
                }
            }

            if (data.TryGetValue("lnsMode", out var lnsModeVal) && lnsModeVal != null)
            {
                if (lnsModeVal is System.Text.Json.JsonElement lnsModeElem)
                {
                    if (lnsModeElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.lnsMode = lnsModeElem.GetString();
                }
            }

            if (data.TryGetValue("_lnsInitNoOverlapPropagationLevel", out var lnsInitNoOverlapPropagationLevelVal) && lnsInitNoOverlapPropagationLevelVal != null)
            {
                if (lnsInitNoOverlapPropagationLevelVal is System.Text.Json.JsonElement lnsInitNoOverlapPropagationLevelElem)
                {
                    if (lnsInitNoOverlapPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsInitNoOverlapPropagationLevel = lnsInitNoOverlapPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsInitCumulPropagationLevel", out var lnsInitCumulPropagationLevelVal) && lnsInitCumulPropagationLevelVal != null)
            {
                if (lnsInitCumulPropagationLevelVal is System.Text.Json.JsonElement lnsInitCumulPropagationLevelElem)
                {
                    if (lnsInitCumulPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsInitCumulPropagationLevel = lnsInitCumulPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsFirstFailLimit", out var lnsFirstFailLimitVal) && lnsFirstFailLimitVal != null)
            {
                if (lnsFirstFailLimitVal is System.Text.Json.JsonElement lnsFirstFailLimitElem)
                {
                    if (lnsFirstFailLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsFirstFailLimit = lnsFirstFailLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsFailLimitGrowthFactor", out var lnsFailLimitGrowthFactorVal) && lnsFailLimitGrowthFactorVal != null)
            {
                if (lnsFailLimitGrowthFactorVal is System.Text.Json.JsonElement lnsFailLimitGrowthFactorElem)
                {
                    if (lnsFailLimitGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsFailLimitGrowthFactor = lnsFailLimitGrowthFactorElem.GetDouble();
                    else if (lnsFailLimitGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsFailLimitGrowthFactorElem.GetString();
                        if (str == "Infinity") parameters._lnsFailLimitGrowthFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsFailLimitGrowthFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsFailLimitCoefficient", out var lnsFailLimitCoefficientVal) && lnsFailLimitCoefficientVal != null)
            {
                if (lnsFailLimitCoefficientVal is System.Text.Json.JsonElement lnsFailLimitCoefficientElem)
                {
                    if (lnsFailLimitCoefficientElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsFailLimitCoefficient = lnsFailLimitCoefficientElem.GetDouble();
                    else if (lnsFailLimitCoefficientElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsFailLimitCoefficientElem.GetString();
                        if (str == "Infinity") parameters._lnsFailLimitCoefficient = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsFailLimitCoefficient = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsIterationsAfterFirstSolution", out var lnsIterationsAfterFirstSolutionVal) && lnsIterationsAfterFirstSolutionVal != null)
            {
                if (lnsIterationsAfterFirstSolutionVal is System.Text.Json.JsonElement lnsIterationsAfterFirstSolutionElem)
                {
                    if (lnsIterationsAfterFirstSolutionElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsIterationsAfterFirstSolution = lnsIterationsAfterFirstSolutionElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsAggressiveDominance", out var lnsAggressiveDominanceVal) && lnsAggressiveDominanceVal != null)
            {
                if (lnsAggressiveDominanceVal is System.Text.Json.JsonElement lnsAggressiveDominanceElem)
                {
                    if (lnsAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsAggressiveDominance = true;
                    else if (lnsAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsAggressiveDominance = false;
                }
            }

            if (data.TryGetValue("_lnsSameSolutionPeriod", out var lnsSameSolutionPeriodVal) && lnsSameSolutionPeriodVal != null)
            {
                if (lnsSameSolutionPeriodVal is System.Text.Json.JsonElement lnsSameSolutionPeriodElem)
                {
                    if (lnsSameSolutionPeriodElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsSameSolutionPeriod = lnsSameSolutionPeriodElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier1Size", out var lnsTier1SizeVal) && lnsTier1SizeVal != null)
            {
                if (lnsTier1SizeVal is System.Text.Json.JsonElement lnsTier1SizeElem)
                {
                    if (lnsTier1SizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsTier1Size = lnsTier1SizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier2Size", out var lnsTier2SizeVal) && lnsTier2SizeVal != null)
            {
                if (lnsTier2SizeVal is System.Text.Json.JsonElement lnsTier2SizeElem)
                {
                    if (lnsTier2SizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsTier2Size = lnsTier2SizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier3Size", out var lnsTier3SizeVal) && lnsTier3SizeVal != null)
            {
                if (lnsTier3SizeVal is System.Text.Json.JsonElement lnsTier3SizeElem)
                {
                    if (lnsTier3SizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsTier3Size = lnsTier3SizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier2Effort", out var lnsTier2EffortVal) && lnsTier2EffortVal != null)
            {
                if (lnsTier2EffortVal is System.Text.Json.JsonElement lnsTier2EffortElem)
                {
                    if (lnsTier2EffortElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsTier2Effort = lnsTier2EffortElem.GetDouble();
                    else if (lnsTier2EffortElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsTier2EffortElem.GetString();
                        if (str == "Infinity") parameters._lnsTier2Effort = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsTier2Effort = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsTier3Effort", out var lnsTier3EffortVal) && lnsTier3EffortVal != null)
            {
                if (lnsTier3EffortVal is System.Text.Json.JsonElement lnsTier3EffortElem)
                {
                    if (lnsTier3EffortElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsTier3Effort = lnsTier3EffortElem.GetDouble();
                    else if (lnsTier3EffortElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsTier3EffortElem.GetString();
                        if (str == "Infinity") parameters._lnsTier3Effort = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsTier3Effort = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsStepFailLimitFactor", out var lnsStepFailLimitFactorVal) && lnsStepFailLimitFactorVal != null)
            {
                if (lnsStepFailLimitFactorVal is System.Text.Json.JsonElement lnsStepFailLimitFactorElem)
                {
                    if (lnsStepFailLimitFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsStepFailLimitFactor = lnsStepFailLimitFactorElem.GetDouble();
                    else if (lnsStepFailLimitFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsStepFailLimitFactorElem.GetString();
                        if (str == "Infinity") parameters._lnsStepFailLimitFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsStepFailLimitFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsApplyCutProbability", out var lnsApplyCutProbabilityVal) && lnsApplyCutProbabilityVal != null)
            {
                if (lnsApplyCutProbabilityVal is System.Text.Json.JsonElement lnsApplyCutProbabilityElem)
                {
                    if (lnsApplyCutProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsApplyCutProbability = lnsApplyCutProbabilityElem.GetDouble();
                    else if (lnsApplyCutProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsApplyCutProbabilityElem.GetString();
                        if (str == "Infinity") parameters._lnsApplyCutProbability = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsApplyCutProbability = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsSkipNoImprovementProbability", out var lnsSkipNoImprovementProbabilityVal) && lnsSkipNoImprovementProbabilityVal != null)
            {
                if (lnsSkipNoImprovementProbabilityVal is System.Text.Json.JsonElement lnsSkipNoImprovementProbabilityElem)
                {
                    if (lnsSkipNoImprovementProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsSkipNoImprovementProbability = lnsSkipNoImprovementProbabilityElem.GetDouble();
                    else if (lnsSkipNoImprovementProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsSkipNoImprovementProbabilityElem.GetString();
                        if (str == "Infinity") parameters._lnsSkipNoImprovementProbability = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsSkipNoImprovementProbability = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsSmallStructureLimit", out var lnsSmallStructureLimitVal) && lnsSmallStructureLimitVal != null)
            {
                if (lnsSmallStructureLimitVal is System.Text.Json.JsonElement lnsSmallStructureLimitElem)
                {
                    if (lnsSmallStructureLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsSmallStructureLimit = lnsSmallStructureLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsResourceOptimization", out var lnsResourceOptimizationVal) && lnsResourceOptimizationVal != null)
            {
                if (lnsResourceOptimizationVal is System.Text.Json.JsonElement lnsResourceOptimizationElem)
                {
                    if (lnsResourceOptimizationElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsResourceOptimization = true;
                    else if (lnsResourceOptimizationElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsResourceOptimization = false;
                }
            }

            if (data.TryGetValue("_lnsRestoreAbsentIntervals", out var lnsRestoreAbsentIntervalsVal) && lnsRestoreAbsentIntervalsVal != null)
            {
                if (lnsRestoreAbsentIntervalsVal is System.Text.Json.JsonElement lnsRestoreAbsentIntervalsElem)
                {
                    if (lnsRestoreAbsentIntervalsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsRestoreAbsentIntervals = true;
                    else if (lnsRestoreAbsentIntervalsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsRestoreAbsentIntervals = false;
                }
            }

            if (data.TryGetValue("_lnsRestoreIntervalLengths", out var lnsRestoreIntervalLengthsVal) && lnsRestoreIntervalLengthsVal != null)
            {
                if (lnsRestoreIntervalLengthsVal is System.Text.Json.JsonElement lnsRestoreIntervalLengthsElem)
                {
                    if (lnsRestoreIntervalLengthsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsRestoreIntervalLengths = true;
                    else if (lnsRestoreIntervalLengthsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsRestoreIntervalLengths = false;
                }
            }

            if (data.TryGetValue("_lnsRestoreIntVarValues", out var lnsRestoreIntVarValuesVal) && lnsRestoreIntVarValuesVal != null)
            {
                if (lnsRestoreIntVarValuesVal is System.Text.Json.JsonElement lnsRestoreIntVarValuesElem)
                {
                    if (lnsRestoreIntVarValuesElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsRestoreIntVarValues = true;
                    else if (lnsRestoreIntVarValuesElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsRestoreIntVarValues = false;
                }
            }

            if (data.TryGetValue("_lnsAlwaysRelaxObjective", out var lnsAlwaysRelaxObjectiveVal) && lnsAlwaysRelaxObjectiveVal != null)
            {
                if (lnsAlwaysRelaxObjectiveVal is System.Text.Json.JsonElement lnsAlwaysRelaxObjectiveElem)
                {
                    if (lnsAlwaysRelaxObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsAlwaysRelaxObjective = true;
                    else if (lnsAlwaysRelaxObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsAlwaysRelaxObjective = false;
                }
            }

            if (data.TryGetValue("lnsUseWarmStartOnly", out var lnsUseWarmStartOnlyVal) && lnsUseWarmStartOnlyVal != null)
            {
                if (lnsUseWarmStartOnlyVal is System.Text.Json.JsonElement lnsUseWarmStartOnlyElem)
                {
                    if (lnsUseWarmStartOnlyElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters.lnsUseWarmStartOnly = true;
                    else if (lnsUseWarmStartOnlyElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters.lnsUseWarmStartOnly = false;
                }
            }

            if (data.TryGetValue("_lnsHeuristicsEpsilon", out var lnsHeuristicsEpsilonVal) && lnsHeuristicsEpsilonVal != null)
            {
                if (lnsHeuristicsEpsilonVal is System.Text.Json.JsonElement lnsHeuristicsEpsilonElem)
                {
                    if (lnsHeuristicsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsHeuristicsEpsilon = lnsHeuristicsEpsilonElem.GetDouble();
                    else if (lnsHeuristicsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsEpsilonElem.GetString();
                        if (str == "Infinity") parameters._lnsHeuristicsEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsHeuristicsEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsHeuristicsAlpha", out var lnsHeuristicsAlphaVal) && lnsHeuristicsAlphaVal != null)
            {
                if (lnsHeuristicsAlphaVal is System.Text.Json.JsonElement lnsHeuristicsAlphaElem)
                {
                    if (lnsHeuristicsAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsHeuristicsAlpha = lnsHeuristicsAlphaElem.GetDouble();
                    else if (lnsHeuristicsAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsAlphaElem.GetString();
                        if (str == "Infinity") parameters._lnsHeuristicsAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsHeuristicsAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsHeuristicsTemperature", out var lnsHeuristicsTemperatureVal) && lnsHeuristicsTemperatureVal != null)
            {
                if (lnsHeuristicsTemperatureVal is System.Text.Json.JsonElement lnsHeuristicsTemperatureElem)
                {
                    if (lnsHeuristicsTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsHeuristicsTemperature = lnsHeuristicsTemperatureElem.GetDouble();
                    else if (lnsHeuristicsTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsTemperatureElem.GetString();
                        if (str == "Infinity") parameters._lnsHeuristicsTemperature = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsHeuristicsTemperature = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsHeuristicsUniform", out var lnsHeuristicsUniformVal) && lnsHeuristicsUniformVal != null)
            {
                if (lnsHeuristicsUniformVal is System.Text.Json.JsonElement lnsHeuristicsUniformElem)
                {
                    if (lnsHeuristicsUniformElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsHeuristicsUniform = true;
                    else if (lnsHeuristicsUniformElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsHeuristicsUniform = false;
                }
            }

            if (data.TryGetValue("_lnsHeuristicsInitialQ", out var lnsHeuristicsInitialQVal) && lnsHeuristicsInitialQVal != null)
            {
                if (lnsHeuristicsInitialQVal is System.Text.Json.JsonElement lnsHeuristicsInitialQElem)
                {
                    if (lnsHeuristicsInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsHeuristicsInitialQ = lnsHeuristicsInitialQElem.GetDouble();
                    else if (lnsHeuristicsInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsInitialQElem.GetString();
                        if (str == "Infinity") parameters._lnsHeuristicsInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsHeuristicsInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionEpsilon", out var lnsPortionEpsilonVal) && lnsPortionEpsilonVal != null)
            {
                if (lnsPortionEpsilonVal is System.Text.Json.JsonElement lnsPortionEpsilonElem)
                {
                    if (lnsPortionEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionEpsilon = lnsPortionEpsilonElem.GetDouble();
                    else if (lnsPortionEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionEpsilonElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionAlpha", out var lnsPortionAlphaVal) && lnsPortionAlphaVal != null)
            {
                if (lnsPortionAlphaVal is System.Text.Json.JsonElement lnsPortionAlphaElem)
                {
                    if (lnsPortionAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionAlpha = lnsPortionAlphaElem.GetDouble();
                    else if (lnsPortionAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionAlphaElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionTemperature", out var lnsPortionTemperatureVal) && lnsPortionTemperatureVal != null)
            {
                if (lnsPortionTemperatureVal is System.Text.Json.JsonElement lnsPortionTemperatureElem)
                {
                    if (lnsPortionTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionTemperature = lnsPortionTemperatureElem.GetDouble();
                    else if (lnsPortionTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionTemperatureElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionTemperature = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionTemperature = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionUniform", out var lnsPortionUniformVal) && lnsPortionUniformVal != null)
            {
                if (lnsPortionUniformVal is System.Text.Json.JsonElement lnsPortionUniformElem)
                {
                    if (lnsPortionUniformElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsPortionUniform = true;
                    else if (lnsPortionUniformElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsPortionUniform = false;
                }
            }

            if (data.TryGetValue("_lnsPortionInitialQ", out var lnsPortionInitialQVal) && lnsPortionInitialQVal != null)
            {
                if (lnsPortionInitialQVal is System.Text.Json.JsonElement lnsPortionInitialQElem)
                {
                    if (lnsPortionInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionInitialQ = lnsPortionInitialQElem.GetDouble();
                    else if (lnsPortionInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionInitialQElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionHandicapLimit", out var lnsPortionHandicapLimitVal) && lnsPortionHandicapLimitVal != null)
            {
                if (lnsPortionHandicapLimitVal is System.Text.Json.JsonElement lnsPortionHandicapLimitElem)
                {
                    if (lnsPortionHandicapLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionHandicapLimit = lnsPortionHandicapLimitElem.GetDouble();
                    else if (lnsPortionHandicapLimitElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionHandicapLimitElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionHandicapLimit = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionHandicapLimit = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionHandicapValue", out var lnsPortionHandicapValueVal) && lnsPortionHandicapValueVal != null)
            {
                if (lnsPortionHandicapValueVal is System.Text.Json.JsonElement lnsPortionHandicapValueElem)
                {
                    if (lnsPortionHandicapValueElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionHandicapValue = lnsPortionHandicapValueElem.GetDouble();
                    else if (lnsPortionHandicapValueElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionHandicapValueElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionHandicapValue = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionHandicapValue = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionHandicapInitialQ", out var lnsPortionHandicapInitialQVal) && lnsPortionHandicapInitialQVal != null)
            {
                if (lnsPortionHandicapInitialQVal is System.Text.Json.JsonElement lnsPortionHandicapInitialQElem)
                {
                    if (lnsPortionHandicapInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionHandicapInitialQ = lnsPortionHandicapInitialQElem.GetDouble();
                    else if (lnsPortionHandicapInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionHandicapInitialQElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionHandicapInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionHandicapInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodStrategy", out var lnsNeighborhoodStrategyVal) && lnsNeighborhoodStrategyVal != null)
            {
                if (lnsNeighborhoodStrategyVal is System.Text.Json.JsonElement lnsNeighborhoodStrategyElem)
                {
                    if (lnsNeighborhoodStrategyElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsNeighborhoodStrategy = lnsNeighborhoodStrategyElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodEpsilon", out var lnsNeighborhoodEpsilonVal) && lnsNeighborhoodEpsilonVal != null)
            {
                if (lnsNeighborhoodEpsilonVal is System.Text.Json.JsonElement lnsNeighborhoodEpsilonElem)
                {
                    if (lnsNeighborhoodEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsNeighborhoodEpsilon = lnsNeighborhoodEpsilonElem.GetDouble();
                    else if (lnsNeighborhoodEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodEpsilonElem.GetString();
                        if (str == "Infinity") parameters._lnsNeighborhoodEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsNeighborhoodEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodAlpha", out var lnsNeighborhoodAlphaVal) && lnsNeighborhoodAlphaVal != null)
            {
                if (lnsNeighborhoodAlphaVal is System.Text.Json.JsonElement lnsNeighborhoodAlphaElem)
                {
                    if (lnsNeighborhoodAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsNeighborhoodAlpha = lnsNeighborhoodAlphaElem.GetDouble();
                    else if (lnsNeighborhoodAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodAlphaElem.GetString();
                        if (str == "Infinity") parameters._lnsNeighborhoodAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsNeighborhoodAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodTemperature", out var lnsNeighborhoodTemperatureVal) && lnsNeighborhoodTemperatureVal != null)
            {
                if (lnsNeighborhoodTemperatureVal is System.Text.Json.JsonElement lnsNeighborhoodTemperatureElem)
                {
                    if (lnsNeighborhoodTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsNeighborhoodTemperature = lnsNeighborhoodTemperatureElem.GetDouble();
                    else if (lnsNeighborhoodTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodTemperatureElem.GetString();
                        if (str == "Infinity") parameters._lnsNeighborhoodTemperature = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsNeighborhoodTemperature = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodUniform", out var lnsNeighborhoodUniformVal) && lnsNeighborhoodUniformVal != null)
            {
                if (lnsNeighborhoodUniformVal is System.Text.Json.JsonElement lnsNeighborhoodUniformElem)
                {
                    if (lnsNeighborhoodUniformElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsNeighborhoodUniform = true;
                    else if (lnsNeighborhoodUniformElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsNeighborhoodUniform = false;
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodInitialQ", out var lnsNeighborhoodInitialQVal) && lnsNeighborhoodInitialQVal != null)
            {
                if (lnsNeighborhoodInitialQVal is System.Text.Json.JsonElement lnsNeighborhoodInitialQElem)
                {
                    if (lnsNeighborhoodInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsNeighborhoodInitialQ = lnsNeighborhoodInitialQElem.GetDouble();
                    else if (lnsNeighborhoodInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodInitialQElem.GetString();
                        if (str == "Infinity") parameters._lnsNeighborhoodInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsNeighborhoodInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsDivingLimit", out var lnsDivingLimitVal) && lnsDivingLimitVal != null)
            {
                if (lnsDivingLimitVal is System.Text.Json.JsonElement lnsDivingLimitElem)
                {
                    if (lnsDivingLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsDivingLimit = lnsDivingLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsDivingFailLimitRatio", out var lnsDivingFailLimitRatioVal) && lnsDivingFailLimitRatioVal != null)
            {
                if (lnsDivingFailLimitRatioVal is System.Text.Json.JsonElement lnsDivingFailLimitRatioElem)
                {
                    if (lnsDivingFailLimitRatioElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsDivingFailLimitRatio = lnsDivingFailLimitRatioElem.GetDouble();
                    else if (lnsDivingFailLimitRatioElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsDivingFailLimitRatioElem.GetString();
                        if (str == "Infinity") parameters._lnsDivingFailLimitRatio = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsDivingFailLimitRatio = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsLearningRun", out var lnsLearningRunVal) && lnsLearningRunVal != null)
            {
                if (lnsLearningRunVal is System.Text.Json.JsonElement lnsLearningRunElem)
                {
                    if (lnsLearningRunElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsLearningRun = true;
                    else if (lnsLearningRunElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsLearningRun = false;
                }
            }

            if (data.TryGetValue("_lnsStayOnObjective", out var lnsStayOnObjectiveVal) && lnsStayOnObjectiveVal != null)
            {
                if (lnsStayOnObjectiveVal is System.Text.Json.JsonElement lnsStayOnObjectiveElem)
                {
                    if (lnsStayOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsStayOnObjective = true;
                    else if (lnsStayOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsStayOnObjective = false;
                }
            }

            if (data.TryGetValue("_lnsFDS", out var lnsFDSVal) && lnsFDSVal != null)
            {
                if (lnsFDSVal is System.Text.Json.JsonElement lnsFDSElem)
                {
                    if (lnsFDSElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._lnsFDS = true;
                    else if (lnsFDSElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._lnsFDS = false;
                }
            }

            if (data.TryGetValue("_lnsFragmentFreezing", out var lnsFragmentFreezingVal) && lnsFragmentFreezingVal != null)
            {
                if (lnsFragmentFreezingVal is System.Text.Json.JsonElement lnsFragmentFreezingElem)
                {
                    if (lnsFragmentFreezingElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters._lnsFragmentFreezing = lnsFragmentFreezingElem.GetString();
                }
            }

            if (data.TryGetValue("_lnsRelaxSlack", out var lnsRelaxSlackVal) && lnsRelaxSlackVal != null)
            {
                if (lnsRelaxSlackVal is System.Text.Json.JsonElement lnsRelaxSlackElem)
                {
                    if (lnsRelaxSlackElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsRelaxSlack = lnsRelaxSlackElem.GetDouble();
                    else if (lnsRelaxSlackElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsRelaxSlackElem.GetString();
                        if (str == "Infinity") parameters._lnsRelaxSlack = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsRelaxSlack = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionMultiplier", out var lnsPortionMultiplierVal) && lnsPortionMultiplierVal != null)
            {
                if (lnsPortionMultiplierVal is System.Text.Json.JsonElement lnsPortionMultiplierElem)
                {
                    if (lnsPortionMultiplierElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsPortionMultiplier = lnsPortionMultiplierElem.GetDouble();
                    else if (lnsPortionMultiplierElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionMultiplierElem.GetString();
                        if (str == "Infinity") parameters._lnsPortionMultiplier = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsPortionMultiplier = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("simpleLBWorker", out var simpleLBWorkerVal) && simpleLBWorkerVal != null)
            {
                if (simpleLBWorkerVal is System.Text.Json.JsonElement simpleLBWorkerElem)
                {
                    if (simpleLBWorkerElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.simpleLBWorker = simpleLBWorkerElem.GetInt64();
                }
            }

            if (data.TryGetValue("simpleLBMaxIterations", out var simpleLBMaxIterationsVal) && simpleLBMaxIterationsVal != null)
            {
                if (simpleLBMaxIterationsVal is System.Text.Json.JsonElement simpleLBMaxIterationsElem)
                {
                    if (simpleLBMaxIterationsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.simpleLBMaxIterations = simpleLBMaxIterationsElem.GetInt64();
                }
            }

            if (data.TryGetValue("simpleLBShavingRounds", out var simpleLBShavingRoundsVal) && simpleLBShavingRoundsVal != null)
            {
                if (simpleLBShavingRoundsVal is System.Text.Json.JsonElement simpleLBShavingRoundsElem)
                {
                    if (simpleLBShavingRoundsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.simpleLBShavingRounds = simpleLBShavingRoundsElem.GetInt64();
                }
            }

            if (data.TryGetValue("_debugTraceLevel", out var debugTraceLevelVal) && debugTraceLevelVal != null)
            {
                if (debugTraceLevelVal is System.Text.Json.JsonElement debugTraceLevelElem)
                {
                    if (debugTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._debugTraceLevel = debugTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_memoryTraceLevel", out var memoryTraceLevelVal) && memoryTraceLevelVal != null)
            {
                if (memoryTraceLevelVal is System.Text.Json.JsonElement memoryTraceLevelElem)
                {
                    if (memoryTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._memoryTraceLevel = memoryTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_propagationDetailTraceLevel", out var propagationDetailTraceLevelVal) && propagationDetailTraceLevelVal != null)
            {
                if (propagationDetailTraceLevelVal is System.Text.Json.JsonElement propagationDetailTraceLevelElem)
                {
                    if (propagationDetailTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._propagationDetailTraceLevel = propagationDetailTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_setTimesTraceLevel", out var setTimesTraceLevelVal) && setTimesTraceLevelVal != null)
            {
                if (setTimesTraceLevelVal is System.Text.Json.JsonElement setTimesTraceLevelElem)
                {
                    if (setTimesTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._setTimesTraceLevel = setTimesTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_communicationTraceLevel", out var communicationTraceLevelVal) && communicationTraceLevelVal != null)
            {
                if (communicationTraceLevelVal is System.Text.Json.JsonElement communicationTraceLevelElem)
                {
                    if (communicationTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._communicationTraceLevel = communicationTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_presolveTraceLevel", out var presolveTraceLevelVal) && presolveTraceLevelVal != null)
            {
                if (presolveTraceLevelVal is System.Text.Json.JsonElement presolveTraceLevelElem)
                {
                    if (presolveTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._presolveTraceLevel = presolveTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_conversionTraceLevel", out var conversionTraceLevelVal) && conversionTraceLevelVal != null)
            {
                if (conversionTraceLevelVal is System.Text.Json.JsonElement conversionTraceLevelElem)
                {
                    if (conversionTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._conversionTraceLevel = conversionTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_expressionBuilderTraceLevel", out var expressionBuilderTraceLevelVal) && expressionBuilderTraceLevelVal != null)
            {
                if (expressionBuilderTraceLevelVal is System.Text.Json.JsonElement expressionBuilderTraceLevelElem)
                {
                    if (expressionBuilderTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._expressionBuilderTraceLevel = expressionBuilderTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_memorizationTraceLevel", out var memorizationTraceLevelVal) && memorizationTraceLevelVal != null)
            {
                if (memorizationTraceLevelVal is System.Text.Json.JsonElement memorizationTraceLevelElem)
                {
                    if (memorizationTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._memorizationTraceLevel = memorizationTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_searchDetailTraceLevel", out var searchDetailTraceLevelVal) && searchDetailTraceLevelVal != null)
            {
                if (searchDetailTraceLevelVal is System.Text.Json.JsonElement searchDetailTraceLevelElem)
                {
                    if (searchDetailTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._searchDetailTraceLevel = searchDetailTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_fdsTraceLevel", out var fdsTraceLevelVal) && fdsTraceLevelVal != null)
            {
                if (fdsTraceLevelVal is System.Text.Json.JsonElement fdsTraceLevelElem)
                {
                    if (fdsTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._fdsTraceLevel = fdsTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_shavingTraceLevel", out var shavingTraceLevelVal) && shavingTraceLevelVal != null)
            {
                if (shavingTraceLevelVal is System.Text.Json.JsonElement shavingTraceLevelElem)
                {
                    if (shavingTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._shavingTraceLevel = shavingTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_fdsRatingsTraceLevel", out var fdsRatingsTraceLevelVal) && fdsRatingsTraceLevelVal != null)
            {
                if (fdsRatingsTraceLevelVal is System.Text.Json.JsonElement fdsRatingsTraceLevelElem)
                {
                    if (fdsRatingsTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._fdsRatingsTraceLevel = fdsRatingsTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTraceLevel", out var lnsTraceLevelVal) && lnsTraceLevelVal != null)
            {
                if (lnsTraceLevelVal is System.Text.Json.JsonElement lnsTraceLevelElem)
                {
                    if (lnsTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsTraceLevel = lnsTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_heuristicReplayTraceLevel", out var heuristicReplayTraceLevelVal) && heuristicReplayTraceLevelVal != null)
            {
                if (heuristicReplayTraceLevelVal is System.Text.Json.JsonElement heuristicReplayTraceLevelElem)
                {
                    if (heuristicReplayTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._heuristicReplayTraceLevel = heuristicReplayTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_allowSetTimesProofs", out var allowSetTimesProofsVal) && allowSetTimesProofsVal != null)
            {
                if (allowSetTimesProofsVal is System.Text.Json.JsonElement allowSetTimesProofsElem)
                {
                    if (allowSetTimesProofsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._allowSetTimesProofs = true;
                    else if (allowSetTimesProofsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._allowSetTimesProofs = false;
                }
            }

            if (data.TryGetValue("_setTimesAggressiveDominance", out var setTimesAggressiveDominanceVal) && setTimesAggressiveDominanceVal != null)
            {
                if (setTimesAggressiveDominanceVal is System.Text.Json.JsonElement setTimesAggressiveDominanceElem)
                {
                    if (setTimesAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._setTimesAggressiveDominance = true;
                    else if (setTimesAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._setTimesAggressiveDominance = false;
                }
            }

            if (data.TryGetValue("_setTimesExtendsCoef", out var setTimesExtendsCoefVal) && setTimesExtendsCoefVal != null)
            {
                if (setTimesExtendsCoefVal is System.Text.Json.JsonElement setTimesExtendsCoefElem)
                {
                    if (setTimesExtendsCoefElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._setTimesExtendsCoef = setTimesExtendsCoefElem.GetDouble();
                    else if (setTimesExtendsCoefElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = setTimesExtendsCoefElem.GetString();
                        if (str == "Infinity") parameters._setTimesExtendsCoef = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._setTimesExtendsCoef = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_setTimesHeightStrategy", out var setTimesHeightStrategyVal) && setTimesHeightStrategyVal != null)
            {
                if (setTimesHeightStrategyVal is System.Text.Json.JsonElement setTimesHeightStrategyElem)
                {
                    if (setTimesHeightStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters._setTimesHeightStrategy = setTimesHeightStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("_setTimesItvMappingStrategy", out var setTimesItvMappingStrategyVal) && setTimesItvMappingStrategyVal != null)
            {
                if (setTimesItvMappingStrategyVal is System.Text.Json.JsonElement setTimesItvMappingStrategyElem)
                {
                    if (setTimesItvMappingStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters._setTimesItvMappingStrategy = setTimesItvMappingStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("_setTimesInitDensity", out var setTimesInitDensityVal) && setTimesInitDensityVal != null)
            {
                if (setTimesInitDensityVal is System.Text.Json.JsonElement setTimesInitDensityElem)
                {
                    if (setTimesInitDensityElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._setTimesInitDensity = setTimesInitDensityElem.GetDouble();
                    else if (setTimesInitDensityElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = setTimesInitDensityElem.GetString();
                        if (str == "Infinity") parameters._setTimesInitDensity = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._setTimesInitDensity = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_setTimesDensityLength", out var setTimesDensityLengthVal) && setTimesDensityLengthVal != null)
            {
                if (setTimesDensityLengthVal is System.Text.Json.JsonElement setTimesDensityLengthElem)
                {
                    if (setTimesDensityLengthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._setTimesDensityLength = setTimesDensityLengthElem.GetInt64();
                }
            }

            if (data.TryGetValue("_setTimesDensityReliabilityThreshold", out var setTimesDensityReliabilityThresholdVal) && setTimesDensityReliabilityThresholdVal != null)
            {
                if (setTimesDensityReliabilityThresholdVal is System.Text.Json.JsonElement setTimesDensityReliabilityThresholdElem)
                {
                    if (setTimesDensityReliabilityThresholdElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._setTimesDensityReliabilityThreshold = setTimesDensityReliabilityThresholdElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsDisabledHeuristics", out var lnsDisabledHeuristicsVal) && lnsDisabledHeuristicsVal != null)
            {
                if (lnsDisabledHeuristicsVal is System.Text.Json.JsonElement lnsDisabledHeuristicsElem)
                {
                    if (lnsDisabledHeuristicsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsDisabledHeuristics = lnsDisabledHeuristicsElem.GetInt64();
                }
            }

            if (data.TryGetValue("_setTimesNbExtendsFactor", out var setTimesNbExtendsFactorVal) && setTimesNbExtendsFactorVal != null)
            {
                if (setTimesNbExtendsFactorVal is System.Text.Json.JsonElement setTimesNbExtendsFactorElem)
                {
                    if (setTimesNbExtendsFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._setTimesNbExtendsFactor = setTimesNbExtendsFactorElem.GetDouble();
                    else if (setTimesNbExtendsFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = setTimesNbExtendsFactorElem.GetString();
                        if (str == "Infinity") parameters._setTimesNbExtendsFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._setTimesNbExtendsFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_discreteLowCapacityLimit", out var discreteLowCapacityLimitVal) && discreteLowCapacityLimitVal != null)
            {
                if (discreteLowCapacityLimitVal is System.Text.Json.JsonElement discreteLowCapacityLimitElem)
                {
                    if (discreteLowCapacityLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._discreteLowCapacityLimit = discreteLowCapacityLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTrainingObjectiveLimit", out var lnsTrainingObjectiveLimitVal) && lnsTrainingObjectiveLimitVal != null)
            {
                if (lnsTrainingObjectiveLimitVal is System.Text.Json.JsonElement lnsTrainingObjectiveLimitElem)
                {
                    if (lnsTrainingObjectiveLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._lnsTrainingObjectiveLimit = lnsTrainingObjectiveLimitElem.GetDouble();
                    else if (lnsTrainingObjectiveLimitElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsTrainingObjectiveLimitElem.GetString();
                        if (str == "Infinity") parameters._lnsTrainingObjectiveLimit = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._lnsTrainingObjectiveLimit = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_posAbsentRelated", out var posAbsentRelatedVal) && posAbsentRelatedVal != null)
            {
                if (posAbsentRelatedVal is System.Text.Json.JsonElement posAbsentRelatedElem)
                {
                    if (posAbsentRelatedElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._posAbsentRelated = true;
                    else if (posAbsentRelatedElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._posAbsentRelated = false;
                }
            }

            if (data.TryGetValue("_defaultCallbackBlockSize", out var defaultCallbackBlockSizeVal) && defaultCallbackBlockSizeVal != null)
            {
                if (defaultCallbackBlockSizeVal is System.Text.Json.JsonElement defaultCallbackBlockSizeElem)
                {
                    if (defaultCallbackBlockSizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._defaultCallbackBlockSize = defaultCallbackBlockSizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_useReservoirPegging", out var useReservoirPeggingVal) && useReservoirPeggingVal != null)
            {
                if (useReservoirPeggingVal is System.Text.Json.JsonElement useReservoirPeggingElem)
                {
                    if (useReservoirPeggingElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._useReservoirPegging = true;
                    else if (useReservoirPeggingElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._useReservoirPegging = false;
                }
            }

            if (data.TryGetValue("_useTimeNet", out var useTimeNetVal) && useTimeNetVal != null)
            {
                if (useTimeNetVal is System.Text.Json.JsonElement useTimeNetElem)
                {
                    if (useTimeNetElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        parameters._useTimeNet = true;
                    else if (useTimeNetElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        parameters._useTimeNet = false;
                }
            }

            if (data.TryGetValue("_timeNetVarsToPreprocess", out var timeNetVarsToPreprocessVal) && timeNetVarsToPreprocessVal != null)
            {
                if (timeNetVarsToPreprocessVal is System.Text.Json.JsonElement timeNetVarsToPreprocessElem)
                {
                    if (timeNetVarsToPreprocessElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._timeNetVarsToPreprocess = timeNetVarsToPreprocessElem.GetInt64();
                }
            }

            if (data.TryGetValue("_timeNetSubPriorityBits", out var timeNetSubPriorityBitsVal) && timeNetSubPriorityBitsVal != null)
            {
                if (timeNetSubPriorityBitsVal is System.Text.Json.JsonElement timeNetSubPriorityBitsElem)
                {
                    if (timeNetSubPriorityBitsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._timeNetSubPriorityBits = timeNetSubPriorityBitsElem.GetInt64();
                }
            }

            if (data.TryGetValue("_minChainLength", out var minChainLengthVal) && minChainLengthVal != null)
            {
                if (minChainLengthVal is System.Text.Json.JsonElement minChainLengthElem)
                {
                    if (minChainLengthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._minChainLength = minChainLengthElem.GetInt64();
                }
            }

            if (data.TryGetValue("_monitorInterval", out var monitorIntervalVal) && monitorIntervalVal != null)
            {
                if (monitorIntervalVal is System.Text.Json.JsonElement monitorIntervalElem)
                {
                    if (monitorIntervalElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._monitorInterval = monitorIntervalElem.GetDouble();
                    else if (monitorIntervalElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = monitorIntervalElem.GetString();
                        if (str == "Infinity") parameters._monitorInterval = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._monitorInterval = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_monitorThreshold", out var monitorThresholdVal) && monitorThresholdVal != null)
            {
                if (monitorThresholdVal is System.Text.Json.JsonElement monitorThresholdElem)
                {
                    if (monitorThresholdElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters._monitorThreshold = monitorThresholdElem.GetDouble();
                    else if (monitorThresholdElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = monitorThresholdElem.GetString();
                        if (str == "Infinity") parameters._monitorThreshold = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters._monitorThreshold = double.NegativeInfinity;
                    }
                }
            }



            // Deserialize workers
            if (data.TryGetValue("workers", out var workersVal) && workersVal != null)
            {
                if (workersVal is System.Text.Json.JsonElement workersElem
                    && workersElem.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    parameters.workers = new List<WorkerParameters>();
                    foreach (var workerElem in workersElem.EnumerateArray())
                    {
                        if (workerElem.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            var workerDict = new Dictionary<string, object?>();
                            foreach (var prop in workerElem.EnumerateObject())
                                workerDict[prop.Name] = prop.Value;
                            parameters.workers.Add(WorkerParameters.FromDict(workerDict));
                        }
                    }
                }
            }

            return parameters;
        }

        private const string ParametersHelpText = @"Help:
          --help, -h                       Print this help
          --optalcpVersion                 Print OptalCP version information

        Terminal output:
          --color Never|Auto|Always        Whether to colorize output to the terminal

        Major options:
          --nbWorkers uint32               Number of threads dedicated to search
          --preset Auto|Default|Large      Preset configuration for solver parameters
          --searchType Auto|LNS|FDS|FDSDual|SetTimes|FDSLB
                                           Type of search to use
          --randomSeed uint32              Random seed
          --logLevel uint32                Level of the log
          --warningLevel uint32            Level of warnings
          --logPeriod double               How often to print log messages (in seconds)
          --verifySolutions bool           When on, the correctness of solutions is verified
          --verifyExternalSolutions bool   Whether to verify corectness of external solutions
          --allocationBlockSize uint32     The minimal amount of memory in kB for a single allocation
          --processExitTimeout double      Timeout for solver process to exit after finishing

        Limits:
          --timeLimit double               Wall clock limit for execution
          --solutionLimit uint64           Stop the search after the given number of solutions

        Gap Tolerance:
          --absoluteGapTolerance double    Stop the search when the gap is below the tolerance
          --relativeGapTolerance double    Stop the search when the gap is below the tolerance

        Propagation levels:
          --noOverlapPropagationLevel uint32
                                           How much to propagate noOverlap constraints
          --cumulPropagationLevel uint32   How much to propagate constraints on cumul functions
          --reservoirPropagationLevel uint32
                                           How much to propagate constraints on cumul functions
          --positionPropagationLevel uint32
                                           How much to propagate position expressions on noOverlap constraints
          --integralPropagationLevel uint32
                                           How much to propagate stepFunctionSum expression
          --usePrecedenceEnergy uint32     Whether to use precedence energy propagation algorithm

        Failure-Directed Search:
          --fdsInitialRating double        Initial rating for newly created choices
          --fdsReductionWeight double      Weight of the reduction factor in rating computation
          --fdsRatingAverageLength int32   Length of average rating computed for choices
          --fdsFixedAlpha double           When non-zero, alpha factor for rating updates
          --fdsRatingAverageComparison Off|Global|Depth
                                           Whether to compare the local rating with the average
          --fdsReductionFactor Normal|Zero|Random
                                           Reduction factor R for rating computation
          --fdsReuseClosing bool           Whether always reuse closing choice
          --fdsUniformChoiceStep bool      Whether all initial choices have the same step length
          --fdsLengthStepRatio double      Choice step relative to average length
          --fdsMaxInitialChoicesPerVariable uint32
                                           Maximum number of choices generated initially per a variable
          --fdsAdditionalStepRatio double  Domain split ratio when run out of choices
          --fdsPresenceStatusChoices bool  Whether to generate choices on presence status
          --fdsMaxInitialLengthChoices uint32
                                           Maximum number of initial choices on length of an interval variable
          --fdsMinLengthChoiceStep uint32  Maximum step when generating initial choices for length of an interval variable
          --fdsMinIntVarChoiceStep uint32  Minimum step when generating choices for integer variables.
          --fdsEventTimeInfluence double   Influence of event time to initial choice rating
          --fdsBothFailRewardFactor double
                                           How much to improve rating when both branches fail immediately
          --fdsEpsilon double              How often to chose a choice randomly
          --fdsStrongBranchingSize uint32  Number of choices to try in strong branching
          --fdsStrongBranchingDepth uint32
                                           Up-to what search depth apply strong branching
          --fdsStrongBranchingCriterion Both|Left|Right
                                           How to choose the best choice in strong branching
          --fdsInitialRestartLimit uint64  Fail limit for the first restart
          --fdsRestartStrategy Geometric|Nested|Luby
                                           Restart strategy to use
          --fdsRestartGrowthFactor double  Growth factor for fail limit after each restart
          --fdsMaxCounterAfterRestart uint8
                                           Truncate choice use counts after a restart to this value
          --fdsMaxCounterAfterSolution uint8
                                           Truncate choice use counts after a solution is found
          --fdsResetRestartsAfterSolution bool
                                           Reset restart size after a solution is found (ignored in Luby)
          --fdsUseNogoods bool             Whether to use or not nogood constraints
          --fdsBranchOnObjective bool      Whether to generate choices for objective expression/variable
          --fdsBranchOrdering FailureFirst|FailureLast|Random
                                           Controls which side of a choice is is explored first (considering the rating).
          --fdsDualStrategy Minimum|Random|Split
                                           A strategy to choose objective cuts during FDSDual search.
          --fdsDualResetRatings bool       Whether to reset ratings when a new LB is proved

        Large Neighborhood Search:
          --lnsMode Robust|Focused         LNS solution pool strategy
          --lnsUseWarmStartOnly bool       Use only the user-provided warm start as the initial solution in LNS

        Simple Lower Bound:
          --simpleLBWorker int32           Which worker computes simple lower bound
          --simpleLBMaxIterations uint32   Maximum number of feasibility checks
          --simpleLBShavingRounds uint32   Number of shaving rounds";


        /// <summary>
        /// Parses OptalCP solver parameters from the command line.
        /// </summary>
        /// <param name="args">Command-line arguments to parse. Defaults to Environment.GetCommandLineArgs() (skipping the executable name).</param>
        /// <param name="defaults">Default parameter values. CLI arguments override these.</param>
        /// <param name="usage">Custom usage text shown before the parameter list when --help is used.</param>
        /// <param name="exitOnError">If true (default), exits on error or --help. If false, throws instead.</param>
        /// <returns>The parsed parameters object.</returns>
        /// <remarks>
        /// Parses OptalCP solver parameters from the command line and returns a
        /// <see cref="Parameters">Parameters</see> object ready for use with <see cref="Model.Solve">Model.Solve</see>.
        ///
        /// Instead of hardcoding solver settings like time limits or worker counts in your code,
        /// you can let users configure them when running your application. For example, running
        /// <c>dotnet run -- --timeLimit 120 --nbWorkers 8</c> would override any defaults. This makes
        /// your application flexible without requiring code changes for different scenarios.
        ///
        /// The <c>defaults</c> argument lets you specify sensible default values for your application.
        /// When users don't provide a parameter on the command line, the default value is used.
        ///
        /// By default (<c>exitOnError: true</c>), parse errors and <c>--help</c>/<c>--optalcpVersion</c> flags
        /// cause the process to exit. Set <c>exitOnError: false</c> to throw instead.
        ///
        /// If <c>--help</c> or <c>-h</c> is given, the method prints help starting with the <c>usage</c>
        /// argument (if provided), followed by the list of recognized parameters:
        ///
        /// <see cref="WorkerParameters">WorkerParameters</see> can be specified for individual worker(s) using the following prefixes:
        ///
        /// - <c>--workerN.</c> or <c>--workersN.</c> for worker <c>N</c>
        /// - <c>--workerN-M.</c> or <c>--workersN-M.</c> for workers in the range <c>N</c> to <c>M</c>
        ///
        /// For example:
        ///
        /// - <c>--worker0.searchType FDS</c> sets the search type for the first worker only.
        /// - <c>--workers4-8.noOverlapPropagationLevel 4</c> sets the propagation level of <c>noOverlap</c> constraint for workers 4, 5, 6, 7, and 8.
        ///
        /// This method does not accept unrecognized arguments (they cause an error).
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// // Parse with defaults that CLI can override
        /// var parameters = Parameters.ParseParameters(
        ///     defaults: new Parameters { timeLimit = 60, nbWorkers = 4 },
        ///     usage: "Usage: myapp [OPTIONS] &lt;input-file&gt;\n\nSolve scheduling problem."
        /// );
        ///
        /// var result = model.Solve(parameters);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="ParseKnownParameters">ParseKnownParameters</see> — to handle unrecognized arguments.</description></item>
        /// </list>
        /// </remarks>
        public static Parameters ParseParameters(
            string[]? args = null,
            Parameters? defaults = null,
            string? usage = null,
            bool exitOnError = true)
        {
            args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
            var parameters = defaults != null ? CopyParameters(defaults) : new Parameters();

            HandleHelpFlags(args, usage, exitOnError);

            try
            {
                ParseArgs(parameters, args, allowUnknown: false);
            }
            catch (ArgumentException e) when (exitOnError)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
                Environment.Exit(1);
            }

            return parameters;
        }

        /// <summary>
        /// Parses OptalCP solver parameters from the command line, passing through unrecognized arguments.
        /// </summary>
        /// <param name="args">Command-line arguments to parse. Defaults to Environment.GetCommandLineArgs() (skipping the executable name).</param>
        /// <param name="defaults">Default parameter values. CLI arguments override these.</param>
        /// <param name="usage">Custom usage text shown before the parameter list when --help is used.</param>
        /// <param name="exitOnError">If true (default), exits on error or --help. If false, throws instead.</param>
        /// <returns>A tuple containing the parsed parameters and the list of unrecognized arguments.</returns>
        /// <remarks>
        /// Like <see cref="ParseParameters">ParseParameters</see>, but unrecognized arguments are collected and returned
        /// instead of causing an error.
        ///
        /// This is useful when your application has its own command-line arguments alongside
        /// OptalCP solver parameters. For example, running
        /// <c>dotnet run -- --timeLimit 120 input.txt</c> would parse <c>--timeLimit 120</c> as a solver
        /// parameter and return <c>input.txt</c> as an unrecognized argument for your code to handle.
        ///
        /// The <c>defaults</c> argument lets you specify sensible default values for your application.
        /// When users don't provide a parameter on the command line, the default value is used.
        ///
        /// By default (<c>exitOnError: true</c>), parse errors and <c>--help</c>/<c>--optalcpVersion</c> flags
        /// cause the process to exit. Set <c>exitOnError: false</c> to throw instead, which is useful
        /// for testing or custom error handling.
        ///
        /// If <c>--help</c> or <c>-h</c> is given, the method prints help starting with the <c>usage</c>
        /// argument (if provided), followed by the list of recognized parameters:
        ///
        /// <see cref="WorkerParameters">WorkerParameters</see> can be specified for individual workers using <c>--workerN.</c> prefix.
        /// For example, <c>--worker0.searchType FDS</c> sets the search type for the first worker only.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// // Parse solver parameters, collect input files as unrecognized args
        /// var (parameters, inputFiles) = Parameters.ParseKnownParameters(
        ///     defaults: new Parameters { timeLimit = 60 },
        ///     usage: "Usage: myapp [OPTIONS] &lt;input-file&gt;..."
        /// );
        ///
        /// foreach (var file in inputFiles)
        /// {
        ///     // Load and solve each input file:
        ///     var (model, _, _) = Model.FromJSON(File.ReadAllText(file));
        ///     model.Solve(parameters);
        /// }
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="ParseParameters">ParseParameters</see> — to reject unrecognized arguments with an error.</description></item>
        /// </list>
        /// </remarks>
        public static (Parameters Parameters, List<string> Unrecognized) ParseKnownParameters(
            string[]? args = null,
            Parameters? defaults = null,
            string? usage = null,
            bool exitOnError = true)
        {
            args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
            var parameters = defaults != null ? CopyParameters(defaults) : new Parameters();

            HandleHelpFlags(args, usage, exitOnError);

            List<string> unrecognized;
            try
            {
                unrecognized = ParseArgs(parameters, args, allowUnknown: true);
            }
            catch (ArgumentException e) when (exitOnError)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
                Environment.Exit(1);
                return (parameters, new List<string>());
            }

            return (parameters, unrecognized);
        }

        private static void HandleHelpFlags(string[] args, string? usage, bool exitOnError)
        {
            bool showHelp = args.Contains("--help") || args.Contains("-h");
            bool showVersion = args.Any(a => a.Equals("--optalcpVersion", StringComparison.OrdinalIgnoreCase));

            if (!showHelp && !showVersion)
                return;

            if (showHelp)
            {
                if (usage != null)
                    Console.WriteLine(usage + "\n");
                else
                    Console.WriteLine($"Usage: {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "program"} [options]\n");
                Console.WriteLine(ParametersHelpText);
            }

            if (showVersion)
            {
                try
                {
                    var solverPath = Solver.FindSolver();
                    var psi = new ProcessStartInfo
                    {
                        FileName = solverPath,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false
                    };
                    using var proc = Process.Start(psi);
                    proc?.WaitForExit(5000);
                }
                catch { }
                Console.WriteLine($"OptalCP C# API {Solver.Version}");
                try
                {
                    Console.WriteLine($"Solver path: '{Solver.FindSolver()}'");
                }
                catch { }
            }

            if (exitOnError)
                Environment.Exit(0);
            else
                throw new ArgumentException(showHelp ? "Help requested" : "Version requested");
        }

        private static List<string> ParseArgs(Parameters parameters, string[] args, bool allowUnknown)
        {
            var unrecognized = new List<string>();
            int i = 0;
            while (i < args.Length)
            {
                var opt = args[i];
                if (!opt.StartsWith("--") && opt != "-h")
                {
                    if (allowUnknown)
                    {
                        unrecognized.Add(opt);
                        i++;
                        continue;
                    }
                    throw new ArgumentException($"Unrecognized command line option: {opt}");
                }

                // Skip help/version flags (already handled)
                if (opt == "--help" || opt == "-h" || opt.Equals("--optalcpVersion", StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    continue;
                }

                // Check for --param=value syntax
                var eqPos = opt.IndexOf('=');
                if (eqPos != -1)
                {
                    if (!ApplyParameter(parameters, opt.Substring(0, eqPos), opt.Substring(eqPos + 1)))
                    {
                        if (allowUnknown)
                            unrecognized.Add(args[i]);
                        else
                            throw new ArgumentException($"Unrecognized command line option: {opt}");
                    }
                    i++;
                    continue;
                }

                // Check for --param value syntax
                if (i + 1 >= args.Length)
                {
                    if (allowUnknown)
                    {
                        unrecognized.Add(args[i]);
                        i++;
                        continue;
                    }
                    throw new ArgumentException($"Missing value for command line option: {opt}");
                }

                if (ApplyParameter(parameters, opt, args[i + 1]))
                {
                    i += 2;
                }
                else
                {
                    if (allowUnknown)
                        unrecognized.Add(args[i]);
                    else
                        throw new ArgumentException($"Unrecognized command line option: {opt}");
                    i++;
                }
            }
            return unrecognized;
        }

        private readonly struct WorkerRange
        {
            public int Min { get; }
            public int Max { get; }
            public WorkerRange(int min, int max) { Min = min; Max = max; }
        }

        private static bool ApplyParameter(Parameters parameters, string name, string value)
        {
            var opt = name.StartsWith("--") ? name.Substring(2).ToLowerInvariant() : name.ToLowerInvariant();
            WorkerRange? workerRange = null;

            // Check for worker syntax: --worker3.searchType or --worker0-3.searchType
            var match = Regex.Match(opt, @"^workers?(\d+)(?:-(\d+))?\.(.+)$");
            if (match.Success)
            {
                var minWorker = int.Parse(match.Groups[1].Value);
                var maxWorker = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : minWorker;
                if (minWorker > maxWorker)
                    throw new ArgumentException($"Empty range worker specification: {name}");
                workerRange = new WorkerRange(minWorker, maxWorker);
                opt = match.Groups[3].Value;
            }

            switch (opt)
            {
                case "solver":
                    if (workerRange != null)
                        throw new ArgumentException($"Parameter 'solver' cannot be set per-worker in '{name}'.");
                    parameters.solver = value;
                    return true;

                case "color":
                {
                    var parsed = ParseEnum(value, "Color", new[] { "Never", "Auto", "Always" });
                    if (workerRange == null)
                        parameters.color = parsed;
                    else
                        throw new ArgumentException("Parameter 'color' is global and cannot be set per-worker. Use '--color' instead.");
                    return true;
                }
                case "nbworkers":
                {
                    var parsed = ParseLong(value, "NbWorkers");
                    if (workerRange == null)
                        parameters.nbWorkers = parsed;
                    else
                        throw new ArgumentException("Parameter 'nbWorkers' is global and cannot be set per-worker. Use '--nbWorkers' instead.");
                    return true;
                }
                case "nbhelpers":
                {
                    var parsed = ParseLong(value, "NbHelpers");
                    if (workerRange == null)
                        parameters._nbHelpers = parsed;
                    else
                        throw new ArgumentException("Parameter '_nbHelpers' is global and cannot be set per-worker. Use '--_nbHelpers' instead.");
                    return true;
                }
                case "preset":
                {
                    var parsed = ParseEnum(value, "Preset", new[] { "Auto", "Default", "Large" });
                    if (workerRange == null)
                        parameters.preset = parsed;
                    else
                        throw new ArgumentException("Parameter 'preset' is global and cannot be set per-worker. Use '--preset' instead.");
                    return true;
                }
                case "searchtype":
                {
                    var parsed = ParseEnum(value, "SearchType", new[] { "Auto", "LNS", "FDS", "FDSDual", "SetTimes", "FDSLB" });
                    if (workerRange == null)
                        parameters.searchType = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).searchType = parsed;
                    return true;
                }
                case "randomseed":
                {
                    var parsed = ParseLong(value, "RandomSeed");
                    if (workerRange == null)
                        parameters.randomSeed = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).randomSeed = parsed;
                    return true;
                }
                case "loglevel":
                {
                    var parsed = ParseLong(value, "LogLevel");
                    if (parsed < 0 || parsed > 3)
                        throw new ArgumentException($"Value {parsed} for parameter LogLevel is out of range [0, 3]");
                    if (workerRange == null)
                        parameters.logLevel = parsed;
                    else
                        throw new ArgumentException("Parameter 'logLevel' is global and cannot be set per-worker. Use '--logLevel' instead.");
                    return true;
                }
                case "warninglevel":
                {
                    var parsed = ParseLong(value, "WarningLevel");
                    if (parsed < 0 || parsed > 3)
                        throw new ArgumentException($"Value {parsed} for parameter WarningLevel is out of range [0, 3]");
                    if (workerRange == null)
                        parameters.warningLevel = parsed;
                    else
                        throw new ArgumentException("Parameter 'warningLevel' is global and cannot be set per-worker. Use '--warningLevel' instead.");
                    return true;
                }
                case "logperiod":
                {
                    var parsed = ParseDouble(value, "LogPeriod");
                    if (workerRange == null)
                        parameters.logPeriod = parsed;
                    else
                        throw new ArgumentException("Parameter 'logPeriod' is global and cannot be set per-worker. Use '--logPeriod' instead.");
                    return true;
                }
                case "verifysolutions":
                {
                    var parsed = ParseBool(value, "VerifySolutions");
                    if (workerRange == null)
                        parameters.verifySolutions = parsed;
                    else
                        throw new ArgumentException("Parameter 'verifySolutions' is global and cannot be set per-worker. Use '--verifySolutions' instead.");
                    return true;
                }
                case "verifyexternalsolutions":
                {
                    var parsed = ParseBool(value, "VerifyExternalSolutions");
                    if (workerRange == null)
                        parameters.verifyExternalSolutions = parsed;
                    else
                        throw new ArgumentException("Parameter 'verifyExternalSolutions' is global and cannot be set per-worker. Use '--verifyExternalSolutions' instead.");
                    return true;
                }
                case "allocationblocksize":
                {
                    var parsed = ParseLong(value, "AllocationBlockSize");
                    if (parsed < 4 || parsed > 1073741824)
                        throw new ArgumentException($"Value {parsed} for parameter AllocationBlockSize is out of range [4, 1073741824]");
                    if (workerRange == null)
                        parameters.allocationBlockSize = parsed;
                    else
                        throw new ArgumentException("Parameter 'allocationBlockSize' is global and cannot be set per-worker. Use '--allocationBlockSize' instead.");
                    return true;
                }
                case "processexittimeout":
                {
                    var parsed = ParseDouble(value, "ProcessExitTimeout");
                    if (workerRange == null)
                        parameters.processExitTimeout = parsed;
                    else
                        throw new ArgumentException("Parameter 'processExitTimeout' is global and cannot be set per-worker. Use '--processExitTimeout' instead.");
                    return true;
                }
                case "timelimit":
                {
                    var parsed = ParseDouble(value, "TimeLimit");
                    if (workerRange == null)
                        parameters.timeLimit = parsed;
                    else
                        throw new ArgumentException("Parameter 'timeLimit' is global and cannot be set per-worker. Use '--timeLimit' instead.");
                    return true;
                }
                case "solutionlimit":
                {
                    var parsed = ParseLong(value, "SolutionLimit");
                    if (workerRange == null)
                        parameters.solutionLimit = parsed;
                    else
                        throw new ArgumentException("Parameter 'solutionLimit' is global and cannot be set per-worker. Use '--solutionLimit' instead.");
                    return true;
                }
                case "workerfaillimit":
                {
                    var parsed = ParseLong(value, "WorkerFailLimit");
                    if (workerRange == null)
                        parameters._workerFailLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._workerFailLimit = parsed;
                    return true;
                }
                case "workerbranchlimit":
                {
                    var parsed = ParseLong(value, "WorkerBranchLimit");
                    if (workerRange == null)
                        parameters._workerBranchLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._workerBranchLimit = parsed;
                    return true;
                }
                case "workersolutionlimit":
                {
                    var parsed = ParseLong(value, "WorkerSolutionLimit");
                    if (workerRange == null)
                        parameters._workerSolutionLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._workerSolutionLimit = parsed;
                    return true;
                }
                case "workerlnssteplimit":
                {
                    var parsed = ParseLong(value, "WorkerLNSStepLimit");
                    if (workerRange == null)
                        parameters._workerLNSStepLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._workerLNSStepLimit = parsed;
                    return true;
                }
                case "workerrestartlimit":
                {
                    var parsed = ParseLong(value, "WorkerRestartLimit");
                    if (workerRange == null)
                        parameters._workerRestartLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._workerRestartLimit = parsed;
                    return true;
                }
                case "absolutegaptolerance":
                {
                    var parsed = ParseDouble(value, "AbsoluteGapTolerance");
                    if (workerRange == null)
                        parameters.absoluteGapTolerance = parsed;
                    else
                        throw new ArgumentException("Parameter 'absoluteGapTolerance' is global and cannot be set per-worker. Use '--absoluteGapTolerance' instead.");
                    return true;
                }
                case "relativegaptolerance":
                {
                    var parsed = ParseDouble(value, "RelativeGapTolerance");
                    if (workerRange == null)
                        parameters.relativeGapTolerance = parsed;
                    else
                        throw new ArgumentException("Parameter 'relativeGapTolerance' is global and cannot be set per-worker. Use '--relativeGapTolerance' instead.");
                    return true;
                }
                case "nooverlappropagationlevel":
                {
                    var parsed = ParseLong(value, "NoOverlapPropagationLevel");
                    if (parsed < 0 || parsed > 4)
                        throw new ArgumentException($"Value {parsed} for parameter NoOverlapPropagationLevel is out of range [0, 4]");
                    if (workerRange == null)
                        parameters.noOverlapPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).noOverlapPropagationLevel = parsed;
                    return true;
                }
                case "cumulpropagationlevel":
                {
                    var parsed = ParseLong(value, "CumulPropagationLevel");
                    if (parsed < 0 || parsed > 4)
                        throw new ArgumentException($"Value {parsed} for parameter CumulPropagationLevel is out of range [0, 4]");
                    if (workerRange == null)
                        parameters.cumulPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).cumulPropagationLevel = parsed;
                    return true;
                }
                case "reservoirpropagationlevel":
                {
                    var parsed = ParseLong(value, "ReservoirPropagationLevel");
                    if (parsed < 1 || parsed > 2)
                        throw new ArgumentException($"Value {parsed} for parameter ReservoirPropagationLevel is out of range [1, 2]");
                    if (workerRange == null)
                        parameters.reservoirPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).reservoirPropagationLevel = parsed;
                    return true;
                }
                case "positionpropagationlevel":
                {
                    var parsed = ParseLong(value, "PositionPropagationLevel");
                    if (parsed < 1 || parsed > 3)
                        throw new ArgumentException($"Value {parsed} for parameter PositionPropagationLevel is out of range [1, 3]");
                    if (workerRange == null)
                        parameters.positionPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).positionPropagationLevel = parsed;
                    return true;
                }
                case "integralpropagationlevel":
                {
                    var parsed = ParseLong(value, "IntegralPropagationLevel");
                    if (parsed < 1 || parsed > 2)
                        throw new ArgumentException($"Value {parsed} for parameter IntegralPropagationLevel is out of range [1, 2]");
                    if (workerRange == null)
                        parameters.integralPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).integralPropagationLevel = parsed;
                    return true;
                }
                case "useprecedenceenergy":
                {
                    var parsed = ParseLong(value, "UsePrecedenceEnergy");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter UsePrecedenceEnergy is out of range [0, 1]");
                    if (workerRange == null)
                        parameters.usePrecedenceEnergy = parsed;
                    else
                        throw new ArgumentException("Parameter 'usePrecedenceEnergy' is global and cannot be set per-worker. Use '--usePrecedenceEnergy' instead.");
                    return true;
                }
                case "packpropagationlevel":
                {
                    var parsed = ParseLong(value, "PackPropagationLevel");
                    if (parsed < 1 || parsed > 2)
                        throw new ArgumentException($"Value {parsed} for parameter PackPropagationLevel is out of range [1, 2]");
                    if (workerRange == null)
                        parameters._packPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._packPropagationLevel = parsed;
                    return true;
                }
                case "itvmappingpropagationlevel":
                {
                    var parsed = ParseLong(value, "ItvMappingPropagationLevel");
                    if (parsed < 1 || parsed > 2)
                        throw new ArgumentException($"Value {parsed} for parameter ItvMappingPropagationLevel is out of range [1, 2]");
                    if (workerRange == null)
                        parameters._itvMappingPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._itvMappingPropagationLevel = parsed;
                    return true;
                }
                case "searchtracelevel":
                {
                    var parsed = ParseLong(value, "SearchTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter SearchTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters.searchTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).searchTraceLevel = parsed;
                    return true;
                }
                case "propagationtracelevel":
                {
                    var parsed = ParseLong(value, "PropagationTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter PropagationTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters.propagationTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).propagationTraceLevel = parsed;
                    return true;
                }
                case "infotracelevel":
                {
                    var parsed = ParseLong(value, "InfoTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter InfoTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters.infoTraceLevel = parsed;
                    else
                        throw new ArgumentException("Parameter 'infoTraceLevel' is global and cannot be set per-worker. Use '--infoTraceLevel' instead.");
                    return true;
                }
                case "fdsinitialrating":
                {
                    var parsed = ParseDouble(value, "FDSInitialRating");
                    if (parsed < 0.000000 || parsed > 2.000000)
                        throw new ArgumentException($"Value {parsed} for parameter FDSInitialRating is out of range [0.000000, 2.000000]");
                    if (workerRange == null)
                        parameters.fdsInitialRating = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsInitialRating = parsed;
                    return true;
                }
                case "fdsreductionweight":
                {
                    var parsed = ParseDouble(value, "FDSReductionWeight");
                    if (workerRange == null)
                        parameters.fdsReductionWeight = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsReductionWeight = parsed;
                    return true;
                }
                case "fdsratingaveragelength":
                {
                    var parsed = ParseLong(value, "FDSRatingAverageLength");
                    if (parsed < 0 || parsed > 254)
                        throw new ArgumentException($"Value {parsed} for parameter FDSRatingAverageLength is out of range [0, 254]");
                    if (workerRange == null)
                        parameters.fdsRatingAverageLength = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsRatingAverageLength = parsed;
                    return true;
                }
                case "fdsfixedalpha":
                {
                    var parsed = ParseDouble(value, "FDSFixedAlpha");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter FDSFixedAlpha is out of range [0, 1]");
                    if (workerRange == null)
                        parameters.fdsFixedAlpha = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsFixedAlpha = parsed;
                    return true;
                }
                case "fdsratingaveragecomparison":
                {
                    var parsed = ParseEnum(value, "FDSRatingAverageComparison", new[] { "Off", "Global", "Depth" });
                    if (workerRange == null)
                        parameters.fdsRatingAverageComparison = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsRatingAverageComparison = parsed;
                    return true;
                }
                case "fdsreductionfactor":
                {
                    var parsed = ParseEnum(value, "FDSReductionFactor", new[] { "Normal", "Zero", "Random" });
                    if (workerRange == null)
                        parameters.fdsReductionFactor = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsReductionFactor = parsed;
                    return true;
                }
                case "fdsreuseclosing":
                {
                    var parsed = ParseBool(value, "FDSReuseClosing");
                    if (workerRange == null)
                        parameters.fdsReuseClosing = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsReuseClosing = parsed;
                    return true;
                }
                case "fdsuniformchoicestep":
                {
                    var parsed = ParseBool(value, "FDSUniformChoiceStep");
                    if (workerRange == null)
                        parameters.fdsUniformChoiceStep = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsUniformChoiceStep = parsed;
                    return true;
                }
                case "fdslengthstepratio":
                {
                    var parsed = ParseDouble(value, "FDSLengthStepRatio");
                    if (workerRange == null)
                        parameters.fdsLengthStepRatio = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsLengthStepRatio = parsed;
                    return true;
                }
                case "fdsmaxinitialchoicespervariable":
                {
                    var parsed = ParseLong(value, "FDSMaxInitialChoicesPerVariable");
                    if (parsed < 2 || parsed > 2147483647)
                        throw new ArgumentException($"Value {parsed} for parameter FDSMaxInitialChoicesPerVariable is out of range [2, 2147483647]");
                    if (workerRange == null)
                        parameters.fdsMaxInitialChoicesPerVariable = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsMaxInitialChoicesPerVariable = parsed;
                    return true;
                }
                case "fdsadditionalstepratio":
                {
                    var parsed = ParseDouble(value, "FDSAdditionalStepRatio");
                    if (workerRange == null)
                        parameters.fdsAdditionalStepRatio = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsAdditionalStepRatio = parsed;
                    return true;
                }
                case "fdspresencestatuschoices":
                {
                    var parsed = ParseBool(value, "FDSPresenceStatusChoices");
                    if (workerRange == null)
                        parameters.fdsPresenceStatusChoices = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsPresenceStatusChoices = parsed;
                    return true;
                }
                case "fdsmaxinitiallengthchoices":
                {
                    var parsed = ParseLong(value, "FDSMaxInitialLengthChoices");
                    if (parsed < 0 || parsed > 2147483647)
                        throw new ArgumentException($"Value {parsed} for parameter FDSMaxInitialLengthChoices is out of range [0, 2147483647]");
                    if (workerRange == null)
                        parameters.fdsMaxInitialLengthChoices = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsMaxInitialLengthChoices = parsed;
                    return true;
                }
                case "fdsminlengthchoicestep":
                {
                    var parsed = ParseLong(value, "FDSMinLengthChoiceStep");
                    if (parsed < 1 || parsed > 1073741823)
                        throw new ArgumentException($"Value {parsed} for parameter FDSMinLengthChoiceStep is out of range [1, 1073741823]");
                    if (workerRange == null)
                        parameters.fdsMinLengthChoiceStep = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsMinLengthChoiceStep = parsed;
                    return true;
                }
                case "fdsminintvarchoicestep":
                {
                    var parsed = ParseLong(value, "FDSMinIntVarChoiceStep");
                    if (parsed < 1 || parsed > 1073741823)
                        throw new ArgumentException($"Value {parsed} for parameter FDSMinIntVarChoiceStep is out of range [1, 1073741823]");
                    if (workerRange == null)
                        parameters.fdsMinIntVarChoiceStep = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsMinIntVarChoiceStep = parsed;
                    return true;
                }
                case "fdseventtimeinfluence":
                {
                    var parsed = ParseDouble(value, "FDSEventTimeInfluence");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter FDSEventTimeInfluence is out of range [0, 1]");
                    if (workerRange == null)
                        parameters.fdsEventTimeInfluence = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsEventTimeInfluence = parsed;
                    return true;
                }
                case "fdsbothfailrewardfactor":
                {
                    var parsed = ParseDouble(value, "FDSBothFailRewardFactor");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter FDSBothFailRewardFactor is out of range [0, 1]");
                    if (workerRange == null)
                        parameters.fdsBothFailRewardFactor = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsBothFailRewardFactor = parsed;
                    return true;
                }
                case "fdsepsilon":
                {
                    var parsed = ParseDouble(value, "FDSEpsilon");
                    if (parsed < 0.000000 || parsed > 0.999990)
                        throw new ArgumentException($"Value {parsed} for parameter FDSEpsilon is out of range [0.000000, 0.999990]");
                    if (workerRange == null)
                        parameters.fdsEpsilon = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsEpsilon = parsed;
                    return true;
                }
                case "fdsstrongbranchingsize":
                {
                    var parsed = ParseLong(value, "FDSStrongBranchingSize");
                    if (workerRange == null)
                        parameters.fdsStrongBranchingSize = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsStrongBranchingSize = parsed;
                    return true;
                }
                case "fdsstrongbranchingdepth":
                {
                    var parsed = ParseLong(value, "FDSStrongBranchingDepth");
                    if (workerRange == null)
                        parameters.fdsStrongBranchingDepth = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsStrongBranchingDepth = parsed;
                    return true;
                }
                case "fdsstrongbranchingcriterion":
                {
                    var parsed = ParseEnum(value, "FDSStrongBranchingCriterion", new[] { "Both", "Left", "Right" });
                    if (workerRange == null)
                        parameters.fdsStrongBranchingCriterion = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsStrongBranchingCriterion = parsed;
                    return true;
                }
                case "fdsinitialrestartlimit":
                {
                    var parsed = ParseLong(value, "FDSInitialRestartLimit");
                    if (workerRange == null)
                        parameters.fdsInitialRestartLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsInitialRestartLimit = parsed;
                    return true;
                }
                case "fdsrestartstrategy":
                {
                    var parsed = ParseEnum(value, "FDSRestartStrategy", new[] { "Geometric", "Nested", "Luby" });
                    if (workerRange == null)
                        parameters.fdsRestartStrategy = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsRestartStrategy = parsed;
                    return true;
                }
                case "fdsrestartgrowthfactor":
                {
                    var parsed = ParseDouble(value, "FDSRestartGrowthFactor");
                    if (workerRange == null)
                        parameters.fdsRestartGrowthFactor = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsRestartGrowthFactor = parsed;
                    return true;
                }
                case "fdsmaxcounterafterrestart":
                {
                    var parsed = ParseLong(value, "FDSMaxCounterAfterRestart");
                    if (workerRange == null)
                        parameters.fdsMaxCounterAfterRestart = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsMaxCounterAfterRestart = parsed;
                    return true;
                }
                case "fdsmaxcounteraftersolution":
                {
                    var parsed = ParseLong(value, "FDSMaxCounterAfterSolution");
                    if (workerRange == null)
                        parameters.fdsMaxCounterAfterSolution = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsMaxCounterAfterSolution = parsed;
                    return true;
                }
                case "fdsresetrestartsaftersolution":
                {
                    var parsed = ParseBool(value, "FDSResetRestartsAfterSolution");
                    if (workerRange == null)
                        parameters.fdsResetRestartsAfterSolution = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsResetRestartsAfterSolution = parsed;
                    return true;
                }
                case "fdsusenogoods":
                {
                    var parsed = ParseBool(value, "FDSUseNogoods");
                    if (workerRange == null)
                        parameters.fdsUseNogoods = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsUseNogoods = parsed;
                    return true;
                }
                case "fdsfreezeratingsafterproof":
                {
                    var parsed = ParseBool(value, "FDSFreezeRatingsAfterProof");
                    if (workerRange == null)
                        parameters._fdsFreezeRatingsAfterProof = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsFreezeRatingsAfterProof = parsed;
                    return true;
                }
                case "fdscontinueafterproof":
                {
                    var parsed = ParseBool(value, "FDSContinueAfterProof");
                    if (workerRange == null)
                        parameters._fdsContinueAfterProof = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsContinueAfterProof = parsed;
                    return true;
                }
                case "fdsrepeatlimit":
                {
                    var parsed = ParseLong(value, "FDSRepeatLimit");
                    if (workerRange == null)
                        parameters._fdsRepeatLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsRepeatLimit = parsed;
                    return true;
                }
                case "fdscompletelyrandom":
                {
                    var parsed = ParseBool(value, "FDSCompletelyRandom");
                    if (workerRange == null)
                        parameters._fdsCompletelyRandom = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsCompletelyRandom = parsed;
                    return true;
                }
                case "fdsbranchonobjective":
                {
                    var parsed = ParseBool(value, "FDSBranchOnObjective");
                    if (workerRange == null)
                        parameters.fdsBranchOnObjective = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsBranchOnObjective = parsed;
                    return true;
                }
                case "fdsimprovenogoods":
                {
                    var parsed = ParseBool(value, "FDSImproveNogoods");
                    if (workerRange == null)
                        parameters._fdsImproveNogoods = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsImproveNogoods = parsed;
                    return true;
                }
                case "fdsbranchordering":
                {
                    var parsed = ParseEnum(value, "FDSBranchOrdering", new[] { "FailureFirst", "FailureLast", "Random" });
                    if (workerRange == null)
                        parameters.fdsBranchOrdering = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsBranchOrdering = parsed;
                    return true;
                }
                case "fdsdivebysettimes":
                {
                    var parsed = ParseBool(value, "FDSDiveBySetTimes");
                    if (workerRange == null)
                        parameters._fdsDiveBySetTimes = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsDiveBySetTimes = parsed;
                    return true;
                }
                case "fdsdualstrategy":
                {
                    var parsed = ParseEnum(value, "FDSDualStrategy", new[] { "Minimum", "Random", "Split" });
                    if (workerRange == null)
                        parameters.fdsDualStrategy = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsDualStrategy = parsed;
                    return true;
                }
                case "fdsdualresetratings":
                {
                    var parsed = ParseBool(value, "FDSDualResetRatings");
                    if (workerRange == null)
                        parameters.fdsDualResetRatings = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).fdsDualResetRatings = parsed;
                    return true;
                }
                case "lnsmode":
                {
                    var parsed = ParseEnum(value, "LNSMode", new[] { "Robust", "Focused" });
                    if (workerRange == null)
                        parameters.lnsMode = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).lnsMode = parsed;
                    return true;
                }
                case "lnsinitnooverlappropagationlevel":
                {
                    var parsed = ParseLong(value, "LNSInitNoOverlapPropagationLevel");
                    if (parsed < 0 || parsed > 4)
                        throw new ArgumentException($"Value {parsed} for parameter LNSInitNoOverlapPropagationLevel is out of range [0, 4]");
                    if (workerRange == null)
                        parameters._lnsInitNoOverlapPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsInitNoOverlapPropagationLevel = parsed;
                    return true;
                }
                case "lnsinitcumulpropagationlevel":
                {
                    var parsed = ParseLong(value, "LNSInitCumulPropagationLevel");
                    if (parsed < 0 || parsed > 4)
                        throw new ArgumentException($"Value {parsed} for parameter LNSInitCumulPropagationLevel is out of range [0, 4]");
                    if (workerRange == null)
                        parameters._lnsInitCumulPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsInitCumulPropagationLevel = parsed;
                    return true;
                }
                case "lnsfirstfaillimit":
                {
                    var parsed = ParseLong(value, "LNSFirstFailLimit");
                    if (workerRange == null)
                        parameters._lnsFirstFailLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsFirstFailLimit = parsed;
                    return true;
                }
                case "lnsfaillimitgrowthfactor":
                {
                    var parsed = ParseDouble(value, "LNSFailLimitGrowthFactor");
                    if (workerRange == null)
                        parameters._lnsFailLimitGrowthFactor = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsFailLimitGrowthFactor = parsed;
                    return true;
                }
                case "lnsfaillimitcoefficient":
                {
                    var parsed = ParseDouble(value, "LNSFailLimitCoefficient");
                    if (workerRange == null)
                        parameters._lnsFailLimitCoefficient = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsFailLimitCoefficient = parsed;
                    return true;
                }
                case "lnsiterationsafterfirstsolution":
                {
                    var parsed = ParseLong(value, "LNSIterationsAfterFirstSolution");
                    if (parsed < -1 || parsed > 2147483647)
                        throw new ArgumentException($"Value {parsed} for parameter LNSIterationsAfterFirstSolution is out of range [-1, 2147483647]");
                    if (workerRange == null)
                        parameters._lnsIterationsAfterFirstSolution = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsIterationsAfterFirstSolution = parsed;
                    return true;
                }
                case "lnsaggressivedominance":
                {
                    var parsed = ParseBool(value, "LNSAggressiveDominance");
                    if (workerRange == null)
                        parameters._lnsAggressiveDominance = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsAggressiveDominance = parsed;
                    return true;
                }
                case "lnssamesolutionperiod":
                {
                    var parsed = ParseLong(value, "LNSSameSolutionPeriod");
                    if (parsed < 1 || parsed > 2147483647)
                        throw new ArgumentException($"Value {parsed} for parameter LNSSameSolutionPeriod is out of range [1, 2147483647]");
                    if (workerRange == null)
                        parameters._lnsSameSolutionPeriod = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsSameSolutionPeriod = parsed;
                    return true;
                }
                case "lnstier1size":
                {
                    var parsed = ParseLong(value, "LNSTier1Size");
                    if (parsed < 1 || parsed > 1000)
                        throw new ArgumentException($"Value {parsed} for parameter LNSTier1Size is out of range [1, 1000]");
                    if (workerRange == null)
                        parameters._lnsTier1Size = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsTier1Size = parsed;
                    return true;
                }
                case "lnstier2size":
                {
                    var parsed = ParseLong(value, "LNSTier2Size");
                    if (parsed < 0 || parsed > 1000)
                        throw new ArgumentException($"Value {parsed} for parameter LNSTier2Size is out of range [0, 1000]");
                    if (workerRange == null)
                        parameters._lnsTier2Size = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsTier2Size = parsed;
                    return true;
                }
                case "lnstier3size":
                {
                    var parsed = ParseLong(value, "LNSTier3Size");
                    if (parsed < 0 || parsed > 1000)
                        throw new ArgumentException($"Value {parsed} for parameter LNSTier3Size is out of range [0, 1000]");
                    if (workerRange == null)
                        parameters._lnsTier3Size = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsTier3Size = parsed;
                    return true;
                }
                case "lnstier2effort":
                {
                    var parsed = ParseDouble(value, "LNSTier2Effort");
                    if (parsed < 0.000000 || parsed > 1.000000)
                        throw new ArgumentException($"Value {parsed} for parameter LNSTier2Effort is out of range [0.000000, 1.000000]");
                    if (workerRange == null)
                        parameters._lnsTier2Effort = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsTier2Effort = parsed;
                    return true;
                }
                case "lnstier3effort":
                {
                    var parsed = ParseDouble(value, "LNSTier3Effort");
                    if (parsed < 0.000000 || parsed > 1.000000)
                        throw new ArgumentException($"Value {parsed} for parameter LNSTier3Effort is out of range [0.000000, 1.000000]");
                    if (workerRange == null)
                        parameters._lnsTier3Effort = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsTier3Effort = parsed;
                    return true;
                }
                case "lnsstepfaillimitfactor":
                {
                    var parsed = ParseDouble(value, "LNSStepFailLimitFactor");
                    if (workerRange == null)
                        parameters._lnsStepFailLimitFactor = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsStepFailLimitFactor = parsed;
                    return true;
                }
                case "lnsapplycutprobability":
                {
                    var parsed = ParseDouble(value, "LNSApplyCutProbability");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSApplyCutProbability is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsApplyCutProbability = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsApplyCutProbability = parsed;
                    return true;
                }
                case "lnsskipnoimprovementprobability":
                {
                    var parsed = ParseDouble(value, "LNSSkipNoImprovementProbability");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSSkipNoImprovementProbability is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsSkipNoImprovementProbability = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsSkipNoImprovementProbability = parsed;
                    return true;
                }
                case "lnssmallstructurelimit":
                {
                    var parsed = ParseLong(value, "LNSSmallStructureLimit");
                    if (parsed < 0 || parsed > 10)
                        throw new ArgumentException($"Value {parsed} for parameter LNSSmallStructureLimit is out of range [0, 10]");
                    if (workerRange == null)
                        parameters._lnsSmallStructureLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsSmallStructureLimit = parsed;
                    return true;
                }
                case "lnsresourceoptimization":
                {
                    var parsed = ParseBool(value, "LNSResourceOptimization");
                    if (workerRange == null)
                        parameters._lnsResourceOptimization = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsResourceOptimization = parsed;
                    return true;
                }
                case "lnsrestoreabsentintervals":
                {
                    var parsed = ParseBool(value, "LNSRestoreAbsentIntervals");
                    if (workerRange == null)
                        parameters._lnsRestoreAbsentIntervals = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsRestoreAbsentIntervals = parsed;
                    return true;
                }
                case "lnsrestoreintervallengths":
                {
                    var parsed = ParseBool(value, "LNSRestoreIntervalLengths");
                    if (workerRange == null)
                        parameters._lnsRestoreIntervalLengths = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsRestoreIntervalLengths = parsed;
                    return true;
                }
                case "lnsrestoreintvarvalues":
                {
                    var parsed = ParseBool(value, "LNSRestoreIntVarValues");
                    if (workerRange == null)
                        parameters._lnsRestoreIntVarValues = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsRestoreIntVarValues = parsed;
                    return true;
                }
                case "lnsalwaysrelaxobjective":
                {
                    var parsed = ParseBool(value, "LNSAlwaysRelaxObjective");
                    if (workerRange == null)
                        parameters._lnsAlwaysRelaxObjective = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsAlwaysRelaxObjective = parsed;
                    return true;
                }
                case "lnsusewarmstartonly":
                {
                    var parsed = ParseBool(value, "LNSUseWarmStartOnly");
                    if (workerRange == null)
                        parameters.lnsUseWarmStartOnly = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).lnsUseWarmStartOnly = parsed;
                    return true;
                }
                case "lnsheuristicsepsilon":
                {
                    var parsed = ParseDouble(value, "LNSHeuristicsEpsilon");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSHeuristicsEpsilon is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsHeuristicsEpsilon = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsHeuristicsEpsilon = parsed;
                    return true;
                }
                case "lnsheuristicsalpha":
                {
                    var parsed = ParseDouble(value, "LNSHeuristicsAlpha");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSHeuristicsAlpha is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsHeuristicsAlpha = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsHeuristicsAlpha = parsed;
                    return true;
                }
                case "lnsheuristicstemperature":
                {
                    var parsed = ParseDouble(value, "LNSHeuristicsTemperature");
                    if (parsed < -1 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSHeuristicsTemperature is out of range [-1, 1]");
                    if (workerRange == null)
                        parameters._lnsHeuristicsTemperature = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsHeuristicsTemperature = parsed;
                    return true;
                }
                case "lnsheuristicsuniform":
                {
                    var parsed = ParseBool(value, "LNSHeuristicsUniform");
                    if (workerRange == null)
                        parameters._lnsHeuristicsUniform = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsHeuristicsUniform = parsed;
                    return true;
                }
                case "lnsheuristicsinitialq":
                {
                    var parsed = ParseDouble(value, "LNSHeuristicsInitialQ");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSHeuristicsInitialQ is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsHeuristicsInitialQ = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsHeuristicsInitialQ = parsed;
                    return true;
                }
                case "lnsportionepsilon":
                {
                    var parsed = ParseDouble(value, "LNSPortionEpsilon");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionEpsilon is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsPortionEpsilon = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionEpsilon = parsed;
                    return true;
                }
                case "lnsportionalpha":
                {
                    var parsed = ParseDouble(value, "LNSPortionAlpha");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionAlpha is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsPortionAlpha = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionAlpha = parsed;
                    return true;
                }
                case "lnsportiontemperature":
                {
                    var parsed = ParseDouble(value, "LNSPortionTemperature");
                    if (parsed < -1 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionTemperature is out of range [-1, 1]");
                    if (workerRange == null)
                        parameters._lnsPortionTemperature = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionTemperature = parsed;
                    return true;
                }
                case "lnsportionuniform":
                {
                    var parsed = ParseBool(value, "LNSPortionUniform");
                    if (workerRange == null)
                        parameters._lnsPortionUniform = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionUniform = parsed;
                    return true;
                }
                case "lnsportioninitialq":
                {
                    var parsed = ParseDouble(value, "LNSPortionInitialQ");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionInitialQ is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsPortionInitialQ = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionInitialQ = parsed;
                    return true;
                }
                case "lnsportionhandicaplimit":
                {
                    var parsed = ParseDouble(value, "LNSPortionHandicapLimit");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionHandicapLimit is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsPortionHandicapLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionHandicapLimit = parsed;
                    return true;
                }
                case "lnsportionhandicapvalue":
                {
                    var parsed = ParseDouble(value, "LNSPortionHandicapValue");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionHandicapValue is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsPortionHandicapValue = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionHandicapValue = parsed;
                    return true;
                }
                case "lnsportionhandicapinitialq":
                {
                    var parsed = ParseDouble(value, "LNSPortionHandicapInitialQ");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionHandicapInitialQ is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsPortionHandicapInitialQ = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionHandicapInitialQ = parsed;
                    return true;
                }
                case "lnsneighborhoodstrategy":
                {
                    var parsed = ParseLong(value, "LNSNeighborhoodStrategy");
                    if (parsed < 0 || parsed > 2)
                        throw new ArgumentException($"Value {parsed} for parameter LNSNeighborhoodStrategy is out of range [0, 2]");
                    if (workerRange == null)
                        parameters._lnsNeighborhoodStrategy = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsNeighborhoodStrategy = parsed;
                    return true;
                }
                case "lnsneighborhoodepsilon":
                {
                    var parsed = ParseDouble(value, "LNSNeighborhoodEpsilon");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSNeighborhoodEpsilon is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsNeighborhoodEpsilon = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsNeighborhoodEpsilon = parsed;
                    return true;
                }
                case "lnsneighborhoodalpha":
                {
                    var parsed = ParseDouble(value, "LNSNeighborhoodAlpha");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSNeighborhoodAlpha is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsNeighborhoodAlpha = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsNeighborhoodAlpha = parsed;
                    return true;
                }
                case "lnsneighborhoodtemperature":
                {
                    var parsed = ParseDouble(value, "LNSNeighborhoodTemperature");
                    if (parsed < -1 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSNeighborhoodTemperature is out of range [-1, 1]");
                    if (workerRange == null)
                        parameters._lnsNeighborhoodTemperature = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsNeighborhoodTemperature = parsed;
                    return true;
                }
                case "lnsneighborhooduniform":
                {
                    var parsed = ParseBool(value, "LNSNeighborhoodUniform");
                    if (workerRange == null)
                        parameters._lnsNeighborhoodUniform = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsNeighborhoodUniform = parsed;
                    return true;
                }
                case "lnsneighborhoodinitialq":
                {
                    var parsed = ParseDouble(value, "LNSNeighborhoodInitialQ");
                    if (parsed < 0 || parsed > 1)
                        throw new ArgumentException($"Value {parsed} for parameter LNSNeighborhoodInitialQ is out of range [0, 1]");
                    if (workerRange == null)
                        parameters._lnsNeighborhoodInitialQ = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsNeighborhoodInitialQ = parsed;
                    return true;
                }
                case "lnsdivinglimit":
                {
                    var parsed = ParseLong(value, "LNSDivingLimit");
                    if (workerRange == null)
                        parameters._lnsDivingLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsDivingLimit = parsed;
                    return true;
                }
                case "lnsdivingfaillimitratio":
                {
                    var parsed = ParseDouble(value, "LNSDivingFailLimitRatio");
                    if (workerRange == null)
                        parameters._lnsDivingFailLimitRatio = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsDivingFailLimitRatio = parsed;
                    return true;
                }
                case "lnslearningrun":
                {
                    var parsed = ParseBool(value, "LNSLearningRun");
                    if (workerRange == null)
                        parameters._lnsLearningRun = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsLearningRun = parsed;
                    return true;
                }
                case "lnsstayonobjective":
                {
                    var parsed = ParseBool(value, "LNSStayOnObjective");
                    if (workerRange == null)
                        parameters._lnsStayOnObjective = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsStayOnObjective = parsed;
                    return true;
                }
                case "lnsfds":
                {
                    var parsed = ParseBool(value, "LNSFDS");
                    if (workerRange == null)
                        parameters._lnsFDS = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsFDS = parsed;
                    return true;
                }
                case "lnsfragmentfreezing":
                {
                    var parsed = ParseEnum(value, "LNSFragmentFreezing", new[] { "Off", "BeforeOnly", "On", "Unsafe" });
                    if (workerRange == null)
                        parameters._lnsFragmentFreezing = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsFragmentFreezing = parsed;
                    return true;
                }
                case "lnsrelaxslack":
                {
                    var parsed = ParseDouble(value, "LNSRelaxSlack");
                    if (parsed < 0.000000 || parsed > 1.000000)
                        throw new ArgumentException($"Value {parsed} for parameter LNSRelaxSlack is out of range [0.000000, 1.000000]");
                    if (workerRange == null)
                        parameters._lnsRelaxSlack = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsRelaxSlack = parsed;
                    return true;
                }
                case "lnsportionmultiplier":
                {
                    var parsed = ParseDouble(value, "LNSPortionMultiplier");
                    if (parsed < 0.000001 || parsed > 10.000000)
                        throw new ArgumentException($"Value {parsed} for parameter LNSPortionMultiplier is out of range [0.000001, 10.000000]");
                    if (workerRange == null)
                        parameters._lnsPortionMultiplier = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsPortionMultiplier = parsed;
                    return true;
                }
                case "simplelbworker":
                {
                    var parsed = ParseLong(value, "SimpleLBWorker");
                    if (parsed < -1 || parsed > 2147483647)
                        throw new ArgumentException($"Value {parsed} for parameter SimpleLBWorker is out of range [-1, 2147483647]");
                    if (workerRange == null)
                        parameters.simpleLBWorker = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).simpleLBWorker = parsed;
                    return true;
                }
                case "simplelbmaxiterations":
                {
                    var parsed = ParseLong(value, "SimpleLBMaxIterations");
                    if (parsed < 0 || parsed > 2147483647)
                        throw new ArgumentException($"Value {parsed} for parameter SimpleLBMaxIterations is out of range [0, 2147483647]");
                    if (workerRange == null)
                        parameters.simpleLBMaxIterations = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).simpleLBMaxIterations = parsed;
                    return true;
                }
                case "simplelbshavingrounds":
                {
                    var parsed = ParseLong(value, "SimpleLBShavingRounds");
                    if (parsed < 0 || parsed > 2147483647)
                        throw new ArgumentException($"Value {parsed} for parameter SimpleLBShavingRounds is out of range [0, 2147483647]");
                    if (workerRange == null)
                        parameters.simpleLBShavingRounds = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).simpleLBShavingRounds = parsed;
                    return true;
                }
                case "debugtracelevel":
                {
                    var parsed = ParseLong(value, "DebugTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter DebugTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._debugTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._debugTraceLevel = parsed;
                    return true;
                }
                case "memorytracelevel":
                {
                    var parsed = ParseLong(value, "MemoryTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter MemoryTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._memoryTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._memoryTraceLevel = parsed;
                    return true;
                }
                case "propagationdetailtracelevel":
                {
                    var parsed = ParseLong(value, "PropagationDetailTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter PropagationDetailTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._propagationDetailTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._propagationDetailTraceLevel = parsed;
                    return true;
                }
                case "settimestracelevel":
                {
                    var parsed = ParseLong(value, "SetTimesTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter SetTimesTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._setTimesTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesTraceLevel = parsed;
                    return true;
                }
                case "communicationtracelevel":
                {
                    var parsed = ParseLong(value, "CommunicationTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter CommunicationTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._communicationTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._communicationTraceLevel = parsed;
                    return true;
                }
                case "presolvetracelevel":
                {
                    var parsed = ParseLong(value, "PresolveTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter PresolveTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._presolveTraceLevel = parsed;
                    else
                        throw new ArgumentException("Parameter '_presolveTraceLevel' is global and cannot be set per-worker. Use '--_presolveTraceLevel' instead.");
                    return true;
                }
                case "conversiontracelevel":
                {
                    var parsed = ParseLong(value, "ConversionTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter ConversionTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._conversionTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._conversionTraceLevel = parsed;
                    return true;
                }
                case "expressionbuildertracelevel":
                {
                    var parsed = ParseLong(value, "ExpressionBuilderTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter ExpressionBuilderTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._expressionBuilderTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._expressionBuilderTraceLevel = parsed;
                    return true;
                }
                case "memorizationtracelevel":
                {
                    var parsed = ParseLong(value, "MemorizationTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter MemorizationTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._memorizationTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._memorizationTraceLevel = parsed;
                    return true;
                }
                case "searchdetailtracelevel":
                {
                    var parsed = ParseLong(value, "SearchDetailTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter SearchDetailTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._searchDetailTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._searchDetailTraceLevel = parsed;
                    return true;
                }
                case "fdstracelevel":
                {
                    var parsed = ParseLong(value, "FDSTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter FDSTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._fdsTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsTraceLevel = parsed;
                    return true;
                }
                case "shavingtracelevel":
                {
                    var parsed = ParseLong(value, "ShavingTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter ShavingTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._shavingTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._shavingTraceLevel = parsed;
                    return true;
                }
                case "fdsratingstracelevel":
                {
                    var parsed = ParseLong(value, "FDSRatingsTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter FDSRatingsTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._fdsRatingsTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._fdsRatingsTraceLevel = parsed;
                    return true;
                }
                case "lnstracelevel":
                {
                    var parsed = ParseLong(value, "LNSTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter LNSTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._lnsTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsTraceLevel = parsed;
                    return true;
                }
                case "heuristicreplaytracelevel":
                {
                    var parsed = ParseLong(value, "HeuristicReplayTraceLevel");
                    if (parsed < 0 || parsed > 5)
                        throw new ArgumentException($"Value {parsed} for parameter HeuristicReplayTraceLevel is out of range [0, 5]");
                    if (workerRange == null)
                        parameters._heuristicReplayTraceLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._heuristicReplayTraceLevel = parsed;
                    return true;
                }
                case "allowsettimesproofs":
                {
                    var parsed = ParseBool(value, "AllowSetTimesProofs");
                    if (workerRange == null)
                        parameters._allowSetTimesProofs = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._allowSetTimesProofs = parsed;
                    return true;
                }
                case "settimesaggressivedominance":
                {
                    var parsed = ParseBool(value, "SetTimesAggressiveDominance");
                    if (workerRange == null)
                        parameters._setTimesAggressiveDominance = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesAggressiveDominance = parsed;
                    return true;
                }
                case "settimesextendscoef":
                {
                    var parsed = ParseDouble(value, "SetTimesExtendsCoef");
                    if (workerRange == null)
                        parameters._setTimesExtendsCoef = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesExtendsCoef = parsed;
                    return true;
                }
                case "settimesheightstrategy":
                {
                    var parsed = ParseEnum(value, "SetTimesHeightStrategy", new[] { "FromMax", "FromMin", "Random" });
                    if (workerRange == null)
                        parameters._setTimesHeightStrategy = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesHeightStrategy = parsed;
                    return true;
                }
                case "settimesitvmappingstrategy":
                {
                    var parsed = ParseEnum(value, "SetTimesItvMappingStrategy", new[] { "FromMax", "FromMin", "Random" });
                    if (workerRange == null)
                        parameters._setTimesItvMappingStrategy = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesItvMappingStrategy = parsed;
                    return true;
                }
                case "settimesinitdensity":
                {
                    var parsed = ParseDouble(value, "SetTimesInitDensity");
                    if (workerRange == null)
                        parameters._setTimesInitDensity = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesInitDensity = parsed;
                    return true;
                }
                case "settimesdensitylength":
                {
                    var parsed = ParseLong(value, "SetTimesDensityLength");
                    if (workerRange == null)
                        parameters._setTimesDensityLength = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesDensityLength = parsed;
                    return true;
                }
                case "settimesdensityreliabilitythreshold":
                {
                    var parsed = ParseLong(value, "SetTimesDensityReliabilityThreshold");
                    if (workerRange == null)
                        parameters._setTimesDensityReliabilityThreshold = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesDensityReliabilityThreshold = parsed;
                    return true;
                }
                case "lnsdisabledheuristics":
                {
                    var parsed = ParseLong(value, "LNSDisabledHeuristics");
                    if (workerRange == null)
                        parameters._lnsDisabledHeuristics = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsDisabledHeuristics = parsed;
                    return true;
                }
                case "settimesnbextendsfactor":
                {
                    var parsed = ParseDouble(value, "SetTimesNbExtendsFactor");
                    if (workerRange == null)
                        parameters._setTimesNbExtendsFactor = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._setTimesNbExtendsFactor = parsed;
                    return true;
                }
                case "discretelowcapacitylimit":
                {
                    var parsed = ParseLong(value, "DiscreteLowCapacityLimit");
                    if (parsed < 0 || parsed > 16)
                        throw new ArgumentException($"Value {parsed} for parameter DiscreteLowCapacityLimit is out of range [0, 16]");
                    if (workerRange == null)
                        parameters._discreteLowCapacityLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._discreteLowCapacityLimit = parsed;
                    return true;
                }
                case "lnstrainingobjectivelimit":
                {
                    var parsed = ParseDouble(value, "LNSTrainingObjectiveLimit");
                    if (workerRange == null)
                        parameters._lnsTrainingObjectiveLimit = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._lnsTrainingObjectiveLimit = parsed;
                    return true;
                }
                case "posabsentrelated":
                {
                    var parsed = ParseBool(value, "POSAbsentRelated");
                    if (workerRange == null)
                        parameters._posAbsentRelated = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._posAbsentRelated = parsed;
                    return true;
                }
                case "defaultcallbackblocksize":
                {
                    var parsed = ParseLong(value, "DefaultCallbackBlockSize");
                    if (workerRange == null)
                        parameters._defaultCallbackBlockSize = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._defaultCallbackBlockSize = parsed;
                    return true;
                }
                case "usereservoirpegging":
                {
                    var parsed = ParseBool(value, "UseReservoirPegging");
                    if (workerRange == null)
                        parameters._useReservoirPegging = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._useReservoirPegging = parsed;
                    return true;
                }
                case "usetimenet":
                {
                    var parsed = ParseBool(value, "UseTimeNet");
                    if (workerRange == null)
                        parameters._useTimeNet = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._useTimeNet = parsed;
                    return true;
                }
                case "timenetvarstopreprocess":
                {
                    var parsed = ParseLong(value, "TimeNetVarsToPreprocess");
                    if (workerRange == null)
                        parameters._timeNetVarsToPreprocess = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._timeNetVarsToPreprocess = parsed;
                    return true;
                }
                case "timenetsubprioritybits":
                {
                    var parsed = ParseLong(value, "TimeNetSubPriorityBits");
                    if (parsed < 4 || parsed > 20)
                        throw new ArgumentException($"Value {parsed} for parameter TimeNetSubPriorityBits is out of range [4, 20]");
                    if (workerRange == null)
                        parameters._timeNetSubPriorityBits = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._timeNetSubPriorityBits = parsed;
                    return true;
                }
                case "minchainlength":
                {
                    var parsed = ParseLong(value, "MinChainLength");
                    if (workerRange == null)
                        parameters._minChainLength = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i)._minChainLength = parsed;
                    return true;
                }
                case "monitorinterval":
                {
                    var parsed = ParseDouble(value, "MonitorInterval");
                    if (workerRange == null)
                        parameters._monitorInterval = parsed;
                    else
                        throw new ArgumentException("Parameter '_monitorInterval' is global and cannot be set per-worker. Use '--_monitorInterval' instead.");
                    return true;
                }
                case "monitorthreshold":
                {
                    var parsed = ParseDouble(value, "MonitorThreshold");
                    if (parsed < 0 || parsed > 100)
                        throw new ArgumentException($"Value {parsed} for parameter MonitorThreshold is out of range [0, 100]");
                    if (workerRange == null)
                        parameters._monitorThreshold = parsed;
                    else
                        throw new ArgumentException("Parameter '_monitorThreshold' is global and cannot be set per-worker. Use '--_monitorThreshold' instead.");
                    return true;
                }


                default:
                    if (workerRange != null)
                        throw new ArgumentException($"Unknown worker parameter '{opt}' in '{name}'.");
                    return false;
            }
        }

        private static WorkerParameters GetOrCreateWorker(Parameters parameters, int workerIndex)
        {
            parameters.workers ??= new List<WorkerParameters>();
            while (parameters.workers.Count <= workerIndex)
                parameters.workers.Add(new WorkerParameters());
            return parameters.workers[workerIndex];
        }

        /// <summary>
        /// Creates a deep copy of the input Parameters object.
        /// </summary>
        /// <param name="params">The Parameters object to copy</param>
        /// <returns>A deep copy of the input Parameters object.</returns>
        /// <remarks>
        /// Creates a deep copy of the input <see cref="Parameters">Parameters</see> object.
        /// Afterwards, the copy can be modified without affecting
        /// the original <see cref="Parameters">Parameters</see> object.
        ///
        /// <code>
        /// var parameters = new Parameters { timeLimit = 60, nbWorkers = 4 };
        /// var copy = Parameters.CopyParameters(parameters);
        /// copy.timeLimit = 120; // Does not affect original parameters
        /// </code>
        /// </remarks>
        public static Parameters CopyParameters(Parameters @params)
        {
            var dst = new Parameters();
            dst.color = @params.color;
            dst.nbWorkers = @params.nbWorkers;
            dst._nbHelpers = @params._nbHelpers;
            dst.preset = @params.preset;
            dst.searchType = @params.searchType;
            dst.randomSeed = @params.randomSeed;
            dst.logLevel = @params.logLevel;
            dst.warningLevel = @params.warningLevel;
            dst.logPeriod = @params.logPeriod;
            dst.verifySolutions = @params.verifySolutions;
            dst.verifyExternalSolutions = @params.verifyExternalSolutions;
            dst.allocationBlockSize = @params.allocationBlockSize;
            dst.processExitTimeout = @params.processExitTimeout;
            dst.timeLimit = @params.timeLimit;
            dst.solutionLimit = @params.solutionLimit;
            dst._workerFailLimit = @params._workerFailLimit;
            dst._workerBranchLimit = @params._workerBranchLimit;
            dst._workerSolutionLimit = @params._workerSolutionLimit;
            dst._workerLNSStepLimit = @params._workerLNSStepLimit;
            dst._workerRestartLimit = @params._workerRestartLimit;
            dst.absoluteGapTolerance = @params.absoluteGapTolerance;
            dst.relativeGapTolerance = @params.relativeGapTolerance;
            dst.noOverlapPropagationLevel = @params.noOverlapPropagationLevel;
            dst.cumulPropagationLevel = @params.cumulPropagationLevel;
            dst.reservoirPropagationLevel = @params.reservoirPropagationLevel;
            dst.positionPropagationLevel = @params.positionPropagationLevel;
            dst.integralPropagationLevel = @params.integralPropagationLevel;
            dst.usePrecedenceEnergy = @params.usePrecedenceEnergy;
            dst._packPropagationLevel = @params._packPropagationLevel;
            dst._itvMappingPropagationLevel = @params._itvMappingPropagationLevel;
            dst.searchTraceLevel = @params.searchTraceLevel;
            dst.propagationTraceLevel = @params.propagationTraceLevel;
            dst.infoTraceLevel = @params.infoTraceLevel;
            dst.fdsInitialRating = @params.fdsInitialRating;
            dst.fdsReductionWeight = @params.fdsReductionWeight;
            dst.fdsRatingAverageLength = @params.fdsRatingAverageLength;
            dst.fdsFixedAlpha = @params.fdsFixedAlpha;
            dst.fdsRatingAverageComparison = @params.fdsRatingAverageComparison;
            dst.fdsReductionFactor = @params.fdsReductionFactor;
            dst.fdsReuseClosing = @params.fdsReuseClosing;
            dst.fdsUniformChoiceStep = @params.fdsUniformChoiceStep;
            dst.fdsLengthStepRatio = @params.fdsLengthStepRatio;
            dst.fdsMaxInitialChoicesPerVariable = @params.fdsMaxInitialChoicesPerVariable;
            dst.fdsAdditionalStepRatio = @params.fdsAdditionalStepRatio;
            dst.fdsPresenceStatusChoices = @params.fdsPresenceStatusChoices;
            dst.fdsMaxInitialLengthChoices = @params.fdsMaxInitialLengthChoices;
            dst.fdsMinLengthChoiceStep = @params.fdsMinLengthChoiceStep;
            dst.fdsMinIntVarChoiceStep = @params.fdsMinIntVarChoiceStep;
            dst.fdsEventTimeInfluence = @params.fdsEventTimeInfluence;
            dst.fdsBothFailRewardFactor = @params.fdsBothFailRewardFactor;
            dst.fdsEpsilon = @params.fdsEpsilon;
            dst.fdsStrongBranchingSize = @params.fdsStrongBranchingSize;
            dst.fdsStrongBranchingDepth = @params.fdsStrongBranchingDepth;
            dst.fdsStrongBranchingCriterion = @params.fdsStrongBranchingCriterion;
            dst.fdsInitialRestartLimit = @params.fdsInitialRestartLimit;
            dst.fdsRestartStrategy = @params.fdsRestartStrategy;
            dst.fdsRestartGrowthFactor = @params.fdsRestartGrowthFactor;
            dst.fdsMaxCounterAfterRestart = @params.fdsMaxCounterAfterRestart;
            dst.fdsMaxCounterAfterSolution = @params.fdsMaxCounterAfterSolution;
            dst.fdsResetRestartsAfterSolution = @params.fdsResetRestartsAfterSolution;
            dst.fdsUseNogoods = @params.fdsUseNogoods;
            dst._fdsFreezeRatingsAfterProof = @params._fdsFreezeRatingsAfterProof;
            dst._fdsContinueAfterProof = @params._fdsContinueAfterProof;
            dst._fdsRepeatLimit = @params._fdsRepeatLimit;
            dst._fdsCompletelyRandom = @params._fdsCompletelyRandom;
            dst.fdsBranchOnObjective = @params.fdsBranchOnObjective;
            dst._fdsImproveNogoods = @params._fdsImproveNogoods;
            dst.fdsBranchOrdering = @params.fdsBranchOrdering;
            dst._fdsDiveBySetTimes = @params._fdsDiveBySetTimes;
            dst.fdsDualStrategy = @params.fdsDualStrategy;
            dst.fdsDualResetRatings = @params.fdsDualResetRatings;
            dst.lnsMode = @params.lnsMode;
            dst._lnsInitNoOverlapPropagationLevel = @params._lnsInitNoOverlapPropagationLevel;
            dst._lnsInitCumulPropagationLevel = @params._lnsInitCumulPropagationLevel;
            dst._lnsFirstFailLimit = @params._lnsFirstFailLimit;
            dst._lnsFailLimitGrowthFactor = @params._lnsFailLimitGrowthFactor;
            dst._lnsFailLimitCoefficient = @params._lnsFailLimitCoefficient;
            dst._lnsIterationsAfterFirstSolution = @params._lnsIterationsAfterFirstSolution;
            dst._lnsAggressiveDominance = @params._lnsAggressiveDominance;
            dst._lnsSameSolutionPeriod = @params._lnsSameSolutionPeriod;
            dst._lnsTier1Size = @params._lnsTier1Size;
            dst._lnsTier2Size = @params._lnsTier2Size;
            dst._lnsTier3Size = @params._lnsTier3Size;
            dst._lnsTier2Effort = @params._lnsTier2Effort;
            dst._lnsTier3Effort = @params._lnsTier3Effort;
            dst._lnsStepFailLimitFactor = @params._lnsStepFailLimitFactor;
            dst._lnsApplyCutProbability = @params._lnsApplyCutProbability;
            dst._lnsSkipNoImprovementProbability = @params._lnsSkipNoImprovementProbability;
            dst._lnsSmallStructureLimit = @params._lnsSmallStructureLimit;
            dst._lnsResourceOptimization = @params._lnsResourceOptimization;
            dst._lnsRestoreAbsentIntervals = @params._lnsRestoreAbsentIntervals;
            dst._lnsRestoreIntervalLengths = @params._lnsRestoreIntervalLengths;
            dst._lnsRestoreIntVarValues = @params._lnsRestoreIntVarValues;
            dst._lnsAlwaysRelaxObjective = @params._lnsAlwaysRelaxObjective;
            dst.lnsUseWarmStartOnly = @params.lnsUseWarmStartOnly;
            dst._lnsHeuristicsEpsilon = @params._lnsHeuristicsEpsilon;
            dst._lnsHeuristicsAlpha = @params._lnsHeuristicsAlpha;
            dst._lnsHeuristicsTemperature = @params._lnsHeuristicsTemperature;
            dst._lnsHeuristicsUniform = @params._lnsHeuristicsUniform;
            dst._lnsHeuristicsInitialQ = @params._lnsHeuristicsInitialQ;
            dst._lnsPortionEpsilon = @params._lnsPortionEpsilon;
            dst._lnsPortionAlpha = @params._lnsPortionAlpha;
            dst._lnsPortionTemperature = @params._lnsPortionTemperature;
            dst._lnsPortionUniform = @params._lnsPortionUniform;
            dst._lnsPortionInitialQ = @params._lnsPortionInitialQ;
            dst._lnsPortionHandicapLimit = @params._lnsPortionHandicapLimit;
            dst._lnsPortionHandicapValue = @params._lnsPortionHandicapValue;
            dst._lnsPortionHandicapInitialQ = @params._lnsPortionHandicapInitialQ;
            dst._lnsNeighborhoodStrategy = @params._lnsNeighborhoodStrategy;
            dst._lnsNeighborhoodEpsilon = @params._lnsNeighborhoodEpsilon;
            dst._lnsNeighborhoodAlpha = @params._lnsNeighborhoodAlpha;
            dst._lnsNeighborhoodTemperature = @params._lnsNeighborhoodTemperature;
            dst._lnsNeighborhoodUniform = @params._lnsNeighborhoodUniform;
            dst._lnsNeighborhoodInitialQ = @params._lnsNeighborhoodInitialQ;
            dst._lnsDivingLimit = @params._lnsDivingLimit;
            dst._lnsDivingFailLimitRatio = @params._lnsDivingFailLimitRatio;
            dst._lnsLearningRun = @params._lnsLearningRun;
            dst._lnsStayOnObjective = @params._lnsStayOnObjective;
            dst._lnsFDS = @params._lnsFDS;
            dst._lnsFragmentFreezing = @params._lnsFragmentFreezing;
            dst._lnsRelaxSlack = @params._lnsRelaxSlack;
            dst._lnsPortionMultiplier = @params._lnsPortionMultiplier;
            dst.simpleLBWorker = @params.simpleLBWorker;
            dst.simpleLBMaxIterations = @params.simpleLBMaxIterations;
            dst.simpleLBShavingRounds = @params.simpleLBShavingRounds;
            dst._debugTraceLevel = @params._debugTraceLevel;
            dst._memoryTraceLevel = @params._memoryTraceLevel;
            dst._propagationDetailTraceLevel = @params._propagationDetailTraceLevel;
            dst._setTimesTraceLevel = @params._setTimesTraceLevel;
            dst._communicationTraceLevel = @params._communicationTraceLevel;
            dst._presolveTraceLevel = @params._presolveTraceLevel;
            dst._conversionTraceLevel = @params._conversionTraceLevel;
            dst._expressionBuilderTraceLevel = @params._expressionBuilderTraceLevel;
            dst._memorizationTraceLevel = @params._memorizationTraceLevel;
            dst._searchDetailTraceLevel = @params._searchDetailTraceLevel;
            dst._fdsTraceLevel = @params._fdsTraceLevel;
            dst._shavingTraceLevel = @params._shavingTraceLevel;
            dst._fdsRatingsTraceLevel = @params._fdsRatingsTraceLevel;
            dst._lnsTraceLevel = @params._lnsTraceLevel;
            dst._heuristicReplayTraceLevel = @params._heuristicReplayTraceLevel;
            dst._allowSetTimesProofs = @params._allowSetTimesProofs;
            dst._setTimesAggressiveDominance = @params._setTimesAggressiveDominance;
            dst._setTimesExtendsCoef = @params._setTimesExtendsCoef;
            dst._setTimesHeightStrategy = @params._setTimesHeightStrategy;
            dst._setTimesItvMappingStrategy = @params._setTimesItvMappingStrategy;
            dst._setTimesInitDensity = @params._setTimesInitDensity;
            dst._setTimesDensityLength = @params._setTimesDensityLength;
            dst._setTimesDensityReliabilityThreshold = @params._setTimesDensityReliabilityThreshold;
            dst._lnsDisabledHeuristics = @params._lnsDisabledHeuristics;
            dst._setTimesNbExtendsFactor = @params._setTimesNbExtendsFactor;
            dst._discreteLowCapacityLimit = @params._discreteLowCapacityLimit;
            dst._lnsTrainingObjectiveLimit = @params._lnsTrainingObjectiveLimit;
            dst._posAbsentRelated = @params._posAbsentRelated;
            dst._defaultCallbackBlockSize = @params._defaultCallbackBlockSize;
            dst._useReservoirPegging = @params._useReservoirPegging;
            dst._useTimeNet = @params._useTimeNet;
            dst._timeNetVarsToPreprocess = @params._timeNetVarsToPreprocess;
            dst._timeNetSubPriorityBits = @params._timeNetSubPriorityBits;
            dst._minChainLength = @params._minChainLength;
            dst._monitorInterval = @params._monitorInterval;
            dst._monitorThreshold = @params._monitorThreshold;

            if (@params.workers != null)
            {
                dst.workers = new List<WorkerParameters>();
                foreach (var w in @params.workers)
                {
                    var wc = new WorkerParameters();
                    wc.searchType = w.searchType;
                    wc.randomSeed = w.randomSeed;
                    wc._workerFailLimit = w._workerFailLimit;
                    wc._workerBranchLimit = w._workerBranchLimit;
                    wc._workerSolutionLimit = w._workerSolutionLimit;
                    wc._workerLNSStepLimit = w._workerLNSStepLimit;
                    wc._workerRestartLimit = w._workerRestartLimit;
                    wc.noOverlapPropagationLevel = w.noOverlapPropagationLevel;
                    wc.cumulPropagationLevel = w.cumulPropagationLevel;
                    wc.reservoirPropagationLevel = w.reservoirPropagationLevel;
                    wc.positionPropagationLevel = w.positionPropagationLevel;
                    wc.integralPropagationLevel = w.integralPropagationLevel;
                    wc._packPropagationLevel = w._packPropagationLevel;
                    wc._itvMappingPropagationLevel = w._itvMappingPropagationLevel;
                    wc.searchTraceLevel = w.searchTraceLevel;
                    wc.propagationTraceLevel = w.propagationTraceLevel;
                    wc.fdsInitialRating = w.fdsInitialRating;
                    wc.fdsReductionWeight = w.fdsReductionWeight;
                    wc.fdsRatingAverageLength = w.fdsRatingAverageLength;
                    wc.fdsFixedAlpha = w.fdsFixedAlpha;
                    wc.fdsRatingAverageComparison = w.fdsRatingAverageComparison;
                    wc.fdsReductionFactor = w.fdsReductionFactor;
                    wc.fdsReuseClosing = w.fdsReuseClosing;
                    wc.fdsUniformChoiceStep = w.fdsUniformChoiceStep;
                    wc.fdsLengthStepRatio = w.fdsLengthStepRatio;
                    wc.fdsMaxInitialChoicesPerVariable = w.fdsMaxInitialChoicesPerVariable;
                    wc.fdsAdditionalStepRatio = w.fdsAdditionalStepRatio;
                    wc.fdsPresenceStatusChoices = w.fdsPresenceStatusChoices;
                    wc.fdsMaxInitialLengthChoices = w.fdsMaxInitialLengthChoices;
                    wc.fdsMinLengthChoiceStep = w.fdsMinLengthChoiceStep;
                    wc.fdsMinIntVarChoiceStep = w.fdsMinIntVarChoiceStep;
                    wc.fdsEventTimeInfluence = w.fdsEventTimeInfluence;
                    wc.fdsBothFailRewardFactor = w.fdsBothFailRewardFactor;
                    wc.fdsEpsilon = w.fdsEpsilon;
                    wc.fdsStrongBranchingSize = w.fdsStrongBranchingSize;
                    wc.fdsStrongBranchingDepth = w.fdsStrongBranchingDepth;
                    wc.fdsStrongBranchingCriterion = w.fdsStrongBranchingCriterion;
                    wc.fdsInitialRestartLimit = w.fdsInitialRestartLimit;
                    wc.fdsRestartStrategy = w.fdsRestartStrategy;
                    wc.fdsRestartGrowthFactor = w.fdsRestartGrowthFactor;
                    wc.fdsMaxCounterAfterRestart = w.fdsMaxCounterAfterRestart;
                    wc.fdsMaxCounterAfterSolution = w.fdsMaxCounterAfterSolution;
                    wc.fdsResetRestartsAfterSolution = w.fdsResetRestartsAfterSolution;
                    wc.fdsUseNogoods = w.fdsUseNogoods;
                    wc._fdsFreezeRatingsAfterProof = w._fdsFreezeRatingsAfterProof;
                    wc._fdsContinueAfterProof = w._fdsContinueAfterProof;
                    wc._fdsRepeatLimit = w._fdsRepeatLimit;
                    wc._fdsCompletelyRandom = w._fdsCompletelyRandom;
                    wc.fdsBranchOnObjective = w.fdsBranchOnObjective;
                    wc._fdsImproveNogoods = w._fdsImproveNogoods;
                    wc.fdsBranchOrdering = w.fdsBranchOrdering;
                    wc._fdsDiveBySetTimes = w._fdsDiveBySetTimes;
                    wc.fdsDualStrategy = w.fdsDualStrategy;
                    wc.fdsDualResetRatings = w.fdsDualResetRatings;
                    wc.lnsMode = w.lnsMode;
                    wc._lnsInitNoOverlapPropagationLevel = w._lnsInitNoOverlapPropagationLevel;
                    wc._lnsInitCumulPropagationLevel = w._lnsInitCumulPropagationLevel;
                    wc._lnsFirstFailLimit = w._lnsFirstFailLimit;
                    wc._lnsFailLimitGrowthFactor = w._lnsFailLimitGrowthFactor;
                    wc._lnsFailLimitCoefficient = w._lnsFailLimitCoefficient;
                    wc._lnsIterationsAfterFirstSolution = w._lnsIterationsAfterFirstSolution;
                    wc._lnsAggressiveDominance = w._lnsAggressiveDominance;
                    wc._lnsSameSolutionPeriod = w._lnsSameSolutionPeriod;
                    wc._lnsTier1Size = w._lnsTier1Size;
                    wc._lnsTier2Size = w._lnsTier2Size;
                    wc._lnsTier3Size = w._lnsTier3Size;
                    wc._lnsTier2Effort = w._lnsTier2Effort;
                    wc._lnsTier3Effort = w._lnsTier3Effort;
                    wc._lnsStepFailLimitFactor = w._lnsStepFailLimitFactor;
                    wc._lnsApplyCutProbability = w._lnsApplyCutProbability;
                    wc._lnsSkipNoImprovementProbability = w._lnsSkipNoImprovementProbability;
                    wc._lnsSmallStructureLimit = w._lnsSmallStructureLimit;
                    wc._lnsResourceOptimization = w._lnsResourceOptimization;
                    wc._lnsRestoreAbsentIntervals = w._lnsRestoreAbsentIntervals;
                    wc._lnsRestoreIntervalLengths = w._lnsRestoreIntervalLengths;
                    wc._lnsRestoreIntVarValues = w._lnsRestoreIntVarValues;
                    wc._lnsAlwaysRelaxObjective = w._lnsAlwaysRelaxObjective;
                    wc.lnsUseWarmStartOnly = w.lnsUseWarmStartOnly;
                    wc._lnsHeuristicsEpsilon = w._lnsHeuristicsEpsilon;
                    wc._lnsHeuristicsAlpha = w._lnsHeuristicsAlpha;
                    wc._lnsHeuristicsTemperature = w._lnsHeuristicsTemperature;
                    wc._lnsHeuristicsUniform = w._lnsHeuristicsUniform;
                    wc._lnsHeuristicsInitialQ = w._lnsHeuristicsInitialQ;
                    wc._lnsPortionEpsilon = w._lnsPortionEpsilon;
                    wc._lnsPortionAlpha = w._lnsPortionAlpha;
                    wc._lnsPortionTemperature = w._lnsPortionTemperature;
                    wc._lnsPortionUniform = w._lnsPortionUniform;
                    wc._lnsPortionInitialQ = w._lnsPortionInitialQ;
                    wc._lnsPortionHandicapLimit = w._lnsPortionHandicapLimit;
                    wc._lnsPortionHandicapValue = w._lnsPortionHandicapValue;
                    wc._lnsPortionHandicapInitialQ = w._lnsPortionHandicapInitialQ;
                    wc._lnsNeighborhoodStrategy = w._lnsNeighborhoodStrategy;
                    wc._lnsNeighborhoodEpsilon = w._lnsNeighborhoodEpsilon;
                    wc._lnsNeighborhoodAlpha = w._lnsNeighborhoodAlpha;
                    wc._lnsNeighborhoodTemperature = w._lnsNeighborhoodTemperature;
                    wc._lnsNeighborhoodUniform = w._lnsNeighborhoodUniform;
                    wc._lnsNeighborhoodInitialQ = w._lnsNeighborhoodInitialQ;
                    wc._lnsDivingLimit = w._lnsDivingLimit;
                    wc._lnsDivingFailLimitRatio = w._lnsDivingFailLimitRatio;
                    wc._lnsLearningRun = w._lnsLearningRun;
                    wc._lnsStayOnObjective = w._lnsStayOnObjective;
                    wc._lnsFDS = w._lnsFDS;
                    wc._lnsFragmentFreezing = w._lnsFragmentFreezing;
                    wc._lnsRelaxSlack = w._lnsRelaxSlack;
                    wc._lnsPortionMultiplier = w._lnsPortionMultiplier;
                    wc.simpleLBWorker = w.simpleLBWorker;
                    wc.simpleLBMaxIterations = w.simpleLBMaxIterations;
                    wc.simpleLBShavingRounds = w.simpleLBShavingRounds;
                    wc._debugTraceLevel = w._debugTraceLevel;
                    wc._memoryTraceLevel = w._memoryTraceLevel;
                    wc._propagationDetailTraceLevel = w._propagationDetailTraceLevel;
                    wc._setTimesTraceLevel = w._setTimesTraceLevel;
                    wc._communicationTraceLevel = w._communicationTraceLevel;
                    wc._conversionTraceLevel = w._conversionTraceLevel;
                    wc._expressionBuilderTraceLevel = w._expressionBuilderTraceLevel;
                    wc._memorizationTraceLevel = w._memorizationTraceLevel;
                    wc._searchDetailTraceLevel = w._searchDetailTraceLevel;
                    wc._fdsTraceLevel = w._fdsTraceLevel;
                    wc._shavingTraceLevel = w._shavingTraceLevel;
                    wc._fdsRatingsTraceLevel = w._fdsRatingsTraceLevel;
                    wc._lnsTraceLevel = w._lnsTraceLevel;
                    wc._heuristicReplayTraceLevel = w._heuristicReplayTraceLevel;
                    wc._allowSetTimesProofs = w._allowSetTimesProofs;
                    wc._setTimesAggressiveDominance = w._setTimesAggressiveDominance;
                    wc._setTimesExtendsCoef = w._setTimesExtendsCoef;
                    wc._setTimesHeightStrategy = w._setTimesHeightStrategy;
                    wc._setTimesItvMappingStrategy = w._setTimesItvMappingStrategy;
                    wc._setTimesInitDensity = w._setTimesInitDensity;
                    wc._setTimesDensityLength = w._setTimesDensityLength;
                    wc._setTimesDensityReliabilityThreshold = w._setTimesDensityReliabilityThreshold;
                    wc._lnsDisabledHeuristics = w._lnsDisabledHeuristics;
                    wc._setTimesNbExtendsFactor = w._setTimesNbExtendsFactor;
                    wc._discreteLowCapacityLimit = w._discreteLowCapacityLimit;
                    wc._lnsTrainingObjectiveLimit = w._lnsTrainingObjectiveLimit;
                    wc._posAbsentRelated = w._posAbsentRelated;
                    wc._defaultCallbackBlockSize = w._defaultCallbackBlockSize;
                    wc._useReservoirPegging = w._useReservoirPegging;
                    wc._useTimeNet = w._useTimeNet;
                    wc._timeNetVarsToPreprocess = w._timeNetVarsToPreprocess;
                    wc._timeNetSubPriorityBits = w._timeNetSubPriorityBits;
                    wc._minChainLength = w._minChainLength;

                    dst.workers.Add(wc);
                }
            }
            dst.solverArgs = @params.solverArgs?.ToArray();
            dst.solver = @params.solver;
            dst.printLog = @params.printLog;
            return dst;
        }

        /// <summary>
        /// Merges two Parameters settings into a new one.
        /// </summary>
        /// <param name="base">Base parameters that can be overridden</param>
        /// <param name="overrides">Parameters that will overwrite values from base</param>
        /// <returns>The merged parameters object</returns>
        /// <remarks>
        /// The new object contains all parameters from both inputs. If the same
        /// parameter is specified in both input objects, then the value from <c>overrides</c>
        /// is used.
        ///
        /// Input objects are not modified.
        ///
        /// <code>
        /// var defaults = new Parameters { timeLimit = 60, nbWorkers = 4 };
        /// var overrides = new Parameters { timeLimit = 120 };
        /// var merged = Parameters.MergeParameters(defaults, overrides);
        /// // merged = { timeLimit = 120, nbWorkers = 4 }
        /// </code>
        /// </remarks>
        public static Parameters MergeParameters(Parameters @base, Parameters overrides)
        {
            var dst = CopyParameters(@base);

            // Overlay non-null scalar parameters from overrides
            if (overrides.color != null) dst.color = overrides.color;
            if (overrides.nbWorkers != null) dst.nbWorkers = overrides.nbWorkers;
            if (overrides._nbHelpers != null) dst._nbHelpers = overrides._nbHelpers;
            if (overrides.preset != null) dst.preset = overrides.preset;
            if (overrides.searchType != null) dst.searchType = overrides.searchType;
            if (overrides.randomSeed != null) dst.randomSeed = overrides.randomSeed;
            if (overrides.logLevel != null) dst.logLevel = overrides.logLevel;
            if (overrides.warningLevel != null) dst.warningLevel = overrides.warningLevel;
            if (overrides.logPeriod != null) dst.logPeriod = overrides.logPeriod;
            if (overrides.verifySolutions != null) dst.verifySolutions = overrides.verifySolutions;
            if (overrides.verifyExternalSolutions != null) dst.verifyExternalSolutions = overrides.verifyExternalSolutions;
            if (overrides.allocationBlockSize != null) dst.allocationBlockSize = overrides.allocationBlockSize;
            if (overrides.processExitTimeout != null) dst.processExitTimeout = overrides.processExitTimeout;
            if (overrides.timeLimit != null) dst.timeLimit = overrides.timeLimit;
            if (overrides.solutionLimit != null) dst.solutionLimit = overrides.solutionLimit;
            if (overrides._workerFailLimit != null) dst._workerFailLimit = overrides._workerFailLimit;
            if (overrides._workerBranchLimit != null) dst._workerBranchLimit = overrides._workerBranchLimit;
            if (overrides._workerSolutionLimit != null) dst._workerSolutionLimit = overrides._workerSolutionLimit;
            if (overrides._workerLNSStepLimit != null) dst._workerLNSStepLimit = overrides._workerLNSStepLimit;
            if (overrides._workerRestartLimit != null) dst._workerRestartLimit = overrides._workerRestartLimit;
            if (overrides.absoluteGapTolerance != null) dst.absoluteGapTolerance = overrides.absoluteGapTolerance;
            if (overrides.relativeGapTolerance != null) dst.relativeGapTolerance = overrides.relativeGapTolerance;
            if (overrides.noOverlapPropagationLevel != null) dst.noOverlapPropagationLevel = overrides.noOverlapPropagationLevel;
            if (overrides.cumulPropagationLevel != null) dst.cumulPropagationLevel = overrides.cumulPropagationLevel;
            if (overrides.reservoirPropagationLevel != null) dst.reservoirPropagationLevel = overrides.reservoirPropagationLevel;
            if (overrides.positionPropagationLevel != null) dst.positionPropagationLevel = overrides.positionPropagationLevel;
            if (overrides.integralPropagationLevel != null) dst.integralPropagationLevel = overrides.integralPropagationLevel;
            if (overrides.usePrecedenceEnergy != null) dst.usePrecedenceEnergy = overrides.usePrecedenceEnergy;
            if (overrides._packPropagationLevel != null) dst._packPropagationLevel = overrides._packPropagationLevel;
            if (overrides._itvMappingPropagationLevel != null) dst._itvMappingPropagationLevel = overrides._itvMappingPropagationLevel;
            if (overrides.searchTraceLevel != null) dst.searchTraceLevel = overrides.searchTraceLevel;
            if (overrides.propagationTraceLevel != null) dst.propagationTraceLevel = overrides.propagationTraceLevel;
            if (overrides.infoTraceLevel != null) dst.infoTraceLevel = overrides.infoTraceLevel;
            if (overrides.fdsInitialRating != null) dst.fdsInitialRating = overrides.fdsInitialRating;
            if (overrides.fdsReductionWeight != null) dst.fdsReductionWeight = overrides.fdsReductionWeight;
            if (overrides.fdsRatingAverageLength != null) dst.fdsRatingAverageLength = overrides.fdsRatingAverageLength;
            if (overrides.fdsFixedAlpha != null) dst.fdsFixedAlpha = overrides.fdsFixedAlpha;
            if (overrides.fdsRatingAverageComparison != null) dst.fdsRatingAverageComparison = overrides.fdsRatingAverageComparison;
            if (overrides.fdsReductionFactor != null) dst.fdsReductionFactor = overrides.fdsReductionFactor;
            if (overrides.fdsReuseClosing != null) dst.fdsReuseClosing = overrides.fdsReuseClosing;
            if (overrides.fdsUniformChoiceStep != null) dst.fdsUniformChoiceStep = overrides.fdsUniformChoiceStep;
            if (overrides.fdsLengthStepRatio != null) dst.fdsLengthStepRatio = overrides.fdsLengthStepRatio;
            if (overrides.fdsMaxInitialChoicesPerVariable != null) dst.fdsMaxInitialChoicesPerVariable = overrides.fdsMaxInitialChoicesPerVariable;
            if (overrides.fdsAdditionalStepRatio != null) dst.fdsAdditionalStepRatio = overrides.fdsAdditionalStepRatio;
            if (overrides.fdsPresenceStatusChoices != null) dst.fdsPresenceStatusChoices = overrides.fdsPresenceStatusChoices;
            if (overrides.fdsMaxInitialLengthChoices != null) dst.fdsMaxInitialLengthChoices = overrides.fdsMaxInitialLengthChoices;
            if (overrides.fdsMinLengthChoiceStep != null) dst.fdsMinLengthChoiceStep = overrides.fdsMinLengthChoiceStep;
            if (overrides.fdsMinIntVarChoiceStep != null) dst.fdsMinIntVarChoiceStep = overrides.fdsMinIntVarChoiceStep;
            if (overrides.fdsEventTimeInfluence != null) dst.fdsEventTimeInfluence = overrides.fdsEventTimeInfluence;
            if (overrides.fdsBothFailRewardFactor != null) dst.fdsBothFailRewardFactor = overrides.fdsBothFailRewardFactor;
            if (overrides.fdsEpsilon != null) dst.fdsEpsilon = overrides.fdsEpsilon;
            if (overrides.fdsStrongBranchingSize != null) dst.fdsStrongBranchingSize = overrides.fdsStrongBranchingSize;
            if (overrides.fdsStrongBranchingDepth != null) dst.fdsStrongBranchingDepth = overrides.fdsStrongBranchingDepth;
            if (overrides.fdsStrongBranchingCriterion != null) dst.fdsStrongBranchingCriterion = overrides.fdsStrongBranchingCriterion;
            if (overrides.fdsInitialRestartLimit != null) dst.fdsInitialRestartLimit = overrides.fdsInitialRestartLimit;
            if (overrides.fdsRestartStrategy != null) dst.fdsRestartStrategy = overrides.fdsRestartStrategy;
            if (overrides.fdsRestartGrowthFactor != null) dst.fdsRestartGrowthFactor = overrides.fdsRestartGrowthFactor;
            if (overrides.fdsMaxCounterAfterRestart != null) dst.fdsMaxCounterAfterRestart = overrides.fdsMaxCounterAfterRestart;
            if (overrides.fdsMaxCounterAfterSolution != null) dst.fdsMaxCounterAfterSolution = overrides.fdsMaxCounterAfterSolution;
            if (overrides.fdsResetRestartsAfterSolution != null) dst.fdsResetRestartsAfterSolution = overrides.fdsResetRestartsAfterSolution;
            if (overrides.fdsUseNogoods != null) dst.fdsUseNogoods = overrides.fdsUseNogoods;
            if (overrides._fdsFreezeRatingsAfterProof != null) dst._fdsFreezeRatingsAfterProof = overrides._fdsFreezeRatingsAfterProof;
            if (overrides._fdsContinueAfterProof != null) dst._fdsContinueAfterProof = overrides._fdsContinueAfterProof;
            if (overrides._fdsRepeatLimit != null) dst._fdsRepeatLimit = overrides._fdsRepeatLimit;
            if (overrides._fdsCompletelyRandom != null) dst._fdsCompletelyRandom = overrides._fdsCompletelyRandom;
            if (overrides.fdsBranchOnObjective != null) dst.fdsBranchOnObjective = overrides.fdsBranchOnObjective;
            if (overrides._fdsImproveNogoods != null) dst._fdsImproveNogoods = overrides._fdsImproveNogoods;
            if (overrides.fdsBranchOrdering != null) dst.fdsBranchOrdering = overrides.fdsBranchOrdering;
            if (overrides._fdsDiveBySetTimes != null) dst._fdsDiveBySetTimes = overrides._fdsDiveBySetTimes;
            if (overrides.fdsDualStrategy != null) dst.fdsDualStrategy = overrides.fdsDualStrategy;
            if (overrides.fdsDualResetRatings != null) dst.fdsDualResetRatings = overrides.fdsDualResetRatings;
            if (overrides.lnsMode != null) dst.lnsMode = overrides.lnsMode;
            if (overrides._lnsInitNoOverlapPropagationLevel != null) dst._lnsInitNoOverlapPropagationLevel = overrides._lnsInitNoOverlapPropagationLevel;
            if (overrides._lnsInitCumulPropagationLevel != null) dst._lnsInitCumulPropagationLevel = overrides._lnsInitCumulPropagationLevel;
            if (overrides._lnsFirstFailLimit != null) dst._lnsFirstFailLimit = overrides._lnsFirstFailLimit;
            if (overrides._lnsFailLimitGrowthFactor != null) dst._lnsFailLimitGrowthFactor = overrides._lnsFailLimitGrowthFactor;
            if (overrides._lnsFailLimitCoefficient != null) dst._lnsFailLimitCoefficient = overrides._lnsFailLimitCoefficient;
            if (overrides._lnsIterationsAfterFirstSolution != null) dst._lnsIterationsAfterFirstSolution = overrides._lnsIterationsAfterFirstSolution;
            if (overrides._lnsAggressiveDominance != null) dst._lnsAggressiveDominance = overrides._lnsAggressiveDominance;
            if (overrides._lnsSameSolutionPeriod != null) dst._lnsSameSolutionPeriod = overrides._lnsSameSolutionPeriod;
            if (overrides._lnsTier1Size != null) dst._lnsTier1Size = overrides._lnsTier1Size;
            if (overrides._lnsTier2Size != null) dst._lnsTier2Size = overrides._lnsTier2Size;
            if (overrides._lnsTier3Size != null) dst._lnsTier3Size = overrides._lnsTier3Size;
            if (overrides._lnsTier2Effort != null) dst._lnsTier2Effort = overrides._lnsTier2Effort;
            if (overrides._lnsTier3Effort != null) dst._lnsTier3Effort = overrides._lnsTier3Effort;
            if (overrides._lnsStepFailLimitFactor != null) dst._lnsStepFailLimitFactor = overrides._lnsStepFailLimitFactor;
            if (overrides._lnsApplyCutProbability != null) dst._lnsApplyCutProbability = overrides._lnsApplyCutProbability;
            if (overrides._lnsSkipNoImprovementProbability != null) dst._lnsSkipNoImprovementProbability = overrides._lnsSkipNoImprovementProbability;
            if (overrides._lnsSmallStructureLimit != null) dst._lnsSmallStructureLimit = overrides._lnsSmallStructureLimit;
            if (overrides._lnsResourceOptimization != null) dst._lnsResourceOptimization = overrides._lnsResourceOptimization;
            if (overrides._lnsRestoreAbsentIntervals != null) dst._lnsRestoreAbsentIntervals = overrides._lnsRestoreAbsentIntervals;
            if (overrides._lnsRestoreIntervalLengths != null) dst._lnsRestoreIntervalLengths = overrides._lnsRestoreIntervalLengths;
            if (overrides._lnsRestoreIntVarValues != null) dst._lnsRestoreIntVarValues = overrides._lnsRestoreIntVarValues;
            if (overrides._lnsAlwaysRelaxObjective != null) dst._lnsAlwaysRelaxObjective = overrides._lnsAlwaysRelaxObjective;
            if (overrides.lnsUseWarmStartOnly != null) dst.lnsUseWarmStartOnly = overrides.lnsUseWarmStartOnly;
            if (overrides._lnsHeuristicsEpsilon != null) dst._lnsHeuristicsEpsilon = overrides._lnsHeuristicsEpsilon;
            if (overrides._lnsHeuristicsAlpha != null) dst._lnsHeuristicsAlpha = overrides._lnsHeuristicsAlpha;
            if (overrides._lnsHeuristicsTemperature != null) dst._lnsHeuristicsTemperature = overrides._lnsHeuristicsTemperature;
            if (overrides._lnsHeuristicsUniform != null) dst._lnsHeuristicsUniform = overrides._lnsHeuristicsUniform;
            if (overrides._lnsHeuristicsInitialQ != null) dst._lnsHeuristicsInitialQ = overrides._lnsHeuristicsInitialQ;
            if (overrides._lnsPortionEpsilon != null) dst._lnsPortionEpsilon = overrides._lnsPortionEpsilon;
            if (overrides._lnsPortionAlpha != null) dst._lnsPortionAlpha = overrides._lnsPortionAlpha;
            if (overrides._lnsPortionTemperature != null) dst._lnsPortionTemperature = overrides._lnsPortionTemperature;
            if (overrides._lnsPortionUniform != null) dst._lnsPortionUniform = overrides._lnsPortionUniform;
            if (overrides._lnsPortionInitialQ != null) dst._lnsPortionInitialQ = overrides._lnsPortionInitialQ;
            if (overrides._lnsPortionHandicapLimit != null) dst._lnsPortionHandicapLimit = overrides._lnsPortionHandicapLimit;
            if (overrides._lnsPortionHandicapValue != null) dst._lnsPortionHandicapValue = overrides._lnsPortionHandicapValue;
            if (overrides._lnsPortionHandicapInitialQ != null) dst._lnsPortionHandicapInitialQ = overrides._lnsPortionHandicapInitialQ;
            if (overrides._lnsNeighborhoodStrategy != null) dst._lnsNeighborhoodStrategy = overrides._lnsNeighborhoodStrategy;
            if (overrides._lnsNeighborhoodEpsilon != null) dst._lnsNeighborhoodEpsilon = overrides._lnsNeighborhoodEpsilon;
            if (overrides._lnsNeighborhoodAlpha != null) dst._lnsNeighborhoodAlpha = overrides._lnsNeighborhoodAlpha;
            if (overrides._lnsNeighborhoodTemperature != null) dst._lnsNeighborhoodTemperature = overrides._lnsNeighborhoodTemperature;
            if (overrides._lnsNeighborhoodUniform != null) dst._lnsNeighborhoodUniform = overrides._lnsNeighborhoodUniform;
            if (overrides._lnsNeighborhoodInitialQ != null) dst._lnsNeighborhoodInitialQ = overrides._lnsNeighborhoodInitialQ;
            if (overrides._lnsDivingLimit != null) dst._lnsDivingLimit = overrides._lnsDivingLimit;
            if (overrides._lnsDivingFailLimitRatio != null) dst._lnsDivingFailLimitRatio = overrides._lnsDivingFailLimitRatio;
            if (overrides._lnsLearningRun != null) dst._lnsLearningRun = overrides._lnsLearningRun;
            if (overrides._lnsStayOnObjective != null) dst._lnsStayOnObjective = overrides._lnsStayOnObjective;
            if (overrides._lnsFDS != null) dst._lnsFDS = overrides._lnsFDS;
            if (overrides._lnsFragmentFreezing != null) dst._lnsFragmentFreezing = overrides._lnsFragmentFreezing;
            if (overrides._lnsRelaxSlack != null) dst._lnsRelaxSlack = overrides._lnsRelaxSlack;
            if (overrides._lnsPortionMultiplier != null) dst._lnsPortionMultiplier = overrides._lnsPortionMultiplier;
            if (overrides.simpleLBWorker != null) dst.simpleLBWorker = overrides.simpleLBWorker;
            if (overrides.simpleLBMaxIterations != null) dst.simpleLBMaxIterations = overrides.simpleLBMaxIterations;
            if (overrides.simpleLBShavingRounds != null) dst.simpleLBShavingRounds = overrides.simpleLBShavingRounds;
            if (overrides._debugTraceLevel != null) dst._debugTraceLevel = overrides._debugTraceLevel;
            if (overrides._memoryTraceLevel != null) dst._memoryTraceLevel = overrides._memoryTraceLevel;
            if (overrides._propagationDetailTraceLevel != null) dst._propagationDetailTraceLevel = overrides._propagationDetailTraceLevel;
            if (overrides._setTimesTraceLevel != null) dst._setTimesTraceLevel = overrides._setTimesTraceLevel;
            if (overrides._communicationTraceLevel != null) dst._communicationTraceLevel = overrides._communicationTraceLevel;
            if (overrides._presolveTraceLevel != null) dst._presolveTraceLevel = overrides._presolveTraceLevel;
            if (overrides._conversionTraceLevel != null) dst._conversionTraceLevel = overrides._conversionTraceLevel;
            if (overrides._expressionBuilderTraceLevel != null) dst._expressionBuilderTraceLevel = overrides._expressionBuilderTraceLevel;
            if (overrides._memorizationTraceLevel != null) dst._memorizationTraceLevel = overrides._memorizationTraceLevel;
            if (overrides._searchDetailTraceLevel != null) dst._searchDetailTraceLevel = overrides._searchDetailTraceLevel;
            if (overrides._fdsTraceLevel != null) dst._fdsTraceLevel = overrides._fdsTraceLevel;
            if (overrides._shavingTraceLevel != null) dst._shavingTraceLevel = overrides._shavingTraceLevel;
            if (overrides._fdsRatingsTraceLevel != null) dst._fdsRatingsTraceLevel = overrides._fdsRatingsTraceLevel;
            if (overrides._lnsTraceLevel != null) dst._lnsTraceLevel = overrides._lnsTraceLevel;
            if (overrides._heuristicReplayTraceLevel != null) dst._heuristicReplayTraceLevel = overrides._heuristicReplayTraceLevel;
            if (overrides._allowSetTimesProofs != null) dst._allowSetTimesProofs = overrides._allowSetTimesProofs;
            if (overrides._setTimesAggressiveDominance != null) dst._setTimesAggressiveDominance = overrides._setTimesAggressiveDominance;
            if (overrides._setTimesExtendsCoef != null) dst._setTimesExtendsCoef = overrides._setTimesExtendsCoef;
            if (overrides._setTimesHeightStrategy != null) dst._setTimesHeightStrategy = overrides._setTimesHeightStrategy;
            if (overrides._setTimesItvMappingStrategy != null) dst._setTimesItvMappingStrategy = overrides._setTimesItvMappingStrategy;
            if (overrides._setTimesInitDensity != null) dst._setTimesInitDensity = overrides._setTimesInitDensity;
            if (overrides._setTimesDensityLength != null) dst._setTimesDensityLength = overrides._setTimesDensityLength;
            if (overrides._setTimesDensityReliabilityThreshold != null) dst._setTimesDensityReliabilityThreshold = overrides._setTimesDensityReliabilityThreshold;
            if (overrides._lnsDisabledHeuristics != null) dst._lnsDisabledHeuristics = overrides._lnsDisabledHeuristics;
            if (overrides._setTimesNbExtendsFactor != null) dst._setTimesNbExtendsFactor = overrides._setTimesNbExtendsFactor;
            if (overrides._discreteLowCapacityLimit != null) dst._discreteLowCapacityLimit = overrides._discreteLowCapacityLimit;
            if (overrides._lnsTrainingObjectiveLimit != null) dst._lnsTrainingObjectiveLimit = overrides._lnsTrainingObjectiveLimit;
            if (overrides._posAbsentRelated != null) dst._posAbsentRelated = overrides._posAbsentRelated;
            if (overrides._defaultCallbackBlockSize != null) dst._defaultCallbackBlockSize = overrides._defaultCallbackBlockSize;
            if (overrides._useReservoirPegging != null) dst._useReservoirPegging = overrides._useReservoirPegging;
            if (overrides._useTimeNet != null) dst._useTimeNet = overrides._useTimeNet;
            if (overrides._timeNetVarsToPreprocess != null) dst._timeNetVarsToPreprocess = overrides._timeNetVarsToPreprocess;
            if (overrides._timeNetSubPriorityBits != null) dst._timeNetSubPriorityBits = overrides._timeNetSubPriorityBits;
            if (overrides._minChainLength != null) dst._minChainLength = overrides._minChainLength;
            if (overrides._monitorInterval != null) dst._monitorInterval = overrides._monitorInterval;
            if (overrides._monitorThreshold != null) dst._monitorThreshold = overrides._monitorThreshold;

            if (overrides.solverArgs != null) dst.solverArgs = overrides.solverArgs.ToArray();
            if (overrides.solver != null) dst.solver = overrides.solver;
            if (overrides.printLog != null) dst.printLog = overrides.printLog;

            // Merge workers per-index
            if (overrides.workers != null)
            {
                dst.workers ??= new List<WorkerParameters>();
                for (int i = 0; i < overrides.workers.Count; i++)
                {
                    var ow = overrides.workers[i];
                    if (ow == null)
                        continue;
                    // Extend list if needed
                    while (dst.workers.Count <= i)
                        dst.workers.Add(new WorkerParameters());
                    var rw = dst.workers[i];
                    if (ow.searchType != null) rw.searchType = ow.searchType;
                    if (ow.randomSeed != null) rw.randomSeed = ow.randomSeed;
                    if (ow._workerFailLimit != null) rw._workerFailLimit = ow._workerFailLimit;
                    if (ow._workerBranchLimit != null) rw._workerBranchLimit = ow._workerBranchLimit;
                    if (ow._workerSolutionLimit != null) rw._workerSolutionLimit = ow._workerSolutionLimit;
                    if (ow._workerLNSStepLimit != null) rw._workerLNSStepLimit = ow._workerLNSStepLimit;
                    if (ow._workerRestartLimit != null) rw._workerRestartLimit = ow._workerRestartLimit;
                    if (ow.noOverlapPropagationLevel != null) rw.noOverlapPropagationLevel = ow.noOverlapPropagationLevel;
                    if (ow.cumulPropagationLevel != null) rw.cumulPropagationLevel = ow.cumulPropagationLevel;
                    if (ow.reservoirPropagationLevel != null) rw.reservoirPropagationLevel = ow.reservoirPropagationLevel;
                    if (ow.positionPropagationLevel != null) rw.positionPropagationLevel = ow.positionPropagationLevel;
                    if (ow.integralPropagationLevel != null) rw.integralPropagationLevel = ow.integralPropagationLevel;
                    if (ow._packPropagationLevel != null) rw._packPropagationLevel = ow._packPropagationLevel;
                    if (ow._itvMappingPropagationLevel != null) rw._itvMappingPropagationLevel = ow._itvMappingPropagationLevel;
                    if (ow.searchTraceLevel != null) rw.searchTraceLevel = ow.searchTraceLevel;
                    if (ow.propagationTraceLevel != null) rw.propagationTraceLevel = ow.propagationTraceLevel;
                    if (ow.fdsInitialRating != null) rw.fdsInitialRating = ow.fdsInitialRating;
                    if (ow.fdsReductionWeight != null) rw.fdsReductionWeight = ow.fdsReductionWeight;
                    if (ow.fdsRatingAverageLength != null) rw.fdsRatingAverageLength = ow.fdsRatingAverageLength;
                    if (ow.fdsFixedAlpha != null) rw.fdsFixedAlpha = ow.fdsFixedAlpha;
                    if (ow.fdsRatingAverageComparison != null) rw.fdsRatingAverageComparison = ow.fdsRatingAverageComparison;
                    if (ow.fdsReductionFactor != null) rw.fdsReductionFactor = ow.fdsReductionFactor;
                    if (ow.fdsReuseClosing != null) rw.fdsReuseClosing = ow.fdsReuseClosing;
                    if (ow.fdsUniformChoiceStep != null) rw.fdsUniformChoiceStep = ow.fdsUniformChoiceStep;
                    if (ow.fdsLengthStepRatio != null) rw.fdsLengthStepRatio = ow.fdsLengthStepRatio;
                    if (ow.fdsMaxInitialChoicesPerVariable != null) rw.fdsMaxInitialChoicesPerVariable = ow.fdsMaxInitialChoicesPerVariable;
                    if (ow.fdsAdditionalStepRatio != null) rw.fdsAdditionalStepRatio = ow.fdsAdditionalStepRatio;
                    if (ow.fdsPresenceStatusChoices != null) rw.fdsPresenceStatusChoices = ow.fdsPresenceStatusChoices;
                    if (ow.fdsMaxInitialLengthChoices != null) rw.fdsMaxInitialLengthChoices = ow.fdsMaxInitialLengthChoices;
                    if (ow.fdsMinLengthChoiceStep != null) rw.fdsMinLengthChoiceStep = ow.fdsMinLengthChoiceStep;
                    if (ow.fdsMinIntVarChoiceStep != null) rw.fdsMinIntVarChoiceStep = ow.fdsMinIntVarChoiceStep;
                    if (ow.fdsEventTimeInfluence != null) rw.fdsEventTimeInfluence = ow.fdsEventTimeInfluence;
                    if (ow.fdsBothFailRewardFactor != null) rw.fdsBothFailRewardFactor = ow.fdsBothFailRewardFactor;
                    if (ow.fdsEpsilon != null) rw.fdsEpsilon = ow.fdsEpsilon;
                    if (ow.fdsStrongBranchingSize != null) rw.fdsStrongBranchingSize = ow.fdsStrongBranchingSize;
                    if (ow.fdsStrongBranchingDepth != null) rw.fdsStrongBranchingDepth = ow.fdsStrongBranchingDepth;
                    if (ow.fdsStrongBranchingCriterion != null) rw.fdsStrongBranchingCriterion = ow.fdsStrongBranchingCriterion;
                    if (ow.fdsInitialRestartLimit != null) rw.fdsInitialRestartLimit = ow.fdsInitialRestartLimit;
                    if (ow.fdsRestartStrategy != null) rw.fdsRestartStrategy = ow.fdsRestartStrategy;
                    if (ow.fdsRestartGrowthFactor != null) rw.fdsRestartGrowthFactor = ow.fdsRestartGrowthFactor;
                    if (ow.fdsMaxCounterAfterRestart != null) rw.fdsMaxCounterAfterRestart = ow.fdsMaxCounterAfterRestart;
                    if (ow.fdsMaxCounterAfterSolution != null) rw.fdsMaxCounterAfterSolution = ow.fdsMaxCounterAfterSolution;
                    if (ow.fdsResetRestartsAfterSolution != null) rw.fdsResetRestartsAfterSolution = ow.fdsResetRestartsAfterSolution;
                    if (ow.fdsUseNogoods != null) rw.fdsUseNogoods = ow.fdsUseNogoods;
                    if (ow._fdsFreezeRatingsAfterProof != null) rw._fdsFreezeRatingsAfterProof = ow._fdsFreezeRatingsAfterProof;
                    if (ow._fdsContinueAfterProof != null) rw._fdsContinueAfterProof = ow._fdsContinueAfterProof;
                    if (ow._fdsRepeatLimit != null) rw._fdsRepeatLimit = ow._fdsRepeatLimit;
                    if (ow._fdsCompletelyRandom != null) rw._fdsCompletelyRandom = ow._fdsCompletelyRandom;
                    if (ow.fdsBranchOnObjective != null) rw.fdsBranchOnObjective = ow.fdsBranchOnObjective;
                    if (ow._fdsImproveNogoods != null) rw._fdsImproveNogoods = ow._fdsImproveNogoods;
                    if (ow.fdsBranchOrdering != null) rw.fdsBranchOrdering = ow.fdsBranchOrdering;
                    if (ow._fdsDiveBySetTimes != null) rw._fdsDiveBySetTimes = ow._fdsDiveBySetTimes;
                    if (ow.fdsDualStrategy != null) rw.fdsDualStrategy = ow.fdsDualStrategy;
                    if (ow.fdsDualResetRatings != null) rw.fdsDualResetRatings = ow.fdsDualResetRatings;
                    if (ow.lnsMode != null) rw.lnsMode = ow.lnsMode;
                    if (ow._lnsInitNoOverlapPropagationLevel != null) rw._lnsInitNoOverlapPropagationLevel = ow._lnsInitNoOverlapPropagationLevel;
                    if (ow._lnsInitCumulPropagationLevel != null) rw._lnsInitCumulPropagationLevel = ow._lnsInitCumulPropagationLevel;
                    if (ow._lnsFirstFailLimit != null) rw._lnsFirstFailLimit = ow._lnsFirstFailLimit;
                    if (ow._lnsFailLimitGrowthFactor != null) rw._lnsFailLimitGrowthFactor = ow._lnsFailLimitGrowthFactor;
                    if (ow._lnsFailLimitCoefficient != null) rw._lnsFailLimitCoefficient = ow._lnsFailLimitCoefficient;
                    if (ow._lnsIterationsAfterFirstSolution != null) rw._lnsIterationsAfterFirstSolution = ow._lnsIterationsAfterFirstSolution;
                    if (ow._lnsAggressiveDominance != null) rw._lnsAggressiveDominance = ow._lnsAggressiveDominance;
                    if (ow._lnsSameSolutionPeriod != null) rw._lnsSameSolutionPeriod = ow._lnsSameSolutionPeriod;
                    if (ow._lnsTier1Size != null) rw._lnsTier1Size = ow._lnsTier1Size;
                    if (ow._lnsTier2Size != null) rw._lnsTier2Size = ow._lnsTier2Size;
                    if (ow._lnsTier3Size != null) rw._lnsTier3Size = ow._lnsTier3Size;
                    if (ow._lnsTier2Effort != null) rw._lnsTier2Effort = ow._lnsTier2Effort;
                    if (ow._lnsTier3Effort != null) rw._lnsTier3Effort = ow._lnsTier3Effort;
                    if (ow._lnsStepFailLimitFactor != null) rw._lnsStepFailLimitFactor = ow._lnsStepFailLimitFactor;
                    if (ow._lnsApplyCutProbability != null) rw._lnsApplyCutProbability = ow._lnsApplyCutProbability;
                    if (ow._lnsSkipNoImprovementProbability != null) rw._lnsSkipNoImprovementProbability = ow._lnsSkipNoImprovementProbability;
                    if (ow._lnsSmallStructureLimit != null) rw._lnsSmallStructureLimit = ow._lnsSmallStructureLimit;
                    if (ow._lnsResourceOptimization != null) rw._lnsResourceOptimization = ow._lnsResourceOptimization;
                    if (ow._lnsRestoreAbsentIntervals != null) rw._lnsRestoreAbsentIntervals = ow._lnsRestoreAbsentIntervals;
                    if (ow._lnsRestoreIntervalLengths != null) rw._lnsRestoreIntervalLengths = ow._lnsRestoreIntervalLengths;
                    if (ow._lnsRestoreIntVarValues != null) rw._lnsRestoreIntVarValues = ow._lnsRestoreIntVarValues;
                    if (ow._lnsAlwaysRelaxObjective != null) rw._lnsAlwaysRelaxObjective = ow._lnsAlwaysRelaxObjective;
                    if (ow.lnsUseWarmStartOnly != null) rw.lnsUseWarmStartOnly = ow.lnsUseWarmStartOnly;
                    if (ow._lnsHeuristicsEpsilon != null) rw._lnsHeuristicsEpsilon = ow._lnsHeuristicsEpsilon;
                    if (ow._lnsHeuristicsAlpha != null) rw._lnsHeuristicsAlpha = ow._lnsHeuristicsAlpha;
                    if (ow._lnsHeuristicsTemperature != null) rw._lnsHeuristicsTemperature = ow._lnsHeuristicsTemperature;
                    if (ow._lnsHeuristicsUniform != null) rw._lnsHeuristicsUniform = ow._lnsHeuristicsUniform;
                    if (ow._lnsHeuristicsInitialQ != null) rw._lnsHeuristicsInitialQ = ow._lnsHeuristicsInitialQ;
                    if (ow._lnsPortionEpsilon != null) rw._lnsPortionEpsilon = ow._lnsPortionEpsilon;
                    if (ow._lnsPortionAlpha != null) rw._lnsPortionAlpha = ow._lnsPortionAlpha;
                    if (ow._lnsPortionTemperature != null) rw._lnsPortionTemperature = ow._lnsPortionTemperature;
                    if (ow._lnsPortionUniform != null) rw._lnsPortionUniform = ow._lnsPortionUniform;
                    if (ow._lnsPortionInitialQ != null) rw._lnsPortionInitialQ = ow._lnsPortionInitialQ;
                    if (ow._lnsPortionHandicapLimit != null) rw._lnsPortionHandicapLimit = ow._lnsPortionHandicapLimit;
                    if (ow._lnsPortionHandicapValue != null) rw._lnsPortionHandicapValue = ow._lnsPortionHandicapValue;
                    if (ow._lnsPortionHandicapInitialQ != null) rw._lnsPortionHandicapInitialQ = ow._lnsPortionHandicapInitialQ;
                    if (ow._lnsNeighborhoodStrategy != null) rw._lnsNeighborhoodStrategy = ow._lnsNeighborhoodStrategy;
                    if (ow._lnsNeighborhoodEpsilon != null) rw._lnsNeighborhoodEpsilon = ow._lnsNeighborhoodEpsilon;
                    if (ow._lnsNeighborhoodAlpha != null) rw._lnsNeighborhoodAlpha = ow._lnsNeighborhoodAlpha;
                    if (ow._lnsNeighborhoodTemperature != null) rw._lnsNeighborhoodTemperature = ow._lnsNeighborhoodTemperature;
                    if (ow._lnsNeighborhoodUniform != null) rw._lnsNeighborhoodUniform = ow._lnsNeighborhoodUniform;
                    if (ow._lnsNeighborhoodInitialQ != null) rw._lnsNeighborhoodInitialQ = ow._lnsNeighborhoodInitialQ;
                    if (ow._lnsDivingLimit != null) rw._lnsDivingLimit = ow._lnsDivingLimit;
                    if (ow._lnsDivingFailLimitRatio != null) rw._lnsDivingFailLimitRatio = ow._lnsDivingFailLimitRatio;
                    if (ow._lnsLearningRun != null) rw._lnsLearningRun = ow._lnsLearningRun;
                    if (ow._lnsStayOnObjective != null) rw._lnsStayOnObjective = ow._lnsStayOnObjective;
                    if (ow._lnsFDS != null) rw._lnsFDS = ow._lnsFDS;
                    if (ow._lnsFragmentFreezing != null) rw._lnsFragmentFreezing = ow._lnsFragmentFreezing;
                    if (ow._lnsRelaxSlack != null) rw._lnsRelaxSlack = ow._lnsRelaxSlack;
                    if (ow._lnsPortionMultiplier != null) rw._lnsPortionMultiplier = ow._lnsPortionMultiplier;
                    if (ow.simpleLBWorker != null) rw.simpleLBWorker = ow.simpleLBWorker;
                    if (ow.simpleLBMaxIterations != null) rw.simpleLBMaxIterations = ow.simpleLBMaxIterations;
                    if (ow.simpleLBShavingRounds != null) rw.simpleLBShavingRounds = ow.simpleLBShavingRounds;
                    if (ow._debugTraceLevel != null) rw._debugTraceLevel = ow._debugTraceLevel;
                    if (ow._memoryTraceLevel != null) rw._memoryTraceLevel = ow._memoryTraceLevel;
                    if (ow._propagationDetailTraceLevel != null) rw._propagationDetailTraceLevel = ow._propagationDetailTraceLevel;
                    if (ow._setTimesTraceLevel != null) rw._setTimesTraceLevel = ow._setTimesTraceLevel;
                    if (ow._communicationTraceLevel != null) rw._communicationTraceLevel = ow._communicationTraceLevel;
                    if (ow._conversionTraceLevel != null) rw._conversionTraceLevel = ow._conversionTraceLevel;
                    if (ow._expressionBuilderTraceLevel != null) rw._expressionBuilderTraceLevel = ow._expressionBuilderTraceLevel;
                    if (ow._memorizationTraceLevel != null) rw._memorizationTraceLevel = ow._memorizationTraceLevel;
                    if (ow._searchDetailTraceLevel != null) rw._searchDetailTraceLevel = ow._searchDetailTraceLevel;
                    if (ow._fdsTraceLevel != null) rw._fdsTraceLevel = ow._fdsTraceLevel;
                    if (ow._shavingTraceLevel != null) rw._shavingTraceLevel = ow._shavingTraceLevel;
                    if (ow._fdsRatingsTraceLevel != null) rw._fdsRatingsTraceLevel = ow._fdsRatingsTraceLevel;
                    if (ow._lnsTraceLevel != null) rw._lnsTraceLevel = ow._lnsTraceLevel;
                    if (ow._heuristicReplayTraceLevel != null) rw._heuristicReplayTraceLevel = ow._heuristicReplayTraceLevel;
                    if (ow._allowSetTimesProofs != null) rw._allowSetTimesProofs = ow._allowSetTimesProofs;
                    if (ow._setTimesAggressiveDominance != null) rw._setTimesAggressiveDominance = ow._setTimesAggressiveDominance;
                    if (ow._setTimesExtendsCoef != null) rw._setTimesExtendsCoef = ow._setTimesExtendsCoef;
                    if (ow._setTimesHeightStrategy != null) rw._setTimesHeightStrategy = ow._setTimesHeightStrategy;
                    if (ow._setTimesItvMappingStrategy != null) rw._setTimesItvMappingStrategy = ow._setTimesItvMappingStrategy;
                    if (ow._setTimesInitDensity != null) rw._setTimesInitDensity = ow._setTimesInitDensity;
                    if (ow._setTimesDensityLength != null) rw._setTimesDensityLength = ow._setTimesDensityLength;
                    if (ow._setTimesDensityReliabilityThreshold != null) rw._setTimesDensityReliabilityThreshold = ow._setTimesDensityReliabilityThreshold;
                    if (ow._lnsDisabledHeuristics != null) rw._lnsDisabledHeuristics = ow._lnsDisabledHeuristics;
                    if (ow._setTimesNbExtendsFactor != null) rw._setTimesNbExtendsFactor = ow._setTimesNbExtendsFactor;
                    if (ow._discreteLowCapacityLimit != null) rw._discreteLowCapacityLimit = ow._discreteLowCapacityLimit;
                    if (ow._lnsTrainingObjectiveLimit != null) rw._lnsTrainingObjectiveLimit = ow._lnsTrainingObjectiveLimit;
                    if (ow._posAbsentRelated != null) rw._posAbsentRelated = ow._posAbsentRelated;
                    if (ow._defaultCallbackBlockSize != null) rw._defaultCallbackBlockSize = ow._defaultCallbackBlockSize;
                    if (ow._useReservoirPegging != null) rw._useReservoirPegging = ow._useReservoirPegging;
                    if (ow._useTimeNet != null) rw._useTimeNet = ow._useTimeNet;
                    if (ow._timeNetVarsToPreprocess != null) rw._timeNetVarsToPreprocess = ow._timeNetVarsToPreprocess;
                    if (ow._timeNetSubPriorityBits != null) rw._timeNetSubPriorityBits = ow._timeNetSubPriorityBits;
                    if (ow._minChainLength != null) rw._minChainLength = ow._minChainLength;

                }
            }

            return dst;
        }

        private static bool ParseBool(string value, string paramName)
        {
            var lower = value.ToLowerInvariant();
            if (lower is "1" or "t" or "true" or "y" or "yes")
                return true;
            if (lower is "0" or "f" or "false" or "n" or "no")
                return false;
            throw new ArgumentException($"Value '{value}' for parameter {paramName} is not a boolean.");
        }

        private static double ParseDouble(string value, string paramName)
        {
            if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase) || value == "inf")
                return double.PositiveInfinity;
            if (value.Equals("-Infinity", StringComparison.OrdinalIgnoreCase) || value == "-inf")
                return double.NegativeInfinity;
            if (double.TryParse(value, System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;
            throw new ArgumentException($"Value '{value}' for parameter {paramName} is not a number.");
        }

        private static long ParseLong(string value, string paramName)
        {
            if (long.TryParse(value, out var result))
                return result;
            throw new ArgumentException($"Value '{value}' for parameter {paramName} is not an integer.");
        }

        private static string ParseEnum(string value, string paramName, string[] validValues)
        {
            foreach (var valid in validValues)
            {
                if (string.Equals(valid, value, StringComparison.OrdinalIgnoreCase))
                    return valid;
            }
            var validStr = string.Join(", ", validValues);
            throw new ArgumentException($"Value '{value}' for parameter {paramName} is not valid. Valid values: {validStr}");
        }
    }


    /// <summary>
    /// Specifies the behavior of each worker separately.
    /// </summary>
    /// <remarks>
    /// WorkerParameters specify the behavior of each worker separately.
    /// It is part of the <see cref="Parameters">Parameters</see> object.
    ///
    /// If a parameter is not listed here, then it can be set only globally (in <see cref="Parameters">Parameters</see>), not per worker.  For example, _timeLimit_ or _logPeriod_ are
    /// global parameters.
    /// </remarks>
    public class WorkerParameters
    {
        /// <summary>
        /// Specifies the behavior of each worker separately.
        /// </summary>
        /// <remarks>
        /// WorkerParameters specify the behavior of each worker separately.
        /// It is part of the <see cref="Parameters">Parameters</see> object.
        ///
        /// If a parameter is not listed here, then it can be set only globally (in <see cref="Parameters">Parameters</see>), not per worker.  For example, _timeLimit_ or _logPeriod_ are
        /// global parameters.
        /// </remarks>
        public WorkerParameters() { }

        /// <summary>
        /// Type of search to use
        /// </summary>
        /// <remarks>
        /// This parameter controls which search algorithm the solver uses. Different search types have different strengths:
        ///
        /// - <c>Auto</c>: Automatically determined based on the <see cref="Parameters.preset">Parameters.preset</see> (the default). With the <c>Default</c> preset, workers are distributed across LNS, FDS, and FDSDual. With the <c>Large</c> preset, all workers use LNS.
        ///
        /// - <c>LNS</c>: Large Neighborhood Search. Starts from an initial solution and iteratively improves it by relaxing and re-optimizing parts of the solution. Good for finding high-quality solutions quickly, especially on large problems. Works best when a good initial solution can be found.
        ///
        /// - <c>FDS</c>: Failure-Directed Search. A systematic search that learns from failures to guide exploration. Uses restarts with no-good learning. Often effective at proving optimality and works well with strong propagation.
        ///
        /// - <c>FDSDual</c>: Failure-Directed Search working on objective bounds. Similar to FDS but focuses on proving bounds on the objective value. Useful for optimization problems where you want to know how far from optimal your solutions are.
        ///
        /// - <c>SetTimes</c>: Depth-first set-times search (not restarted). A simple chronological search that assigns start times in order. Can be effective for tightly constrained problems but generally less robust than other methods.
        ///
        /// Interaction with presets:
        ///
        /// When <c>searchType</c> is set to <c>Auto</c>, the actual search type is determined by the <see cref="Parameters.preset">Parameters.preset</see>:
        ///
        /// - <c>Default</c> preset: Distributes workers across different search types. Half use LNS, 3/8 use FDS, and the rest use FDSDual. This portfolio approach provides robustness across different problem types.
        ///
        /// - <c>Large</c> preset: All workers use LNS. For very large problems, the overhead of systematic search methods like FDS becomes prohibitive, so LNS is used exclusively.
        ///
        /// If you explicitly set <c>searchType</c> to a specific value (not <c>Auto</c>), that value is used regardless of the preset.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model ...
        ///
        /// // Let the preset decide (default behavior)
        /// var result = model.Solve();
        ///
        /// // Or explicitly use FDS for systematic search
        /// result = model.Solve(new Parameters { searchType = "FDS" });
        ///
        /// // Or use LNS for quick solutions on large problems
        /// result = model.Solve(new Parameters { searchType = "LNS" });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of search and propagation.</description></item>
        /// <item><description><see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see> — which works well with FDS at higher levels.</description></item>
        /// </list>
        /// </remarks>
        public string? searchType { get; set; }

        /// <summary>
        /// Random seed
        /// </summary>
        /// <remarks>
        /// The solver breaks ties randomly using a pseudorandom number generator. This parameter sets the seed of the generator.
        ///
        /// Note that when <see cref="Parameters.nbWorkers">Parameters.nbWorkers</see> is more than 1 then there is also another source of randomness: the time it takes for a message to pass from one worker to another. Therefore with 1 worker the solver is deterministic (random behavior depends only on random seed). With more workers the solver is not deterministic.
        ///
        /// Even with the same random seed, the solver may behave differently on different platforms. This can be due to different implementations of certain functions such as <c>std::sort</c>.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>1</c>.
        ///
        /// <b>Environment variable <c>OPTALCP_RANDOM_SEED</c></b>
        ///
        /// When <c>randomSeed</c> is not explicitly set (i.e. it remains at its default value of 1), the solver checks the environment variable <c>OPTALCP_RANDOM_SEED</c>. If the variable is set:
        ///
        /// *  A numeric value (e.g. <c>42</c>) is used directly as the random seed.
        /// *  The special value <c>RANDOM</c> (case-insensitive) tells the solver to generate a time-based seed.
        ///
        /// This is useful for randomizing test runs without changing application code.
        /// </remarks>
        public long? randomSeed { get; set; }

        /// <summary>
        /// Fail limit for each worker
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerFailLimit { get; set; }

        /// <summary>
        /// Branch limit for every worker
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerBranchLimit { get; set; }

        /// <summary>
        /// Stop the worker after the given number of solutions
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerSolutionLimit { get; set; }

        /// <summary>
        /// Stop the worker after given number of LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerLNSStepLimit { get; set; }

        /// <summary>
        /// Stop the worker after the given number of restarts
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _workerRestartLimit { get; set; }

        /// <summary>
        /// How much to propagate noOverlap constraints
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for noOverlap constraints. Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
        ///
        /// Propagation levels:
        ///
        /// - Level 1: Basic timetable propagation only
        /// - Level 2: Adds detectable precedences algorithm
        /// - Level 3: Adds edge-finding reasoning
        /// - Level 4: Maximum propagation with all available algorithms
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the propagation level is determined automatically based on the <see cref="Parameters.preset">Parameters.preset</see>:
        ///
        /// - <c>Default</c> preset: Uses level 4 (maximum propagation)
        /// - <c>Large</c> preset: Uses level 1 (minimum propagation for scalability)
        ///
        /// Performance considerations:
        ///
        /// More propagation doesn't necessarily mean better overall performance. The trade-off depends on your problem:
        ///
        /// - Dense scheduling problems with many overlapping intervals often benefit from higher propagation levels because the extra pruning reduces the search space significantly.
        ///
        /// - Sparse problems or very large problems may perform better with lower propagation levels because the overhead of sophisticated algorithms outweighs the benefit.
        ///
        /// - FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) typically benefits from higher propagation levels because it relies on strong propagation to guide the search.
        ///
        /// If you're unsure, start with the automatic selection (level 0) and let the preset choose. You can then experiment with explicit levels if needed.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model with noOverlap constraints ...
        ///
        /// // Let the preset decide (default)
        /// var result = model.Solve();
        ///
        /// // Or use maximum propagation for dense problems
        /// result = model.Solve(new Parameters { noOverlapPropagationLevel = 4 });
        ///
        /// // Or use minimum propagation for very large problems
        /// result = model.Solve(new Parameters { noOverlapPropagationLevel = 1 });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for creating noOverlap constraints.</description></item>
        /// </list>
        /// </remarks>
        public long? noOverlapPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate constraints on cumul functions
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for cumulative constraints (e.g., <c>cumul &lt;= limit</c>) when used with a sum of <see cref="Model.Pulse">Model.Pulse</see> pulses.
        ///
        /// Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
        ///
        /// Propagation levels:
        ///
        /// - Level 1: Basic timetable propagation
        /// - Level 2: Adds time-table edge-finding
        /// - Level 3: Maximum propagation with all available algorithms
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the propagation level is determined automatically based on the <see cref="Parameters.preset">Parameters.preset</see>:
        ///
        /// - <c>Default</c> preset: Uses level 3 (maximum propagation)
        /// - <c>Large</c> preset: Uses level 1 (minimum propagation for scalability)
        ///
        /// Performance considerations:
        ///
        /// More propagation doesn't necessarily mean better overall performance. The trade-off depends on your problem:
        ///
        /// - Resource-constrained problems with tight capacity limits often benefit from higher propagation levels because cumulative reasoning can prune many infeasible assignments.
        ///
        /// - Problems with loose resource constraints may not benefit much from higher levels because the extra computation doesn't lead to significant pruning.
        ///
        /// - Very large problems may perform better with lower propagation levels because the overhead becomes prohibitive.
        ///
        /// - FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) typically benefits from higher propagation levels.
        ///
        /// If you're unsure, start with the automatic selection (level 0) and let the preset choose.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model with cumulative constraints ...
        ///
        /// // Let the preset decide (default)
        /// var result = model.Solve();
        ///
        /// // Or use maximum propagation for resource-constrained problems
        /// result = model.Solve(new Parameters { cumulPropagationLevel = 3 });
        ///
        /// // Or use minimum propagation for very large problems
        /// result = model.Solve(new Parameters { cumulPropagationLevel = 1 });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// <item><description><see cref="Model.Pulse">Model.Pulse</see> — for creating pulse contributions to cumulative functions.</description></item>
        /// </list>
        /// </remarks>
        public long? cumulPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate constraints on cumul functions
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for cumulative constraints (e.g., <c>cumul &lt;= limit</c>, <c>cumul &gt;= limit</c>) when used together with steps (<see cref="Model.StepAtStart">Model.StepAtStart</see>, <see cref="Model.StepAtEnd">Model.StepAtEnd</see>, <see cref="Model.StepAt">Model.StepAt</see>).
        /// The bigger the value, the more algorithms are used for propagation.
        /// It means that more time is spent by the propagation, and possibly more values are removed from domains.
        /// More propagation doesn't necessarily mean better performance.
        /// FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) usually benefits from higher propagation levels.
        ///
        /// The parameter takes an integer value in range <c>1..2</c>.
        ///
        /// The default value is <c>1</c>.
        /// </remarks>
        public long? reservoirPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate position expressions on noOverlap constraints
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for position expressions on noOverlap constraints.
        /// The bigger the value, the more algorithms are used for propagation.
        /// It means that more time is spent by the propagation, and possibly more values are removed from domains.
        /// However, more propagation doesn't necessarily mean better performance.
        /// FDS search (see <see cref="Parameters.searchType">Parameters.searchType</see>) usually benefits from higher propagation levels.
        ///
        /// The parameter takes an integer value in range <c>1..3</c>.
        ///
        /// The default value is <c>2</c>.
        /// </remarks>
        public long? positionPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate integral expression
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for <see cref="Model.Integral">Model.Integral</see> expressions.
        /// In particular, it controls whether the propagation also affects the minimum and the maximum length of the associated interval variable:
        ///
        /// * <c>1</c>: The length is updated only once during initial constraint propagation.
        /// * <c>2</c>: The length is updated every time the expression is propagated.
        ///
        /// The parameter takes an integer value in range <c>1..2</c>.
        ///
        /// The default value is <c>1</c>.
        /// </remarks>
        public long? integralPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate pack constraints
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _packPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate itvMapping constraint
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _itvMappingPropagationLevel { get; set; }

        /// <summary>
        /// Level of search trace
        /// </summary>
        /// <remarks>
        /// This parameter is available only in the development edition of the solver.
        ///
        /// When set to a value bigger than zero, the solver prints a trace of the search.
        /// The trace contains information about every choice taken by the solver.
        /// The higher the value, the more information is printed.
        ///
        /// The parameter takes an integer value in range <c>0..5</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? searchTraceLevel { get; set; }

        /// <summary>
        /// Level of propagation trace
        /// </summary>
        /// <remarks>
        /// This parameter is available only in the development edition of the solver.
        ///
        /// When set to a value bigger than zero, the solver prints a trace of the propagation,
        /// that is a line for every domain change.
        /// The higher the value, the more information is printed.
        ///
        /// The parameter takes an integer value in range <c>0..5</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? propagationTraceLevel { get; set; }

        /// <summary>
        /// Initial rating for newly created choices
        /// </summary>
        /// <remarks>
        /// Default rating for newly created choices. Both left and right branches get the same rating.
        /// Choice is initially permuted so that bigger domain change is the left branch.
        ///
        /// The parameter takes a floating point value in range <c>0.0..2.0</c>.
        ///
        /// The default value is <c>0.5</c>.
        /// </remarks>
        public double? fdsInitialRating { get; set; }

        /// <summary>
        /// Weight of the reduction factor in rating computation
        /// </summary>
        /// <remarks>
        /// When computing the local rating of a branch, multiply reduction factor by the given weight.
        ///
        /// The parameter takes a floating point value in range <c>0.0..Infinity</c>.
        ///
        /// The default value is <c>1</c>.
        /// </remarks>
        public double? fdsReductionWeight { get; set; }

        /// <summary>
        /// Length of average rating computed for choices
        /// </summary>
        /// <remarks>
        /// For the computation of rating of a branch. Arithmetic average is used until the branch
        /// is taken at least FDSRatingAverageLength times. After that exponential moving average
        /// is used with parameter alpha = 1 - 1 / FDSRatingAverageLength.
        ///
        /// The parameter takes an integer value in range <c>0..254</c>.
        ///
        /// The default value is <c>25</c>.
        /// </remarks>
        public long? fdsRatingAverageLength { get; set; }

        /// <summary>
        /// When non-zero, alpha factor for rating updates
        /// </summary>
        /// <remarks>
        /// When this parameter is set to a non-zero, parameter FDSRatingAverageLength is ignored.
        /// Instead, the rating of a branch is computed as an exponential moving average with the given parameter alpha.
        ///
        /// The parameter takes a floating point value in range <c>0..1</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public double? fdsFixedAlpha { get; set; }

        /// <summary>
        /// Whether to compare the local rating with the average
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        ///  * <c>Off</c> (the default): No comparison is done.
        ///  * <c>Global</c>: Compare with the global average.
        ///  * <c>Depth</c>: Compare with the average on the current search depth
        ///
        /// Arithmetic average is used for global and depth averages.
        ///
        /// The default value is <c>Off</c>.
        /// </remarks>
        public string? fdsRatingAverageComparison { get; set; }

        /// <summary>
        /// Reduction factor R for rating computation
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        ///  * <c>Normal</c> (the default): Normal reduction factor.
        ///  * <c>Zero</c>: Factor is not used (it is 0 all the time).
        ///  * <c>Random</c>: A random number in the range [0,1] is used instead.
        ///
        /// The default value is <c>Normal</c>.
        /// </remarks>
        public string? fdsReductionFactor { get; set; }

        /// <summary>
        /// Whether always reuse closing choice
        /// </summary>
        /// <remarks>
        /// Most of the time, FDS reuses closing choice automatically. This parameter enforces it all the time.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? fdsReuseClosing { get; set; }

        /// <summary>
        /// Whether all initial choices have the same step length
        /// </summary>
        /// <remarks>
        /// When set, then initial choices generated on interval variables will have the same step size.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsUniformChoiceStep { get; set; }

        /// <summary>
        /// Choice step relative to average length
        /// </summary>
        /// <remarks>
        /// Ratio of initial choice step size to the minimum length of interval variable. When FDSUniformChoiceStep is set, this ratio is used to compute global choice step using the average of interval var length. When FDSUniformChoiceStep is not set, this ratio is used to compute the choice step for every interval var individually.
        ///
        /// The parameter takes a floating point value in range <c>0.0..Infinity</c>.
        ///
        /// The default value is <c>0.699999988079071</c>.
        /// </remarks>
        public double? fdsLengthStepRatio { get; set; }

        /// <summary>
        /// Maximum number of choices generated initially per a variable
        /// </summary>
        /// <remarks>
        /// Initial domains are often very large (e.g., <c>0..IntervalMax</c>). Therefore initial
        /// number of generated choices is limited: only choices near startMin are kept.
        ///
        /// The parameter takes an integer value in range <c>2..2147483647</c>.
        ///
        /// The default value is <c>90</c>.
        /// </remarks>
        public long? fdsMaxInitialChoicesPerVariable { get; set; }

        /// <summary>
        /// Domain split ratio when run out of choices
        /// </summary>
        /// <remarks>
        /// When all choices are decided, and a greedy algorithm cannot find a solution, then
        /// more choices are generated by splitting domains into the specified number of pieces.
        ///
        /// The parameter takes a floating point value in range <c>2.0..Infinity</c>.
        ///
        /// The default value is <c>7</c>.
        /// </remarks>
        public double? fdsAdditionalStepRatio { get; set; }

        /// <summary>
        /// Whether to generate choices on presence status
        /// </summary>
        /// <remarks>
        /// Choices on start time also include a choice on presence status. Therefore, dedicated choices on presence status only are not mandatory.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsPresenceStatusChoices { get; set; }

        /// <summary>
        /// Maximum number of initial choices on length of an interval variable
        /// </summary>
        /// <remarks>
        /// When non-zero, this parameter limits the number of initial choices generated on length of an interval variable.
        /// When zero (the default), no choices on length are generated.
        ///
        /// The parameter takes an integer value in range <c>0..2147483647</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? fdsMaxInitialLengthChoices { get; set; }

        /// <summary>
        /// Maximum step when generating initial choices for length of an interval variable
        /// </summary>
        /// <remarks>
        /// Steps between choices for length of an interval variable are never bigger than the specified value.
        ///
        /// The parameter takes an integer value in range <c>1..1073741823</c>.
        ///
        /// The default value is <c>1073741823</c>.
        /// </remarks>
        public long? fdsMinLengthChoiceStep { get; set; }

        /// <summary>
        /// Minimum step when generating choices for integer variables.
        /// </summary>
        /// <remarks>
        /// Steps between choices for integer variables are never smaller than the specified value.
        ///
        /// The parameter takes an integer value in range <c>1..1073741823</c>.
        ///
        /// The default value is <c>1073741823</c>.
        /// </remarks>
        public long? fdsMinIntVarChoiceStep { get; set; }

        /// <summary>
        /// Influence of event time to initial choice rating
        /// </summary>
        /// <remarks>
        /// When non-zero, the initial choice rating is influenced by the date of the choice.
        /// This way, very first choices in the search should be taken chronologically.
        ///
        /// The parameter takes a floating point value in range <c>0..1</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public double? fdsEventTimeInfluence { get; set; }

        /// <summary>
        /// How much to improve rating when both branches fail immediately
        /// </summary>
        /// <remarks>
        /// This parameter sets a bonus reward for a choice when both left and right branches fail immediately.
        /// Current rating of both branches is multiplied by the specified value.
        ///
        /// The parameter takes a floating point value in range <c>0..1</c>.
        ///
        /// The default value is <c>0.98</c>.
        /// </remarks>
        public double? fdsBothFailRewardFactor { get; set; }

        /// <summary>
        /// How often to chose a choice randomly
        /// </summary>
        /// <remarks>
        /// Probability that a choice is taken randomly. A randomly selected choice is not added to the search tree automatically. Instead, the choice is tried, its rating is updated,
        /// but it is added to the search tree only if one of the branches fails.
        /// The mechanism is similar to strong branching.
        ///
        /// The parameter takes a floating point value in range <c>0.0..0.99999</c>.
        ///
        /// The default value is <c>0.1</c>.
        /// </remarks>
        public double? fdsEpsilon { get; set; }

        /// <summary>
        /// Number of choices to try in strong branching
        /// </summary>
        /// <remarks>
        /// Strong branching means that instead of taking a choice with the best rating,
        /// we take the specified number (FDSStrongBranchingSize) of best choices,
        /// try them in dry-run mode, measure their local rating, and
        /// then chose the one with the best local rating.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>10</c>.
        /// </remarks>
        public long? fdsStrongBranchingSize { get; set; }

        /// <summary>
        /// Up-to what search depth apply strong branching
        /// </summary>
        /// <remarks>
        /// Strong branching is typically used in the root node. This parameter controls
        /// the maximum search depth when strong branching is used.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>6</c>.
        /// </remarks>
        public long? fdsStrongBranchingDepth { get; set; }

        /// <summary>
        /// How to choose the best choice in strong branching
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        /// * <c>Both</c>: Choose the the choice with best combined rating.
        /// * <c>Left</c> (the default): Choose the choice with the best rating of the left branch.
        /// * <c>Right</c>: Choose the choice with the best rating of the right branch.
        ///
        /// The default value is <c>Left</c>.
        /// </remarks>
        public string? fdsStrongBranchingCriterion { get; set; }

        /// <summary>
        /// Fail limit for the first restart
        /// </summary>
        /// <remarks>
        /// Failure-directed search is periodically restarted: explored part of the current search tree is turned into a no-good constraint, and the search starts again in the root node.
        /// This parameter specifies the size of the very first search tree (measured in number of failures).
        ///
        /// The parameter takes an integer value in range <c>1..9223372036854775807</c>.
        ///
        /// The default value is <c>100</c>.
        /// </remarks>
        public long? fdsInitialRestartLimit { get; set; }

        /// <summary>
        /// Restart strategy to use
        /// </summary>
        /// <remarks>
        /// This parameter specifies how the restart limit (maximum number of failures) changes from restart to restart.
        /// Possible values are:
        ///
        /// * <c>Geometric</c> (the default): After each restart, restart limit is multiplied by <see cref="Parameters.fdsRestartGrowthFactor">Parameters.fdsRestartGrowthFactor</see>.
        /// * <c>Nested</c>: Similar to <c>Geometric</c> but the limit is changed back to <see cref="Parameters.fdsInitialRestartLimit">Parameters.fdsInitialRestartLimit</see> each time a new maximum limit is reached.
        /// * <c>Luby</c>: Luby restart strategy is used. Parameter <see cref="Parameters.fdsRestartGrowthFactor">Parameters.fdsRestartGrowthFactor</see> is ignored.
        ///
        /// The default value is <c>Geometric</c>.
        /// </remarks>
        public string? fdsRestartStrategy { get; set; }

        /// <summary>
        /// Growth factor for fail limit after each restart
        /// </summary>
        /// <remarks>
        /// After each restart, the fail limit for the restart is multiplied by the specified factor.
        /// This parameter is ignored when <see cref="Parameters.fdsRestartStrategy">Parameters.fdsRestartStrategy</see> is <c>Luby</c>.
        ///
        /// The parameter takes a floating point value in range <c>1.0..Infinity</c>.
        ///
        /// The default value is <c>1.15</c>.
        /// </remarks>
        public double? fdsRestartGrowthFactor { get; set; }

        /// <summary>
        /// Truncate choice use counts after a restart to this value
        /// </summary>
        /// <remarks>
        /// The idea is that ratings learned in the previous restart are less valid in the new restart.
        /// Using this parameter, it is possible to truncate use counts on choices so that new local ratings will have bigger weights (when FDSFixedAlpha is not used).
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>255</c>.
        /// </remarks>
        public long? fdsMaxCounterAfterRestart { get; set; }

        /// <summary>
        /// Truncate choice use counts after a solution is found
        /// </summary>
        /// <remarks>
        /// Similar to <see cref="Parameters.fdsMaxCounterAfterRestart">Parameters.fdsMaxCounterAfterRestart</see>, this parameter allows truncating use counts on choices when a solution is found.
        ///
        /// The parameter takes an integer value.
        ///
        /// The default value is <c>255</c>.
        /// </remarks>
        public long? fdsMaxCounterAfterSolution { get; set; }

        /// <summary>
        /// Reset restart size after a solution is found (ignored in Luby)
        /// </summary>
        /// <remarks>
        /// When this parameter is set (the default), then restart limit is set back to <see cref="Parameters.fdsInitialRestartLimit">Parameters.fdsInitialRestartLimit</see> when a solution is found.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsResetRestartsAfterSolution { get; set; }

        /// <summary>
        /// Whether to use or not nogood constraints
        /// </summary>
        /// <remarks>
        /// By default, no-good constraint is generated after each restart. This parameter allows to turn no-good constraints off.
        ///
        /// The default value is <c>True</c>.
        /// </remarks>
        public bool? fdsUseNogoods { get; set; }

        /// <summary>
        /// Whether to freeze ratings after a proof is found
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsFreezeRatingsAfterProof { get; set; }

        /// <summary>
        /// Whether to continue after proof
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsContinueAfterProof { get; set; }

        /// <summary>
        /// How many times repeat the proof
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _fdsRepeatLimit { get; set; }

        /// <summary>
        /// Choose completely randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsCompletelyRandom { get; set; }

        /// <summary>
        /// Whether to generate choices for objective expression/variable
        /// </summary>
        /// <remarks>
        /// This option controls the generation of choices on the objective. It works regardless of the objective is given by an expression or a variable.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? fdsBranchOnObjective { get; set; }

        /// <summary>
        /// Whether to improve nogoods from FDS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsImproveNogoods { get; set; }

        /// <summary>
        /// Controls which side of a choice is explored first (considering the rating).
        /// </summary>
        /// <remarks>
        /// This option can take the following values:
        ///
        /// * <c>FailureFirst</c>: Explore the failure side first.
        /// * <c>FailureLast</c>: Explore the failure side last.
        /// * <c>Random</c>: Explore either side randomly.
        ///
        /// The default value is <c>FailureFirst</c>.
        /// </remarks>
        public string? fdsBranchOrdering { get; set; }

        /// <summary>
        /// Use SetTimes when out of choices
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _fdsDiveBySetTimes { get; set; }

        /// <summary>
        /// A strategy to choose objective cuts during FDSDual search.
        /// </summary>
        /// <remarks>
        /// Possible values are:
        ///
        /// * <c>Minimum</c>: Always change the cut by the minimum amount.
        /// * <c>Random</c>: At each restart, randomly choose a value in range LB..UB. The default.
        /// * <c>Split</c>: Always split the current range LB..UB in half.
        ///
        /// The default value is <c>Random</c>.
        /// </remarks>
        public string? fdsDualStrategy { get; set; }

        /// <summary>
        /// Whether to reset ratings when a new LB is proved
        /// </summary>
        /// <remarks>
        /// When this parameter is on, and FDSDual proves a new lower bound, then all ratings are reset to default values.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? fdsDualResetRatings { get; set; }

        /// <summary>
        /// LNS solution pool strategy.
        /// </summary>
        /// <remarks>
        /// Controls how LNS manages its solution pool.
        ///
        /// - <c>Robust</c>: Maintain multiple solution tiers for diverse exploration (the default).
        ///   The solver keeps several solutions of varying quality and occasionally works on
        ///   improving worse solutions. This provides robustness against getting stuck in
        ///   local optima.
        ///
        /// - <c>Focused</c>: Concentrate all LNS effort on improving the best solution.
        ///   Secondary solution tiers are disabled, reducing memory and CPU overhead.
        ///   Recommended for large problems where maintaining multiple solutions is too
        ///   expensive.
        ///
        /// The <c>Large</c> <see cref="Parameters.preset">Parameters.preset</see> automatically selects <c>Focused</c> mode. If you
        /// explicitly set <c>lnsMode</c>, your value takes precedence over the preset.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model ...
        ///
        /// // Default: robust mode with multiple solution tiers
        /// var result = model.Solve();
        ///
        /// // Focused mode for large problems
        /// result = model.Solve(new Parameters { lnsMode = "Focused" });
        ///
        /// // Or use the Large preset, which enables Focused mode automatically
        /// result = model.Solve(new Parameters { preset = "Large" });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration based on problem size.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// </list>
        /// </remarks>
        public string? lnsMode { get; set; }

        /// <summary>
        /// NoOverlap propagation level for LNS initial phase
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsInitNoOverlapPropagationLevel { get; set; }

        /// <summary>
        /// Cumul propagation level for LNS initial phase
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsInitCumulPropagationLevel { get; set; }

        /// <summary>
        /// Initial fail limit in first solution phase (after 0 fails)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsFirstFailLimit { get; set; }

        /// <summary>
        /// How much to increase fail limit during first solution phase
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsFailLimitGrowthFactor { get; set; }

        /// <summary>
        /// Multiply fail limit for LNS steps by this number
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsFailLimitCoefficient { get; set; }

        /// <summary>
        /// Number of all-heuristic iterations after the first solution
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsIterationsAfterFirstSolution { get; set; }

        /// <summary>
        /// Whether to use aggressive SetTimes dominance during LNS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsAggressiveDominance { get; set; }

        /// <summary>
        /// How many times iterate on the current solution (unless new one is found)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsSameSolutionPeriod { get; set; }

        /// <summary>
        /// Number of best solutions that receive most attention
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTier1Size { get; set; }

        /// <summary>
        /// How many worse solutions to remember and sometimes work on
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTier2Size { get; set; }

        /// <summary>
        /// How many other solutions to remember
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTier3Size { get; set; }

        /// <summary>
        /// Probability to work on a solution from tier 2
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsTier2Effort { get; set; }

        /// <summary>
        /// Probability to work on a solution from tier 3
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsTier3Effort { get; set; }

        /// <summary>
        /// Multiplication factor to affect fail limit of every LNS step
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsStepFailLimitFactor { get; set; }

        /// <summary>
        /// How often apply objective cut in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsApplyCutProbability { get; set; }

        /// <summary>
        /// Probability to skip an LNS step when no improvement is possible
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsSkipNoImprovementProbability { get; set; }

        /// <summary>
        /// Maximum size of a model part considered for structure detection
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsSmallStructureLimit { get; set; }

        /// <summary>
        /// Whether to minimize resources in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsResourceOptimization { get; set; }

        /// <summary>
        /// Whether to restore absent interval variables in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsRestoreAbsentIntervals { get; set; }

        /// <summary>
        /// Whether to restore lengths of interval variables in LNS steps
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsRestoreIntervalLengths { get; set; }

        /// <summary>
        /// Always restore values of non-relaxed integer variables
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsRestoreIntVarValues { get; set; }

        /// <summary>
        /// Whether to always relax the objective variable in LNS neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsAlwaysRelaxObjective { get; set; }

        /// <summary>
        /// Use only the user-provided warm start as the initial solution in LNS
        /// </summary>
        /// <remarks>
        /// When this parameter is on, the solver will use only the user-specified warm start solution for the initial solution phase in LNS. If no warm start is provided, the solver will search for its own initial solution as usual.
        ///
        /// The default value is <c>False</c>.
        /// </remarks>
        public bool? lnsUseWarmStartOnly { get; set; }

        /// <summary>
        /// Probability to chose heuristics randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsEpsilon { get; set; }

        /// <summary>
        /// Learning coefficient for heuristics
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsAlpha { get; set; }

        /// <summary>
        /// Temperature for heuristics SoftMax bandits
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsTemperature { get; set; }

        /// <summary>
        /// Whether to choose heuristics uniformly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsHeuristicsUniform { get; set; }

        /// <summary>
        /// Initial Q-values of all heuristics
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsHeuristicsInitialQ { get; set; }

        /// <summary>
        /// Probability to chose portions to relax randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionEpsilon { get; set; }

        /// <summary>
        /// Learning coefficient for portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionAlpha { get; set; }

        /// <summary>
        /// Temperature for portions SoftMax bandits
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionTemperature { get; set; }

        /// <summary>
        /// Whether to choose portions uniformly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsPortionUniform { get; set; }

        /// <summary>
        /// Initial Q-values of portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionInitialQ { get; set; }

        /// <summary>
        /// Limit for portions to relax that are considered big and used less often
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionHandicapLimit { get; set; }

        /// <summary>
        /// Relative probability to use handicapped portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionHandicapValue { get; set; }

        /// <summary>
        /// Initial Q-values of handicapped portions to relax
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionHandicapInitialQ { get; set; }

        /// <summary>
        /// How to choose neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsNeighborhoodStrategy { get; set; }

        /// <summary>
        /// Probability to chose neighborhoods randomly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodEpsilon { get; set; }

        /// <summary>
        /// Learning coefficient for neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodAlpha { get; set; }

        /// <summary>
        /// Temperature for neighborhoods SoftMax bandits
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodTemperature { get; set; }

        /// <summary>
        /// Whether to choose neighborhoods uniformly
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsNeighborhoodUniform { get; set; }

        /// <summary>
        /// Initial Q-values of neighborhoods
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsNeighborhoodInitialQ { get; set; }

        /// <summary>
        /// How many dives before switching to normal LNS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsDivingLimit { get; set; }

        /// <summary>
        /// Diving fail limit compared to final first solution fail limit
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsDivingFailLimitRatio { get; set; }

        /// <summary>
        /// Dump learning data
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsLearningRun { get; set; }

        /// <summary>
        /// Don't accept better solutions (for learning)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsStayOnObjective { get; set; }

        /// <summary>
        /// Use FDS inside LNS
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _lnsFDS { get; set; }

        /// <summary>
        /// Freeze interval variables around the relaxed fragment
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string? _lnsFragmentFreezing { get; set; }

        /// <summary>
        /// How much to relax bounds of relaxed interval variables
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsRelaxSlack { get; set; }

        /// <summary>
        /// Size of the relaxed fragment is multiplied by this factor
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsPortionMultiplier { get; set; }

        /// <summary>
        /// Which worker computes simple lower bound
        /// </summary>
        /// <remarks>
        /// Simple lower bound is a bound such that infeasibility of a better objective can be proved by propagation only (without the search). The given worker computes simple lower bound before it starts the normal search. If a worker with the given number doesn't exist, then the lower bound is not computed.
        ///
        /// The parameter takes an integer value in range <c>-1..2147483647</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? simpleLBWorker { get; set; }

        /// <summary>
        /// Maximum number of feasibility checks
        /// </summary>
        /// <remarks>
        /// Simple lower bound is computed by binary search for the best objective value that is not infeasible by propagation. This parameter limits the maximum number of iterations of the binary search. When the value is 0, then simple lower bound is not computed at all.
        ///
        /// The parameter takes an integer value in range <c>0..2147483647</c>.
        ///
        /// The default value is <c>2147483647</c>.
        /// </remarks>
        public long? simpleLBMaxIterations { get; set; }

        /// <summary>
        /// Number of shaving rounds
        /// </summary>
        /// <remarks>
        /// When non-zero, the solver shaves on variable domains to improve the lower bound. This parameter controls the number of shaving rounds.
        ///
        /// The parameter takes an integer value in range <c>0..2147483647</c>.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public long? simpleLBShavingRounds { get; set; }

        /// <summary>
        /// Level of internal trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _debugTraceLevel { get; set; }

        /// <summary>
        /// Level of memory trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _memoryTraceLevel { get; set; }

        /// <summary>
        /// Addition internal propagation trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _propagationDetailTraceLevel { get; set; }

        /// <summary>
        /// Level of SetTimes trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _setTimesTraceLevel { get; set; }

        /// <summary>
        /// Trace messages between master and workers
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _communicationTraceLevel { get; set; }

        /// <summary>
        /// Level of conversion trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _conversionTraceLevel { get; set; }

        /// <summary>
        /// Trace creation of expressions in the engine
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _expressionBuilderTraceLevel { get; set; }

        /// <summary>
        /// Level of Memorization trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _memorizationTraceLevel { get; set; }

        /// <summary>
        /// Additional internal search trace
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _searchDetailTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for Failure-Directed Search
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _fdsTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for shaving
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _shavingTraceLevel { get; set; }

        /// <summary>
        /// Internal trace that dumps ratings of FDS choices
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _fdsRatingsTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for Large Neighborhood Search
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsTraceLevel { get; set; }

        /// <summary>
        /// Internal trace for heuristic replay
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _heuristicReplayTraceLevel { get; set; }

        /// <summary>
        /// Whether to allow (optimality) proofs by SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _allowSetTimesProofs { get; set; }

        /// <summary>
        /// Always use aggressive dominance rules in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _setTimesAggressiveDominance { get; set; }

        /// <summary>
        /// Coefficient for computation of the number of extends in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _setTimesExtendsCoef { get; set; }

        /// <summary>
        /// Strategy for selecting pulse and step heights in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string? _setTimesHeightStrategy { get; set; }

        /// <summary>
        /// Strategy for selecting itvMapping assignment in SetTimes
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string? _setTimesItvMappingStrategy { get; set; }

        /// <summary>
        /// Initial density for SetTimes (nb of intervals / time unit)
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _setTimesInitDensity { get; set; }

        /// <summary>
        /// For how many iterations to use arithmetic average for density computation
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _setTimesDensityLength { get; set; }

        /// <summary>
        /// After how many iterations density is considered reliable
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _setTimesDensityReliabilityThreshold { get; set; }

        /// <summary>
        /// Bitset of disabled SetTimes heuristics
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _lnsDisabledHeuristics { get; set; }

        /// <summary>
        /// Multiply nbRemainingExtends by this factor
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _setTimesNbExtendsFactor { get; set; }

        /// <summary>
        /// Maximum capacity to use low-capacity timetable algorithm
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _discreteLowCapacityLimit { get; set; }

        /// <summary>
        /// Objective value to start LNS training
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public double? _lnsTrainingObjectiveLimit { get; set; }

        /// <summary>
        /// Whether try to find related of absent intervals
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _posAbsentRelated { get; set; }

        /// <summary>
        /// Default size of CallbackBlock. Must be multiple of 8.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _defaultCallbackBlockSize { get; set; }

        /// <summary>
        /// Whether to peg reservoir during SetTimes search
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _useReservoirPegging { get; set; }

        /// <summary>
        /// Whether to use TimeNet
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool? _useTimeNet { get; set; }

        /// <summary>
        /// Number of variables to process after objective cut before processing TimeNet
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _timeNetVarsToPreprocess { get; set; }

        /// <summary>
        /// Number of last bits in priority for At precedences
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _timeNetSubPriorityBits { get; set; }

        /// <summary>
        /// Minimum chain length to replace precedences with chain constraint
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public long? _minChainLength { get; set; }



        internal Dictionary<string, object?> ToDict()
        {
            var result = new Dictionary<string, object?>();

            if (searchType != null)
            {
                var validValues = new[] { "Auto", "LNS", "FDS", "FDSDual", "SetTimes", "FDSLB" };
                if (!validValues.Contains(searchType, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{searchType}' for parameter SearchType");
                result["searchType"] = searchType;
            }

            if (randomSeed != null)
            {
                result["randomSeed"] = randomSeed;
            }

            if (_workerFailLimit != null)
            {
                result["_workerFailLimit"] = _workerFailLimit;
            }

            if (_workerBranchLimit != null)
            {
                result["_workerBranchLimit"] = _workerBranchLimit;
            }

            if (_workerSolutionLimit != null)
            {
                result["_workerSolutionLimit"] = _workerSolutionLimit;
            }

            if (_workerLNSStepLimit != null)
            {
                result["_workerLNSStepLimit"] = _workerLNSStepLimit;
            }

            if (_workerRestartLimit != null)
            {
                result["_workerRestartLimit"] = _workerRestartLimit;
            }

            if (noOverlapPropagationLevel != null)
            {
                if (noOverlapPropagationLevel < 0 || noOverlapPropagationLevel > 4)
                    throw new ArgumentException($"Value {noOverlapPropagationLevel} for parameter NoOverlapPropagationLevel is out of range [0, 4]");
                result["noOverlapPropagationLevel"] = noOverlapPropagationLevel;
            }

            if (cumulPropagationLevel != null)
            {
                if (cumulPropagationLevel < 0 || cumulPropagationLevel > 4)
                    throw new ArgumentException($"Value {cumulPropagationLevel} for parameter CumulPropagationLevel is out of range [0, 4]");
                result["cumulPropagationLevel"] = cumulPropagationLevel;
            }

            if (reservoirPropagationLevel != null)
            {
                if (reservoirPropagationLevel < 1 || reservoirPropagationLevel > 2)
                    throw new ArgumentException($"Value {reservoirPropagationLevel} for parameter ReservoirPropagationLevel is out of range [1, 2]");
                result["reservoirPropagationLevel"] = reservoirPropagationLevel;
            }

            if (positionPropagationLevel != null)
            {
                if (positionPropagationLevel < 1 || positionPropagationLevel > 3)
                    throw new ArgumentException($"Value {positionPropagationLevel} for parameter PositionPropagationLevel is out of range [1, 3]");
                result["positionPropagationLevel"] = positionPropagationLevel;
            }

            if (integralPropagationLevel != null)
            {
                if (integralPropagationLevel < 1 || integralPropagationLevel > 2)
                    throw new ArgumentException($"Value {integralPropagationLevel} for parameter IntegralPropagationLevel is out of range [1, 2]");
                result["integralPropagationLevel"] = integralPropagationLevel;
            }

            if (_packPropagationLevel != null)
            {
                if (_packPropagationLevel < 1 || _packPropagationLevel > 2)
                    throw new ArgumentException($"Value {_packPropagationLevel} for parameter PackPropagationLevel is out of range [1, 2]");
                result["_packPropagationLevel"] = _packPropagationLevel;
            }

            if (_itvMappingPropagationLevel != null)
            {
                if (_itvMappingPropagationLevel < 1 || _itvMappingPropagationLevel > 2)
                    throw new ArgumentException($"Value {_itvMappingPropagationLevel} for parameter ItvMappingPropagationLevel is out of range [1, 2]");
                result["_itvMappingPropagationLevel"] = _itvMappingPropagationLevel;
            }

            if (searchTraceLevel != null)
            {
                if (searchTraceLevel < 0 || searchTraceLevel > 5)
                    throw new ArgumentException($"Value {searchTraceLevel} for parameter SearchTraceLevel is out of range [0, 5]");
                result["searchTraceLevel"] = searchTraceLevel;
            }

            if (propagationTraceLevel != null)
            {
                if (propagationTraceLevel < 0 || propagationTraceLevel > 5)
                    throw new ArgumentException($"Value {propagationTraceLevel} for parameter PropagationTraceLevel is out of range [0, 5]");
                result["propagationTraceLevel"] = propagationTraceLevel;
            }

            if (fdsInitialRating != null)
            {
                if (fdsInitialRating < 0.000000 || fdsInitialRating > 2.000000)
                    throw new ArgumentException($"Value {fdsInitialRating} for parameter FDSInitialRating is out of range [0.000000, 2.000000]");
                if (double.IsPositiveInfinity(fdsInitialRating.Value))
                    result["fdsInitialRating"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsInitialRating.Value))
                    result["fdsInitialRating"] = "-Infinity";
                else
                    result["fdsInitialRating"] = fdsInitialRating;
            }

            if (fdsReductionWeight != null)
            {
                if (double.IsPositiveInfinity(fdsReductionWeight.Value))
                    result["fdsReductionWeight"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsReductionWeight.Value))
                    result["fdsReductionWeight"] = "-Infinity";
                else
                    result["fdsReductionWeight"] = fdsReductionWeight;
            }

            if (fdsRatingAverageLength != null)
            {
                if (fdsRatingAverageLength < 0 || fdsRatingAverageLength > 254)
                    throw new ArgumentException($"Value {fdsRatingAverageLength} for parameter FDSRatingAverageLength is out of range [0, 254]");
                result["fdsRatingAverageLength"] = fdsRatingAverageLength;
            }

            if (fdsFixedAlpha != null)
            {
                if (fdsFixedAlpha < 0 || fdsFixedAlpha > 1)
                    throw new ArgumentException($"Value {fdsFixedAlpha} for parameter FDSFixedAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(fdsFixedAlpha.Value))
                    result["fdsFixedAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsFixedAlpha.Value))
                    result["fdsFixedAlpha"] = "-Infinity";
                else
                    result["fdsFixedAlpha"] = fdsFixedAlpha;
            }

            if (fdsRatingAverageComparison != null)
            {
                var validValues = new[] { "Off", "Global", "Depth" };
                if (!validValues.Contains(fdsRatingAverageComparison, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsRatingAverageComparison}' for parameter FDSRatingAverageComparison");
                result["fdsRatingAverageComparison"] = fdsRatingAverageComparison;
            }

            if (fdsReductionFactor != null)
            {
                var validValues = new[] { "Normal", "Zero", "Random" };
                if (!validValues.Contains(fdsReductionFactor, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsReductionFactor}' for parameter FDSReductionFactor");
                result["fdsReductionFactor"] = fdsReductionFactor;
            }

            if (fdsReuseClosing != null)
            {
                result["fdsReuseClosing"] = fdsReuseClosing;
            }

            if (fdsUniformChoiceStep != null)
            {
                result["fdsUniformChoiceStep"] = fdsUniformChoiceStep;
            }

            if (fdsLengthStepRatio != null)
            {
                if (double.IsPositiveInfinity(fdsLengthStepRatio.Value))
                    result["fdsLengthStepRatio"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsLengthStepRatio.Value))
                    result["fdsLengthStepRatio"] = "-Infinity";
                else
                    result["fdsLengthStepRatio"] = fdsLengthStepRatio;
            }

            if (fdsMaxInitialChoicesPerVariable != null)
            {
                if (fdsMaxInitialChoicesPerVariable < 2 || fdsMaxInitialChoicesPerVariable > 2147483647)
                    throw new ArgumentException($"Value {fdsMaxInitialChoicesPerVariable} for parameter FDSMaxInitialChoicesPerVariable is out of range [2, 2147483647]");
                result["fdsMaxInitialChoicesPerVariable"] = fdsMaxInitialChoicesPerVariable;
            }

            if (fdsAdditionalStepRatio != null)
            {
                if (double.IsPositiveInfinity(fdsAdditionalStepRatio.Value))
                    result["fdsAdditionalStepRatio"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsAdditionalStepRatio.Value))
                    result["fdsAdditionalStepRatio"] = "-Infinity";
                else
                    result["fdsAdditionalStepRatio"] = fdsAdditionalStepRatio;
            }

            if (fdsPresenceStatusChoices != null)
            {
                result["fdsPresenceStatusChoices"] = fdsPresenceStatusChoices;
            }

            if (fdsMaxInitialLengthChoices != null)
            {
                if (fdsMaxInitialLengthChoices < 0 || fdsMaxInitialLengthChoices > 2147483647)
                    throw new ArgumentException($"Value {fdsMaxInitialLengthChoices} for parameter FDSMaxInitialLengthChoices is out of range [0, 2147483647]");
                result["fdsMaxInitialLengthChoices"] = fdsMaxInitialLengthChoices;
            }

            if (fdsMinLengthChoiceStep != null)
            {
                if (fdsMinLengthChoiceStep < 1 || fdsMinLengthChoiceStep > 1073741823)
                    throw new ArgumentException($"Value {fdsMinLengthChoiceStep} for parameter FDSMinLengthChoiceStep is out of range [1, 1073741823]");
                result["fdsMinLengthChoiceStep"] = fdsMinLengthChoiceStep;
            }

            if (fdsMinIntVarChoiceStep != null)
            {
                if (fdsMinIntVarChoiceStep < 1 || fdsMinIntVarChoiceStep > 1073741823)
                    throw new ArgumentException($"Value {fdsMinIntVarChoiceStep} for parameter FDSMinIntVarChoiceStep is out of range [1, 1073741823]");
                result["fdsMinIntVarChoiceStep"] = fdsMinIntVarChoiceStep;
            }

            if (fdsEventTimeInfluence != null)
            {
                if (fdsEventTimeInfluence < 0 || fdsEventTimeInfluence > 1)
                    throw new ArgumentException($"Value {fdsEventTimeInfluence} for parameter FDSEventTimeInfluence is out of range [0, 1]");
                if (double.IsPositiveInfinity(fdsEventTimeInfluence.Value))
                    result["fdsEventTimeInfluence"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsEventTimeInfluence.Value))
                    result["fdsEventTimeInfluence"] = "-Infinity";
                else
                    result["fdsEventTimeInfluence"] = fdsEventTimeInfluence;
            }

            if (fdsBothFailRewardFactor != null)
            {
                if (fdsBothFailRewardFactor < 0 || fdsBothFailRewardFactor > 1)
                    throw new ArgumentException($"Value {fdsBothFailRewardFactor} for parameter FDSBothFailRewardFactor is out of range [0, 1]");
                if (double.IsPositiveInfinity(fdsBothFailRewardFactor.Value))
                    result["fdsBothFailRewardFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsBothFailRewardFactor.Value))
                    result["fdsBothFailRewardFactor"] = "-Infinity";
                else
                    result["fdsBothFailRewardFactor"] = fdsBothFailRewardFactor;
            }

            if (fdsEpsilon != null)
            {
                if (fdsEpsilon < 0.000000 || fdsEpsilon > 0.999990)
                    throw new ArgumentException($"Value {fdsEpsilon} for parameter FDSEpsilon is out of range [0.000000, 0.999990]");
                if (double.IsPositiveInfinity(fdsEpsilon.Value))
                    result["fdsEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsEpsilon.Value))
                    result["fdsEpsilon"] = "-Infinity";
                else
                    result["fdsEpsilon"] = fdsEpsilon;
            }

            if (fdsStrongBranchingSize != null)
            {
                result["fdsStrongBranchingSize"] = fdsStrongBranchingSize;
            }

            if (fdsStrongBranchingDepth != null)
            {
                result["fdsStrongBranchingDepth"] = fdsStrongBranchingDepth;
            }

            if (fdsStrongBranchingCriterion != null)
            {
                var validValues = new[] { "Both", "Left", "Right" };
                if (!validValues.Contains(fdsStrongBranchingCriterion, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsStrongBranchingCriterion}' for parameter FDSStrongBranchingCriterion");
                result["fdsStrongBranchingCriterion"] = fdsStrongBranchingCriterion;
            }

            if (fdsInitialRestartLimit != null)
            {
                result["fdsInitialRestartLimit"] = fdsInitialRestartLimit;
            }

            if (fdsRestartStrategy != null)
            {
                var validValues = new[] { "Geometric", "Nested", "Luby" };
                if (!validValues.Contains(fdsRestartStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsRestartStrategy}' for parameter FDSRestartStrategy");
                result["fdsRestartStrategy"] = fdsRestartStrategy;
            }

            if (fdsRestartGrowthFactor != null)
            {
                if (double.IsPositiveInfinity(fdsRestartGrowthFactor.Value))
                    result["fdsRestartGrowthFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(fdsRestartGrowthFactor.Value))
                    result["fdsRestartGrowthFactor"] = "-Infinity";
                else
                    result["fdsRestartGrowthFactor"] = fdsRestartGrowthFactor;
            }

            if (fdsMaxCounterAfterRestart != null)
            {
                result["fdsMaxCounterAfterRestart"] = fdsMaxCounterAfterRestart;
            }

            if (fdsMaxCounterAfterSolution != null)
            {
                result["fdsMaxCounterAfterSolution"] = fdsMaxCounterAfterSolution;
            }

            if (fdsResetRestartsAfterSolution != null)
            {
                result["fdsResetRestartsAfterSolution"] = fdsResetRestartsAfterSolution;
            }

            if (fdsUseNogoods != null)
            {
                result["fdsUseNogoods"] = fdsUseNogoods;
            }

            if (_fdsFreezeRatingsAfterProof != null)
            {
                result["_fdsFreezeRatingsAfterProof"] = _fdsFreezeRatingsAfterProof;
            }

            if (_fdsContinueAfterProof != null)
            {
                result["_fdsContinueAfterProof"] = _fdsContinueAfterProof;
            }

            if (_fdsRepeatLimit != null)
            {
                result["_fdsRepeatLimit"] = _fdsRepeatLimit;
            }

            if (_fdsCompletelyRandom != null)
            {
                result["_fdsCompletelyRandom"] = _fdsCompletelyRandom;
            }

            if (fdsBranchOnObjective != null)
            {
                result["fdsBranchOnObjective"] = fdsBranchOnObjective;
            }

            if (_fdsImproveNogoods != null)
            {
                result["_fdsImproveNogoods"] = _fdsImproveNogoods;
            }

            if (fdsBranchOrdering != null)
            {
                var validValues = new[] { "FailureFirst", "FailureLast", "Random" };
                if (!validValues.Contains(fdsBranchOrdering, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsBranchOrdering}' for parameter FDSBranchOrdering");
                result["fdsBranchOrdering"] = fdsBranchOrdering;
            }

            if (_fdsDiveBySetTimes != null)
            {
                result["_fdsDiveBySetTimes"] = _fdsDiveBySetTimes;
            }

            if (fdsDualStrategy != null)
            {
                var validValues = new[] { "Minimum", "Random", "Split" };
                if (!validValues.Contains(fdsDualStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsDualStrategy}' for parameter FDSDualStrategy");
                result["fdsDualStrategy"] = fdsDualStrategy;
            }

            if (fdsDualResetRatings != null)
            {
                result["fdsDualResetRatings"] = fdsDualResetRatings;
            }

            if (lnsMode != null)
            {
                var validValues = new[] { "Robust", "Focused" };
                if (!validValues.Contains(lnsMode, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{lnsMode}' for parameter LNSMode");
                result["lnsMode"] = lnsMode;
            }

            if (_lnsInitNoOverlapPropagationLevel != null)
            {
                if (_lnsInitNoOverlapPropagationLevel < 0 || _lnsInitNoOverlapPropagationLevel > 4)
                    throw new ArgumentException($"Value {_lnsInitNoOverlapPropagationLevel} for parameter LNSInitNoOverlapPropagationLevel is out of range [0, 4]");
                result["_lnsInitNoOverlapPropagationLevel"] = _lnsInitNoOverlapPropagationLevel;
            }

            if (_lnsInitCumulPropagationLevel != null)
            {
                if (_lnsInitCumulPropagationLevel < 0 || _lnsInitCumulPropagationLevel > 4)
                    throw new ArgumentException($"Value {_lnsInitCumulPropagationLevel} for parameter LNSInitCumulPropagationLevel is out of range [0, 4]");
                result["_lnsInitCumulPropagationLevel"] = _lnsInitCumulPropagationLevel;
            }

            if (_lnsFirstFailLimit != null)
            {
                result["_lnsFirstFailLimit"] = _lnsFirstFailLimit;
            }

            if (_lnsFailLimitGrowthFactor != null)
            {
                if (double.IsPositiveInfinity(_lnsFailLimitGrowthFactor.Value))
                    result["_lnsFailLimitGrowthFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsFailLimitGrowthFactor.Value))
                    result["_lnsFailLimitGrowthFactor"] = "-Infinity";
                else
                    result["_lnsFailLimitGrowthFactor"] = _lnsFailLimitGrowthFactor;
            }

            if (_lnsFailLimitCoefficient != null)
            {
                if (double.IsPositiveInfinity(_lnsFailLimitCoefficient.Value))
                    result["_lnsFailLimitCoefficient"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsFailLimitCoefficient.Value))
                    result["_lnsFailLimitCoefficient"] = "-Infinity";
                else
                    result["_lnsFailLimitCoefficient"] = _lnsFailLimitCoefficient;
            }

            if (_lnsIterationsAfterFirstSolution != null)
            {
                if (_lnsIterationsAfterFirstSolution < -1 || _lnsIterationsAfterFirstSolution > 2147483647)
                    throw new ArgumentException($"Value {_lnsIterationsAfterFirstSolution} for parameter LNSIterationsAfterFirstSolution is out of range [-1, 2147483647]");
                result["_lnsIterationsAfterFirstSolution"] = _lnsIterationsAfterFirstSolution;
            }

            if (_lnsAggressiveDominance != null)
            {
                result["_lnsAggressiveDominance"] = _lnsAggressiveDominance;
            }

            if (_lnsSameSolutionPeriod != null)
            {
                if (_lnsSameSolutionPeriod < 1 || _lnsSameSolutionPeriod > 2147483647)
                    throw new ArgumentException($"Value {_lnsSameSolutionPeriod} for parameter LNSSameSolutionPeriod is out of range [1, 2147483647]");
                result["_lnsSameSolutionPeriod"] = _lnsSameSolutionPeriod;
            }

            if (_lnsTier1Size != null)
            {
                if (_lnsTier1Size < 1 || _lnsTier1Size > 1000)
                    throw new ArgumentException($"Value {_lnsTier1Size} for parameter LNSTier1Size is out of range [1, 1000]");
                result["_lnsTier1Size"] = _lnsTier1Size;
            }

            if (_lnsTier2Size != null)
            {
                if (_lnsTier2Size < 0 || _lnsTier2Size > 1000)
                    throw new ArgumentException($"Value {_lnsTier2Size} for parameter LNSTier2Size is out of range [0, 1000]");
                result["_lnsTier2Size"] = _lnsTier2Size;
            }

            if (_lnsTier3Size != null)
            {
                if (_lnsTier3Size < 0 || _lnsTier3Size > 1000)
                    throw new ArgumentException($"Value {_lnsTier3Size} for parameter LNSTier3Size is out of range [0, 1000]");
                result["_lnsTier3Size"] = _lnsTier3Size;
            }

            if (_lnsTier2Effort != null)
            {
                if (_lnsTier2Effort < 0.000000 || _lnsTier2Effort > 1.000000)
                    throw new ArgumentException($"Value {_lnsTier2Effort} for parameter LNSTier2Effort is out of range [0.000000, 1.000000]");
                if (double.IsPositiveInfinity(_lnsTier2Effort.Value))
                    result["_lnsTier2Effort"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsTier2Effort.Value))
                    result["_lnsTier2Effort"] = "-Infinity";
                else
                    result["_lnsTier2Effort"] = _lnsTier2Effort;
            }

            if (_lnsTier3Effort != null)
            {
                if (_lnsTier3Effort < 0.000000 || _lnsTier3Effort > 1.000000)
                    throw new ArgumentException($"Value {_lnsTier3Effort} for parameter LNSTier3Effort is out of range [0.000000, 1.000000]");
                if (double.IsPositiveInfinity(_lnsTier3Effort.Value))
                    result["_lnsTier3Effort"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsTier3Effort.Value))
                    result["_lnsTier3Effort"] = "-Infinity";
                else
                    result["_lnsTier3Effort"] = _lnsTier3Effort;
            }

            if (_lnsStepFailLimitFactor != null)
            {
                if (double.IsPositiveInfinity(_lnsStepFailLimitFactor.Value))
                    result["_lnsStepFailLimitFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsStepFailLimitFactor.Value))
                    result["_lnsStepFailLimitFactor"] = "-Infinity";
                else
                    result["_lnsStepFailLimitFactor"] = _lnsStepFailLimitFactor;
            }

            if (_lnsApplyCutProbability != null)
            {
                if (_lnsApplyCutProbability < 0 || _lnsApplyCutProbability > 1)
                    throw new ArgumentException($"Value {_lnsApplyCutProbability} for parameter LNSApplyCutProbability is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsApplyCutProbability.Value))
                    result["_lnsApplyCutProbability"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsApplyCutProbability.Value))
                    result["_lnsApplyCutProbability"] = "-Infinity";
                else
                    result["_lnsApplyCutProbability"] = _lnsApplyCutProbability;
            }

            if (_lnsSkipNoImprovementProbability != null)
            {
                if (_lnsSkipNoImprovementProbability < 0 || _lnsSkipNoImprovementProbability > 1)
                    throw new ArgumentException($"Value {_lnsSkipNoImprovementProbability} for parameter LNSSkipNoImprovementProbability is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsSkipNoImprovementProbability.Value))
                    result["_lnsSkipNoImprovementProbability"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsSkipNoImprovementProbability.Value))
                    result["_lnsSkipNoImprovementProbability"] = "-Infinity";
                else
                    result["_lnsSkipNoImprovementProbability"] = _lnsSkipNoImprovementProbability;
            }

            if (_lnsSmallStructureLimit != null)
            {
                if (_lnsSmallStructureLimit < 0 || _lnsSmallStructureLimit > 10)
                    throw new ArgumentException($"Value {_lnsSmallStructureLimit} for parameter LNSSmallStructureLimit is out of range [0, 10]");
                result["_lnsSmallStructureLimit"] = _lnsSmallStructureLimit;
            }

            if (_lnsResourceOptimization != null)
            {
                result["_lnsResourceOptimization"] = _lnsResourceOptimization;
            }

            if (_lnsRestoreAbsentIntervals != null)
            {
                result["_lnsRestoreAbsentIntervals"] = _lnsRestoreAbsentIntervals;
            }

            if (_lnsRestoreIntervalLengths != null)
            {
                result["_lnsRestoreIntervalLengths"] = _lnsRestoreIntervalLengths;
            }

            if (_lnsRestoreIntVarValues != null)
            {
                result["_lnsRestoreIntVarValues"] = _lnsRestoreIntVarValues;
            }

            if (_lnsAlwaysRelaxObjective != null)
            {
                result["_lnsAlwaysRelaxObjective"] = _lnsAlwaysRelaxObjective;
            }

            if (lnsUseWarmStartOnly != null)
            {
                result["lnsUseWarmStartOnly"] = lnsUseWarmStartOnly;
            }

            if (_lnsHeuristicsEpsilon != null)
            {
                if (_lnsHeuristicsEpsilon < 0 || _lnsHeuristicsEpsilon > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsEpsilon} for parameter LNSHeuristicsEpsilon is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsEpsilon.Value))
                    result["_lnsHeuristicsEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsEpsilon.Value))
                    result["_lnsHeuristicsEpsilon"] = "-Infinity";
                else
                    result["_lnsHeuristicsEpsilon"] = _lnsHeuristicsEpsilon;
            }

            if (_lnsHeuristicsAlpha != null)
            {
                if (_lnsHeuristicsAlpha < 0 || _lnsHeuristicsAlpha > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsAlpha} for parameter LNSHeuristicsAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsAlpha.Value))
                    result["_lnsHeuristicsAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsAlpha.Value))
                    result["_lnsHeuristicsAlpha"] = "-Infinity";
                else
                    result["_lnsHeuristicsAlpha"] = _lnsHeuristicsAlpha;
            }

            if (_lnsHeuristicsTemperature != null)
            {
                if (_lnsHeuristicsTemperature < -1 || _lnsHeuristicsTemperature > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsTemperature} for parameter LNSHeuristicsTemperature is out of range [-1, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsTemperature.Value))
                    result["_lnsHeuristicsTemperature"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsTemperature.Value))
                    result["_lnsHeuristicsTemperature"] = "-Infinity";
                else
                    result["_lnsHeuristicsTemperature"] = _lnsHeuristicsTemperature;
            }

            if (_lnsHeuristicsUniform != null)
            {
                result["_lnsHeuristicsUniform"] = _lnsHeuristicsUniform;
            }

            if (_lnsHeuristicsInitialQ != null)
            {
                if (_lnsHeuristicsInitialQ < 0 || _lnsHeuristicsInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsHeuristicsInitialQ} for parameter LNSHeuristicsInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsHeuristicsInitialQ.Value))
                    result["_lnsHeuristicsInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsHeuristicsInitialQ.Value))
                    result["_lnsHeuristicsInitialQ"] = "-Infinity";
                else
                    result["_lnsHeuristicsInitialQ"] = _lnsHeuristicsInitialQ;
            }

            if (_lnsPortionEpsilon != null)
            {
                if (_lnsPortionEpsilon < 0 || _lnsPortionEpsilon > 1)
                    throw new ArgumentException($"Value {_lnsPortionEpsilon} for parameter LNSPortionEpsilon is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionEpsilon.Value))
                    result["_lnsPortionEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionEpsilon.Value))
                    result["_lnsPortionEpsilon"] = "-Infinity";
                else
                    result["_lnsPortionEpsilon"] = _lnsPortionEpsilon;
            }

            if (_lnsPortionAlpha != null)
            {
                if (_lnsPortionAlpha < 0 || _lnsPortionAlpha > 1)
                    throw new ArgumentException($"Value {_lnsPortionAlpha} for parameter LNSPortionAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionAlpha.Value))
                    result["_lnsPortionAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionAlpha.Value))
                    result["_lnsPortionAlpha"] = "-Infinity";
                else
                    result["_lnsPortionAlpha"] = _lnsPortionAlpha;
            }

            if (_lnsPortionTemperature != null)
            {
                if (_lnsPortionTemperature < -1 || _lnsPortionTemperature > 1)
                    throw new ArgumentException($"Value {_lnsPortionTemperature} for parameter LNSPortionTemperature is out of range [-1, 1]");
                if (double.IsPositiveInfinity(_lnsPortionTemperature.Value))
                    result["_lnsPortionTemperature"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionTemperature.Value))
                    result["_lnsPortionTemperature"] = "-Infinity";
                else
                    result["_lnsPortionTemperature"] = _lnsPortionTemperature;
            }

            if (_lnsPortionUniform != null)
            {
                result["_lnsPortionUniform"] = _lnsPortionUniform;
            }

            if (_lnsPortionInitialQ != null)
            {
                if (_lnsPortionInitialQ < 0 || _lnsPortionInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsPortionInitialQ} for parameter LNSPortionInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionInitialQ.Value))
                    result["_lnsPortionInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionInitialQ.Value))
                    result["_lnsPortionInitialQ"] = "-Infinity";
                else
                    result["_lnsPortionInitialQ"] = _lnsPortionInitialQ;
            }

            if (_lnsPortionHandicapLimit != null)
            {
                if (_lnsPortionHandicapLimit < 0 || _lnsPortionHandicapLimit > 1)
                    throw new ArgumentException($"Value {_lnsPortionHandicapLimit} for parameter LNSPortionHandicapLimit is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionHandicapLimit.Value))
                    result["_lnsPortionHandicapLimit"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionHandicapLimit.Value))
                    result["_lnsPortionHandicapLimit"] = "-Infinity";
                else
                    result["_lnsPortionHandicapLimit"] = _lnsPortionHandicapLimit;
            }

            if (_lnsPortionHandicapValue != null)
            {
                if (_lnsPortionHandicapValue < 0 || _lnsPortionHandicapValue > 1)
                    throw new ArgumentException($"Value {_lnsPortionHandicapValue} for parameter LNSPortionHandicapValue is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionHandicapValue.Value))
                    result["_lnsPortionHandicapValue"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionHandicapValue.Value))
                    result["_lnsPortionHandicapValue"] = "-Infinity";
                else
                    result["_lnsPortionHandicapValue"] = _lnsPortionHandicapValue;
            }

            if (_lnsPortionHandicapInitialQ != null)
            {
                if (_lnsPortionHandicapInitialQ < 0 || _lnsPortionHandicapInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsPortionHandicapInitialQ} for parameter LNSPortionHandicapInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsPortionHandicapInitialQ.Value))
                    result["_lnsPortionHandicapInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionHandicapInitialQ.Value))
                    result["_lnsPortionHandicapInitialQ"] = "-Infinity";
                else
                    result["_lnsPortionHandicapInitialQ"] = _lnsPortionHandicapInitialQ;
            }

            if (_lnsNeighborhoodStrategy != null)
            {
                if (_lnsNeighborhoodStrategy < 0 || _lnsNeighborhoodStrategy > 2)
                    throw new ArgumentException($"Value {_lnsNeighborhoodStrategy} for parameter LNSNeighborhoodStrategy is out of range [0, 2]");
                result["_lnsNeighborhoodStrategy"] = _lnsNeighborhoodStrategy;
            }

            if (_lnsNeighborhoodEpsilon != null)
            {
                if (_lnsNeighborhoodEpsilon < 0 || _lnsNeighborhoodEpsilon > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodEpsilon} for parameter LNSNeighborhoodEpsilon is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodEpsilon.Value))
                    result["_lnsNeighborhoodEpsilon"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodEpsilon.Value))
                    result["_lnsNeighborhoodEpsilon"] = "-Infinity";
                else
                    result["_lnsNeighborhoodEpsilon"] = _lnsNeighborhoodEpsilon;
            }

            if (_lnsNeighborhoodAlpha != null)
            {
                if (_lnsNeighborhoodAlpha < 0 || _lnsNeighborhoodAlpha > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodAlpha} for parameter LNSNeighborhoodAlpha is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodAlpha.Value))
                    result["_lnsNeighborhoodAlpha"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodAlpha.Value))
                    result["_lnsNeighborhoodAlpha"] = "-Infinity";
                else
                    result["_lnsNeighborhoodAlpha"] = _lnsNeighborhoodAlpha;
            }

            if (_lnsNeighborhoodTemperature != null)
            {
                if (_lnsNeighborhoodTemperature < -1 || _lnsNeighborhoodTemperature > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodTemperature} for parameter LNSNeighborhoodTemperature is out of range [-1, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodTemperature.Value))
                    result["_lnsNeighborhoodTemperature"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodTemperature.Value))
                    result["_lnsNeighborhoodTemperature"] = "-Infinity";
                else
                    result["_lnsNeighborhoodTemperature"] = _lnsNeighborhoodTemperature;
            }

            if (_lnsNeighborhoodUniform != null)
            {
                result["_lnsNeighborhoodUniform"] = _lnsNeighborhoodUniform;
            }

            if (_lnsNeighborhoodInitialQ != null)
            {
                if (_lnsNeighborhoodInitialQ < 0 || _lnsNeighborhoodInitialQ > 1)
                    throw new ArgumentException($"Value {_lnsNeighborhoodInitialQ} for parameter LNSNeighborhoodInitialQ is out of range [0, 1]");
                if (double.IsPositiveInfinity(_lnsNeighborhoodInitialQ.Value))
                    result["_lnsNeighborhoodInitialQ"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsNeighborhoodInitialQ.Value))
                    result["_lnsNeighborhoodInitialQ"] = "-Infinity";
                else
                    result["_lnsNeighborhoodInitialQ"] = _lnsNeighborhoodInitialQ;
            }

            if (_lnsDivingLimit != null)
            {
                result["_lnsDivingLimit"] = _lnsDivingLimit;
            }

            if (_lnsDivingFailLimitRatio != null)
            {
                if (double.IsPositiveInfinity(_lnsDivingFailLimitRatio.Value))
                    result["_lnsDivingFailLimitRatio"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsDivingFailLimitRatio.Value))
                    result["_lnsDivingFailLimitRatio"] = "-Infinity";
                else
                    result["_lnsDivingFailLimitRatio"] = _lnsDivingFailLimitRatio;
            }

            if (_lnsLearningRun != null)
            {
                result["_lnsLearningRun"] = _lnsLearningRun;
            }

            if (_lnsStayOnObjective != null)
            {
                result["_lnsStayOnObjective"] = _lnsStayOnObjective;
            }

            if (_lnsFDS != null)
            {
                result["_lnsFDS"] = _lnsFDS;
            }

            if (_lnsFragmentFreezing != null)
            {
                var validValues = new[] { "Off", "BeforeOnly", "On", "Unsafe" };
                if (!validValues.Contains(_lnsFragmentFreezing, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{_lnsFragmentFreezing}' for parameter LNSFragmentFreezing");
                result["_lnsFragmentFreezing"] = _lnsFragmentFreezing;
            }

            if (_lnsRelaxSlack != null)
            {
                if (_lnsRelaxSlack < 0.000000 || _lnsRelaxSlack > 1.000000)
                    throw new ArgumentException($"Value {_lnsRelaxSlack} for parameter LNSRelaxSlack is out of range [0.000000, 1.000000]");
                if (double.IsPositiveInfinity(_lnsRelaxSlack.Value))
                    result["_lnsRelaxSlack"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsRelaxSlack.Value))
                    result["_lnsRelaxSlack"] = "-Infinity";
                else
                    result["_lnsRelaxSlack"] = _lnsRelaxSlack;
            }

            if (_lnsPortionMultiplier != null)
            {
                if (_lnsPortionMultiplier < 0.000001 || _lnsPortionMultiplier > 10.000000)
                    throw new ArgumentException($"Value {_lnsPortionMultiplier} for parameter LNSPortionMultiplier is out of range [0.000001, 10.000000]");
                if (double.IsPositiveInfinity(_lnsPortionMultiplier.Value))
                    result["_lnsPortionMultiplier"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsPortionMultiplier.Value))
                    result["_lnsPortionMultiplier"] = "-Infinity";
                else
                    result["_lnsPortionMultiplier"] = _lnsPortionMultiplier;
            }

            if (simpleLBWorker != null)
            {
                if (simpleLBWorker < -1 || simpleLBWorker > 2147483647)
                    throw new ArgumentException($"Value {simpleLBWorker} for parameter SimpleLBWorker is out of range [-1, 2147483647]");
                result["simpleLBWorker"] = simpleLBWorker;
            }

            if (simpleLBMaxIterations != null)
            {
                if (simpleLBMaxIterations < 0 || simpleLBMaxIterations > 2147483647)
                    throw new ArgumentException($"Value {simpleLBMaxIterations} for parameter SimpleLBMaxIterations is out of range [0, 2147483647]");
                result["simpleLBMaxIterations"] = simpleLBMaxIterations;
            }

            if (simpleLBShavingRounds != null)
            {
                if (simpleLBShavingRounds < 0 || simpleLBShavingRounds > 2147483647)
                    throw new ArgumentException($"Value {simpleLBShavingRounds} for parameter SimpleLBShavingRounds is out of range [0, 2147483647]");
                result["simpleLBShavingRounds"] = simpleLBShavingRounds;
            }

            if (_debugTraceLevel != null)
            {
                if (_debugTraceLevel < 0 || _debugTraceLevel > 5)
                    throw new ArgumentException($"Value {_debugTraceLevel} for parameter DebugTraceLevel is out of range [0, 5]");
                result["_debugTraceLevel"] = _debugTraceLevel;
            }

            if (_memoryTraceLevel != null)
            {
                if (_memoryTraceLevel < 0 || _memoryTraceLevel > 5)
                    throw new ArgumentException($"Value {_memoryTraceLevel} for parameter MemoryTraceLevel is out of range [0, 5]");
                result["_memoryTraceLevel"] = _memoryTraceLevel;
            }

            if (_propagationDetailTraceLevel != null)
            {
                if (_propagationDetailTraceLevel < 0 || _propagationDetailTraceLevel > 5)
                    throw new ArgumentException($"Value {_propagationDetailTraceLevel} for parameter PropagationDetailTraceLevel is out of range [0, 5]");
                result["_propagationDetailTraceLevel"] = _propagationDetailTraceLevel;
            }

            if (_setTimesTraceLevel != null)
            {
                if (_setTimesTraceLevel < 0 || _setTimesTraceLevel > 5)
                    throw new ArgumentException($"Value {_setTimesTraceLevel} for parameter SetTimesTraceLevel is out of range [0, 5]");
                result["_setTimesTraceLevel"] = _setTimesTraceLevel;
            }

            if (_communicationTraceLevel != null)
            {
                if (_communicationTraceLevel < 0 || _communicationTraceLevel > 5)
                    throw new ArgumentException($"Value {_communicationTraceLevel} for parameter CommunicationTraceLevel is out of range [0, 5]");
                result["_communicationTraceLevel"] = _communicationTraceLevel;
            }

            if (_conversionTraceLevel != null)
            {
                if (_conversionTraceLevel < 0 || _conversionTraceLevel > 5)
                    throw new ArgumentException($"Value {_conversionTraceLevel} for parameter ConversionTraceLevel is out of range [0, 5]");
                result["_conversionTraceLevel"] = _conversionTraceLevel;
            }

            if (_expressionBuilderTraceLevel != null)
            {
                if (_expressionBuilderTraceLevel < 0 || _expressionBuilderTraceLevel > 5)
                    throw new ArgumentException($"Value {_expressionBuilderTraceLevel} for parameter ExpressionBuilderTraceLevel is out of range [0, 5]");
                result["_expressionBuilderTraceLevel"] = _expressionBuilderTraceLevel;
            }

            if (_memorizationTraceLevel != null)
            {
                if (_memorizationTraceLevel < 0 || _memorizationTraceLevel > 5)
                    throw new ArgumentException($"Value {_memorizationTraceLevel} for parameter MemorizationTraceLevel is out of range [0, 5]");
                result["_memorizationTraceLevel"] = _memorizationTraceLevel;
            }

            if (_searchDetailTraceLevel != null)
            {
                if (_searchDetailTraceLevel < 0 || _searchDetailTraceLevel > 5)
                    throw new ArgumentException($"Value {_searchDetailTraceLevel} for parameter SearchDetailTraceLevel is out of range [0, 5]");
                result["_searchDetailTraceLevel"] = _searchDetailTraceLevel;
            }

            if (_fdsTraceLevel != null)
            {
                if (_fdsTraceLevel < 0 || _fdsTraceLevel > 5)
                    throw new ArgumentException($"Value {_fdsTraceLevel} for parameter FDSTraceLevel is out of range [0, 5]");
                result["_fdsTraceLevel"] = _fdsTraceLevel;
            }

            if (_shavingTraceLevel != null)
            {
                if (_shavingTraceLevel < 0 || _shavingTraceLevel > 5)
                    throw new ArgumentException($"Value {_shavingTraceLevel} for parameter ShavingTraceLevel is out of range [0, 5]");
                result["_shavingTraceLevel"] = _shavingTraceLevel;
            }

            if (_fdsRatingsTraceLevel != null)
            {
                if (_fdsRatingsTraceLevel < 0 || _fdsRatingsTraceLevel > 5)
                    throw new ArgumentException($"Value {_fdsRatingsTraceLevel} for parameter FDSRatingsTraceLevel is out of range [0, 5]");
                result["_fdsRatingsTraceLevel"] = _fdsRatingsTraceLevel;
            }

            if (_lnsTraceLevel != null)
            {
                if (_lnsTraceLevel < 0 || _lnsTraceLevel > 5)
                    throw new ArgumentException($"Value {_lnsTraceLevel} for parameter LNSTraceLevel is out of range [0, 5]");
                result["_lnsTraceLevel"] = _lnsTraceLevel;
            }

            if (_heuristicReplayTraceLevel != null)
            {
                if (_heuristicReplayTraceLevel < 0 || _heuristicReplayTraceLevel > 5)
                    throw new ArgumentException($"Value {_heuristicReplayTraceLevel} for parameter HeuristicReplayTraceLevel is out of range [0, 5]");
                result["_heuristicReplayTraceLevel"] = _heuristicReplayTraceLevel;
            }

            if (_allowSetTimesProofs != null)
            {
                result["_allowSetTimesProofs"] = _allowSetTimesProofs;
            }

            if (_setTimesAggressiveDominance != null)
            {
                result["_setTimesAggressiveDominance"] = _setTimesAggressiveDominance;
            }

            if (_setTimesExtendsCoef != null)
            {
                if (double.IsPositiveInfinity(_setTimesExtendsCoef.Value))
                    result["_setTimesExtendsCoef"] = "Infinity";
                else if (double.IsNegativeInfinity(_setTimesExtendsCoef.Value))
                    result["_setTimesExtendsCoef"] = "-Infinity";
                else
                    result["_setTimesExtendsCoef"] = _setTimesExtendsCoef;
            }

            if (_setTimesHeightStrategy != null)
            {
                var validValues = new[] { "FromMax", "FromMin", "Random" };
                if (!validValues.Contains(_setTimesHeightStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{_setTimesHeightStrategy}' for parameter SetTimesHeightStrategy");
                result["_setTimesHeightStrategy"] = _setTimesHeightStrategy;
            }

            if (_setTimesItvMappingStrategy != null)
            {
                var validValues = new[] { "FromMax", "FromMin", "Random" };
                if (!validValues.Contains(_setTimesItvMappingStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{_setTimesItvMappingStrategy}' for parameter SetTimesItvMappingStrategy");
                result["_setTimesItvMappingStrategy"] = _setTimesItvMappingStrategy;
            }

            if (_setTimesInitDensity != null)
            {
                if (double.IsPositiveInfinity(_setTimesInitDensity.Value))
                    result["_setTimesInitDensity"] = "Infinity";
                else if (double.IsNegativeInfinity(_setTimesInitDensity.Value))
                    result["_setTimesInitDensity"] = "-Infinity";
                else
                    result["_setTimesInitDensity"] = _setTimesInitDensity;
            }

            if (_setTimesDensityLength != null)
            {
                result["_setTimesDensityLength"] = _setTimesDensityLength;
            }

            if (_setTimesDensityReliabilityThreshold != null)
            {
                result["_setTimesDensityReliabilityThreshold"] = _setTimesDensityReliabilityThreshold;
            }

            if (_lnsDisabledHeuristics != null)
            {
                result["_lnsDisabledHeuristics"] = _lnsDisabledHeuristics;
            }

            if (_setTimesNbExtendsFactor != null)
            {
                if (double.IsPositiveInfinity(_setTimesNbExtendsFactor.Value))
                    result["_setTimesNbExtendsFactor"] = "Infinity";
                else if (double.IsNegativeInfinity(_setTimesNbExtendsFactor.Value))
                    result["_setTimesNbExtendsFactor"] = "-Infinity";
                else
                    result["_setTimesNbExtendsFactor"] = _setTimesNbExtendsFactor;
            }

            if (_discreteLowCapacityLimit != null)
            {
                if (_discreteLowCapacityLimit < 0 || _discreteLowCapacityLimit > 16)
                    throw new ArgumentException($"Value {_discreteLowCapacityLimit} for parameter DiscreteLowCapacityLimit is out of range [0, 16]");
                result["_discreteLowCapacityLimit"] = _discreteLowCapacityLimit;
            }

            if (_lnsTrainingObjectiveLimit != null)
            {
                if (double.IsPositiveInfinity(_lnsTrainingObjectiveLimit.Value))
                    result["_lnsTrainingObjectiveLimit"] = "Infinity";
                else if (double.IsNegativeInfinity(_lnsTrainingObjectiveLimit.Value))
                    result["_lnsTrainingObjectiveLimit"] = "-Infinity";
                else
                    result["_lnsTrainingObjectiveLimit"] = _lnsTrainingObjectiveLimit;
            }

            if (_posAbsentRelated != null)
            {
                result["_posAbsentRelated"] = _posAbsentRelated;
            }

            if (_defaultCallbackBlockSize != null)
            {
                result["_defaultCallbackBlockSize"] = _defaultCallbackBlockSize;
            }

            if (_useReservoirPegging != null)
            {
                result["_useReservoirPegging"] = _useReservoirPegging;
            }

            if (_useTimeNet != null)
            {
                result["_useTimeNet"] = _useTimeNet;
            }

            if (_timeNetVarsToPreprocess != null)
            {
                result["_timeNetVarsToPreprocess"] = _timeNetVarsToPreprocess;
            }

            if (_timeNetSubPriorityBits != null)
            {
                if (_timeNetSubPriorityBits < 4 || _timeNetSubPriorityBits > 20)
                    throw new ArgumentException($"Value {_timeNetSubPriorityBits} for parameter TimeNetSubPriorityBits is out of range [4, 20]");
                result["_timeNetSubPriorityBits"] = _timeNetSubPriorityBits;
            }

            if (_minChainLength != null)
            {
                result["_minChainLength"] = _minChainLength;
            }



            return result;
        }

        internal static WorkerParameters FromDict(Dictionary<string, object?> data)
        {
            var wp = new WorkerParameters();

            // Parse infinities
            var keys = data.Keys.ToList();
            foreach (var key in keys)
            {
                if (data[key] is string s)
                {
                    if (s == "Infinity")
                        data[key] = double.PositiveInfinity;
                    else if (s == "-Infinity")
                        data[key] = double.NegativeInfinity;
                }
            }

            if (data.TryGetValue("searchType", out var searchTypeVal) && searchTypeVal != null)
            {
                if (searchTypeVal is System.Text.Json.JsonElement searchTypeElem)
                {
                    if (searchTypeElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.searchType = searchTypeElem.GetString();
                }
            }

            if (data.TryGetValue("randomSeed", out var randomSeedVal) && randomSeedVal != null)
            {
                if (randomSeedVal is System.Text.Json.JsonElement randomSeedElem)
                {
                    if (randomSeedElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.randomSeed = randomSeedElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerFailLimit", out var workerFailLimitVal) && workerFailLimitVal != null)
            {
                if (workerFailLimitVal is System.Text.Json.JsonElement workerFailLimitElem)
                {
                    if (workerFailLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._workerFailLimit = workerFailLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerBranchLimit", out var workerBranchLimitVal) && workerBranchLimitVal != null)
            {
                if (workerBranchLimitVal is System.Text.Json.JsonElement workerBranchLimitElem)
                {
                    if (workerBranchLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._workerBranchLimit = workerBranchLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerSolutionLimit", out var workerSolutionLimitVal) && workerSolutionLimitVal != null)
            {
                if (workerSolutionLimitVal is System.Text.Json.JsonElement workerSolutionLimitElem)
                {
                    if (workerSolutionLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._workerSolutionLimit = workerSolutionLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerLNSStepLimit", out var workerLNSStepLimitVal) && workerLNSStepLimitVal != null)
            {
                if (workerLNSStepLimitVal is System.Text.Json.JsonElement workerLNSStepLimitElem)
                {
                    if (workerLNSStepLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._workerLNSStepLimit = workerLNSStepLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_workerRestartLimit", out var workerRestartLimitVal) && workerRestartLimitVal != null)
            {
                if (workerRestartLimitVal is System.Text.Json.JsonElement workerRestartLimitElem)
                {
                    if (workerRestartLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._workerRestartLimit = workerRestartLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("noOverlapPropagationLevel", out var noOverlapPropagationLevelVal) && noOverlapPropagationLevelVal != null)
            {
                if (noOverlapPropagationLevelVal is System.Text.Json.JsonElement noOverlapPropagationLevelElem)
                {
                    if (noOverlapPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.noOverlapPropagationLevel = noOverlapPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("cumulPropagationLevel", out var cumulPropagationLevelVal) && cumulPropagationLevelVal != null)
            {
                if (cumulPropagationLevelVal is System.Text.Json.JsonElement cumulPropagationLevelElem)
                {
                    if (cumulPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.cumulPropagationLevel = cumulPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("reservoirPropagationLevel", out var reservoirPropagationLevelVal) && reservoirPropagationLevelVal != null)
            {
                if (reservoirPropagationLevelVal is System.Text.Json.JsonElement reservoirPropagationLevelElem)
                {
                    if (reservoirPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.reservoirPropagationLevel = reservoirPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("positionPropagationLevel", out var positionPropagationLevelVal) && positionPropagationLevelVal != null)
            {
                if (positionPropagationLevelVal is System.Text.Json.JsonElement positionPropagationLevelElem)
                {
                    if (positionPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.positionPropagationLevel = positionPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("integralPropagationLevel", out var integralPropagationLevelVal) && integralPropagationLevelVal != null)
            {
                if (integralPropagationLevelVal is System.Text.Json.JsonElement integralPropagationLevelElem)
                {
                    if (integralPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.integralPropagationLevel = integralPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_packPropagationLevel", out var packPropagationLevelVal) && packPropagationLevelVal != null)
            {
                if (packPropagationLevelVal is System.Text.Json.JsonElement packPropagationLevelElem)
                {
                    if (packPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._packPropagationLevel = packPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_itvMappingPropagationLevel", out var itvMappingPropagationLevelVal) && itvMappingPropagationLevelVal != null)
            {
                if (itvMappingPropagationLevelVal is System.Text.Json.JsonElement itvMappingPropagationLevelElem)
                {
                    if (itvMappingPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._itvMappingPropagationLevel = itvMappingPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("searchTraceLevel", out var searchTraceLevelVal) && searchTraceLevelVal != null)
            {
                if (searchTraceLevelVal is System.Text.Json.JsonElement searchTraceLevelElem)
                {
                    if (searchTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.searchTraceLevel = searchTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("propagationTraceLevel", out var propagationTraceLevelVal) && propagationTraceLevelVal != null)
            {
                if (propagationTraceLevelVal is System.Text.Json.JsonElement propagationTraceLevelElem)
                {
                    if (propagationTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.propagationTraceLevel = propagationTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsInitialRating", out var fdsInitialRatingVal) && fdsInitialRatingVal != null)
            {
                if (fdsInitialRatingVal is System.Text.Json.JsonElement fdsInitialRatingElem)
                {
                    if (fdsInitialRatingElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsInitialRating = fdsInitialRatingElem.GetDouble();
                    else if (fdsInitialRatingElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsInitialRatingElem.GetString();
                        if (str == "Infinity") wp.fdsInitialRating = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsInitialRating = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsReductionWeight", out var fdsReductionWeightVal) && fdsReductionWeightVal != null)
            {
                if (fdsReductionWeightVal is System.Text.Json.JsonElement fdsReductionWeightElem)
                {
                    if (fdsReductionWeightElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsReductionWeight = fdsReductionWeightElem.GetDouble();
                    else if (fdsReductionWeightElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsReductionWeightElem.GetString();
                        if (str == "Infinity") wp.fdsReductionWeight = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsReductionWeight = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsRatingAverageLength", out var fdsRatingAverageLengthVal) && fdsRatingAverageLengthVal != null)
            {
                if (fdsRatingAverageLengthVal is System.Text.Json.JsonElement fdsRatingAverageLengthElem)
                {
                    if (fdsRatingAverageLengthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsRatingAverageLength = fdsRatingAverageLengthElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsFixedAlpha", out var fdsFixedAlphaVal) && fdsFixedAlphaVal != null)
            {
                if (fdsFixedAlphaVal is System.Text.Json.JsonElement fdsFixedAlphaElem)
                {
                    if (fdsFixedAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsFixedAlpha = fdsFixedAlphaElem.GetDouble();
                    else if (fdsFixedAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsFixedAlphaElem.GetString();
                        if (str == "Infinity") wp.fdsFixedAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsFixedAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsRatingAverageComparison", out var fdsRatingAverageComparisonVal) && fdsRatingAverageComparisonVal != null)
            {
                if (fdsRatingAverageComparisonVal is System.Text.Json.JsonElement fdsRatingAverageComparisonElem)
                {
                    if (fdsRatingAverageComparisonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.fdsRatingAverageComparison = fdsRatingAverageComparisonElem.GetString();
                }
            }

            if (data.TryGetValue("fdsReductionFactor", out var fdsReductionFactorVal) && fdsReductionFactorVal != null)
            {
                if (fdsReductionFactorVal is System.Text.Json.JsonElement fdsReductionFactorElem)
                {
                    if (fdsReductionFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.fdsReductionFactor = fdsReductionFactorElem.GetString();
                }
            }

            if (data.TryGetValue("fdsReuseClosing", out var fdsReuseClosingVal) && fdsReuseClosingVal != null)
            {
                if (fdsReuseClosingVal is System.Text.Json.JsonElement fdsReuseClosingElem)
                {
                    if (fdsReuseClosingElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.fdsReuseClosing = true;
                    else if (fdsReuseClosingElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.fdsReuseClosing = false;
                }
            }

            if (data.TryGetValue("fdsUniformChoiceStep", out var fdsUniformChoiceStepVal) && fdsUniformChoiceStepVal != null)
            {
                if (fdsUniformChoiceStepVal is System.Text.Json.JsonElement fdsUniformChoiceStepElem)
                {
                    if (fdsUniformChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.fdsUniformChoiceStep = true;
                    else if (fdsUniformChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.fdsUniformChoiceStep = false;
                }
            }

            if (data.TryGetValue("fdsLengthStepRatio", out var fdsLengthStepRatioVal) && fdsLengthStepRatioVal != null)
            {
                if (fdsLengthStepRatioVal is System.Text.Json.JsonElement fdsLengthStepRatioElem)
                {
                    if (fdsLengthStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsLengthStepRatio = fdsLengthStepRatioElem.GetDouble();
                    else if (fdsLengthStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsLengthStepRatioElem.GetString();
                        if (str == "Infinity") wp.fdsLengthStepRatio = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsLengthStepRatio = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsMaxInitialChoicesPerVariable", out var fdsMaxInitialChoicesPerVariableVal) && fdsMaxInitialChoicesPerVariableVal != null)
            {
                if (fdsMaxInitialChoicesPerVariableVal is System.Text.Json.JsonElement fdsMaxInitialChoicesPerVariableElem)
                {
                    if (fdsMaxInitialChoicesPerVariableElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsMaxInitialChoicesPerVariable = fdsMaxInitialChoicesPerVariableElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsAdditionalStepRatio", out var fdsAdditionalStepRatioVal) && fdsAdditionalStepRatioVal != null)
            {
                if (fdsAdditionalStepRatioVal is System.Text.Json.JsonElement fdsAdditionalStepRatioElem)
                {
                    if (fdsAdditionalStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsAdditionalStepRatio = fdsAdditionalStepRatioElem.GetDouble();
                    else if (fdsAdditionalStepRatioElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsAdditionalStepRatioElem.GetString();
                        if (str == "Infinity") wp.fdsAdditionalStepRatio = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsAdditionalStepRatio = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsPresenceStatusChoices", out var fdsPresenceStatusChoicesVal) && fdsPresenceStatusChoicesVal != null)
            {
                if (fdsPresenceStatusChoicesVal is System.Text.Json.JsonElement fdsPresenceStatusChoicesElem)
                {
                    if (fdsPresenceStatusChoicesElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.fdsPresenceStatusChoices = true;
                    else if (fdsPresenceStatusChoicesElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.fdsPresenceStatusChoices = false;
                }
            }

            if (data.TryGetValue("fdsMaxInitialLengthChoices", out var fdsMaxInitialLengthChoicesVal) && fdsMaxInitialLengthChoicesVal != null)
            {
                if (fdsMaxInitialLengthChoicesVal is System.Text.Json.JsonElement fdsMaxInitialLengthChoicesElem)
                {
                    if (fdsMaxInitialLengthChoicesElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsMaxInitialLengthChoices = fdsMaxInitialLengthChoicesElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsMinLengthChoiceStep", out var fdsMinLengthChoiceStepVal) && fdsMinLengthChoiceStepVal != null)
            {
                if (fdsMinLengthChoiceStepVal is System.Text.Json.JsonElement fdsMinLengthChoiceStepElem)
                {
                    if (fdsMinLengthChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsMinLengthChoiceStep = fdsMinLengthChoiceStepElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsMinIntVarChoiceStep", out var fdsMinIntVarChoiceStepVal) && fdsMinIntVarChoiceStepVal != null)
            {
                if (fdsMinIntVarChoiceStepVal is System.Text.Json.JsonElement fdsMinIntVarChoiceStepElem)
                {
                    if (fdsMinIntVarChoiceStepElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsMinIntVarChoiceStep = fdsMinIntVarChoiceStepElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsEventTimeInfluence", out var fdsEventTimeInfluenceVal) && fdsEventTimeInfluenceVal != null)
            {
                if (fdsEventTimeInfluenceVal is System.Text.Json.JsonElement fdsEventTimeInfluenceElem)
                {
                    if (fdsEventTimeInfluenceElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsEventTimeInfluence = fdsEventTimeInfluenceElem.GetDouble();
                    else if (fdsEventTimeInfluenceElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsEventTimeInfluenceElem.GetString();
                        if (str == "Infinity") wp.fdsEventTimeInfluence = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsEventTimeInfluence = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsBothFailRewardFactor", out var fdsBothFailRewardFactorVal) && fdsBothFailRewardFactorVal != null)
            {
                if (fdsBothFailRewardFactorVal is System.Text.Json.JsonElement fdsBothFailRewardFactorElem)
                {
                    if (fdsBothFailRewardFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsBothFailRewardFactor = fdsBothFailRewardFactorElem.GetDouble();
                    else if (fdsBothFailRewardFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsBothFailRewardFactorElem.GetString();
                        if (str == "Infinity") wp.fdsBothFailRewardFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsBothFailRewardFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsEpsilon", out var fdsEpsilonVal) && fdsEpsilonVal != null)
            {
                if (fdsEpsilonVal is System.Text.Json.JsonElement fdsEpsilonElem)
                {
                    if (fdsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsEpsilon = fdsEpsilonElem.GetDouble();
                    else if (fdsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsEpsilonElem.GetString();
                        if (str == "Infinity") wp.fdsEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsStrongBranchingSize", out var fdsStrongBranchingSizeVal) && fdsStrongBranchingSizeVal != null)
            {
                if (fdsStrongBranchingSizeVal is System.Text.Json.JsonElement fdsStrongBranchingSizeElem)
                {
                    if (fdsStrongBranchingSizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsStrongBranchingSize = fdsStrongBranchingSizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsStrongBranchingDepth", out var fdsStrongBranchingDepthVal) && fdsStrongBranchingDepthVal != null)
            {
                if (fdsStrongBranchingDepthVal is System.Text.Json.JsonElement fdsStrongBranchingDepthElem)
                {
                    if (fdsStrongBranchingDepthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsStrongBranchingDepth = fdsStrongBranchingDepthElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsStrongBranchingCriterion", out var fdsStrongBranchingCriterionVal) && fdsStrongBranchingCriterionVal != null)
            {
                if (fdsStrongBranchingCriterionVal is System.Text.Json.JsonElement fdsStrongBranchingCriterionElem)
                {
                    if (fdsStrongBranchingCriterionElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.fdsStrongBranchingCriterion = fdsStrongBranchingCriterionElem.GetString();
                }
            }

            if (data.TryGetValue("fdsInitialRestartLimit", out var fdsInitialRestartLimitVal) && fdsInitialRestartLimitVal != null)
            {
                if (fdsInitialRestartLimitVal is System.Text.Json.JsonElement fdsInitialRestartLimitElem)
                {
                    if (fdsInitialRestartLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsInitialRestartLimit = fdsInitialRestartLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsRestartStrategy", out var fdsRestartStrategyVal) && fdsRestartStrategyVal != null)
            {
                if (fdsRestartStrategyVal is System.Text.Json.JsonElement fdsRestartStrategyElem)
                {
                    if (fdsRestartStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.fdsRestartStrategy = fdsRestartStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("fdsRestartGrowthFactor", out var fdsRestartGrowthFactorVal) && fdsRestartGrowthFactorVal != null)
            {
                if (fdsRestartGrowthFactorVal is System.Text.Json.JsonElement fdsRestartGrowthFactorElem)
                {
                    if (fdsRestartGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsRestartGrowthFactor = fdsRestartGrowthFactorElem.GetDouble();
                    else if (fdsRestartGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = fdsRestartGrowthFactorElem.GetString();
                        if (str == "Infinity") wp.fdsRestartGrowthFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") wp.fdsRestartGrowthFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("fdsMaxCounterAfterRestart", out var fdsMaxCounterAfterRestartVal) && fdsMaxCounterAfterRestartVal != null)
            {
                if (fdsMaxCounterAfterRestartVal is System.Text.Json.JsonElement fdsMaxCounterAfterRestartElem)
                {
                    if (fdsMaxCounterAfterRestartElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsMaxCounterAfterRestart = fdsMaxCounterAfterRestartElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsMaxCounterAfterSolution", out var fdsMaxCounterAfterSolutionVal) && fdsMaxCounterAfterSolutionVal != null)
            {
                if (fdsMaxCounterAfterSolutionVal is System.Text.Json.JsonElement fdsMaxCounterAfterSolutionElem)
                {
                    if (fdsMaxCounterAfterSolutionElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.fdsMaxCounterAfterSolution = fdsMaxCounterAfterSolutionElem.GetInt64();
                }
            }

            if (data.TryGetValue("fdsResetRestartsAfterSolution", out var fdsResetRestartsAfterSolutionVal) && fdsResetRestartsAfterSolutionVal != null)
            {
                if (fdsResetRestartsAfterSolutionVal is System.Text.Json.JsonElement fdsResetRestartsAfterSolutionElem)
                {
                    if (fdsResetRestartsAfterSolutionElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.fdsResetRestartsAfterSolution = true;
                    else if (fdsResetRestartsAfterSolutionElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.fdsResetRestartsAfterSolution = false;
                }
            }

            if (data.TryGetValue("fdsUseNogoods", out var fdsUseNogoodsVal) && fdsUseNogoodsVal != null)
            {
                if (fdsUseNogoodsVal is System.Text.Json.JsonElement fdsUseNogoodsElem)
                {
                    if (fdsUseNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.fdsUseNogoods = true;
                    else if (fdsUseNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.fdsUseNogoods = false;
                }
            }

            if (data.TryGetValue("_fdsFreezeRatingsAfterProof", out var fdsFreezeRatingsAfterProofVal) && fdsFreezeRatingsAfterProofVal != null)
            {
                if (fdsFreezeRatingsAfterProofVal is System.Text.Json.JsonElement fdsFreezeRatingsAfterProofElem)
                {
                    if (fdsFreezeRatingsAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._fdsFreezeRatingsAfterProof = true;
                    else if (fdsFreezeRatingsAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._fdsFreezeRatingsAfterProof = false;
                }
            }

            if (data.TryGetValue("_fdsContinueAfterProof", out var fdsContinueAfterProofVal) && fdsContinueAfterProofVal != null)
            {
                if (fdsContinueAfterProofVal is System.Text.Json.JsonElement fdsContinueAfterProofElem)
                {
                    if (fdsContinueAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._fdsContinueAfterProof = true;
                    else if (fdsContinueAfterProofElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._fdsContinueAfterProof = false;
                }
            }

            if (data.TryGetValue("_fdsRepeatLimit", out var fdsRepeatLimitVal) && fdsRepeatLimitVal != null)
            {
                if (fdsRepeatLimitVal is System.Text.Json.JsonElement fdsRepeatLimitElem)
                {
                    if (fdsRepeatLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._fdsRepeatLimit = fdsRepeatLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_fdsCompletelyRandom", out var fdsCompletelyRandomVal) && fdsCompletelyRandomVal != null)
            {
                if (fdsCompletelyRandomVal is System.Text.Json.JsonElement fdsCompletelyRandomElem)
                {
                    if (fdsCompletelyRandomElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._fdsCompletelyRandom = true;
                    else if (fdsCompletelyRandomElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._fdsCompletelyRandom = false;
                }
            }

            if (data.TryGetValue("fdsBranchOnObjective", out var fdsBranchOnObjectiveVal) && fdsBranchOnObjectiveVal != null)
            {
                if (fdsBranchOnObjectiveVal is System.Text.Json.JsonElement fdsBranchOnObjectiveElem)
                {
                    if (fdsBranchOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.fdsBranchOnObjective = true;
                    else if (fdsBranchOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.fdsBranchOnObjective = false;
                }
            }

            if (data.TryGetValue("_fdsImproveNogoods", out var fdsImproveNogoodsVal) && fdsImproveNogoodsVal != null)
            {
                if (fdsImproveNogoodsVal is System.Text.Json.JsonElement fdsImproveNogoodsElem)
                {
                    if (fdsImproveNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._fdsImproveNogoods = true;
                    else if (fdsImproveNogoodsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._fdsImproveNogoods = false;
                }
            }

            if (data.TryGetValue("fdsBranchOrdering", out var fdsBranchOrderingVal) && fdsBranchOrderingVal != null)
            {
                if (fdsBranchOrderingVal is System.Text.Json.JsonElement fdsBranchOrderingElem)
                {
                    if (fdsBranchOrderingElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.fdsBranchOrdering = fdsBranchOrderingElem.GetString();
                }
            }

            if (data.TryGetValue("_fdsDiveBySetTimes", out var fdsDiveBySetTimesVal) && fdsDiveBySetTimesVal != null)
            {
                if (fdsDiveBySetTimesVal is System.Text.Json.JsonElement fdsDiveBySetTimesElem)
                {
                    if (fdsDiveBySetTimesElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._fdsDiveBySetTimes = true;
                    else if (fdsDiveBySetTimesElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._fdsDiveBySetTimes = false;
                }
            }

            if (data.TryGetValue("fdsDualStrategy", out var fdsDualStrategyVal) && fdsDualStrategyVal != null)
            {
                if (fdsDualStrategyVal is System.Text.Json.JsonElement fdsDualStrategyElem)
                {
                    if (fdsDualStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.fdsDualStrategy = fdsDualStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("fdsDualResetRatings", out var fdsDualResetRatingsVal) && fdsDualResetRatingsVal != null)
            {
                if (fdsDualResetRatingsVal is System.Text.Json.JsonElement fdsDualResetRatingsElem)
                {
                    if (fdsDualResetRatingsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.fdsDualResetRatings = true;
                    else if (fdsDualResetRatingsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.fdsDualResetRatings = false;
                }
            }

            if (data.TryGetValue("lnsMode", out var lnsModeVal) && lnsModeVal != null)
            {
                if (lnsModeVal is System.Text.Json.JsonElement lnsModeElem)
                {
                    if (lnsModeElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.lnsMode = lnsModeElem.GetString();
                }
            }

            if (data.TryGetValue("_lnsInitNoOverlapPropagationLevel", out var lnsInitNoOverlapPropagationLevelVal) && lnsInitNoOverlapPropagationLevelVal != null)
            {
                if (lnsInitNoOverlapPropagationLevelVal is System.Text.Json.JsonElement lnsInitNoOverlapPropagationLevelElem)
                {
                    if (lnsInitNoOverlapPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsInitNoOverlapPropagationLevel = lnsInitNoOverlapPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsInitCumulPropagationLevel", out var lnsInitCumulPropagationLevelVal) && lnsInitCumulPropagationLevelVal != null)
            {
                if (lnsInitCumulPropagationLevelVal is System.Text.Json.JsonElement lnsInitCumulPropagationLevelElem)
                {
                    if (lnsInitCumulPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsInitCumulPropagationLevel = lnsInitCumulPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsFirstFailLimit", out var lnsFirstFailLimitVal) && lnsFirstFailLimitVal != null)
            {
                if (lnsFirstFailLimitVal is System.Text.Json.JsonElement lnsFirstFailLimitElem)
                {
                    if (lnsFirstFailLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsFirstFailLimit = lnsFirstFailLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsFailLimitGrowthFactor", out var lnsFailLimitGrowthFactorVal) && lnsFailLimitGrowthFactorVal != null)
            {
                if (lnsFailLimitGrowthFactorVal is System.Text.Json.JsonElement lnsFailLimitGrowthFactorElem)
                {
                    if (lnsFailLimitGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsFailLimitGrowthFactor = lnsFailLimitGrowthFactorElem.GetDouble();
                    else if (lnsFailLimitGrowthFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsFailLimitGrowthFactorElem.GetString();
                        if (str == "Infinity") wp._lnsFailLimitGrowthFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsFailLimitGrowthFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsFailLimitCoefficient", out var lnsFailLimitCoefficientVal) && lnsFailLimitCoefficientVal != null)
            {
                if (lnsFailLimitCoefficientVal is System.Text.Json.JsonElement lnsFailLimitCoefficientElem)
                {
                    if (lnsFailLimitCoefficientElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsFailLimitCoefficient = lnsFailLimitCoefficientElem.GetDouble();
                    else if (lnsFailLimitCoefficientElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsFailLimitCoefficientElem.GetString();
                        if (str == "Infinity") wp._lnsFailLimitCoefficient = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsFailLimitCoefficient = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsIterationsAfterFirstSolution", out var lnsIterationsAfterFirstSolutionVal) && lnsIterationsAfterFirstSolutionVal != null)
            {
                if (lnsIterationsAfterFirstSolutionVal is System.Text.Json.JsonElement lnsIterationsAfterFirstSolutionElem)
                {
                    if (lnsIterationsAfterFirstSolutionElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsIterationsAfterFirstSolution = lnsIterationsAfterFirstSolutionElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsAggressiveDominance", out var lnsAggressiveDominanceVal) && lnsAggressiveDominanceVal != null)
            {
                if (lnsAggressiveDominanceVal is System.Text.Json.JsonElement lnsAggressiveDominanceElem)
                {
                    if (lnsAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsAggressiveDominance = true;
                    else if (lnsAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsAggressiveDominance = false;
                }
            }

            if (data.TryGetValue("_lnsSameSolutionPeriod", out var lnsSameSolutionPeriodVal) && lnsSameSolutionPeriodVal != null)
            {
                if (lnsSameSolutionPeriodVal is System.Text.Json.JsonElement lnsSameSolutionPeriodElem)
                {
                    if (lnsSameSolutionPeriodElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsSameSolutionPeriod = lnsSameSolutionPeriodElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier1Size", out var lnsTier1SizeVal) && lnsTier1SizeVal != null)
            {
                if (lnsTier1SizeVal is System.Text.Json.JsonElement lnsTier1SizeElem)
                {
                    if (lnsTier1SizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsTier1Size = lnsTier1SizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier2Size", out var lnsTier2SizeVal) && lnsTier2SizeVal != null)
            {
                if (lnsTier2SizeVal is System.Text.Json.JsonElement lnsTier2SizeElem)
                {
                    if (lnsTier2SizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsTier2Size = lnsTier2SizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier3Size", out var lnsTier3SizeVal) && lnsTier3SizeVal != null)
            {
                if (lnsTier3SizeVal is System.Text.Json.JsonElement lnsTier3SizeElem)
                {
                    if (lnsTier3SizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsTier3Size = lnsTier3SizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTier2Effort", out var lnsTier2EffortVal) && lnsTier2EffortVal != null)
            {
                if (lnsTier2EffortVal is System.Text.Json.JsonElement lnsTier2EffortElem)
                {
                    if (lnsTier2EffortElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsTier2Effort = lnsTier2EffortElem.GetDouble();
                    else if (lnsTier2EffortElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsTier2EffortElem.GetString();
                        if (str == "Infinity") wp._lnsTier2Effort = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsTier2Effort = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsTier3Effort", out var lnsTier3EffortVal) && lnsTier3EffortVal != null)
            {
                if (lnsTier3EffortVal is System.Text.Json.JsonElement lnsTier3EffortElem)
                {
                    if (lnsTier3EffortElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsTier3Effort = lnsTier3EffortElem.GetDouble();
                    else if (lnsTier3EffortElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsTier3EffortElem.GetString();
                        if (str == "Infinity") wp._lnsTier3Effort = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsTier3Effort = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsStepFailLimitFactor", out var lnsStepFailLimitFactorVal) && lnsStepFailLimitFactorVal != null)
            {
                if (lnsStepFailLimitFactorVal is System.Text.Json.JsonElement lnsStepFailLimitFactorElem)
                {
                    if (lnsStepFailLimitFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsStepFailLimitFactor = lnsStepFailLimitFactorElem.GetDouble();
                    else if (lnsStepFailLimitFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsStepFailLimitFactorElem.GetString();
                        if (str == "Infinity") wp._lnsStepFailLimitFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsStepFailLimitFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsApplyCutProbability", out var lnsApplyCutProbabilityVal) && lnsApplyCutProbabilityVal != null)
            {
                if (lnsApplyCutProbabilityVal is System.Text.Json.JsonElement lnsApplyCutProbabilityElem)
                {
                    if (lnsApplyCutProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsApplyCutProbability = lnsApplyCutProbabilityElem.GetDouble();
                    else if (lnsApplyCutProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsApplyCutProbabilityElem.GetString();
                        if (str == "Infinity") wp._lnsApplyCutProbability = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsApplyCutProbability = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsSkipNoImprovementProbability", out var lnsSkipNoImprovementProbabilityVal) && lnsSkipNoImprovementProbabilityVal != null)
            {
                if (lnsSkipNoImprovementProbabilityVal is System.Text.Json.JsonElement lnsSkipNoImprovementProbabilityElem)
                {
                    if (lnsSkipNoImprovementProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsSkipNoImprovementProbability = lnsSkipNoImprovementProbabilityElem.GetDouble();
                    else if (lnsSkipNoImprovementProbabilityElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsSkipNoImprovementProbabilityElem.GetString();
                        if (str == "Infinity") wp._lnsSkipNoImprovementProbability = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsSkipNoImprovementProbability = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsSmallStructureLimit", out var lnsSmallStructureLimitVal) && lnsSmallStructureLimitVal != null)
            {
                if (lnsSmallStructureLimitVal is System.Text.Json.JsonElement lnsSmallStructureLimitElem)
                {
                    if (lnsSmallStructureLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsSmallStructureLimit = lnsSmallStructureLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsResourceOptimization", out var lnsResourceOptimizationVal) && lnsResourceOptimizationVal != null)
            {
                if (lnsResourceOptimizationVal is System.Text.Json.JsonElement lnsResourceOptimizationElem)
                {
                    if (lnsResourceOptimizationElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsResourceOptimization = true;
                    else if (lnsResourceOptimizationElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsResourceOptimization = false;
                }
            }

            if (data.TryGetValue("_lnsRestoreAbsentIntervals", out var lnsRestoreAbsentIntervalsVal) && lnsRestoreAbsentIntervalsVal != null)
            {
                if (lnsRestoreAbsentIntervalsVal is System.Text.Json.JsonElement lnsRestoreAbsentIntervalsElem)
                {
                    if (lnsRestoreAbsentIntervalsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsRestoreAbsentIntervals = true;
                    else if (lnsRestoreAbsentIntervalsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsRestoreAbsentIntervals = false;
                }
            }

            if (data.TryGetValue("_lnsRestoreIntervalLengths", out var lnsRestoreIntervalLengthsVal) && lnsRestoreIntervalLengthsVal != null)
            {
                if (lnsRestoreIntervalLengthsVal is System.Text.Json.JsonElement lnsRestoreIntervalLengthsElem)
                {
                    if (lnsRestoreIntervalLengthsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsRestoreIntervalLengths = true;
                    else if (lnsRestoreIntervalLengthsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsRestoreIntervalLengths = false;
                }
            }

            if (data.TryGetValue("_lnsRestoreIntVarValues", out var lnsRestoreIntVarValuesVal) && lnsRestoreIntVarValuesVal != null)
            {
                if (lnsRestoreIntVarValuesVal is System.Text.Json.JsonElement lnsRestoreIntVarValuesElem)
                {
                    if (lnsRestoreIntVarValuesElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsRestoreIntVarValues = true;
                    else if (lnsRestoreIntVarValuesElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsRestoreIntVarValues = false;
                }
            }

            if (data.TryGetValue("_lnsAlwaysRelaxObjective", out var lnsAlwaysRelaxObjectiveVal) && lnsAlwaysRelaxObjectiveVal != null)
            {
                if (lnsAlwaysRelaxObjectiveVal is System.Text.Json.JsonElement lnsAlwaysRelaxObjectiveElem)
                {
                    if (lnsAlwaysRelaxObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsAlwaysRelaxObjective = true;
                    else if (lnsAlwaysRelaxObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsAlwaysRelaxObjective = false;
                }
            }

            if (data.TryGetValue("lnsUseWarmStartOnly", out var lnsUseWarmStartOnlyVal) && lnsUseWarmStartOnlyVal != null)
            {
                if (lnsUseWarmStartOnlyVal is System.Text.Json.JsonElement lnsUseWarmStartOnlyElem)
                {
                    if (lnsUseWarmStartOnlyElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp.lnsUseWarmStartOnly = true;
                    else if (lnsUseWarmStartOnlyElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp.lnsUseWarmStartOnly = false;
                }
            }

            if (data.TryGetValue("_lnsHeuristicsEpsilon", out var lnsHeuristicsEpsilonVal) && lnsHeuristicsEpsilonVal != null)
            {
                if (lnsHeuristicsEpsilonVal is System.Text.Json.JsonElement lnsHeuristicsEpsilonElem)
                {
                    if (lnsHeuristicsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsHeuristicsEpsilon = lnsHeuristicsEpsilonElem.GetDouble();
                    else if (lnsHeuristicsEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsEpsilonElem.GetString();
                        if (str == "Infinity") wp._lnsHeuristicsEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsHeuristicsEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsHeuristicsAlpha", out var lnsHeuristicsAlphaVal) && lnsHeuristicsAlphaVal != null)
            {
                if (lnsHeuristicsAlphaVal is System.Text.Json.JsonElement lnsHeuristicsAlphaElem)
                {
                    if (lnsHeuristicsAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsHeuristicsAlpha = lnsHeuristicsAlphaElem.GetDouble();
                    else if (lnsHeuristicsAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsAlphaElem.GetString();
                        if (str == "Infinity") wp._lnsHeuristicsAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsHeuristicsAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsHeuristicsTemperature", out var lnsHeuristicsTemperatureVal) && lnsHeuristicsTemperatureVal != null)
            {
                if (lnsHeuristicsTemperatureVal is System.Text.Json.JsonElement lnsHeuristicsTemperatureElem)
                {
                    if (lnsHeuristicsTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsHeuristicsTemperature = lnsHeuristicsTemperatureElem.GetDouble();
                    else if (lnsHeuristicsTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsTemperatureElem.GetString();
                        if (str == "Infinity") wp._lnsHeuristicsTemperature = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsHeuristicsTemperature = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsHeuristicsUniform", out var lnsHeuristicsUniformVal) && lnsHeuristicsUniformVal != null)
            {
                if (lnsHeuristicsUniformVal is System.Text.Json.JsonElement lnsHeuristicsUniformElem)
                {
                    if (lnsHeuristicsUniformElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsHeuristicsUniform = true;
                    else if (lnsHeuristicsUniformElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsHeuristicsUniform = false;
                }
            }

            if (data.TryGetValue("_lnsHeuristicsInitialQ", out var lnsHeuristicsInitialQVal) && lnsHeuristicsInitialQVal != null)
            {
                if (lnsHeuristicsInitialQVal is System.Text.Json.JsonElement lnsHeuristicsInitialQElem)
                {
                    if (lnsHeuristicsInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsHeuristicsInitialQ = lnsHeuristicsInitialQElem.GetDouble();
                    else if (lnsHeuristicsInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsHeuristicsInitialQElem.GetString();
                        if (str == "Infinity") wp._lnsHeuristicsInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsHeuristicsInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionEpsilon", out var lnsPortionEpsilonVal) && lnsPortionEpsilonVal != null)
            {
                if (lnsPortionEpsilonVal is System.Text.Json.JsonElement lnsPortionEpsilonElem)
                {
                    if (lnsPortionEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionEpsilon = lnsPortionEpsilonElem.GetDouble();
                    else if (lnsPortionEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionEpsilonElem.GetString();
                        if (str == "Infinity") wp._lnsPortionEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionAlpha", out var lnsPortionAlphaVal) && lnsPortionAlphaVal != null)
            {
                if (lnsPortionAlphaVal is System.Text.Json.JsonElement lnsPortionAlphaElem)
                {
                    if (lnsPortionAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionAlpha = lnsPortionAlphaElem.GetDouble();
                    else if (lnsPortionAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionAlphaElem.GetString();
                        if (str == "Infinity") wp._lnsPortionAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionTemperature", out var lnsPortionTemperatureVal) && lnsPortionTemperatureVal != null)
            {
                if (lnsPortionTemperatureVal is System.Text.Json.JsonElement lnsPortionTemperatureElem)
                {
                    if (lnsPortionTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionTemperature = lnsPortionTemperatureElem.GetDouble();
                    else if (lnsPortionTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionTemperatureElem.GetString();
                        if (str == "Infinity") wp._lnsPortionTemperature = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionTemperature = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionUniform", out var lnsPortionUniformVal) && lnsPortionUniformVal != null)
            {
                if (lnsPortionUniformVal is System.Text.Json.JsonElement lnsPortionUniformElem)
                {
                    if (lnsPortionUniformElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsPortionUniform = true;
                    else if (lnsPortionUniformElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsPortionUniform = false;
                }
            }

            if (data.TryGetValue("_lnsPortionInitialQ", out var lnsPortionInitialQVal) && lnsPortionInitialQVal != null)
            {
                if (lnsPortionInitialQVal is System.Text.Json.JsonElement lnsPortionInitialQElem)
                {
                    if (lnsPortionInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionInitialQ = lnsPortionInitialQElem.GetDouble();
                    else if (lnsPortionInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionInitialQElem.GetString();
                        if (str == "Infinity") wp._lnsPortionInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionHandicapLimit", out var lnsPortionHandicapLimitVal) && lnsPortionHandicapLimitVal != null)
            {
                if (lnsPortionHandicapLimitVal is System.Text.Json.JsonElement lnsPortionHandicapLimitElem)
                {
                    if (lnsPortionHandicapLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionHandicapLimit = lnsPortionHandicapLimitElem.GetDouble();
                    else if (lnsPortionHandicapLimitElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionHandicapLimitElem.GetString();
                        if (str == "Infinity") wp._lnsPortionHandicapLimit = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionHandicapLimit = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionHandicapValue", out var lnsPortionHandicapValueVal) && lnsPortionHandicapValueVal != null)
            {
                if (lnsPortionHandicapValueVal is System.Text.Json.JsonElement lnsPortionHandicapValueElem)
                {
                    if (lnsPortionHandicapValueElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionHandicapValue = lnsPortionHandicapValueElem.GetDouble();
                    else if (lnsPortionHandicapValueElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionHandicapValueElem.GetString();
                        if (str == "Infinity") wp._lnsPortionHandicapValue = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionHandicapValue = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionHandicapInitialQ", out var lnsPortionHandicapInitialQVal) && lnsPortionHandicapInitialQVal != null)
            {
                if (lnsPortionHandicapInitialQVal is System.Text.Json.JsonElement lnsPortionHandicapInitialQElem)
                {
                    if (lnsPortionHandicapInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionHandicapInitialQ = lnsPortionHandicapInitialQElem.GetDouble();
                    else if (lnsPortionHandicapInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionHandicapInitialQElem.GetString();
                        if (str == "Infinity") wp._lnsPortionHandicapInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionHandicapInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodStrategy", out var lnsNeighborhoodStrategyVal) && lnsNeighborhoodStrategyVal != null)
            {
                if (lnsNeighborhoodStrategyVal is System.Text.Json.JsonElement lnsNeighborhoodStrategyElem)
                {
                    if (lnsNeighborhoodStrategyElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsNeighborhoodStrategy = lnsNeighborhoodStrategyElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodEpsilon", out var lnsNeighborhoodEpsilonVal) && lnsNeighborhoodEpsilonVal != null)
            {
                if (lnsNeighborhoodEpsilonVal is System.Text.Json.JsonElement lnsNeighborhoodEpsilonElem)
                {
                    if (lnsNeighborhoodEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsNeighborhoodEpsilon = lnsNeighborhoodEpsilonElem.GetDouble();
                    else if (lnsNeighborhoodEpsilonElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodEpsilonElem.GetString();
                        if (str == "Infinity") wp._lnsNeighborhoodEpsilon = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsNeighborhoodEpsilon = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodAlpha", out var lnsNeighborhoodAlphaVal) && lnsNeighborhoodAlphaVal != null)
            {
                if (lnsNeighborhoodAlphaVal is System.Text.Json.JsonElement lnsNeighborhoodAlphaElem)
                {
                    if (lnsNeighborhoodAlphaElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsNeighborhoodAlpha = lnsNeighborhoodAlphaElem.GetDouble();
                    else if (lnsNeighborhoodAlphaElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodAlphaElem.GetString();
                        if (str == "Infinity") wp._lnsNeighborhoodAlpha = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsNeighborhoodAlpha = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodTemperature", out var lnsNeighborhoodTemperatureVal) && lnsNeighborhoodTemperatureVal != null)
            {
                if (lnsNeighborhoodTemperatureVal is System.Text.Json.JsonElement lnsNeighborhoodTemperatureElem)
                {
                    if (lnsNeighborhoodTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsNeighborhoodTemperature = lnsNeighborhoodTemperatureElem.GetDouble();
                    else if (lnsNeighborhoodTemperatureElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodTemperatureElem.GetString();
                        if (str == "Infinity") wp._lnsNeighborhoodTemperature = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsNeighborhoodTemperature = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodUniform", out var lnsNeighborhoodUniformVal) && lnsNeighborhoodUniformVal != null)
            {
                if (lnsNeighborhoodUniformVal is System.Text.Json.JsonElement lnsNeighborhoodUniformElem)
                {
                    if (lnsNeighborhoodUniformElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsNeighborhoodUniform = true;
                    else if (lnsNeighborhoodUniformElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsNeighborhoodUniform = false;
                }
            }

            if (data.TryGetValue("_lnsNeighborhoodInitialQ", out var lnsNeighborhoodInitialQVal) && lnsNeighborhoodInitialQVal != null)
            {
                if (lnsNeighborhoodInitialQVal is System.Text.Json.JsonElement lnsNeighborhoodInitialQElem)
                {
                    if (lnsNeighborhoodInitialQElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsNeighborhoodInitialQ = lnsNeighborhoodInitialQElem.GetDouble();
                    else if (lnsNeighborhoodInitialQElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsNeighborhoodInitialQElem.GetString();
                        if (str == "Infinity") wp._lnsNeighborhoodInitialQ = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsNeighborhoodInitialQ = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsDivingLimit", out var lnsDivingLimitVal) && lnsDivingLimitVal != null)
            {
                if (lnsDivingLimitVal is System.Text.Json.JsonElement lnsDivingLimitElem)
                {
                    if (lnsDivingLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsDivingLimit = lnsDivingLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsDivingFailLimitRatio", out var lnsDivingFailLimitRatioVal) && lnsDivingFailLimitRatioVal != null)
            {
                if (lnsDivingFailLimitRatioVal is System.Text.Json.JsonElement lnsDivingFailLimitRatioElem)
                {
                    if (lnsDivingFailLimitRatioElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsDivingFailLimitRatio = lnsDivingFailLimitRatioElem.GetDouble();
                    else if (lnsDivingFailLimitRatioElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsDivingFailLimitRatioElem.GetString();
                        if (str == "Infinity") wp._lnsDivingFailLimitRatio = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsDivingFailLimitRatio = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsLearningRun", out var lnsLearningRunVal) && lnsLearningRunVal != null)
            {
                if (lnsLearningRunVal is System.Text.Json.JsonElement lnsLearningRunElem)
                {
                    if (lnsLearningRunElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsLearningRun = true;
                    else if (lnsLearningRunElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsLearningRun = false;
                }
            }

            if (data.TryGetValue("_lnsStayOnObjective", out var lnsStayOnObjectiveVal) && lnsStayOnObjectiveVal != null)
            {
                if (lnsStayOnObjectiveVal is System.Text.Json.JsonElement lnsStayOnObjectiveElem)
                {
                    if (lnsStayOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsStayOnObjective = true;
                    else if (lnsStayOnObjectiveElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsStayOnObjective = false;
                }
            }

            if (data.TryGetValue("_lnsFDS", out var lnsFDSVal) && lnsFDSVal != null)
            {
                if (lnsFDSVal is System.Text.Json.JsonElement lnsFDSElem)
                {
                    if (lnsFDSElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._lnsFDS = true;
                    else if (lnsFDSElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._lnsFDS = false;
                }
            }

            if (data.TryGetValue("_lnsFragmentFreezing", out var lnsFragmentFreezingVal) && lnsFragmentFreezingVal != null)
            {
                if (lnsFragmentFreezingVal is System.Text.Json.JsonElement lnsFragmentFreezingElem)
                {
                    if (lnsFragmentFreezingElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp._lnsFragmentFreezing = lnsFragmentFreezingElem.GetString();
                }
            }

            if (data.TryGetValue("_lnsRelaxSlack", out var lnsRelaxSlackVal) && lnsRelaxSlackVal != null)
            {
                if (lnsRelaxSlackVal is System.Text.Json.JsonElement lnsRelaxSlackElem)
                {
                    if (lnsRelaxSlackElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsRelaxSlack = lnsRelaxSlackElem.GetDouble();
                    else if (lnsRelaxSlackElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsRelaxSlackElem.GetString();
                        if (str == "Infinity") wp._lnsRelaxSlack = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsRelaxSlack = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_lnsPortionMultiplier", out var lnsPortionMultiplierVal) && lnsPortionMultiplierVal != null)
            {
                if (lnsPortionMultiplierVal is System.Text.Json.JsonElement lnsPortionMultiplierElem)
                {
                    if (lnsPortionMultiplierElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsPortionMultiplier = lnsPortionMultiplierElem.GetDouble();
                    else if (lnsPortionMultiplierElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsPortionMultiplierElem.GetString();
                        if (str == "Infinity") wp._lnsPortionMultiplier = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsPortionMultiplier = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("simpleLBWorker", out var simpleLBWorkerVal) && simpleLBWorkerVal != null)
            {
                if (simpleLBWorkerVal is System.Text.Json.JsonElement simpleLBWorkerElem)
                {
                    if (simpleLBWorkerElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.simpleLBWorker = simpleLBWorkerElem.GetInt64();
                }
            }

            if (data.TryGetValue("simpleLBMaxIterations", out var simpleLBMaxIterationsVal) && simpleLBMaxIterationsVal != null)
            {
                if (simpleLBMaxIterationsVal is System.Text.Json.JsonElement simpleLBMaxIterationsElem)
                {
                    if (simpleLBMaxIterationsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.simpleLBMaxIterations = simpleLBMaxIterationsElem.GetInt64();
                }
            }

            if (data.TryGetValue("simpleLBShavingRounds", out var simpleLBShavingRoundsVal) && simpleLBShavingRoundsVal != null)
            {
                if (simpleLBShavingRoundsVal is System.Text.Json.JsonElement simpleLBShavingRoundsElem)
                {
                    if (simpleLBShavingRoundsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.simpleLBShavingRounds = simpleLBShavingRoundsElem.GetInt64();
                }
            }

            if (data.TryGetValue("_debugTraceLevel", out var debugTraceLevelVal) && debugTraceLevelVal != null)
            {
                if (debugTraceLevelVal is System.Text.Json.JsonElement debugTraceLevelElem)
                {
                    if (debugTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._debugTraceLevel = debugTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_memoryTraceLevel", out var memoryTraceLevelVal) && memoryTraceLevelVal != null)
            {
                if (memoryTraceLevelVal is System.Text.Json.JsonElement memoryTraceLevelElem)
                {
                    if (memoryTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._memoryTraceLevel = memoryTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_propagationDetailTraceLevel", out var propagationDetailTraceLevelVal) && propagationDetailTraceLevelVal != null)
            {
                if (propagationDetailTraceLevelVal is System.Text.Json.JsonElement propagationDetailTraceLevelElem)
                {
                    if (propagationDetailTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._propagationDetailTraceLevel = propagationDetailTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_setTimesTraceLevel", out var setTimesTraceLevelVal) && setTimesTraceLevelVal != null)
            {
                if (setTimesTraceLevelVal is System.Text.Json.JsonElement setTimesTraceLevelElem)
                {
                    if (setTimesTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._setTimesTraceLevel = setTimesTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_communicationTraceLevel", out var communicationTraceLevelVal) && communicationTraceLevelVal != null)
            {
                if (communicationTraceLevelVal is System.Text.Json.JsonElement communicationTraceLevelElem)
                {
                    if (communicationTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._communicationTraceLevel = communicationTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_conversionTraceLevel", out var conversionTraceLevelVal) && conversionTraceLevelVal != null)
            {
                if (conversionTraceLevelVal is System.Text.Json.JsonElement conversionTraceLevelElem)
                {
                    if (conversionTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._conversionTraceLevel = conversionTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_expressionBuilderTraceLevel", out var expressionBuilderTraceLevelVal) && expressionBuilderTraceLevelVal != null)
            {
                if (expressionBuilderTraceLevelVal is System.Text.Json.JsonElement expressionBuilderTraceLevelElem)
                {
                    if (expressionBuilderTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._expressionBuilderTraceLevel = expressionBuilderTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_memorizationTraceLevel", out var memorizationTraceLevelVal) && memorizationTraceLevelVal != null)
            {
                if (memorizationTraceLevelVal is System.Text.Json.JsonElement memorizationTraceLevelElem)
                {
                    if (memorizationTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._memorizationTraceLevel = memorizationTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_searchDetailTraceLevel", out var searchDetailTraceLevelVal) && searchDetailTraceLevelVal != null)
            {
                if (searchDetailTraceLevelVal is System.Text.Json.JsonElement searchDetailTraceLevelElem)
                {
                    if (searchDetailTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._searchDetailTraceLevel = searchDetailTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_fdsTraceLevel", out var fdsTraceLevelVal) && fdsTraceLevelVal != null)
            {
                if (fdsTraceLevelVal is System.Text.Json.JsonElement fdsTraceLevelElem)
                {
                    if (fdsTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._fdsTraceLevel = fdsTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_shavingTraceLevel", out var shavingTraceLevelVal) && shavingTraceLevelVal != null)
            {
                if (shavingTraceLevelVal is System.Text.Json.JsonElement shavingTraceLevelElem)
                {
                    if (shavingTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._shavingTraceLevel = shavingTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_fdsRatingsTraceLevel", out var fdsRatingsTraceLevelVal) && fdsRatingsTraceLevelVal != null)
            {
                if (fdsRatingsTraceLevelVal is System.Text.Json.JsonElement fdsRatingsTraceLevelElem)
                {
                    if (fdsRatingsTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._fdsRatingsTraceLevel = fdsRatingsTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTraceLevel", out var lnsTraceLevelVal) && lnsTraceLevelVal != null)
            {
                if (lnsTraceLevelVal is System.Text.Json.JsonElement lnsTraceLevelElem)
                {
                    if (lnsTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsTraceLevel = lnsTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_heuristicReplayTraceLevel", out var heuristicReplayTraceLevelVal) && heuristicReplayTraceLevelVal != null)
            {
                if (heuristicReplayTraceLevelVal is System.Text.Json.JsonElement heuristicReplayTraceLevelElem)
                {
                    if (heuristicReplayTraceLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._heuristicReplayTraceLevel = heuristicReplayTraceLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("_allowSetTimesProofs", out var allowSetTimesProofsVal) && allowSetTimesProofsVal != null)
            {
                if (allowSetTimesProofsVal is System.Text.Json.JsonElement allowSetTimesProofsElem)
                {
                    if (allowSetTimesProofsElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._allowSetTimesProofs = true;
                    else if (allowSetTimesProofsElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._allowSetTimesProofs = false;
                }
            }

            if (data.TryGetValue("_setTimesAggressiveDominance", out var setTimesAggressiveDominanceVal) && setTimesAggressiveDominanceVal != null)
            {
                if (setTimesAggressiveDominanceVal is System.Text.Json.JsonElement setTimesAggressiveDominanceElem)
                {
                    if (setTimesAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._setTimesAggressiveDominance = true;
                    else if (setTimesAggressiveDominanceElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._setTimesAggressiveDominance = false;
                }
            }

            if (data.TryGetValue("_setTimesExtendsCoef", out var setTimesExtendsCoefVal) && setTimesExtendsCoefVal != null)
            {
                if (setTimesExtendsCoefVal is System.Text.Json.JsonElement setTimesExtendsCoefElem)
                {
                    if (setTimesExtendsCoefElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._setTimesExtendsCoef = setTimesExtendsCoefElem.GetDouble();
                    else if (setTimesExtendsCoefElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = setTimesExtendsCoefElem.GetString();
                        if (str == "Infinity") wp._setTimesExtendsCoef = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._setTimesExtendsCoef = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_setTimesHeightStrategy", out var setTimesHeightStrategyVal) && setTimesHeightStrategyVal != null)
            {
                if (setTimesHeightStrategyVal is System.Text.Json.JsonElement setTimesHeightStrategyElem)
                {
                    if (setTimesHeightStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp._setTimesHeightStrategy = setTimesHeightStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("_setTimesItvMappingStrategy", out var setTimesItvMappingStrategyVal) && setTimesItvMappingStrategyVal != null)
            {
                if (setTimesItvMappingStrategyVal is System.Text.Json.JsonElement setTimesItvMappingStrategyElem)
                {
                    if (setTimesItvMappingStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp._setTimesItvMappingStrategy = setTimesItvMappingStrategyElem.GetString();
                }
            }

            if (data.TryGetValue("_setTimesInitDensity", out var setTimesInitDensityVal) && setTimesInitDensityVal != null)
            {
                if (setTimesInitDensityVal is System.Text.Json.JsonElement setTimesInitDensityElem)
                {
                    if (setTimesInitDensityElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._setTimesInitDensity = setTimesInitDensityElem.GetDouble();
                    else if (setTimesInitDensityElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = setTimesInitDensityElem.GetString();
                        if (str == "Infinity") wp._setTimesInitDensity = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._setTimesInitDensity = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_setTimesDensityLength", out var setTimesDensityLengthVal) && setTimesDensityLengthVal != null)
            {
                if (setTimesDensityLengthVal is System.Text.Json.JsonElement setTimesDensityLengthElem)
                {
                    if (setTimesDensityLengthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._setTimesDensityLength = setTimesDensityLengthElem.GetInt64();
                }
            }

            if (data.TryGetValue("_setTimesDensityReliabilityThreshold", out var setTimesDensityReliabilityThresholdVal) && setTimesDensityReliabilityThresholdVal != null)
            {
                if (setTimesDensityReliabilityThresholdVal is System.Text.Json.JsonElement setTimesDensityReliabilityThresholdElem)
                {
                    if (setTimesDensityReliabilityThresholdElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._setTimesDensityReliabilityThreshold = setTimesDensityReliabilityThresholdElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsDisabledHeuristics", out var lnsDisabledHeuristicsVal) && lnsDisabledHeuristicsVal != null)
            {
                if (lnsDisabledHeuristicsVal is System.Text.Json.JsonElement lnsDisabledHeuristicsElem)
                {
                    if (lnsDisabledHeuristicsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsDisabledHeuristics = lnsDisabledHeuristicsElem.GetInt64();
                }
            }

            if (data.TryGetValue("_setTimesNbExtendsFactor", out var setTimesNbExtendsFactorVal) && setTimesNbExtendsFactorVal != null)
            {
                if (setTimesNbExtendsFactorVal is System.Text.Json.JsonElement setTimesNbExtendsFactorElem)
                {
                    if (setTimesNbExtendsFactorElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._setTimesNbExtendsFactor = setTimesNbExtendsFactorElem.GetDouble();
                    else if (setTimesNbExtendsFactorElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = setTimesNbExtendsFactorElem.GetString();
                        if (str == "Infinity") wp._setTimesNbExtendsFactor = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._setTimesNbExtendsFactor = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_discreteLowCapacityLimit", out var discreteLowCapacityLimitVal) && discreteLowCapacityLimitVal != null)
            {
                if (discreteLowCapacityLimitVal is System.Text.Json.JsonElement discreteLowCapacityLimitElem)
                {
                    if (discreteLowCapacityLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._discreteLowCapacityLimit = discreteLowCapacityLimitElem.GetInt64();
                }
            }

            if (data.TryGetValue("_lnsTrainingObjectiveLimit", out var lnsTrainingObjectiveLimitVal) && lnsTrainingObjectiveLimitVal != null)
            {
                if (lnsTrainingObjectiveLimitVal is System.Text.Json.JsonElement lnsTrainingObjectiveLimitElem)
                {
                    if (lnsTrainingObjectiveLimitElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._lnsTrainingObjectiveLimit = lnsTrainingObjectiveLimitElem.GetDouble();
                    else if (lnsTrainingObjectiveLimitElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = lnsTrainingObjectiveLimitElem.GetString();
                        if (str == "Infinity") wp._lnsTrainingObjectiveLimit = double.PositiveInfinity;
                        else if (str == "-Infinity") wp._lnsTrainingObjectiveLimit = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("_posAbsentRelated", out var posAbsentRelatedVal) && posAbsentRelatedVal != null)
            {
                if (posAbsentRelatedVal is System.Text.Json.JsonElement posAbsentRelatedElem)
                {
                    if (posAbsentRelatedElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._posAbsentRelated = true;
                    else if (posAbsentRelatedElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._posAbsentRelated = false;
                }
            }

            if (data.TryGetValue("_defaultCallbackBlockSize", out var defaultCallbackBlockSizeVal) && defaultCallbackBlockSizeVal != null)
            {
                if (defaultCallbackBlockSizeVal is System.Text.Json.JsonElement defaultCallbackBlockSizeElem)
                {
                    if (defaultCallbackBlockSizeElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._defaultCallbackBlockSize = defaultCallbackBlockSizeElem.GetInt64();
                }
            }

            if (data.TryGetValue("_useReservoirPegging", out var useReservoirPeggingVal) && useReservoirPeggingVal != null)
            {
                if (useReservoirPeggingVal is System.Text.Json.JsonElement useReservoirPeggingElem)
                {
                    if (useReservoirPeggingElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._useReservoirPegging = true;
                    else if (useReservoirPeggingElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._useReservoirPegging = false;
                }
            }

            if (data.TryGetValue("_useTimeNet", out var useTimeNetVal) && useTimeNetVal != null)
            {
                if (useTimeNetVal is System.Text.Json.JsonElement useTimeNetElem)
                {
                    if (useTimeNetElem.ValueKind == System.Text.Json.JsonValueKind.True)
                        wp._useTimeNet = true;
                    else if (useTimeNetElem.ValueKind == System.Text.Json.JsonValueKind.False)
                        wp._useTimeNet = false;
                }
            }

            if (data.TryGetValue("_timeNetVarsToPreprocess", out var timeNetVarsToPreprocessVal) && timeNetVarsToPreprocessVal != null)
            {
                if (timeNetVarsToPreprocessVal is System.Text.Json.JsonElement timeNetVarsToPreprocessElem)
                {
                    if (timeNetVarsToPreprocessElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._timeNetVarsToPreprocess = timeNetVarsToPreprocessElem.GetInt64();
                }
            }

            if (data.TryGetValue("_timeNetSubPriorityBits", out var timeNetSubPriorityBitsVal) && timeNetSubPriorityBitsVal != null)
            {
                if (timeNetSubPriorityBitsVal is System.Text.Json.JsonElement timeNetSubPriorityBitsElem)
                {
                    if (timeNetSubPriorityBitsElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._timeNetSubPriorityBits = timeNetSubPriorityBitsElem.GetInt64();
                }
            }

            if (data.TryGetValue("_minChainLength", out var minChainLengthVal) && minChainLengthVal != null)
            {
                if (minChainLengthVal is System.Text.Json.JsonElement minChainLengthElem)
                {
                    if (minChainLengthElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp._minChainLength = minChainLengthElem.GetInt64();
                }
            }



            return wp;
        }
    }
}
