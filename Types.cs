// ===========================================================================
// OptalCP C# API - Core Types
// Version: 2026.4.0
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptalCP
{
    /// <summary>
    /// Presence status for optional variables.
    /// Values must match C++ enum Presence::Value.
    /// </summary>
    internal enum PresenceStatus
    {
        /// <summary>
        /// Variable is optional (may be present or absent).
        /// </summary>
        Optional = 0,
        /// <summary>
        /// Variable is always present.
        /// </summary>
        Present = 1,
        /// <summary>
        /// Variable is always absent.
        /// </summary>
        Absent = 2
    }

    /// <summary>
    /// Represents an integer bound that can be a fixed value or a range (min, max).
    /// Supports implicit conversion from int, (int, int), and (int?, int?) tuples.
    /// </summary>
    public readonly struct IntBound
    {
        /// <summary>The minimum value of the bound (null if unbounded).</summary>
        public int? Min { get; }

        /// <summary>The maximum value of the bound (null if unbounded).</summary>
        public int? Max { get; }

        /// <summary>Creates a new IntBound with explicit min and max values.</summary>
        public IntBound(int? min, int? max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>Creates a new IntBound with a fixed value (min == max).</summary>
        public IntBound(int value)
        {
            Min = value;
            Max = value;
        }

        /// <summary>Implicit conversion from a single int (creates fixed value bound).</summary>
        public static implicit operator IntBound(int value) => new IntBound(value);

        /// <summary>Implicit conversion from (int, int) tuple (creates range bound).</summary>
        public static implicit operator IntBound((int min, int max) tuple) => new IntBound(tuple.min, tuple.max);

        /// <summary>Implicit conversion from (int?, int?) tuple.</summary>
        public static implicit operator IntBound((int? min, int? max) tuple) => new IntBound(tuple.min, tuple.max);

        /// <summary>Implicit conversion from (int, int?) tuple.</summary>
        public static implicit operator IntBound((int min, int? max) tuple) => new IntBound(tuple.min, tuple.max);

        /// <summary>Implicit conversion from (int?, int) tuple.</summary>
        public static implicit operator IntBound((int? min, int max) tuple) => new IntBound(tuple.min, tuple.max);

        /// <summary>Returns true if this is a fixed value (min == max).</summary>
        public bool IsFixed => Min.HasValue && Max.HasValue && Min.Value == Max.Value;

        /// <inheritdoc />
        public override string ToString()
        {
            if (IsFixed)
                return Min.ToString()!;
            return $"({Min?.ToString() ?? "null"}, {Max?.ToString() ?? "null"})";
        }
    }

    /// <summary>
    /// The base class for all modeling objects.
    /// </summary>
    /// <remarks>
    /// The base class for all modeling objects.
    ///
    /// <para>Example:</para>
    /// <example>
    /// <code>
    /// var model = new Model();
    /// var x = model.IntervalVar(length: 10, name: "x");
    /// var start = model.Start(x);
    /// var result = model.Solve();
    /// </code>
    ///
    /// Interval variable <c>x</c> and expression <c>start</c> are both instances of <see cref="ModelElement">ModelElement</see>.
    /// There are specialized descendant classes such as <see cref="IntervalVar">IntervalVar</see> and <see cref="IntExpr">IntExpr</see>.
    ///
    /// Any modeling object can be assigned a name using the <see cref="ModelElement.Name">ModelElement.Name</see> property.
    /// </example>
    /// </remarks>
    public abstract class ModelElement
    {
        private protected readonly Model _model;
        private protected readonly Dictionary<string, object?> _props;
        private protected int? _refId;
        private Dictionary<string, object?>? _arg;

        internal ModelElement(Model model, string funcName, IEnumerable<object?>? args = null)
        {
            _model = model;
            _props = new Dictionary<string, object?>
            {
                ["func"] = funcName,
                ["args"] = args != null ? new List<object?>(args) : new List<object?>()
            };
        }

        internal ModelElement(Model model, Dictionary<string, object?> props, int? refId = null)
        {
            _model = model;
            _props = props;
            _refId = refId;
        }

        /// <summary>
        /// Gets the model this element belongs to.
        /// </summary>
        internal Model Model => _model;

        /// <summary>
        /// The name assigned to this model element.
        /// </summary>
        /// <remarks>
        /// The name is optional and primarily useful for debugging purposes. When set,
        /// it helps identify the element in solver logs, error messages, and when
        /// inspecting solutions.
        ///
        /// Names can be assigned to any model element including variables, expressions,
        /// and constraints.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        ///
        /// // Name a variable at creation
        /// var task = model.IntervalVar(length: 10, name: "assembly");
        ///
        /// // Or set name later
        /// var x = model.IntVar(min: 0, max: 100);
        /// x.Name = "quantity";
        ///
        /// Console.WriteLine(task.Name);  // "assembly"
        /// Console.WriteLine(x.Name);     // "quantity"
        /// </code>
        /// </example>
        /// </remarks>
        public string? Name
        {
            get => _props.TryGetValue("name", out var n) ? n as string : null;
            set => _props["name"] = value;
        }

        internal object AsArg()
        {
            if (_refId.HasValue)
                return new Dictionary<string, object> { ["ref"] = _refId.Value };
            if (_arg == null)
            {
                // First use: inline the element
                _arg = new Dictionary<string, object?> { ["arg"] = _props };
            }
            else if (!_arg.ContainsKey("ref"))
            {
                // Second use: promote to shared reference.
                // Mutate in-place so that the first consumer (which already holds
                // a reference to this dict) also sees the update.
                _refId = _model.GetNewRefId(_props);
                _arg.Remove("arg");
                _arg["ref"] = _refId.Value;
            }
            return _arg;
        }

        internal Dictionary<string, object?> GetProps() => _props;

        internal void EnsureRef()
        {
            if (!_refId.HasValue)
                _refId = _model.GetNewRefId(_props);
        }

        /// <summary>
        /// Gets the reference ID of this element.
        /// </summary>
        internal int GetId()
        {
            if (!_refId.HasValue)
                throw new InvalidOperationException("Element has no reference ID");
            return _refId.Value;
        }

        // Wrap helpers for converting .NET types to model arguments
        internal static object WrapBool(bool value) => value;
        internal static object WrapInt(int value) => value;

        internal static List<object> WrapIntList(IEnumerable<int> values)
        {
            var result = new List<object>();
            foreach (var v in values)
                result.Add(v);
            return result;
        }

        internal static List<List<object>> WrapIntMatrix(IEnumerable<IEnumerable<int>> matrix)
        {
            var result = new List<List<object>>();
            foreach (var row in matrix)
            {
                var rowList = new List<object>();
                foreach (var v in row)
                    rowList.Add(v);
                result.Add(rowList);
            }
            return result;
        }
    }

    /// <summary>
    /// A restriction on variable values, created by comparison operators or constraint-posting methods.
    /// </summary>
    /// <remarks>
    /// A constraint is a condition that must be satisfied by a valid solution. Constraints are created by calling constraint-creating methods on the model or on expressions.
    ///
    /// Important: Constraints are automatically registered with the model when created. There is no need to explicitly add them using <c>Model.enforce()</c> or similar methods.
    ///
    /// Note that comparison operators on integer expressions (like <c>x &lt;= 10</c>) return <see cref="BoolExpr">BoolExpr</see>, not <c>Constraint</c>. Boolean expressions must be explicitly enforced using <see cref="Model.Enforce">Model.Enforce</see> or <see cref="BoolExpr.Enforce">BoolExpr.Enforce</see>.
    ///
    /// Unlike <see cref="BoolExpr">BoolExpr</see>, constraints cannot be combined using logical operators like <see cref="Model.And">Model.And</see>/<see cref="Model.Or">Model.Or</see> or <c>&amp;</c>/<c>|</c>.
    ///
    /// Common ways to create constraints:
    ///
    /// - Scheduling constraints: <see cref="Model.NoOverlap">Model.NoOverlap</see>, <see cref="Model.Alternative">Model.Alternative</see>, <see cref="Model.Span">Model.Span</see>
    /// - Precedence constraints: <see cref="IntervalVar.EndBeforeStart">IntervalVar.EndBeforeStart</see>, <see cref="IntervalVar.StartAtEnd">IntervalVar.StartAtEnd</see>
    /// - Cumulative constraints: <c>cumul &lt;= capacity</c>, <c>cumul &gt;= min_level</c>
    ///
    /// <code>
    /// var model = new Model();
    ///
    /// // Scheduling constraint - automatically registered
    /// var tasks = Enumerable.Range(0, 3).Select(i =&gt;
    ///   model.IntervalVar(length: 5, name: $"t_{i}")
    /// ).ToArray();
    /// model.NoOverlap(tasks);
    ///
    /// // Precedence constraint - automatically registered
    /// var task1 = model.IntervalVar(length: 10, name: "task1");
    /// var task2 = model.IntervalVar(length: 10, name: "task2");
    /// task1.EndBeforeStart(task2);
    ///
    /// // Cumulative constraint - use model.Enforce() for clarity
    /// var resource = model.Sum(tasks.Select(t =&gt; model.Pulse(t, 1)));
    /// model.Enforce(resource &lt;= 2);
    /// </code>
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="Model.Enforce">Model.Enforce</see> — for enforcing boolean expressions as constraints.</description></item>
    /// <item><description><see cref="BoolExpr">BoolExpr</see> — for boolean expressions that can also be enforced as constraints.</description></item>
    /// </list>
    /// </remarks>
    public class Constraint : ModelElement
    {
        internal Constraint(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args)
        {
            model.AddConstraint(this);
        }

        internal Constraint(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId) { }

        internal static object Wrap(Constraint c) => c.AsArg();
        internal static List<object> WrapList(IEnumerable<Constraint> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }
    }

    /// <summary>
    /// Represents an optimization objective in the model.
    /// </summary>
    /// <remarks>
    /// An objective specifies what value should be minimized or maximized when solving the model. Objectives are created by calling <see cref="Model.Minimize">Model.Minimize</see> or <see cref="Model.Maximize">Model.Maximize</see>, or by using the fluent methods <see cref="IntExpr.Minimize">IntExpr.Minimize</see> or <see cref="IntExpr.Maximize">IntExpr.Maximize</see>.
    ///
    /// A model can have at most one objective.
    ///
    /// <code>
    /// var model = new Model();
    /// var x = model.IntervalVar(length: 10, name: "x");
    /// var y = model.IntervalVar(length: 20, name: "y");
    ///
    /// // Create objective using Model.Minimize() - automatically registered:
    /// model.Minimize(y.End());
    ///
    /// // Or using fluent style on expressions - automatically registered:
    /// y.End().Minimize();
    /// </code>
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="Model.Minimize">Model.Minimize</see> — for creating minimization objectives.</description></item>
    /// <item><description><see cref="Model.Maximize">Model.Maximize</see> — for creating maximization objectives.</description></item>
    /// <item><description><see cref="IntExpr.Minimize">IntExpr.Minimize</see> — for fluent-style minimization.</description></item>
    /// <item><description><see cref="IntExpr.Maximize">IntExpr.Maximize</see> — for fluent-style maximization.</description></item>
    /// </list>
    /// </remarks>
    public class Objective : ModelElement
    {
        internal Objective(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args)
        {
            model.SetObjective(this);
        }

        internal static object Wrap(Objective o) => o.AsArg();
    }

    /// <summary>
    /// Internal search decision type.
    /// </summary>
    internal class SearchDecision : ModelElement
    {
        internal SearchDecision(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args) { }

        internal static object Wrap(SearchDecision d) => d.AsArg();
        internal static List<object> WrapList(IEnumerable<SearchDecision> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }
    }

    /// <summary>
    /// Internal directive type.
    /// </summary>
    internal class Directive : ModelElement
    {
        internal Directive(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args)
        {
            model.AddDirective(this);
        }

        internal static object Wrap(Directive d) => d.AsArg();
    }
}
