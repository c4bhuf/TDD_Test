using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentAssertions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestProject1
{
    public class UnitTest1
    {
        private static SortedDictionary<int, char> ArabicToRomanNumerals = new() 
        {
            { 100, 'C' },
            { 50, 'L' },
            { 10, 'X' },
            { 5, 'V' },
            { 1, 'I' }
        };

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
        [InlineData(45, "XLV")]
        [InlineData(50, "L")]
        [InlineData(70, "LXX")]
        [InlineData(99, "XCIX")]
        [InlineData(100, "C")]
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

        public string GetRoemisch(int i)
        {
            if (i <= 0)
                throw new ArgumentOutOfRangeException(nameof(i), $"{i} is not supported number");

            int teiler;
            int rest = i;
            var stringBuilder = new StringBuilder();
            foreach (var numberToRoman in ArabicToRomanNumerals.OrderByDescending(kvp => kvp.Key))
            {
                teiler = AppendRomanNumber(ref rest, stringBuilder, numberToRoman.Value, numberToRoman.Key);

                rest = SubtractionRule(rest, stringBuilder, numberToRoman.Key);
            }

            return stringBuilder.ToString();
        }

        private static int SubtractionRule(int rest, StringBuilder stringBuilder, int number)
        {
            if (number == 1)
                return 0;

            var numberOfZeros = number.ToString().Length - 1;
            var tenPowOfNumberOfZeros = (int)Math.Pow(10, numberOfZeros);
            var numberToSubstract = tenPowOfNumberOfZeros - number == 0 ? (int)Math.Pow(10, numberOfZeros - 1) : tenPowOfNumberOfZeros;

            if (rest >= number - numberToSubstract)
            {
                rest -= number - numberToSubstract;

                stringBuilder.Append(ArabicToRomanNumerals[numberToSubstract]);
                stringBuilder.Append(ArabicToRomanNumerals[number]);
            }

            return rest;
        }

        private static int AppendRomanNumber(ref int rest, StringBuilder stringBuilder, char romanNumber, int number)
        {
            int teiler;
            if ((teiler = CalculateTeiler(rest, number)) >= 1)
            {
                stringBuilder.Append(new string(romanNumber, teiler));
                rest -= teiler * number;
            }

            return teiler;
        }

        public static int CalculateTeiler(int rest, int number)
        {
            return (int)Math.Floor((double)rest / number);
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