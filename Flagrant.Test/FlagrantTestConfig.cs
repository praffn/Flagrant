using System.Linq;

namespace Flagrant.Test
{
    public enum Color
    {
        Red,
        Green,
        Blue,
    }
    
    public class FlagrantTestConfig
    {
        [Flag("name")] public string Name { get; set; }
        
        [Flag("age")] public int Age { get; set; }

        [Flag("verbose")] public bool Verbose { get; set; }
        
        [Flag("height")] public float Height { get; set; }

        [Flag("production")] public bool Production { get; set; } = true;
        
        [Flag("color")] public Color Color { get; set; }
        
        [Flag("volume", Custom = "clamp")] public int Volume { get; set; }
    }
}