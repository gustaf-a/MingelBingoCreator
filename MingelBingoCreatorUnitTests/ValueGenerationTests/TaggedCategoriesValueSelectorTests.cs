using MingelBingoCreator.Data;
using MingelBingoCreator.ValuesGeneration;
using Xunit;

namespace MingelBingoCreatorUnitTests.ValueGenerationTests
{
    public static class TaggedCategoriesValueSelectorTests
    {
        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(9)]
        public static void TaggedCategoriesValueSelector_UniqueValues_returns_correct_quantity(int quantity)
        {
            //Arrange
            var testValues = new List<Category>
            {
                new Category("Heading 1", new List<string> { "h1v1", "h1v2", "h1v3" }),
                new Category("Heading 2 #UniquePerBoard_1", new List<string> { "UniqueValuesPerBoard1" }),
                new Category("Heading 3", new List<string> { "h3v1", "h3v2", "h3v3", "h3v4" }),
                new Category("Heading 4 #UniquePerBoard_2", new List<string> { "UniqueValuesPerBoard2", "UniqueValuesPerBoard3" })
            };

            var taggedValueSelector = new TaggedCategoriesValueSelector(testValues);

            //Act
            var result = taggedValueSelector.GetValues(quantity);
            var result2 = taggedValueSelector.GetValues(quantity);

            
            //Assert
            Assert.Equal(quantity, result.Count);
            Assert.Equal(1, result.Count(r => r == "UniqueValuesPerBoard1"));
            Assert.Equal(1, result.Count(r => r == "UniqueValuesPerBoard2"));
            Assert.Equal(1, result.Count(r => r == "UniqueValuesPerBoard3"));

            Assert.Equal(quantity, result2.Count);
            Assert.DoesNotContain("UniqueValuesPerBoard1", result2);
            Assert.DoesNotContain("UniqueValuesPerBoard2", result2);
            Assert.DoesNotContain("UniqueValuesPerBoard3", result2);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public static void TaggedCategoriesValueSelector_returns_OnEachBoard_correct_quantity(int quantity)
        {
            //Arrange
            var testValues = new List<Category>
            {
                new Category("Heading 1", new List<string> { "h1v1", "h1v2", "h1v3" }),
                new Category("Heading 2 #OnEachBoard_1", new List<string> { "OnEachBoard1" }),
                new Category("Heading 3", new List<string> { "h3v1", "h3v2", "h3v3", "h3v4" }),
                new Category("Heading 4 #OnEachBoard_2", new List<string> { "OnEachBoard2", "OnEachBoard3" })
            };

            var taggedValueSelector = new TaggedCategoriesValueSelector(testValues);

            //Act
            var result = taggedValueSelector.GetValues(quantity);
            
            var result2 = taggedValueSelector.GetValues(quantity);

            //Assert
            Assert.Equal(quantity, result.Count);
            Assert.Equal(1, result.Count(r => r == "OnEachBoard1"));
            Assert.Equal(1, result.Count(r => r == "OnEachBoard2"));
            Assert.Equal(1, result.Count(r => r == "OnEachBoard3"));

            Assert.Equal(quantity, result2.Count);
            Assert.Equal(1, result2.Count(r => r == "OnEachBoard1"));
            Assert.Equal(1, result2.Count(r => r == "OnEachBoard2"));
            Assert.Equal(1, result2.Count(r => r == "OnEachBoard3"));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public static void TaggedCategoriesValueSelector_NoTagged_returns_correct_quantity(int quantity)
        {
            //Arrange
            var testValues = new List<Category>
            {
                new Category("Heading 1", new List<string> { "h1v1", "h1v2", "h1v3" }),
                new Category("Heading 2", new List<string> { "h2v1", "h2v2" }),
                new Category("Heading 3", new List<string> { "h3v1", "h3v2", "h3v3", "h3v4" })
            };

            var taggedValueSelector = new TaggedCategoriesValueSelector(testValues);

            //Act
            var result = taggedValueSelector.GetValues(quantity);

            //Assert
            Assert.Equal(quantity, result.Count);
        }

        [Fact]
        public static void TaggedCategoriesValueSelector_NoTagged_throws_if_no_values_provided()
        {
            //Arrange
            var testValues = new List<Category>
            {
                new Category("Heading 1", new List<string>()),
                new Category("Heading 2", new List<string>())
            };

            //Act and Assert
            Assert.Throws<Exception>(() => new TaggedCategoriesValueSelector(testValues));
        }

        [Fact]
        public static void TaggedCategoriesValueSelector_NoTagged_returns_no_duplicates_if_enough_values_exist()
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

            var taggedValueSelector = new TaggedCategoriesValueSelector(testValues);

            //Act
            var result = taggedValueSelector.GetValues(totalValues);

            //Assert
            Assert.Equal(totalValues, result.Count);

            Assert.Equal(totalValues, result.Distinct().Count());
        }
    }
}
