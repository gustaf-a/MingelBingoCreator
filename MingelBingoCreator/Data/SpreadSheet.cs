namespace MingelBingoCreator.Data
{
    internal class SpreadSheet
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<int> SheetIds { get; set; }
        public List<string> SheetNames { get; set; }
    }
}
