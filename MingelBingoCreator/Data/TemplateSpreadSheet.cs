namespace MingelBingoCreator.Data
{
    public class TemplateSpreadSheet : SpreadSheet
    {
        public int TemplateSheetId { get; set; }
        public List<string> A1PlaceHolderNotations { get; set; }
    }
}
