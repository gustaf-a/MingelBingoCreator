using MingelBingoCreator.CardValueCreator.SelectorBehaviors;
using Xunit;

namespace MingelBingoCreatorUnitTests.CardValueCreatorTests.SelectorBehaviorTests
{
    public static class UniqueValuesSelectorBehaviorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(9)]
        public static void UniqueValuesSelectorBehavior_returns_correct_quantity_when_enough_values(int quantity)
        {
            //Arrange
            var selector = new UniqueValuesSelectorBehavior();

            var testValues = new List<string>
            {
                $"Value 1",
                $"Value 2",
                $"Value 3",
                $"Value 4",
                $"Value 5",
                $"Value 6",
                $"Value 7",
                $"Value 8",
                $"Value 9"
            };

            //Act
            var result = selector.GetValues(testValues, quantity);

            //Assert
            Assert.Equal(quantity, result.Count);
        }

        [Fact]
        public static void UniqueValuesSelectorBehavior_returns_only_unique_values_when_not_enough_values()
        {
            //Arrange
            var selector = new UniqueValuesSelectorBehavior();

            var testValues = new List<string>
            {
                $"Value 1",
                $"Value 2",
                $"Value 3",
                $"Value 4",
                $"Value 5",
                $"Value 6",
                $"Value 7",
                $"Value 8",
                $"Value 9"
            };

            //Act
            var result = selector.GetValues(testValues, 15);

            //Assert
            Assert.Equal(9, result.Count);
        }

        [Fact]
        public static void UniqueValuesSelectorBehavior_returns_zero_values_when_all_values_been_taken()
        {
            //Arrange
            var selector = new UniqueValuesSelectorBehavior();

            var testValues = new List<string>
            {
                $"Value 1",
                $"Value 2",
                $"Value 3",
                $"Value 4",
                $"Value 5",
                $"Value 6",
                $"Value 7",
                $"Value 8",
                $"Value 9"
            };

            //Act
            var result = selector.GetValues(testValues, 15);
            var result2 = selector.GetValues(testValues, 15);

            //Assert
            Assert.Equal(9, result.Count);

            Assert.Empty(result2);
        }
    }
}
