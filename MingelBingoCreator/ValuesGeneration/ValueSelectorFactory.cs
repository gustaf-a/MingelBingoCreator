using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    internal class ValueSelectorFactory
    {
        private readonly SelectorMethod _selectorMethod;

        public enum SelectorMethod
        {
            Random
        }

        public ValueSelectorFactory(SelectorMethod selectorMethod)
        {
            _selectorMethod = selectorMethod;
        }

        public IValueSelector Build(MingelBingoData mingelBingoData)
        {
            switch (_selectorMethod)
            {
                case SelectorMethod.Random:
                    return new RandomValueSelector(mingelBingoData);

                default:
                    throw new NotImplementedException($"Factory key not implemented: {_selectorMethod}");
                    
            }
        }
    }
}
