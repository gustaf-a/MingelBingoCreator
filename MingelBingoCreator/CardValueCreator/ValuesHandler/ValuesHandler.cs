using MingelBingoCreator.CardValueCreator.SelectorBehaviors;

namespace MingelBingoCreator.CardValueCreator.ValuesHandler
{
    internal class ValuesHandler : IValuesHandler
    {
        public List<string> Values;

        public bool HasArgument;
        public int Argument;

        public ISelectorBehavior SelectorBehavior;

        public List<string> GetValues(int MaxValues)
        {
            return SelectorBehavior.GetValues(Values, HasArgument ? Argument : MaxValues);
        }
    }
}
