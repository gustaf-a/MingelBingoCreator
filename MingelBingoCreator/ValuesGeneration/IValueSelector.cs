namespace MingelBingoCreator.ValuesGeneration
{
    internal interface IValueSelector
    {
        public List<string> GetValues();
        public List<string> GetValues(int numberOfValues);
    }
}
