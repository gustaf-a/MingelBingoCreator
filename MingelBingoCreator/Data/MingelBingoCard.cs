namespace MingelBingoCreator.Data
{
    internal class MingelBingoCard
    {
        public List<string> Values;

        public List<A1Notation> A1NotationRanges;
        
        public int CardSize 
            => Values != null ? Values.Count : 0;

        public MingelBingoCard(List<string> values)
        {
            Values = values;

            A1NotationRanges = new();
        }
    }
}
