using MingelBingoCreator.Data;
using MingelBingoCreator.ValuesGeneration;
using Xunit;

namespace MingeBingoCreatorUnitTests.ValueGenerationTests
{
    public static class TaggedCategoriesValueSelectorTests
    {
        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public static void TaggedCategoriesValueSelector_returns_correct_quantity_with_OnEachBoard(int quantity)
        {
            //Arrange
            var testValues = new List<Category>();
            testValues.Add(new Category("Heading 1", new List<string> { "h1v1", "h1v2", "h1v3" }));
            testValues.Add(new Category("Heading 2 #OnEachBoard_1", new List<string> { "UniqueOneEachBoard1" }));
            testValues.Add(new Category("Heading 3", new List<string> { "h3v1", "h3v2", "h3v3", "h3v4" }));
            testValues.Add(new Category("Heading 4 #OnEachBoard_2", new List<string> { "UniqueOneEachBoard2", "UniqueOneEachBoard3" }));

            var taggedValueSelector = new TaggedCategoriesValueSelector(quantity, testValues);

            //Act
            var result = taggedValueSelector.GetValues();

            //Assert
            Assert.Equal(quantity, result.Count);

            Assert.Equal(1, result.Count(r => r == "UniqueOneEachBoard1"));
            Assert.Equal(1, result.Count(r => r == "UniqueOneEachBoard2"));
            Assert.Equal(1, result.Count(r => r == "UniqueOneEachBoard3"));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public static void TaggedCategoriesValueSelector_returns_correct_quantity_if_no_tagged_categories(int quantity)
        {
            //Arrange
            var testValues = new List<Category>();
            testValues.Add(new Category("Heading 1", new List<string> { "h1v1", "h1v2", "h1v3" }));
            testValues.Add(new Category("Heading 2", new List<string> { "h2v1", "h2v2" }));
            testValues.Add(new Category("Heading 3", new List<string> { "h3v1", "h3v2", "h3v3", "h3v4" }));

            var taggedValueSelector = new TaggedCategoriesValueSelector(quantity, testValues);

            //Act
            var result = taggedValueSelector.GetValues();

            //Assert
            Assert.Equal(quantity, result.Count);
        }

        [Fact]
        public static void TaggedCategoriesValueSelector_throws_if_no_values_provided_if_no_tagged_categories()
        {
            //Arrange
            var testValues = new List<Category>();
            testValues.Add(new Category("Heading 1", new List<string>()));
            testValues.Add(new Category("Heading 2", new List<string>()));

            //Act and Assert
            Assert.Throws<Exception>(() => new TaggedCategoriesValueSelector(10, testValues));
        }

        [Fact]
        public static void TaggedCategoriesValueSelector_returns_no_duplicates_if_enough_values_exist_if_no_tagged_categories()
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

            var taggedValueSelector = new TaggedCategoriesValueSelector(totalValues, testValues);

            //Act
            var result = taggedValueSelector.GetValues();

            //Assert
            Assert.Equal(totalValues, result.Count);

            Assert.Equal(totalValues, result.Distinct().Count());
        }
    }
}
