using Newtonsoft.Json;

namespace project_c.Models
{
    public class ImageOrderModel
    {
        [JsonProperty("alreadyExists")] public bool AlreadyExists { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
    }
}