using Newtonsoft.Json;

namespace MingelBingoCreator.Configurations
{
    public sealed class GoogleSheetsOptions
    {
        [JsonProperty(Required = Required.Always)]
        public string ApplicationName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DataSheetId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DataSheetTabName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string PlaceHolderValue { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string TemplateSheetId { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public string TemplateSheetTabName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string CredentialsFileName { get; set; }
    }
}
