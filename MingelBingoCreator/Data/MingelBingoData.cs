
namespace MingelBingoCreator.Data
{
    internal class MingelBingoData
    {
        public List<DataCategory> RawDataCategories { get; private set; }
        public int CellsInEachBoard { get; private set; }

        public MingelBingoData(List<DataCategory> rawDataCategories, int cellsInEachBoard)
        {
            RawDataCategories = rawDataCategories;

            CellsInEachBoard = cellsInEachBoard;
        }
    }
}
