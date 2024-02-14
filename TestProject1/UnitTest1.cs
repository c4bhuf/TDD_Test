using FluentAssertions;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void RangeCheck_Roemisch_1_To_9()
        {
            GetRoemisch(0).Should().Be("error");
        }

        string GetRoemisch(int i)
        {
            if (i == 1)
                return "I";

            return "error";
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