using MingelBingoCreator.Data;
using MingelBingoCreator.ValuesGeneration;
using Xunit;

namespace MingelBingoCreatorUnitTests.ValueGenerationTests
{
    public static class UniqueValueSelectorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(9)]
        public static void UniqueValueSelector_returns_correct_quantity_through_argument(int quantity)
        {
            //Arrange
            var testValues = new List<Category>
            {
                new Category("Heading 1", new List<string> { "h1v1", "h1v2", "h1v3" }),
                new Category("Heading 2", new List<string> { "h2v1", "h2v2" }),
                new Category("Heading 3", new List<string> { "h3v1", "h3v2", "h3v3", "h3v4" })
            };

            var uniqueValueSelector = new UniqueValueSelector(testValues, quantity);

            //Act
            var result = uniqueValueSelector.GetValues();

            //Assert
            Assert.Equal(quantity, result.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(9)]
        public static void UniqueValueSelector_returns_correct_quantity_through_constructor(int quantity)
        {
            //Arrange
            var testValues = new List<Category>
            {
                new Category("Heading 1", new List<string> { "h1v1", "h1v2", "h1v3" }),
                new Category("Heading 2", new List<string> { "h2v1", "h2v2" }),
                new Category("Heading 3", new List<string> { "h3v1", "h3v2", "h3v3", "h3v4" })
            };

            var uniqueValueSelector = new UniqueValueSelector(testValues, quantity);

            //Act
            var result = uniqueValueSelector.GetValues();

            //Assert
            Assert.Equal(quantity, result.Count);
        }

        [Fact]
        public static void UniqueValueSelector_throws_if_no_values_provided()
        {
            //Arrange
            var testValues = new List<Category>
            {
                new Category("Heading 1", new List<string>()),
                new Category("Heading 2", new List<string>())
            };

            //Act and Assert
            Assert.Throws<Exception>(() => new UniqueValueSelector(testValues));
        }

        [Fact]
        public static void UniqueValueSelector_returns_no_values_if_not_enough_values_exist()
        {
            //Arrange
            var totalValues = 10;

            var testValues = new List<Category>();

            for (int i = 0; i < 2; i++)
            {
                var values = new List<string>();

                for (int j = 0; j < 5; j++)
                    values.Add($"Category {i} Value {j}");

                testValues.Add(new Category($"Category {i}", values));
            }

            var uniqueValueSelector = new UniqueValueSelector(testValues);

            //Act
            var result = uniqueValueSelector.GetValues(totalValues+5);
            var result2 = uniqueValueSelector.GetValues(totalValues);

            //Assert
            Assert.Equal(totalValues, result.Count);
         
            Assert.Empty(result2);
        }
    }
}
