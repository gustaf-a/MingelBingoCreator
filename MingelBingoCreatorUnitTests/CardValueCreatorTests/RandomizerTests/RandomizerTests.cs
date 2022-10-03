using MingelBingoCreator.CardValueCreator.Randomizer;
using Xunit;

namespace MingelBingoCreatorUnitTests.CardValueCreatorTests.RandomizerTests
{
    public static class RandomizerTests
    {
        [Fact]
        public static void Randomizer_returns_throws_if_no_values_to_select_from()
        {
            //Arrange
            var randomizer = new Randomizer();

            var values = new List<string>
            {
                "test_1",
                "test_2",
                "test_3",
                "test_4",
                "test_5"
            };

            var valuesToAvoid = new List<int>
            {
                0, 1, 2, 3, 4, 5
            };

            //Act Assert
            Assert.Throws<Exception>(() => randomizer.GetNextRandomIndex(values, valuesToAvoid));
        }

        [Theory]
        [InlineData(100, 0)]
        [InlineData(100, 7)]
        [InlineData(100, 55)]
        [InlineData(100, 99)]
        [InlineData(1000, 543)]
        public static void Randomizer_returns_value_not_to_avoid(int numberOfValues, int valueToReturn)
        {
            //Arrange
            var randomizer = new Randomizer();

            var values = new List<string>();
            var valuesToAvoid = new List<int>();

            for (int i = 0; i < numberOfValues; i++)
            {
                values.Add($"test_{i}");
                valuesToAvoid.Add(i);
            }
            
            valuesToAvoid.Remove(valueToReturn);

            //Act
            var result = randomizer.GetNextRandomIndex(values, valuesToAvoid);

            //Assert
            Assert.Equal(valueToReturn, result);
        }

        [Theory]
        [InlineData(100000, 0)]
        [InlineData(100000, 5)]
        [InlineData(100000, 43)]
        [InlineData(100000, 99)]
        [InlineData(100000, 543)]
        public static void Randomizer_returns_values_not_to_avoid_when_valuesToAvoid_exceed_values(int numberOfValues, int valueToReturn)
        {
            //Arrange
            var randomizer = new Randomizer();

            var values = new List<string>();
            var valuesToAvoid = new List<int>();

            for (int i = 0; i < numberOfValues; i++)
            {
                values.Add($"test_{i}");
                valuesToAvoid.Add(i);
            }

            valuesToAvoid.Remove(valueToReturn);
            valuesToAvoid.Add(numberOfValues);

            //Act
            var result = randomizer.GetNextRandomIndex(values, valuesToAvoid);

            //Assert
            Assert.Equal(valueToReturn, result);
        }
    }
}
