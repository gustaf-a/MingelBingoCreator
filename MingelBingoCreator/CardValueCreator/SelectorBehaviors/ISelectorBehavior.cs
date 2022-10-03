namespace MingelBingoCreator.CardValueCreator.SelectorBehaviors
{
    public interface ISelectorBehavior
    {
        public List<T> GetValues<T>(List<T> values, int quantity);
    }
}
