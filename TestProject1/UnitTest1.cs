using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentAssertions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestProject1
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(1, "I")]
        [InlineData(2, "II")]
        [InlineData(3, "III")]
        [InlineData(5, "V")]
        [InlineData(8, "VIII")]
        [InlineData(10, "X")]
        [InlineData(11, "XI")]
        [InlineData(37, "XXXVII")]
        [InlineData(50, "L")]
        [InlineData(70, "LXX")]
        [InlineData(100, "C")]
        [InlineData(101, "CI")]
        [InlineData(3000, "MMM")]
        public void Converts_1_To_3000_To_Roman(int integer, string romanNumber)
        {
            GetRomanNumber(integer).Should().Be(romanNumber);
        }

        [Theory]
        [InlineData(4, "IV")]
        [InlineData(9, "IX")]
        [InlineData(45, "XLV")]
        [InlineData(99, "XCIX")]
        [InlineData(2149, "MMCXLIX")]
        [InlineData(2999, "MMCMXCIX")]
        public void Roman_Convert_Satisfies_Substraction_Rule(int integer, string romanNumber)
        {
            GetRomanNumber(integer).Should().Be(romanNumber);
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
        public void Converts_1_To_3000_Roman_To_Integer(int integer, string romanNumber)
        {
            ConvertRomanNumber(romanNumber).Should().Be(integer);
        }

        [Theory]
        [InlineData(4, "IV")]
        [InlineData(9, "IX")]
        [InlineData(45, "XLV")]
        [InlineData(99, "XCIX")]
        [InlineData(2149, "MMCXLIX")]
        [InlineData(2999, "MMCMXCIX")]
        public void Integer_Convert_Satisfies_Substraction_Rule(int integer, string romanNumber)
        {
            ConvertRomanNumber(romanNumber).Should().Be(integer);
        }

        [Fact]
        public void Below_Zero_Not_Supported()
        {
            Action act = () => GetRomanNumber(0);
            act.Should().Throw<ArgumentOutOfRangeException>("0 is not supported number", 0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Empty_Roman_Number_Not_Supported(string romanNumber)
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
            act.Should().Throw<ArgumentOutOfRangeException>(nameof(romanNumber));
        }

        [Theory]
        [InlineData("MMMM")]
        [InlineData("MMMCCCC")]
        [InlineData("IIIIII")]
        public void More_Than_Three_Roman_Numeral_Invalid(string romanNumber)
        {
            Action act = () => ConvertRomanNumber(romanNumber);
            act.Should().Throw<InvalidOperationException>($"Cannot convert invalid roman number {romanNumber}. More than three of the same roman numerals in a row.");
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
            ValidateRomanNumber(romanNumber);

            var resultInteger = 0;
            for (int romanNumberIndex = 0; romanNumberIndex < romanNumber.Length; romanNumberIndex++)
            {
                char romanNumeral = romanNumber[romanNumberIndex];
                var integer = RomanToArabicNumerals[romanNumeral];

                if (TryGetNumberToSubstract(romanNumber, romanNumberIndex, integer, out var numberToSubstract))
                    resultInteger -= numberToSubstract;
                else
                    resultInteger += integer;

            }
            return resultInteger;
        }

        private static void ValidateRomanNumber(string romanNumber)
        {
            if (string.IsNullOrWhiteSpace(romanNumber))
                throw new ArgumentNullException(nameof(romanNumber));

            var longestSequence = new string(romanNumber.Select((romanNumeral, index) => romanNumber[index..].TakeWhile(e => e == romanNumeral)).OrderByDescending(romanNumber => romanNumber.Count()).First().ToArray());
            if (longestSequence.Length >= 4)
                throw new InvalidOperationException($"Cannot convert invalid roman number {romanNumber}. More than three of the same roman numerals in a row.");

            var romanNumeralsExist = romanNumber.Any(romanNumeral => !RomanToArabicNumerals.ContainsKey(romanNumeral));
            if (romanNumeralsExist)
                throw new ArgumentOutOfRangeException(nameof(romanNumber));
        }

        private static bool TryGetNumberToSubstract(string romanNumber, int romanNumberIndex, int integer, out int numberToSubstract)
        {
            numberToSubstract = 0;
            const int maxRomanNumbersInARow = 3;

            for (int i = 0; i < maxRomanNumbersInARow; i++)
            {
                var nextIndex = romanNumberIndex + i + 1;

                if (nextIndex >= romanNumber.Length)
                    break;

                char nextRomanNumeral = romanNumber[nextIndex];
                var nextInteger = RomanToArabicNumerals[nextRomanNumeral];

                if (integer >= nextInteger)
                    break;

                numberToSubstract += integer;
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