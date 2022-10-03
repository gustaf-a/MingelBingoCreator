namespace MingelBingoCreator.Data
{
    internal class CardValue
    {
        public List<string> Values;

        public List<A1Notation> A1NotationRanges;
        
        public int CardSize 
            => Values != null ? Values.Count : 0;

        public CardValue(List<string> values)
        {
            Values = values;

            A1NotationRanges = new();
        }
    }
}
