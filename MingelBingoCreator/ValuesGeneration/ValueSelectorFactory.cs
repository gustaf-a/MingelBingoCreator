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

        public IValueSelector Build(List<Category> categories)
            => _selectorMethod switch
            {
                SelectorMethod.Random 
                    => new RandomValueSelector(categories),

                SelectorMethod.Tagged 
                    => new TaggedCategoriesValueSelector(categories),

                _ => throw new NotImplementedException($"Factory key not implemented: {_selectorMethod}"),
            };
    }
}
