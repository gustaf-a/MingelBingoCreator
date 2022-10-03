
namespace MingelBingoCreator.Data
{
    public class MingelBingoData
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
