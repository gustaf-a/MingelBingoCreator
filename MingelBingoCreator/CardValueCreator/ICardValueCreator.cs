using MingelBingoCreator.Data;

namespace MingelBingoCreator.CardValueCreator
{
    public interface ICardValueCreator
    {
        public List<CardValue> CreateCardValues(MingelBingoData data);
    }
}