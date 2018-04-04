using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flagrant
{
    public class Flagrant
    {
        public static void Run<T>(IEnumerable<string> args, T config)
        {
            new Flagrant(args).Bind(config);
        }
        
        private readonly IDictionary<string, string> _dict;
        private readonly IDictionary<string, Func<string, object>>_handlers;

        public Flagrant()
        {
            _dict = new Dictionary<string, string>(); 
            _handlers = new Dictionary<string, Func<string, object>>();
        }

        public Flagrant Bind<T>(T config)
        {
            var properties = config.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var attrs = prop.GetCustomAttributes();
                foreach (var attr in attrs)
                {
                    if (!(attr is FlagAttribute flag)) continue;
                    if (_dict.TryGetValue(flag.Name, out var value) || (flag.ShortName != null && _dict.TryGetValue(flag.ShortName, out value)))
                    {
                        // special bool case. If value is null, then set prop to true
                        var type = prop.PropertyType;
                        if (type == typeof(bool) && value == null)
                        {
                            prop.SetValue(config, true, null);
                            continue;
                        }
                        // do nothing if value is null
                        if (value == null) continue;
                        // check if custom func is set, and let it handle conversion
                        if (flag.Custom != null && _handlers.TryGetValue(flag.Custom, out var handler))
                        {
                            prop.SetValue(config, handler(value), null);
                            continue;
                        }
                        // special enum case, we use enum tryparse
                        if (type.IsEnum)
                        {
                            try
                            {
                                var enumValue = Enum.Parse(type, value);
                                prop.SetValue(config, enumValue, null);
                            }
                            catch (ArgumentException)
                            {    
                            }

                            continue;
                        }
                        prop.SetValue(config, Convert.ChangeType(value, prop.PropertyType), null);
                    }
                }
            }
            return this;
        }
        
        public Flagrant(IEnumerable<string> args) : this()
        {
            Parse(args.ToList());
        }

        public Flagrant RegisterCustomHandler(string name, Func<string, object> handler)
        {
            _handlers[name] = handler;
            return this;
        }

        private void ParseFlag(IReadOnlyList<string> args, ref int i)
        {
            var flag = args[i].TrimStart('-');
            string value = null;

            // boolean flag special case!
            // if flag contains a = we the substring after the =.
            // if it is NOT empty the following cases evaluates to false: "0", "n", "no".
            // All other cases (including empty) evaluates to true
            var idxEq = flag.IndexOf('=');
            if (idxEq > 0)
            {
                // contains =
                var bval = flag.Substring(idxEq + 1);
                flag = flag.Substring(0, idxEq);
                switch (bval)
                {
                    case "0": case "n": case "no":
                        value = "false";
                        break;
                    default:
                        value = "true";
                        break;
                }
            }
            else if (i < args.Count - 1)
            {
                var value_ = args[i + 1];
                if (!value_.StartsWith("-"))
                {
                    value = value_;
                    i = i + 1;
                }
            }

            _dict[flag] = value;
        }
        
        public Flagrant Parse(IReadOnlyList<string> args)
        {
            for (var i = 0; i < args.Count; i++)
            {
                var current = args[i];
                if (current.StartsWith("-"))
                {
                    ParseFlag(args, ref i);
                }
            }

            return this;
        }
    }
}