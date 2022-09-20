namespace MingelBingoCreator.Data
{
    internal class Category
    {
        public string Heading;
        public List<string> Values;

        public Category(string heading, List<string> values)
        {
            Heading = heading;
            Values = values;
        }
    }
}
