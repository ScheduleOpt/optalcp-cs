// ===========================================================================
// OptalCP C# API - Integer and Boolean Variables
// Version: 2026.3.0
// ===========================================================================

using System;
using System.Collections.Generic;

namespace OptalCP
{
    /// <summary>
    /// Integer decision variable with a domain.
    /// </summary>
    /// <remarks>
    /// Integer variable represents an unknown (integer) value that solver has to find.
    ///
    /// The value of the integer variable can be constrained using arithmetic operators (<c>+</c>, <c>-</c>, <c>*</c>, <c>/</c>) and comparison operators (<c>&lt;</c>, <c>&lt;=</c>, <c>==</c>, <c>!=</c>, <c>&gt;</c>, <c>&gt;=</c>).
    ///
    /// OptalCP solver focuses on scheduling problems and concentrates on <see cref="IntervalVar">IntervalVar</see> variables.
    /// Therefore, interval variables should be the primary choice for modeling in OptalCP.
    /// However, integer variables can be used for other purposes, such as counting or indexing.
    /// In particular, integer variables can be helpful for cumulative expressions with variable heights; see <see cref="Model.Pulse">Model.Pulse</see>, <see cref="Model.StepAtStart">Model.StepAtStart</see>, <see cref="Model.StepAtEnd">Model.StepAtEnd</see>, and <see cref="Model.StepAt">Model.StepAt</see>.
    ///
    /// The integer variable can be optional.
    /// In this case, the solver can make the variable absent, which is usually interpreted as the fact that the solver does not use the variable at all.
    /// Functions <see cref="Model.Presence">Model.Presence</see> and <see cref="IntExpr.Presence">IntExpr.Presence</see> can constrain the presence of the variable.
    ///
    /// Integer variables can be created using the function <see cref="Model.IntVar">Model.IntVar</see>.
    ///
    /// <para>Example:</para>
    /// <example>
    /// In the following example we create three integer variables <c>x</c>, <c>y</c> and <c>z</c>.
    /// Variables <c>x</c> and <c>y</c> are present, but variable <c>z</c> is optional.
    /// Each variable has a different range of possible values.
    ///
    /// <code>
    /// var model = new Model();
    /// var x = model.IntVar(min: 1, max: 3, name: "x");
    /// var y = model.IntVar(min: 0, max: 100, name: "y");
    /// var z = model.IntVar(min: 10, max: 20, optional: true, name: "z");
    /// </code>
    /// </example>
    /// </remarks>
    public class IntVar : IntExpr
    {
        internal IntVar(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId)
        {
            EnsureRef();
        }

        /// <summary>
        /// The minimum value of the integer variable's domain.
        /// </summary>
        /// <remarks>
        /// Gets or sets the minimum value of the integer variable's domain.
        ///
        /// The initial value is set during construction by <see cref="Model.IntVar">Model.IntVar</see>.
        ///
        /// Note: This property reflects the variable's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range <see cref="Model.IntVarMin">Model.IntVarMin</see> to <see cref="Model.IntVarMax">Model.IntVarMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.IntVar(min: 5, max: 10, name: "x");
        ///
        /// Console.WriteLine(x.Min);  // 5
        ///
        /// x.Min = 7;
        /// Console.WriteLine(x.Min);  // 7
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntVar.Max">IntVar.Max</see></description></item>
        /// <item><description><see cref="IntVar.Optional">IntVar.Optional</see></description></item>
        /// </list>
        /// </remarks>
        public int Min
        {
            get => _props.TryGetValue("min", out var v) ? Convert.ToInt32(v) : 0;
            set => _props["min"] = value;
        }

        /// <summary>
        /// The maximum value of the integer variable's domain.
        /// </summary>
        /// <remarks>
        /// Gets or sets the maximum value of the integer variable's domain.
        ///
        /// The initial value is set during construction by <see cref="Model.IntVar">Model.IntVar</see>.
        ///
        /// Note: This property reflects the variable's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// The value must be in the range <see cref="Model.IntVarMin">Model.IntVarMin</see> to <see cref="Model.IntVarMax">Model.IntVarMax</see>.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.IntVar(min: 5, max: 10, name: "x");
        ///
        /// Console.WriteLine(x.Max);  // 10
        ///
        /// x.Max = 8;
        /// Console.WriteLine(x.Max);  // 8
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntVar.Min">IntVar.Min</see></description></item>
        /// <item><description><see cref="IntVar.Optional">IntVar.Optional</see></description></item>
        /// </list>
        /// </remarks>
        public int Max
        {
            get => _props.TryGetValue("max", out var v) ? Convert.ToInt32(v) : Model.IntVarMax;
            set => _props["max"] = value;
        }

        /// <summary>
        /// Whether the integer variable is optional.
        /// </summary>
        /// <remarks>
        /// Gets or sets whether the integer variable is optional:
        ///
        /// - <c>True</c> / <see langword="true"/>: The variable is *optional* - the solver decides whether it is present or absent in the solution.
        /// - <c>False</c> / <see langword="false"/>: The variable is *present* - it must have a value in the solution.
        ///
        /// To force a variable to be absent, make it optional and constrain its presence
        /// to be false: <c>model.enforce(~model.presence(x))</c> (Python),
        /// <c>model.enforce(model.presence(x).not())</c> (TypeScript), or
        /// <c>model.Enforce(!model.Presence(x))</c> (C#).
        ///
        /// Note: This property reflects the presence status in the model
        /// (before the solve), not in the solution.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.IntVar(min: 0, max: 10, name: "x");
        /// var y = model.IntVar(min: 0, max: 10, optional: true, name: "y");
        ///
        /// Console.WriteLine(x.Optional);  // False (present by default)
        /// Console.WriteLine(y.Optional);  // True (optional)
        ///
        /// // Make x optional
        /// x.Optional = true;
        /// Console.WriteLine(x.Optional);  // True
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="IntVar.Min">IntVar.Min</see></description></item>
        /// <item><description><see cref="IntVar.Max">IntVar.Max</see></description></item>
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

        internal static object Wrap(IntVar v) => v.AsArg();

        internal static List<object> WrapList(IEnumerable<IntVar> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }
    }

    /// <summary>
    /// Boolean decision variable whose value is determined by the solver.
    /// </summary>
    /// <remarks>
    /// Boolean variable represents an unknown truth value (<c>True</c> or <c>False</c>) that the solver must find.
    ///
    /// Boolean variables are useful for modeling decisions, choices, or logical conditions in your problem. For example, you can use boolean variables to represent whether a machine is used, whether a task is assigned to a particular worker, or whether a constraint should be enforced.
    ///
    /// Boolean variables can be created using the function <see cref="Model.BoolVar">Model.BoolVar</see>.
    /// By default, boolean variables are *present* (not optional).
    /// To create an optional boolean variable, specify <c>optional=True</c> in the arguments of the function.
    ///
    /// <b>Logical operators</b>
    ///
    /// Boolean variables support the following logical operators:
    ///
    /// - <c>!x</c> for logical NOT
    /// - <c>x | y</c> for logical OR
    /// - <c>x &amp; y</c> for logical AND
    ///
    /// Note: use <c>&amp;</c> and <c>|</c> (bitwise operators), not <c>&amp;&amp;</c> and <c>||</c> (short-circuit operators), as C# does not allow overloading <c>&amp;&amp;</c> and <c>||</c>.
    ///
    /// These operators can be used to create complex boolean expressions and constraints.
    ///
    /// <b>Boolean variables as integer expressions</b>
    ///
    /// Class <c>BoolVar</c> derives from <see cref="BoolExpr">BoolExpr</see>, which derives from <see cref="IntExpr">IntExpr</see>.
    /// Therefore, boolean variables can be used as integer expressions:
    /// *True* is equal to *1*, *False* is equal to *0*, and *absent* remains *absent*.
    ///
    /// This is useful for counting how many conditions are satisfied or for weighted sums.
    ///
    /// <b>Optional boolean variables</b>
    ///
    /// A boolean variable can be optional. In this case, the solver can decide to make the variable *absent*, which means the variable doesn't participate in the solution. When a boolean variable is absent, its value is neither <c>True</c> nor <c>False</c> — it is *absent*.
    ///
    /// Most expressions that depend on an absent variable are also *absent*. For example, if <c>x</c> is an absent boolean variable, then <c>~x</c>, <c>x | y</c>, and <c>x &amp; y</c> are all *absent*, regardless of the value of <c>y</c>. However, some functions handle absent values specially, such as <see cref="IntExpr.Presence">IntExpr.Presence</see> or <see cref="Model.Sum">Model.Sum</see>.
    ///
    /// When a boolean expression is added as a constraint using <see cref="Model.Enforce">Model.Enforce</see>, the constraint requires that the expression is not <see langword="false"/> in the solution. The expression can be <see langword="true"/> or *absent*. This means that constraints involving optional variables are automatically satisfied when the underlying variables are absent.
    ///
    /// Functions <see cref="Model.Presence">Model.Presence</see> and <see cref="IntExpr.Presence">IntExpr.Presence</see> can constrain the presence of the variable.
    ///
    /// <para>Example:</para>
    /// <example>
    /// In the following example, we create two boolean variables representing whether to use each of two machines. We require that at least one machine is used, but not both:
    ///
    /// <code>
    /// var model = new Model();
    /// var useMachineA = model.BoolVar(name: "use_machine_a");
    /// var useMachineB = model.BoolVar(name: "use_machine_b");
    ///
    /// // Constraint: must use at least one machine
    /// model.Enforce(useMachineA | useMachineB);
    ///
    /// // Constraint: cannot use both machines (exclusive choice)
    /// model.Enforce(!(useMachineA &amp; useMachineB));
    ///
    /// var result = model.Solve();
    /// </code>
    /// </example><para>Example:</para>
    /// <example>
    /// Boolean variables can be used in arithmetic expressions by treating <c>True</c> as 1 and <c>False</c> as 0:
    ///
    /// <code>
    /// var model = new Model();
    /// var options = Enumerable.Range(0, 5).Select(i =&gt; model.BoolVar(name: $"option_{i}")).ToArray();
    ///
    /// // Constraint: select exactly 2 options
    /// model.Enforce(model.Sum(options) == 2);
    ///
    /// var result = model.Solve();
    /// </code>
    /// </example>
    ///
    /// See also:
    /// <list type="bullet">
    /// <item><description><see cref="Model.BoolVar">Model.BoolVar</see> — to create boolean variables.</description></item>
    /// <item><description><see cref="BoolExpr">BoolExpr</see> — for boolean expressions and their operations.</description></item>
    /// <item><description><see cref="IntVar">IntVar</see> — for integer decision variables.</description></item>
    /// <item><description><see cref="IntervalVar">IntervalVar</see> — for the primary variable type for scheduling problems.</description></item>
    /// </list>
    /// </remarks>
    public class BoolVar : BoolExpr
    {
        internal BoolVar(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId)
        {
            EnsureRef();
        }

        /// <summary>
        /// The minimum value of the boolean variable's domain.
        /// </summary>
        /// <remarks>
        /// Gets or sets the minimum value of the boolean variable's domain.
        ///
        /// For a free (unconstrained) boolean variable, returns <c>False</c>.
        /// If set to <c>True</c>, the variable is fixed to <c>True</c>.
        ///
        /// Note: This property reflects the variable's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.BoolVar(name: "x");
        ///
        /// Console.WriteLine(x.Min);  // False (default minimum)
        ///
        /// x.Min = true;
        /// Console.WriteLine(x.Min);  // True (variable is now fixed to True)
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolVar.Max">BoolVar.Max</see></description></item>
        /// <item><description><see cref="BoolVar.Optional">BoolVar.Optional</see></description></item>
        /// </list>
        /// </remarks>
        public bool Min
        {
            get => _props.TryGetValue("min", out var v) && Convert.ToInt32(v) > 0;
            set => _props["min"] = value;
        }

        /// <summary>
        /// The maximum value of the boolean variable's domain.
        /// </summary>
        /// <remarks>
        /// Gets or sets the maximum value of the boolean variable's domain.
        ///
        /// For a free (unconstrained) boolean variable, returns <c>True</c>.
        /// If set to <c>False</c>, the variable is fixed to <c>False</c>.
        ///
        /// Note: This property reflects the variable's domain in the model
        /// (before the solve), not in the solution.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.BoolVar(name: "x");
        ///
        /// Console.WriteLine(x.Max);  // True (default maximum)
        ///
        /// x.Max = false;
        /// Console.WriteLine(x.Max);  // False (variable is now fixed to False)
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolVar.Min">BoolVar.Min</see></description></item>
        /// <item><description><see cref="BoolVar.Optional">BoolVar.Optional</see></description></item>
        /// </list>
        /// </remarks>
        public bool Max
        {
            get => !_props.TryGetValue("max", out var v) || Convert.ToInt32(v) > 0;
            set => _props["max"] = value;
        }

        /// <summary>
        /// Whether the boolean variable is optional.
        /// </summary>
        /// <remarks>
        /// Gets or sets whether the boolean variable is optional:
        ///
        /// - <c>True</c> / <see langword="true"/>: The variable is *optional* - the solver decides whether it is present or absent in the solution.
        /// - <c>False</c> / <see langword="false"/>: The variable is *present* - it must have a value in the solution.
        ///
        /// To force a variable to be absent, make it optional and constrain its presence
        /// to be false: <c>model.enforce(~model.presence(x))</c> (Python),
        /// <c>model.enforce(model.presence(x).not())</c> (TypeScript), or
        /// <c>model.Enforce(!model.Presence(x))</c> (C#).
        ///
        /// Note: This property reflects the presence status in the model
        /// (before the solve), not in the solution.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.BoolVar(name: "x");
        /// var y = model.BoolVar(optional: true, name: "y");
        ///
        /// Console.WriteLine(x.Optional);  // False (present by default)
        /// Console.WriteLine(y.Optional);  // True (optional)
        ///
        /// // Make x optional
        /// x.Optional = true;
        /// Console.WriteLine(x.Optional);  // True
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolVar.Min">BoolVar.Min</see></description></item>
        /// <item><description><see cref="BoolVar.Max">BoolVar.Max</see></description></item>
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

        internal static object Wrap(BoolVar v) => v.AsArg();

        internal static List<object> WrapList(IEnumerable<BoolVar> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }
    }
}
