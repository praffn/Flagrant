# 🏁 Flagrant
[![Build Status](https://travis-ci.org/praffn/Flagrant.svg?branch=master)](https://travis-ci.org/praffn/Flagrant)

> A little flag parsing tool for .Net

## How to use

1) Create a new instance of `Flagrant` by giving the static method `Parse` an array of strings.

2) Tell flagrant which flags you want to extract by providing three parameters:
  1. Flagname - Name of the flag in the given dictionary.
  2. DefaultValue - Default value that will be used, if the flat wasn't found in the dictionary.
  3. Value - References the variable where the retrieved value will be stored.

```csharp
public static void Main(string[] args)
{
  Flagrant.Parse(args)
    .String("name", "Phillip", out var name)
    .Int("age", 24, out var age);

  Console.WriteLine($"Hey {name}, only {10%age} years to the big day!");
}
```

## Types of flags
You can use the following methods to extract the flags with different types:

* `.String(string flagname, string defaultValue, out string value)`
* `.Int(string flagname, int defaultValue, out int value)`
* `.Long(string flagname, long defaultValue, out long value)`
* `.Float(string flagname, float defaultValue, out float value)`
* `.Double(string flagname, double defaultValue, out double value)`
* `.Bool(string flagname, bool defaultValue, out var value)`
* `.Enum<TEnum>(string flagename, TEnum defaultValue, out TEnum value)`
* `.Custom<T>(string flagename, T defaultValue, Func<string, T> func, out T value)`

### Custom Flags
With the Custom method, it's possible to convert arguments into custom types. The additional Func parameter is responsible for converting the string representation into it's corresponding custom value.