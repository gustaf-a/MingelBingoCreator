using MingelBingoCreator.CardValueCreator.ValuesHandler;
using MingelBingoCreator.Data;

namespace MingelBingoCreator.CardValueCreator.ValuesHandlerSelector
{
    public interface IValuesHandlerSelector
    {
        public List<IValuesHandler> GetValuesHandlers(MingelBingoData data);
    }
}
