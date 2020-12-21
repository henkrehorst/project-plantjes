using System;
using System.Globalization;
using Newtonsoft.Json;

namespace project_c.Services.GeoRegister.Model
{
    public class Document
    {
        [JsonProperty("bron")] public string Bron;
        [JsonProperty("woonplaatscode")] public string Woonplaatscode;
        [JsonProperty("woonplaatsnaam")] public string Woonplaatsnaam;
        [JsonProperty("type")] public string Type;
        [JsonProperty("openbareruimtetype")] public string Openbareruimtetype;
        [JsonProperty("weergavenaam")] public string Weergavenaam;
        [JsonProperty("straatnaam_verkort")] public string StraatnaamVerkort;
        [JsonProperty("postcode")] public string Postcode;
        [JsonProperty("straatnaam")] public string Straatnaam;
        [JsonProperty("centroide_ll")] public string CentroideLl;
        [JsonProperty("centroide_rd")] public string CentroideRd;

        public double Longitude
        {
            get
            {
                if (CentroideLl != null)
                    return Convert.ToDouble(
                        CentroideLl.Split(" ")[0].Replace("POINT(", ""),
                        CultureInfo.InvariantCulture);
                return 0.0;
            }
        }

        public double Latitude
        {
            get
            {
                if (CentroideLl != null)
                    return Convert.ToDouble(
                        CentroideLl.Split(" ")[1].Replace(")", ""),
                        CultureInfo.InvariantCulture);
                return 0.0;
            }
        }
    }
}