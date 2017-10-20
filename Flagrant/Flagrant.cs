using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Flagrant
{
    /// <summary>
    /// Flagrant keeps a dictionary mapping arguments to values.
    /// Methods allows you to extract the values with the given type
    /// </summary>
    public class Flagrant
    {
        private readonly Dictionary<string, string> _arguments;

        /// <summary>
        /// Creates a new instance of Flagrant that will use the given
        /// dictionary of argument/value pairs. It is recommended to use
        /// the <see cref="Parse"/> static method to parse a typical arguments
        /// array.
        /// </summary>
        /// <param name="arguments">Argument/value pairs</param>
        public Flagrant(Dictionary<string, string> arguments)
        {
            _arguments = arguments;
        }

        private T Run<T>(string flag, T defaultValue, Func<string, T> func)
        {
            return _arguments.TryGetValue(flag, out var value) ? func.Invoke(value) : defaultValue;
        }

        /// <summary>
        /// Looks for the given flag and sets <c>value</c> to the found value
        /// or <c>defaultValue</c> if value does not exist.
        /// </summary>
        /// <param name="flag">Flag to look for</param>
        /// <param name="defaultValue">Default value to out</param>
        /// <param name="value">Variable that will be set</param>
        /// <returns></returns>
        public Flagrant String(string flag, string defaultValue, out string value)
        {
            value = Run(flag, defaultValue, v => v ?? defaultValue);
            return this;
        }
        
        /// <summary>
        /// Looks for the given flag and if a value exists it will try to parse
        /// it as an int. If successful, <c>value</c> will be set to the parsed integer.
        /// Otherwise it will be set to <c>defaultValue</c>
        /// </summary>
        /// <param name="flag">Flag to look for</param>
        /// <param name="defaultValue">Default value to out</param>
        /// <param name="value">Variable that will be set</param>
        /// <returns></returns>
        public Flagrant Int(string flag, int defaultValue, out int value)
        {
            value = Run(flag, defaultValue, v => int.TryParse(v, out var iv) ? iv : defaultValue);
            return this;
        }
        
        /// <summary>
        /// Looks for the given flag and if a value exists it will try to parse
        /// it as a long. If successful, <c>value</c> will be set to the parsed long.
        /// Otherwise it will be set to <c>defaultValue</c>
        /// </summary>
        /// <param name="flag">Flag to look for</param>
        /// <param name="defaultValue">Default value to out</param>
        /// <param name="value">Variable that will be set</param>
        /// <returns></returns>
        public Flagrant Long(string flag, long defaultValue, out long value)
        {
            value = Run(flag, defaultValue, v => long.TryParse(v, out var lv) ? lv : defaultValue);
            return this;
        }

        /// <summary>
        /// Looks for the given flag and if a value exists it will try to parse
        /// it as a float. If successful, <c>value</c> will be set to the parsed float.
        /// Otherwise it will be set to <c>defaultValue</c>
        /// </summary>
        /// <param name="flag">Flag to look for</param>
        /// <param name="defaultValue">Default value to out</param>
        /// <param name="value">Variable that will be set</param>
        /// <returns></returns>
        public Flagrant Float(string flag, float defaultValue, out float value)
        {
            value = Run(flag, defaultValue, v => float.TryParse(v, out var fv) ? fv : defaultValue);
            return this;
        }
        
        /// <summary>
        /// Looks for the given flag and if a value exists it will try to parse
        /// it as a double. If successful, <c>value</c> will be set to the parsed double.
        /// Otherwise it will be set to <c>defaultValue</c>
        /// </summary>
        /// <param name="flag">Flag to look for</param>
        /// <param name="defaultValue">Default value to out</param>
        /// <param name="value">Variable that will be set</param>
        /// <returns></returns>
        public Flagrant Double(string flag, double defaultValue, out double value)
        {
            value = Run(flag, defaultValue, v => double.TryParse(v, out var dv) ? dv : defaultValue);
            return this;
        }

        /// <summary>
        /// Looks for the given flag. If the flag exists in the dictionary,
        /// checks if the flag is appended with <c>=false</c>. If it is,
        /// <c>value</c> will be set to false, otherwise true. If no flag exists
        /// <c>value</c> will be set to <c>defaultValue</c>
        /// </summary>
        /// <param name="flag">Flag to look for</param>
        /// <param name="defaultValue">Default value to out</param>
        /// <param name="value">Variable that will be set</param>
        /// <returns></returns>
        public Flagrant Bool(string flag, bool defaultValue, out bool value)
        {
            value = Run(flag, defaultValue, v =>
            {
                if (v == null)
                {
                    return true;
                }
                return !v.Equals("false");
            });
            return this;
        }

        /// <summary>
        /// Looks for the given flag. If the flag exists and the value
        /// is not null it will try to parse the value as the supplied enum type.
        /// If successfull <c>value</c> will be set to the parsed enum, and in all
        /// other cases it will be set to <c>defaultValue</c>.
        /// 
        /// Parsing of enums is case-insensitive, meaning that bLuE will be parsed
        /// to Blue without problems.
        /// </summary>
        /// <param name="flag">Flag to look for</param>
        /// <param name="defaultValue">Default value to out</param>
        /// <param name="value">Variable that will be set</param>
        /// <returns></returns>
        public Flagrant Enum<TEnum>(string flag, TEnum defaultValue, out TEnum value)
        where TEnum : struct, IConvertible
        {
            value = Run(flag, defaultValue, v =>
            {
                if (v == null)
                {
                    return defaultValue;
                }
                return System.Enum.TryParse<TEnum>(v, true, out var ev) ? ev : defaultValue;
            });
            return this;
        }

        /// <summary>
        /// If the flag is found, <c>func</c> will be invoked with the value of the flag.
        /// <c>value</c> will be set to the returned value from <c>func</c>. If no flag
        /// was found it will be set to <c>defaultValue</c>
        /// </summary>
        /// <param name="flag">The flag to look for</param>
        /// <param name="defaultValue">Value to default to</param>
        /// <param name="func">The function to run when flag has been found</param>
        /// <param name="value">The variable to set</param>
        /// <typeparam name="T">The type of the outputted value</typeparam>
        /// <returns></returns>
        public Flagrant Custom<T>(string flag, T defaultValue, Func<string, T> func, out T value)
        {
            value = Run(flag, defaultValue, func);
            return this;
        }

        /// <summary>
        /// Returns a new instance of flagrant with the given arguments array
        /// correctly parsed.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Flagrant Parse(IReadOnlyList<string> args)
        {
            var arguments = new Dictionary<string, string>();
            var flagRegex = new Regex("^-{1,2}([^-=]+)=?(.*)?$");
            for (var i = 0; i < args.Count; i++)
            {
                var argument = args[i];
                if (!flagRegex.IsMatch(argument)) continue;
                var match = flagRegex.Match(argument);
                var key = match.Groups[1].Value;
                string value = null;
                if (!match.Groups[2].Value.Equals(""))
                {
                    value = match.Groups[2].Value;
                }
                else if (i != args.Count - 1)
                {
                    if (!flagRegex.IsMatch(args[i + 1]))
                    {
                        value = args[i + 1];
                    }
                }
                arguments[key] = value;
            }
            return new Flagrant(arguments);
        }
    }
}