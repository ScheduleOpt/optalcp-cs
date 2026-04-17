// ===========================================================================
// OptalCP C# API - Solution
// Version: 2026.3.0
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace OptalCP
{
    /// <summary>
    /// Access to variable values after solving.
    /// </summary>
    /// <remarks>
    /// Solution of a <see cref="Model">Model</see>. When a model is solved, the solution is stored
    /// in this object. The solution contains values of all variables in the model
    /// (including optional variables) and the value of the objective (if the model
    /// specified one).
    ///
    /// <b>Preview version of OptalCP</b>
    ///
    /// Note that in the preview version of OptalCP, the values of variables in
    /// the solution are masked and replaced by value *absent* (<see langword="null"/>).
    /// </remarks>
    public class Solution
    {
        private readonly Dictionary<int, object?> _values = new();
        private long? _objective;

        /// <summary>
        /// Creates an empty solution.
        /// </summary>
        /// <remarks>
        /// Creates a solution where all variables are absent, and the
        /// objective value is <c>None</c>/<c>undefined</c>.
        ///
        /// Use this constructor to create an external solution that can be passed to
        /// the solver as a warm start (see <see cref="Model.Solve">Model.Solve</see>)
        /// or sent during solving using <see cref="Solver.SendSolution">Solver.SendSolution</see>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "x");
        /// model.Minimize(x.End());
        ///
        /// // Create an external solution
        /// var solution = new Solution();
        /// solution.SetValue(x, 0, 10);  // x starts at 0, ends at 10
        /// solution.SetObjective(10);    // objective value = x.End() = 10
        ///
        /// // Use it as a warm start
        /// var result = model.Solve(new Parameters { timeLimit = 60 }, solution);
        /// </code>
        /// </remarks>
        public Solution() { }

        /// <summary>
        /// Returns the objective value of the solution.
        /// </summary>
        /// <returns>The objective value</returns>
        /// <remarks>
        /// Returns <see langword="null"/> if no objective was specified or if the objective expression
        /// is *absent* (see optional <see cref="IntExpr">IntExpr</see>).
        ///
        /// The correct value is reported even in the preview version of OptalCP.
        /// </remarks>
        public long? GetObjective() => _objective;

        /// <summary>
        /// Sets objective value of the solution.
        /// </summary>
        /// <param name="value">The objective value to set</param>
        /// <remarks>
        /// This function can be used for construction of an external solution that can be
        /// passed to the solver (see <see cref="Model.Solve">Model.Solve</see>, <see cref="Solver">Solver</see> and
        /// <see cref="Solver.SendSolution">Solver.SendSolution</see>).
        /// </remarks>
        public void SetObjective(long? value) => _objective = value;

        /// <summary>
        /// Returns <see langword="true"/> if the given variable is present in the solution.
        /// </summary>
        /// <param name="variable">The variable to check</param>
        /// <returns>True if the variable is present</returns>
        /// <remarks>
        /// In the preview version of OptalCP, this method always returns <see langword="false"/>
        /// because real values of variables are masked and replaced by value *absent*.
        /// </remarks>
        public bool IsPresent(IntVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            return _values.ContainsKey(variable.GetId());
        }

        /// <inheritdoc cref="IsPresent(IntVar)"/>
        public bool IsPresent(BoolVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            return _values.ContainsKey(variable.GetId());
        }

        /// <inheritdoc cref="IsPresent(IntVar)"/>
        public bool IsPresent(IntervalVar interval)
        {
            ArgumentNullException.ThrowIfNull(interval);
            return _values.ContainsKey(interval.GetId());
        }

        /// <summary>
        /// Returns <see langword="true"/> if the given variable is absent in the solution.
        /// </summary>
        /// <param name="variable">The variable to check</param>
        /// <returns>True if the variable is absent</returns>
        /// <remarks>
        /// In the preview version of OptalCP, this method always returns <see langword="true"/>
        /// because real values of variables are masked and replaced by value *absent*.
        /// </remarks>
        public bool IsAbsent(IntVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            return !_values.ContainsKey(variable.GetId());
        }

        /// <inheritdoc cref="IsAbsent(IntVar)"/>
        public bool IsAbsent(BoolVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            return !_values.ContainsKey(variable.GetId());
        }

        /// <inheritdoc cref="IsAbsent(IntVar)"/>
        public bool IsAbsent(IntervalVar interval)
        {
            ArgumentNullException.ThrowIfNull(interval);
            return !_values.ContainsKey(interval.GetId());
        }

        /// <summary>
        /// Gets an integer variable's value from the solution.
        /// </summary>
        /// <param name="variable">The integer variable to get the value of.</param>
        /// <returns>The value, or null if the variable is absent.</returns>
        /// <remarks>
        /// Returns the value assigned to the given integer variable in this solution,
        /// or <see langword="null"/> if the variable is absent.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solution.IsPresent">Solution.IsPresent</see> — to check presence without retrieving the value.</description></item>
        /// </list>
        /// </remarks>
        public int? GetValue(IntVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            if (_values.TryGetValue(variable.GetId(), out var val) && val != null)
                return Convert.ToInt32(val);
            return null;
        }

        /// <summary>
        /// Gets a boolean variable's value from the solution.
        /// </summary>
        /// <param name="variable">The boolean variable to get the value of.</param>
        /// <returns>The value, or null if the variable is absent.</returns>
        /// <remarks>
        /// Returns the value assigned to the given boolean variable in this solution,
        /// or <see langword="null"/> if the variable is absent.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solution.IsPresent">Solution.IsPresent</see> — to check presence without retrieving the value.</description></item>
        /// </list>
        /// </remarks>
        public bool? GetValue(BoolVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            if (_values.TryGetValue(variable.GetId(), out var val) && val != null)
                return Convert.ToInt32(val) != 0;
            return null;
        }

        /// <summary>
        /// Gets an interval variable's start and end from the solution.
        /// </summary>
        /// <param name="interval">The interval variable to get the value of.</param>
        /// <returns>A (Start, End) tuple, or null if the interval is absent.</returns>
        /// <remarks>
        /// Returns the start and end assigned to the given interval variable in this solution,
        /// or <see langword="null"/> if the variable is absent.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solution.GetStart">Solution.GetStart</see> — to get start/end individually.</description></item>
        /// <item><description><see cref="Solution.GetEnd">Solution.GetEnd</see> — to get start/end individually.</description></item>
        /// </list>
        /// </remarks>
        public (int Start, int End)? GetValue(IntervalVar interval)
        {
            ArgumentNullException.ThrowIfNull(interval);
            if (_values.TryGetValue(interval.GetId(), out var val) && val is ValueTuple<int, int> tuple)
                return tuple;
            return null;
        }

        /// <summary>
        /// Gets an interval variable's start time from the solution.
        /// </summary>
        /// <param name="variable">The interval variable to query</param>
        /// <returns>The start value, or None if absent</returns>
        /// <remarks>
        /// If the variable is *absent* in the solution, it returns <see langword="null"/>.
        ///
        /// In the preview version of OptalCP, this function always returns <see langword="null"/>
        /// because real values of variables are masked and replaced by value *absent*.
        /// </remarks>
        public int? GetStart(IntervalVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            var value = GetValue(variable);
            return value?.Start;
        }

        /// <summary>
        /// Gets an interval variable's end time from the solution.
        /// </summary>
        /// <param name="variable">The interval variable to query</param>
        /// <returns>The end value, or None if absent</returns>
        /// <remarks>
        /// If the variable is *absent* in the solution, it returns <see langword="null"/>.
        ///
        /// In the preview version of OptalCP, this function always returns <see langword="null"/>
        /// because real values of variables are masked and replaced by value *absent*.
        /// </remarks>
        public int? GetEnd(IntervalVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            var value = GetValue(variable);
            return value?.End;
        }

        /// <summary>
        /// Gets an interval variable's length from the solution.
        /// </summary>
        /// <param name="variable">The interval variable to query</param>
        /// <returns>The length value, or None if absent</returns>
        /// <remarks>
        /// If the variable is *absent* in the solution, it returns <see langword="null"/>.
        ///
        /// In the preview version of OptalCP, this function always returns <see langword="null"/>
        /// because real values of variables are masked and replaced by value *absent*.
        /// </remarks>
        public int? GetLength(IntervalVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            var value = GetValue(variable);
            if (value.HasValue)
                return value.Value.End - value.Value.Start;
            return null;
        }

        /// <summary>
        /// Sets the value of the given integer variable in the solution.
        /// </summary>
        /// <param name="variable">The integer variable.</param>
        /// <param name="value">The value to set.</param>
        /// <remarks>
        /// The variable will be present in the solution with the given value.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solution.SetAbsent">Solution.SetAbsent</see> — to make the variable absent.</description></item>
        /// </list>
        /// </remarks>
        public void SetValue(IntVar variable, int value)
        {
            ArgumentNullException.ThrowIfNull(variable);
            _values[variable.GetId()] = value;
        }

        /// <summary>
        /// Sets the value of the given boolean variable in the solution.
        /// </summary>
        /// <param name="variable">The boolean variable.</param>
        /// <param name="value">The value to set.</param>
        /// <remarks>
        /// The variable will be present in the solution with the given value.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solution.SetAbsent">Solution.SetAbsent</see> — to make the variable absent.</description></item>
        /// </list>
        /// </remarks>
        public void SetValue(BoolVar variable, bool value)
        {
            ArgumentNullException.ThrowIfNull(variable);
            _values[variable.GetId()] = value ? 1 : 0;
        }

        /// <summary>
        /// Sets the start and end of the given interval variable in the solution.
        /// </summary>
        /// <param name="variable">The interval variable.</param>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        /// <remarks>
        /// The interval variable will be present in the solution with the given start and end.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Solution.SetAbsent">Solution.SetAbsent</see> — to make the variable absent.</description></item>
        /// </list>
        /// </remarks>
        public void SetValue(IntervalVar variable, int start, int end)
        {
            ArgumentNullException.ThrowIfNull(variable);
            _values[variable.GetId()] = (start, end);
        }

        /// <summary>
        /// Sets the given variable to be absent in the solution.
        /// </summary>
        /// <param name="variable">The variable to set as absent</param>
        /// <remarks>
        /// This function can be used for construction of an external solution that can be
        /// passed to the solver (see <see cref="Model.Solve">Model.Solve</see> and <see cref="Solver.SendSolution">Solver.SendSolution</see>).
        /// </remarks>
        public void SetAbsent(IntVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            _values.Remove(variable.GetId());
        }

        /// <inheritdoc cref="SetAbsent(IntVar)"/>
        public void SetAbsent(BoolVar variable)
        {
            ArgumentNullException.ThrowIfNull(variable);
            _values.Remove(variable.GetId());
        }

        /// <inheritdoc cref="SetAbsent(IntVar)"/>
        public void SetAbsent(IntervalVar interval)
        {
            ArgumentNullException.ThrowIfNull(interval);
            _values.Remove(interval.GetId());
        }

        /// <summary>
        /// Initializes solution from solver message data.
        /// </summary>
        internal void InitFromDict(Dictionary<string, object?> data)
        {
            // Parse values array from solver
            // Format: [{"id": 0, "value": ...}, ...]
            if (data.TryGetValue("values", out var valuesObj) && valuesObj is JsonElement valuesElem
                && valuesElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in valuesElem.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object) continue;
                    if (!item.TryGetProperty("id", out var idProp)) continue;
                    if (!idProp.TryGetInt32(out var id)) continue;

                    if (item.TryGetProperty("value", out var valueProp))
                    {
                        if (valueProp.ValueKind == JsonValueKind.Null)
                        {
                            // Absent variable - don't store
                            continue;
                        }
                        else if (valueProp.ValueKind == JsonValueKind.Object)
                        {
                            // IntervalVar value
                            if (valueProp.TryGetProperty("start", out var startProp) &&
                                valueProp.TryGetProperty("end", out var endProp))
                            {
                                // Handle both int and double values
                                int start, end;
                                if (startProp.TryGetInt32(out start) || (startProp.TryGetDouble(out var startDbl) && (start = (int)startDbl) == startDbl))
                                {
                                    if (endProp.TryGetInt32(out end) || (endProp.TryGetDouble(out var endDbl) && (end = (int)endDbl) == endDbl))
                                    {
                                        _values[id] = (start, end);
                                    }
                                }
                            }
                        }
                        else if (valueProp.ValueKind == JsonValueKind.Number)
                        {
                            // IntVar/BoolVar value - handle both int and double
                            if (valueProp.TryGetInt32(out var intVal))
                            {
                                _values[id] = intVal;
                            }
                            else if (valueProp.TryGetDouble(out var dblVal))
                            {
                                _values[id] = (int)dblVal;
                            }
                        }
                    }
                }
            }

            // Parse objective
            if (data.TryGetValue("objective", out var objVal) && objVal != null)
            {
                if (objVal is JsonElement objElem && objElem.ValueKind == JsonValueKind.Number)
                {
                    if (objElem.TryGetInt64(out var longVal))
                        _objective = longVal;
                    else if (objElem.TryGetDouble(out var dblVal))
                        _objective = (long)dblVal;
                }
                else if (objVal is long l)
                    _objective = l;
                else if (objVal is int i)
                    _objective = i;
                else if (objVal is double d)
                    _objective = (long)d;
            }
        }

        /// <summary>
        /// Converts solution to dictionary for serialization.
        /// </summary>
        internal Dictionary<string, object?> ToDict()
        {
            var valuesList = new List<Dictionary<string, object?>>();

            foreach (var kvp in _values)
            {
                var entry = new Dictionary<string, object?> { ["id"] = kvp.Key };

                if (kvp.Value is ValueTuple<int, int> tuple)
                {
                    entry["value"] = new Dictionary<string, object?>
                    {
                        ["start"] = tuple.Item1,
                        ["end"] = tuple.Item2
                    };
                }
                else
                {
                    entry["value"] = kvp.Value;
                }

                valuesList.Add(entry);
            }

            var result = new Dictionary<string, object?> { ["values"] = valuesList };
            if (_objective.HasValue)
                result["objective"] = _objective.Value;

            return result;
        }

        /// <summary>
        /// Converts the solution to JSON string.
        /// </summary>
        internal string ToJSON()
        {
            return JsonSerializer.Serialize(ToDict());
        }

        /// <summary>
        /// Parses a solution from JSON string.
        /// </summary>
        internal static Solution FromJSON(string json)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(json)
                ?? throw new ArgumentException("Invalid JSON");
            var solution = new Solution();
            solution.InitFromDict(data);
            return solution;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var objStr = _objective.HasValue ? $", objective={_objective}" : "";
            return $"Solution({_values.Count} present{objStr})";
        }
    }
}

