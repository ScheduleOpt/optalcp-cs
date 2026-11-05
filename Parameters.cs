// ===========================================================================
// OptalCP C# API - Parameters
// Version: 2026.11.0
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
        /// Preset configuration for solver parameters
        /// </summary>
        /// <remarks>
        /// Presets provide reasonable default values for multiple solver parameters at once. Instead of manually tuning individual parameters, you can select a preset that matches your problem characteristics. The solver will then configure search strategies and propagation levels appropriately.
        ///
        /// Available presets:
        ///
        /// - <c>Auto</c>: The solver automatically selects a preset based on problem size (the default). Problems with more than 100,000 variables use <c>Large</c>, otherwise <c>Default</c>.
        ///
        /// - <c>Default</c>: Balanced configuration for most problems. Uses strong propagation and distributes workers across different search strategies: half use LNS, 3/8 use FDS, and the rest use FDSDual. This provides a good mix of exploration and exploitation.
        ///
        /// - <c>Large</c>: Optimized for big problems with more than 100,000 variables. Uses weaker propagation to reduce overhead, all workers use LNS search, and <see cref="Parameters.lnsMode">Parameters.lnsMode</see> is set to <c>Focused</c>. This trades propagation strength for scalability.
        ///
        /// Parameters affected by presets:
        ///
        /// The preset sets default values for the following parameters:
        ///
        /// - <see cref="Parameters.searchType">Parameters.searchType</see>: How workers are distributed across LNS, FDS, and FDSDual
        /// - <see cref="Parameters.lnsMode">Parameters.lnsMode</see>: LNS solution pool strategy (<c>Large</c> preset selects <c>Focused</c>)
        /// - <see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see>: Propagation strength for noOverlap constraints without transition times
        /// - <see cref="Parameters.noOverlapTransitionPropagationLevel">Parameters.noOverlapTransitionPropagationLevel</see>: Propagation strength for noOverlap constraints with transition times
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
        /// The default value is <see langword="false"/>.
        /// </remarks>
        public bool? verifySolutions { get; set; }

        /// <summary>
        /// Whether to verify correctness of external solutions
        /// </summary>
        /// <remarks>
        /// External solutions can be passed to the solver as a warm start via <see cref="Model.Solve">Model.Solve</see>, or using <see cref="Solver.SendSolution">Solver.SendSolution</see> during the search. Normally, all external solutions are checked before they are used. However, the check may be time consuming, especially if too many external solutions are sent simultaneously. This parameter allows to turn the check off.
        ///
        /// The default value is <see langword="true"/>.
        /// </remarks>
        public bool? verifyExternalSolutions { get; set; }

        /// <summary>
        /// Timeout for solver process to exit after finishing
        /// </summary>
        /// <remarks>
        /// After the solver finishes, wait up to this many seconds for the process to exit. If it doesn't exit in time, it is silently killed.
        ///
        /// The parameter takes a floating point value in range <c>0..Infinity</c>.
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
        /// The parameter takes a floating point value in range <c>0..Infinity</c>.
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
        /// Stop the search when the gap is below the tolerance
        /// </summary>
        /// <remarks>
        /// The search is stopped if the absolute difference between the current solution
        /// value and current lower/upper bound is not bigger than the specified value.
        ///
        /// This parameter works together with <see cref="Parameters.relativeGapTolerance">Parameters.relativeGapTolerance</see> as an OR condition: the search stops when *either* the absolute gap or the relative gap is within tolerance.
        ///
        /// The gap tolerance only decides when the search stops. How much better the
        /// next solution must be is set by <see cref="Parameters.absoluteImprovementStep">Parameters.absoluteImprovementStep</see>
        /// and <see cref="Parameters.relativeImprovementStep">Parameters.relativeImprovementStep</see>. The search never asks for a
        /// smaller improvement than the gap tolerance allows to ignore.
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
        /// The gap tolerance only decides when the search stops. How much better the
        /// next solution must be is set by <see cref="Parameters.absoluteImprovementStep">Parameters.absoluteImprovementStep</see>
        /// and <see cref="Parameters.relativeImprovementStep">Parameters.relativeImprovementStep</see>. The search never asks for a
        /// smaller improvement than the gap tolerance allows to ignore.
        ///
        /// The parameter takes a floating point value.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public double? relativeGapTolerance { get; set; }

        /// <summary>
        /// Minimum absolute improvement of the next solution
        /// </summary>
        /// <remarks>
        /// When the search finds a solution, it then looks only for solutions that are
        /// better at least by the specified value. A bigger step skips small improvements
        /// and can reach good solutions faster.
        ///
        /// The step never leads to a false claim of optimality. When the search fails
        /// to find a solution better by the step, it does not claim optimality. Instead
        /// it proves a new lower/upper bound and continues with a smaller step. The
        /// search stops only when the gap is within
        /// <see cref="Parameters.absoluteGapTolerance">Parameters.absoluteGapTolerance</see> or <see cref="Parameters.relativeGapTolerance">Parameters.relativeGapTolerance</see>.
        ///
        /// However, LNS search proves new bounds only rarely. So an LNS worker can miss
        /// improvements smaller than the step. With a time limit, the solve can then end
        /// with a worse solution than with a smaller step.
        ///
        /// This parameter works together with <see cref="Parameters.relativeImprovementStep">Parameters.relativeImprovementStep</see> as an OR condition: the bigger of the two improvements is required.
        /// The step used by the search is never smaller than the gap tolerance.
        ///
        /// For integer objectives, the search always requires an improvement of at least 1.
        ///
        /// The parameter takes a floating point value.
        ///
        /// The default value is <c>0</c>.
        /// </remarks>
        public double? absoluteImprovementStep { get; set; }

        /// <summary>
        /// Minimum relative improvement of the next solution
        /// </summary>
        /// <remarks>
        /// When the search finds a solution, it then looks only for solutions that are
        /// better at least by the specified fraction of the objective value. A bigger
        /// step skips small improvements and can reach good solutions faster.
        ///
        /// The step never leads to a false claim of optimality. When the search fails
        /// to find a solution better by the step, it does not claim optimality. Instead
        /// it proves a new lower/upper bound and continues with a smaller step. The
        /// search stops only when the gap is within
        /// <see cref="Parameters.absoluteGapTolerance">Parameters.absoluteGapTolerance</see> or <see cref="Parameters.relativeGapTolerance">Parameters.relativeGapTolerance</see>.
        ///
        /// However, LNS search proves new bounds only rarely. So an LNS worker can miss
        /// improvements smaller than the step. With a time limit, the solve can then end
        /// with a worse solution than with a smaller step.
        ///
        /// This parameter works together with <see cref="Parameters.absoluteImprovementStep">Parameters.absoluteImprovementStep</see> as an OR condition: the bigger of the two improvements is required.
        /// The step used by the search is never smaller than the gap tolerance.
        ///
        /// The parameter takes a floating point value.
        ///
        /// The default value is <c>0.0001</c>.
        /// </remarks>
        public double? relativeImprovementStep { get; set; }

        /// <summary>
        /// How much to propagate noOverlap constraints without transition times
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for noOverlap constraints without transition times. For noOverlap constraints with transition times, see <see cref="Parameters.noOverlapTransitionPropagationLevel">Parameters.noOverlapTransitionPropagationLevel</see>. Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
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
        /// When set to 0 (the default), the <see cref="Parameters.preset">Parameters.preset</see> chooses the level. The <c>Default</c> preset uses strong propagation. The <c>Large</c> preset uses a lower level to reduce the propagation overhead on big models. The exact levels may change between versions.
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
        /// <item><description><see cref="Parameters.noOverlapTransitionPropagationLevel">Parameters.noOverlapTransitionPropagationLevel</see> — for noOverlap constraints with transition times.</description></item>
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for creating noOverlap constraints.</description></item>
        /// </list>
        /// </remarks>
        public long? noOverlapPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate noOverlap constraints with transition times
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for noOverlap constraints with transition times. For noOverlap constraints without transition times, see <see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see>. Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
        ///
        /// Propagation levels:
        ///
        /// - Level 1: Reasoning on pairs of intervals and their transition times only
        /// - Level 2: Adds detectable precedences algorithm
        /// - Level 3: Adds edge-finding reasoning
        /// - Level 4: Maximum propagation with all available algorithms
        ///
        /// From level 2 on, the algorithms work with each interval extended to the left by the smallest transition time it can have from a preceding interval.
        ///
        /// When the sequence is used in <see cref="IntervalVar.Position">IntervalVar.Position</see>, the propagation of the positions runs on every level.
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the <see cref="Parameters.preset">Parameters.preset</see> chooses the level. The <c>Default</c> preset uses strong propagation. The <c>Large</c> preset uses a lower level to reduce the propagation overhead on big models. The exact levels may change between versions.
        ///
        /// Performance considerations:
        ///
        /// The same trade-offs apply as for <see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see>. Reasoning on pairs cannot detect that three or more intervals do not fit into a time window. When the transition times are small compared to the interval lengths, higher levels often pay off.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model with noOverlap constraints with transition times ...
        ///
        /// // Let the preset decide (default)
        /// var result = model.Solve();
        ///
        /// // Or use maximum propagation
        /// result = model.Solve(new Parameters { noOverlapTransitionPropagationLevel = 4 });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see> — for noOverlap constraints without transition times.</description></item>
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for creating noOverlap constraints.</description></item>
        /// </list>
        /// </remarks>
        public long? noOverlapTransitionPropagationLevel { get; set; }

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
        /// - Level 3: Adds disjunctive reasoning between pulses that are too high to overlap
        /// - Level 4: Maximum propagation. Adds a redundant <see cref="Model.NoOverlap">Model.NoOverlap</see> over the pulses higher than half of the capacity, and max-energy reasoning that limits the length and height of pulses whose length or height is not fixed
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the <see cref="Parameters.preset">Parameters.preset</see> chooses the level. The <c>Default</c> preset uses strong propagation. The <c>Large</c> preset uses a lower level to reduce the propagation overhead on big models. The exact levels may change between versions.
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
        /// result = model.Solve(new Parameters { cumulPropagationLevel = 4 });
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
        /// LNS solution pool strategy.
        /// </summary>
        /// <remarks>
        /// Controls how LNS manages its solution pool.
        ///
        /// - <c>Auto</c>: Let the <see cref="Parameters.preset">Parameters.preset</see> decide (the default). The <c>Default</c>
        ///   preset resolves <c>Auto</c> to <c>Robust</c>; the <c>Large</c> preset resolves it to <c>Focused</c>.
        ///
        /// - <c>Robust</c>: Maintain multiple solution tiers for diverse exploration.
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
        /// explicitly set <c>lnsMode</c> to <c>Robust</c> or <c>Focused</c>, your value takes precedence over
        /// the preset — for example, setting <c>Robust</c> keeps the multiple solution tiers even
        /// under the <c>Large</c> preset.
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
        /// Use only the user-provided warm start as the initial solution in LNS
        /// </summary>
        /// <remarks>
        /// When this parameter is on, the solver will use only the user-specified warm start solution for the initial solution phase in LNS. If no warm start is provided, the solver will search for its own initial solution as usual.
        ///
        /// The default value is <see langword="false"/>.
        /// </remarks>
        public bool? lnsUseWarmStartOnly { get; set; }

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

        // Parameters the typed properties do not offer, by name. The solver reads a
        // string value as on its command line.
        #pragma warning disable CS1591
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Dictionary<string, object>? _extra { get; set; }
        #pragma warning restore CS1591

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

            if (absoluteImprovementStep != null)
            {
                if (double.IsPositiveInfinity(absoluteImprovementStep.Value))
                    result["absoluteImprovementStep"] = "Infinity";
                else if (double.IsNegativeInfinity(absoluteImprovementStep.Value))
                    result["absoluteImprovementStep"] = "-Infinity";
                else
                    result["absoluteImprovementStep"] = absoluteImprovementStep;
            }

            if (relativeImprovementStep != null)
            {
                if (double.IsPositiveInfinity(relativeImprovementStep.Value))
                    result["relativeImprovementStep"] = "Infinity";
                else if (double.IsNegativeInfinity(relativeImprovementStep.Value))
                    result["relativeImprovementStep"] = "-Infinity";
                else
                    result["relativeImprovementStep"] = relativeImprovementStep;
            }

            if (noOverlapPropagationLevel != null)
            {
                if (noOverlapPropagationLevel < 0 || noOverlapPropagationLevel > 4)
                    throw new ArgumentException($"Value {noOverlapPropagationLevel} for parameter NoOverlapPropagationLevel is out of range [0, 4]");
                result["noOverlapPropagationLevel"] = noOverlapPropagationLevel;
            }

            if (noOverlapTransitionPropagationLevel != null)
            {
                if (noOverlapTransitionPropagationLevel < 0 || noOverlapTransitionPropagationLevel > 4)
                    throw new ArgumentException($"Value {noOverlapTransitionPropagationLevel} for parameter NoOverlapTransitionPropagationLevel is out of range [0, 4]");
                result["noOverlapTransitionPropagationLevel"] = noOverlapTransitionPropagationLevel;
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

            if (fdsDualStrategy != null)
            {
                var validValues = new[] { "Minimum", "Random", "Split" };
                if (!validValues.Contains(fdsDualStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsDualStrategy}' for parameter FDSDualStrategy");
                result["fdsDualStrategy"] = fdsDualStrategy;
            }

            if (lnsMode != null)
            {
                var validValues = new[] { "Auto", "Robust", "Focused" };
                if (!validValues.Contains(lnsMode, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{lnsMode}' for parameter LNSMode");
                result["lnsMode"] = lnsMode;
            }

            if (lnsUseWarmStartOnly != null)
            {
                result["lnsUseWarmStartOnly"] = lnsUseWarmStartOnly;
            }

            if (simpleLBWorker != null)
            {
                if (simpleLBWorker < -1 || simpleLBWorker > 2147483647)
                    throw new ArgumentException($"Value {simpleLBWorker} for parameter SimpleLBWorker is out of range [-1, 2147483647]");
                result["simpleLBWorker"] = simpleLBWorker;
            }



            if (_extra != null)
                result["_extra"] = ExtraToDict(_extra);

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

            if (data.TryGetValue("absoluteImprovementStep", out var absoluteImprovementStepVal) && absoluteImprovementStepVal != null)
            {
                if (absoluteImprovementStepVal is System.Text.Json.JsonElement absoluteImprovementStepElem)
                {
                    if (absoluteImprovementStepElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.absoluteImprovementStep = absoluteImprovementStepElem.GetDouble();
                    else if (absoluteImprovementStepElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = absoluteImprovementStepElem.GetString();
                        if (str == "Infinity") parameters.absoluteImprovementStep = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.absoluteImprovementStep = double.NegativeInfinity;
                    }
                }
            }

            if (data.TryGetValue("relativeImprovementStep", out var relativeImprovementStepVal) && relativeImprovementStepVal != null)
            {
                if (relativeImprovementStepVal is System.Text.Json.JsonElement relativeImprovementStepElem)
                {
                    if (relativeImprovementStepElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.relativeImprovementStep = relativeImprovementStepElem.GetDouble();
                    else if (relativeImprovementStepElem.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        var str = relativeImprovementStepElem.GetString();
                        if (str == "Infinity") parameters.relativeImprovementStep = double.PositiveInfinity;
                        else if (str == "-Infinity") parameters.relativeImprovementStep = double.NegativeInfinity;
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

            if (data.TryGetValue("noOverlapTransitionPropagationLevel", out var noOverlapTransitionPropagationLevelVal) && noOverlapTransitionPropagationLevelVal != null)
            {
                if (noOverlapTransitionPropagationLevelVal is System.Text.Json.JsonElement noOverlapTransitionPropagationLevelElem)
                {
                    if (noOverlapTransitionPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.noOverlapTransitionPropagationLevel = noOverlapTransitionPropagationLevelElem.GetInt64();
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

            if (data.TryGetValue("fdsDualStrategy", out var fdsDualStrategyVal) && fdsDualStrategyVal != null)
            {
                if (fdsDualStrategyVal is System.Text.Json.JsonElement fdsDualStrategyElem)
                {
                    if (fdsDualStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        parameters.fdsDualStrategy = fdsDualStrategyElem.GetString();
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

            if (data.TryGetValue("simpleLBWorker", out var simpleLBWorkerVal) && simpleLBWorkerVal != null)
            {
                if (simpleLBWorkerVal is System.Text.Json.JsonElement simpleLBWorkerElem)
                {
                    if (simpleLBWorkerElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        parameters.simpleLBWorker = simpleLBWorkerElem.GetInt64();
                }
            }


            parameters._extra = ExtraFromDict(data);

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
          --color Never|[Auto]|Always      Whether to colorize output to the terminal

        Major options:
          --nbWorkers uint32=0             Number of threads dedicated to search
          --preset [Auto]|Default|Large    Preset configuration for solver parameters
          --searchType [Auto]|LNS|FDS|FDSDual|SetTimes|FDSLB
                                           Type of search to use
          --randomSeed uint32=1            Random seed
          --logLevel uint32=2              Level of the log
          --warningLevel uint32=2          Level of warnings
          --logPeriod double=10            How often to print log messages (in seconds)
          --verifySolutions bool=false     When on, the correctness of solutions is verified
          --verifyExternalSolutions bool=true
                                           Whether to verify correctness of external solutions
          --processExitTimeout double=3    Timeout for solver process to exit after finishing

        Limits:
          --timeLimit double=Infinity      Wall clock limit for execution in seconds
          --solutionLimit uint64=0         Stop the search after the given number of solutions

        Gap Tolerance:
          --absoluteGapTolerance double=0  Stop the search when the gap is below the tolerance
          --relativeGapTolerance double=0  Stop the search when the gap is below the tolerance
          --absoluteImprovementStep double=0
                                           Minimum absolute improvement of the next solution
          --relativeImprovementStep double=0.0001
                                           Minimum relative improvement of the next solution

        Propagation levels:
          --noOverlapPropagationLevel uint32=0
                                           How much to propagate noOverlap constraints without transition times
          --noOverlapTransitionPropagationLevel uint32=0
                                           How much to propagate noOverlap constraints with transition times
          --cumulPropagationLevel uint32=0
                                           How much to propagate constraints on cumul functions
          --reservoirPropagationLevel uint32=1
                                           How much to propagate constraints on cumul functions
          --positionPropagationLevel uint32=2
                                           How much to propagate position expressions on noOverlap constraints
          --integralPropagationLevel uint32=1
                                           How much to propagate integral expression
          --usePrecedenceEnergy uint32=0   Whether to use precedence energy propagation algorithm

        Failure-Directed Search:
          --fdsDualStrategy Minimum|[Random]|Split
                                           A strategy to choose objective cuts during FDSDual search.

        Large Neighborhood Search:
          --lnsMode [Auto]|Robust|Focused  LNS solution pool strategy.
          --lnsUseWarmStartOnly bool=false
                                           Use only the user-provided warm start as the initial solution in LNS

        Simple Lower Bound:
          --simpleLBWorker int32=0         Which worker computes simple lower bound";


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
                Console.WriteLine($"OptalCP C# API {Solver.ApiVersion}");
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

            // Strip one leading '_', as the solver does. So "--_timeLimit" sets the typed
            // property, and "--_x" with an unknown x goes into `_extra` as a string:
            var isHidden = opt.StartsWith("_", StringComparison.Ordinal);
            if (isHidden)
                opt = opt.Substring(1);

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
                case "absoluteimprovementstep":
                {
                    var parsed = ParseDouble(value, "AbsoluteImprovementStep");
                    if (workerRange == null)
                        parameters.absoluteImprovementStep = parsed;
                    else
                        throw new ArgumentException("Parameter 'absoluteImprovementStep' is global and cannot be set per-worker. Use '--absoluteImprovementStep' instead.");
                    return true;
                }
                case "relativeimprovementstep":
                {
                    var parsed = ParseDouble(value, "RelativeImprovementStep");
                    if (workerRange == null)
                        parameters.relativeImprovementStep = parsed;
                    else
                        throw new ArgumentException("Parameter 'relativeImprovementStep' is global and cannot be set per-worker. Use '--relativeImprovementStep' instead.");
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
                case "nooverlaptransitionpropagationlevel":
                {
                    var parsed = ParseLong(value, "NoOverlapTransitionPropagationLevel");
                    if (parsed < 0 || parsed > 4)
                        throw new ArgumentException($"Value {parsed} for parameter NoOverlapTransitionPropagationLevel is out of range [0, 4]");
                    if (workerRange == null)
                        parameters.noOverlapTransitionPropagationLevel = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).noOverlapTransitionPropagationLevel = parsed;
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
                case "lnsmode":
                {
                    var parsed = ParseEnum(value, "LNSMode", new[] { "Auto", "Robust", "Focused" });
                    if (workerRange == null)
                        parameters.lnsMode = parsed;
                    else
                        for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            GetOrCreateWorker(parameters, i).lnsMode = parsed;
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


                default:
                    if (isHidden)
                    {
                        if (workerRange == null)
                            SetExtra(parameters._extra ??= new Dictionary<string, object>(), opt, value);
                        else
                            for (int i = workerRange.Value.Min; i <= workerRange.Value.Max; i++)
                            {
                                var worker = GetOrCreateWorker(parameters, i);
                                SetExtra(worker._extra ??= new Dictionary<string, object>(), opt, value);
                            }
                        return true;
                    }
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
            dst.preset = @params.preset;
            dst.searchType = @params.searchType;
            dst.randomSeed = @params.randomSeed;
            dst.logLevel = @params.logLevel;
            dst.warningLevel = @params.warningLevel;
            dst.logPeriod = @params.logPeriod;
            dst.verifySolutions = @params.verifySolutions;
            dst.verifyExternalSolutions = @params.verifyExternalSolutions;
            dst.processExitTimeout = @params.processExitTimeout;
            dst.timeLimit = @params.timeLimit;
            dst.solutionLimit = @params.solutionLimit;
            dst.absoluteGapTolerance = @params.absoluteGapTolerance;
            dst.relativeGapTolerance = @params.relativeGapTolerance;
            dst.absoluteImprovementStep = @params.absoluteImprovementStep;
            dst.relativeImprovementStep = @params.relativeImprovementStep;
            dst.noOverlapPropagationLevel = @params.noOverlapPropagationLevel;
            dst.noOverlapTransitionPropagationLevel = @params.noOverlapTransitionPropagationLevel;
            dst.cumulPropagationLevel = @params.cumulPropagationLevel;
            dst.reservoirPropagationLevel = @params.reservoirPropagationLevel;
            dst.positionPropagationLevel = @params.positionPropagationLevel;
            dst.integralPropagationLevel = @params.integralPropagationLevel;
            dst.usePrecedenceEnergy = @params.usePrecedenceEnergy;
            dst.searchTraceLevel = @params.searchTraceLevel;
            dst.propagationTraceLevel = @params.propagationTraceLevel;
            dst.infoTraceLevel = @params.infoTraceLevel;
            dst.fdsDualStrategy = @params.fdsDualStrategy;
            dst.lnsMode = @params.lnsMode;
            dst.lnsUseWarmStartOnly = @params.lnsUseWarmStartOnly;
            dst.simpleLBWorker = @params.simpleLBWorker;

            if (@params.workers != null)
            {
                dst.workers = new List<WorkerParameters>();
                foreach (var w in @params.workers)
                {
                    var wc = new WorkerParameters();
                    wc.searchType = w.searchType;
                    wc.randomSeed = w.randomSeed;
                    wc.noOverlapPropagationLevel = w.noOverlapPropagationLevel;
                    wc.noOverlapTransitionPropagationLevel = w.noOverlapTransitionPropagationLevel;
                    wc.cumulPropagationLevel = w.cumulPropagationLevel;
                    wc.reservoirPropagationLevel = w.reservoirPropagationLevel;
                    wc.positionPropagationLevel = w.positionPropagationLevel;
                    wc.integralPropagationLevel = w.integralPropagationLevel;
                    wc.searchTraceLevel = w.searchTraceLevel;
                    wc.propagationTraceLevel = w.propagationTraceLevel;
                    wc.fdsDualStrategy = w.fdsDualStrategy;
                    wc.lnsMode = w.lnsMode;
                    wc.lnsUseWarmStartOnly = w.lnsUseWarmStartOnly;
                    wc.simpleLBWorker = w.simpleLBWorker;

                    wc._extra = w._extra != null ? new Dictionary<string, object>(w._extra) : null;
                    dst.workers.Add(wc);
                }
            }
            dst._extra = @params._extra != null ? new Dictionary<string, object>(@params._extra) : null;
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
            if (overrides.preset != null) dst.preset = overrides.preset;
            if (overrides.searchType != null) dst.searchType = overrides.searchType;
            if (overrides.randomSeed != null) dst.randomSeed = overrides.randomSeed;
            if (overrides.logLevel != null) dst.logLevel = overrides.logLevel;
            if (overrides.warningLevel != null) dst.warningLevel = overrides.warningLevel;
            if (overrides.logPeriod != null) dst.logPeriod = overrides.logPeriod;
            if (overrides.verifySolutions != null) dst.verifySolutions = overrides.verifySolutions;
            if (overrides.verifyExternalSolutions != null) dst.verifyExternalSolutions = overrides.verifyExternalSolutions;
            if (overrides.processExitTimeout != null) dst.processExitTimeout = overrides.processExitTimeout;
            if (overrides.timeLimit != null) dst.timeLimit = overrides.timeLimit;
            if (overrides.solutionLimit != null) dst.solutionLimit = overrides.solutionLimit;
            if (overrides.absoluteGapTolerance != null) dst.absoluteGapTolerance = overrides.absoluteGapTolerance;
            if (overrides.relativeGapTolerance != null) dst.relativeGapTolerance = overrides.relativeGapTolerance;
            if (overrides.absoluteImprovementStep != null) dst.absoluteImprovementStep = overrides.absoluteImprovementStep;
            if (overrides.relativeImprovementStep != null) dst.relativeImprovementStep = overrides.relativeImprovementStep;
            if (overrides.noOverlapPropagationLevel != null) dst.noOverlapPropagationLevel = overrides.noOverlapPropagationLevel;
            if (overrides.noOverlapTransitionPropagationLevel != null) dst.noOverlapTransitionPropagationLevel = overrides.noOverlapTransitionPropagationLevel;
            if (overrides.cumulPropagationLevel != null) dst.cumulPropagationLevel = overrides.cumulPropagationLevel;
            if (overrides.reservoirPropagationLevel != null) dst.reservoirPropagationLevel = overrides.reservoirPropagationLevel;
            if (overrides.positionPropagationLevel != null) dst.positionPropagationLevel = overrides.positionPropagationLevel;
            if (overrides.integralPropagationLevel != null) dst.integralPropagationLevel = overrides.integralPropagationLevel;
            if (overrides.usePrecedenceEnergy != null) dst.usePrecedenceEnergy = overrides.usePrecedenceEnergy;
            if (overrides.searchTraceLevel != null) dst.searchTraceLevel = overrides.searchTraceLevel;
            if (overrides.propagationTraceLevel != null) dst.propagationTraceLevel = overrides.propagationTraceLevel;
            if (overrides.infoTraceLevel != null) dst.infoTraceLevel = overrides.infoTraceLevel;
            if (overrides.fdsDualStrategy != null) dst.fdsDualStrategy = overrides.fdsDualStrategy;
            if (overrides.lnsMode != null) dst.lnsMode = overrides.lnsMode;
            if (overrides.lnsUseWarmStartOnly != null) dst.lnsUseWarmStartOnly = overrides.lnsUseWarmStartOnly;
            if (overrides.simpleLBWorker != null) dst.simpleLBWorker = overrides.simpleLBWorker;

            if (overrides._extra != null) dst._extra = MergeExtra(dst._extra, overrides._extra);
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
                    if (ow.noOverlapPropagationLevel != null) rw.noOverlapPropagationLevel = ow.noOverlapPropagationLevel;
                    if (ow.noOverlapTransitionPropagationLevel != null) rw.noOverlapTransitionPropagationLevel = ow.noOverlapTransitionPropagationLevel;
                    if (ow.cumulPropagationLevel != null) rw.cumulPropagationLevel = ow.cumulPropagationLevel;
                    if (ow.reservoirPropagationLevel != null) rw.reservoirPropagationLevel = ow.reservoirPropagationLevel;
                    if (ow.positionPropagationLevel != null) rw.positionPropagationLevel = ow.positionPropagationLevel;
                    if (ow.integralPropagationLevel != null) rw.integralPropagationLevel = ow.integralPropagationLevel;
                    if (ow.searchTraceLevel != null) rw.searchTraceLevel = ow.searchTraceLevel;
                    if (ow.propagationTraceLevel != null) rw.propagationTraceLevel = ow.propagationTraceLevel;
                    if (ow.fdsDualStrategy != null) rw.fdsDualStrategy = ow.fdsDualStrategy;
                    if (ow.lnsMode != null) rw.lnsMode = ow.lnsMode;
                    if (ow.lnsUseWarmStartOnly != null) rw.lnsUseWarmStartOnly = ow.lnsUseWarmStartOnly;
                    if (ow.simpleLBWorker != null) rw.simpleLBWorker = ow.simpleLBWorker;

                    if (ow._extra != null) rw._extra = MergeExtra(rw._extra, ow._extra);
                }
            }

            return dst;
        }

        // Whether two parameter names name the same parameter. The solver ignores
        // the case and one leading '_'.
        private static bool SameParamName(string a, string b)
        {
            static string Normalize(string name) => name.StartsWith("_", StringComparison.Ordinal) ? name.Substring(1) : name;
            return string.Equals(Normalize(a), Normalize(b), StringComparison.OrdinalIgnoreCase);
        }

        // Set `name` in `extra` to `value`. Remove an entry that names the same
        // parameter in another spelling, or the solver would see the name twice.
        private static void SetExtra(Dictionary<string, object> extra, string name, object value)
        {
            foreach (var key in extra.Keys.Where(key => SameParamName(key, name)).ToList())
                extra.Remove(key);
            extra[name] = value;
        }

        private static Dictionary<string, object> MergeExtra(Dictionary<string, object>? @base, Dictionary<string, object> overrides)
        {
            var result = @base != null ? new Dictionary<string, object>(@base) : new Dictionary<string, object>();
            foreach (var entry in overrides)
                SetExtra(result, entry.Key, entry.Value);
            return result;
        }

        // JSON cannot represent infinity. So we send it as the strings "Infinity"
        // and "-Infinity", which the solver reads as numbers:
        internal static Dictionary<string, object> ExtraToDict(Dictionary<string, object> extra)
        {
            var result = new Dictionary<string, object>();
            foreach (var entry in extra)
                result[entry.Key] = entry.Value switch
                {
                    double.PositiveInfinity => "Infinity",
                    double.NegativeInfinity => "-Infinity",
                    _ => entry.Value
                };
            return result;
        }

        // Read "_extra" of a parameter object parsed from JSON:
        internal static Dictionary<string, object>? ExtraFromDict(Dictionary<string, object?> data)
        {
            if (!data.TryGetValue("_extra", out var extraVal)
                || extraVal is not System.Text.Json.JsonElement extraElem
                || extraElem.ValueKind != System.Text.Json.JsonValueKind.Object)
                return null;
            var result = new Dictionary<string, object>();
            foreach (var prop in extraElem.EnumerateObject())
            {
                var value = prop.Value;
                switch (value.ValueKind)
                {
                    case System.Text.Json.JsonValueKind.String:
                        result[prop.Name] = value.GetString()!;
                        break;
                    case System.Text.Json.JsonValueKind.Number:
                        if (value.TryGetInt64(out var integer))
                            result[prop.Name] = integer;
                        else if (value.TryGetUInt64(out var unsignedInteger))
                            result[prop.Name] = unsignedInteger;
                        else
                            result[prop.Name] = value.GetDouble();
                        break;
                    case System.Text.Json.JsonValueKind.True:
                    case System.Text.Json.JsonValueKind.False:
                        result[prop.Name] = value.GetBoolean();
                        break;
                    default:
                        throw new ArgumentException($"Invalid value of '_extra.{prop.Name}' in parameters.");
                }
            }
            return result;
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
        /// How much to propagate noOverlap constraints without transition times
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for noOverlap constraints without transition times. For noOverlap constraints with transition times, see <see cref="Parameters.noOverlapTransitionPropagationLevel">Parameters.noOverlapTransitionPropagationLevel</see>. Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
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
        /// When set to 0 (the default), the <see cref="Parameters.preset">Parameters.preset</see> chooses the level. The <c>Default</c> preset uses strong propagation. The <c>Large</c> preset uses a lower level to reduce the propagation overhead on big models. The exact levels may change between versions.
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
        /// <item><description><see cref="Parameters.noOverlapTransitionPropagationLevel">Parameters.noOverlapTransitionPropagationLevel</see> — for noOverlap constraints with transition times.</description></item>
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Parameters.searchType">Parameters.searchType</see> — for choosing the search algorithm.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for creating noOverlap constraints.</description></item>
        /// </list>
        /// </remarks>
        public long? noOverlapPropagationLevel { get; set; }

        /// <summary>
        /// How much to propagate noOverlap constraints with transition times
        /// </summary>
        /// <remarks>
        /// This parameter controls the amount of propagation done for noOverlap constraints with transition times. For noOverlap constraints without transition times, see <see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see>. Higher levels use more sophisticated algorithms that can detect more infeasibilities and prune more values from domains, but at the cost of increased computation time.
        ///
        /// Propagation levels:
        ///
        /// - Level 1: Reasoning on pairs of intervals and their transition times only
        /// - Level 2: Adds detectable precedences algorithm
        /// - Level 3: Adds edge-finding reasoning
        /// - Level 4: Maximum propagation with all available algorithms
        ///
        /// From level 2 on, the algorithms work with each interval extended to the left by the smallest transition time it can have from a preceding interval.
        ///
        /// When the sequence is used in <see cref="IntervalVar.Position">IntervalVar.Position</see>, the propagation of the positions runs on every level.
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the <see cref="Parameters.preset">Parameters.preset</see> chooses the level. The <c>Default</c> preset uses strong propagation. The <c>Large</c> preset uses a lower level to reduce the propagation overhead on big models. The exact levels may change between versions.
        ///
        /// Performance considerations:
        ///
        /// The same trade-offs apply as for <see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see>. Reasoning on pairs cannot detect that three or more intervals do not fit into a time window. When the transition times are small compared to the interval lengths, higher levels often pay off.
        ///
        /// <code>
        /// var model = new Model();
        /// // ... build your model with noOverlap constraints with transition times ...
        ///
        /// // Let the preset decide (default)
        /// var result = model.Solve();
        ///
        /// // Or use maximum propagation
        /// result = model.Solve(new Parameters { noOverlapTransitionPropagationLevel = 4 });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.noOverlapPropagationLevel">Parameters.noOverlapPropagationLevel</see> — for noOverlap constraints without transition times.</description></item>
        /// <item><description><see cref="Parameters.preset">Parameters.preset</see> — for automatic configuration of propagation levels.</description></item>
        /// <item><description><see cref="Model.NoOverlap">Model.NoOverlap</see> — for creating noOverlap constraints.</description></item>
        /// </list>
        /// </remarks>
        public long? noOverlapTransitionPropagationLevel { get; set; }

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
        /// - Level 3: Adds disjunctive reasoning between pulses that are too high to overlap
        /// - Level 4: Maximum propagation. Adds a redundant <see cref="Model.NoOverlap">Model.NoOverlap</see> over the pulses higher than half of the capacity, and max-energy reasoning that limits the length and height of pulses whose length or height is not fixed
        ///
        /// Automatic selection (level 0):
        ///
        /// When set to 0 (the default), the <see cref="Parameters.preset">Parameters.preset</see> chooses the level. The <c>Default</c> preset uses strong propagation. The <c>Large</c> preset uses a lower level to reduce the propagation overhead on big models. The exact levels may change between versions.
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
        /// result = model.Solve(new Parameters { cumulPropagationLevel = 4 });
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
        /// LNS solution pool strategy.
        /// </summary>
        /// <remarks>
        /// Controls how LNS manages its solution pool.
        ///
        /// - <c>Auto</c>: Let the <see cref="Parameters.preset">Parameters.preset</see> decide (the default). The <c>Default</c>
        ///   preset resolves <c>Auto</c> to <c>Robust</c>; the <c>Large</c> preset resolves it to <c>Focused</c>.
        ///
        /// - <c>Robust</c>: Maintain multiple solution tiers for diverse exploration.
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
        /// explicitly set <c>lnsMode</c> to <c>Robust</c> or <c>Focused</c>, your value takes precedence over
        /// the preset — for example, setting <c>Robust</c> keeps the multiple solution tiers even
        /// under the <c>Large</c> preset.
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
        /// Use only the user-provided warm start as the initial solution in LNS
        /// </summary>
        /// <remarks>
        /// When this parameter is on, the solver will use only the user-specified warm start solution for the initial solution phase in LNS. If no warm start is provided, the solver will search for its own initial solution as usual.
        ///
        /// The default value is <see langword="false"/>.
        /// </remarks>
        public bool? lnsUseWarmStartOnly { get; set; }

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



        // Parameters the typed properties do not offer, by name. The solver reads a
        // string value as on its command line.
        #pragma warning disable CS1591
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Dictionary<string, object>? _extra { get; set; }
        #pragma warning restore CS1591

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

            if (noOverlapPropagationLevel != null)
            {
                if (noOverlapPropagationLevel < 0 || noOverlapPropagationLevel > 4)
                    throw new ArgumentException($"Value {noOverlapPropagationLevel} for parameter NoOverlapPropagationLevel is out of range [0, 4]");
                result["noOverlapPropagationLevel"] = noOverlapPropagationLevel;
            }

            if (noOverlapTransitionPropagationLevel != null)
            {
                if (noOverlapTransitionPropagationLevel < 0 || noOverlapTransitionPropagationLevel > 4)
                    throw new ArgumentException($"Value {noOverlapTransitionPropagationLevel} for parameter NoOverlapTransitionPropagationLevel is out of range [0, 4]");
                result["noOverlapTransitionPropagationLevel"] = noOverlapTransitionPropagationLevel;
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

            if (fdsDualStrategy != null)
            {
                var validValues = new[] { "Minimum", "Random", "Split" };
                if (!validValues.Contains(fdsDualStrategy, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{fdsDualStrategy}' for parameter FDSDualStrategy");
                result["fdsDualStrategy"] = fdsDualStrategy;
            }

            if (lnsMode != null)
            {
                var validValues = new[] { "Auto", "Robust", "Focused" };
                if (!validValues.Contains(lnsMode, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException($"Invalid value '{lnsMode}' for parameter LNSMode");
                result["lnsMode"] = lnsMode;
            }

            if (lnsUseWarmStartOnly != null)
            {
                result["lnsUseWarmStartOnly"] = lnsUseWarmStartOnly;
            }

            if (simpleLBWorker != null)
            {
                if (simpleLBWorker < -1 || simpleLBWorker > 2147483647)
                    throw new ArgumentException($"Value {simpleLBWorker} for parameter SimpleLBWorker is out of range [-1, 2147483647]");
                result["simpleLBWorker"] = simpleLBWorker;
            }



            if (_extra != null)
                result["_extra"] = Parameters.ExtraToDict(_extra);

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

            if (data.TryGetValue("noOverlapPropagationLevel", out var noOverlapPropagationLevelVal) && noOverlapPropagationLevelVal != null)
            {
                if (noOverlapPropagationLevelVal is System.Text.Json.JsonElement noOverlapPropagationLevelElem)
                {
                    if (noOverlapPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.noOverlapPropagationLevel = noOverlapPropagationLevelElem.GetInt64();
                }
            }

            if (data.TryGetValue("noOverlapTransitionPropagationLevel", out var noOverlapTransitionPropagationLevelVal) && noOverlapTransitionPropagationLevelVal != null)
            {
                if (noOverlapTransitionPropagationLevelVal is System.Text.Json.JsonElement noOverlapTransitionPropagationLevelElem)
                {
                    if (noOverlapTransitionPropagationLevelElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.noOverlapTransitionPropagationLevel = noOverlapTransitionPropagationLevelElem.GetInt64();
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

            if (data.TryGetValue("fdsDualStrategy", out var fdsDualStrategyVal) && fdsDualStrategyVal != null)
            {
                if (fdsDualStrategyVal is System.Text.Json.JsonElement fdsDualStrategyElem)
                {
                    if (fdsDualStrategyElem.ValueKind == System.Text.Json.JsonValueKind.String)
                        wp.fdsDualStrategy = fdsDualStrategyElem.GetString();
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

            if (data.TryGetValue("simpleLBWorker", out var simpleLBWorkerVal) && simpleLBWorkerVal != null)
            {
                if (simpleLBWorkerVal is System.Text.Json.JsonElement simpleLBWorkerElem)
                {
                    if (simpleLBWorkerElem.ValueKind == System.Text.Json.JsonValueKind.Number)
                        wp.simpleLBWorker = simpleLBWorkerElem.GetInt64();
                }
            }


            wp._extra = Parameters.ExtraFromDict(data);

            return wp;
        }
    }
}
