using MingelBingoCreator.Data;

namespace MingelBingoCreator.ValuesGeneration
{
    internal class ValueSelectorFactory
    {
        private readonly SelectorMethod _selectorMethod;

        public enum SelectorMethod
        {
            Random,
            Tagged
        }

        public ValueSelectorFactory(SelectorMethod selectorMethod)
        {
            _selectorMethod = selectorMethod;
        }

        public IValueSelector Build(MingelBingoData mingelBingoData)
            => _selectorMethod switch
            {
                SelectorMethod.Random 
                    => new RandomValueSelector(mingelBingoData.CellsInEachBoard, mingelBingoData.RawDataCategories),

                SelectorMethod.Tagged 
                    => new TaggedCategoriesValueSelector(mingelBingoData.CellsInEachBoard, mingelBingoData.RawDataCategories),

                _ => throw new NotImplementedException($"Factory key not implemented: {_selectorMethod}"),
            };
    }
}
