
namespace MingelBingoCreator.ValuesGeneration
{
    internal class IgnoreValueSelector : IValueSelector
    {
        public List<string> GetValues()
        {
            return new List<string>();
        }

        public List<string> GetValues(int numberOfValues)
        {
            return new List<string>();
        }
    }
}
