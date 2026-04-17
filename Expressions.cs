// ===========================================================================
// OptalCP C# API - Expression Types
// Version: 2026.3.0
// ===========================================================================

using System;
using System.Collections.Generic;

namespace OptalCP
{
    /// <summary>
    /// Integer expression, e.g. an arithmetic combination of variables and constants.
    /// </summary>
    /// <remarks>
    /// The expression may depend on the value of a variable (or variables), so the
    /// value of the expression is not known until a solution is found. The value must
    /// be in the range <see cref="Model.IntVarMin">Model.IntVarMin</see> to <see cref="Model.IntVarMax">Model.IntVarMax</see>.
    ///
    /// Use standard arithmetic operators (<c>+</c>, <c>-</c>, <c>*</c>, <c>/</c>, unary <c>-</c>) and
    /// comparison operators (<c>&lt;</c>, <c>&lt;=</c>, <c>==</c>, <c>!=</c>, <c>&gt;</c>, <c>&gt;=</c>).
    ///
    /// <para>Example:</para>
    /// <example>
    /// The following code creates two interval variables <c>x</c> and <c>y</c>
    /// and an integer expression <c>makespan</c> that is equal to the maximum of the end
    /// times of <c>x</c> and <c>y</c> (see <see cref="Model.Max2">Model.Max2</see>):
    /// <code>
    /// var model = new Model();
    /// var x = model.IntervalVar(length: 10, name: "x");
    /// var y = model.IntervalVar(length: 20, name: "y");
    /// var makespan = model.Max2(x.End(), y.End());
    /// </code>
    ///
    /// <b>Optional integer expressions</b>
    ///
    /// Underlying variables of an integer expression may be optional, i.e., they may
    /// or may not be present in a solution (for example, an optional task
    /// can be omitted entirely from the solution). In this case, the value of the
    /// integer expression is *absent*. The value *absent* means that the variable
    /// has no meaning; it does not exist in the solution.
    ///
    /// Except <see cref="IntExpr.Guard">IntExpr.Guard</see> expression, any value of an integer
    /// expression that depends on an absent variable is also *absent*.
    /// As we don't know the value of the expression before the solution is found,
    /// we call such expression *optional*.
    /// </example><para>Example:</para>
    /// <example>
    /// In the following model, there is an optional interval variable <c>x</c> and
    /// a non-optional interval variable <c>y</c>.  We add a constraint that the end of <c>x</c> plus
    /// 10 must be less or equal to the start of <c>y</c>:
    /// <code>
    /// var model = new Model();
    /// var x = model.IntervalVar(length: 10, name: "x", optional: true);
    /// var y = model.IntervalVar(length: 20, name: "y");
    /// var finish = x.End();
    /// var ready = finish + 10;
    /// var begin = y.Start();
    /// var precedes = ready &lt;= begin;
    /// model.Enforce(precedes);
    /// var result = model.Solve();
    /// </code>
    /// In this model:
    ///
    /// * <c>finish</c> is an optional integer expression because it depends on
    ///   an optional variable <c>x</c>.
    /// * The expression <c>ready</c> is optional for the same reason.
    /// * The expression <c>begin</c> is not optional because it depends only on a
    ///   non-optional variable <c>y</c>.
    /// * Boolean expression <c>precedes</c> is also optional. Its value could be
    ///   <see langword="true"/>, <see langword="false"/> or *absent*.
    ///
    /// The expression <c>precedes</c> is turned into a constraint using
    /// <see cref="Model.Enforce">Model.Enforce</see>. Therefore, it cannot be <see langword="false"/>
    /// in the solution. However, it can still be *absent*. Therefore the constraint
    /// <c>precedes</c> can be satisfied in two ways:
    ///
    /// 1. Both <c>x</c> and <c>y</c> are present, <c>x</c> is before <c>y</c>, and the delay between them
    /// is at least 10. In this case, <c>precedes</c> is <see langword="true"/>.
    /// 2. <c>x</c> is absent and <c>y</c> is present. In this case, <c>precedes</c> is *absent*.
    /// </example>
    /// </remarks>
    public class IntExpr : ModelElement
    {
        internal IntExpr(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args) { }

        internal IntExpr(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId) { }

        internal static object Wrap(IntExpr expr) => expr.AsArg();
        internal static object Wrap(int value) => value;

        internal static List<object> WrapList(IEnumerable<IntExpr> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }

        internal static List<object> WrapMixedList(IEnumerable<object> items)
        {
            var result = new List<object>();
            foreach (var item in items)
            {
                if (item is IntExpr expr)
                    result.Add(expr.AsArg());
                else if (item is int i)
                    result.Add(i);
                else
                    throw new ArgumentException($"Expected IntExpr or int, got {item?.GetType().Name}");
            }
            return result;
        }

        /// <summary>
        /// Add two integer expressions using the <c>+</c> operator.
        /// </summary>
        /// <returns>A new expression representing the sum.</returns>
        /// <remarks>
        /// Returns a new <see cref="IntExpr">IntExpr</see> representing the sum. All three overloads are supported:
        /// <c>expr + expr</c>, <c>expr + constant</c>, and <c>constant + expr</c>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// <code>
        /// var sum = x + y;
        /// var incremented = x + 1;
        /// var also = 1 + x;
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Sum">Model.Sum</see> — for summing multiple expressions. Note: <see cref="Model.Sum">Model.Sum</see> skips *absent* values (so <c>Model.Sum(new[] { x, absent })</c> equals <c>x</c>), while <c>x + absent</c> is *absent*.</description></item>
        /// </list>
        /// </remarks>
        public static IntExpr operator +(IntExpr lhs, IntExpr rhs)
            => new IntExpr(lhs._model, "intPlus", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Add two integer expressions using the <c>+</c> operator.
        /// </summary>
        /// <returns>A new expression representing the sum.</returns>
        /// <remarks>
        /// Returns a new <see cref="IntExpr">IntExpr</see> representing the sum. All three overloads are supported:
        /// <c>expr + expr</c>, <c>expr + constant</c>, and <c>constant + expr</c>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// <code>
        /// var sum = x + y;
        /// var incremented = x + 1;
        /// var also = 1 + x;
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Sum">Model.Sum</see> — for summing multiple expressions. Note: <see cref="Model.Sum">Model.Sum</see> skips *absent* values (so <c>Model.Sum(new[] { x, absent })</c> equals <c>x</c>), while <c>x + absent</c> is *absent*.</description></item>
        /// </list>
        /// </remarks>
        public static IntExpr operator +(IntExpr lhs, int rhs)
            => new IntExpr(lhs._model, "intPlus", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Add two integer expressions using the <c>+</c> operator.
        /// </summary>
        /// <returns>A new expression representing the sum.</returns>
        /// <remarks>
        /// Returns a new <see cref="IntExpr">IntExpr</see> representing the sum. All three overloads are supported:
        /// <c>expr + expr</c>, <c>expr + constant</c>, and <c>constant + expr</c>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// <code>
        /// var sum = x + y;
        /// var incremented = x + 1;
        /// var also = 1 + x;
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Sum">Model.Sum</see> — for summing multiple expressions. Note: <see cref="Model.Sum">Model.Sum</see> skips *absent* values (so <c>Model.Sum(new[] { x, absent })</c> equals <c>x</c>), while <c>x + absent</c> is *absent*.</description></item>
        /// </list>
        /// </remarks>
        public static IntExpr operator +(int lhs, IntExpr rhs)
            => new IntExpr(rhs._model, "intPlus", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Subtract integer expressions using the <c>-</c> operator.
        /// </summary>
        /// <returns>A new expression representing the difference.</returns>
        /// <remarks>
        /// Returns a new <c>IntExpr</c> representing the difference. All three overloads are supported:
        /// <c>expr - expr</c>, <c>expr - constant</c>, and <c>constant - expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// var diff = x - y;
        /// var decremented = x - 1;
        /// var fromConst = 10 - x;
        /// </code>
        /// </remarks>
        public static IntExpr operator -(IntExpr lhs, IntExpr rhs)
            => new IntExpr(lhs._model, "intMinus", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Subtract integer expressions using the <c>-</c> operator.
        /// </summary>
        /// <returns>A new expression representing the difference.</returns>
        /// <remarks>
        /// Returns a new <c>IntExpr</c> representing the difference. All three overloads are supported:
        /// <c>expr - expr</c>, <c>expr - constant</c>, and <c>constant - expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// var diff = x - y;
        /// var decremented = x - 1;
        /// var fromConst = 10 - x;
        /// </code>
        /// </remarks>
        public static IntExpr operator -(IntExpr lhs, int rhs)
            => new IntExpr(lhs._model, "intMinus", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Subtract integer expressions using the <c>-</c> operator.
        /// </summary>
        /// <returns>A new expression representing the difference.</returns>
        /// <remarks>
        /// Returns a new <c>IntExpr</c> representing the difference. All three overloads are supported:
        /// <c>expr - expr</c>, <c>expr - constant</c>, and <c>constant - expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// var diff = x - y;
        /// var decremented = x - 1;
        /// var fromConst = 10 - x;
        /// </code>
        /// </remarks>
        public static IntExpr operator -(int lhs, IntExpr rhs)
            => new IntExpr(rhs._model, "intMinus", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Negate an integer expression using the unary <c>-</c> operator.
        /// </summary>
        /// <returns>A new expression representing the negation.</returns>
        /// <remarks>
        /// Returns a new <c>IntExpr</c> representing <c>-expr</c>.
        ///
        /// If the operand is absent, the result is also absent.
        ///
        /// <code>
        /// var neg = -x;
        /// model.Enforce(neg &gt;= -5); // equivalent to x &lt;= 5
        /// </code>
        /// </remarks>
        public static IntExpr operator -(IntExpr expr)
            => new IntExpr(expr._model, "intNeg", new object[] { expr.AsArg() });

        /// <summary>
        /// Multiply integer expressions using the <c>*</c> operator.
        /// </summary>
        /// <returns>A new expression representing the product.</returns>
        /// <remarks>
        /// Returns a new <c>IntExpr</c> representing the product. All three overloads are supported:
        /// <c>expr * expr</c>, <c>expr * constant</c>, and <c>constant * expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// var product = x * y;
        /// var scaled = x * 2;
        /// var also = 2 * x;
        /// </code>
        /// </remarks>
        public static IntExpr operator *(IntExpr lhs, IntExpr rhs)
            => new IntExpr(lhs._model, "intTimes", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Multiply integer expressions using the <c>*</c> operator.
        /// </summary>
        /// <returns>A new expression representing the product.</returns>
        /// <remarks>
        /// Returns a new <c>IntExpr</c> representing the product. All three overloads are supported:
        /// <c>expr * expr</c>, <c>expr * constant</c>, and <c>constant * expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// var product = x * y;
        /// var scaled = x * 2;
        /// var also = 2 * x;
        /// </code>
        /// </remarks>
        public static IntExpr operator *(IntExpr lhs, int rhs)
            => new IntExpr(lhs._model, "intTimes", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Multiply integer expressions using the <c>*</c> operator.
        /// </summary>
        /// <returns>A new expression representing the product.</returns>
        /// <remarks>
        /// Returns a new <c>IntExpr</c> representing the product. All three overloads are supported:
        /// <c>expr * expr</c>, <c>expr * constant</c>, and <c>constant * expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// var product = x * y;
        /// var scaled = x * 2;
        /// var also = 2 * x;
        /// </code>
        /// </remarks>
        public static IntExpr operator *(int lhs, IntExpr rhs)
            => new IntExpr(rhs._model, "intTimes", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Divide integer expressions using the <c>/</c> operator (integer division).
        /// </summary>
        /// <returns>A new expression representing the integer quotient.</returns>
        /// <remarks>
        /// Returns a new <see cref="IntExpr">IntExpr</see> representing integer division. All three overloads are supported:
        /// <c>expr / expr</c>, <c>expr / constant</c>, and <c>constant / expr</c>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// The result is truncated toward zero, matching C#'s native integer <c>/</c> semantics.
        /// For example, <c>-7 / 2</c> yields <c>-3</c>, not <c>-4</c>. OptalCP's integer division always rounds
        /// toward zero, and this is consistent across the TypeScript, Python, and C# APIs.
        ///
        /// <code>
        /// var quotient = x / y;
        /// var halved = x / 2;
        /// var fromConst = 100 / x;
        /// </code>
        /// </remarks>
        public static IntExpr operator /(IntExpr lhs, IntExpr rhs)
            => new IntExpr(lhs._model, "intDiv", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Divide integer expressions using the <c>/</c> operator (integer division).
        /// </summary>
        /// <returns>A new expression representing the integer quotient.</returns>
        /// <remarks>
        /// Returns a new <see cref="IntExpr">IntExpr</see> representing integer division. All three overloads are supported:
        /// <c>expr / expr</c>, <c>expr / constant</c>, and <c>constant / expr</c>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// The result is truncated toward zero, matching C#'s native integer <c>/</c> semantics.
        /// For example, <c>-7 / 2</c> yields <c>-3</c>, not <c>-4</c>. OptalCP's integer division always rounds
        /// toward zero, and this is consistent across the TypeScript, Python, and C# APIs.
        ///
        /// <code>
        /// var quotient = x / y;
        /// var halved = x / 2;
        /// var fromConst = 100 / x;
        /// </code>
        /// </remarks>
        public static IntExpr operator /(IntExpr lhs, int rhs)
            => new IntExpr(lhs._model, "intDiv", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Divide integer expressions using the <c>/</c> operator (integer division).
        /// </summary>
        /// <returns>A new expression representing the integer quotient.</returns>
        /// <remarks>
        /// Returns a new <see cref="IntExpr">IntExpr</see> representing integer division. All three overloads are supported:
        /// <c>expr / expr</c>, <c>expr / constant</c>, and <c>constant / expr</c>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// The result is truncated toward zero, matching C#'s native integer <c>/</c> semantics.
        /// For example, <c>-7 / 2</c> yields <c>-3</c>, not <c>-4</c>. OptalCP's integer division always rounds
        /// toward zero, and this is consistent across the TypeScript, Python, and C# APIs.
        ///
        /// <code>
        /// var quotient = x / y;
        /// var halved = x / 2;
        /// var fromConst = 100 / x;
        /// </code>
        /// </remarks>
        public static IntExpr operator /(int lhs, IntExpr rhs)
            => new IntExpr(rhs._model, "intDiv", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Create an equality constraint using the <c>==</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs == rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs == rhs</c>. All three overloads are supported:
        /// <c>expr == expr</c>, <c>expr == constant</c>, and <c>constant == expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x == y);
        /// model.Enforce(x == 5);
        /// model.Enforce(5 == x);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Enforce">BoolExpr.Enforce</see> — to post the constraint directly.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator ==(IntExpr lhs, IntExpr rhs)
            => new BoolExpr(lhs._model, "intEq", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create an equality constraint using the <c>==</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs == rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs == rhs</c>. All three overloads are supported:
        /// <c>expr == expr</c>, <c>expr == constant</c>, and <c>constant == expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x == y);
        /// model.Enforce(x == 5);
        /// model.Enforce(5 == x);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Enforce">BoolExpr.Enforce</see> — to post the constraint directly.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator ==(IntExpr lhs, int rhs)
            => new BoolExpr(lhs._model, "intEq", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create an equality constraint using the <c>==</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs == rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs == rhs</c>. All three overloads are supported:
        /// <c>expr == expr</c>, <c>expr == constant</c>, and <c>constant == expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x == y);
        /// model.Enforce(x == 5);
        /// model.Enforce(5 == x);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Enforce">BoolExpr.Enforce</see> — to post the constraint directly.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator ==(int lhs, IntExpr rhs)
            => new BoolExpr(rhs._model, "intEq", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Create an inequality constraint using the <c>!=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs != rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs != rhs</c>. All three overloads are supported:
        /// <c>expr != expr</c>, <c>expr != constant</c>, and <c>constant != expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x != y);
        /// model.Enforce(x != 0);
        /// </code>
        /// </remarks>
        public static BoolExpr operator !=(IntExpr lhs, IntExpr rhs)
            => new BoolExpr(lhs._model, "intNe", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create an inequality constraint using the <c>!=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs != rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs != rhs</c>. All three overloads are supported:
        /// <c>expr != expr</c>, <c>expr != constant</c>, and <c>constant != expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x != y);
        /// model.Enforce(x != 0);
        /// </code>
        /// </remarks>
        public static BoolExpr operator !=(IntExpr lhs, int rhs)
            => new BoolExpr(lhs._model, "intNe", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create an inequality constraint using the <c>!=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs != rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs != rhs</c>. All three overloads are supported:
        /// <c>expr != expr</c>, <c>expr != constant</c>, and <c>constant != expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x != y);
        /// model.Enforce(x != 0);
        /// </code>
        /// </remarks>
        public static BoolExpr operator !=(int lhs, IntExpr rhs)
            => new BoolExpr(rhs._model, "intNe", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Create a less-than constraint using the <c>&lt;</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &lt; rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &lt; rhs</c>. All three overloads are supported:
        /// <c>expr &lt; expr</c>, <c>expr &lt; constant</c>, and <c>constant &lt; expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &lt; y);
        /// model.Enforce(x &lt; 10);
        /// model.Enforce(0 &lt; x); // equivalent to x &gt; 0
        /// </code>
        /// </remarks>
        public static BoolExpr operator <(IntExpr lhs, IntExpr rhs)
            => new BoolExpr(lhs._model, "intLt", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create a less-than constraint using the <c>&lt;</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &lt; rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &lt; rhs</c>. All three overloads are supported:
        /// <c>expr &lt; expr</c>, <c>expr &lt; constant</c>, and <c>constant &lt; expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &lt; y);
        /// model.Enforce(x &lt; 10);
        /// model.Enforce(0 &lt; x); // equivalent to x &gt; 0
        /// </code>
        /// </remarks>
        public static BoolExpr operator <(IntExpr lhs, int rhs)
            => new BoolExpr(lhs._model, "intLt", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create a less-than constraint using the <c>&lt;</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &lt; rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &lt; rhs</c>. All three overloads are supported:
        /// <c>expr &lt; expr</c>, <c>expr &lt; constant</c>, and <c>constant &lt; expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &lt; y);
        /// model.Enforce(x &lt; 10);
        /// model.Enforce(0 &lt; x); // equivalent to x &gt; 0
        /// </code>
        /// </remarks>
        public static BoolExpr operator <(int lhs, IntExpr rhs)
            => new BoolExpr(rhs._model, "intLt", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Create a less-than-or-equal constraint using the <c>&lt;=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &lt;= rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &lt;= rhs</c>. All three overloads are supported:
        /// <c>expr &lt;= expr</c>, <c>expr &lt;= constant</c>, and <c>constant &lt;= expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &lt;= y);
        /// model.Enforce(x &lt;= 10);
        /// model.Enforce(0 &lt;= x); // equivalent to x &gt;= 0
        /// </code>
        /// </remarks>
        public static BoolExpr operator <=(IntExpr lhs, IntExpr rhs)
            => new BoolExpr(lhs._model, "intLe", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create a less-than-or-equal constraint using the <c>&lt;=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &lt;= rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &lt;= rhs</c>. All three overloads are supported:
        /// <c>expr &lt;= expr</c>, <c>expr &lt;= constant</c>, and <c>constant &lt;= expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &lt;= y);
        /// model.Enforce(x &lt;= 10);
        /// model.Enforce(0 &lt;= x); // equivalent to x &gt;= 0
        /// </code>
        /// </remarks>
        public static BoolExpr operator <=(IntExpr lhs, int rhs)
            => new BoolExpr(lhs._model, "intLe", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create a less-than-or-equal constraint using the <c>&lt;=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &lt;= rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &lt;= rhs</c>. All three overloads are supported:
        /// <c>expr &lt;= expr</c>, <c>expr &lt;= constant</c>, and <c>constant &lt;= expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &lt;= y);
        /// model.Enforce(x &lt;= 10);
        /// model.Enforce(0 &lt;= x); // equivalent to x &gt;= 0
        /// </code>
        /// </remarks>
        public static BoolExpr operator <=(int lhs, IntExpr rhs)
            => new BoolExpr(rhs._model, "intLe", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Create a greater-than constraint using the <c>&gt;</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &gt; rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &gt; rhs</c>. All three overloads are supported:
        /// <c>expr &gt; expr</c>, <c>expr &gt; constant</c>, and <c>constant &gt; expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &gt; y);
        /// model.Enforce(x &gt; 0);
        /// model.Enforce(10 &gt; x); // equivalent to x &lt; 10
        /// </code>
        /// </remarks>
        public static BoolExpr operator >(IntExpr lhs, IntExpr rhs)
            => new BoolExpr(lhs._model, "intGt", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create a greater-than constraint using the <c>&gt;</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &gt; rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &gt; rhs</c>. All three overloads are supported:
        /// <c>expr &gt; expr</c>, <c>expr &gt; constant</c>, and <c>constant &gt; expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &gt; y);
        /// model.Enforce(x &gt; 0);
        /// model.Enforce(10 &gt; x); // equivalent to x &lt; 10
        /// </code>
        /// </remarks>
        public static BoolExpr operator >(IntExpr lhs, int rhs)
            => new BoolExpr(lhs._model, "intGt", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create a greater-than constraint using the <c>&gt;</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &gt; rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &gt; rhs</c>. All three overloads are supported:
        /// <c>expr &gt; expr</c>, <c>expr &gt; constant</c>, and <c>constant &gt; expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &gt; y);
        /// model.Enforce(x &gt; 0);
        /// model.Enforce(10 &gt; x); // equivalent to x &lt; 10
        /// </code>
        /// </remarks>
        public static BoolExpr operator >(int lhs, IntExpr rhs)
            => new BoolExpr(rhs._model, "intGt", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Create a greater-than-or-equal constraint using the <c>&gt;=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &gt;= rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &gt;= rhs</c>. All three overloads are supported:
        /// <c>expr &gt;= expr</c>, <c>expr &gt;= constant</c>, and <c>constant &gt;= expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &gt;= y);
        /// model.Enforce(x &gt;= 0);
        /// model.Enforce(10 &gt;= x); // equivalent to x &lt;= 10
        /// </code>
        /// </remarks>
        public static BoolExpr operator >=(IntExpr lhs, IntExpr rhs)
            => new BoolExpr(lhs._model, "intGe", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create a greater-than-or-equal constraint using the <c>&gt;=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &gt;= rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &gt;= rhs</c>. All three overloads are supported:
        /// <c>expr &gt;= expr</c>, <c>expr &gt;= constant</c>, and <c>constant &gt;= expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &gt;= y);
        /// model.Enforce(x &gt;= 0);
        /// model.Enforce(10 &gt;= x); // equivalent to x &lt;= 10
        /// </code>
        /// </remarks>
        public static BoolExpr operator >=(IntExpr lhs, int rhs)
            => new BoolExpr(lhs._model, "intGe", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create a greater-than-or-equal constraint using the <c>&gt;=</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when lhs &gt;= rhs.</returns>
        /// <remarks>
        /// Returns a <c>BoolExpr</c> representing <c>lhs &gt;= rhs</c>. All three overloads are supported:
        /// <c>expr &gt;= expr</c>, <c>expr &gt;= constant</c>, and <c>constant &gt;= expr</c>.
        ///
        /// If either operand is absent, the result is also absent.
        ///
        /// <code>
        /// model.Enforce(x &gt;= y);
        /// model.Enforce(x &gt;= 0);
        /// model.Enforce(10 &gt;= x); // equivalent to x &lt;= 10
        /// </code>
        /// </remarks>
        public static BoolExpr operator >=(int lhs, IntExpr rhs)
            => new BoolExpr(rhs._model, "intGe", new object[] { lhs, rhs.AsArg() });

        /// <inheritdoc/>
        public override bool Equals(object? obj) => base.Equals(obj);
        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Creates a minimization objective for this expression.
        /// </summary>
        /// <returns>An Objective that minimizes this expression.</returns>
        /// <remarks>
        /// Creates an objective to minimize the value of this integer expression.
        /// A model can have at most one objective. New objective replaces the old one.
        ///
        /// This is a fluent-style alternative to <see cref="Model.Minimize">Model.Minimize</see> that allows
        /// creating objectives directly from expressions.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(length: 10, name: "x");
        /// var y = model.IntervalVar(length: 20, name: "y");
        ///
        /// // Fluent style - minimize the end of y
        /// y.End().Minimize();
        ///
        /// // Equivalent Model.Minimize() style:
        /// model.Minimize(y.End());
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Minimize">Model.Minimize</see> — for Model-centric minimization.</description></item>
        /// <item><description><see cref="IntExpr.Maximize">IntExpr.Maximize</see> — for maximization.</description></item>
        /// </list>
        /// </remarks>
        public Objective Minimize()
            => _model.Minimize(this);

        /// <summary>
        /// Creates a maximization objective for this expression.
        /// </summary>
        /// <returns>An Objective that maximizes this expression.</returns>
        /// <remarks>
        /// Creates an objective to maximize the value of this integer expression.
        /// A model can have at most one objective. New objective replaces the old one.
        ///
        /// This is a fluent-style alternative to <see cref="Model.Maximize">Model.Maximize</see> that allows
        /// creating objectives directly from expressions.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.IntervalVar(start: (0, 100), length: 10, name: "x");
        ///
        /// // Fluent style - maximize the start of x
        /// x.Start().Maximize();
        ///
        /// // Equivalent Model.Maximize() style:
        /// model.Maximize(x.Start());
        /// </code>
        ///
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Maximize">Model.Maximize</see> — for Model-centric maximization.</description></item>
        /// <item><description><see cref="IntExpr.Minimize">IntExpr.Minimize</see> — for minimization.</description></item>
        /// </list>
        /// </remarks>
        public Objective Maximize()
            => _model.Maximize(this);

        /// <summary>
        /// Creates a _reusableIntExpr expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal IntExpr ReusableIntExpr()
        {
            var outParams = new List<object?>() { AsArg() };
            return new IntExpr(_model, "reusableIntExpr", outParams);
        }

        /// <summary>
        /// Returns an expression which is <see langword="true"/> if the expression is *present* and <see langword="false"/> when it is *absent*.
        /// </summary>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// The resulting expression is never *absent*.
        ///
        /// Same as <see cref="Model.Presence">Model.Presence</see>.
        /// </remarks>
        public BoolExpr Presence()
        {
            var outParams = new List<object?>() { AsArg() };
            return new BoolExpr(_model, "intPresenceOf", outParams);
        }

        /// <summary>
        /// Creates an expression that replaces value *absent* by a constant.
        /// </summary>
        /// <param name="absentValue">The value to use when the expression is absent.</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// The resulting expression is:
        ///
        /// * equal to the expression if the expression is *present*
        /// * and equal to <c>absentValue</c> otherwise (i.e. when the expression is *absent*).
        ///
        /// The default value of <c>absentValue</c> is 0.
        ///
        /// The resulting expression is never *absent*.
        ///
        /// Same as <see cref="Model.Guard">Model.Guard</see>.
        /// </remarks>
        public IntExpr Guard(int absentValue = 0)
        {
            var outParams = new List<object?>() { AsArg(), absentValue };
            return new IntExpr(_model, "intGuard", outParams);
        }

        /// <summary>
        /// Constrains two expressions to be identical, including their presence status.
        /// </summary>
        /// <param name="rhs">The right-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <returns>The constraint object</returns>
        /// <remarks>
        /// Identity is different than equality. For example, if <c>x</c> is *absent*, then <c>x == 0</c> is *absent*, but <c>x.identity(0)</c> is <see langword="false"/>.
        ///
        /// Same as <see cref="Model.Identity">Model.Identity</see>.
        /// </remarks>
        public Constraint Identity(IntExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(rhs) };
            return new Constraint(_model, "intIdentity", outParams);
        }

        /// <inheritdoc cref="Identity(IntExpr)"/>
        public Constraint Identity(int rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new Constraint(_model, "intIdentity", outParams);
        }

        /// <summary>
        /// Creates Boolean expression <c>lb</c> ≤ <c>this</c> ≤ <c>ub</c>.
        /// </summary>
        /// <param name="lb">The lower bound of the range.</param>
        /// <param name="ub">The upper bound of the range.</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If the expression has value *absent*, then the resulting expression has also value *absent*.
        ///
        /// Use <see cref="Model.Enforce">Model.Enforce</see> to add this expression as a constraint to the model.
        ///
        /// Same as <see cref="Model.InRange">Model.InRange</see>.
        /// </remarks>
        public BoolExpr InRange(int lb, int ub)
        {
            var outParams = new List<object?>() { AsArg(), lb, ub };
            return new BoolExpr(_model, "intInRange", outParams);
        }

        /// <summary>
        /// Creates a _notInRange expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr NotInRange(int lb, int ub)
        {
            var outParams = new List<object?>() { AsArg(), lb, ub };
            return new BoolExpr(_model, "intNotInRange", outParams);
        }

        /// <summary>
        /// Creates an integer expression which is absolute value of the expression.
        /// </summary>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the expression has value *absent*, the resulting expression also has value *absent*.
        ///
        /// Same as <see cref="Model.Abs">Model.Abs</see>.
        /// </remarks>
        public IntExpr Abs()
        {
            var outParams = new List<object?>() { AsArg() };
            return new IntExpr(_model, "intAbs", outParams);
        }

        /// <summary>
        /// Creates an integer expression which is the minimum of the expression and <c>arg</c>.
        /// </summary>
        /// <param name="rhs">The right-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the expression or <c>arg</c> has value *absent*, then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="Model.Min2">Model.Min2</see>. See <see cref="Model.Min">Model.Min</see> for the n-ary minimum.
        /// </remarks>
        public IntExpr Min2(IntExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(rhs) };
            return new IntExpr(_model, "intMin2", outParams);
        }

        /// <inheritdoc cref="Min2(IntExpr)"/>
        public IntExpr Min2(int rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new IntExpr(_model, "intMin2", outParams);
        }

        /// <summary>
        /// Creates an integer expression which is the maximum of the expression and <c>arg</c>.
        /// </summary>
        /// <param name="rhs">The right-hand side operand (<see cref="IntExpr">IntExpr</see> or <see langword="int"/>).</param>
        /// <returns>The resulting integer expression</returns>
        /// <remarks>
        /// If the expression or <c>arg</c> has value *absent*, then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="Model.Max2">Model.Max2</see>. See <see cref="Model.Max">Model.Max</see> for n-ary maximum.
        /// </remarks>
        public IntExpr Max2(IntExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntExpr.Wrap(rhs) };
            return new IntExpr(_model, "intMax2", outParams);
        }

        /// <inheritdoc cref="Max2(IntExpr)"/>
        public IntExpr Max2(int rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new IntExpr(_model, "intMax2", outParams);
        }


    }

    /// <summary>
    /// Boolean expression, e.g. a comparison or a logical combination of other expressions.
    /// </summary>
    /// <remarks>
    /// A class that represents a boolean expression in the model.
    /// The expression may depend on one or more variables; therefore, its value
    /// may be unknown until a solution is found.
    ///
    /// <para>Example:</para>
    /// <example>
    /// For example, the following code creates two interval variables, <c>x</c> and <c>y</c>
    /// and a boolean expression <c>precedes</c> that is true if <c>x</c> ends before <c>y</c> starts,
    /// that is, if the end of <c>x</c> is less than or equal to
    /// the start of <c>y</c>:
    ///
    /// Use standard comparison operators on expressions: <c>x.End() &lt;= y.Start()</c>.
    /// <code>
    /// var model = new Model();
    /// var x = model.IntervalVar(length: 10, name: "x");
    /// var y = model.IntervalVar(length: 20, name: "y");
    /// var precedes = x.End() &lt;= y.Start();
    /// var result = model.Solve();
    /// </code>
    ///
    /// Boolean expressions can be used to create constraints using <see cref="Model.Enforce">Model.Enforce</see>. In the example above, we may require that <c>precedes</c> is
    /// true or *absent*:
    /// <code>
    /// model.Enforce(precedes);
    /// </code>
    ///
    /// <b>Optional boolean expressions</b>
    ///
    /// _OptalCP_ is using 3-value logic: a boolean expression can be <see langword="true"/>, <see langword="false"/>
    /// or *absent*. Typically, the expression is *absent* only if one or
    /// more underlying variables are *absent*.  The value *absent*
    /// means that the expression doesn't have a meaning because one or more
    /// underlying variables are absent (not part of the solution).
    ///
    /// <b>Difference between constraints and boolean expressions</b>
    ///
    /// Boolean expressions can take arbitrary value (<see langword="true"/>, <see langword="false"/>, or *absent*)
    /// and can be combined into composed expressions (e.g., using <see cref="BoolExpr.And">BoolExpr.And</see> or
    /// <see cref="BoolExpr.Or">BoolExpr.Or</see>).
    ///
    /// Constraints can only be <see langword="true"/> or *absent* (in a solution) and cannot
    /// be combined into composed expressions.
    ///
    /// Some functions create constraints directly, e.g. <see cref="Model.NoOverlap">Model.NoOverlap</see>.
    /// It is not possible to combine constraints using logical operators like <c>or</c>.
    /// </example><para>Example:</para>
    /// <example>
    /// Let's consider a similar example to the one above but with an optional interval
    /// variables <c>a</c> and <c>b</c>:
    ///
    /// <code>
    /// var model = new Model();
    /// var a = model.IntervalVar(length: 10, name: "a", optional: true);
    /// var b = model.IntervalVar(length: 20, name: "b", optional: true);
    /// var precedes = a.End() &lt;= b.Start();
    /// model.Enforce(precedes);
    /// var result = model.Solve();
    /// </code>
    ///
    /// Adding a boolean expression as a constraint requires that the expression
    /// cannot be <see langword="false"/> in a solution. It could be *absent* though.
    /// Therefore, in our example, there are four kinds of solutions:
    ///
    /// 1. Both <c>a</c> and <c>b</c> are present, and <c>a</c> ends before <c>b</c> starts.
    /// 2. Only <c>a</c> is present, and <c>b</c> is absent.
    /// 3. Only <c>b</c> is present, and <c>a</c> is absent.
    /// 4. Both <c>a</c> and <c>b</c> are absent.
    ///
    /// In case 1, the expression <c>precedes</c> is <see langword="true"/>. In all the other cases
    /// <c>precedes</c> is *absent* as at least one of the variables <c>a</c> and <c>b</c> is
    /// absent, and then <c>precedes</c> doesn't have a meaning.
    ///
    /// <b>Boolean expressions as integer expressions</b>
    ///
    /// Class <c>BoolExpr</c> derives from <see cref="IntExpr">IntExpr</see>. Therefore, boolean expressions can be used
    /// as integer expressions. In this case, <see langword="true"/> is equal to <c>1</c>, <see langword="false"/> is
    /// equal to <c>0</c>, and *absent* remains *absent*.
    /// </example>
    /// </remarks>
    public class BoolExpr : IntExpr
    {
        internal BoolExpr(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args) { }

        internal BoolExpr(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId) { }

        internal static object Wrap(BoolExpr expr) => expr.AsArg();
        internal static object Wrap(bool value) => value;

        internal static List<object> WrapList(IEnumerable<BoolExpr> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }

        internal static new List<object> WrapMixedList(IEnumerable<object> items)
        {
            var result = new List<object>();
            foreach (var item in items)
            {
                if (item is BoolExpr expr)
                    result.Add(expr.AsArg());
                else if (item is bool b)
                    result.Add(b);
                else
                    throw new ArgumentException($"Expected BoolExpr or bool, got {item?.GetType().Name}");
            }
            return result;
        }

        /// <summary>
        /// Negate a boolean expression using the <c>!</c> operator.
        /// </summary>
        /// <returns>A boolean expression that is true when the operand is false.</returns>
        /// <remarks>
        /// Returns a <see cref="BoolExpr">BoolExpr</see> representing <c>!expr</c>.
        ///
        /// If the operand is *absent*, the result is also *absent*.
        ///
        /// Operator precedence: In C#, <c>!</c> has higher precedence than comparison operators. Use parentheses when negating a comparison:
        ///
        /// <code>
        /// // Wrong: !a == b parses as (!a) == b
        /// // Correct: negate an equality
        /// model.Enforce(!(a == b));
        /// </code>
        ///
        /// <code>
        /// var notX = !x;
        /// model.Enforce(!x | y); // x implies y
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Not">BoolExpr.Not</see> — for method alternatives.</description></item>
        /// <item><description><see cref="Model.Not">Model.Not</see> — for method alternatives.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator !(BoolExpr expr)
            => new BoolExpr(expr._model, "boolNot", new object[] { expr.AsArg() });

        /// <summary>
        /// Create a logical AND of boolean expressions using the <c>&amp;</c> operator.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>A boolean expression that is true when both operands are true.</returns>
        /// <remarks>
        /// Returns a <see cref="BoolExpr">BoolExpr</see> representing <c>lhs &amp; rhs</c>. All three overloads are supported:
        /// <see cref="BoolExpr">BoolExpr</see> <c>&amp;</c> <see cref="BoolExpr">BoolExpr</see>, <see cref="BoolExpr">BoolExpr</see> <c>&amp;</c> <see langword="bool"/>, and <see langword="bool"/> <c>&amp;</c> <see cref="BoolExpr">BoolExpr</see>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// Note: use <c>&amp;</c> (bitwise AND), not <c>&amp;&amp;</c> (short-circuit AND), as C# does not allow overloading <c>&amp;&amp;</c>.
        ///
        /// Operator precedence: In C#, <c>&amp;</c> has lower precedence than comparison operators (<c>==</c>, <c>!=</c>, <c>&lt;</c>, etc.). Use parentheses when combining with comparisons:
        ///
        /// <code>
        /// // Wrong: a == b &amp; c == d parses as a == (b &amp; c) == d
        /// // Correct:
        /// model.Enforce((a == b) &amp; (c == d));
        /// </code>
        ///
        /// <code>
        /// model.Enforce(x &amp; y);        // both must be true
        /// model.Enforce(x &amp; true);     // equivalent to x
        /// model.Enforce(false &amp; x);    // always false
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.And">BoolExpr.And</see> — for method alternatives that avoid precedence issues.</description></item>
        /// <item><description><see cref="Model.And">Model.And</see> — for method alternatives that avoid precedence issues.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator &(BoolExpr lhs, BoolExpr rhs)
            => new BoolExpr(lhs._model, "boolAnd", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create a logical OR of boolean expressions using the <c>|</c> operator.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>A boolean expression that is true when at least one operand is true.</returns>
        /// <remarks>
        /// Returns a <see cref="BoolExpr">BoolExpr</see> representing <c>lhs | rhs</c>. All three overloads are supported:
        /// <see cref="BoolExpr">BoolExpr</see> <c>|</c> <see cref="BoolExpr">BoolExpr</see>, <see cref="BoolExpr">BoolExpr</see> <c>|</c> <see langword="bool"/>, and <see langword="bool"/> <c>|</c> <see cref="BoolExpr">BoolExpr</see>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// Note: use <c>|</c> (bitwise OR), not <c>||</c> (short-circuit OR), as C# does not allow overloading <c>||</c>.
        ///
        /// Operator precedence: In C#, <c>|</c> has lower precedence than comparison operators (<c>==</c>, <c>!=</c>, <c>&lt;</c>, etc.). Use parentheses when combining with comparisons:
        ///
        /// <code>
        /// // Wrong: a == b | c == d parses as a == (b | c) == d
        /// // Correct:
        /// model.Enforce((a == b) | (c == d));
        /// </code>
        ///
        /// <code>
        /// model.Enforce(x | y);        // at least one must be true
        /// model.Enforce(x | false);    // equivalent to x
        /// model.Enforce(true | x);     // always true
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Or">BoolExpr.Or</see> — for method alternatives that avoid precedence issues.</description></item>
        /// <item><description><see cref="Model.Or">Model.Or</see> — for method alternatives that avoid precedence issues.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator |(BoolExpr lhs, BoolExpr rhs)
            => new BoolExpr(lhs._model, "boolOr", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Create a logical AND of boolean expressions using the <c>&amp;</c> operator.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>A boolean expression that is true when both operands are true.</returns>
        /// <remarks>
        /// Returns a <see cref="BoolExpr">BoolExpr</see> representing <c>lhs &amp; rhs</c>. All three overloads are supported:
        /// <see cref="BoolExpr">BoolExpr</see> <c>&amp;</c> <see cref="BoolExpr">BoolExpr</see>, <see cref="BoolExpr">BoolExpr</see> <c>&amp;</c> <see langword="bool"/>, and <see langword="bool"/> <c>&amp;</c> <see cref="BoolExpr">BoolExpr</see>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// Note: use <c>&amp;</c> (bitwise AND), not <c>&amp;&amp;</c> (short-circuit AND), as C# does not allow overloading <c>&amp;&amp;</c>.
        ///
        /// Operator precedence: In C#, <c>&amp;</c> has lower precedence than comparison operators (<c>==</c>, <c>!=</c>, <c>&lt;</c>, etc.). Use parentheses when combining with comparisons:
        ///
        /// <code>
        /// // Wrong: a == b &amp; c == d parses as a == (b &amp; c) == d
        /// // Correct:
        /// model.Enforce((a == b) &amp; (c == d));
        /// </code>
        ///
        /// <code>
        /// model.Enforce(x &amp; y);        // both must be true
        /// model.Enforce(x &amp; true);     // equivalent to x
        /// model.Enforce(false &amp; x);    // always false
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.And">BoolExpr.And</see> — for method alternatives that avoid precedence issues.</description></item>
        /// <item><description><see cref="Model.And">Model.And</see> — for method alternatives that avoid precedence issues.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator &(BoolExpr lhs, bool rhs)
            => new BoolExpr(lhs._model, "boolAnd", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create a logical AND of boolean expressions using the <c>&amp;</c> operator.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>A boolean expression that is true when both operands are true.</returns>
        /// <remarks>
        /// Returns a <see cref="BoolExpr">BoolExpr</see> representing <c>lhs &amp; rhs</c>. All three overloads are supported:
        /// <see cref="BoolExpr">BoolExpr</see> <c>&amp;</c> <see cref="BoolExpr">BoolExpr</see>, <see cref="BoolExpr">BoolExpr</see> <c>&amp;</c> <see langword="bool"/>, and <see langword="bool"/> <c>&amp;</c> <see cref="BoolExpr">BoolExpr</see>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// Note: use <c>&amp;</c> (bitwise AND), not <c>&amp;&amp;</c> (short-circuit AND), as C# does not allow overloading <c>&amp;&amp;</c>.
        ///
        /// Operator precedence: In C#, <c>&amp;</c> has lower precedence than comparison operators (<c>==</c>, <c>!=</c>, <c>&lt;</c>, etc.). Use parentheses when combining with comparisons:
        ///
        /// <code>
        /// // Wrong: a == b &amp; c == d parses as a == (b &amp; c) == d
        /// // Correct:
        /// model.Enforce((a == b) &amp; (c == d));
        /// </code>
        ///
        /// <code>
        /// model.Enforce(x &amp; y);        // both must be true
        /// model.Enforce(x &amp; true);     // equivalent to x
        /// model.Enforce(false &amp; x);    // always false
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.And">BoolExpr.And</see> — for method alternatives that avoid precedence issues.</description></item>
        /// <item><description><see cref="Model.And">Model.And</see> — for method alternatives that avoid precedence issues.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator &(bool lhs, BoolExpr rhs)
            => new BoolExpr(rhs._model, "boolAnd", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Create a logical OR of boolean expressions using the <c>|</c> operator.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>A boolean expression that is true when at least one operand is true.</returns>
        /// <remarks>
        /// Returns a <see cref="BoolExpr">BoolExpr</see> representing <c>lhs | rhs</c>. All three overloads are supported:
        /// <see cref="BoolExpr">BoolExpr</see> <c>|</c> <see cref="BoolExpr">BoolExpr</see>, <see cref="BoolExpr">BoolExpr</see> <c>|</c> <see langword="bool"/>, and <see langword="bool"/> <c>|</c> <see cref="BoolExpr">BoolExpr</see>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// Note: use <c>|</c> (bitwise OR), not <c>||</c> (short-circuit OR), as C# does not allow overloading <c>||</c>.
        ///
        /// Operator precedence: In C#, <c>|</c> has lower precedence than comparison operators (<c>==</c>, <c>!=</c>, <c>&lt;</c>, etc.). Use parentheses when combining with comparisons:
        ///
        /// <code>
        /// // Wrong: a == b | c == d parses as a == (b | c) == d
        /// // Correct:
        /// model.Enforce((a == b) | (c == d));
        /// </code>
        ///
        /// <code>
        /// model.Enforce(x | y);        // at least one must be true
        /// model.Enforce(x | false);    // equivalent to x
        /// model.Enforce(true | x);     // always true
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Or">BoolExpr.Or</see> — for method alternatives that avoid precedence issues.</description></item>
        /// <item><description><see cref="Model.Or">Model.Or</see> — for method alternatives that avoid precedence issues.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator |(BoolExpr lhs, bool rhs)
            => new BoolExpr(lhs._model, "boolOr", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Create a logical OR of boolean expressions using the <c>|</c> operator.
        /// </summary>
        /// <param name="lhs">The left-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>A boolean expression that is true when at least one operand is true.</returns>
        /// <remarks>
        /// Returns a <see cref="BoolExpr">BoolExpr</see> representing <c>lhs | rhs</c>. All three overloads are supported:
        /// <see cref="BoolExpr">BoolExpr</see> <c>|</c> <see cref="BoolExpr">BoolExpr</see>, <see cref="BoolExpr">BoolExpr</see> <c>|</c> <see langword="bool"/>, and <see langword="bool"/> <c>|</c> <see cref="BoolExpr">BoolExpr</see>.
        ///
        /// If either operand is *absent*, the result is also *absent*.
        ///
        /// Note: use <c>|</c> (bitwise OR), not <c>||</c> (short-circuit OR), as C# does not allow overloading <c>||</c>.
        ///
        /// Operator precedence: In C#, <c>|</c> has lower precedence than comparison operators (<c>==</c>, <c>!=</c>, <c>&lt;</c>, etc.). Use parentheses when combining with comparisons:
        ///
        /// <code>
        /// // Wrong: a == b | c == d parses as a == (b | c) == d
        /// // Correct:
        /// model.Enforce((a == b) | (c == d));
        /// </code>
        ///
        /// <code>
        /// model.Enforce(x | y);        // at least one must be true
        /// model.Enforce(x | false);    // equivalent to x
        /// model.Enforce(true | x);     // always true
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="BoolExpr.Or">BoolExpr.Or</see> — for method alternatives that avoid precedence issues.</description></item>
        /// <item><description><see cref="Model.Or">Model.Or</see> — for method alternatives that avoid precedence issues.</description></item>
        /// </list>
        /// </remarks>
        public static BoolExpr operator |(bool lhs, BoolExpr rhs)
            => new BoolExpr(rhs._model, "boolOr", new object[] { lhs, rhs.AsArg() });

        /// <summary>
        /// Adds this boolean expression as a constraint to the model.
        /// </summary>
        /// <remarks>
        /// This method adds the boolean expression as a constraint to the model. It provides
        /// a fluent-style alternative to <see cref="Model.Enforce">Model.Enforce</see>.
        ///
        /// A constraint is satisfied if it is not <see langword="false"/>. In other words, a constraint is
        /// satisfied if it is <see langword="true"/> or *absent*.
        ///
        /// A boolean expression that is *not* added as a constraint can have
        /// arbitrary value in a solution (<see langword="true"/>, <see langword="false"/>, or *absent*). Once added
        /// as a constraint, it can only be <see langword="true"/> or *absent* in the solution.
        ///
        /// <para>Example:</para>
        /// <example>
        /// <code>
        /// var model = new Model();
        /// var x = model.IntVar(min: 0, max: 10, name: "x");
        /// var y = model.IntVar(min: 0, max: 10, name: "y");
        ///
        /// // Enforce constraint using fluent style
        /// (x + y &lt;= 15).Enforce();
        ///
        /// // Equivalent to:
        /// // model.Enforce(x + y &lt;= 15);
        ///
        /// var result = model.Solve();
        /// </code>
        /// </example>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Enforce">Model.Enforce</see> — for the Model-centric style of adding constraints.</description></item>
        /// <item><description><see cref="BoolExpr">BoolExpr</see> — for more about boolean expressions.</description></item>
        /// </list>
        /// </remarks>
        public void Enforce()
        {
            _model.Enforce(this);
        }

        /// <summary>
        /// Creates a _reusableBoolExpr expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr ReusableBoolExpr()
        {
            var outParams = new List<object?>() { AsArg() };
            return new BoolExpr(_model, "reusableBoolExpr", outParams);
        }

        /// <summary>
        /// Returns negation of the expression.
        /// </summary>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If the expression has value *absent* then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="Model.Not">Model.Not</see>.
        /// </remarks>
        public BoolExpr Not()
        {
            var outParams = new List<object?>() { AsArg() };
            return new BoolExpr(_model, "boolNot", outParams);
        }

        /// <summary>
        /// Returns logical _OR_ of the expression and <c>arg</c>.
        /// </summary>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If the expression or <c>arg</c> has value *absent* then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="Model.Or">Model.Or</see>.
        /// </remarks>
        public BoolExpr Or(BoolExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(_model, "boolOr", outParams);
        }

        /// <inheritdoc cref="Or(BoolExpr)"/>
        public BoolExpr Or(bool rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new BoolExpr(_model, "boolOr", outParams);
        }

        /// <summary>
        /// Returns logical _AND_ of the expression and <c>arg</c>.
        /// </summary>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If the expression or <c>arg</c> has value *absent*, then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="Model.And">Model.And</see>.
        /// </remarks>
        public BoolExpr And(BoolExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(_model, "boolAnd", outParams);
        }

        /// <inheritdoc cref="And(BoolExpr)"/>
        public BoolExpr And(bool rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new BoolExpr(_model, "boolAnd", outParams);
        }

        /// <summary>
        /// Returns implication between the expression and <c>arg</c>.
        /// </summary>
        /// <param name="rhs">The right-hand side operand (<see cref="BoolExpr">BoolExpr</see> or <see langword="bool"/>).</param>
        /// <returns>The resulting Boolean expression</returns>
        /// <remarks>
        /// If the expression or <c>arg</c> has value *absent*, then the resulting expression has also value *absent*.
        ///
        /// Same as <see cref="Model.Implies">Model.Implies</see>.
        /// </remarks>
        public BoolExpr Implies(BoolExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(_model, "boolImplies", outParams);
        }

        /// <inheritdoc cref="Implies(BoolExpr)"/>
        public BoolExpr Implies(bool rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new BoolExpr(_model, "boolImplies", outParams);
        }

        /// <summary>
        /// Creates a _eq expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Eq(BoolExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(_model, "boolEq", outParams);
        }

        /// <summary>
        /// Creates a _eq expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Eq(bool rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new BoolExpr(_model, "boolEq", outParams);
        }

        /// <summary>
        /// Creates a _ne expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Ne(BoolExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(_model, "boolNe", outParams);
        }

        /// <summary>
        /// Creates a _ne expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Ne(bool rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new BoolExpr(_model, "boolNe", outParams);
        }

        /// <summary>
        /// Creates a _nand expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Nand(BoolExpr rhs)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.BoolExpr.Wrap(rhs) };
            return new BoolExpr(_model, "boolNand", outParams);
        }

        /// <summary>
        /// Creates a _nand expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BoolExpr Nand(bool rhs)
        {
            var outParams = new List<object?>() { AsArg(), rhs };
            return new BoolExpr(_model, "boolNand", outParams);
        }


    }

    /// <summary>
    /// Tracks cumulative resource usage over time for capacity constraints.
    /// </summary>
    /// <remarks>
    /// Cumulative expression.
    ///
    /// Cumulative expression represents resource usage over time.  The resource
    /// could be a machine, a group of workers, a material, or anything of a limited
    /// capacity.  The resource usage is not known in advance as it depends on the
    /// variables of the problem.  Cumulative expressions allow us to model the resource
    /// usage and constrain it.
    ///
    /// Basic cumulative expressions are:
    ///
    /// * *Pulse*: the resource is used over an interval of time.
    ///   For example, a pulse can represent a task requiring a certain
    ///   number of workers during its execution.  At the beginning of the interval,
    ///   the resource usage increases by a given amount, and at the end of the
    ///   interval, the resource usage decreases by the same amount.
    ///   Pulse can be created by function <see cref="Model.Pulse">Model.Pulse</see>
    ///   or <see cref="IntervalVar.Pulse">IntervalVar.Pulse</see>.
    /// * *Step*: a given amount of resource is consumed or produced at a specified
    ///   time (e.g., at the start of an interval variable).
    ///   Steps may represent an inventory of a material that is
    ///   consumed or produced by some tasks (a _reservoir_).
    ///   Steps can be created by functions
    ///   <see cref="Model.StepAtStart">Model.StepAtStart</see>,
    ///   <see cref="IntervalVar.StepAtStart">IntervalVar.StepAtStart</see>,
    ///   <see cref="Model.StepAtEnd">Model.StepAtEnd</see>,
    ///   <see cref="IntervalVar.StepAtEnd">IntervalVar.StepAtEnd</see>. and
    ///   <see cref="Model.StepAt">Model.StepAt</see>.
    ///
    /// Cumulative expressions can be combined using operators (<c>+</c>, <c>-</c>, unary <c>-</c>) and
    /// <see cref="Model.Sum">Model.Sum</see>. The resulting cumulative expression represents
    /// a sum of the resource usage of the combined expressions.
    ///
    /// Cumulative expressions can be constrained using comparison operators (<c>&lt;=</c>, <c>&gt;=</c>)
    /// to specify the minimum and maximum allowed resource usage.
    ///
    /// Limitations:
    ///
    /// * Pulse-based and step-based cumulative expressions cannot be mixed.
    /// * Pulses cannot have negative height. Use <c>-</c> and unary <c>-</c> only with step-based expressions.
    ///
    /// See <see cref="Model.Pulse">Model.Pulse</see> for examples.
    /// </remarks>
    public class CumulExpr : ModelElement
    {
        internal CumulExpr(Model model, string funcName, IEnumerable<object?>? args = null)
            : base(model, funcName, args) { }

        internal CumulExpr(Model model, Dictionary<string, object?> props, int? refId = null)
            : base(model, props, refId) { }

        internal static object Wrap(CumulExpr expr) => expr.AsArg();

        internal static List<object> WrapList(IEnumerable<CumulExpr> items)
        {
            var result = new List<object>();
            foreach (var item in items)
                result.Add(item.AsArg());
            return result;
        }

        /// <summary>
        /// Add two cumulative expressions using the <c>+</c> operator.
        /// </summary>
        /// <returns>A new cumulative expression representing the sum.</returns>
        /// <remarks>
        /// Returns a new <see cref="CumulExpr">CumulExpr</see> representing the combined resource usage of two cumulative expressions.
        ///
        /// Use this to combine resource usages from different sources. For example, combining the cumulative usage of two different task types on the same resource.
        ///
        /// Limitation: Currently, pulse-based and step-based cumulative expressions cannot be mixed. You can add pulses to pulses and steps to steps, but not pulses to steps.
        ///
        /// <code>
        /// var total = workers + machines;
        /// model.Enforce(total &lt;= 10);
        /// </code>
        ///
        /// See also:
        /// <list type="bullet">
        /// <item><description><see cref="Model.Sum">Model.Sum</see> — for summing multiple cumulative expressions.</description></item>
        /// </list>
        /// </remarks>
        public static CumulExpr operator +(CumulExpr lhs, CumulExpr rhs)
            => new CumulExpr(lhs._model, "cumulPlus", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Subtract two cumulative expressions using the <c>-</c> operator.
        /// </summary>
        /// <returns>A new cumulative expression representing the difference.</returns>
        /// <remarks>
        /// Returns a new <see cref="CumulExpr">CumulExpr</see> representing the difference of two cumulative expressions.
        ///
        /// Limitation: This operator can only be used with step-based cumulative expressions, not with pulses. Pulses cannot have negative height.
        ///
        /// Use this to compute the net effect of additions and removals. For example, tracking inventory where items are added and removed over time.
        ///
        /// <code>
        /// var net = produced - consumed;
        /// model.Enforce(net &gt;= 0);
        /// </code>
        /// </remarks>
        public static CumulExpr operator -(CumulExpr lhs, CumulExpr rhs)
            => new CumulExpr(lhs._model, "cumulMinus", new object[] { lhs.AsArg(), rhs.AsArg() });

        /// <summary>
        /// Negate a cumulative expression using the unary <c>-</c> operator.
        /// </summary>
        /// <returns>A new cumulative expression representing the negation.</returns>
        /// <remarks>
        /// Returns a new <see cref="CumulExpr">CumulExpr</see> representing <c>-expr</c>.
        ///
        /// Limitation: This operator should only be used with step-based cumulative expressions, not with pulses. Pulses cannot have negative height.
        ///
        /// Negating a cumulative expression flips the sign of all its values over time. Useful for modeling resource production as the negation of consumption.
        ///
        /// <code>
        /// var production = -consumption;
        /// </code>
        /// </remarks>
        public static CumulExpr operator -(CumulExpr expr)
            => new CumulExpr(expr._model, "cumulNeg", new object[] { expr.AsArg() });

        /// <summary>
        /// Constrain a cumulative expression to be at most a bound using <c>&lt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression does not exceed the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at most the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitations:
        ///
        /// - Variable capacity (using <see cref="IntExpr">IntExpr</see>) is only supported for discrete resources (pulses). Reservoir resources (steps) require a constant capacity.
        /// - The capacity expression must not be optional or absent.
        ///
        /// All equivalent forms are supported: <c>cumul &lt;= bound</c> and <c>bound &gt;= cumul</c>.
        ///
        /// <code>
        /// // Limit resource usage to 5 workers at any time
        /// model.Enforce(workerUsage &lt;= 5);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &lt;= capacity);
        /// </code>
        ///
        /// See also the <c>&gt;=</c> operator for the minimum capacity constraint.
        /// </remarks>
        public static Constraint operator <=(CumulExpr lhs, int rhs)
            => new Constraint(lhs._model, "cumulLe", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Constrain a cumulative expression to be at least a bound using <c>&gt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression meets or exceeds the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at least the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitation: This constraint can only be used with step-based cumulative expressions, not with pulses.
        ///
        /// Important: The constraint requires the minimum level to be maintained *everywhere*, including at the beginning of time. If the required level is greater than 0, you must include a step at <c>Model.IntervalMin</c> (not at 0) to establish the initial level.
        ///
        /// All equivalent forms are supported: <c>cumul &gt;= bound</c> and <c>bound &lt;= cumul</c>.
        ///
        /// <code>
        /// // Require at least 2 workers active at any time
        /// model.Enforce(workerUsage &gt;= 2);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &gt;= minRequired);
        /// </code>
        ///
        /// See also the <c>&lt;=</c> operator for the maximum capacity constraint.
        /// </remarks>
        public static Constraint operator >=(CumulExpr lhs, int rhs)
            => new Constraint(lhs._model, "cumulGe", new object[] { lhs.AsArg(), rhs });

        /// <summary>
        /// Constrain a cumulative expression to be at least a bound using <c>&gt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression meets or exceeds the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at least the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitation: This constraint can only be used with step-based cumulative expressions, not with pulses.
        ///
        /// Important: The constraint requires the minimum level to be maintained *everywhere*, including at the beginning of time. If the required level is greater than 0, you must include a step at <c>Model.IntervalMin</c> (not at 0) to establish the initial level.
        ///
        /// All equivalent forms are supported: <c>cumul &gt;= bound</c> and <c>bound &lt;= cumul</c>.
        ///
        /// <code>
        /// // Require at least 2 workers active at any time
        /// model.Enforce(workerUsage &gt;= 2);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &gt;= minRequired);
        /// </code>
        ///
        /// See also the <c>&lt;=</c> operator for the maximum capacity constraint.
        /// </remarks>
        public static Constraint operator <=(int lhs, CumulExpr rhs)
            => new Constraint(rhs._model, "cumulGe", new object[] { rhs.AsArg(), lhs });

        /// <summary>
        /// Constrain a cumulative expression to be at most a bound using <c>&lt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression does not exceed the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at most the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitations:
        ///
        /// - Variable capacity (using <see cref="IntExpr">IntExpr</see>) is only supported for discrete resources (pulses). Reservoir resources (steps) require a constant capacity.
        /// - The capacity expression must not be optional or absent.
        ///
        /// All equivalent forms are supported: <c>cumul &lt;= bound</c> and <c>bound &gt;= cumul</c>.
        ///
        /// <code>
        /// // Limit resource usage to 5 workers at any time
        /// model.Enforce(workerUsage &lt;= 5);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &lt;= capacity);
        /// </code>
        ///
        /// See also the <c>&gt;=</c> operator for the minimum capacity constraint.
        /// </remarks>
        public static Constraint operator >=(int lhs, CumulExpr rhs)
            => new Constraint(rhs._model, "cumulLe", new object[] { rhs.AsArg(), lhs });

        // Variable capacity overloads (IntExpr bounds)

        /// <summary>
        /// Constrain a cumulative expression to be at most a bound using <c>&lt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression does not exceed the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at most the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitations:
        ///
        /// - Variable capacity (using <see cref="IntExpr">IntExpr</see>) is only supported for discrete resources (pulses). Reservoir resources (steps) require a constant capacity.
        /// - The capacity expression must not be optional or absent.
        ///
        /// All equivalent forms are supported: <c>cumul &lt;= bound</c> and <c>bound &gt;= cumul</c>.
        ///
        /// <code>
        /// // Limit resource usage to 5 workers at any time
        /// model.Enforce(workerUsage &lt;= 5);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &lt;= capacity);
        /// </code>
        ///
        /// See also the <c>&gt;=</c> operator for the minimum capacity constraint.
        /// </remarks>
        public static Constraint operator <=(CumulExpr lhs, IntExpr rhs)
            => new Constraint(lhs._model, "cumulLe", new object[] { lhs.AsArg(), IntExpr.Wrap(rhs) });

        /// <summary>
        /// Constrain a cumulative expression to be at least a bound using <c>&gt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression meets or exceeds the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at least the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitation: This constraint can only be used with step-based cumulative expressions, not with pulses.
        ///
        /// Important: The constraint requires the minimum level to be maintained *everywhere*, including at the beginning of time. If the required level is greater than 0, you must include a step at <c>Model.IntervalMin</c> (not at 0) to establish the initial level.
        ///
        /// All equivalent forms are supported: <c>cumul &gt;= bound</c> and <c>bound &lt;= cumul</c>.
        ///
        /// <code>
        /// // Require at least 2 workers active at any time
        /// model.Enforce(workerUsage &gt;= 2);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &gt;= minRequired);
        /// </code>
        ///
        /// See also the <c>&lt;=</c> operator for the maximum capacity constraint.
        /// </remarks>
        public static Constraint operator >=(CumulExpr lhs, IntExpr rhs)
            => new Constraint(lhs._model, "cumulGe", new object[] { lhs.AsArg(), IntExpr.Wrap(rhs) });

        /// <summary>
        /// Constrain a cumulative expression to be at least a bound using <c>&gt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression meets or exceeds the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at least the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitation: This constraint can only be used with step-based cumulative expressions, not with pulses.
        ///
        /// Important: The constraint requires the minimum level to be maintained *everywhere*, including at the beginning of time. If the required level is greater than 0, you must include a step at <c>Model.IntervalMin</c> (not at 0) to establish the initial level.
        ///
        /// All equivalent forms are supported: <c>cumul &gt;= bound</c> and <c>bound &lt;= cumul</c>.
        ///
        /// <code>
        /// // Require at least 2 workers active at any time
        /// model.Enforce(workerUsage &gt;= 2);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &gt;= minRequired);
        /// </code>
        ///
        /// See also the <c>&lt;=</c> operator for the maximum capacity constraint.
        /// </remarks>
        public static Constraint operator <=(IntExpr lhs, CumulExpr rhs)
            => new Constraint(rhs._model, "cumulGe", new object[] { rhs.AsArg(), IntExpr.Wrap(lhs) });

        /// <summary>
        /// Constrain a cumulative expression to be at most a bound using <c>&lt;=</c>.
        /// </summary>
        /// <returns>A constraint that the cumulative expression does not exceed the bound.</returns>
        /// <remarks>
        /// Returns a <see cref="Constraint">Constraint</see> enforcing that the cumulative expression is at most the given bound
        /// at every point in time. The bound can be an <see langword="int"/> constant or an <see cref="IntExpr">IntExpr</see>.
        ///
        /// Limitations:
        ///
        /// - Variable capacity (using <see cref="IntExpr">IntExpr</see>) is only supported for discrete resources (pulses). Reservoir resources (steps) require a constant capacity.
        /// - The capacity expression must not be optional or absent.
        ///
        /// All equivalent forms are supported: <c>cumul &lt;= bound</c> and <c>bound &gt;= cumul</c>.
        ///
        /// <code>
        /// // Limit resource usage to 5 workers at any time
        /// model.Enforce(workerUsage &lt;= 5);
        ///
        /// // With an integer expression bound
        /// model.Enforce(workerUsage &lt;= capacity);
        /// </code>
        ///
        /// See also the <c>&gt;=</c> operator for the minimum capacity constraint.
        /// </remarks>
        public static Constraint operator >=(IntExpr lhs, CumulExpr rhs)
            => new Constraint(rhs._model, "cumulLe", new object[] { rhs.AsArg(), IntExpr.Wrap(lhs) });

        /// <summary>
        /// Creates a _cumulMaxProfile expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint CumulMaxProfile(IntStepFunction profile)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntStepFunction.Wrap(profile) };
            return new Constraint(_model, "cumulMaxProfile", outParams);
        }

        /// <summary>
        /// Creates a _cumulMinProfile expression.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Constraint CumulMinProfile(IntStepFunction profile)
        {
            var outParams = new List<object?>() { AsArg(), OptalCP.IntStepFunction.Wrap(profile) };
            return new Constraint(_model, "cumulMinProfile", outParams);
        }


    }
}
