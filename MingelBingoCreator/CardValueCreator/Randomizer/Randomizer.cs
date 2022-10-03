namespace MingelBingoCreator.CardValueCreator.Randomizer
{
    public class Randomizer
    {
        private readonly Random _random;

        public Randomizer()
        {
            _random = new Random();
        }

        public int GetNextRandomIndex<T>(List<T> values, List<int> valuesToAvoid)
        {
            int nextIndex;

            if (values.Count <= valuesToAvoid.Count)
            {
                for (int i = 0; i < values.Count; i++)
                    if (!valuesToAvoid.Contains(i))
                        return i;

                throw new Exception("Randomizer can't find NextRandomIndex. All possible values are to be avoided.");
            }

            do
            {
                nextIndex = _random.Next(values.Count);

            } while (valuesToAvoid.Contains(nextIndex));

            return nextIndex;
        }
    }
}
