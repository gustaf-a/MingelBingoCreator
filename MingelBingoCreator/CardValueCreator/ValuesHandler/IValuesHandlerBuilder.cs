using MingelBingoCreator.CardValueCreator.SelectorBehaviors;

namespace MingelBingoCreator.CardValueCreator.ValuesHandler
{
    internal interface IValuesHandlerBuilder
    {
        public void NewBuild();

        public IValuesHandler GetResult();

        public void SetValues(List<string> values);

        public void SetSelectorBehavior(ISelectorBehavior selectorBehavior);
    }
}
