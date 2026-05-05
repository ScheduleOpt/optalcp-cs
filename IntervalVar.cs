// ===========================================================================
// OptalCP C# API - Scheduling Types (IntervalVar, SequenceVar, IntStepFunction)
// Version: 2026.4.0
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace OptalCP
{
    /// <summary>
    /// Interval (task) variable for scheduling. Has start, end, length, and optional presence.
    /// </summary>
    /// <remarks>
    /// Interval variable is a task, action, operation, or any other interval with a start
    /// and an end. The start and the end of the interval are unknowns that the solver
    /// has to find. They could be accessed as integer expressions using
    /// <see cref="IntervalVar.Start">IntervalVar.Start</see> and <see cref="IntervalVar.End">IntervalVar.End</see>.
    /// or using <see cref="Model.Start">Model.Start</see> and <see cref="Model.End">Model.End</see>.
    /// In addition to the start and the end of the interval, the interval variable
    /// has a length (equal to _end - start_) that can be accessed using
    /// <see cref="IntervalVar.Length">IntervalVar.Length</see> or <see cref="Model.Length">Model.Length</see>.
    ///
    /// The interval variable can be optional. In this case, the solver can decide
    /// to make the interval absent, which is usually interpreted as the fact that
    /// the interval doesn't exist, the task/action was not executed, or the operation
    /// was not performed.  When the interval variable is absent, its start, end,
    /// and length are also absent.  A boolean expression that represents the presence
    /// of the interval variable can be accessed using
    /// <see cref="IntervalVar.Presence">IntervalVar.Presence</see> and <see cref="Model.Presence">Model.Presence</see>.
    ///
    /// Interval variables can be created using the function
    /// <see cref="Model.IntervalVar">Model.IntervalVar</see>.
    /// By default, interval variables are *present* (not optional).
    /// To create an optional interval, specify <c>optional: true</c> in the
    /// arguments of the function.
    ///
    /// <para>Example:</para>
    /// <example>
    /// In the following example we create three present interval variables <c>x</c>, <c>y</c> and <c>z</c>
    /// and we make sure that they don't overlap.  Then, we minimize the maximum of
    /// the end times of the three intervals (the makespan):
    ///
    /// <code>
    /// var model = new Model();
    /// var x = model.IntervalVar(length: 10, name: "x");
    /// var y = model.IntervalVar(length: 10, name: "y");
    /// var z = model.IntervalVar(length: 10, name: "z");
    /// model.NoOverlap(new[] { x, y, z });
    /// model.Minimize(model.Max(new[] { x.End(), y.End(), z.End() }));
    /// var result = model.Solve();
    /// </code>
    /// </example><para>Example:</para>
    /// <example>
    /// In the following example, there is a task _X_ that could be performed by two
    /// different workers _A_ and _B_.  The interval variable <c>X</c> represents the task.
    /// It is not optional because the task <c>X</c> is mandatory. Interval variable
    /// <c>XA</c> represents the task <c>X</c> when performed by worker _A_ and
    /// similarly <c>XB</c> represents the task <c>X</c> when performed by worker _B_.
    /// Both <c>XA</c> and <c>XB</c> are optional because it is not known beforehand which
    /// worker will perform the task.  The constraint <see cref="IntervalVar.Alternative">IntervalVar.Alternative</see> links
    /// <c>X</c>, <c>XA</c> and <c>XB</c> together and ensures that only one of <c>XA</c> and <c>XB</c> is present and that
    /// <c>X</c> and the present interval are equal.
    ///
    /// <code>
    /// var model = new Model();
    /// var X = model.IntervalVar(length: 10, name: "X");
    /// var XA = model.IntervalVar(name: "XA", optional: true);
    /// var XB = model.IntervalVar(name: "XB", optional: true);
    /// model.Alternative(X, new[] { XA, XB });
    /// var result = model.Solve();
    /// </code>
    ///
    /// Variables <c>XA</c> and <c>XB</c> can be used elsewhere in the model, e.g. to make sure
    /// that each worker is assigned to at most one task at a time:
    ///
    /// <code>
    /// // Tasks of worker A don't overlap:
    /// model.NoOverlap(new[] { XA /* ... other Worker A tasks */ });
    /// // Tasks of worker B don't overlap:
    /// model.NoOverlap(new[] { XB /* ... other Worker B tasks */ });
    /// </code>
    /// </example>
    /// </remarks>
    public class IntervalVar : ModelElement
    {
        internal IntervalVar(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId)
        {
            EnsureRef();
        }

        internal IntervalVar(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args)
        {
            EnsureRef();
        }

        /// <summary>
        /// The minimum start time of the interval variable.
        /// </summary>
        /// <remarks>
        /// Gets or sets the minimum start time of the interval variable.
        ///
        /// The initial value is set during construction by <see cref="Model.IntervalVar">Model.IntervalVar</see>.
        ///
        /// Note: This property reflects the interval's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range <see cref="Model.IntervalMin">Model.IntervalMin</see> to <see cref="Model.IntervalMax">Model.IntervalMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task = model.IntervalVar(length: 10, name: "task");
        ///
        /// Console.WriteLine(task.StartMin);  // 0 (default)
        ///
        /// task.StartMin = 5;
        /// Console.WriteLine(task.StartMin);  // 5
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StartMax">IntervalVar.StartMax</see></description></item>
        /// <item><description><see cref="IntervalVar.EndMin">IntervalVar.EndMin</see></description></item>
        /// <item><description><see cref="IntervalVar.LengthMin">IntervalVar.LengthMin</see></description></item>
        /// </list>
        /// </remarks>
        public int StartMin
        {
            get => _props.TryGetValue("startMin", out var v) ? Convert.ToInt32(v) : 0;
            set => _props["startMin"] = value;
        }

        /// <summary>
        /// The maximum start time of the interval variable.
        /// </summary>
        /// <remarks>
        /// Gets or sets the maximum start time of the interval variable.
        ///
        /// The initial value is set during construction by <see cref="Model.IntervalVar">Model.IntervalVar</see>.
        ///
        /// Note: This property reflects the interval's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range <see cref="Model.IntervalMin">Model.IntervalMin</see> to <see cref="Model.IntervalMax">Model.IntervalMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task = model.IntervalVar(length: 10, name: "task");
        ///
        /// task.StartMax = 100;
        /// Console.WriteLine(task.StartMax);  // 100
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StartMin">IntervalVar.StartMin</see></description></item>
        /// <item><description><see cref="IntervalVar.EndMax">IntervalVar.EndMax</see></description></item>
        /// <item><description><see cref="IntervalVar.LengthMax">IntervalVar.LengthMax</see></description></item>
        /// </list>
        /// </remarks>
        public int StartMax
        {
            get => _props.TryGetValue("startMax", out var v) ? Convert.ToInt32(v) : Model.IntervalMax;
            set => _props["startMax"] = value;
        }

        /// <summary>
        /// The minimum end time of the interval variable.
        /// </summary>
        /// <remarks>
        /// Gets or sets the minimum end time of the interval variable.
        ///
        /// The initial value is set during construction by <see cref="Model.IntervalVar">Model.IntervalVar</see>.
        ///
        /// Note: This property reflects the interval's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range <see cref="Model.IntervalMin">Model.IntervalMin</see> to <see cref="Model.IntervalMax">Model.IntervalMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task = model.IntervalVar(length: 10, name: "task");
        ///
        /// Console.WriteLine(task.EndMin);  // 10 (StartMin + length)
        ///
        /// task.EndMin = 20;
        /// Console.WriteLine(task.EndMin);  // 20
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.EndMax">IntervalVar.EndMax</see></description></item>
        /// <item><description><see cref="IntervalVar.StartMin">IntervalVar.StartMin</see></description></item>
        /// <item><description><see cref="IntervalVar.LengthMin">IntervalVar.LengthMin</see></description></item>
        /// </list>
        /// </remarks>
        public int EndMin
        {
            get => _props.TryGetValue("endMin", out var v) ? Convert.ToInt32(v) : 0;
            set => _props["endMin"] = value;
        }

        /// <summary>
        /// The maximum end time of the interval variable.
        /// </summary>
        /// <remarks>
        /// Gets or sets the maximum end time of the interval variable.
        ///
        /// The initial value is set during construction by <see cref="Model.IntervalVar">Model.IntervalVar</see>.
        ///
        /// Note: This property reflects the interval's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range <see cref="Model.IntervalMin">Model.IntervalMin</see> to <see cref="Model.IntervalMax">Model.IntervalMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task = model.IntervalVar(length: 10, name: "task");
        ///
        /// task.EndMax = 100;
        /// Console.WriteLine(task.EndMax);  // 100
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.EndMin">IntervalVar.EndMin</see></description></item>
        /// <item><description><see cref="IntervalVar.StartMax">IntervalVar.StartMax</see></description></item>
        /// <item><description><see cref="IntervalVar.LengthMax">IntervalVar.LengthMax</see></description></item>
        /// </list>
        /// </remarks>
        public int EndMax
        {
            get => _props.TryGetValue("endMax", out var v) ? Convert.ToInt32(v) : Model.IntervalMax;
            set => _props["endMax"] = value;
        }

        /// <summary>
        /// The minimum length of the interval variable.
        /// </summary>
        /// <remarks>
        /// Gets or sets the minimum length of the interval variable.
        ///
        /// The initial value is set during construction by <see cref="Model.IntervalVar">Model.IntervalVar</see>.
        ///
        /// Note: This property reflects the interval's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range 0 to <see cref="Model.LengthMax">Model.LengthMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task = model.IntervalVar(length: 10, name: "task");
        ///
        /// Console.WriteLine(task.LengthMin);  // 10
        ///
        /// task.LengthMin = 5;
        /// Console.WriteLine(task.LengthMin);  // 5
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.LengthMax">IntervalVar.LengthMax</see></description></item>
        /// <item><description><see cref="IntervalVar.StartMin">IntervalVar.StartMin</see></description></item>
        /// <item><description><see cref="IntervalVar.EndMin">IntervalVar.EndMin</see></description></item>
        /// </list>
        /// </remarks>
        public int LengthMin
        {
            get => _props.TryGetValue("lengthMin", out var v) ? Convert.ToInt32(v) : 0;
            set => _props["lengthMin"] = value;
        }

        /// <summary>
        /// The maximum length of the interval variable.
        /// </summary>
        /// <remarks>
        /// Gets or sets the maximum length of the interval variable.
        ///
        /// The initial value is set during construction by <see cref="Model.IntervalVar">Model.IntervalVar</see>.
        ///
        /// Note: This property reflects the interval's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range 0 to <see cref="Model.LengthMax">Model.LengthMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task = model.IntervalVar(length: 10, name: "task");
        ///
        /// Console.WriteLine(task.LengthMax);  // 10
        ///
        /// task.LengthMax = 20;
        /// Console.WriteLine(task.LengthMax);  // 20
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.LengthMin">IntervalVar.LengthMin</see></description></item>
        /// <item><description><see cref="IntervalVar.StartMax">IntervalVar.StartMax</see></description></item>
        /// <item><description><see cref="IntervalVar.EndMax">IntervalVar.EndMax</see></description></item>
        /// </list>
        /// </remarks>
        public int LengthMax
        {
            get => _props.TryGetValue("lengthMax", out var v) ? Convert.ToInt32(v) : Model.LengthMax;
            set => _props["lengthMax"] = value;
        }

        /// <summary>
        /// Whether the interval variable is optional.
        /// </summary>
        /// <remarks>
        /// Gets or sets whether the interval variable is optional:
        ///
        /// - <c>True</c> / <see langword="true"/>: The interval is *optional* - the solver decides whether it is present or absent in the solution.
        /// - <c>False</c> / <see langword="false"/>: The interval is *present* - it must be scheduled in the solution.
        ///
        /// To force an interval to be absent, make it optional and constrain its presence
        /// to be false: <c>model.enforce(~x.presence())</c> (Python),
        /// <c>model.enforce(x.presence().not())</c> (TypeScript), or
        /// <c>model.Enforce(!x.Presence())</c> (C#).
        ///
        /// Note: This property reflects the presence status in the model
        /// (before the solve), not in the solution.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var task1 = model.IntervalVar(length: 10, name: "task1");
        /// var task2 = model.IntervalVar(length: 10, optional: true, name: "task2");
        ///
        /// Console.WriteLine(task1.Optional);  // False (present by default)
        /// Console.WriteLine(task2.Optional);  // True (optional)
        ///
        /// // Make task1 optional
        /// task1.Optional = true;
        /// Console.WriteLine(task1.Optional);  // True
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntervalVar.StartMin">IntervalVar.StartMin</see></description></item>
        /// <item><description><see cref="IntervalVar.EndMin">IntervalVar.EndMin</see></description></item>
        /// <item><description><see cref="IntervalVar.LengthMin">IntervalVar.LengthMin</see></description></item>
        /// </list>
        /// </remarks>
        public bool Optional
        {
            get => _props.TryGetValue("status", out var v) && Convert.ToInt32(v) == (int)PresenceStatus.Optional;
            set
            {
                if (value)
                    _props["status"] = (int)PresenceStatus.Optional;
                else
                    _props.Remove("status");
            }
        }

        internal static object Wrap(IntervalVar v) => v.AsArg();

        internal static List<object> WrapList(IEnumerable<IntervalVar> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }

        /// <summary>
        /// Creates a Boolean expression which is true if the interval variable is present.
        /// </summary>
        /// <returns>A Boolean expression that is true if the interval variable is present in the solution.</returns>
        /// <remarks>
        /// The resulting expression is never *absent*: it is <see langword="true"/> if the interval variable is *present* and <see langword="false"/> if the interval variable is *absent*.
        ///
        /// This function is the same as <see cref="Model.Presence">Model.Presence</see>, see its documentation for more details.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, interval variables <c>x</c> and <c>y</c> must have the same presence status.
        /// I.e. they must either be both *present* or both *absent*.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x", optional: true, length: 10);
        /// var y = model.IntervalVar(name: "y", optional: true, length: 10);
        /// model.Enforce(x.Presence() == y.Presence());
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Presence">Model.Presence</see> — is the equivalent function on <see cref="Model">Model</see>.</description></item>
        /// </list>
        /// </remarks>
        public BoolExpr Presence()
        {
            var outParams = new List<object?>() { AsArg() };
            return new BoolExpr(_model, "intervalPresenceOf", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the start time of the interval variable.
        /// </summary>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the interval variable is absent, then the resulting expression is also absent.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we constrain interval variable <c>y</c> to start after the end of <c>x</c> with a delay of at least 10. In addition, we constrain the length of <c>x</c> to be less or equal to the length of <c>y</c>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x");
        /// var y = model.IntervalVar(name: "y");
        /// model.Enforce(x.End() + 10 &lt;= y.Start());
        /// model.Enforce(x.Length() &lt;= y.Length());
        /// </code>
        ///
        /// When <c>x</c> or <c>y</c> is *absent* then value of both constraints above is *absent* and therefore they are satisfied.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Start">Model.Start</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Start()
        {
            var outParams = new List<object?>() { AsArg() };
            return new IntExpr(_model, "startOf", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the end time of the interval variable.
        /// </summary>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the interval variable is absent, then the resulting expression is also absent.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we constrain interval variable <c>y</c> to start after the end of <c>x</c> with a delay of at least 10. In addition, we constrain the length of <c>x</c> to be less or equal to the length of <c>y</c>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x");
        /// var y = model.IntervalVar(name: "y");
        /// model.Enforce(x.End() + 10 &lt;= y.Start());
        /// model.Enforce(x.Length() &lt;= y.Length());
        /// </code>
        ///
        /// When <c>x</c> or <c>y</c> is *absent* then value of both constraints above is *absent* and therefore they are satisfied.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.End">Model.End</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr End()
        {
            var outParams = new List<object?>() { AsArg() };
            return new IntExpr(_model, "endOf", outParams);
        }

        /// <summary>
        /// Creates an integer expression for the duration (end - start) of the interval variable.
        /// </summary>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the interval variable is absent, then the resulting expression is also absent.
        ///
        /// <para>Example:</para>
        /// <example>
        /// In the following example, we constrain interval variable <c>y</c> to start after the end of <c>x</c> with a delay of at least 10. In addition, we constrain the length of <c>x</c> to be less or equal to the length of <c>y</c>.
        ///
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(name: "x");
        /// var y = model.IntervalVar(name: "y");
        /// model.Enforce(x.End() + 10 &lt;= y.Start());
        /// model.Enforce(x.Length() &lt;= y.Length());
        /// </code>
        ///
        /// When <c>x</c> or <c>y</c> is *absent* then value of both constraints above is *absent* and therefore they are satisfied.
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Length">Model.Length</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Length()
        {
            var outParams = new List<object?>() { AsArg() };
            return new IntExpr(_model, "lengthOf", outParams);
        }

        /// <summary>
        /// Creates a _alternativeCost expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr AlternativeCost(IEnumerable<IntervalVar> options, IEnumerable<int> weights)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.WrapList(options), ModelElement.WrapIntList(weights) };
            return new IntExpr(_model, "intAlternativeCost", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay &lt;= successor.End())
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be less than or equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.EndBeforeEnd">Model.EndBeforeEnd</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndBeforeEnd(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "endBeforeEnd", outParams);
        }

        /// <inheritdoc cref="EndBeforeEnd(IntervalVar, IntExpr)"/>
        public Constraint EndBeforeEnd(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "endBeforeEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay &lt;= successor.Start())
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be less than or equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.EndBeforeStart">Model.EndBeforeStart</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndBeforeStart(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "endBeforeStart", outParams);
        }

        /// <inheritdoc cref="EndBeforeStart(IntervalVar, IntExpr)"/>
        public Constraint EndBeforeStart(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "endBeforeStart", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay &lt;= successor.End())
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be less than or equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.StartBeforeEnd">Model.StartBeforeEnd</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartBeforeEnd(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "startBeforeEnd", outParams);
        }

        /// <inheritdoc cref="StartBeforeEnd(IntervalVar, IntExpr)"/>
        public Constraint StartBeforeEnd(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "startBeforeEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay &lt;= successor.Start())
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be less than or equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.StartBeforeStart">Model.StartBeforeStart</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartBeforeStart(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "startBeforeStart", outParams);
        }

        /// <inheritdoc cref="StartBeforeStart(IntervalVar, IntExpr)"/>
        public Constraint StartBeforeStart(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "startBeforeStart", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay == successor.End())
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.EndAtEnd">Model.EndAtEnd</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndAtEnd(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "endAtEnd", outParams);
        }

        /// <inheritdoc cref="EndAtEnd(IntervalVar, IntExpr)"/>
        public Constraint EndAtEnd(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "endAtEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.End() + delay == successor.Start())
        /// </code>
        ///
        /// In other words, end of <c>predecessor</c> plus <c>delay</c> must be equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.EndAtStart">Model.EndAtStart</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint EndAtStart(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "endAtStart", outParams);
        }

        /// <inheritdoc cref="EndAtStart(IntervalVar, IntExpr)"/>
        public Constraint EndAtStart(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "endAtStart", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay == successor.End())
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be equal to end of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.StartAtEnd">Model.StartAtEnd</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartAtEnd(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "startAtEnd", outParams);
        }

        /// <inheritdoc cref="StartAtEnd(IntervalVar, IntExpr)"/>
        public Constraint StartAtEnd(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "startAtEnd", outParams);
        }

        /// <summary>
        /// Creates a precedence constraint between two interval variables.
        /// </summary>
        /// <param name="successor">The successor interval variable.</param>
        /// <param name="delay">The minimum delay between intervals.</param>
        /// <returns>The precedence constraint.</returns>
        /// <remarks>
        /// Assuming that the current interval is <c>predecessor</c>, the constraint is the same as:
        ///
        /// <code>
        /// model.Enforce(predecessor.Start() + delay == successor.Start())
        /// </code>
        ///
        /// In other words, start of <c>predecessor</c> plus <c>delay</c> must be equal to start of <c>successor</c>.
        ///
        /// When one of the two interval variables is absent, then the constraint is satisfied.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.StartAtStart">Model.StartAtStart</see> — is equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="IntervalVar.Start">IntervalVar.Start</see></description></item>
        /// <item><description><see cref="IntervalVar.End">IntervalVar.End</see></description></item>
        /// </list>
        /// </remarks>
        public Constraint StartAtStart(IntervalVar successor, IntExpr delay)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), OptalCP.IntExpr.Wrap(delay) };
            return new Constraint(_model, "startAtStart", outParams);
        }

        /// <inheritdoc cref="StartAtStart(IntervalVar, IntExpr)"/>
        public Constraint StartAtStart(IntervalVar successor, int delay = 0)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(successor), delay };
            return new Constraint(_model, "startAtStart", outParams);
        }

        /// <summary>
        /// Creates alternative constraints for the interval variable and provided <c>options</c>.
        /// </summary>
        /// <param name="options">The interval variables to choose from.</param>
        /// <returns>The alternative constraint.</returns>
        /// <remarks>
        /// The alternative constraint requires that exactly one of the <c>options</c> intervals
        /// is present when <c>self</c> is present. The selected option must have the same
        /// start, end, and length as <c>self</c>. If <c>self</c> is absent, all options must be absent.
        ///
        /// This is useful for modeling choices, such as assigning a task to one of several
        /// machines, where each option represents the task executed on a different machine.
        ///
        /// This constraint is equivalent to <see cref="Model.Alternative">Model.Alternative</see> with <c>self</c> as the main interval.
        /// </remarks>
        public Constraint Alternative(IEnumerable<IntervalVar> options)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.WrapList(options) };
            return new Constraint(_model, "alternative", outParams);
        }

        /// <summary>
        /// Constrains the interval variable to span (cover) a set of other interval variables.
        /// </summary>
        /// <param name="covered">The set of interval variables to cover.</param>
        /// <returns>The span constraint.</returns>
        /// <remarks>
        /// The span constraint ensures that <c>self</c> exactly covers all present intervals
        /// in <c>covered</c>. Specifically, <c>self</c> starts at the minimum start time and ends
        /// at the maximum end time of the present covered intervals. If all covered
        /// intervals are absent, <c>self</c> must also be absent.
        ///
        /// This is useful for modeling composite tasks or projects where a parent interval
        /// represents the overall duration of multiple sub-tasks.
        ///
        /// This constraint is equivalent to <see cref="Model.Span">Model.Span</see> with <c>self</c> as the spanning interval.
        /// </remarks>
        public Constraint Span(IEnumerable<IntervalVar> covered)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.WrapList(covered) };
            return new Constraint(_model, "span", outParams);
        }

        /// <summary>
        /// Creates an expression equal to the position of the interval on the sequence.
        /// </summary>
        /// <param name="sequence">The sequence variable.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// Returns an integer expression representing the 0-based position of this interval
        /// in the given sequence. The position reflects the order in which intervals appear
        /// after applying the <see cref="SequenceVar.NoOverlap">SequenceVar.NoOverlap</see> constraint.
        ///
        /// If this interval is absent, the position expression is also absent.
        ///
        /// This method is equivalent to <see cref="Model.Position">Model.Position</see> with <c>self</c> as the interval.
        /// </remarks>
        public IntExpr Position(SequenceVar sequence)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.SequenceVar.Wrap(sequence) };
            return new IntExpr(_model, "position", outParams);
        }

        /// <summary>
        /// Creates cumulative function (expression) pulse for the interval variable and specified height.
        /// </summary>
        /// <param name="height">The height value.</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// Limitation: The <c>height</c> must be non-negative. Pulses with negative height are not supported.
        ///
        /// This function is the same as <see cref="Model.Pulse">Model.Pulse</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var task = model.IntervalVar(name: "task", length: 10);
        /// var pulse = task.Pulse(5);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Pulse">Model.Pulse</see> — for detailed documentation and examples.</description></item>
        /// </list>
        /// </remarks>
        public CumulExpr Pulse(IntExpr height)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(height) };
            return new CumulExpr(_model, "pulse", outParams);
        }

        /// <inheritdoc cref="Pulse(IntExpr)"/>
        public CumulExpr Pulse(int height)
        {
            var outParams = new List<object?>() { AsArg(), height };
            return new CumulExpr(_model, "pulse", outParams);
        }

        /// <summary>
        /// Creates cumulative function (expression) that changes value at start of the interval variable by the given height.
        /// </summary>
        /// <param name="height">The height value.</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// Creates cumulative function (expression) that changes value at start of the interval variable by the given height.
        ///
        /// This function is the same as <see cref="Model.StepAtStart">Model.StepAtStart</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var task = model.IntervalVar(name: "task", length: 10);
        /// var step = task.StepAtStart(5);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.StepAtStart">Model.StepAtStart</see> — for detailed documentation.</description></item>
        /// <item><description><see cref="IntervalVar.StepAtEnd">IntervalVar.StepAtEnd</see> — for the opposite function.</description></item>
        /// </list>
        /// </remarks>
        public CumulExpr StepAtStart(IntExpr height)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(height) };
            return new CumulExpr(_model, "stepAtStart", outParams);
        }

        /// <inheritdoc cref="StepAtStart(IntExpr)"/>
        public CumulExpr StepAtStart(int height)
        {
            var outParams = new List<object?>() { AsArg(), height };
            return new CumulExpr(_model, "stepAtStart", outParams);
        }

        /// <summary>
        /// Creates cumulative function (expression) that changes value at end of the interval variable by the given height.
        /// </summary>
        /// <param name="height">The height value.</param>
        /// <returns>The resulting cumulative expression</returns>
        /// <remarks>
        /// Creates cumulative function (expression) that changes value at end of the interval variable by the given height.
        ///
        /// This function is the same as <see cref="Model.StepAtEnd">Model.StepAtEnd</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var task = model.IntervalVar(name: "task", length: 10);
        /// var step = task.StepAtEnd(5);
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.StepAtEnd">Model.StepAtEnd</see> — for detailed documentation.</description></item>
        /// <item><description><see cref="IntervalVar.StepAtStart">IntervalVar.StepAtStart</see> — for the opposite function.</description></item>
        /// </list>
        /// </remarks>
        public CumulExpr StepAtEnd(IntExpr height)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(height) };
            return new CumulExpr(_model, "stepAtEnd", outParams);
        }

        /// <inheritdoc cref="StepAtEnd(IntExpr)"/>
        public CumulExpr StepAtEnd(int height)
        {
            var outParams = new List<object?>() { AsArg(), height };
            return new CumulExpr(_model, "stepAtEnd", outParams);
        }

        /// <summary>
        /// Creates a _precedenceEnergy expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint PrecedenceEnergyBefore(IEnumerable<IntervalVar> others, IEnumerable<int> heights, int capacity)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.WrapList(others), ModelElement.WrapIntList(heights), capacity };
            return new Constraint(_model, "precedenceEnergyBefore", outParams);
        }

        /// <summary>
        /// Creates a _precedenceEnergy expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint PrecedenceEnergyAfter(IEnumerable<IntervalVar> others, IEnumerable<int> heights, int capacity)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.WrapList(others), ModelElement.WrapIntList(heights), capacity };
            return new Constraint(_model, "precedenceEnergyAfter", outParams);
        }

        /// <summary>
        /// This function prevents the specified interval variable from overlapping with segments of the step function where the value is zero.
        /// </summary>
        /// <param name="func">The step function.</param>
        /// <returns>The constraint forbidding the extent (entire interval).</returns>
        /// <remarks>
        /// This function prevents the specified interval variable from overlapping with segments of the step function where the value is zero. I.e., if $[s, e)$ is a segment of the step function where the value is zero, then the interval variable either ends before $s$ ($\mathtt{interval.end()} \le s$) or starts after $e$ ($e \le \mathtt{interval.start()}$).
        ///
        /// @example A production task that cannot overlap with scheduled maintenance windows:
        ///
        /// <code>
        /// var model = new Model();
        ///
        /// // A 3-hour production run
        /// var production = model.IntervalVar(length: 3, name: "production");
        ///
        /// // Machine availability: 1 = available, 0 = under maintenance
        /// var availability = model.StepFunction(new[] {
        ///     (0, 1),   // Initially available
        ///     (8, 0),   // 8h: maintenance starts
        ///     (10, 1),  // 10h: maintenance ends
        /// });
        ///
        /// // Production cannot overlap periods where availability is 0
        /// production.ForbidExtent(availability);
        /// model.Minimize(production.End());
        ///
        /// var result = model.Solve();
        /// // Production runs [0, 3) - finishes before maintenance window
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ForbidExtent">Model.ForbidExtent</see> — for the equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="Model.ForbidStart">Model.ForbidStart</see> — for similar functions that constrain the start/end of an interval variable.</description></item>
        /// <item><description><see cref="Model.ForbidEnd">Model.ForbidEnd</see> — for similar functions that constrain the start/end of an interval variable.</description></item>
        /// <item><description><see cref="Model.Eval">Model.Eval</see> — for evaluation of a step function.</description></item>
        /// </list>
        /// </remarks>
        public Constraint ForbidExtent(IntStepFunction func)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntStepFunction.Wrap(func) };
            return new Constraint(_model, "forbidExtent", outParams);
        }

        /// <summary>
        /// Constrains the start of an interval variable to not coincide with zero segments of a step function.
        /// </summary>
        /// <param name="func">The step function whose zero segments define forbidden start times.</param>
        /// <returns>The constraint forbidding the start point.</returns>
        /// <remarks>
        /// This function is equivalent to:
        ///
        /// <code>
        ///   model.Enforce(func.Eval(interval.Start()) != 0);
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
        /// task.ForbidStart(allowedStarts);
        /// model.Minimize(task.Start());
        ///
        /// var result = model.Solve();
        /// // Task starts at 6 (earliest allowed start time)
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ForbidStart">Model.ForbidStart</see> — for the equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="Model.ForbidEnd">Model.ForbidEnd</see> — for similar function that constrains end an interval variable.</description></item>
        /// <item><description><see cref="Model.Eval">Model.Eval</see> — for evaluation of a step function.</description></item>
        /// </list>
        /// </remarks>
        public Constraint ForbidStart(IntStepFunction func)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntStepFunction.Wrap(func) };
            return new Constraint(_model, "forbidStart", outParams);
        }

        /// <summary>
        /// Constrains the end of an interval variable to not coincide with zero segments of a step function.
        /// </summary>
        /// <param name="func">The step function whose zero segments define forbidden end times.</param>
        /// <returns>The constraint forbidding the end point.</returns>
        /// <remarks>
        /// This function is equivalent to:
        ///
        /// <code>
        ///   model.Enforce(func.Eval(interval.End()) != 0);
        /// </code>
        ///
        /// I.e., the function value at the end of the interval variable cannot be zero.
        ///
        /// @example A delivery task that must complete during business hours (not during lunch break):
        ///
        /// <code>
        /// var model = new Model();
        ///
        /// // A 1-hour delivery task
        /// var delivery = model.IntervalVar(length: 1, name: "delivery");
        ///
        /// // Allowed end times: 1 = allowed, 0 = forbidden
        /// var allowedEnds = model.StepFunction(new[] {
        ///     (0, 0),   // Before 9h: forbidden
        ///     (9, 1),   // 9h: business opens, allowed
        ///     (12, 0),  // 12h: lunch break, forbidden
        ///     (13, 1),  // 13h: lunch ends, allowed
        ///     (17, 0),  // 17h: business closes, forbidden
        /// });
        ///
        /// // Delivery cannot end when allowedEnds is 0
        /// delivery.ForbidEnd(allowedEnds);
        /// model.Minimize(delivery.End());
        ///
        /// var result = model.Solve();
        /// // Delivery ends at 9 (starts at 8, ends at earliest allowed time)
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.ForbidEnd">Model.ForbidEnd</see> — for the equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="Model.ForbidStart">Model.ForbidStart</see> — for similar function that constrains start an interval variable.</description></item>
        /// <item><description><see cref="Model.Eval">Model.Eval</see> — for evaluation of a step function.</description></item>
        /// </list>
        /// </remarks>
        public Constraint ForbidEnd(IntStepFunction func)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntStepFunction.Wrap(func) };
            return new Constraint(_model, "forbidEnd", outParams);
        }

        /// <summary>
        /// Creates a _disjunctiveIsBefore expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr DisjunctiveIsBefore(IntervalVar y)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(y) };
            return new BoolExpr(_model, "disjunctiveIsBefore", outParams);
        }

        /// <summary>
        /// Creates a _related expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Directive Related(IntervalVar y)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(y) };
            return new Directive(_model, "related", outParams);
        }


    }

    /// <summary>
    /// Ordered sequence of intervals for routing and sequencing problems.
    /// </summary>
    /// <remarks>
    /// Models a sequence (order) of interval variables.
    ///
    /// A sequence variable represents an ordered arrangement of interval variables
    /// where no two intervals overlap. The sequence captures not just that the intervals
    /// don't overlap, but also their relative ordering in the solution.
    ///
    /// Sequence variables are created using <see cref="Model.SequenceVar">Model.SequenceVar</see> and are typically
    /// used with the <see cref="SequenceVar.NoOverlap">SequenceVar.NoOverlap</see> constraint to enforce non-overlapping
    /// with optional transition times between intervals.
    ///
    /// The position of each interval in the sequence can be queried using
    /// <see cref="Model.Position">Model.Position</see> or <see cref="IntervalVar.Position">IntervalVar.Position</see>, which returns an integer
    /// expression representing the interval's index in the final ordering (0-based).
    /// Absent intervals have an absent position.
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="Model.SequenceVar">Model.SequenceVar</see> — to create sequence variables.</description></item>
    /// <item><description><see cref="SequenceVar.NoOverlap">SequenceVar.NoOverlap</see> — for the no-overlap constraint with transitions.</description></item>
    /// <item><description><see cref="Model.Position">Model.Position</see> — to get an interval's position in the sequence.</description></item>
    /// </list>
    /// </remarks>
    public class SequenceVar : ModelElement
    {
        internal SequenceVar(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args)
        {
            EnsureRef();
        }

        internal SequenceVar(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId)
        {
            EnsureRef();
        }

        internal void MakeAuxiliary() => _props["func"] = "_sequenceVar";
        internal bool IsAuxiliary => _props.TryGetValue("func", out var f) && f?.ToString() == "_sequenceVar";

        internal static object Wrap(SequenceVar v) => v.AsArg();

        internal static List<object> WrapList(IEnumerable<SequenceVar> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }

        /// <summary>
        /// Constrain the interval variables forming the sequence to not overlap.
        /// </summary>
        /// <param name="transitions">2D square array of minimum transition distances between the intervals. The first index is the type (index) of the first interval in the sequence, the second index is the type (index) of the second interval in the sequence</param>
        /// <returns>The no-overlap constraint.</returns>
        /// <remarks>
        /// The <c>noOverlap</c> constraint makes sure that the intervals in the sequence
        /// do not overlap.  That is, for every pair of interval variables <c>x</c> and <c>y</c>
        /// at least one of the following conditions must hold (in a solution):
        ///
        /// 1. Interval variable <c>x</c> is *absent*. This means that the interval is not
        /// present in the solution (not performed), so it cannot overlap
        /// with any other interval. Only optional interval variables can be *absent*.
        /// 2. Interval variable <c>y</c> is *absent*.
        /// 3. <c>x</c> ends before <c>y</c> starts, i.e. <c>x.end()</c> is less or equal to <c>y.start()</c>.
        /// 4. <c>y</c> ends before <c>x</c> starts, i.e. <c>y.end()</c> is less or equal to <c>x.start()</c>.
        ///
        /// In addition, if the <c>transitions</c> parameter is specified, then the cases 3 and 4
        /// are further constrained by the minimum transition distance between the
        /// intervals:
        ///
        /// 3. <c>x.end() + transitions[x.type][y.type]</c> is less or equal to <c>y.start()</c>.
        /// 4. <c>y.end() + transitions[y.type][x.type]</c> is less or equal to <c>x.start()</c>.
        ///
        /// where <c>x.type</c> and <c>y.type</c> are the types of the interval variables <c>x</c> and <c>y</c>
        /// as given in <see cref="Model.SequenceVar">Model.SequenceVar</see>. If types were not specified,
        /// then they are equal to the indices of the interval variables in the array
        /// passed to <see cref="Model.SequenceVar">Model.SequenceVar</see>. Transition times
        /// cannot be negative.
        ///
        /// Note that transition times are enforced between every pair of interval variables,
        /// not only between direct neighbors.
        ///
        /// The size of the 2D array <c>transitions</c> must be equal to the number of types
        /// of the interval variables.
        ///
        /// This constraint is equivalent to <see cref="Model.NoOverlap">Model.NoOverlap</see> called with the
        /// sequence variable's intervals and types.
        ///
        /// <para>Example:</para>
        /// <example>
        /// A worker must perform a set of tasks. Each task is characterized by:
        ///
        /// * <c>length</c> of the task (how long it takes to perform it),
        /// * <c>location</c> of the task (where it must be performed),
        /// * a time window <c>earliest</c> to <c>deadline</c> when the task must be performed.
        ///
        /// There are three locations, <c>0</c>, <c>1</c>, and <c>2</c>. The minimum travel times between
        /// the locations are given by a transition matrix <c>transitions</c>. Transition times
        /// are not symmetric. For example, it takes 10 minutes to travel from location <c>0</c>
        /// to location <c>1</c> but 15 minutes to travel back from location <c>1</c> to location <c>0</c>.
        ///
        /// We will model this problem using <c>noOverlap</c> constraint with transition times.
        ///
        /// <code>
        /// // Travel times between locations:
        /// int[][] transitions = {
        ///     new[] { 0, 10, 10 },
        ///     new[] { 15,  0, 10 },
        ///     new[] {  5,  5,  0 },
        /// };
        /// // Tasks to be scheduled:
        /// var tasks = new[] {
        ///     (location: 0, length: 20, earliest: 0, deadline: 100),
        ///     (location: 0, length: 40, earliest: 70, deadline: 200),
        ///     (location: 1, length: 10, earliest: 0, deadline: 200),
        ///     (location: 1, length: 30, earliest: 100, deadline: 200),
        ///     (location: 1, length: 10, earliest: 0, deadline: 150),
        ///     (location: 2, length: 15, earliest: 50, deadline: 250),
        ///     (location: 2, length: 10, earliest: 20, deadline: 60),
        ///     (location: 2, length: 20, earliest: 110, deadline: 250),
        /// };
        ///
        /// var model = new Model();
        ///
        /// var taskVars = tasks.Select((t, i) =&gt; model.IntervalVar(
        ///     name: $"Task{i}", length: t.length,
        ///     start: (t.earliest, null), end: (null, t.deadline)
        /// )).ToArray();
        /// var types = tasks.Select(t =&gt; t.location).ToArray();
        ///
        /// // Create the sequence variable for the tasks, location is the type:
        /// var sequence = model.SequenceVar(taskVars, types);
        /// // Tasks must not overlap and transitions must be respected:
        /// sequence.NoOverlap(transitions);
        ///
        /// // Solve the model:
        /// var result = model.Solve(new Parameters { solutionLimit = 1 });
        /// </code>
        /// </example>
        /// </remarks>
        public Constraint NoOverlap(IEnumerable<IEnumerable<int>>? transitions = null)
        {
            var args = new List<object?> { AsArg() };
            if (transitions != null)
                args.Add(WrapIntMatrix(transitions));
            return new Constraint(_model, "noOverlap", args);
        }

        /// <summary>
        /// Creates a _sameSequence expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint SameSequence(SequenceVar sequence2)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.SequenceVar.Wrap(sequence2) };
            return new Constraint(_model, "sameSequence", outParams);
        }


    }

    /// <summary>
    /// Piecewise-constant function mapping time to integers, used for costs and availability windows.
    /// </summary>
    /// <remarks>
    /// Integer step function.
    ///
    /// Integer step function is a piecewise constant function defined on integer
    /// values in range <see cref="Model.IntVarMin">Model.IntVarMin</see> to <see cref="Model.IntVarMax">Model.IntVarMax</see>. The function can be
    /// created by <see cref="Model.StepFunction">Model.StepFunction</see>.
    ///
    /// Step functions can be used in the following ways:
    ///
    /// * Function <see cref="Model.Eval">Model.Eval</see> evaluates the function at the given point (given as <see cref="IntExpr">IntExpr</see>).
    /// * Function <see cref="Model.Integral">Model.Integral</see> computes a sum (integral) of the function over an <see cref="IntervalVar">IntervalVar</see>.
    /// * Constraints <see cref="Model.ForbidStart">Model.ForbidStart</see> and <see cref="Model.ForbidEnd">Model.ForbidEnd</see> forbid the start/end of an <see cref="IntervalVar">IntervalVar</see> to be in a zero-value interval of the function.
    /// * Constraint <see cref="Model.ForbidExtent">Model.ForbidExtent</see> forbids the extent of an <see cref="IntervalVar">IntervalVar</see> to be in a zero-value interval of the function.
    /// </remarks>
    public class IntStepFunction : ModelElement
    {
        internal IntStepFunction(Model model, IEnumerable<(int, int)> values)
            : base(model, "intStepFunction", null)
        {
            var valueList = new List<List<object>>();
            foreach (var (x, y) in values)
                valueList.Add(new List<object> { x, y });
            _props["values"] = valueList;
            EnsureRef();
        }

        internal IntStepFunction(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId)
        {
            EnsureRef();
        }

        internal static object Wrap(IntStepFunction f) => f.AsArg();

        /// <summary>
        /// Computes sum of values of the step function over the interval <c>interval</c>.
        /// </summary>
        /// <param name="interval">The interval variable.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// The sum is computed over all integer time points from <c>interval.start()</c> to <c>interval.end()-1</c> inclusive. In other words, the sum includes the function value at the start time but excludes the value at the end time (half-open interval). If the interval variable has zero length, then the result is 0. If the interval variable is absent, then the result is *absent*.
        ///
        /// Requirement: The step function <c>func</c> must be non-negative.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Integral">Model.Integral</see> — for the equivalent function on <see cref="Model">Model</see>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Integral(IntervalVar interval)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(interval) };
            return new IntExpr(_model, "intStepFunctionIntegral", outParams);
        }

        /// <summary>
        /// Creates a _stepFunctionIntegralInRange expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionIntegralInRange(IntervalVar interval, int lb, int ub)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(interval), lb, ub };
            return new Constraint(_model, "intStepFunctionIntegralInRange", outParams);
        }

        /// <summary>
        /// Evaluates the step function at a given point.
        /// </summary>
        /// <param name="arg">The point at which to evaluate the step function.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// The result is the value of the step function at the point <c>arg</c>. If the value of <c>arg</c> is <c>absent</c>, then the result is also <c>absent</c>.
        ///
        /// By constraining the returned value, it is possible to limit <c>arg</c> to be only within certain segments of the segmented function. In particular, functions <see cref="Model.ForbidStart">Model.ForbidStart</see> and <see cref="Model.ForbidEnd">Model.ForbidEnd</see> work that way.
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Eval">Model.Eval</see> — for the equivalent function on <see cref="Model">Model</see>.</description></item>
        /// <item><description><see cref="Model.ForbidStart">Model.ForbidStart</see> — are convenience functions built on top of <c>eval</c>.</description></item>
        /// <item><description><see cref="Model.ForbidEnd">Model.ForbidEnd</see> — are convenience functions built on top of <c>eval</c>.</description></item>
        /// </list>
        /// </remarks>
        public IntExpr Eval(IntExpr arg)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(arg) };
            return new IntExpr(_model, "intStepFunctionEval", outParams);
        }

        /// <inheritdoc cref="Eval(IntExpr)"/>
        public IntExpr Eval(int arg)
        {
            var outParams = new List<object?>() { AsArg(), arg };
            return new IntExpr(_model, "intStepFunctionEval", outParams);
        }

        /// <summary>
        /// Creates a _stepFunctionEvalInRange expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalInRange(IntExpr arg, int lb, int ub)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(arg), lb, ub };
            return new Constraint(_model, "intStepFunctionEvalInRange", outParams);
        }

        /// <summary>
        /// Creates a _stepFunctionEvalInRange expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalInRange(int arg, int lb, int ub)
        {
            var outParams = new List<object?>() { AsArg(), arg, lb, ub };
            return new Constraint(_model, "intStepFunctionEvalInRange", outParams);
        }

        /// <summary>
        /// Creates a _stepFunctionEvalNotInRange expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalNotInRange(IntExpr arg, int lb, int ub)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(arg), lb, ub };
            return new Constraint(_model, "intStepFunctionEvalNotInRange", outParams);
        }

        /// <summary>
        /// Creates a _stepFunctionEvalNotInRange expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint StepFunctionEvalNotInRange(int arg, int lb, int ub)
        {
            var outParams = new List<object?>() { AsArg(), arg, lb, ub };
            return new Constraint(_model, "intStepFunctionEvalNotInRange", outParams);
        }


    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class StateFunction : ModelElement
    {
        internal StateFunction(Model model, IEnumerable<IEnumerable<int>>? transitions = null)
            : base(model, "stateFunction", transitions != null
                ? new object?[] { WrapIntMatrix(transitions) } : null)
        {
            EnsureRef();
        }

        internal StateFunction(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId)
        {
            EnsureRef();
        }

        internal static object Wrap(StateFunction f) => f.AsArg();

        // Generated methods
        /// <summary>
        /// Creates a _alwaysEqual expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint AlwaysEqual(IntervalVar interval, int value)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntervalVar.Wrap(interval), value };
            return new Constraint(_model, "alwaysEqual", outParams);
        }


    }
}
