
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{
    /// <summary>
    /// IMPORTANT: <see cref="JsonPropertyAttribute"/> dependency is only used for Cosmos DB SDK.
    /// In AspNetCore dotnetcore 3.1 System.Text.Json.Serialization is the serializer and references the property names or <see cref=" System.Text.Json.Serialization.JsonPropertyNameAttribute"/>.
    /// </summary>
    public class PocoSample
    {
        [JsonProperty("id")]
        [Key]
        public string ClientId { get; set; }

        [JsonProperty("pk")]
        public string PartitionKey { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AccountState State { get; set; }

        public string ResourceType { get; set; }
    }

    public enum AccountState
    {
        Activated = 0,
        Locked = 1,
        Deleted = 2
    }
}
