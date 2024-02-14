using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentAssertions;

namespace TestProject1
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(1, "I")]
        [InlineData(2, "II")]
        [InlineData(3, "III")]
        [InlineData(4, "IV")]
        [InlineData(5, "V")]
        [InlineData(6, "VI")]
        [InlineData(7, "VII")]
        [InlineData(8, "VIII")]
        [InlineData(9, "IX")]
        [InlineData(10, "X")]
        public void Check_Roemisch_1_to_10(int number, string roemisch)
        {
            GetRoemisch(number).Should().Be(roemisch);
        }
        
        [Theory]
        [InlineData(11, "XI")]
        [InlineData(37, "XXXVII")]
        [InlineData(45, "VL")]
        [InlineData(50, "L")]
        [InlineData(70, "LXX")]
        [InlineData(99, "IC")]
        [InlineData(100, "C")]
        [InlineData(150, "CL")]
        public void Check_Roemisch_11_to_100(int number, string roemisch)
        {
            GetRoemisch(number).Should().Be(roemisch);
        }

        [Fact]
        public void BelowZeroNotSupported()
        {
            Action act = () => GetRoemisch(0);
            act.Should().Throw<ArgumentOutOfRangeException>("0 is not supported number",0);
        }


        string GetRoemisch(int i)
        {
            int teiler;
            var stringBuilder = new StringBuilder();
            if((teiler = (int)Math.Floor((double)i / 100)) >= 1)
            {
                stringBuilder.Append(new string('C', teiler));
                i -= teiler * 100;
            }
            if ((teiler = (int)Math.Floor((double)i / 50)) >= 1)
            {
                stringBuilder.Append(new string('L', teiler));
                i -= teiler * 50;
            }

            return stringBuilder.ToString();
            return i switch
            {
                <= 0 => throw new ArgumentOutOfRangeException(nameof(i), $"{i} is not supported number"),
                <= 3 => new string('I', i),
                4    => "IV",
                <= 8 => "V" + new string('I', i % 5),
                9    => "IX",
                10   => "X",
                _    => throw new NotImplementedException($"{i} cannot be parsed"),
            };
        }
        /*
         * I
         * II
         * III
         * V
         * X
         * L
         * C
         * D
         * M
         */
    }
}