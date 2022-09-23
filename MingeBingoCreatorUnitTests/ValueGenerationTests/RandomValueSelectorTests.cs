using Xunit;
using MingelBingoCreator.Data;
using MingelBingoCreator.ValuesGeneration;

namespace MingeBingoCreatorUnitTests.ValueGenerationTests
{
    public static class RandomValueSelectorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public static void RandomValueSelector_returns_correct_quantity(int quantity)
        {
            //Arrange
            var testValues = new List<Category>();
            testValues.Add(new Category("Heading 1", new List<string>{ "h1v1", "h1v2", "h1v3" }));
            testValues.Add(new Category("Heading 2", new List<string>{ "h2v1", "h2v2" }));
            testValues.Add(new Category("Heading 3", new List<string>{ "h3v1", "h3v2", "h3v3", "h3v4" }));

            var randomValueSelector = new RandomValueSelector(quantity, testValues);

            //Act
            var result = randomValueSelector.GetValues();

            //Assert
            Assert.Equal(quantity, result.Count);
        }

        [Fact]
        public static void RandomValueSelector_throws_if_no_values_provided()
        {
            //Arrange
            var testValues = new List<Category>();
            testValues.Add(new Category("Heading 1", new List<string>()));
            testValues.Add(new Category("Heading 2", new List<string>()));

            //Act and Assert
            Assert.Throws<Exception>(() => new RandomValueSelector(10, testValues));
        }

        [Fact]
        public static void RandomValueSelector_returns_no_duplicates_if_enough_values_exist()
        {
            //Arrange
            var totalValues = 800;

            var testValues = new List<Category>();

            for (int i = 0; i < 8; i++)
            {
                var values = new List<string>();

                for (int j = 0; j < 100; j++)
                    values.Add($"Category {i} Value {j}");

                testValues.Add(new Category($"Category {i}", values));
            }

            var randomValueSelector = new RandomValueSelector(totalValues, testValues);

            //Act
            var result = randomValueSelector.GetValues();

            //Assert
            Assert.Equal(totalValues, result.Count);

            Assert.Equal(totalValues, result.Distinct().Count());
        }
    }
}
