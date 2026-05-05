// ===========================================================================
// OptalCP C# API - Result Types
// Version: 2026.4.0
// ===========================================================================

using System;
using System.Collections.Generic;

namespace OptalCP
{
    /// <summary>
    /// Single entry in the objective value history.
    /// </summary>
    /// <remarks>
    /// Tracks when each improving solution was found during the solve, along with
    /// its objective value and validation status.
    ///
    /// There is one entry for each solution found. The entries are ordered by
    /// solve time.
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="SolveResult.ObjectiveHistory">SolveResult.ObjectiveHistory</see> — for accessing the history.</description></item>
    /// </list>
    /// </remarks>
    public class ObjectiveEntry
    {
        /// <summary>
        /// Duration of the solve when this solution was found, in seconds.
        /// </summary>
        /// <returns>Seconds elapsed.</returns>
        public double SolveTime { get; }
        /// <summary>
        /// The objective value of this solution.
        /// </summary>
        /// <returns>The objective value, or None if not applicable.</returns>
        /// <remarks>
        /// The value is <c>None</c> when:
        ///
        /// - No objective was specified in the model (no <see cref="Model.Minimize">Model.Minimize</see> or <see cref="Model.Maximize">Model.Maximize</see> call).
        /// - The objective expression has an absent value in this solution.
        /// </remarks>
        public long? Objective { get; }
        /// <summary>
        /// Whether this solution was verified (if verification is enabled).
        /// </summary>
        /// <returns>True if verified, None if not verified.</returns>
        /// <remarks>
        /// When parameter <see cref="Parameters.verifySolutions">Parameters.verifySolutions</see> is set to <c>True</c> (the
        /// default), the solver verifies all solutions found. The verification checks
        /// that all constraints in the model are satisfied and that the objective value
        /// is computed correctly.
        ///
        /// The verification is done using separate code (not used during the search).
        /// The point is to independently verify the correctness of the solution.
        ///
        /// Possible values are:
        ///
        /// - <c>None</c> - the solution was not verified (because the parameter
        ///    <see cref="Parameters.verifySolutions">Parameters.verifySolutions</see> was not set).
        /// - <c>True</c> - the solution was verified and correct.
        ///
        /// The value can never be <c>False</c> because, in this case, the solver would
        /// stop with an error.
        /// </remarks>
        public bool? Valid { get; }

        internal ObjectiveEntry(double solveTime, long? objective, bool? valid = null)
        {
            this.SolveTime = solveTime;
            this.Objective = objective;
            this.Valid = valid;
        }
    }

    /// <summary>
    /// Single entry in the objective bound history.
    /// </summary>
    /// <remarks>
    /// Tracks when a new (better) bound on the objective was proved, along with
    /// the solve time and bound value. For minimization problems, this is the
    /// lower bound; for maximization, the upper bound.
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="SolveResult.ObjectiveBoundHistory">SolveResult.ObjectiveBoundHistory</see> — for accessing the history.</description></item>
    /// </list>
    /// </remarks>
    public class ObjectiveBoundEntry
    {
        /// <summary>
        /// Duration of the solve at the time the bound was found, in seconds.
        /// </summary>
        /// <returns>Seconds elapsed.</returns>
        public double SolveTime { get; }
        /// <summary>
        /// The new bound value.
        /// </summary>
        /// <returns>Bound value.</returns>
        /// <remarks>
        /// For minimization problems, this is a lower bound on the objective.
        /// For maximization problems, this is an upper bound on the objective.
        /// </remarks>
        public long? Value { get; }

        internal ObjectiveBoundEntry(double solveTime, long? value)
        {
            this.SolveTime = solveTime;
            this.Value = value;
        }
    }

    /// <summary>
    /// Holds the solution, objective, statistics, and solve status after solving.
    /// </summary>
    /// <remarks>
    /// The result returned by <see cref="Model.Solve">Model.Solve</see> or <see cref="Solver.Solve">Solver.Solve</see>.
    ///
    /// Contains comprehensive information about the solve:
    ///
    /// Solution data:
    ///
    /// - <see cref="SolveResult.Solution">SolveResult.Solution</see>: The best solution found (or <c>None</c> if no solution exists)
    /// - <see cref="SolveResult.Objective">SolveResult.Objective</see>: The objective value of the best solution
    /// - <see cref="SolveResult.ObjectiveBound">SolveResult.ObjectiveBound</see>: The proved bound on the objective
    ///
    /// Solve statistics:
    ///
    /// - <see cref="SolveResult.NbSolutions">SolveResult.NbSolutions</see>: Total number of solutions found
    /// - <see cref="SolveResult.Proof">SolveResult.Proof</see>: Whether optimality or infeasibility was proved
    /// - <see cref="SolveResult.Duration">SolveResult.Duration</see>: Total solve time in seconds
    ///
    /// History tracking:
    ///
    /// - <see cref="SolveResult.ObjectiveHistory">SolveResult.ObjectiveHistory</see>: When each improving solution was found
    /// - <see cref="SolveResult.ObjectiveBoundHistory">SolveResult.ObjectiveBoundHistory</see>: When each bound improvement was proved
    ///
    /// <para>Example:</para>
    /// <example>
    /// <code>
    /// var model = new Model();
    /// // ... build model ...
    ///
    /// var result = model.Solve();
    ///
    /// if (result.Solution != null) {
    ///     Console.WriteLine($"Found solution with objective {result.Objective}");
    ///     Console.WriteLine($"Best solution found at {result.SolutionTime:F2}s");
    ///     if (result.Proof)
    ///         Console.WriteLine("Solution is optimal!");
    /// } else {
    ///     if (result.Proof)
    ///         Console.WriteLine("Problem is infeasible");
    ///     else
    ///         Console.WriteLine("No solution found within time limit");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public class SolveResult
    {
        /// <summary>
        /// The best solution found during the solve.
        /// </summary>
        /// <returns>The best solution, or None if no solution was found.</returns>
        /// <remarks>
        /// For optimization problems, this is the solution with the best objective value.
        /// For satisfaction problems, this is the last solution found.
        ///
        /// Returns <c>None</c> when no solution was found (the problem may be infeasible
        /// or the solve was interrupted before finding any solution).
        /// </remarks>
        public Solution? Solution { get; internal set; }

        /// <summary>
        /// Best objective value found (for optimization problems).
        /// </summary>
        /// <returns>The objective value, or None if not applicable.</returns>
        /// <remarks>
        /// The value is <c>None</c> when:
        ///
        /// - No objective was specified in the model (no <see cref="Model.Minimize">Model.Minimize</see> or <see cref="Model.Maximize">Model.Maximize</see> call).
        /// - No solution was found.
        /// - The objective expression has an absent value in the best solution.
        /// </remarks>
        public long? Objective { get; internal set; }

        /// <summary>
        /// Proved bound on the objective value.
        /// </summary>
        /// <returns>The objective bound, or None if no bound was proved.</returns>
        /// <remarks>
        /// For minimization problems, this is a lower bound: the solver proved that
        /// no solution exists with an objective value less than this bound.
        ///
        /// For maximization problems, this is an upper bound: the solver proved that
        /// no solution exists with an objective value greater than this bound.
        ///
        /// The value is <c>None</c> when no bound was proved or for satisfaction problems.
        /// </remarks>
        public long? ObjectiveBound { get; internal set; }

        /// <summary>
        /// Number of solutions found during the solve.
        /// </summary>
        /// <returns>Solution count.</returns>
        public int NbSolutions { get; internal set; }

        /// <summary>
        /// Whether the solve ended with a proof (optimality or infeasibility).
        /// </summary>
        /// <returns>True if the solve completed with a proof.</returns>
        /// <remarks>
        /// When <c>True</c>, the solver has either:
        ///
        /// - Proved optimality (found a solution within the bounds defined by
        ///    <see cref="Parameters.absoluteGapTolerance">Parameters.absoluteGapTolerance</see> and <see cref="Parameters.relativeGapTolerance">Parameters.relativeGapTolerance</see>), or
        /// - Proved infeasibility (no solution exists)
        ///
        /// When <c>False</c>, the solve was interrupted (e.g., by time limit) before
        /// a proof could be established.
        /// </remarks>
        public bool Proof { get; internal set; }

        /// <summary>
        /// Total duration of the solve in seconds.
        /// </summary>
        /// <returns>Seconds elapsed.</returns>
        public double Duration { get; internal set; }

        /// <summary>
        /// Total number of branches explored during the solve.
        /// </summary>
        /// <returns>Branch count.</returns>
        public long NbBranches { get; internal set; }

        /// <summary>
        /// Total number of failures encountered during the solve.
        /// </summary>
        /// <returns>Failure count.</returns>
        public long NbFails { get; internal set; }

        /// <summary>
        /// Total number of Large Neighborhood Search steps.
        /// </summary>
        /// <returns>LNS step count.</returns>
        public long NbLNSSteps { get; internal set; }

        /// <summary>
        /// Total number of restarts performed during the solve.
        /// </summary>
        /// <returns>Restart count.</returns>
        public long NbRestarts { get; internal set; }

        /// <summary>
        /// Memory used by the solver in bytes.
        /// </summary>
        /// <returns>Bytes used.</returns>
        public long MemoryUsed { get; internal set; }

        /// <summary>
        /// Number of integer variables in the model.
        /// </summary>
        /// <returns>Integer variable count.</returns>
        public int NbIntVars { get; internal set; }

        /// <summary>
        /// Number of interval variables in the model.
        /// </summary>
        /// <returns>Interval variable count.</returns>
        public int NbIntervalVars { get; internal set; }

        /// <summary>
        /// Number of constraints in the model.
        /// </summary>
        /// <returns>Constraint count.</returns>
        public int NbConstraints { get; internal set; }

        /// <summary>
        /// Solver name and version string.
        /// </summary>
        /// <returns>The solver identification string.</returns>
        /// <remarks>
        /// Contains the solver name followed by its version number.
        /// </remarks>
        public string Solver { get; internal set; } = "";

        /// <summary>
        /// Number of worker threads actually used during solving.
        /// </summary>
        /// <returns>Worker count.</returns>
        /// <remarks>
        /// This is the actual number of workers used by the solver, which may differ
        /// from the requested <see cref="Parameters.nbWorkers">Parameters.nbWorkers</see> if that parameter was not
        /// specified (auto-detect) or if the system has fewer cores than requested.
        /// </remarks>
        public int ActualWorkers { get; internal set; }

        /// <summary>
        /// CPU name detected by the solver.
        /// </summary>
        /// <returns>CPU model name.</returns>
        /// <remarks>
        /// Contains the CPU model name as detected by the operating system.
        /// </remarks>
        public string Cpu { get; internal set; } = "";

        /// <summary>
        /// Objective direction.
        /// </summary>
        /// <returns>'minimize', 'maximize', or None for satisfaction problems.</returns>
        /// <remarks>
        /// Indicates whether the model was a minimization problem, maximization problem,
        /// or a satisfaction problem (no objective).
        /// </remarks>
        public string? ObjectiveSense { get; internal set; }

        /// <summary>
        /// History of objective value improvements during the solve.
        /// </summary>
        /// <returns>Sequence of objective entries, one per solution found.</returns>
        /// <remarks>
        /// Returns a sequence of <see cref="ObjectiveEntry">ObjectiveEntry</see> objects, one for each solution
        /// found during the solve.
        ///
        /// Each entry contains:
        ///
        /// - <see cref="ObjectiveEntry.SolveTime">ObjectiveEntry.SolveTime</see>: When the solution was found
        /// - <see cref="ObjectiveEntry.Objective">ObjectiveEntry.Objective</see>: The objective value of that solution
        /// - <see cref="ObjectiveEntry.Valid">ObjectiveEntry.Valid</see>: Whether the solution was verified
        ///
        /// The entries are ordered chronologically by solve time.
        /// </remarks>
        public IReadOnlyList<ObjectiveEntry> ObjectiveHistory { get; internal set; } = Array.Empty<ObjectiveEntry>();

        /// <summary>
        /// History of objective bound improvements during the solve.
        /// </summary>
        /// <returns>Sequence of bound entries, one per bound improvement.</returns>
        /// <remarks>
        /// Returns a sequence of <see cref="ObjectiveBoundEntry">ObjectiveBoundEntry</see> objects, one for each
        /// bound improvement proved during the solve.
        ///
        /// Each entry contains:
        ///
        /// - <see cref="ObjectiveBoundEntry.SolveTime">ObjectiveBoundEntry.SolveTime</see>: When the bound was proved
        /// - <see cref="ObjectiveBoundEntry.Value">ObjectiveBoundEntry.Value</see>: The bound value
        ///
        /// For minimization problems, these are lower bounds. For maximization problems,
        /// these are upper bounds. The entries are ordered chronologically by solve time.
        /// </remarks>
        public IReadOnlyList<ObjectiveBoundEntry> ObjectiveBoundHistory { get; internal set; } = Array.Empty<ObjectiveBoundEntry>();

        /// <summary>
        /// Time when the best solution was found.
        /// </summary>
        /// <returns>The time in seconds, or None if no solution was found.</returns>
        /// <remarks>
        /// The time is measured from the start of the solve, in seconds.
        ///
        /// Returns <c>None</c> when no solution was found.
        /// </remarks>
        public double? SolutionTime { get; internal set; }

        /// <summary>
        /// Time of the last objective bound improvement.
        /// </summary>
        /// <returns>The time in seconds, or None if no bound was proved.</returns>
        /// <remarks>
        /// The time is measured from the start of the solve, in seconds.
        ///
        /// Returns <c>None</c> when no bound was proved during the solve.
        /// </remarks>
        public double? BoundTime { get; internal set; }

        /// <summary>
        /// Whether the best solution was verified.
        /// </summary>
        /// <returns>True if verified, None if verification was not performed.</returns>
        /// <remarks>
        /// When parameter <see cref="Parameters.verifySolutions">Parameters.verifySolutions</see> is set to <c>True</c> (the
        /// default), the solver verifies all solutions found. The verification checks
        /// that all constraints in the model are satisfied and that the objective value
        /// is computed correctly.
        ///
        /// Possible values:
        ///
        /// - <c>None</c> - verification was not performed (parameter was not set)
        /// - <c>True</c> - the solution was verified and correct
        ///
        /// The value can never be <c>False</c> because, in that case, the solver would
        /// stop with an error.
        /// </remarks>
        public bool? SolutionValid { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (NbSolutions > 0)
            {
                var objStr = Objective.HasValue ? $", objective={Objective}" : "";
                return $"<SolveResult: {NbSolutions} solution(s){objStr}, duration={Duration:F2}s>";
            }
            return $"<SolveResult: no solution, proof={Proof}, duration={Duration:F2}s>";
        }
    }
}
