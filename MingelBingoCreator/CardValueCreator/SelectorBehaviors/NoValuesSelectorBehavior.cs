namespace MingelBingoCreator.CardValueCreator.SelectorBehaviors
{
    internal class NoValuesSelectorBehavior : ISelectorBehavior
    {
        public List<T> GetValues<T>(List<T> values, int quantity)
            => new();
    }
}
