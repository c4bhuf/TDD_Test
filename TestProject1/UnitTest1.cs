using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentAssertions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public void Integer_To_Roman_1_to_10(int number, string roemisch)
        {
            GetRomanNumber(number).Should().Be(roemisch);
        }
        
        [Theory]
        [InlineData(11, "XI")]
        [InlineData(37, "XXXVII")]
        [InlineData(45, "XLV")]
        [InlineData(50, "L")]
        [InlineData(70, "LXX")]
        [InlineData(99, "XCIX")]
        [InlineData(100, "C")]
        public void Integer_To_Roman_11_to_100(int number, string roemisch)
        {
            GetRomanNumber(number).Should().Be(roemisch);
        }

        [Theory]
        [InlineData(101, "CI")]
        [InlineData(1459, "MCDLIX")]
        [InlineData(2149, "MMCXLIX")]
        [InlineData(2999, "MMCMXCIX")]
        public void Integer_To_Roman_101_to_3000(int number, string roemisch)
        {
            GetRomanNumber(number).Should().Be(roemisch);
        }


        [Theory]
        [InlineData(8, "VIII")]
        [InlineData(9, "IX")]
        [InlineData(45, "XLV")]
        [InlineData(99, "XCIX")]
        [InlineData(101, "CI")]
        [InlineData(1459, "MCDLIX")]
        [InlineData(2149, "MMCXLIX")]
        [InlineData(2999, "MMCMXCIX")]
        public void Roman_To_Integer_1_to_3000(int integer, string romanNumber)
        {
            ConvertRomanNumber(romanNumber).Should().Be(integer);
        }

        [Fact]
        public void Below_Zero_Not_Supported()
        {
            Action act = () => GetRomanNumber(0);
            act.Should().Throw<ArgumentOutOfRangeException>("0 is not supported number",0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Empty_Roman_Numerals_Not_Supported(string romanNumber)
        {
            Action act = () => ConvertRomanNumber(romanNumber);
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("U")]
        [InlineData("MMCMXCIXS")]
        public void Undefined_Roman_Numerals_Not_Supported(string romanNumber)
        {
            Action act = () => ConvertRomanNumber(romanNumber);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        private static readonly SortedDictionary<char, int> RomanToArabicNumerals = new()
        {
            { 'M',  1000 },
            { 'D', 500 },
            { 'C', 100 },
            { 'L' , 50 },
            { 'X', 10 },
            { 'V', 5 },
            { 'I', 1 }
        };

        public static int ConvertRomanNumber(string romanNumber)
        {
            if (string.IsNullOrWhiteSpace(romanNumber))
                throw new ArgumentNullException(nameof(romanNumber));

            var resultInteger = 0;
            for (int romanNumberIndex = 0; romanNumberIndex < romanNumber.Length; romanNumberIndex++)
            {
                char romanNumeral = romanNumber[romanNumberIndex];
                var arabicNumeral = RomanToArabicNumerals[romanNumeral];
                if (TryGetNumberToSubstract(romanNumber, romanNumberIndex, romanNumeral, out var numberToSubstract))
                    resultInteger -= numberToSubstract;
                else
                    resultInteger += arabicNumeral;

            }
            return resultInteger;
        }

        private static bool TryGetNumberToSubstract(string romanNumber, int romanNumberIndex, char romanNumeral, out int numberToSubstract)
        {
            numberToSubstract = 0;

            for (int i = 1; i < 4; i++)
            {
                if (romanNumberIndex + i < romanNumber.Length)
                {
                    char nextRomanNumeral = romanNumber[romanNumberIndex + i];
                    if (RomanToArabicNumerals[romanNumeral] >= RomanToArabicNumerals[nextRomanNumeral])
                        break;

                    numberToSubstract += RomanToArabicNumerals[romanNumeral];
                }
            }

            return numberToSubstract != 0;
        }

        private static readonly SortedDictionary<int, char> ArabicToRomanNumerals = new()
        {
            { 1000, 'M' },
            { 500, 'D' },
            { 100, 'C' },
            { 50, 'L' },
            { 10, 'X' },
            { 5, 'V' },
            { 1, 'I' }
        };

        public static string GetRomanNumber(int arabicNumber)
        {
            if (arabicNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(arabicNumber), $"{arabicNumber} is not supported number");

            int remainder = arabicNumber;
            var stringBuilder = new StringBuilder();
            foreach (var numberToRoman in ArabicToRomanNumerals.OrderByDescending(kvp => kvp.Key))
            {
                remainder = AppendRomanNumber(remainder, stringBuilder, numberToRoman.Value, numberToRoman.Key);
                remainder = AppendBySubtractionRule(remainder, stringBuilder, numberToRoman.Key);
            }

            return stringBuilder.ToString();
        }

        private static int AppendBySubtractionRule(int remainder, StringBuilder stringBuilder, int number)
        {
            if (number == 1)
                return 0;

            var numberOfZeros = number.ToString().Length - 1;
            var tenPowOfNumberOfZeros = (int)Math.Pow(10, numberOfZeros);
            var numberToSubstract = tenPowOfNumberOfZeros - number == 0 ? (int)Math.Pow(10, numberOfZeros - 1) : tenPowOfNumberOfZeros;

            if (remainder >= number - numberToSubstract)
            {
                remainder -= number - numberToSubstract;

                stringBuilder.Append(ArabicToRomanNumerals[numberToSubstract]);
                stringBuilder.Append(ArabicToRomanNumerals[number]);
            }

            return remainder;
        }

        private static int AppendRomanNumber(int remainder, StringBuilder stringBuilder, char romanNumber, int arabicNumber)
        {
            var fitsInto = remainder.FitsInto(arabicNumber);
            if (fitsInto >= 1)
            {
                stringBuilder.Append(new string(romanNumber, fitsInto));
                remainder -= fitsInto * arabicNumber;
            }

            return remainder;
        }
    }

    public static class IntExtensions
    {
        public static int FitsInto(this int x, int y)
        {
            return (int)Math.Floor((double)x / y);
        }
    }
}