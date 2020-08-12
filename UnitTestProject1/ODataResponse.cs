
namespace UnitTestProject1
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class ODataResponse<T>
    {
        [JsonPropertyName("@odata.context")]
        public string ODataContext { get; set; }

        [JsonPropertyName("value")]
        public List<T> Value { get; set; }
    }
}
