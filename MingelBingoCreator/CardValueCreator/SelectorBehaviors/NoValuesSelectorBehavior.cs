namespace MingelBingoCreator.CardValueCreator.SelectorBehaviors
{
    public class NoValuesSelectorBehavior : ISelectorBehavior
    {
        public List<T> GetValues<T>(List<T> values, int quantity)
            => new();
    }
}
