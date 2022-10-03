using MingelBingoCreator.CardValueCreator.ValuesHandlerSelector;
using MingelBingoCreator.Data;
using Xunit;

namespace MingelBingoCreatorUnitTests.CardValueCreatorTests.ValuesHandlerSelectorTests
{
    public static class TaggedCategoryIdentifierTests
    {
        [Theory]
        [InlineData("Test name 1 #Ignore", true)]
        [InlineData("Test name 2  #OnEachBoard_2", true)]
        [InlineData("Test name 3 ", false)]
        [InlineData("Test name 4 #UniquePerBoard_5", true)]
        public static void TaggedCategoryIdentifier_identifies_when_categoryHeading_is_tagged(string heading, bool isTagged)
        {
            //Arrange
            var taggedCategoryIdentifier = new TaggedCategoryIdentifier();

            //Act
            var result = taggedCategoryIdentifier.IsAnyTaggedCategory(heading);

            //Assert
            Assert.Equal(isTagged, result);
        }

        [Fact]
        public static void TaggedCategoryIdentifier_tryGetTaggedCategory_returns_OnEachBoard()
        {
            //Arrange
            var taggedCategoryIdentifier = new TaggedCategoryIdentifier();

            var dataCategory = new DataCategory("Test #OnEachBoard_12", new()
            {
                "test value 1",
                "test value 2",
                "test value 3"
            });

            var categoryTag = CategoryTags.OnEachBoard;

            //Act
            var result = taggedCategoryIdentifier.TryGetTaggedCategory(dataCategory, categoryTag, out var taggedCategory);

            //Assert
            Assert.True(result);

            Assert.Equal(dataCategory.Heading, taggedCategory.Heading);
            Assert.Equal(dataCategory.Values, taggedCategory.Values);
            Assert.Equal(categoryTag, taggedCategory.Category);

            Assert.True(taggedCategory.HasArgument);
            Assert.Equal(12, taggedCategory.Argument);
        }

        [Fact]
        public static void TaggedCategoryIdentifier_tryGetTaggedCategory_returns_UniquePerBoard()
        {
            //Arrange
            var taggedCategoryIdentifier = new TaggedCategoryIdentifier();

            var dataCategory = new DataCategory("Test #UniquePerBoard_9", new()
            {
                "test value 1",
                "test value 2",
                "test value 3"
            });

            var categoryTag = CategoryTags.UniquePerBoard;

            //Act
            var result = taggedCategoryIdentifier.TryGetTaggedCategory(dataCategory, categoryTag, out var taggedCategory);

            //Assert
            Assert.True(result);

            Assert.Equal(dataCategory.Heading, taggedCategory.Heading);
            Assert.Equal(dataCategory.Values, taggedCategory.Values);
            Assert.Equal(categoryTag, taggedCategory.Category);

            Assert.True(taggedCategory.HasArgument);
            Assert.Equal(9, taggedCategory.Argument);
        }

        [Fact]
        public static void TaggedCategoryIdentifier_tryGetTaggedCategory_returns_Ignore()
        {
            //Arrange
            var taggedCategoryIdentifier = new TaggedCategoryIdentifier();

            var dataCategory = new DataCategory("Test #Ignore", new()
            {
                "test value 1",
                "test value 2",
                "test value 3"
            });

            var categoryTag = CategoryTags.Ignore;

            //Act
            var result = taggedCategoryIdentifier.TryGetTaggedCategory(dataCategory, categoryTag, out var taggedCategory);

            //Assert
            Assert.True(result);

            Assert.Equal(dataCategory.Heading, taggedCategory.Heading);
            Assert.Equal(dataCategory.Values, taggedCategory.Values);
            Assert.Equal(categoryTag, taggedCategory.Category);

            Assert.False(taggedCategory.HasArgument);
            Assert.Equal(0, taggedCategory.Argument);
        }
    }
}
