namespace MingelBingoCreator.Data
{
    internal class A1Notation
    {
        public string A1NotationRange { get; set; }
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
        public string SheetName { get; set; }

        public string GetA1NotationString()
            => GetA1NotationString(SheetName);

        public string GetA1NotationString(string sheetName)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
                throw new ArgumentException("SheetName required but was empty.");

            return $"{sheetName}!{A1NotationRange}";
        }
    }
}
