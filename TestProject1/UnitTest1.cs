using FluentAssertions;

namespace TestProject1
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(1, "I")]
        [InlineData(2, "II")]
        //[InlineData(4, "IV")]
        [InlineData(5, "V")]
        [InlineData(7, "VII")]
        //[InlineData(9, "IX")]
        //[InlineData(10, "X")]
        public void Check_Roemisch_1_to_10(int number, string roemisch)
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
            if(i <= 0)
                throw new ArgumentOutOfRangeException(nameof(i),$"{i} is not supported number");

            if (i <= 3)
                return new string('I', i);

            if (i >= 5 && i <= 8)
            {
                var res = i % 5;

                var strRes = $"V{new string('I', res)}";
                return strRes;
            }
                

            

            throw new NotImplementedException($"{i} cannot be parsed");
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