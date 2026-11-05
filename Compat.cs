// ===========================================================================
// OptalCP C# API - Compatibility Shims
// Version: 2026.11.0
// ===========================================================================

// The package targets net10.0 and netstandard2.0. The netstandard2.0 build
// exists for .NET Framework consumers. These helpers wrap the framework APIs
// that netstandard2.0 lacks. On net10.0 each one forwards to the framework.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    // The compiler recognizes this attribute by name. Declaring it makes
    // Compat.ThrowIfNull report the caller's argument name, as on net10.0.
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName) => ParameterName = parameterName;
        public string ParameterName { get; }
    }
}
#endif

namespace OptalCP
{
    internal static class Compat
    {
        /// <summary>
        /// Equivalent of <c>ArgumentNullException.ThrowIfNull</c>.
        /// </summary>
        public static void ThrowIfNull(object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
#if NETSTANDARD2_0
            if (argument is null)
                throw new ArgumentNullException(paramName);
#else
            ArgumentNullException.ThrowIfNull(argument, paramName);
#endif
        }

        /// <summary>
        /// Appends one command-line argument to <paramref name="psi"/>.
        /// </summary>
        public static void AddArgument(ProcessStartInfo psi, string argument)
        {
#if NETSTANDARD2_0
            // ProcessStartInfo.Arguments is a single string. Windows hands it to
            // the started process, which splits it by the CRT rules. .NET on
            // Unix splits it by the same rules before exec.
            if (psi.Arguments.Length > 0)
                psi.Arguments += " ";
            psi.Arguments += QuoteArgument(argument);
#else
            psi.ArgumentList.Add(argument);
#endif
        }

        /// <summary>
        /// Kills the process. On net10.0 it kills the whole process tree.
        /// </summary>
        /// <remarks>
        /// On netstandard2.0 only the process itself is killed. The solver in
        /// server mode starts no child processes, so nothing is left behind.
        /// </remarks>
        public static void KillTree(Process process)
        {
#if NETSTANDARD2_0
            process.Kill();
#else
            process.Kill(entireProcessTree: true);
#endif
        }

        /// <summary>
        /// Quotes one argument by the Windows CRT command-line rules, the same
        /// rules <c>ProcessStartInfo.ArgumentList</c> applies on modern .NET.
        /// </summary>
        /// <remarks>
        /// Only the netstandard2.0 build calls it. It is compiled on every
        /// target so that the tests can check it.
        /// </remarks>
        internal static string QuoteArgument(string argument)
        {
            if (argument.Length > 0 && argument.IndexOfAny(new[] { ' ', '\t', '\n', '\v', '"' }) < 0)
                return argument;

            // Backslashes are literal unless a quote follows them. So a run of
            // backslashes is doubled before an escaped quote and before the
            // closing quote, and copied as is elsewhere:
            var sb = new StringBuilder();
            sb.Append('"');
            for (int i = 0; i < argument.Length; i++)
            {
                int backslashes = 0;
                while (i < argument.Length && argument[i] == '\\')
                {
                    backslashes++;
                    i++;
                }

                if (i == argument.Length)
                {
                    sb.Append('\\', backslashes * 2);
                    break;
                }

                if (argument[i] == '"')
                    sb.Append('\\', backslashes * 2 + 1);
                else
                    sb.Append('\\', backslashes);
                sb.Append(argument[i]);
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
