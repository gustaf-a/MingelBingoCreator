using MingelBingoCreator.CardValueCreator.SelectorBehaviors;
using Xunit;

namespace MingelBingoCreatorUnitTests.CardValueCreatorTests.SelectorBehaviorTests
{
    public static class RepeatingValuesSelectorBehaviorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public static void RepeatingValuesSelectorBehavior_returns_correct_quantity(int quantity)
        {
            //Arrange
            var selector = new RepeatingValuesSelectorBehavior();

            var testValues = new List<string>
            {
                $"Value 1",
                $"Value 2",
            };

            //Act
            var result = selector.GetValues(testValues, quantity);

            //Assert
            Assert.Equal(quantity, result.Count);
        }

        [Fact]
        public static void RepeatingValuesSelectorBehavior_returns_no_duplicates_if_enough_values_exist()
        {
            //Arrange
            var selector = new RepeatingValuesSelectorBehavior();

            var totalValues = 800;

            var testValues = new List<string>();

            for (int i = 0; i < totalValues; i++)
                testValues.Add($"Value {i}");

            //Act
            var result = selector.GetValues(testValues, totalValues);

            //Assert
            Assert.Equal(totalValues, result.Count);

            Assert.Equal(totalValues, result.Distinct().Count());
        }
    }
}
