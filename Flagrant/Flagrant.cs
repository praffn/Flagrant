using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Flagrant
{
    public class Flagrant
    {
        private readonly Dictionary<string, string> _arguments;

        public Flagrant(Dictionary<string, string> arguments)
        {
            _arguments = arguments;
        }

        private T Run<T>(string flag, T defaultValue, Func<string, T> func)
        {
            return _arguments.TryGetValue(flag, out var value) ? func.Invoke(value) : defaultValue;
        }

        public Flagrant String(string flag, string defaultValue, out string value)
        {
            value = Run(flag, defaultValue, v => v ?? defaultValue);
            return this;
        }

        public Flagrant Int(string flag, int defaultValue, out int value)
        {
            value = Run(flag, defaultValue, v => int.TryParse(v, out var iv) ? iv : defaultValue);
            return this;
        }
        
        public Flagrant Long(string flag, long defaultValue, out long value)
        {
            value = Run(flag, defaultValue, v => long.TryParse(v, out var lv) ? lv : defaultValue);
            return this;
        }

        public Flagrant Float(string flag, float defaultValue, out float value)
        {
            value = Run(flag, defaultValue, v => float.TryParse(v, out var fv) ? fv : defaultValue);
            return this;
        }
        
        public Flagrant Double(string flag, double defaultValue, out double value)
        {
            value = Run(flag, defaultValue, v => double.TryParse(v, out var dv) ? dv : defaultValue);
            return this;
        }

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

        public Flagrant Custom<T>(string flag, T defaultValue, Func<string, T> func, out T value)
        {
            value = Run(flag, defaultValue, func);
            return this;
        }

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