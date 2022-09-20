using MingelBingoCreator.Data;
using MingelBingoCreator.MingelBingoCreator;
using MingelBingoCreator.ValueSelector;
using Moq;
using Xunit;

namespace MingeBingoCreatorUnitTests.MingelBingoCreatorTests
{
    public static class MingelBingoGeneratorTests
    {
        [Fact]
        public static void MingelBingoGenerator_CreateBingoCards_creates_correct_number_of_BingoCards()
        {
            //Arrange
            var valueSelectorMock = new Mock<IValueSelector>();
            valueSelectorMock
                .Setup(v => v.GetValues())
                .Returns(new List<string> { "test 1", "test 2", "test 3", "test 4" });

            var mingelBingoGenerator = new MingelBingoGenerator(valueSelectorMock.Object);

            //Act
            var result = mingelBingoGenerator.GetBingoCards(5);

            //Assert
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public static void MingelBingoGenerator_2()
        {
            //Arrange


            //Act


            //Assert


        }

        //TODO Clean out?
        //private static List<Category> GetRawTestData()
        //{
        //    return new List<Category>
        //    {
        //        new Category("heading_1", new List<string>{"h1_v1", "h1_v2", "h1_v3"}),
        //        new Category("heading_1", new List<string>{"h2_v1", "h2_v2", "h2_v3", "h2_v4"}),
        //        new Category("heading_1", new List<string>{"h3_v1", "h3_v2"})
        //    };
        //}
    }
}
