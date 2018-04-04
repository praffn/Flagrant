namespace Flagrant.App
{
    public class Config
    {
        [Flag("name", ShortName = "n")]
        public string Name { get; set; }
        
        [Flag("age", ShortName = "a")]
        public int Age { get; set; }
        
    }
}