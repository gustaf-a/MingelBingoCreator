namespace MingelBingoCreator.CardValueCreator.SelectorBehaviors
{
    public class RepeatingValuesSelectorBehavior : ISelectorBehavior
    {
        private List<int> _selectedIndices;

        private readonly Randomizer.Randomizer _randomizer;

        public RepeatingValuesSelectorBehavior()
        {
            _randomizer = new Randomizer.Randomizer();

            _selectedIndices = new();
        }

        public List<T> GetValues<T>(List<T> values, int quantity)
        {
            var result = new List<T>();

            for (int i = 0; i < quantity; i++)
            {
                if (_selectedIndices.Count >= values.Count)
                    _selectedIndices = new();

                var nextIndex = _randomizer.GetNextRandomIndex(values, _selectedIndices);

                _selectedIndices.Add(nextIndex);

                result.Add(values[nextIndex]);
            }

            return result;
        }
    }
}
