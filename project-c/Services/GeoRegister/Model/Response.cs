using System.Collections.Generic;
using Newtonsoft.Json;

namespace project_c.Services.GeoRegister.Model
{
    public class Response
    {
        [JsonProperty("numFound")] public int NumFound;
        [JsonProperty("start")] public int Start;
        [JsonProperty("maxScore")] public double MaxScore;
        [JsonProperty("docs")] public List<Document> Docs;
    }
}