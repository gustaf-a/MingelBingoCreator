
using MingelBingoCreator.Data;

namespace MingelBingoCreator.FinalFileGenerator
{
    public interface IFinalFileCreator
    {
        public SpreadSheet CreateFinalFile(List<CardValue> mingelBingoCards);
    }
}
