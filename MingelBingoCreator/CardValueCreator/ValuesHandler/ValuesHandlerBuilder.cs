using MingelBingoCreator.CardValueCreator.SelectorBehaviors;

namespace MingelBingoCreator.CardValueCreator.ValuesHandler
{
    public class ValuesHandlerBuilder : IValuesHandlerBuilder
    {
        private ValuesHandler _valuesHandler;

        public ValuesHandlerBuilder()
        {
            NewBuild();
        }

        public IValuesHandler GetResult()
        {
            return _valuesHandler;
        }

        public void NewBuild()
        {
            _valuesHandler = new ValuesHandler();
        }

        public void SetArgument(int argument)
        {
            _valuesHandler.HasArgument = true;

            _valuesHandler.Argument = argument;
        }

        public void SetSelectorBehavior(ISelectorBehavior selectorBehavior)
        {
            _valuesHandler.SelectorBehavior = selectorBehavior;
        }

        public void SetValues(List<string> values)
        {
            _valuesHandler.Values ??= new();

            _valuesHandler.Values.AddRange(values);
        }
    }
}
