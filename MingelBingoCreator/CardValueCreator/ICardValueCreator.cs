using MingelBingoCreator.Data;

namespace MingelBingoCreator.CardValueCreator
{
    internal interface ICardValueCreator
    {
        public List<CardValue> CreateCardValues(MingelBingoData data);
    }
}