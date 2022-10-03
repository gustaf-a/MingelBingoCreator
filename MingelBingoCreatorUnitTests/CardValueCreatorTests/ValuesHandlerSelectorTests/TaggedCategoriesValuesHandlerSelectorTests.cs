using MingelBingoCreator.CardValueCreator.ValuesHandlerSelector;
using MingelBingoCreator.Data;
using Moq;
using Xunit;

namespace MingelBingoCreatorUnitTests.CardValueCreatorTests.ValuesHandlerSelectorTests
{
    public static class TaggedCategoriesValuesHandlerSelectorTests
    {
        [Fact]
        public static void TaggedCategoriesValuesHandlerSelector_returns_correct_valuesHandlers()
        {
            //Arrange
            var dataCategories = new List<DataCategory>
            {
                new DataCategory(
                    "Filler values",
                    new(){"filler"}
                ),
                new DataCategory(
                    "Ignore values #Ignore",
                    new(){"ignore"}
                ),
                new DataCategory(
                    "Unique values #UniquePerBoard_1",
                    new(){"unique"}
                ),
                new DataCategory(
                    "Each board values #OnEachBoard_1",
                    new(){"on each board"}
                )
            };   

            var taggedCategoryIdentifier = new TaggedCategoryIdentifier();

            var taggedCategoriesValuesHandlerSelector = new TaggedCategoriesValuesHandlerSelector(taggedCategoryIdentifier);

            var data = new MingelBingoData(dataCategories, 16);

            //Act
            var valuesHandlers = taggedCategoriesValuesHandlerSelector.GetValuesHandlers(data);

            //Assert
            Assert.Equal(4, valuesHandlers.Count);

            Assert.Equal(0, valuesHandlers[0].GetValues(1).Count);

            Assert.Equal("unique", valuesHandlers[1].GetValues(1)[0]);
            Assert.Equal("on each board", valuesHandlers[2].GetValues(1)[0]);
            Assert.Equal("filler", valuesHandlers[3].GetValues(1)[0]);
        }
    }
}
