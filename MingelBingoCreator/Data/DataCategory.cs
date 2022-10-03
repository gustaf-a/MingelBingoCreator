namespace MingelBingoCreator.Data
{
    internal class DataCategory
    {
        public string Heading;

        public List<string> Values;

        public DataCategory(string heading, List<string> values)
        {
            Heading = heading;
            Values = values;
        }
    }
}
