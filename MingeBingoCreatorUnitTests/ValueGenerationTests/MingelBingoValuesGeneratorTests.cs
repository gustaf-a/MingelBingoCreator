using MingelBingoCreator.ValuesGeneration;
using Moq;
using Xunit;

namespace MingeBingoCreatorUnitTests.ValueGenerationTests
{
    public static class MingelBingoValuesGeneratorTests
    {
        [Fact]
        public static void MingelBingoGenerator_CreateBingoCards_creates_correct_number_of_BingoCards()
        {
            //Arrange
            var valueSelectorMock = new Mock<IValueSelector>();
            valueSelectorMock
                .Setup(v => v.GetValues())
                .Returns(new List<string> { "test 1", "test 2", "test 3", "test 4" });

            var mingelBingoGenerator = new MingelBingoValuesGenerator(valueSelectorMock.Object);

            //Act
            var result = mingelBingoGenerator.GetValues(5);

            //Assert
            Assert.Equal(5, result.Count);
        }
    }
}
