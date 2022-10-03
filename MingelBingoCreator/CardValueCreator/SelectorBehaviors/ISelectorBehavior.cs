namespace MingelBingoCreator.CardValueCreator.SelectorBehaviors
{
    internal interface ISelectorBehavior
    {
        public List<T> GetValues<T>(List<T> values, int quantity);
    }
}
