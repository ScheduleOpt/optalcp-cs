// ===========================================================================
// OptalCP C# API - Solver
// Version: 2026.3.0
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace OptalCP
{
    /// <summary>
    /// An event emitted when the solver finds a solution.
    /// </summary>
    /// <remarks>
    /// An event emitted by <see cref="Solver">Solver</see> when a solution is found.
    ///
    /// The event can be intercepted by setting the <see cref="Solver.OnSolution">Solver.OnSolution</see> callback.
    /// The event contains the solution, solving time so far, and the result of
    /// solution verification.
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="Solver">Solver</see></description></item>
    /// <item><description><see cref="Solver.OnSolution">Solver.OnSolution</see> — to register a solution callback.</description></item>
    /// </list>
    /// </remarks>
    public sealed class SolutionEvent
    {
        /// <summary>
        /// The duration of the solve at the time the solution was found, in seconds.
        /// </summary>
        public double SolveTime { get; }

        /// <summary>
        /// The solution containing values for all variables and the objective value.
        /// </summary>
        /// <remarks>
        /// The solution contains values of all variables in the model (including
        /// optional variables) and the value of the objective (if the model specified
        /// one).
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solution">Solution</see> — for accessing variable values.</description></item>
        /// </list>
        /// </remarks>
        public Solution Solution { get; }

        /// <summary>
        /// Result of the verification of the solution.
        /// </summary>
        /// <remarks>
        /// When parameter <see cref="Parameters.verifySolutions">Parameters.verifySolutions</see> is set to <c>True</c> (the
        /// default), the solver verifies all solutions found. The
        /// verification checks that all constraints in the model are satisfied and
        /// that the objective value is computed correctly.
        ///
        /// The verification is done using a separate code (not used during the
        /// search). The point is to independently verify the correctness of the
        /// solution.
        ///
        /// Possible values are:
        ///
        /// * <c>None</c> - the solution was not verified (because the parameter
        ///   <see cref="Parameters.verifySolutions">Parameters.verifySolutions</see> was not set).
        /// * <c>True</c> - the solution was verified and correct.
        ///
        /// The value can never be <c>False</c> because, in that case, the solver ends with an
        /// error.
        /// </remarks>
        public bool? Valid { get; }

        internal SolutionEvent(double solveTime, Solution solution, bool? valid = null)
        {
            this.SolveTime = solveTime;
            this.Solution = solution;
            this.Valid = valid;
        }
    }

    /// <summary>
    /// Async solver with event callbacks for solutions, bounds, and log messages.
    /// </summary>
    /// <remarks>
    /// Provides event-driven communication with the solver subprocess.
    ///
    /// Unlike <see cref="Model.Solve">Model.Solve</see>, <c>Solver</c> allows the user to process individual events
    /// during the solve and to stop the solver at any time. If you're
    /// interested in the final result only, use <see cref="Model.Solve">Model.Solve</see> instead.
    ///
    /// To solve a model, create a new <c>Solver</c> object and call its method <see cref="Solver.Solve">Solver.Solve</see>.
    ///
    /// Event callbacks are set as properties before calling <c>Solve</c>:
    ///
    /// - <see cref="Solver.OnError">Solver.OnError</see>: Called with a <see langword="string"/> message when an error occurs.
    /// - <see cref="Solver.OnWarning">Solver.OnWarning</see>: Called with a <see langword="string"/> for every issued warning.
    /// - <see cref="Solver.OnLog">Solver.OnLog</see>: Called with a <see langword="string"/> for every log message.
    /// - <see cref="Solver.OnSolution">Solver.OnSolution</see>: Called with a <see cref="SolutionEvent">SolutionEvent</see> when a solution is found.
    /// - <see cref="Solver.OnObjectiveBound">Solver.OnObjectiveBound</see>: Called with an <c>ObjectiveBoundEntry</c> when a new bound is proved.
    ///
    /// The solver output (log, trace, and warnings) is printed to console by default.
    /// It can be redirected to a file or suppressed using the <see cref="Parameters.printLog">Parameters.printLog</see> parameter.
    ///
    /// <para>Example:</para>
    /// <example>
    /// In the following example, we create a solver with a solution callback.
    /// We print the objective value and the value of interval variable <c>x</c>.
    /// After finding the first solution, we request the solver to stop.
    ///
    /// <code>
    /// var model = new Model();
    /// var x = model.IntervalVar(length: 10, name: "x");
    /// model.Minimize(x.End());
    ///
    /// using var solver = new Solver();
    ///
    /// // Define solution handler:
    /// solver.OnSolution = (evt) =&gt;
    /// {
    ///     var solution = evt.Solution;
    ///     Console.WriteLine($"At time {evt.SolveTime:F2}, objective = {solution.GetObjective()}");
    ///     if (solution.IsPresent(x))
    ///         Console.WriteLine($"  x: [{solution.GetStart(x)} -- {solution.GetEnd(x)}]");
    ///     else
    ///         Console.WriteLine("  x is absent");
    ///     // Request the solver to stop:
    ///     solver.Stop("We are happy with the first solution.");
    ///     return Task.CompletedTask;
    /// };
    ///
    /// var result = await solver.Solve(model, new Parameters { timeLimit = 60 });
    /// Console.WriteLine($"Duration: {result.Duration}");
    /// </code>
    /// </example>
    /// </remarks>
    public class Solver : IDisposable
    {
        internal const string Version = "2026.3.0";
        
        private Process? _process;
        private StreamWriter? _stdin;
        private StreamReader? _stdout;
        private StreamReader? _stderr;
        private bool _disposed;
        private volatile bool _stopRequested;
        private volatile bool _solving;
        private bool _closing;
        private readonly List<string> _pendingSolutions = new();
        private bool _modelSent;
        private string? _syncTextResult;
        private Parameters? _currentParameters;
        private readonly List<string> _stderrOutput = new();
        private readonly List<string> _errors = new();
        internal TextWriter? OutputWriter { get; set; } = Console.Out;
        internal TimeSpan HandshakeTimeout { get; set; } = TimeSpan.FromSeconds(20);
        private Func<string, Task>? _onLog;
        private Func<string, Task>? _onWarning;
        private Func<string?, Task>? _onError;
        private Func<SolutionEvent, Task>? _onSolution;
        private Func<ObjectiveBoundEntry, Task>? _onObjectiveBound;

        /// <summary>
        /// Callback for log messages from the solver.
        /// </summary>
        /// <remarks>
        /// The callback is called for each log message, after the message is written to the output stream (see <see cref="Parameters.printLog">Parameters.printLog</see>). The default is no callback.
        ///
        /// The callback receives one argument:
        ///
        /// - <c>msg</c> (<see langword="string"/>): The log message text.
        ///
        /// In C#, callbacks must return <c>Task</c>. Use <c>Task.CompletedTask</c> for synchronous implementations.
        ///
        /// If the callback throws an exception, the solve is aborted with that error.
        ///
        /// Note: This property cannot be changed while a solve is in progress. Attempting to set it during an active solve raises an error.
        ///
        /// <code>
        /// var solver = new Solver();
        ///
        /// // Synchronous callback
        /// solver.OnLog = (msg) =&gt; {
        ///     Console.WriteLine($"LOG: {msg}");
        ///     return Task.CompletedTask;
        /// };
        ///
        /// // Asynchronous callback
        /// solver.OnLog = async (msg) =&gt; {
        ///     await asyncLogger.InfoAsync(msg);
        /// };
        /// </code>
        ///
        /// The amount of log messages and their periodicity can be controlled by <see cref="Parameters.logLevel">Parameters.logLevel</see> and <see cref="Parameters.logPeriod">Parameters.logPeriod</see>.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.printLog">Parameters.printLog</see> — for redirecting output to a stream.</description></item>
        /// <item><description><see cref="Solver.OnWarning">Solver.OnWarning</see> — for warning messages.</description></item>
        /// </list>
        /// </remarks>
        public Func<string, Task>? OnLog
        {
            get => _onLog;
            set { ThrowIfSolving(); _onLog = value; }
        }

        /// <summary>
        /// Callback for warning messages from the solver.
        /// </summary>
        /// <remarks>
        /// The callback is called for each warning message, after the message is written to the output stream (see <see cref="Parameters.printLog">Parameters.printLog</see>). The default is no callback.
        ///
        /// The callback receives one argument:
        ///
        /// - <c>msg</c> (<see langword="string"/>): The warning message text.
        ///
        /// In C#, callbacks must return <c>Task</c>. Use <c>Task.CompletedTask</c> for synchronous implementations.
        ///
        /// If the callback throws an exception, the solve is aborted with that error.
        ///
        /// Note: This property cannot be changed while a solve is in progress. Attempting to set it during an active solve raises an error.
        ///
        /// <code>
        /// var solver = new Solver();
        ///
        /// // Synchronous callback
        /// solver.OnWarning = (msg) =&gt; {
        ///     Console.WriteLine($"WARNING: {msg}");
        ///     return Task.CompletedTask;
        /// };
        ///
        /// // Asynchronous callback
        /// solver.OnWarning = async (msg) =&gt; {
        ///     await asyncLogger.WarningAsync(msg);
        /// };
        /// </code>
        ///
        /// The amount of warning messages can be configured using <see cref="Parameters.warningLevel">Parameters.warningLevel</see>.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.printLog">Parameters.printLog</see> — for redirecting output to a stream.</description></item>
        /// <item><description><see cref="Solver.OnLog">Solver.OnLog</see> — for log messages.</description></item>
        /// <item><description><see cref="Solver.OnError">Solver.OnError</see> — for error messages.</description></item>
        /// </list>
        /// </remarks>
        public Func<string, Task>? OnWarning
        {
            get => _onWarning;
            set { ThrowIfSolving(); _onWarning = value; }
        }

        /// <summary>
        /// Callback for error messages from the solver.
        /// </summary>
        /// <remarks>
        /// The callback is called for each error message. The default is no callback.
        ///
        /// The callback receives one argument:
        ///
        /// - <c>msg</c> (<see langword="string"/>): The error message text.
        ///
        /// In C#, callbacks must return <c>Task</c>. Use <c>Task.CompletedTask</c> for synchronous implementations.
        ///
        /// If the callback throws an exception, the solve is aborted with that error.
        ///
        /// Note: This property cannot be changed while a solve is in progress. Attempting to set it during an active solve raises an error.
        ///
        /// <code>
        /// var solver = new Solver();
        ///
        /// // Synchronous callback
        /// solver.OnError = (msg) =&gt; {
        ///     Console.Error.WriteLine($"ERROR: {msg}");
        ///     return Task.CompletedTask;
        /// };
        ///
        /// // Asynchronous callback
        /// solver.OnError = async (msg) =&gt; {
        ///     await asyncLogger.ErrorAsync(msg);
        /// };
        /// </code>
        ///
        /// An error message indicates that the solve is closing. However, other messages (log, warning, solution) may still arrive after the error.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solver.OnWarning">Solver.OnWarning</see> — for warning messages.</description></item>
        /// <item><description><see cref="Solver.OnLog">Solver.OnLog</see> — for log messages.</description></item>
        /// </list>
        /// </remarks>
        public Func<string?, Task>? OnError
        {
            get => _onError;
            set { ThrowIfSolving(); _onError = value; }
        }

        /// <summary>
        /// Callback for solution events from the solver.
        /// </summary>
        /// <remarks>
        /// The callback is called each time the solver finds a new solution. The default is no callback.
        ///
        /// The callback receives one argument:
        ///
        /// - <c>event</c> (<see cref="SolutionEvent">SolutionEvent</see>): An object with the following fields:
        ///    - <c>Solution</c> (<see cref="Solution">Solution</see>): The solution object with variable values.
        ///    - <c>SolveTime</c> (<see langword="double"/>): Time when the solution was found (seconds since solve start).
        ///    - <c>Valid</c> (<c>bool?</c>): Solution verification result if enabled, <see langword="null"/> otherwise.
        ///
        /// In C#, callbacks must return <c>Task</c>. Use <c>Task.CompletedTask</c> for synchronous implementations.
        ///
        /// If the callback throws an exception, the solve is aborted with that error.
        ///
        /// Note: This property cannot be changed while a solve is in progress. Attempting to set it during an active solve raises an error.
        ///
        /// <code>
        /// var solver = new Solver();
        ///
        /// // Synchronous callback
        /// solver.OnSolution = (e) =&gt; {
        ///     var sol = e.Solution;
        ///     var time = e.SolveTime;
        ///     Console.WriteLine($"Solution found at {time:F2}s, objective={sol.GetObjective()}");
        ///     return Task.CompletedTask;
        /// };
        ///
        /// // Asynchronous callback
        /// solver.OnSolution = async (e) =&gt; {
        ///     await SaveToDatabase(e.Solution);
        /// };
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="SolutionEvent">SolutionEvent</see> — for the event structure.</description></item>
        /// <item><description><see cref="Solution">Solution</see> — for accessing variable values.</description></item>
        /// <item><description><see cref="Solver.OnObjectiveBound">Solver.OnObjectiveBound</see> — for objective bound updates.</description></item>
        /// </list>
        /// </remarks>
        public Func<SolutionEvent, Task>? OnSolution
        {
            get => _onSolution;
            set { ThrowIfSolving(); _onSolution = value; }
        }

        /// <summary>
        /// Callback for objective bound events from the solver.
        /// </summary>
        /// <remarks>
        /// The callback is called when the solver improves the bound on the objective (lower bound for minimization, upper bound for maximization). The default is no callback.
        ///
        /// The callback receives one argument:
        ///
        /// - <c>event</c> (<see cref="ObjectiveBoundEntry">ObjectiveBoundEntry</see>): An object with the following fields:
        ///    - <c>Value</c> (<see langword="double"/>): The new bound value.
        ///    - <c>SolveTime</c> (<see langword="double"/>): Time when the bound was proved (seconds since solve start).
        ///
        /// In C#, callbacks must return <c>Task</c>. Use <c>Task.CompletedTask</c> for synchronous implementations.
        ///
        /// If the callback throws an exception, the solve is aborted with that error.
        ///
        /// Note: This property cannot be changed while a solve is in progress. Attempting to set it during an active solve raises an error.
        ///
        /// <code>
        /// var solver = new Solver();
        ///
        /// // Synchronous callback
        /// solver.OnObjectiveBound = (e) =&gt; {
        ///     Console.WriteLine($"Bound: {e.Value}");
        ///     return Task.CompletedTask;
        /// };
        ///
        /// // Asynchronous callback
        /// solver.OnObjectiveBound = async (e) =&gt; {
        ///     await UpdateDashboard(e.Value);
        /// };
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="ObjectiveBoundEntry">ObjectiveBoundEntry</see> — for the event structure.</description></item>
        /// <item><description><see cref="Solver.OnSolution">Solver.OnSolution</see> — for solution events with objective values.</description></item>
        /// </list>
        /// </remarks>
        public Func<ObjectiveBoundEntry, Task>? OnObjectiveBound
        {
            get => _onObjectiveBound;
            set { ThrowIfSolving(); _onObjectiveBound = value; }
        }

        private void ThrowIfSolving()
        {
            if (_solving)
                throw new InvalidOperationException("Cannot change callbacks while solve is running");
        }



        /// <summary>
        /// Async solver with event callbacks for solutions, bounds, and log messages.
        /// </summary>
        /// <remarks>
        /// Provides event-driven communication with the solver subprocess.
        ///
        /// Unlike <see cref="Model.Solve">Model.Solve</see>, <c>Solver</c> allows the user to process individual events
        /// during the solve and to stop the solver at any time. If you're
        /// interested in the final result only, use <see cref="Model.Solve">Model.Solve</see> instead.
        ///
        /// To solve a model, create a new <c>Solver</c> object and call its method <see cref="Solver.Solve">Solver.Solve</see>.
        ///
        /// Event callbacks are set as properties before calling <c>Solve</c>:
        ///
        /// - <see cref="Solver.OnError">Solver.OnError</see>: Called with a <see langword="string"/> message when an error occurs.
        /// - <see cref="Solver.OnWarning">Solver.OnWarning</see>: Called with a <see langword="string"/> for every issued warning.
        /// - <see cref="Solver.OnLog">Solver.OnLog</see>: Called with a <see langword="string"/> for every log message.
        /// - <see cref="Solver.OnSolution">Solver.OnSolution</see>: Called with a <see cref="SolutionEvent">SolutionEvent</see> when a solution is found.
        /// - <see cref="Solver.OnObjectiveBound">Solver.OnObjectiveBound</see>: Called with an <c>ObjectiveBoundEntry</c> when a new bound is proved.
        ///
        /// The solver output (log, trace, and warnings) is printed to console by default.
        /// It can be redirected to a file or suppressed using the <see cref="Parameters.printLog">Parameters.printLog</see> parameter.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we create a solver with a solution callback.
        /// We print the objective value and the value of interval variable <c>x</c>.
        /// After finding the first solution, we request the solver to stop.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "x");
        /// model.Minimize(x.End());
        ///
        /// using var solver = new Solver();
        ///
        /// // Define solution handler:
        /// solver.OnSolution = (evt) =&gt;
        /// {
        ///     var solution = evt.Solution;
        ///     Console.WriteLine($"At time {evt.SolveTime:F2}, objective = {solution.GetObjective()}");
        ///     if (solution.IsPresent(x))
        ///         Console.WriteLine($"  x: [{solution.GetStart(x)} -- {solution.GetEnd(x)}]");
        ///     else
        ///         Console.WriteLine("  x is absent");
        ///     // Request the solver to stop:
        ///     solver.Stop("We are happy with the first solution.");
        ///     return Task.CompletedTask;
        /// };
        ///
        /// var result = await solver.Solve(model, new Parameters { timeLimit = 60 });
        /// Console.WriteLine($"Duration: {result.Duration}");
        /// </code>
        /// </example>
        /// </remarks>
        public Solver() { }

        /// <summary>
        /// Solves the model synchronously.
        /// Uses blocking I/O to avoid deadlocks in contexts with synchronization context
        /// (WinForms, WPF, Jupyter, ASP.NET).
        /// For async solving with callbacks, use Solve() (async).
        /// </summary>
        internal SolveResult SyncSolve(Model model, Parameters? parameters = null, Solution? warmStart = null)
        {
            return SyncRun("solve", model, parameters, warmStart);
        }

        /// <summary>
        /// Requests the solver to stop as soon as possible.
        /// </summary>
        /// <param name="reason">The reason why to stop. The reason will appear in the log</param>
        /// <remarks>
        /// This method only initiates the stop; it returns immediately without waiting
        /// for the solver to actually stop. Because the solver runs asynchronously,
        /// more events (such as additional solutions or log messages) may still arrive
        /// after <c>stop</c> is called. The solver will stop as soon as possible, but the
        /// exact timing depends on what it is currently doing.
        ///
        /// Requesting a stop on a solver that has already stopped has no effect.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we issue a stop command 1 minute after the first
        /// solution is found.
        ///
        /// <code>
        /// using var solver = new Solver();
        /// bool timerStarted = false;
        /// solver.OnSolution = (SolutionEvent e) =&gt; {
        ///     // We just found a solution. Set a timeout if there isn't any.
        ///     if (!timerStarted) {
        ///         timerStarted = true;
        ///         // Register a function to be called after 60 seconds:
        ///         Task.Delay(60000).ContinueWith(_ =&gt; {
        ///             Console.WriteLine("Requesting solver to stop");
        ///             solver.Stop("Stop because I said so!");
        ///         });
        ///     }
        ///     return Task.CompletedTask;
        /// };
        /// var result = await solver.Solve(model, new Parameters { timeLimit = 300 });
        /// </code>
        /// </example>
        /// </remarks>
        public void Stop(string reason = "User requested")
        {
            // No process running or already finished
            if (_process == null || _process.HasExited)
                return;

            // Second call to stop - kill immediately as a safety measure
            if (_stopRequested)
            {
                try { _process.Kill(entireProcessTree: true); } catch { }
                return;
            }

            // First call - try graceful stop via stop message
            _stopRequested = true;
            if (_stdin != null)
            {
                try
                {
                    var stopMsg = JsonSerializer.Serialize(new Dictionary<string, object?>
                    {
                        ["msg"] = "stop",
                        ["reason"] = reason
                    });
                    _stdin.WriteLine(stopMsg);
                    _stdin.Flush();
                }
                catch
                {
                    // Ignore errors when sending stop message
                }
            }
        }

        /// <summary>
        /// Send an external solution to the solver.
        /// </summary>
        /// <param name="solution">The solution to send. It must be compatible with the model; otherwise, an error is raised</param>
        /// <remarks>
        /// This function can be used to send an external solution to the solver, e.g.
        /// found by another solver, a heuristic, or a user.  The solver will take
        /// advantage of the solution to speed up the search: it will search only for
        /// better solutions (if it is a minimization or maximization problem). The
        /// solver may try to improve the provided solution by Large Neighborhood
        /// Search.
        ///
        /// The solution does not have to be better than the current best solution
        /// found by the solver. It is up to the solver whether or not it will use the
        /// solution in this case.
        ///
        /// Sending a solution to a solver that has already stopped has no effect.
        ///
        /// If the function is called before the model is sent to the solver, the
        /// solution is queued internally and sent automatically once the model transfer
        /// completes. Unless parameter <see cref="Parameters.logLevel">Parameters.logLevel</see> is set to 0, the
        /// solver will log a message when it receives the solution.
        /// </remarks>
        public void SendSolution(Solution solution)
        {
            var msg = new Dictionary<string, object?>
            {
                ["msg"] = "solution",
                ["data"] = solution.ToDict()
            };
            var json = JsonSerializer.Serialize(msg);
            if (_modelSent && _stdin != null)
            {
                _stdin.WriteLine(json);
                _stdin.Flush();
            }
            else
            {
                _pendingSolutions.Add(json);
            }
        }

        /// <summary>
        /// Solves a model with the specified parameters.
        /// </summary>
        /// <param name="model">The model to solve</param>
        /// <param name="params">The parameters for the solver</param>
        /// <param name="warmStart">An initial solution to start the solver with</param>
        /// <returns>The result of the solve when finished.</returns>
        /// <remarks>
        /// The solving process runs asynchronously. Use <c>await</c> to wait for the
        /// solver to finish.  During the solve, the solver emits events that can be
        /// intercepted using callback properties like <see cref="Solver.OnSolution">Solver.OnSolution</see>,
        /// <see cref="Solver.OnLog">Solver.OnLog</see>, etc.
        ///
        /// Communication with the solver subprocess happens through the event loop.
        /// The user code must yield control (using <c>await</c> or waiting for an event)
        /// for the solver to receive commands and send updates.
        ///
        /// <b>Warm start and external solutions</b>
        ///
        /// If the <c>warmStart</c> parameter is specified, the solver will start with the
        /// given solution.  The solution must be compatible with the model; otherwise
        /// an error is raised.  The solver will take advantage of the
        /// solution to speed up the search: it will search only for better solutions
        /// (if it is a minimization or maximization problem). The solver may try to
        /// improve the provided solution by Large Neighborhood Search.
        ///
        /// There are two ways to pass a solution to the solver: using <c>warmStart</c>
        /// parameter and using function <see cref="Solver.SendSolution">Solver.SendSolution</see>.
        /// The difference is that <c>warmStart</c> is guaranteed to be used by the solver
        /// before the solve starts.  On the other hand, <c>sendSolution</c> can be called
        /// at any time during the solve.
        ///
        /// Parameter <see cref="Parameters.lnsUseWarmStartOnly">Parameters.lnsUseWarmStartOnly</see> controls whether the
        /// solver should only use the warm start solution (and not search for other
        /// initial solutions). If no warm start is provided, the solver searches for
        /// its own initial solution as usual.
        /// </remarks>
        public async Task<SolveResult> Solve(
            Model model,
            Parameters? @params = null,
            Solution? warmStart = null)
        {
            // Re-entrancy guard
            if (_solving)
                throw new InvalidOperationException("Solver is already running. Create a new Solver instance for concurrent solves.");

            _solving = true;
            _currentParameters = @params;
            if (@params?.printLog != null)
                OutputWriter = @params.printLog;
            _stopRequested = false;
            _closing = false;
            _modelSent = false;
            _pendingSolutions.Clear();
            _stderrOutput.Clear();
            _errors.Clear();
            var result = new SolveResult();
            var keyboardInterruptCount = 0;

            // Set up Ctrl-C handler for graceful interruption
            ConsoleCancelEventHandler? cancelHandler = null;
            cancelHandler = (sender, e) =>
            {
                keyboardInterruptCount++;
                if (keyboardInterruptCount == 1)
                {
                    // First Ctrl-C: suppress and request graceful stop
                    e.Cancel = true;
                    Stop("Interrupted by user");
                }
                else
                {
                    // Second Ctrl-C: allow process termination
                    e.Cancel = false;
                }
            };
            Console.CancelKeyPress += cancelHandler;
            var objectiveHistory = new List<ObjectiveEntry>();
            var boundHistory = new List<ObjectiveBoundEntry>();

            try
            {
                await StartProcessAsync();

                // Send handshake
                await SendMessageAsync(new Dictionary<string, object?>
                {
                    ["msg"] = "handshake",
                    ["version"] = Version,
                    ["colors"] = CanUseColors()
                });

                // Read and validate handshake response
                var handshakeResponse = await ReadMessageWithTimeoutAsync(HandshakeTimeout);
                if (handshakeResponse == null)
                {
                    throw new InvalidOperationException("Solver closed unexpectedly during handshake");
                }

                var handshakeMsgType = GetString(handshakeResponse, "msg");
                if (handshakeMsgType == "error")
                {
                    var errorData = GetString(handshakeResponse, "data");
                    throw new InvalidOperationException($"Solver error: {errorData}");
                }
                if (handshakeMsgType != "handshake")
                {
                    throw new InvalidOperationException($"Unexpected handshake response: {handshakeResponse}");
                }

                // Send solve request
                // Batch when no callbacks registered for solution or objective bound
                bool batchResults = OnSolution == null && OnObjectiveBound == null;
                var solveMsg = BuildSolveMessage(model, @params, warmStart, batchResults);
                await SendMessageAsync(solveMsg);
                _modelSent = true;
                foreach (var pendingJson in _pendingSolutions)
                {
                    await _stdin!.WriteLineAsync(pendingJson);
                    await _stdin.FlushAsync(CancellationToken.None);
                }
                _pendingSolutions.Clear();

                // Read responses until summary
                while (true)
                {
                    var response = await ReadMessageAsync();
                    if (response == null) break;

                    var msgType = GetString(response, "msg");

                    // Handle error messages (allowed even after terminal message)
                    if (msgType == "error")
                    {
                        var errorMessage = GetString(response, "data");
                        var errorPrefix = GetString(response, "prefix") ?? "";
                        if (errorMessage == null)
                        {
                            Console.Error.WriteLine($"{errorPrefix}Unknown error");
                        }
                        else
                        {
                            Console.Error.Write($"{errorPrefix}{errorMessage}");
                            _errors.Add(errorMessage);
                        }
                        Console.Error.Flush();
                        if (OnError != null) await OnError(errorMessage);
                        continue;
                    }

                    // After terminal message, only error messages are allowed
                    if (_closing)
                    {
                        throw new InvalidOperationException($"Unexpected output after terminal message: {msgType}");
                    }

                    switch (msgType)
                    {
                        case "solution":
                            var solutionEvt = ParseSolution(response, result, objectiveHistory);
                            if (OnSolution != null && solutionEvt != null)
                                await OnSolution(solutionEvt);
                            break;

                        case "lowerBound":
                            var lbData = GetDataDict(response);
                            if (lbData != null)
                            {
                                if (lbData.TryGetValue("value", out var lbVal))
                                {
                                    var boundValue = ConvertToLong(lbVal);
                                    if (boundValue.HasValue)
                                    {
                                        result.ObjectiveBound = boundValue;

                                        var solveTime = GetDouble(lbData, "solveTime") ?? result.Duration;
                                        var boundEntry = new ObjectiveBoundEntry(solveTime, boundValue.Value);
                                        boundHistory.Add(boundEntry);
                                        result.BoundTime = solveTime;
                                        if (OnObjectiveBound != null) await OnObjectiveBound(boundEntry);
                                    }
                                }
                            }
                            break;

                        case "log":
                            var logData = GetString(response, "data");
                            if (!string.IsNullOrEmpty(logData))
                            {
                                OutputWriter?.Write(logData);
                                OutputWriter?.Flush();
                                if (OnLog != null) await OnLog(logData);
                            }
                            break;

                        case "warning":
                            var warnData = GetString(response, "data");
                            if (!string.IsNullOrEmpty(warnData))
                            {
                                var warnPrefix = GetString(response, "prefix") ?? "";
                                OutputWriter?.Write($"{warnPrefix}{warnData}");
                                OutputWriter?.Flush();
                                if (OnWarning != null) await OnWarning(warnData);
                            }
                            break;

                        case "summary":
                            ParseSummary(response, result, objectiveHistory, boundHistory);
                            result.ObjectiveHistory = objectiveHistory;
                            result.ObjectiveBoundHistory = boundHistory;
                            _closing = true;  // Terminal message - continue reading until EOF
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown message type from solver: {msgType}");
                    }
                }

                // Wait for process to finish
                var exitCode = StopProcess();

                // Check if stderr had output (indicates solver crash/error)
                if (_stderrOutput.Count > 0)
                {
                    var stderrText = string.Join("\n", _stderrOutput);
                    throw new InvalidOperationException($"Solver error:\n{stderrText}");
                }

                // Check if we accumulated errors during solving
                if (_errors.Count > 0)
                {
                    var errorSummary = string.Join("\n", _errors);
                    throw new InvalidOperationException($"Solver reported error(s):\n{errorSummary}");
                }

                // Check if we got expected result
                if (!_closing)
                {
                    throw new InvalidOperationException(
                        "Solver closed unexpectedly without sending results");
                }

                if (exitCode != 0 && exitCode.HasValue)
                {
                    throw new InvalidOperationException($"Solver failed with return code {exitCode}.");
                }
            }
            catch (Exception)
            {
                throw;  // Re-throw
            }
            finally
            {
                _solving = false;
                result.ObjectiveHistory = objectiveHistory;
                result.ObjectiveBoundHistory = boundHistory;
                StopProcess();

                // Remove Ctrl-C handler
                if (cancelHandler != null)
                    Console.CancelKeyPress -= cancelHandler;
            }

            return result;
        }

        /// <summary>
        /// Converts a model to text format (async version).
        /// </summary>
        /// <param name="model">The model to convert</param>
        /// <param name="parameters">Optional solver parameters</param>
        /// <param name="warmStart">Optional initial solution to include</param>
        /// <returns>Text representation of the model.</returns>
        /// <remarks>
        /// Async version of <see cref="Model.ToText">Model.ToText</see>. This method communicates with
        /// the solver process to generate the text output.
        ///
        /// The output is human-readable and similar to the IBM CP Optimizer file format.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "task_x");
        /// model.Minimize(x.End());
        ///
        /// using var solver = new Solver();
        /// var text = await solver.ToText(model);
        /// Console.WriteLine(text);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ToText">Model.ToText</see> — for synchronous usage.</description></item>
        /// <item><description><see cref="Solver.ToJS">Solver.ToJS</see> — for JavaScript export.</description></item>
        /// </list>
        /// </remarks>
        public async Task<string> ToText(
            Model model,
            Parameters? parameters = null,
            Solution? warmStart = null)
            => await RunTextCommandAsync("toText", model, parameters, warmStart);

        /// <summary>
        /// Converts the model to a text representation synchronously.
        /// </summary>
        internal string SyncToText(Model model, Parameters? parameters = null, Solution? warmStart = null)
        {
            SyncRun("toText", model, parameters, warmStart);
            return _syncTextResult ?? "";
        }

        /// <summary>
        /// Converts a model to JavaScript code (async version).
        /// </summary>
        /// <param name="model">The model to convert</param>
        /// <param name="parameters">Optional solver parameters (included in generated code)</param>
        /// <param name="warmStart">Optional initial solution to include</param>
        /// <returns>JavaScript code representing the model.</returns>
        /// <remarks>
        /// Async version of <see cref="Model.ToJS">Model.ToJS</see>. This method communicates with
        /// the solver process to generate the JavaScript output.
        ///
        /// The output is human-readable, executable with Node.js, and can be stored
        /// in a file.
        ///
        /// This feature is experimental and the result is not guaranteed to be valid
        /// in all cases.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "task_x");
        /// model.Minimize(x.End());
        ///
        /// using var solver = new Solver();
        /// var jsCode = await solver.ToJS(model);
        /// Console.WriteLine(jsCode);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ToJS">Model.ToJS</see> — for synchronous usage.</description></item>
        /// <item><description><see cref="Solver.ToText">Solver.ToText</see> — for text format export.</description></item>
        /// </list>
        /// </remarks>
        public async Task<string> ToJS(
            Model model,
            Parameters? parameters = null,
            Solution? warmStart = null)
            => await RunTextCommandAsync("toJS", model, parameters, warmStart);

        /// <summary>
        /// Converts the model to JavaScript code synchronously.
        /// </summary>
        internal string SyncToJS(Model model, Parameters? parameters = null, Solution? warmStart = null)
        {
            SyncRun("toJS", model, parameters, warmStart);
            return _syncTextResult ?? "";
        }

        /// <summary>
        /// Shared helper for text/JS conversion commands.
        /// </summary>
        private async Task<string> RunTextCommandAsync(
            string command,
            Model model,
            Parameters? parameters,
            Solution? warmStart)
        {
            try
            {
                await StartProcessAsync();
                await SendMessageAsync(new Dictionary<string, object?>
                {
                    ["msg"] = "handshake",
                    ["version"] = Version,
                    ["colors"] = CanUseColors()
                });

                // Read and validate handshake response
                var handshakeResponse = await ReadMessageWithTimeoutAsync(HandshakeTimeout);
                if (handshakeResponse == null)
                {
                    throw new InvalidOperationException("Solver closed unexpectedly during handshake");
                }

                var handshakeMsgType = GetString(handshakeResponse, "msg");
                if (handshakeMsgType == "error")
                {
                    var errorData = GetString(handshakeResponse, "data");
                    throw new InvalidOperationException($"Solver error: {errorData}");
                }
                if (handshakeMsgType != "handshake")
                {
                    throw new InvalidOperationException($"Unexpected handshake response: {handshakeResponse}");
                }

                var msg = BuildSolveMessage(model, parameters, warmStart);
                msg["msg"] = command;
                await SendMessageAsync(msg);

                while (true)
                {
                    var response = await ReadMessageAsync();
                    if (response == null) break;

                    var msgType = GetString(response, "msg");
                    if (msgType == "textModel")
                        return GetString(response, "data") ?? "";
                    if (msgType == "error")
                        throw new InvalidOperationException(GetString(response, "data") ?? "Unknown error");
                }
                return "";
            }
            finally
            {
                StopProcess();
            }
        }

        private Dictionary<string, object?> BuildSolveMessage(
            Model model,
            Parameters? parameters,
            Solution? warmStart,
            bool batchResults = false)
        {
            var msg = new Dictionary<string, object?>
            {
                ["msg"] = "solve"
            };

            var modelDict = model.ToDict();
            foreach (var kv in modelDict)
                msg[kv.Key] = kv.Value;

            if (parameters != null)
                msg["parameters"] = parameters.ToDict();

            if (warmStart != null)
                msg["warmStart"] = warmStart.ToDict();

            if (batchResults)
                msg["batchResults"] = true;

            // Write model to file if OPTALCP_MODEL is set (for debugging)
            var modelFile = Environment.GetEnvironmentVariable("OPTALCP_MODEL");
            if (!string.IsNullOrEmpty(modelFile))
            {
                try
                {
                    var json = JsonSerializer.Serialize(msg);
                    File.WriteAllText(modelFile, json);
                    Console.WriteLine($"Model file '{modelFile}' written successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Cannot write model file '{modelFile}': {ex.Message}");
                }
            }

            return msg;
        }

        private SolutionEvent? ParseSolution(Dictionary<string, object?> response, SolveResult result, List<ObjectiveEntry> objectiveHistory)
        {
            try
            {
                result.NbSolutions++;

                var data = GetDataDict(response);
                if (data == null) return null;

                var solution = new Solution();

                // Extract objective
                long? objective = null;
                if (data.TryGetValue("objective", out var obj) && obj != null)
                {
                    objective = ConvertToLong(obj);
                    solution.SetObjective(objective);
                }

                // Extract solve time and validity
                var solveTime = GetDouble(data, "solveTime") ?? result.Duration;
                var valid = GetBool(data, "verifiedOK");

                // Add to objective history
                objectiveHistory.Add(new ObjectiveEntry(solveTime, objective, valid));

                // Update result
                result.Solution = solution;
                result.SolutionTime = solveTime;
                result.SolutionValid = valid;

                // Initialize solution from values array
                solution.InitFromDict(data);

                return new SolutionEvent(solveTime, solution, valid);
            }
            catch (Exception ex)
            {
                // If solution parsing fails, still mark as feasible but log the error.
                // Fire-and-forget: this is a rare error path and ParseSolution is sync.
                _ = OnWarning?.Invoke($"Failed to parse solution: {ex.Message}");
                result.Solution = new Solution();
                return null;
            }
        }

        private void ParseSummary(
            Dictionary<string, object?> response,
            SolveResult result,
            List<ObjectiveEntry> objectiveHistory,
            List<ObjectiveBoundEntry> boundHistory)
        {
            var data = GetDataDict(response);
            if (data == null) return;

            // Parse summary statistics
            var proof = GetBool(data, "proof") ?? false;
            var nbSolutions = GetLong(data, "nbSolutions") ?? 0;

            result.NbSolutions = (int)nbSolutions;
            result.Proof = proof;

            result.NbBranches = GetLong(data, "nbBranches") ?? 0;
            result.NbFails = GetLong(data, "nbFails") ?? 0;
            result.Duration = GetDouble(data, "duration") ?? 0;
            result.MemoryUsed = GetLong(data, "memoryUsed") ?? 0;
            result.NbLNSSteps = GetLong(data, "nbLNSSteps") ?? 0;
            result.NbRestarts = GetLong(data, "nbRestarts") ?? 0;
            result.NbIntVars = (int)(GetLong(data, "nbIntVars") ?? 0);
            result.NbIntervalVars = (int)(GetLong(data, "nbIntervalVars") ?? 0);
            result.NbConstraints = (int)(GetLong(data, "nbConstraints") ?? 0);
            result.ActualWorkers = (int)(GetLong(data, "nbWorkers") ?? 0);
            result.Solver = GetString(data, "solver") ?? "";
            result.Cpu = GetString(data, "cpu") ?? "";
            result.ObjectiveSense = GetString(data, "objectiveSense");

            if (data.TryGetValue("lowerBound", out var lb) && lb != null)
                result.ObjectiveBound = ConvertToLong(lb);

            if (data.TryGetValue("objective", out var obj) && obj != null)
                result.Objective = ConvertToLong(obj);

            // Handle batched histories (when batchResults was true)
            if (data.TryGetValue("objectiveHistory", out var objHistoryVal)
                && objHistoryVal is JsonElement objHistoryArr
                && objHistoryArr.ValueKind == JsonValueKind.Array)
            {
                foreach (var entry in objHistoryArr.EnumerateArray())
                {
                    var solveTime = entry.TryGetProperty("solveTime", out var st) ? st.GetDouble() : result.Duration;
                    long? objective = entry.TryGetProperty("objective", out var ov) ? ConvertToLong(ov) : null;
                    bool? valid = entry.TryGetProperty("verifiedOK", out var vv) ? vv.GetBoolean() : null;
                    objectiveHistory.Add(new ObjectiveEntry(solveTime, objective, valid));
                }
                // Update SolutionTime and SolutionValid from last entry
                if (objectiveHistory.Count > 0)
                {
                    result.SolutionTime = objectiveHistory[^1].SolveTime;
                    result.SolutionValid = objectiveHistory[^1].Valid;
                }
            }

            if (data.TryGetValue("objectiveBoundHistory", out var boundHistoryVal)
                && boundHistoryVal is JsonElement boundHistoryArr
                && boundHistoryArr.ValueKind == JsonValueKind.Array)
            {
                foreach (var entry in boundHistoryArr.EnumerateArray())
                {
                    var solveTime = entry.TryGetProperty("solveTime", out var st) ? st.GetDouble() : result.Duration;
                    long? value = entry.TryGetProperty("value", out var vp) ? ConvertToLong(vp) : null;
                    if (value.HasValue)
                        boundHistory.Add(new ObjectiveBoundEntry(solveTime, value.Value));
                }
                // Update BoundTime from last entry
                if (boundHistory.Count > 0)
                    result.BoundTime = boundHistory[^1].SolveTime;
            }

            // Handle batched solution values (when batchResults was true)
            if (data.TryGetValue("solutionValues", out var solValuesVal) && solValuesVal != null)
            {
                if (result.Solution == null)
                    result.Solution = new Solution();
                var solutionData = new Dictionary<string, object?>
                {
                    ["values"] = solValuesVal,
                    ["objective"] = data.TryGetValue("objective", out var solObj) ? solObj : null
                };
                result.Solution.InitFromDict(solutionData);
            }
        }

        /// <summary>
        /// Find path to the <c>optalcp</c> binary.
        /// </summary>
        /// <param name="parameters">Parameters object that may contain the solver path or URL</param>
        /// <returns>The path to the solver executable or WebSocket URL</returns>
        /// <remarks>
        /// This static method is used to find the solver for <see cref="Model.Solve">Model.Solve</see>.
        /// Usually, there's no need to call this method directly. However, it could be
        /// used to check what solver would be used.
        ///
        /// The method works as follows:
        ///
        /// - If <c>parameters.solver</c> is set, its value is returned (path or URL).
        /// - If the <c>OPTALCP_SOLVER</c> environment variable is set, then it is used.
        ///
        /// - Finally, if nothing from the above works, the method assumes that
        ///   <c>optalcp</c> is in the <c>PATH</c>.
        ///
        /// <code>
        /// // Check what solver would be used
        /// var solver = Solver.FindSolver();
        /// Console.WriteLine($"Solver: {solver}");
        ///
        /// // Or check with custom parameters
        /// var custom = Solver.FindSolver(new Parameters { solver = "/custom/path/optalcp" });
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Parameters.solver">Parameters.solver</see> — to specify the solver.</description></item>
        /// </list>
        /// </remarks>
        public static string FindSolver(Parameters? parameters = null)
        {
            // 1. Check parameters.solver
            var paramSolver = parameters?.solver;
            if (!string.IsNullOrEmpty(paramSolver))
            {
                if (File.Exists(paramSolver))
                    return paramSolver;
                throw new FileNotFoundException(
                    $"parameters.solver points to invalid executable: {paramSolver}");
            }

            // 2. Check OPTALCP_SOLVER environment variable
            var envPath = Environment.GetEnvironmentVariable("OPTALCP_SOLVER");
            if (!string.IsNullOrEmpty(envPath))
            {
                if (File.Exists(envPath))
                    return envPath;
                throw new FileNotFoundException(
                    $"OPTALCP_SOLVER environment variable points to invalid executable: {envPath}");
            }

            // 3. Try binary NuGet packages (full > academic > preview)
            var binPath = TryGetSolverFromPackage("OptalCP.Bin", "OptalCP.Bin.SolverBinary");
            if (binPath != null) return binPath;

            binPath = TryGetSolverFromPackage("OptalCP.Bin.Academic", "OptalCP.Bin.Academic.SolverBinary");
            if (binPath != null) return binPath;

            binPath = TryGetSolverFromPackage("OptalCP.Bin.Preview", "OptalCP.Bin.Preview.SolverBinary");
            if (binPath != null) return binPath;

            // 4. Search in PATH
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(pathEnv))
            {
                var separator = Path.PathSeparator;
                var paths = pathEnv.Split(separator);
                var executableNames = OperatingSystem.IsWindows()
                    ? new[] { "optalcp.exe", "optalcp" }
                    : new[] { "optalcp" };

                foreach (var path in paths)
                {
                    foreach (var execName in executableNames)
                    {
                        var fullPath = Path.Combine(path, execName);
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                }
            }

            // Not found - throw with helpful message
            throw new FileNotFoundException(
                "OptalCP solver not found. Please either:\n" +
                "  1. Install a binary NuGet package (OptalCP.Bin, OptalCP.Bin.Preview, or OptalCP.Bin.Academic)\n" +
                "  2. Set the OPTALCP_SOLVER environment variable\n" +
                "  3. Ensure 'optalcp' is in your system PATH");
        }

        private static string? TryGetSolverFromPackage(string assemblyName, string typeName)
        {
            Assembly asm;
            try
            {
                asm = Assembly.Load(assemblyName);
            }
            catch (FileNotFoundException)
            {
                return null; // Package not installed
            }

            // Package is installed — errors from here should propagate
            var type = asm.GetType(typeName);
            var method = type?.GetMethod("GetSolverPath", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
                return null;
            try
            {
                return method.Invoke(null, null) as string;
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                throw e.InnerException; // Unwrap: FileNotFoundException, PlatformNotSupportedException, etc.
            }
        }

        private Task StartProcessAsync()
        {
            var resolvedPath = FindSolver(_currentParameters);
            var psi = new ProcessStartInfo
            {
                FileName = resolvedPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Add solverArgs if specified
            if (_currentParameters?.solverArgs != null)
            {
                foreach (var arg in _currentParameters.solverArgs)
                    psi.ArgumentList.Add(arg);
            }

            _process = Process.Start(psi) ??
                throw new InvalidOperationException($"Failed to start solver process: {resolvedPath}");

            _stdin = _process.StandardInput;
            _stdout = _process.StandardOutput;
            _stderr = _process.StandardError;

            // Start reading stderr asynchronously
            _ = ReadStderrAsync();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Detect if terminal supports colors.
        /// </summary>
        private bool CanUseColors()
        {
            return Utils.CanUseColors(OutputWriter);
        }

        private async Task ReadStderrAsync()
        {
            if (_stderr == null) return;

            try
            {
                while (true)
                {
                    var line = await _stderr.ReadLineAsync();
                    if (line == null) break;
                    Console.Error.Write(line);
                    Console.Error.Write('\n');
                    Console.Error.Flush();
                    _stderrOutput.Add(line);
                }
            }
            catch
            {
                // Ignore errors reading stderr
            }
        }

        private int? StopProcess()
        {
            var process = _process;
            int? exitCode = null;
            if (process != null)
            {
                try
                {
                    // Close stdin to signal the process to exit gracefully
                    _stdin?.Close();
                    _stdin = null;

                    // Give the process time to exit gracefully
                    var timeoutMs = (int)((_currentParameters?.processExitTimeout ?? 3.0) * 1000);
                    if (!process.HasExited)
                    {
                        var exited = process.WaitForExit(timeoutMs);
                        if (!exited)
                        {
                            // Force kill if it didn't exit gracefully
                            try { process.Kill(entireProcessTree: true); } catch { }
                            process.WaitForExit(1000);  // Short wait after kill
                        }
                    }

                    if (process.HasExited)
                    {
                        exitCode = process.ExitCode;
                    }
                }
                catch { }
                finally
                {
                    // Ensure streams are closed
                    try { _stdout?.Close(); } catch { }
                    try { _stderr?.Close(); } catch { }
                    _stdout = null;
                    _stderr = null;

                    // Dispose the process
                    try { process.Dispose(); } catch { }
                    _process = null;
                }
            }
            return exitCode;
        }

        private async Task SendMessageAsync(object message)
        {
            if (_stdin == null) throw new InvalidOperationException("Solver not started");

            var json = JsonSerializer.Serialize(message);
            await _stdin.WriteLineAsync(json);
            await _stdin.FlushAsync();
        }

        private async Task<Dictionary<string, object?>?> ReadMessageAsync()
        {
            return await ReadMessageWithTimeoutAsync(Timeout.InfiniteTimeSpan);
        }

        private async Task<Dictionary<string, object?>?> ReadMessageWithTimeoutAsync(TimeSpan timeout)
        {
            if (_stdout == null) throw new InvalidOperationException("Solver not started");

            try
            {
                var readTask = _stdout.ReadLineAsync();

                // Apply timeout if specified
                if (timeout != Timeout.InfiniteTimeSpan)
                {
                    using var cts = new CancellationTokenSource(timeout);

                    var completedTask = await Task.WhenAny(readTask, Task.Delay(Timeout.Infinite, cts.Token));
                    if (completedTask != readTask)
                    {
                        throw new TimeoutException($"Solver did not respond within {timeout.TotalSeconds} seconds");
                    }
                }

                var line = await readTask;
                if (string.IsNullOrEmpty(line)) return null;

                return JsonSerializer.Deserialize<Dictionary<string, object?>>(line);
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (Exception ex) when (ex is not TimeoutException)
            {
                // Log unexpected read errors but don't throw
                System.Diagnostics.Debug.WriteLine($"Solver read error: {ex.Message}");
                return null;
            }
        }

        // Helper methods for parsing JSON
        private static string? GetString(Dictionary<string, object?> dict, string key)
        {
            if (!dict.TryGetValue(key, out var val)) return null;
            if (val is string s) return s;
            if (val is JsonElement elem && elem.ValueKind == JsonValueKind.String)
                return elem.GetString();
            return val?.ToString();
        }

        private static Dictionary<string, object?>? GetDataDict(Dictionary<string, object?> dict)
        {
            if (!dict.TryGetValue("data", out var val)) return null;
            if (val is Dictionary<string, object?> d) return d;
            if (val is JsonElement elem && elem.ValueKind == JsonValueKind.Object)
            {
                var result = new Dictionary<string, object?>();
                foreach (var prop in elem.EnumerateObject())
                {
                    result[prop.Name] = prop.Value;
                }
                return result;
            }
            return null;
        }

        private static bool? GetBool(Dictionary<string, object?> dict, string key)
        {
            if (!dict.TryGetValue(key, out var val)) return null;
            if (val is bool b) return b;
            if (val is JsonElement elem)
            {
                if (elem.ValueKind == JsonValueKind.True) return true;
                if (elem.ValueKind == JsonValueKind.False) return false;
            }
            return null;
        }

        private static long? GetLong(Dictionary<string, object?> dict, string key)
        {
            if (!dict.TryGetValue(key, out var val)) return null;
            return ConvertToLong(val);
        }

        private static double? GetDouble(Dictionary<string, object?> dict, string key)
        {
            if (!dict.TryGetValue(key, out var val)) return null;
            if (val is double d) return d;
            if (val is JsonElement elem && elem.ValueKind == JsonValueKind.Number)
                return elem.GetDouble();
            return null;
        }

        private static long? ConvertToLong(object? val)
        {
            if (val == null) return null;
            if (val is long l) return l;
            if (val is int i) return i;
            if (val is double d) return (long)d;
            if (val is JsonElement elem)
            {
                if (elem.ValueKind == JsonValueKind.Number)
                {
                    if (elem.TryGetInt64(out var longVal))
                        return longVal;
                    if (elem.TryGetDouble(out var dblVal))
                        return (long)dblVal;
                }
                if (elem.ValueKind == JsonValueKind.Null)
                    return null;
            }
            return null;
        }

        /// <summary>
        /// Run a command against the solver synchronously using blocking I/O.
        /// This prevents deadlocks when called from contexts with synchronization context
        /// (WinForms, WPF, Jupyter, ASP.NET).
        /// </summary>
        private SolveResult SyncRun(
            string command,
            Model model,
            Parameters? parameters,
            Solution? warmStart)
        {
            // Re-entrancy guard
            if (_solving)
                throw new InvalidOperationException("Solver is already running. Create a new Solver instance for concurrent solves.");

            _solving = true;
            _currentParameters = parameters;
            if (parameters?.printLog != null)
                OutputWriter = parameters.printLog;
            _stopRequested = false;
            _closing = false;
            _modelSent = false;
            _pendingSolutions.Clear();
            _syncTextResult = null;
            _stderrOutput.Clear();
            _errors.Clear();
            var result = new SolveResult();
            var objectiveHistory = new List<ObjectiveEntry>();
            var boundHistory = new List<ObjectiveBoundEntry>();
            var keyboardInterruptCount = 0;

            // Set up Ctrl-C handler for graceful interruption
            ConsoleCancelEventHandler? cancelHandler = null;
            Process? syncProcess = null;

            cancelHandler = (sender, e) =>
            {
                keyboardInterruptCount++;
                if (keyboardInterruptCount == 1)
                {
                    // First Ctrl-C: suppress and request graceful stop
                    e.Cancel = true;
                    if (syncProcess != null && !syncProcess.HasExited)
                    {
                        try
                        {
                            var stopMsg = JsonSerializer.Serialize(new Dictionary<string, object?>
                            {
                                ["msg"] = "stop",
                                ["reason"] = "Interrupted by user"
                            });
                            syncProcess.StandardInput.WriteLine(stopMsg);
                            syncProcess.StandardInput.Flush();
                        }
                        catch
                        {
                            // Can't send graceful stop - kill the process
                            try { syncProcess.Kill(); } catch { }
                        }
                    }
                }
                else
                {
                    // Second Ctrl-C: allow process termination
                    e.Cancel = false;
                }
            };
            Console.CancelKeyPress += cancelHandler;

            try
            {
                // Start process
                var resolvedPath = FindSolver(parameters);
                var psi = new ProcessStartInfo
                {
                    FileName = resolvedPath,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Add solverArgs if specified
                if (parameters?.solverArgs != null)
                {
                    foreach (var arg in parameters.solverArgs)
                        psi.ArgumentList.Add(arg);
                }

                syncProcess = Process.Start(psi) ??
                    throw new InvalidOperationException($"Failed to start solver process: {resolvedPath}");

                // Send handshake (blocking)
                var handshake = new Dictionary<string, object?>
                {
                    ["msg"] = "handshake",
                    ["version"] = Version,
                    ["colors"] = CanUseColors()
                };
                syncProcess.StandardInput.WriteLine(JsonSerializer.Serialize(handshake));
                syncProcess.StandardInput.Flush();

                // Read handshake response (blocking)
                var handshakeResponse = syncProcess.StandardOutput.ReadLine();
                if (string.IsNullOrEmpty(handshakeResponse))
                {
                    throw new InvalidOperationException("Solver closed unexpectedly during handshake");
                }
                var handshakeData = JsonSerializer.Deserialize<Dictionary<string, object?>>(handshakeResponse);
                if (handshakeData != null)
                {
                    var handshakeMsgType = GetString(handshakeData, "msg");
                    if (handshakeMsgType == "error")
                    {
                        var errorData = GetString(handshakeData, "data");
                        throw new InvalidOperationException($"Solver error: {errorData}");
                    }
                    if (handshakeMsgType != "handshake")
                    {
                        throw new InvalidOperationException($"Unexpected handshake response: {handshakeResponse}");
                    }
                }

                // Build and send command
                // Sync always batches results (no callbacks available, matching Python _sync_run)
                var solveMsg = BuildSolveMessage(model, parameters, warmStart, batchResults: command == "solve");
                solveMsg["msg"] = command;
                syncProcess.StandardInput.WriteLine(JsonSerializer.Serialize(solveMsg));
                syncProcess.StandardInput.Flush();
                _modelSent = true;
                foreach (var pendingJson in _pendingSolutions)
                {
                    syncProcess.StandardInput.WriteLine(pendingJson);
                    syncProcess.StandardInput.Flush();
                }
                _pendingSolutions.Clear();

                // Read responses until done (blocking)
                while (true)
                {
                    var line = syncProcess.StandardOutput.ReadLine();
                    if (string.IsNullOrEmpty(line)) break;

                    var response = JsonSerializer.Deserialize<Dictionary<string, object?>>(line);
                    if (response == null) break;

                    var msgType = GetString(response, "msg");

                    // Handle error messages (allowed even after terminal message)
                    if (msgType == "error")
                    {
                        var errorMessage = GetString(response, "data");
                        var errorPrefix = GetString(response, "prefix") ?? "";
                        if (errorMessage == null)
                        {
                            Console.Error.WriteLine($"{errorPrefix}Unknown error");
                        }
                        else
                        {
                            Console.Error.Write($"{errorPrefix}{errorMessage}");
                            _errors.Add(errorMessage);
                        }
                        Console.Error.Flush();
                        if (OnError != null) Task.Run(() => OnError(errorMessage)).GetAwaiter().GetResult();
                        continue;
                    }

                    // After terminal message, only error messages are allowed
                    if (_closing)
                    {
                        throw new InvalidOperationException($"Unexpected output after terminal message: {msgType}");
                    }

                    switch (msgType)
                    {
                        case "solution":
                            var syncSolutionEvt = ParseSolution(response, result, objectiveHistory);
                            if (OnSolution != null && syncSolutionEvt != null)
                                Task.Run(() => OnSolution(syncSolutionEvt)).GetAwaiter().GetResult();
                            break;

                        case "lowerBound":
                            var lbData = GetDataDict(response);
                            if (lbData != null)
                            {
                                if (lbData.TryGetValue("value", out var lbVal))
                                {
                                    var boundValue = ConvertToLong(lbVal);
                                    if (boundValue.HasValue)
                                    {
                                        result.ObjectiveBound = boundValue;
                                        var solveTime = GetDouble(lbData, "solveTime") ?? result.Duration;
                                        var boundEntry = new ObjectiveBoundEntry(solveTime, boundValue.Value);
                                        boundHistory.Add(boundEntry);
                                        result.BoundTime = solveTime;
                                        if (OnObjectiveBound != null) Task.Run(() => OnObjectiveBound(boundEntry)).GetAwaiter().GetResult();
                                    }
                                }
                            }
                            break;

                        case "log":
                            var logData = GetString(response, "data");
                            if (!string.IsNullOrEmpty(logData))
                            {
                                OutputWriter?.Write(logData);
                                OutputWriter?.Flush();
                                if (OnLog != null) Task.Run(() => OnLog(logData)).GetAwaiter().GetResult();
                            }
                            break;

                        case "warning":
                            var warnData = GetString(response, "data");
                            if (!string.IsNullOrEmpty(warnData))
                            {
                                var warnPrefix = GetString(response, "prefix") ?? "";
                                OutputWriter?.Write($"{warnPrefix}{warnData}");
                                OutputWriter?.Flush();
                                if (OnWarning != null) Task.Run(() => OnWarning(warnData)).GetAwaiter().GetResult();
                            }
                            break;

                        case "textModel":
                            _syncTextResult = GetString(response, "data");
                            _closing = true;
                            break;

                        case "summary":
                            ParseSummary(response, result, objectiveHistory, boundHistory);
                            result.ObjectiveHistory = objectiveHistory;
                            result.ObjectiveBoundHistory = boundHistory;
                            _closing = true;
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown message type from solver: {msgType}");
                    }
                }

                // Wait for process to finish with timeout
                var exitTimeoutMs = (int)((parameters?.processExitTimeout ?? 3.0) * 1000);
                if (!syncProcess.WaitForExit(exitTimeoutMs))
                {
                    try { syncProcess.Kill(entireProcessTree: true); } catch { }
                    syncProcess.WaitForExit(1000);  // Short wait after kill
                }

                // Read any remaining stderr and track it
                var stderrContent = syncProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(stderrContent))
                {
                    Console.Error.Write(stderrContent);
                    Console.Error.Flush();
                    foreach (var stderrLine in stderrContent.Trim().Split('\n'))
                        _stderrOutput.Add(stderrLine);
                }

                // Check if stderr had output (indicates solver crash/error)
                if (_stderrOutput.Count > 0)
                {
                    var stderrText = string.Join("\n", _stderrOutput);
                    throw new InvalidOperationException($"Solver error:\n{stderrText}");
                }

                // Check if we accumulated errors during solving
                if (_errors.Count > 0)
                {
                    var errorSummary = string.Join("\n", _errors);
                    throw new InvalidOperationException($"Solver reported error(s):\n{errorSummary}");
                }

                // Check if we got expected result
                if (!_closing)
                {
                    throw new InvalidOperationException(
                        "Solver closed unexpectedly without sending results");
                }

                if (syncProcess.HasExited && syncProcess.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Solver failed with return code {syncProcess.ExitCode}.");
                }
            }
            catch (Exception)
            {
                if (syncProcess != null && !syncProcess.HasExited)
                {
                    try { syncProcess.Kill(); } catch { }
                }
                throw;  // Re-throw
            }
            finally
            {
                _solving = false;
                result.ObjectiveHistory = objectiveHistory;
                result.ObjectiveBoundHistory = boundHistory;

                // Clean up process
                if (syncProcess != null)
                {
                    try { syncProcess.StandardInput.Close(); } catch { }
                    try { syncProcess.StandardOutput.Close(); } catch { }
                    try { syncProcess.StandardError.Close(); } catch { }
                    try { syncProcess.Dispose(); } catch { }
                }

                // Remove Ctrl-C handler
                if (cancelHandler != null)
                    Console.CancelKeyPress -= cancelHandler;
            }

            return result;
        }


        /// <summary>
        /// Disposes the solver and any active process.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _stopRequested = true;
            StopProcess();
        }
    }
}
