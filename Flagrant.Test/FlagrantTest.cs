using System;
using System.Linq;
using Xunit;

namespace Flagrant.Test
{
    public class FlagrantTest
    {
        private readonly string[] _defaultFlags = new[]
        {
            "--name", "John",
            "--age", "25",
            "--height", "1.89",
            "--verbose",
            "--production=0",
            "--color", "Green",
            "--volume", "130"
        };
        
        [Fact]
        public void Can_parse_string()
        {
            var config = new FlagrantTestConfig();
            Flagrant.Run(_defaultFlags, config);
            Assert.Equal("John", config.Name);
        }

        [Fact]
        public void Can_parse_int()
        {
            var config = new FlagrantTestConfig();
            Flagrant.Run(_defaultFlags, config);
            Assert.Equal(25, config.Age);
        }

        [Fact]
        public void Can_parse_float()
        {
            var config = new FlagrantTestConfig();
            Flagrant.Run(_defaultFlags, config);
            const float expected = 1.89f;
            var x = Math.Abs(expected - config.Height) < expected / Math.Pow(10, 10);
            Assert.True(x);
        }
        

        [Fact]
        public void Can_parse_bool_with_no_value()
        {
            var config = new FlagrantTestConfig();
            Flagrant.Run(_defaultFlags, config);
            Assert.True(config.Verbose);
        }

        [Theory]
        [InlineData("n")]
        [InlineData("no")]
        [InlineData("0")]
        public void Can_parse_bool_with_values(string value)
        {
            var config = new FlagrantTestConfig();
            Flagrant.Run(new[]{$"--production={value}"}, config);
            Assert.False(config.Production);
        }

        [Fact]
        public void Can_parse_enum()
        {
            var config = new FlagrantTestConfig();
            Flagrant.Run(_defaultFlags, config);
            Assert.Equal(Color.Green, config.Color);
        }

        [Fact]
        public void Can_parse_custom()
        {
            var config = new FlagrantTestConfig();
            new Flagrant(_defaultFlags)
                .RegisterCustomHandler("clamp", str =>
                {
                    if (int.TryParse(str, out var vol))
                    {
                        return vol < 0 ? 0 : vol > 100 ? 100 : vol;
                    }

                    return 0;
                })
                .Bind(config);
            Assert.Equal(100, config.Volume);
        }
    }
}