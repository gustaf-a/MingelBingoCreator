
namespace MingelBingoCreator.Data
{
    internal class MingelBingoData
    {
        public List<Category> RawDataCategories { get; private set; }
        public int CellsInEachBoard { get; private set; }

        public MingelBingoData(List<Category> rawDataCategories, int cellsInEachBoard)
        {
            RawDataCategories = rawDataCategories;

            CellsInEachBoard = cellsInEachBoard;
        }
    }
}
